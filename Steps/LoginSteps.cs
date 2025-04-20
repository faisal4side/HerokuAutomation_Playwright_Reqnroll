using Microsoft.Playwright;
using HerokuAutomation_Playwright_Reqnroll.Pages;
using Reqnroll;
using System.Threading.Tasks;
using System.IO;
using HerokuAutomation_Playwright_Reqnroll.Utilities;
using HerokuAutomation_Playwright_Reqnroll.Config;
using NUnit.Framework;
using System;
using System.Threading;

namespace HerokuAutomation_Playwright_Reqnroll.Steps
{
    [Binding]
    public class LoginSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly IPage _page;
        private readonly LoginPage _loginPage;

        public LoginSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _page = _scenarioContext.Get<IPage>("page");
            _loginPage = new LoginPage(_page);
        }

        [Given(@"I am on the login page")]
        public async Task GivenIAmOnTheLoginPage()
        {
            await _loginPage.NavigateToUrlAsync("https://the-internet.herokuapp.com/login");
        }

        [When(@"I enter valid username ""(.*)"" and password ""(.*)""")]
        [When(@"I enter invalid username ""(.*)"" and password ""(.*)""")]
        public async Task WhenIEnterCredentials(string username, string password)
        {
            await _loginPage.LoginAsync(username, password);
        }

        [Then(@"I should be logged in successfully")]
        public async Task ThenIShouldBeLoggedInSuccessfully()
        {
            var isLoggedIn = await _loginPage.IsLoginSuccessfulAsync();
            Assert.IsTrue(isLoggedIn, "Login was not successful");
        }

        [Then(@"I should see an error message")]
        public async Task ThenIShouldSeeAnErrorMessage()
        {
            var flashMessage = await _loginPage.GetFlashMessageAsync();
            TestLogger.LogInfo($"Actual flash message: {flashMessage}");
            
            // The error message can be either of these
            bool isValidError = flashMessage.Contains("Your username is invalid!") || 
                              flashMessage.Contains("Your password is invalid!");
            
            Assert.IsTrue(isValidError, $"Expected error message not found. Actual message: {flashMessage}");
        }

        [Then(@"a screenshot should be captured")]
        public async Task ThenAScreenshotShouldBeCaptured()
        {
            try
            {
                var screenshotFileName = $"login_failure_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                var screenshotPath = Path.Combine(TestConfiguration.ScreenshotsDirectory, screenshotFileName);
                
                TestLogger.LogInfo($"Attempting to capture screenshot at: {screenshotPath}");
                
                await _page.ScreenshotAsync(new PageScreenshotOptions
                {
                    Path = screenshotPath,
                    FullPage = true
                });

                // Wait a moment for the file to be written
                await Task.Delay(1000);

                if (File.Exists(screenshotPath))
                {
                    TestLogger.LogInfo($"Screenshot successfully captured at: {screenshotPath}");
                }
                else
                {
                    TestLogger.LogError($"Screenshot was not captured at: {screenshotPath}");
                    Assert.Fail($"Screenshot was not captured at: {screenshotPath}");
                }
            }
            catch (Exception ex)
            {
                TestLogger.LogError("Failed to capture screenshot", ex);
                throw;
            }
        }

        [Then(@"a video should be recorded")]
        public void ThenAVideoShouldBeRecorded()
        {
            var videoPath = Path.Combine(TestConfiguration.VideoDirectory, 
                $"{_scenarioContext.ScenarioInfo.Title}_{DateTime.Now:yyyyMMdd_HHmmss}.webm");
            
            // Wait a moment for the file to be written
            Thread.Sleep(1000);
            
            if (File.Exists(videoPath))
            {
                TestLogger.LogInfo($"Video successfully recorded at: {videoPath}");
            }
            else
            {
                TestLogger.LogError($"Video was not recorded at: {videoPath}");
                Assert.Fail($"Video was not recorded at: {videoPath}");
            }
        }

        [Then(@"logs should be generated")]
        public void ThenLogsShouldBeGenerated()
        {
            var logFile = TestLogger.GetCurrentLogFile();
            Assert.IsTrue(File.Exists(logFile), $"Log file was not generated at {logFile}");
            var logContent = File.ReadAllText(logFile);
            Assert.IsTrue(logContent.Contains(_scenarioContext.ScenarioInfo.Title), "Scenario title not found in logs");
            TestLogger.LogInfo($"Logs generated at: {logFile}");
        }

        [Then(@"stack trace should be captured")]
        public void ThenStackTraceShouldBeCaptured()
        {
            var logFile = TestLogger.GetCurrentLogFile();
            var logContent = File.ReadAllText(logFile);
            Assert.IsTrue(logContent.Contains("Stack Trace:"), "Stack trace was not captured in logs");
            TestLogger.LogInfo("Stack trace was captured in logs");
        }
    }
} 