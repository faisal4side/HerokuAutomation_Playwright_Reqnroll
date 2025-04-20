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
        // Selectors
        private const string UsernameInputSelector = "#username";
        private const string PasswordInputSelector = "#password";
        private const string LoginButtonSelector = "button[type='submit']";
        private const string FlashMessageSelector = "#flash";
        private const string SecureAreaHeaderSelector = "h2";
        private const string LogoutButton = "a[href='/logout']";

        public LoginPage(IPage page) : base(page)
        {
        }

        public async Task LoginAsync(string username, string password)
        {
            await TypeTextAsync(UsernameInputSelector, username);
            await TypeTextAsync(PasswordInputSelector, password);
            await ClickAsync(LoginButtonSelector);
            await WaitForPageLoadAsync();
        }

        public async Task<bool> IsLoginSuccessfulAsync()
        {
            await Page.WaitForSelectorAsync(SecureAreaHeaderSelector);
            var headerText = await GetTextAsync(SecureAreaHeaderSelector);
            return headerText.Contains("Secure Area");
        }

        public async Task<string> GetFlashMessageAsync()
        {
            await Page.WaitForSelectorAsync(FlashMessageSelector);
            return await GetTextAsync(FlashMessageSelector);
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