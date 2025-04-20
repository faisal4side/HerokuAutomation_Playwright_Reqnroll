using Microsoft.Playwright;

namespace HerokuAutomation_Playwright_Reqnroll.Pages
{
    public class JavaScriptAlertsPage : BasePage
    {
        public JavaScriptAlertsPage(IPage page) : base(page)
        {
        }

        public ILocator JsAlertButton => Page.GetByRole(AriaRole.Button, new() { Name = "Click for JS Alert" });
        public ILocator JsConfirmButton => Page.GetByRole(AriaRole.Button, new() { Name = "Click for JS Confirm" });
        public ILocator JsPromptButton => Page.GetByRole(AriaRole.Button, new() { Name = "Click for JS Prompt" });
        public ILocator ResultText => Page.Locator("#result");

        public async Task ClickJsAlertButton()
        {
            Page.Dialog += async (_, dialog) =>
            {
                await dialog.AcceptAsync();
            };
            await JsAlertButton.ClickAsync();
        }

        public async Task ClickJsConfirmButton()
        {
            Page.Dialog += async (_, dialog) =>
            {
                await dialog.AcceptAsync();
            };
            await JsConfirmButton.ClickAsync();
        }

        public async Task ClickJsPromptButton(string inputText)
        {
            Page.Dialog += async (_, dialog) =>
            {
                await dialog.AcceptAsync(inputText);
            };
            await JsPromptButton.ClickAsync();
        }

        public async Task<string> GetResultText()
        {
            return await ResultText.TextContentAsync();
        }
    }
} 