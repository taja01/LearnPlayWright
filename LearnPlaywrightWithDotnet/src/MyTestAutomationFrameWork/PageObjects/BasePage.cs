using Microsoft.Playwright;

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
    }
}