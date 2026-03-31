#!/usr/bin/env bash
# Single source of truth for CI "build and test" job steps (restore, Release build, test, generated-file check).
# Used by .github/actions/robo-build-verify and runnable locally: ./tools/ci-verify.sh or tools/ci-local.ps1
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]:-$0}")" && pwd)"
if [[ -n "${GITHUB_WORKSPACE:-}" ]]; then
  REPO_ROOT="$GITHUB_WORKSPACE"
else
  REPO_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
fi
cd "$REPO_ROOT"

# Match GitHub Actions / typical CI hosts (see Directory.Build.props).
export CI=true

if [[ -n "${RUNNER_TEMP:-}" ]]; then
  RESULTS_DIR="${RUNNER_TEMP}/test-results"
else
  if [[ -n "${TEMP:-}" ]]; then
    RESULTS_DIR="${TEMP}/robo-ci-test-results"
  elif [[ -n "${TMPDIR:-}" ]]; then
    RESULTS_DIR="${TMPDIR}/robo-ci-test-results"
  else
    RESULTS_DIR="/tmp/robo-ci-test-results"
  fi
fi
mkdir -p "$RESULTS_DIR"

dotnet restore RoboSharp.slnx

if [[ -n "${PACKAGE_VERSION:-}" ]]; then
  dotnet build RoboSharp.slnx --configuration Release --no-restore -p:Version="${PACKAGE_VERSION}"
else
  dotnet build RoboSharp.slnx --configuration Release --no-restore
fi

dotnet test RoboSharp.slnx \
  --configuration Release \
  --no-build \
  --verbosity normal \
  --logger "trx;LogFileName=test-results.trx" \
  --results-directory "$RESULTS_DIR"

dotnet run --file .githooks/GenerateDocDiagrams.cs -- "$REPO_ROOT"
dotnet run --file .githooks/UpdateSlnx.cs -- "$REPO_ROOT"
git diff --exit-code RoboSharp.slnx
git diff --exit-code docs/
