#!/usr/bin/env pwsh
<#
.SYNOPSIS
  Publishes RoboSharp.Player and/or RoboSharp.Studio as a single self-contained executable.

.DESCRIPTION
  Sets -p:RoboSharpSingleFilePublish=true (see eng/RoboSharp.SingleFile.props).
  Player is the smallest useful single-file artifact; Studio (Avalonia) is larger.

.PARAMETER Project
  Player | Studio | Both

.PARAMETER Runtime
  RID, e.g. win-x64, win-arm64, linux-x64, osx-arm64

.PARAMETER OutputDirectory
  Folder to write publish output (created if missing).

.EXAMPLE
  ./tools/publish-singlefile.ps1 -Project Player -Runtime win-x64

.EXAMPLE
  ./tools/publish-singlefile.ps1 -Project Both -Runtime linux-x64 -OutputDirectory ./dist
#>
param(
    [Parameter()]
    [ValidateSet('Player', 'Studio', 'Both')]
    [string] $Project = 'Player',

    [Parameter()]
    [string] $Runtime = 'win-x64',

    [Parameter()]
    [ValidateSet('Debug', 'Release')]
    [string] $Configuration = 'Release',

    [Parameter()]
    [string] $OutputDirectory = ''
)

$ErrorActionPreference = 'Stop'
$repoRoot = Resolve-Path (Join-Path $PSScriptRoot '..')
Set-Location $repoRoot

if ([string]::IsNullOrWhiteSpace($OutputDirectory)) {
    $OutputDirectory = Join-Path $repoRoot "artifacts/publish/$Runtime"
}

$targets = @()
switch ($Project) {
    'Player' { $targets = @('RoboSharp.Player') }
    'Studio' { $targets = @('RoboSharp.Studio') }
    'Both' { $targets = @('RoboSharp.Player', 'RoboSharp.Studio') }
}

New-Item -ItemType Directory -Force -Path $OutputDirectory | Out-Null

foreach ($name in $targets) {
    $proj = Join-Path $repoRoot "src/$name/$name.csproj"
    $out = Join-Path $OutputDirectory $name
    New-Item -ItemType Directory -Force -Path $out | Out-Null
    Write-Host "Publishing $name -> $out ($Runtime)..." -ForegroundColor Cyan
    dotnet publish $proj `
        --configuration $Configuration `
        --runtime $Runtime `
        --output $out `
        -p:RoboSharpSingleFilePublish=true `
        -p:UseAppHost=true
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
}

Write-Host "Done. Look for the executable under: $OutputDirectory" -ForegroundColor Green
