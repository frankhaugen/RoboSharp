#!/usr/bin/env pwsh
<#
.SYNOPSIS
  Runs the same steps as GitHub Actions CI: restore, Release build, tests (with TRX), then generator/diff checks.

.DESCRIPTION
  Sets CI=true for MSBuild parity with agents (see Directory.Build.props). On Windows, prefers Git for Windows
  bash to run tools/ci-verify.sh (same as CI); otherwise uses bash on PATH (e.g. Linux/macOS); if no suitable bash,
  runs equivalent dotnet/git commands in PowerShell.

.PARAMETER PackageVersion
  Optional MSBuild -p:Version (same as release workflow / robo-build-verify input package_version).

.EXAMPLE
  ./tools/ci-local.ps1

.EXAMPLE
  ./tools/ci-local.ps1 -PackageVersion 1.2.3
#>
param(
    [Parameter()]
    [string] $PackageVersion = ''
)

$ErrorActionPreference = 'Stop'
$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
Set-Location $repoRoot

function Find-GitBash {
    $candidates = @(
        (Join-Path $env:ProgramFiles 'Git\bin\bash.exe'),
        (Join-Path ${env:ProgramFiles(x86)} 'Git\bin\bash.exe'),
        (Join-Path $env:LocalAppData 'Programs\Git\bin\bash.exe')
    )
    foreach ($p in $candidates) {
        if ($p -and (Test-Path -LiteralPath $p)) {
            return $p
        }
    }
    return $null
}

$onWindows = [System.Runtime.InteropServices.RuntimeInformation]::IsOSPlatform(
    [System.Runtime.InteropServices.OSPlatform]::Windows)

$bashExe = $null
if ($onWindows) {
    $bashExe = Find-GitBash
}
elseif (Get-Command bash -ErrorAction SilentlyContinue) {
    $bashExe = 'bash'
}

if ($bashExe) {
    if ($PackageVersion) {
        $env:PACKAGE_VERSION = $PackageVersion
    }
    else {
        Remove-Item Env:\PACKAGE_VERSION -ErrorAction SilentlyContinue
    }
    $scriptPath = Join-Path $repoRoot 'tools/ci-verify.sh'
    if ($bashExe -eq 'bash') {
        & bash $scriptPath
    }
    else {
        & $bashExe $scriptPath
    }
    exit $LASTEXITCODE
}

$env:CI = 'true'

$resultsDir = Join-Path ([System.IO.Path]::GetTempPath()) 'robo-ci-test-results'
New-Item -ItemType Directory -Force -Path $resultsDir | Out-Null

dotnet restore RoboSharp.slnx
if ($PackageVersion) {
    dotnet build RoboSharp.slnx --configuration Release --no-restore "-p:Version=$PackageVersion"
}
else {
    dotnet build RoboSharp.slnx --configuration Release --no-restore
}

dotnet test RoboSharp.slnx `
    --configuration Release `
    --no-build `
    --verbosity normal `
    --logger "trx;LogFileName=test-results.trx" `
    --results-directory $resultsDir

dotnet run --file .githooks/GenerateDocDiagrams.cs -- $repoRoot
dotnet run --file .githooks/UpdateSlnx.cs -- $repoRoot
git diff --exit-code RoboSharp.slnx
git diff --exit-code docs/
