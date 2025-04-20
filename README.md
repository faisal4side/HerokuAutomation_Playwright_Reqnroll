# Heroku Automation with Playwright and Reqnroll

This project demonstrates automated testing of the Heroku web application using Playwright for browser automation and Reqnroll for BDD-style test specifications.

## Features

- **Browser Automation**: Uses Playwright for reliable and fast browser automation
- **BDD Testing**: Implements Behavior-Driven Development using Reqnroll
- **Comprehensive Reporting**:
  - Allure Reports for detailed test execution reports
  - Screenshots on test failure
  - Video recordings of test execution
  - Browser traces for debugging
  - Detailed logs for each test scenario
- **CI/CD Integration**: GitHub Actions workflow for automated testing
- **Modern Tech Stack**:
  - .NET 8.0
  - Playwright 1.41.2
  - Reqnroll 2.4.0
  - NUnit 3.14.0
  - Allure Reports 2.11.0

## Project Structure

```
HerokuAutomation_Playwright_Reqnroll/
├── Artifacts/                  # Test execution artifacts
│   ├── Logs/                   # Scenario execution logs
│   ├── Screenshots/            # Failure screenshots
│   ├── Traces/                 # Browser traces
│   ├── Videos/                 # Test execution videos
│   └── Reports/                # Allure reports
├── Config/                     # Configuration files
│   └── TestConfiguration.cs    # Test settings and paths
├── Features/                   # Reqnroll feature files
├── Hooks/                      # Test hooks and setup
│   └── PlaywrightHooks.cs      # Browser and test lifecycle management
├── Pages/                      # Page object models
├── Steps/                      # Step definitions
└── Utils/                      # Utility classes
```

## Prerequisites

- .NET 8.0 SDK
- Node.js (for Playwright installation)
- Git

## Setup

1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/HerokuAutomation_Playwright_Reqnroll.git
   cd HerokuAutomation_Playwright_Reqnroll
   ```

2. Install dependencies:
   ```bash
   dotnet restore
   dotnet build
   ```

3. Install Playwright browsers:
   ```bash
   npx playwright install
   ```

## Running Tests

### Local Execution

Run all tests:
```bash
dotnet test
```

Run specific feature:
```bash
dotnet test --filter "FullyQualifiedName~YourFeatureName"
```

### Viewing Reports

1. Generate Allure report:
   ```bash
   allure serve bin/Debug/net8.0/allure-results
   ```

2. View traces:
   ```bash
   npx playwright show-trace Artifacts/Traces/trace_your_scenario.zip
   ```

## Artifacts

The test execution generates several types of artifacts:

- **Logs**: Detailed execution logs for each scenario
- **Screenshots**: Automatic screenshots on test failure
- **Videos**: Recordings of test execution
- **Traces**: Browser traces for debugging
- **Allure Reports**: Comprehensive test execution reports

All artifacts are stored in the `Artifacts` directory at the project root.

## CI/CD

The project includes a GitHub Actions workflow that:
- Runs on pull requests and pushes to main
- Installs dependencies
- Runs tests
- Generates and publishes Allure reports
- Handles artifacts

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details. 