using Microsoft.Playwright;
using Reqnroll;
using System;
using System.IO;
using System.Threading.Tasks;
using HerokuAutomation_Playwright_Reqnroll.Config;
using HerokuAutomation_Playwright_Reqnroll.Utilities;

namespace HerokuAutomation_Playwright_Reqnroll.Steps
{
    [Binding]
    public class Hooks
    {
        private IPlaywright _playwright;
        private IBrowser _browser;
        private IBrowserContext _context;
        private IPage _page;
        private string _videoPath;

        [BeforeTestRun]
        public static void BeforeTestRun()
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

        [BeforeScenario]
        public async Task BeforeScenario(ScenarioContext scenarioContext)
        {
            TestLogger.LogInfo($"Starting scenario: {scenarioContext.ScenarioInfo.Title}");

            try
            {
                _playwright = await Playwright.CreateAsync();
                TestLogger.LogInfo("Playwright instance created");

                _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                {
                    Headless = false,
                    SlowMo = 50
                });
                TestLogger.LogInfo("Browser launched");

                // Configure browser context with video recording
                var videoPath = Path.Combine(TestConfiguration.VideoDirectory, 
                    $"{scenarioContext.ScenarioInfo.Title}_{DateTime.Now:yyyyMMdd_HHmmss}.webm");
                
                TestLogger.LogInfo($"Video will be saved to: {videoPath}");

                _context = await _browser.NewContextAsync(new BrowserNewContextOptions
                {
                    RecordVideoDir = TestConfiguration.VideoDirectory,
                    RecordVideoSize = new RecordVideoSize 
                    { 
                        Width = TestConfiguration.RecordingOptions.Size.Width,
                        Height = TestConfiguration.RecordingOptions.Size.Height
                    }
                });
                TestLogger.LogInfo("Browser context created with video recording enabled");

                _page = await _context.NewPageAsync();
                scenarioContext.Add("page", _page);
                scenarioContext.Add("videoPath", videoPath);
                
                TestLogger.LogInfo("Page created and added to scenario context");
            }
            catch (Exception ex)
            {
                TestLogger.LogError("Failed to initialize browser", ex);
                throw;
            }
        }

        [AfterScenario]
        public async Task AfterScenario(ScenarioContext scenarioContext)
        {
            try
            {
                if (scenarioContext.TestError != null)
                {
                    TestLogger.LogError($"Scenario failed: {scenarioContext.ScenarioInfo.Title}", scenarioContext.TestError);
                    
                    if (_page != null)
                    {
                        var screenshotPath = Path.Combine(TestConfiguration.ScreenshotsDirectory, 
                            $"error_{scenarioContext.ScenarioInfo.Title}_{DateTime.Now:yyyyMMdd_HHmmss}.png");
                        
                        TestLogger.LogInfo($"Attempting to capture screenshot at: {screenshotPath}");
                        
                        await _page.ScreenshotAsync(new PageScreenshotOptions 
                        { 
                            Path = screenshotPath, 
                            FullPage = true 
                        });
                        
                        if (File.Exists(screenshotPath))
                        {
                            TestLogger.LogInfo($"Screenshot successfully saved to: {screenshotPath}");
                        }
                        else
                        {
                            TestLogger.LogError($"Failed to save screenshot to: {screenshotPath}");
                        }
                    }
                }

                if (_page != null)
                {
                    await _page.CloseAsync();
                    TestLogger.LogInfo("Page closed");
                }

                if (_context != null)
                {
                    await _context.CloseAsync();
                    TestLogger.LogInfo("Browser context closed");

                    // Verify video was recorded
                    var videoPath = scenarioContext.Get<string>("videoPath");
                    if (File.Exists(videoPath))
                    {
                        TestLogger.LogInfo($"Video successfully recorded at: {videoPath}");
                    }
                    else
                    {
                        TestLogger.LogError($"Video not found at expected path: {videoPath}");
                    }
                }

                if (_browser != null)
                {
                    await _browser.CloseAsync();
                    TestLogger.LogInfo("Browser closed");
                }

                if (_playwright != null)
                {
                    _playwright.Dispose();
                    TestLogger.LogInfo("Playwright disposed");
                }

                TestLogger.LogInfo($"Scenario completed: {scenarioContext.ScenarioInfo.Title}");
            }
            catch (Exception ex)
            {
                TestLogger.LogError("Error during cleanup", ex);
                throw;
            }
        }

        [AfterTestRun]
        public static void AfterTestRun()
        {
            TestLogger.LogInfo("Test run completed");
            
            // Verify artifact directories
            var screenshotFiles = Directory.GetFiles(TestConfiguration.ScreenshotsDirectory);
            var videoFiles = Directory.GetFiles(TestConfiguration.VideoDirectory);
            var logFiles = Directory.GetFiles(TestConfiguration.LogsDirectory);

            TestLogger.LogInfo($"Screenshots found: {screenshotFiles.Length}");
            TestLogger.LogInfo($"Videos found: {videoFiles.Length}");
            TestLogger.LogInfo($"Log files found: {logFiles.Length}");

            foreach (var file in screenshotFiles)
            {
                TestLogger.LogInfo($"Screenshot: {file}");
            }

            foreach (var file in videoFiles)
            {
                TestLogger.LogInfo($"Video: {file}");
            }

            foreach (var file in logFiles)
            {
                TestLogger.LogInfo($"Log: {file}");
            }
        }
    }
} 