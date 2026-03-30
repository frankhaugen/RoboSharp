<#
.SYNOPSIS
    Metrics for docs/documentation-todo.md: line counts per linked doc and C# counts per src project.

.DESCRIPTION
    Used by agents (including parallel Cursor subagents) to refresh or verify "Have content" and to cross-check
    "Implemented" against src/RoboSharp.* without hand-counting.

    Use -Shard S1..S5 with -Command DocMetrics so each subagent scans a disjoint slice of the checklist (see
    docs/agents/documentation-checklist/).

.PARAMETER Command
    DocMetrics — parse checklist tables, emit per-file non-empty line counts and suggested Yes/Stub/No.
    SrcMetrics — emit per-project .cs file count under src/RoboSharp.* (excludes obj/bin).
    DocGap — list docs/**/*.md under docs/ that are not linked from documentation-todo.md tables.
    All — DocMetrics + SrcMetrics (respects -Shard for the DocMetrics half).

.PARAMETER Shard
    When set with DocMetrics or All, only rows whose path belongs to that shard are emitted (disjoint sharding).

.PARAMETER Json
    Emit JSON (DocMetrics: array of row objects; SrcMetrics: array; DocGap: array of strings; All: object).

.PARAMETER RepoRoot
    Repository root. Defaults to parent of tools/.

.EXAMPLE
    pwsh -File tools/doc-checklist.ps1 -Command DocMetrics -Shard S2
    pwsh -File tools/doc-checklist.ps1 -Command DocGap
#>
[CmdletBinding()]
param(
    [ValidateSet('DocMetrics', 'SrcMetrics', 'All', 'DocGap')]
    [string] $Command = 'All',

    [ValidateSet('', 'S1', 'S2', 'S3', 'S4', 'S5')]
    [string] $Shard = '',

    [switch] $Json,

    [string] $RepoRoot = ''
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

if (-not $RepoRoot) {
    $RepoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
}

$docsDir = Join-Path $RepoRoot 'docs'
$todoPath = Join-Path $docsDir 'documentation-todo.md'

# Disjoint path ownership for parallel subagents (prefix with trailing / = folder; else exact file under docs/).
$script:ShardPrefixes = @{
    'S1' = @('README.md', 'build.md', 'repository-layout.md', 'nuget.md', 'architecture.md', 'diagrams/', 'governance/')
    'S2' = @('architecture/', 'io/', 'workspaces/')
    'S3' = @('language/', 'semantics/', 'compiler/')
    'S4' = @('runtime/', 'world/', 'rendering/', 'toolchain/')
    'S5' = @('debugger/', 'studio/')
}

function Test-RelativePathInShard {
    param(
        [string] $RelativePath,
        [string] $ShardId
    )
    if (-not $script:ShardPrefixes.ContainsKey($ShardId)) {
        return $false
    }
    foreach ($p in $script:ShardPrefixes[$ShardId]) {
        if ($p.EndsWith('/')) {
            if ($RelativePath.StartsWith($p, [System.StringComparison]::Ordinal)) {
                return $true
            }
        }
        elseif ($RelativePath -ceq $p) {
            return $true
        }
    }
    return $false
}

function Get-NonEmptyLineCount {
    param([string] $FilePath)
    if (-not (Test-Path -LiteralPath $FilePath)) {
        return $null
    }
    $n = 0
    Get-Content -LiteralPath $FilePath | ForEach-Object {
        if ($_.Trim().Length -gt 0) { $n++ }
    }
    return $n
}

function Get-SuggestedHaveContent {
    param([Nullable[int]] $NonEmptyLines)
    if ($null -eq $NonEmptyLines) { return 'No' }
    if ($NonEmptyLines -eq 0) { return 'No' }
    if ($NonEmptyLines -ge 20) { return 'Yes' }
    return 'Stub'
}

function Get-LinkedDocPathsFromTodo {
    param([string] $TodoFilePath)

    if (-not (Test-Path -LiteralPath $TodoFilePath)) {
        throw "Checklist not found: $TodoFilePath"
    }

    $set = New-Object 'System.Collections.Generic.HashSet[string]' ([StringComparer]::Ordinal)
    foreach ($line in Get-Content -LiteralPath $TodoFilePath) {
        if ($line -notmatch '^\|\s*\[([^\]]*)\]\(([^)]+)\)\s*\|') {
            continue
        }
        $relPath = $Matches[2].Trim()
        if ($relPath -match '^\s*https?://') { continue }
        if ($Matches[1].Trim() -ceq 'Document') { continue }
        [void]$set.Add($relPath)
    }
    return $set
}

function Get-DocTableRowsFromTodo {
    param(
        [string] $TodoFilePath,
        [string] $ShardId = ''
    )

    if (-not (Test-Path -LiteralPath $TodoFilePath)) {
        throw "Checklist not found: $TodoFilePath"
    }

    $rows = New-Object System.Collections.Generic.List[object]
    foreach ($line in Get-Content -LiteralPath $TodoFilePath) {
        if ($line -notmatch '^\|\s*\[([^\]]*)\]\(([^)]+)\)\s*\|\s*([^|]*)\|\s*([^|]*)\|\s*$') {
            continue
        }
        $linkText = $Matches[1].Trim()
        $relPath = $Matches[2].Trim()
        $currentHave = $Matches[3].Trim()
        $currentImpl = $Matches[4].Trim()

        if ($relPath -match '^\s*https?://') { continue }
        if ($linkText -ceq 'Document') { continue }

        if ($ShardId -and -not (Test-RelativePathInShard -RelativePath $relPath -ShardId $ShardId)) {
            continue
        }

        $fullPath = Join-Path $docsDir $relPath
        $lines = Get-NonEmptyLineCount -FilePath $fullPath
        $suggested = Get-SuggestedHaveContent -NonEmptyLines $lines

        $rows.Add([PSCustomObject]@{
                Shard        = if ($ShardId) { $ShardId } else { '' }
                RelativePath = $relPath
                LinkText     = $linkText
                NonEmptyLines = $lines
                SuggestedHaveContent = $suggested
                CurrentHaveContent   = $currentHave
                CurrentImplemented   = $currentImpl
                HaveMatches          = ($suggested -eq $currentHave)
            })
    }
    return $rows
}

function Get-SrcProjectMetrics {
    param([string] $SrcRoot)

    $list = New-Object System.Collections.Generic.List[object]
    if (-not (Test-Path -LiteralPath $SrcRoot)) {
        return $list
    }

    Get-ChildItem -LiteralPath $SrcRoot -Directory -Filter 'RoboSharp.*' | ForEach-Object {
        $projDir = $_.FullName
        $csFiles = Get-ChildItem -LiteralPath $projDir -Recurse -Filter '*.cs' -File -ErrorAction SilentlyContinue |
            Where-Object { $_.FullName -notmatch '[\\/]obj[\\/]' -and $_.FullName -notmatch '[\\/]bin[\\/]' }
        $count = if ($csFiles) { @($csFiles).Count } else { 0 }
        $list.Add([PSCustomObject]@{
                Project = $_.Name
                CsFileCount = $count
                ProjectPath = $projDir
            })
    }
    return $list
}

function Get-DocGapPaths {
    param(
        [string] $DocsDirectory,
        [string] $TodoFilePath
    )

    $linked = Get-LinkedDocPathsFromTodo -TodoFilePath $TodoFilePath
    $allMd = Get-ChildItem -LiteralPath $DocsDirectory -Recurse -Filter '*.md' -File |
        ForEach-Object {
            $rel = $_.FullName.Substring($DocsDirectory.Length).TrimStart('\', '/').Replace('\', '/')
            $rel
        }

    $gaps = New-Object System.Collections.Generic.List[string]
    foreach ($p in $allMd) {
        if (-not $linked.Contains($p)) {
            $gaps.Add($p)
        }
    }
    return ($gaps | Sort-Object)
}

function Emit-DocMetrics {
    param(
        [string] $TodoFilePath,
        [string] $DocsDir,
        [string] $ShardId,
        [switch] $AsJson
    )

    $rows = Get-DocTableRowsFromTodo -TodoFilePath $TodoFilePath -ShardId $ShardId
    $mismatches = @($rows | Where-Object { -not $_.HaveMatches })

    if ($AsJson) {
        $rows | ConvertTo-Json -Depth 4 -Compress
        return
    }

    $label = if ($ShardId) { "shard $ShardId" } else { 'all shards' }
    Write-Host "documentation-todo.md linked docs ($label): $($rows.Count) rows" -ForegroundColor Cyan
    Write-Host ("{0,-55} {1,6} {2,-8} {3,-8} {4}" -f 'Path', 'Lines', 'Suggest', 'Table', 'OK') -ForegroundColor DarkGray
    foreach ($r in $rows) {
        $ok = if ($r.HaveMatches) { 'yes' } else { 'NO' }
        $lineCol = if ($null -eq $r.NonEmptyLines) { '—' } else { $r.NonEmptyLines.ToString() }
        Write-Host ("{0,-55} {1,6} {2,-8} {3,-8} {4}" -f $r.RelativePath, $lineCol, $r.SuggestedHaveContent, $r.CurrentHaveContent, $ok)
    }

    if ($mismatches.Count -gt 0) {
        Write-Host ""
        Write-Host "Have content mismatches (suggested vs table): $($mismatches.Count)" -ForegroundColor Yellow
        foreach ($m in $mismatches) {
            Write-Host "  $($m.RelativePath): suggested $($m.SuggestedHaveContent), table $($m.CurrentHaveContent)"
        }
    }
}

function Emit-SrcMetrics {
    param(
        [string] $RepoRootPath,
        [switch] $AsJson
    )

    $srcRoot = Join-Path $RepoRootPath 'src'
    $rows = Get-SrcProjectMetrics -SrcRoot $srcRoot

    if ($AsJson) {
        , @($rows) | ConvertTo-Json -Depth 4 -Compress
        return
    }

    Write-Host "src/RoboSharp.* C# files (excluding obj/bin):" -ForegroundColor Cyan
    foreach ($r in $rows) {
        Write-Host ("  {0,-28} {1,4}" -f $r.Project, $r.CsFileCount)
    }
}

function Emit-DocGap {
    param(
        [string] $TodoFilePath,
        [string] $DocsDir,
        [switch] $AsJson
    )

    $gaps = @(Get-DocGapPaths -DocsDirectory $DocsDir -TodoFilePath $TodoFilePath)
    if ($AsJson) {
        , $gaps | ConvertTo-Json -Depth 2 -Compress
        return
    }

    Write-Host "Markdown files under docs/ not linked from documentation-todo.md tables: $($gaps.Count)" -ForegroundColor Cyan
    foreach ($g in $gaps) {
        Write-Host "  $g"
    }
}

switch ($Command) {
    'DocMetrics' {
        if ($Shard -and -not $script:ShardPrefixes.ContainsKey($Shard)) {
            throw "Invalid shard: $Shard"
        }
        Emit-DocMetrics -TodoFilePath $todoPath -DocsDir $docsDir -ShardId $Shard -AsJson:$Json
    }
    'SrcMetrics' {
        if ($Shard) {
            Write-Warning '-Shard is ignored for SrcMetrics (global snapshot).'
        }
        Emit-SrcMetrics -RepoRootPath $RepoRoot -AsJson:$Json
    }
    'DocGap' {
        if ($Shard) {
            Write-Warning '-Shard is ignored for DocGap.'
        }
        Emit-DocGap -TodoFilePath $todoPath -DocsDir $docsDir -AsJson:$Json
    }
    'All' {
        if ($Json) {
            $doc = Get-DocTableRowsFromTodo -TodoFilePath $todoPath -ShardId $Shard
            $src = @(Get-SrcProjectMetrics -SrcRoot (Join-Path $RepoRoot 'src'))
            [PSCustomObject]@{
                Shard     = $Shard
                DocRows   = @($doc)
                SrcProjects = $src
            } | ConvertTo-Json -Depth 6 -Compress
        }
        else {
            if (-not $Shard) {
                Emit-DocMetrics -TodoFilePath $todoPath -DocsDir $docsDir -ShardId '' -AsJson:$false
                Write-Host ''
                Emit-SrcMetrics -RepoRootPath $RepoRoot
            }
            else {
                Emit-DocMetrics -TodoFilePath $todoPath -DocsDir $docsDir -ShardId $Shard -AsJson:$false
                Write-Host ''
                Emit-SrcMetrics -RepoRootPath $RepoRoot
            }
        }
    }
}
