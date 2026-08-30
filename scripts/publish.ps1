# Market Automation - Dağıtım paketi oluşturma scripti
# Kullanım: .\scripts\publish.ps1

$ErrorActionPreference = "Stop"

$root = Split-Path -Parent $PSScriptRoot

$distRoot = Join-Path $root "dist\MarketAutomation"
$rid = "win-x64"
$config = "Release"

Write-Host "Market Automation dağıtım paketi oluşturuluyor..." -ForegroundColor Cyan
Write-Host "Hedef: $distRoot"

if (Test-Path $distRoot) {
    Remove-Item $distRoot -Recurse -Force
}

$apiOut = Join-Path $distRoot "Api"
$desktopOut = Join-Path $distRoot "Desktop"

Write-Host "`n[1/3] API yayınlanıyor..."
dotnet publish (Join-Path $root "MarketAutomation2.API\MarketAutomation2.API.csproj") `
    -c $config `
    -r $rid `
    --self-contained true `
    -p:PublishReadyToRun=true `
    -o $apiOut
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host "`n[2/3] Desktop yayınlanıyor..."
dotnet publish (Join-Path $root "MarketAutomation2.Desktop\MarketAutomation2.Desktop.csproj") `
    -c $config `
    -r $rid `
    --self-contained true `
    -p:PublishReadyToRun=true `
    -o $desktopOut
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host "`n[3/3] Launcher yayınlanıyor..."
dotnet publish (Join-Path $root "MarketAutomation.Launcher\MarketAutomation.Launcher.csproj") `
    -c $config `
    -r $rid `
    --self-contained true `
    -p:PublishReadyToRun=true `
    -o $distRoot
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

$zipPath = Join-Path $root "dist\MarketAutomation-v1.0.0-win-x64.zip"
if (Test-Path $zipPath) {
    Remove-Item $zipPath -Force
}

Compress-Archive -Path $distRoot -DestinationPath $zipPath

Write-Host "`nDağıtım paketi hazır!" -ForegroundColor Green
Write-Host "  Klasör : $distRoot"
Write-Host "  Zip    : $zipPath"
Write-Host "`nSon kullanıcı MarketAutomation.exe dosyasını çalıştırmalıdır."
