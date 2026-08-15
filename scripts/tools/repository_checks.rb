#!/usr/bin/env ruby
# frozen_string_literal: true

require "json"
require "open3"
require "pathname"
require "psych"

ROOT = Pathname.new(__dir__).join("../..").realpath
errors = []

def repository_files
  tracked = Open3.capture2("git", "-C", ROOT.to_s, "ls-files", "-z").first.split("\0")
  untracked = Open3.capture2("git", "-C", ROOT.to_s, "ls-files", "--others", "--exclude-standard", "-z").first.split("\0")
  (tracked + untracked).uniq.sort
end

files = repository_files

files.grep(/\.md\z/).each do |relative|
  path = ROOT.join(relative)
  File.read(path).scan(/\[[^\]]*\]\(([^)]+)\)/).flatten.each do |raw_target|
    target = raw_target.strip.sub(/\A<(.+)>\z/, "\\1")
    next if target.empty? || target.start_with?("#", "http://", "https://", "mailto:")

    target = target.split("#", 2).first
    resolved = path.dirname.join(target).cleanpath
    errors << "MD001 broken relative link #{relative} -> #{target}" unless resolved.exist?
  end
end

files.grep(/\.(json|asmdef)\z/).each do |relative|
  JSON.parse(File.read(ROOT.join(relative)))
rescue JSON::ParserError => e
  errors << "CFG001 invalid JSON #{relative}: #{e.message.lines.first.strip}"
end

manifest_path = ROOT.join("Packages/manifest.json")
if manifest_path.exist?
  JSON.parse(File.read(manifest_path)).fetch("dependencies", {}).each do |name, version|
    if version.match?(/[\*\^~><=| ]/) && !version.start_with?("file:", "git+")
      errors << "PKG001 dynamic package version #{name}=#{version}"
    end
  end
end

workflow_files = files.grep(%r{\A\.github/workflows/.+\.ya?ml\z})
workflow_files.each do |relative|
  content = File.read(ROOT.join(relative))
  Psych.safe_load(content, aliases: false)
  errors << "CI001 pull_request_target is forbidden in #{relative}" if content.include?("pull_request_target")
  content.scan(/uses:\s*([^\s#]+)/).flatten.each do |action|
    errors << "CI002 action must be pinned to a full commit SHA: #{action}" unless action.match?(%r{\Aactions/[A-Za-z0-9_.-]+@[0-9a-f]{40}\z})
  end
  errors << "CI003 workflow must declare read-only contents permission: #{relative}" unless content.match?(/^permissions:\s*\n\s+contents:\s*read\s*$/)
  errors << "CI004 write permissions are forbidden: #{relative}" if content.match?(/^\s+[a-z-]+:\s*write\s*$/)
end

secret_patterns = {
  "private key" => /-----BEGIN (?:RSA |EC |OPENSSH )?PRIVATE KEY-----/,
  "GitHub token" => /\b(?:ghp|github_pat)_[A-Za-z0-9_]{20,}\b/,
  "Google API key" => /\bAIza[0-9A-Za-z_-]{30,}\b/,
  "Slack token" => /\bxox[baprs]-[0-9A-Za-z-]{20,}\b/
}
files.each do |relative|
  path = ROOT.join(relative)
  next unless path.file? && path.size < 2_000_000
  content = File.binread(path)
  next if content.include?("\0")

  secret_patterns.each do |name, pattern|
    errors << "SEC001 possible #{name} in #{relative}" if content.match?(pattern)
  end
end

unless system("git", "-C", ROOT.to_s, "check-ignore", "-q", "artifacts/probe")
  errors << "REPO001 artifacts/ must be ignored"
end

if errors.empty?
  puts "PE_REPOSITORY_CHECKS_OK markdown=#{files.grep(/\.md\z/).length} json=#{files.grep(/\.(json|asmdef)\z/).length} workflows=#{workflow_files.length} secrets=0"
else
  warn errors.join("\n")
  exit 1
end
