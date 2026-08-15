#!/usr/bin/env bash

set -euo pipefail

SCRIPT_LIB_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(cd "${SCRIPT_LIB_DIR}/../.." && pwd)"
ARTIFACTS_DIR="${PROJECT_ROOT}/artifacts"

find_unity_editor() {
  if [[ -n "${UNITY_EDITOR:-}" && -x "${UNITY_EDITOR}" ]]; then
    printf '%s\n' "${UNITY_EDITOR}"
    return 0
  fi

  local pinned_version
  pinned_version="$(awk '/^m_EditorVersion:/{print $2}' "${PROJECT_ROOT}/ProjectSettings/ProjectVersion.txt")"
  local candidates=(
    "/Applications/Unity/Hub/Editor/${pinned_version}/Unity.app/Contents/MacOS/Unity"
    "${HOME}/Unity/Hub/Editor/${pinned_version}/Editor/Unity"
    "/opt/unity/editors/${pinned_version}/Editor/Unity"
  )
  local candidate
  for candidate in "${candidates[@]}"; do
    if [[ -x "${candidate}" ]]; then
      printf '%s\n' "${candidate}"
      return 0
    fi
  done

  if command -v unity-editor >/dev/null 2>&1; then
    command -v unity-editor
    return 0
  fi

  printf 'Unity %s not found. Set UNITY_EDITOR to its executable.\n' "${pinned_version}" >&2
  return 127
}

prepare_artifacts() {
  mkdir -p "${ARTIFACTS_DIR}/logs" "${ARTIFACTS_DIR}/reports" \
    "${ARTIFACTS_DIR}/test-results" "${ARTIFACTS_DIR}/builds"
}

sanitize_log() {
  local source="$1"
  local destination="$2"
  TASK_PROJECT_ROOT="${PROJECT_ROOT}" TASK_HOME="${HOME}" TASK_UNITY_EDITOR="${UNITY_EDITOR_PATH}" \
    ruby -pe 'gsub(ENV.fetch("TASK_UNITY_EDITOR"), "{UNITY_EDITOR}"); gsub(ENV.fetch("TASK_PROJECT_ROOT"), "{PROJECT_ROOT}"); gsub(ENV.fetch("TASK_HOME"), "{USER_HOME}")' \
    "${source}" > "${destination}"
}

run_unity() {
  local label="$1"
  shift
  prepare_artifacts
  UNITY_EDITOR_PATH="$(find_unity_editor)"
  local raw_log="${ARTIFACTS_DIR}/logs/.${label}.raw.log"
  local final_log="${ARTIFACTS_DIR}/logs/${label}.log"
  local git_commit
  git_commit="$(git -C "${PROJECT_ROOT}" rev-parse HEAD 2>/dev/null || printf 'unknown')"

  set +e
  "${UNITY_EDITOR_PATH}" -batchmode -nographics \
    -projectPath "${PROJECT_ROOT}" -logFile "${raw_log}" \
    -artifactsPath "${ARTIFACTS_DIR}" -gitCommit "${git_commit}" "$@"
  local exit_code=$?
  set -e

  if [[ -f "${raw_log}" ]]; then
    sanitize_log "${raw_log}" "${final_log}"
    rm -f "${raw_log}"
  else
    printf 'Unity did not create its requested log. Exit code: %s\n' "${exit_code}" > "${final_log}"
  fi
  if [[ ${exit_code} -ne 0 ]]; then
    printf '%s failed with exit code %s. See %s\n' "${label}" "${exit_code}" "${final_log}" >&2
  else
    printf '%s succeeded. Log: %s\n' "${label}" "${final_log}"
  fi
  return "${exit_code}"
}
