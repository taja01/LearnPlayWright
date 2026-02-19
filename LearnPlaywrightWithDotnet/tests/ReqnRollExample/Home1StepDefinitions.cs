using Microsoft.Playwright;
using MySUT.PageObjects;
using MyTestAutomationFramework.Helpers;
using Reqnroll;

namespace ReqnRollExample
{
    [Binding]
    public class Home1StepDefinitions : Steps
    {
        private readonly IPage _page;
        private HomePage? _homePage;

        public Home1StepDefinitions(IPage page)
        {
            _page = page;
        }

        [Given(@"I am on the home page")]
        public async Task GivenIAmOnTheHomePage()
        {
            TestLogger.Step("Navigating to home page");
            _homePage = new HomePage(_page);
            await _homePage.IsLoadedAsync();
            TestLogger.Success("Home page loaded");
        }

        [When(@"I click '([^']*)' times to add new item")]
        public async Task WhenIClickTimesToAddNewItem(int p0)
        {
            for (int i = 0; i < p0; i++)
            {
                await _homePage.ButtonsAccordion.AddElementButton.ClickAsync();
            }
        }

        [When(@"I click '([^']*)' times to remove item")]
        public async Task WhenIClickTimesToRemoveItem(int p0)
        {
            for (int i = 0; i < p0; i++)
            {
                await _homePage.ButtonsAccordion.RemoveElementButton.ClickAsync();
            }
        }

        [Then(@"I should have '([^']*)' items in the list")]
        public async Task ThenIShouldHaveItemsInTheList(int p0)
        {
            var actualCount = await _homePage.ButtonsAccordion.Entry.CountAsync();

            Assert.That(actualCount, Is.EqualTo(p0), $"Expected {p0} items in the list but found {actualCount}");
        }
    }
}
