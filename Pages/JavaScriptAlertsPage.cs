using Microsoft.Playwright;

namespace HerokuAutomation_Playwright_Reqnroll.Pages
{
    public class JavaScriptAlertsPage : BasePage
    {
        public JavaScriptAlertsPage(IPage page) : base(page)
        {
            // Set up dialog handlers once when the page is created
            Page.Dialog += async (_, dialog) =>
            {
                switch (dialog.Type)
                {
                    case "alert":
                        await dialog.AcceptAsync();
                        break;
                    case "confirm":
                        await dialog.AcceptAsync();
                        break;
                    case "prompt":
                        await dialog.AcceptAsync("Test Message");
                        break;
                }
            };
        }

        public ILocator JsAlertButton => Page.GetByRole(AriaRole.Button, new() { Name = "Click for JS Alert" });
        public ILocator JsConfirmButton => Page.GetByRole(AriaRole.Button, new() { Name = "Click for JS Confirm" });
        public ILocator JsPromptButton => Page.GetByRole(AriaRole.Button, new() { Name = "Click for JS Prompt" });
        public ILocator ResultText => Page.Locator("#result");

        public async Task ClickJsAlertButton()
        {
            await JsAlertButton.ClickAsync();
        }

        public async Task ClickJsConfirmButton()
        {
            await JsConfirmButton.ClickAsync();
        }

        public async Task ClickJsPromptButton(string inputText)
        {
            await JsPromptButton.ClickAsync();
        }

        public async Task<string> GetResultText()
        {
            return await ResultText.TextContentAsync();
        }
    }
} 