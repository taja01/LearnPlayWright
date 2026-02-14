using Microsoft.Playwright;

namespace MyTestAutomationFramework.PageObjects
{
    public abstract class BasePage(IPage page)
    {
        protected readonly IPage Page = page;
        protected readonly IBrowserContext Context = page.Context;

        public virtual async Task<bool> IsLoadedAsync()
        {
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            return true;
        }
    }
}