using Microsoft.Playwright;
using System;
using System.IO;
using System.Threading.Tasks;
using HerokuAutomation_Playwright_Reqnroll.Config;
using HerokuAutomation_Playwright_Reqnroll.Utilities;

namespace HerokuAutomation_Playwright_Reqnroll.Pages
{
    public class LoginPage : BasePage
    {
        private const string UsernameInput = "#username";
        private const string PasswordInput = "#password";
        private const string LoginButton = "button[type='submit']";
        private const string FlashMessage = "#flash";
        private const string LogoutButton = "a[href='/logout']";

        public LoginPage(IPage page) : base(page)
        {
        }

        public async Task LoginAsync(string username, string password)
        {
            try
            {
                TestLogger.LogInfo($"Attempting login with username: {username}");
                
                // Wait for and fill username
                await Page.WaitForSelectorAsync(UsernameInput, new PageWaitForSelectorOptions { Timeout = 5000 });
                await Page.FillAsync(UsernameInput, username);
                TestLogger.LogInfo("Username filled");

                // Wait for and fill password
                await Page.WaitForSelectorAsync(PasswordInput, new PageWaitForSelectorOptions { Timeout = 5000 });
                await Page.FillAsync(PasswordInput, password);
                TestLogger.LogInfo("Password filled");

                // Wait for and click login button
                await Page.WaitForSelectorAsync(LoginButton, new PageWaitForSelectorOptions { Timeout = 5000 });
                await Page.ClickAsync(LoginButton);
                TestLogger.LogInfo("Login button clicked");

                // Wait for either success or error message
                await Page.WaitForSelectorAsync(FlashMessage, new PageWaitForSelectorOptions { Timeout = 5000 });
                TestLogger.LogInfo("Flash message appeared");
            }
            catch (Exception ex)
            {
                TestLogger.LogError("Login attempt failed", ex);
                throw;
            }
        }

        public async Task<bool> IsLoginSuccessfulAsync()
        {
            try
            {
                TestLogger.LogInfo("Checking if login was successful");
                var isVisible = await Page.IsVisibleAsync(LogoutButton);
                TestLogger.LogInfo($"Logout button visible: {isVisible}");
                return isVisible;
            }
            catch (Exception ex)
            {
                TestLogger.LogError("Error checking login status", ex);
                return false;
            }
        }

        public async Task<string> GetFlashMessageAsync()
        {
            try
            {
                TestLogger.LogInfo("Getting flash message");
                await Page.WaitForSelectorAsync(FlashMessage, new PageWaitForSelectorOptions { Timeout = 5000 });
                var element = await Page.QuerySelectorAsync(FlashMessage);
                
                if (element == null)
                {
                    TestLogger.LogError("Flash message element not found");
                    return string.Empty;
                }

                var message = await element.TextContentAsync();
                TestLogger.LogInfo($"Raw flash message content: {message}");
                
                // Clean up the message (remove the '×' character and trim)
                message = message?.Replace("×", "").Trim() ?? string.Empty;
                TestLogger.LogInfo($"Cleaned flash message: {message}");
                
                return message;
            }
            catch (Exception ex)
            {
                TestLogger.LogError("Error getting flash message", ex);
                throw;
            }
        }

        public async Task TakeScreenshotOnFailureAsync()
        {
            try
            {
                TestLogger.LogInfo("Taking screenshot on failure");
                var screenshotPath = Path.Combine(TestConfiguration.ScreenshotsDirectory, 
                    $"login_failure_{DateTime.Now:yyyyMMdd_HHmmss}.png");
                
                await Page.ScreenshotAsync(new PageScreenshotOptions
                {
                    Path = screenshotPath,
                    FullPage = true
                });
                
                TestLogger.LogInfo($"Screenshot captured at: {screenshotPath}");
            }
            catch (Exception ex)
            {
                TestLogger.LogError("Failed to capture screenshot", ex);
                throw;
            }
        }
    }
} 