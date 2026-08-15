#!/usr/bin/env ruby
# frozen_string_literal: true

require "rexml/document"

abort "usage: nunit_to_junit.rb INPUT OUTPUT" unless ARGV.length == 2
input, output = ARGV
source = REXML::Document.new(File.read(input))
run = source.root
abort "expected NUnit test-run root" unless run&.name == "test-run"

suite = REXML::Element.new("testsuite")
suite.add_attribute("name", run.attributes["name"] || "Unity")
suite.add_attribute("tests", run.attributes["total"] || "0")
suite.add_attribute("failures", run.attributes["failed"] || "0")
suite.add_attribute("errors", "0")
suite.add_attribute("skipped", run.attributes["skipped"] || "0")
suite.add_attribute("time", run.attributes["duration"] || "0")

REXML::XPath.each(source, "//test-case") do |test|
  testcase = suite.add_element("testcase")
  testcase.add_attribute("name", test.attributes["name"] || "unnamed")
  testcase.add_attribute("classname", test.attributes["classname"] || test.attributes["fullname"] || "Unity")
  testcase.add_attribute("time", test.attributes["duration"] || "0")
  case test.attributes["result"]
  when "Failed"
    failure = testcase.add_element("failure")
    failure.add_attribute("message", REXML::XPath.first(test, "./failure/message")&.text.to_s)
    failure.text = REXML::XPath.first(test, "./failure/stack-trace")&.text.to_s
  when "Skipped", "Inconclusive"
    testcase.add_element("skipped")
  end
end

document = REXML::Document.new
document << REXML::XMLDecl.new("1.0", "UTF-8")
document.add_element(suite)
File.write(output, document.to_s + "\n")
puts "PE_JUNIT_OK path=#{output} tests=#{suite.attributes['tests']} failures=#{suite.attributes['failures']}"
