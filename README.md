# Heroku Automation with Playwright and Reqnroll

This project demonstrates automated testing of the Heroku test application using Playwright and Reqnroll.

## Prerequisites

- .NET 8.0 SDK
- Visual Studio 2022 or Visual Studio Code
- Node.js (for Playwright installation)

## Setup Instructions

1. Clone the repository
2. Install Playwright browsers:
   ```bash
   dotnet build
   dotnet tool install --global Microsoft.Playwright.CLI
   playwright install
   ```
3. Restore NuGet packages:
   ```bash
   dotnet restore
   ```

## Project Structure

- `Pages/` - Contains page object models
- `Features/` - Contains Reqnroll feature files
- `Steps/` - Contains step definitions
- `Utilities/` - Contains helper classes
- `Config/` - Contains configuration settings
- `videos/` - Contains test execution recordings
- `screenshots/` - Contains failure screenshots
- `logs/` - Contains test execution logs and stack traces

## Running Tests

To run all tests:
```bash
dotnet test
```

To run specific feature:
```bash
dotnet test --filter "FullyQualifiedName~Login"
```

## CI/CD Integration

### GitHub Actions

The project includes a GitHub Actions workflow (`.github/workflows/run-tests.yml`) that:

1. **Build Process**:
   - Sets up .NET 8.0
   - Installs Playwright CLI
   - Installs required browsers
   - Restores NuGet packages
   - Builds the solution

2. **Test Execution**:
   - Runs all tests
   - Collects code coverage
   - Generates test reports

3. **Artifact Collection**:
   - Screenshots from failed tests
   - Video recordings of test execution
   - Detailed test logs
   - Test results and coverage reports

### Pipeline Setup

1. **Enable GitHub Actions**:
   - Push the repository to GitHub
   - GitHub Actions will automatically detect the workflow file
   - The workflow will run on every push to main and pull requests

2. **View Results**:
   - Go to the "Actions" tab in your GitHub repository
   - Click on a workflow run to see the results
   - Download artifacts from the workflow run page

### Artifact Access

After workflow execution, artifacts can be accessed from:
- GitHub Actions workflow run page
- Artifacts section of the workflow run
- Download links for:
  - Screenshots
  - Videos
  - Logs
  - Test results
  - Coverage reports

## Test Scenarios

1. Login Automation
   - Valid credentials login
   - Invalid credentials login with screenshot capture

2. Dynamic Table Handling (To be implemented)
3. JavaScript Alerts (To be implemented)
4. File Upload (To be implemented)

## Best Practices Implemented

- Page Object Model (POM)
- Explicit waits
- Screenshot capture on failure
- Video recording of test execution
- Detailed logging with stack traces
- Clean separation of concerns
- Reusable components
- Proper exception handling 