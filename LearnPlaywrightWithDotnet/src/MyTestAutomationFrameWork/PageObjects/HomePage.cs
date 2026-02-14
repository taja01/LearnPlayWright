using Microsoft.Playwright;
using MyTestAutomationFrameWork.Core;
using static Microsoft.Playwright.Assertions;

namespace MyTestAutomationFramework.PageObjects
{
    public class HomePage : BasePage
    {
        // WebElements - Low-level access
        public WebElement GetStartedButton { get; }
        public WebElement SearchButton { get; }
        public WebElement DocsLink { get; }
        public WebElement ApiLink { get; }
        public WebElement CommunityLink { get; }

        private readonly ILocator _rootElement;

        public HomePage(IPage page) : base(page)
        {
            _rootElement = Page.Locator("body");

            // Initialize WebElements
            GetStartedButton = new WebElement(Page.Locator("text=Get Started"), "Get Started Button");
            SearchButton = new WebElement(Page.Locator("[aria-label='Search']"), "Search Button");
            DocsLink = new WebElement(Page.Locator("text=Docs"), "Docs Link");
            ApiLink = new WebElement(Page.Locator("text=API"), "API Link");
            CommunityLink = new WebElement(Page.Locator("text=Community"), "Community Link");
        }

        public override async Task<bool> IsLoadedAsync()
        {
            await base.IsLoadedAsync();
            await Expect(Page).ToHaveTitleAsync(new Regex("Playwright"));
            await GetStartedButton.ValidateElementVisibleAsync();
            return true;
        }

        // Business Methods - High-level page actions
        public async Task NavigateToGetStartedAsync()
        {
            await GetStartedButton.ClickAsync();
            await Expect(Page).ToHaveURLAsync(new Regex(".*intro"));
        }

        public async Task SearchForAsync(string searchTerm)
        {
            await SearchButton.ClickAsync();
            await Page.Keyboard.TypeAsync(searchTerm);
            await Page.Keyboard.PressAsync("Enter");
        }

        public async Task NavigateToDocsAsync()
        {
            await DocsLink.ClickAsync();
            await Expect(Page).ToHaveURLAsync(new Regex(".*docs"));
        }

        public async Task NavigateToApiAsync()
        {
            await ApiLink.ClickAsync();
        }

        // Verification Methods
        public async Task VerifyPageLoadedCorrectlyAsync()
        {
            await GetStartedButton.ValidateElementVisibleAsync();
            await SearchButton.ValidateElementVisibleAsync();
            await DocsLink.ValidateElementVisibleAsync();
        }

        public async Task<bool> HasGetStartedButtonAsync()
        {
            return await GetStartedButton.IsVisibleAsync();
        }
    }
}