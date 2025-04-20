using Microsoft.Playwright;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading.Tasks;
using Allure.Net.Commons;
using HerokuAutomation_Playwright_Reqnroll.Config;
using NUnit.Allure.Core;
using NUnit.Allure.Attributes;
using Reqnroll;
using System.Collections.Generic;

[assembly: Parallelizable(ParallelScope.Fixtures)]

namespace HerokuAutomation_Playwright_Reqnroll.Hooks
{
    [SetUpFixture]
    [AllureNUnit]
    public class GlobalHooks
    {
        [OneTimeSetUp]
        public void GlobalSetup()
        {
            Console.WriteLine($"Initializing test environment...");
            Console.WriteLine($"Artifacts will be saved to: {TestConfiguration.ArtifactsRoot}");
            AllureLifecycle.Instance.CleanupResultDirectory();
            Environment.SetEnvironmentVariable("ALLURE_CONFIG_FILE", "allureConfig.json");
        }

        [OneTimeTearDown]
        public void GlobalTeardown()
        {
            Console.WriteLine("Test execution completed. Check the Artifacts directory for logs and reports.");
        }
    }

    [Binding]
    public class PlaywrightFixture
    {
        private IPlaywright _playwright;
        private IBrowser _browser;
        private IBrowserContext _context;
        protected IPage _page;
        private readonly ScenarioContext _scenarioContext;
        private string _currentTestUuid;
        private string _scenarioName;
        private string _logFilePath;
        private string _traceFilePath;

        public PlaywrightFixture(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        private void LogToFile(string message)
        {
            try
            {
                File.AppendAllText(_logFilePath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}{Environment.NewLine}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing to log file: {ex.Message}");
            }
        }

        private async Task SaveTraceAsync()
        {
            try
            {
                if (_context != null)
                {
                    await _context.Tracing.StopAsync(new TracingStopOptions
                    {
                        Path = _traceFilePath
                    });
                    LogToFile($"Trace saved to: {_traceFilePath}");

                    // Attach trace to Allure report
                    AllureLifecycle.Instance.AddAttachment(
                        "Browser Trace",
                        "application/zip",
                        File.ReadAllBytes(_traceFilePath)
                    );
                }
            }
            catch (Exception ex)
            {
                LogToFile($"Error saving trace: {ex.Message}");
                Console.WriteLine($"Error saving trace: {ex.Message}");
            }
        }

        [BeforeScenario]
        public async Task Setup()
        {
            _scenarioName = _scenarioContext.ScenarioInfo.Title;
            _logFilePath = Path.Combine(TestConfiguration.LogsDirectory, $"scenario_{_scenarioName}_{DateTime.Now:yyyyMMdd_HHmmss}.log");
            _traceFilePath = Path.Combine(TestConfiguration.TracesDirectory, $"trace_{_scenarioName}_{DateTime.Now:yyyyMMdd_HHmmss}.zip");
            
            LogToFile($"Starting scenario: {_scenarioName}");
            LogToFile($"Browser: Chromium");
            LogToFile($"Viewport: 1920x1080");

            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false
            });

            LogToFile($"Browser version: {_browser.Version}");

            _context = await _browser.NewContextAsync(new BrowserNewContextOptions
            {
                ViewportSize = new ViewportSize { Width = 1920, Height = 1080 },
                RecordVideoDir = TestConfiguration.VideoDirectory
            });

            // Start tracing
            await _context.Tracing.StartAsync(new TracingStartOptions
            {
                Screenshots = true,
                Snapshots = true,
                Sources = true,
                Title = _scenarioName
            });

            _page = await _context.NewPageAsync();
            _scenarioContext.Set(_page, "page");

            // Initialize Allure test case
            _currentTestUuid = Guid.NewGuid().ToString();
            
            var testResult = new TestResult
            {
                uuid = _currentTestUuid,
                name = _scenarioName,
                parameters = new List<Parameter>
                {
                    new Parameter { name = "Start Time", value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") },
                    new Parameter { name = "Browser", value = "Chromium" },
                    new Parameter { name = "Browser Version", value = _browser.Version },
                    new Parameter { name = "Viewport", value = "1920x1080" }
                }
            };
            
            AllureLifecycle.Instance.StartTestCase(testResult);
            LogToFile("Scenario setup completed successfully");
        }

        [AfterScenario]
        public async Task Teardown()
        {
            try
            {
                if (_scenarioContext.TestError != null)
                {
                    LogToFile($"Test failed: {_scenarioContext.TestError.Message}");
                    LogToFile($"Stack trace: {_scenarioContext.TestError.StackTrace}");

                    // Take screenshot
                    var screenshotFileName = $"failure_{_scenarioName}_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                    var screenshotPath = Path.Combine(TestConfiguration.ScreenshotsDirectory, screenshotFileName);
                    await _page.ScreenshotAsync(new PageScreenshotOptions
                    {
                        Path = screenshotPath,
                        FullPage = true
                    });

                    LogToFile($"Screenshot saved to: {screenshotPath}");

                    // Attach screenshot to Allure report
                    AllureLifecycle.Instance.AddAttachment(
                        "Screenshot",
                        "image/png",
                        File.ReadAllBytes(screenshotPath)
                    );
                    
                    // Save error log
                    var errorDetails = $"Error: {_scenarioContext.TestError.Message}\nStack Trace: {_scenarioContext.TestError.StackTrace}";
                    File.WriteAllText(_logFilePath, errorDetails);

                    // Attach log to Allure report
                    AllureLifecycle.Instance.AddAttachment(
                        "Error Log",
                        "text/plain",
                        File.ReadAllBytes(_logFilePath)
                    );
                    
                    // Update test case with error information
                    AllureLifecycle.Instance.UpdateTestCase(_currentTestUuid, tc =>
                    {
                        tc.status = Status.failed;
                        tc.statusDetails = new StatusDetails
                        {
                            message = _scenarioContext.TestError.Message,
                            trace = _scenarioContext.TestError.StackTrace
                        };
                        tc.parameters.Add(new Parameter { name = "End Time", value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") });
                    });
                }
                else
                {
                    LogToFile("Scenario completed successfully");
                    // Update test case for successful scenario
                    AllureLifecycle.Instance.UpdateTestCase(_currentTestUuid, tc =>
                    {
                        tc.status = Status.passed;
                        tc.parameters.Add(new Parameter { name = "End Time", value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") });
                    });
                }

                // Save trace before closing context
                await SaveTraceAsync();

                if (_context != null)
                {
                    await _context.CloseAsync();
                    LogToFile("Browser context closed");
                }
            }
            catch (Exception ex)
            {
                LogToFile($"Error during teardown: {ex.Message}");
                LogToFile($"Stack trace: {ex.StackTrace}");
                Console.WriteLine($"Error during teardown: {ex.Message}");

                // Try to save trace even if there's an error
                await SaveTraceAsync();

                AllureLifecycle.Instance.UpdateTestCase(_currentTestUuid, tc =>
                {
                    tc.status = Status.broken;
                    tc.statusDetails = new StatusDetails
                    {
                        message = $"Teardown Error: {ex.Message}",
                        trace = ex.StackTrace
                    };
                });
            }
            finally
            {
                // Stop the test case
                AllureLifecycle.Instance.StopTestCase(_currentTestUuid);
                AllureLifecycle.Instance.WriteTestCase(_currentTestUuid);
                LogToFile("Test case completed and results written");

                await _browser?.CloseAsync();
                _playwright?.Dispose();
                LogToFile("Browser closed and resources disposed");
            }
        }
    }
}