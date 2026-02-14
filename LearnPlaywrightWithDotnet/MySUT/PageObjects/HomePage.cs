using Microsoft.Playwright;
using MyTestAutomationFramework.PageObjects;
using System.Text.RegularExpressions;
using static Microsoft.Playwright.Assertions;

namespace MySUT.PageObjects
{
    public class HomePage : BasePage
    {
        public ButtonsAccordion ButtonsAccordion { get; }


        private readonly ILocator _rootElement;

        public HomePage(IPage page) : base(page)
        {
            _rootElement = Page.Locator("[data-testid='main-page']");

            ButtonsAccordion = new ButtonsAccordion(_rootElement.Locator("[data-testid='buttons']"), "Buttons accordion");
        }

        public override async Task<bool> IsLoadedAsync()
        {
            await base.IsLoadedAsync();
            await Expect(Page).ToHaveTitleAsync(new Regex("Main Page"));
            await ButtonsAccordion.ValidateElementVisibleAsync();
            return true;
        }
    }
}