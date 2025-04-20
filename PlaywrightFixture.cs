using Microsoft.Playwright;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace HerokuAutomation_Playwright_Reqnroll
{
    public class PlaywrightFixture
    {
        protected IPlaywright Playwright { get; private set; }
        protected IBrowser Browser { get; private set; }
        protected IPage Page { get; private set; }

        [SetUp]
        public async Task SetUp()
        {
            // Initialize Playwright
            Playwright = await Microsoft.Playwright.Playwright.CreateAsync();
            
            // Launch browser
            Browser = await Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false,
                SlowMo = 50
            });
            
            // Create a new page
            Page = await Browser.NewPageAsync();
        }

        [TearDown]
        public async Task TearDown()
        {
            // Capture screenshot on failure
            if (TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Failed)
            {
                var screenshotPath = $"screenshots/failure_{TestContext.CurrentContext.Test.Name}_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                
                // Ensure directory exists
                var directory = System.IO.Path.GetDirectoryName(screenshotPath);
                if (!System.IO.Directory.Exists(directory))
                {
                    System.IO.Directory.CreateDirectory(directory);
                }
                
                await Page.ScreenshotAsync(new PageScreenshotOptions 
                { 
                    Path = screenshotPath,
                    FullPage = true
                });
                
                TestContext.AddTestAttachment(screenshotPath, "Screenshot of failed test");
            }

            // Clean up
            await Page.CloseAsync();
            await Browser.CloseAsync();
            Playwright.Dispose();
        }
    }
} 