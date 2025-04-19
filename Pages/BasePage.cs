using Microsoft.Playwright;
using System.Threading.Tasks;

namespace HerokuAutomation_Playwright_Reqnroll.Pages
{
    public class BasePage
    {
        protected IPage Page { get; private set; }

        public BasePage(IPage page)
        {
            Page = page;
        }

        public async Task WaitForSelectorAsync(string selector, PageWaitForSelectorOptions options = null)
        {
            await Page.WaitForSelectorAsync(selector, options);
        }

        public async Task TakeScreenshotAsync(string fileName)
        {
            await Page.ScreenshotAsync(new PageScreenshotOptions
            {
                Path = $"screenshots/{fileName}.png",
                FullPage = true
            });
        }

        public async Task NavigateToUrlAsync(string url)
        {
            await Page.GotoAsync(url);
        }
    }
} 