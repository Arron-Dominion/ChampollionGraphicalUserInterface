param(
    [Parameter(Mandatory = $false)]
    [string]$Version = "2.0.0"
)

$ErrorActionPreference = "Stop"

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$projectPath = Join-Path $repositoryRoot "src\ChampollionGraphicalUserInterface\ChampollionGraphicalUserInterface.csproj"
$publishDirectory = Join-Path $repositoryRoot "artifacts\publish\win-x64"
$packageDirectory = Join-Path $repositoryRoot "artifacts\packages"
$archiveName = "ChampollionGraphicalUserInterface-$Version-win-x64-portable.zip"
$archivePath = Join-Path $packageDirectory $archiveName

Remove-Item $publishDirectory -Recurse -Force -ErrorAction SilentlyContinue
New-Item $publishDirectory -ItemType Directory -Force | Out-Null
New-Item $packageDirectory -ItemType Directory -Force | Out-Null

dotnet publish $projectPath `
    -c Release `
    -r win-x64 `
    --self-contained true `
    -p:PublishProfile=win-x64 `
    -p:Version=$Version `
    -o $publishDirectory

if ($LASTEXITCODE -ne 0) {
    throw "Windows publish failed."
}

$bundledChampollion = Get-ChildItem $publishDirectory -Recurse -File |
    Where-Object { $_.Name -ieq "Champollion.exe" }
if ($bundledChampollion) {
    throw "Packaging stopped because a third-party Champollion executable was found in the publish output."
}

Remove-Item $archivePath -Force -ErrorAction SilentlyContinue
Compress-Archive -Path (Join-Path $publishDirectory "*") -DestinationPath $archivePath

$innoCompilerPath = Get-Command ISCC.exe -ErrorAction SilentlyContinue |
    Select-Object -ExpandProperty Source -First 1
if ($null -eq $innoCompilerPath) {
    $innoCompilerPath = @(
        "$env:ProgramFiles\Inno Setup 7\ISCC.exe"
        "${env:ProgramFiles(x86)}\Inno Setup 7\ISCC.exe"
        "$env:ProgramFiles\Inno Setup 6\ISCC.exe"
        "${env:ProgramFiles(x86)}\Inno Setup 6\ISCC.exe"
    ) | Where-Object { Test-Path $_ } | Select-Object -First 1
}

if ($null -ne $innoCompilerPath) {
    & $innoCompilerPath `
        "/DMyAppVersion=$Version" `
        "/DPublishDir=$publishDirectory" `
        "/DOutputDir=$packageDirectory" `
        (Join-Path $repositoryRoot "packaging\windows\setup.iss")
    if ($LASTEXITCODE -ne 0) {
        throw "Inno Setup compilation failed."
    }
}
else {
    Write-Warning "ISCC.exe was not found; the portable ZIP was created without an installer."
}

Get-ChildItem $packageDirectory -File |
    Where-Object { $_.Name -like "ChampollionGraphicalUserInterface-$Version-win-x64-*" -and $_.Extension -ne ".sha256" } |
    ForEach-Object {
        $hash = (Get-FileHash $_.FullName -Algorithm SHA256).Hash.ToLowerInvariant()
        "$hash  $($_.Name)" | Set-Content "$($_.FullName).sha256" -Encoding ascii
    }
