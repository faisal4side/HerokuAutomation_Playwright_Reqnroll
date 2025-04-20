# Playwright Test Automation with NUnit

This project demonstrates automated testing of web applications using Playwright with NUnit in C#. It includes examples of table handling, data verification, and other common web testing scenarios.

## Features

- Table data extraction and verification
- Comprehensive test scenarios using NUnit
- Page Object Model implementation
- Screenshot and video capture on test failure
- Detailed logging
- GitHub Actions CI/CD integration

## Prerequisites

- .NET 8.0 SDK
- Visual Studio 2022 or later (recommended)
- PowerShell 7.0 or later

## Getting Started

1. Clone the repository:
   ```bash
   git clone [your-repository-url]
   ```

2. Install dependencies:
   ```bash
   dotnet restore
   ```

3. Install Playwright browsers:
   ```bash
   pwsh bin/Debug/net8.0/playwright.ps1 install --with-deps
   ```

## Project Structure

- `Pages/`: Page Object Models
- `Steps/`: Step definitions for test scenarios
- `Features/`: Test scenarios in Gherkin syntax
- `TestData/`: Test data files
- `Hooks/`: Test setup and teardown logic

## Running Tests

### Via Command Line

```bash
dotnet test
```

### Via Visual Studio

1. Open the solution in Visual Studio
2. Open Test Explorer
3. Run desired tests

## Test Artifacts

The following artifacts are generated during test execution:

- Screenshots: Captured on test failure
- Videos: Recorded for failed test scenarios
- Logs: Detailed test execution logs

## CI/CD

The project uses GitHub Actions for continuous integration. The workflow:

1. Builds the project
2. Runs all tests
3. Captures test results and artifacts
4. Uploads results to GitHub Actions

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details 