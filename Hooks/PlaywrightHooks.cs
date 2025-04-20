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
            AllureLifecycle.Instance.CleanupResultDirectory();
            Environment.SetEnvironmentVariable("ALLURE_CONFIG_FILE", "allureConfig.json");
        }

        [OneTimeTearDown]
        public void GlobalTeardown()
        {
            // Allure results are automatically generated
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

        public PlaywrightFixture(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [BeforeScenario]
        public async Task Setup()
        {
            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false
            });

            _context = await _browser.NewContextAsync(new BrowserNewContextOptions
            {
                ViewportSize = new ViewportSize { Width = 1920, Height = 1080 }
            });

            _page = await _context.NewPageAsync();
            _scenarioContext.Set(_page, "page");

            // Initialize Allure test case
            _currentTestUuid = Guid.NewGuid().ToString();
            var scenarioName = _scenarioContext.ScenarioInfo.Title;
            
            var testResult = new TestResult
            {
                uuid = _currentTestUuid,
                name = scenarioName,
                parameters = new List<Parameter>
                {
                    new Parameter { name = "Start Time", value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") },
                    new Parameter { name = "Browser", value = "Chromium" },
                    new Parameter { name = "Browser Version", value = _browser.Version },
                    new Parameter { name = "Viewport", value = "1920x1080" }
                }
            };
            
            AllureLifecycle.Instance.StartTestCase(testResult);
        }

        [AfterScenario]
        public async Task Teardown()
        {
            try
            {
                if (_scenarioContext.TestError != null)
                {
                    var screenshotPath = Path.Combine(
                        TestConfiguration.ScreenshotsDirectory,
                        $"failure_{DateTime.Now:yyyyMMdd_HHmmss}.png"
                    );

                    await _page.ScreenshotAsync(new PageScreenshotOptions
                    {
                        Path = screenshotPath,
                        FullPage = true
                    });

                    // Attach screenshot to Allure report
                    AllureLifecycle.Instance.AddAttachment(
                        "Screenshot",
                        "image/png",
                        File.ReadAllBytes(screenshotPath)
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
                    // Update test case for successful scenario
                    AllureLifecycle.Instance.UpdateTestCase(_currentTestUuid, tc =>
                    {
                        tc.status = Status.passed;
                        tc.parameters.Add(new Parameter { name = "End Time", value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") });
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during teardown: {ex.Message}");
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

                await _context?.CloseAsync();
                await _browser?.CloseAsync();
                _playwright?.Dispose();
            }
        }
    }
}