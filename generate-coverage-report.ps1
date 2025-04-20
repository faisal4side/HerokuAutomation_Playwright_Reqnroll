# Install report generator if not already installed
$reportGeneratorInstalled = dotnet tool list --global | Select-String "dotnet-reportgenerator-globaltool"
if (-not $reportGeneratorInstalled) {
    Write-Host "Installing ReportGenerator tool..."
    dotnet tool install --global dotnet-reportgenerator-globaltool
}

# Install Playwright browsers
Write-Host "Installing Playwright browsers..."
dotnet build
$playwrightScript = Get-ChildItem -Path "**/playwright.ps1" -Recurse | Select-Object -First 1
if ($playwrightScript) {
    Write-Host "Found Playwright script at: $($playwrightScript.FullName)"
    & $playwrightScript.FullName install
} else {
    Write-Host "Installing Playwright CLI..."
    dotnet tool install --global Microsoft.Playwright.CLI
    playwright install
}

# Clean previous results
Write-Host "Cleaning previous test results..."
Remove-Item -Path "TestResults" -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item -Path "CoverageReport" -Recurse -Force -ErrorAction SilentlyContinue

# Create directories
New-Item -ItemType Directory -Path "TestResults" -Force | Out-Null
New-Item -ItemType Directory -Path "CoverageReport" -Force | Out-Null

# Run tests with coverage
Write-Host "Running tests with coverage..."
dotnet test --configuration Release /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:ExcludeByFile="**/Features/*.cs%2c**/Steps/*.cs" /p:CoverletOutput="./TestResults/coverage.cobertura.xml" --logger "console;verbosity=detailed"

# Find coverage file
$coverageFile = "TestResults/coverage.cobertura.xml"
if (Test-Path $coverageFile) {
    Write-Host "Found coverage file at: $coverageFile"
} else {
    Write-Host "No coverage file found at: $coverageFile"
    exit 1
}

# Generate report
Write-Host "Generating coverage report..."
reportgenerator -reports:"$coverageFile" -targetdir:"CoverageReport" -reporttypes:"Html;HtmlSummary;HtmlChart;Cobertura"

# Verify report was generated
$reportPath = Join-Path (Get-Location) "CoverageReport\index.html"
if (Test-Path $reportPath) {
    Write-Host "Report generated successfully at: $reportPath"
    Write-Host "Opening coverage report in browser..."
    Start-Process $reportPath
} else {
    Write-Host "Report not found at: $reportPath"
    Write-Host "Contents of CoverageReport directory:"
    Get-ChildItem -Path "CoverageReport"
    exit 1
} 