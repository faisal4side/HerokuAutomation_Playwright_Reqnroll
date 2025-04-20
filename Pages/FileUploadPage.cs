using Microsoft.Playwright;

namespace HerokuAutomation_Playwright_Reqnroll.Pages
{
    public class FileUploadPage : BasePage
    {
        public FileUploadPage(IPage page) : base(page)
        {
        }

        public ILocator ChooseFileButton => Page.GetByLabel("Choose File");
        public ILocator UploadButton => Page.GetByRole(AriaRole.Button, new() { Name = "Upload" });
        public ILocator UploadedFiles => Page.Locator("#uploaded-files");
        public ILocator SuccessMessage => Page.Locator("h3");

        public async Task UploadFile(string filePath)
        {
            var fileChooser = await Page.RunAndWaitForFileChooserAsync(async () =>
            {
                await ChooseFileButton.ClickAsync();
            });
            await fileChooser.SetFilesAsync(filePath);
            await UploadButton.ClickAsync();
        }

        public async Task<string> GetUploadedFileName()
        {
            return await UploadedFiles.TextContentAsync();
        }

        public async Task<string> GetSuccessMessage()
        {
            return await SuccessMessage.TextContentAsync();
        }
    }
} 