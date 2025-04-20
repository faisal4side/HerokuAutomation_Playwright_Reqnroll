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
        private IPlaywright _playwright;
        private IBrowser _browser;
        private IBrowserContext _context;
        private IPage _page;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false
            });
            _context = await _browser.NewContextAsync();
            _page = await _context.NewPageAsync();

            // Store the page in ScenarioContext with the correct key
            ScenarioContext.Current.Set(_page, "page");
        }

        [OneTimeTearDown]
        public async Task Teardown()
        {
            if (_page != null)
            {
                await _page.CloseAsync();
            }
            if (_context != null)
            {
                await _context.CloseAsync();
            }
            if (_browser != null)
            {
                await _browser.CloseAsync();
            }
            if (_playwright != null)
            {
                _playwright.Dispose();
            }
        }
    }

    public class PlaywrightFixture
    {
        protected IPage Page { get; private set; }

        [SetUp]
        public void Setup()
        {
            Page = ScenarioContext.Current.Get<IPage>("page");
        }

        [TearDown]
        public async Task TearDown()
        {
            if (TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Failed)
            {
                var screenshotPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "screenshots");
                Directory.CreateDirectory(screenshotPath);
                var fileName = $"{TestContext.CurrentContext.Test.Name}_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                var fullPath = Path.Combine(screenshotPath, fileName);
                await Page.ScreenshotAsync(new PageScreenshotOptions { Path = fullPath });
                TestContext.AddTestAttachment(fullPath);
            }
        }
    }
} 