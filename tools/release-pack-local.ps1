#!/usr/bin/env pwsh
<#
.SYNOPSIS
  Local mirror of the release workflow after checkout: CI verify steps with a fixed version, then publish + tarballs + SHA256SUMS.

.DESCRIPTION
  Does not create a GitHub Release. Outputs under artifacts/release-staging and artifacts/release-dist.
  Matches .github/workflows/release.yml publish and package steps (RIDs linux-x64, win-x64 for Studio/Player; Web linux-x64).

.PARAMETER Version
  Semantic version without leading v (e.g. 1.2.3), same as the tag body after stripping v.

.EXAMPLE
  ./tools/release-pack-local.ps1 -Version 0.1.0
#>
param(
    [Parameter(Mandatory)]
    [string] $Version
)

$ErrorActionPreference = 'Stop'
$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path

& (Join-Path $repoRoot 'tools/ci-local.ps1') -PackageVersion $Version
if ($LASTEXITCODE -ne 0) {
    exit $LASTEXITCODE
}

$pub = Join-Path $repoRoot 'artifacts/release-staging'
$dist = Join-Path $repoRoot 'artifacts/release-dist'
if (Test-Path $pub) {
    Remove-Item -Recurse -Force $pub
}
if (Test-Path $dist) {
    Remove-Item -Recurse -Force $dist
}
New-Item -ItemType Directory -Force -Path $pub | Out-Null
New-Item -ItemType Directory -Force -Path $dist | Out-Null

foreach ($rid in @('linux-x64', 'win-x64')) {
    foreach ($name in @('Studio', 'Player')) {
        $proj = Join-Path $repoRoot "src/RoboSharp.$name/RoboSharp.$name.csproj"
        $out = Join-Path $pub "RoboSharp.$name-$rid"
        dotnet publish $proj `
            --configuration Release `
            --runtime $rid `
            "-p:Version=$Version" `
            -p:RoboSharpSingleFilePublish=true `
            -o $out
    }
}

$webOut = Join-Path $pub 'RoboSharp.Web-linux-x64'
dotnet publish (Join-Path $repoRoot 'src/RoboSharp.Web/RoboSharp.Web.csproj') `
    --configuration Release `
    --runtime linux-x64 `
    "-p:Version=$Version" `
    -o $webOut

$tag = if ($Version.StartsWith('v')) { $Version } else { "v$Version" }

Push-Location $pub
try {
    foreach ($rid in @('linux-x64', 'win-x64')) {
        foreach ($name in @('Studio', 'Player')) {
            $folder = "RoboSharp.$name-$rid"
            $tarName = Join-Path $dist "RoboSharp.$name-$tag-$rid.tar.gz"
            tar -czf $tarName $folder
        }
    }
    tar -czf (Join-Path $dist "RoboSharp.Web-$tag-linux-x64.tar.gz") 'RoboSharp.Web-linux-x64'
}
finally {
    Pop-Location
}

$sumsPath = Join-Path $dist 'SHA256SUMS.txt'
$lines = Get-ChildItem -Path $dist -Filter '*.tar.gz' | ForEach-Object {
    $hash = (Get-FileHash $_.FullName -Algorithm SHA256).Hash.ToLowerInvariant()
    "$hash  $($_.Name)"
}
$utf8 = [System.Text.UTF8Encoding]::new($false)
[System.IO.File]::WriteAllLines($sumsPath, [string[]]@($lines), $utf8)

Write-Host "Release layout written to $dist"
