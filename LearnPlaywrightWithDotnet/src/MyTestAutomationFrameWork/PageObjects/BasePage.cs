using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace MyTestAutomationFramework.PageObjects
{
    public abstract class BasePage
    {
        protected readonly IPage Page;
        protected readonly IBrowserContext Context;

        protected BasePage(IPage page)
        {
            Page = page;
            Context = page.Context;
        }

        public virtual async Task<bool> IsLoadedAsync()
        {
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            return true;
        }

        protected async Task<ILocator> GetElementAsync(string selector)
        {
            return Page.Locator(selector);
        }

        protected async Task ClickAsync(string selector)
        {
            await Page.ClickAsync(selector);
        }

        protected async Task FillAsync(string selector, string text)
        {
            await Page.FillAsync(selector, text);
        }

        protected async Task<string> GetTextAsync(string selector)
        {
            return await Page.TextContentAsync(selector) ?? string.Empty;
        }

        protected async Task WaitForSelectorAsync(string selector, int? timeout = null)
        {
            await Page.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions
            {
                Timeout = timeout ?? 30000
            });
        }
    }
}