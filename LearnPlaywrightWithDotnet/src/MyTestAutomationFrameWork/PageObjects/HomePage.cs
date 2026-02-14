using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace MyTestAutomationFramework.PageObjects
{
    public class HomePage : BasePage
    {
        // Locators
        private ILocator GetStartedButton => Page.Locator("text=Get Started");
        private ILocator SearchButton => Page.Locator("[aria-label='Search']");
        private ILocator DocsLink => Page.Locator("text=Docs");

        public HomePage(IPage page) : base(page) { }

        public override async Task<bool> IsLoadedAsync()
        {
            await base.IsLoadedAsync();
            await Expect(Page).ToHaveTitleAsync(new Regex("Playwright"));
            return true;
        }

        public async Task ClickGetStartedAsync()
        {
            await GetStartedButton.ClickAsync();
        }

        public async Task<bool> HasGetStartedButtonAsync()
        {
            return await GetStartedButton.IsVisibleAsync();
        }

        public async Task NavigateToDocsAsync()
        {
            await DocsLink.ClickAsync();
        }

        public async Task SearchForAsync(string searchTerm)
        {
            await SearchButton.ClickAsync();
            await Page.Keyboard.TypeAsync(searchTerm);
        }
    }
}