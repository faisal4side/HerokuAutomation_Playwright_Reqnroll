using Microsoft.Playwright;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading.Tasks;
using HerokuAutomation_Playwright_Reqnroll.Config;
using HerokuAutomation_Playwright_Reqnroll.Utilities;
using Reqnroll;

namespace HerokuAutomation_Playwright_Reqnroll.Hooks
{
    [Binding]
    public class PlaywrightHooks
    {
        private readonly ScenarioContext _scenarioContext;
        private IPlaywright _playwright;
        private IBrowser _browser;
        private IPage _page;

        public PlaywrightHooks(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [BeforeScenario]
        public async Task BeforeScenario()
        {
            try
            {
                // Initialize Playwright
                _playwright = await Microsoft.Playwright.Playwright.CreateAsync();
                
                // Launch browser
                _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                {
                    Headless = false,
                    SlowMo = 50
                });
                
                // Create a new page
                _page = await _browser.NewPageAsync();

                // Store page in scenario context
                _scenarioContext.Set(_page, "page");
            }
            catch (Exception ex)
            {
                TestLogger.LogError("Failed to initialize Playwright", ex);
                throw;
            }
        }

        [AfterScenario]
        public async Task AfterScenario()
        {
            try
            {
                // Capture screenshot on failure
                if (TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Failed)
                {
                    var screenshotPath = Path.Combine(TestConfiguration.ScreenshotsDirectory, 
                        $"error_{TestContext.CurrentContext.Test.Name}_{DateTime.Now:yyyyMMdd_HHmmss}.png");
                    
                    await _page.ScreenshotAsync(new PageScreenshotOptions
                    {
                        Path = screenshotPath,
                        FullPage = true
                    });
                    TestLogger.LogInfo($"Screenshot captured: {screenshotPath}");
                }

                // Clean up resources
                if (_page != null)
                {
                    await _page.CloseAsync();
                }
                
                if (_browser != null)
                {
                    await _browser.CloseAsync();
                }
                
                _playwright?.Dispose();
            }
            catch (Exception ex)
            {
                TestLogger.LogError("Failed to clean up Playwright resources", ex);
                throw;
            }
        }
    }

    [SetUpFixture]
    public class GlobalHooks
    {
        [OneTimeSetUp]
        public void BeforeAllTests()
        {
            try
            {
                // Ensure all artifact directories exist
                Directory.CreateDirectory(TestConfiguration.ScreenshotsDirectory);
                Directory.CreateDirectory(TestConfiguration.VideoDirectory);
                Directory.CreateDirectory(TestConfiguration.LogsDirectory);

                TestLogger.LogInfo($"Screenshots directory: {Path.GetFullPath(TestConfiguration.ScreenshotsDirectory)}");
                TestLogger.LogInfo($"Videos directory: {Path.GetFullPath(TestConfiguration.VideoDirectory)}");
                TestLogger.LogInfo($"Logs directory: {Path.GetFullPath(TestConfiguration.LogsDirectory)}");
                TestLogger.LogInfo("Test run started - Directories created and verified");
            }
            catch (Exception ex)
            {
                TestLogger.LogError("Failed to create artifact directories", ex);
                throw;
            }
        }

        [OneTimeTearDown]
        public void AfterAllTests()
        {
            TestLogger.LogInfo("Test run completed");
        }
    }
} 