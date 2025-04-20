using Microsoft.Playwright;
using System;
using System.Threading.Tasks;

namespace HerokuAutomation_Playwright_Reqnroll.Pages
{
    public abstract class BasePage
    {
        protected readonly IPage Page;
        
        public BasePage(IPage page)
        {
            Page = page ?? throw new ArgumentNullException(nameof(page));
        }
        
        public async Task NavigateToUrlAsync(string url)
        {
            await Page.GotoAsync(url);
        }
        
        public async Task WaitForPageLoadAsync()
        {
            await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
        }
        
        public async Task ClickAsync(string selector)
        {
            await Page.ClickAsync(selector);
        }
        
        public async Task TypeTextAsync(string selector, string text)
        {
            await Page.FillAsync(selector, text);
        }
        
        public async Task<string> GetTextAsync(string selector)
        {
            return await Page.TextContentAsync(selector);
        }
        
        public async Task<bool> IsElementVisibleAsync(string selector)
        {
            var element = await Page.QuerySelectorAsync(selector);
            return element != null && await element.IsVisibleAsync();
        }
    }
} 