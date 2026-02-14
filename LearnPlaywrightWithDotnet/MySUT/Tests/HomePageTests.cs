using MySUT.PageObjects;
using MyTestAutomationFramework.API;
using MyTestAutomationFramework.Helpers;
using MyTestAutomationFrameWork.Tests;

namespace MySUT.Tests
{
    [TestFixture]
    public class HomePageTests : BaseTest
    {
        private HomePage? _homePage;
        private MockResponseHandler? _mockHandler;

        [SetUp]
        public async Task SetUp()
        {
            TestLogger.TestStart(TestContext.CurrentContext.Test.Name); _homePage = new HomePage(Page);
            _mockHandler = new MockResponseHandler(Page);
            await _homePage.IsLoadedAsync();
        }

        [TearDown]
        public void TearDown()
        {
            var status = TestContext.CurrentContext.Result.Outcome.Status ==
                NUnit.Framework.Interfaces.TestStatus.Passed ? "PASSED" : "FAILED";
            TestLogger.TestEnd(TestContext.CurrentContext.Test.Name, status);
        }

        [Test]
        public async Task VerifyHomePageLoads()
        {
            TestLogger.Step("Checking if Home Page is loaded and Get Started button is visible");
            var hasGetStarted = await _homePage!.ButtonsAccordion.IsVisibleAsync();
            Assert.That(hasGetStarted, Is.True, "Get Started button should be visible");
        }

        [Test]
        public async Task ElementsAddRemoveAndCounts()
        {
            TestLogger.Step("Check lists");
            await _homePage!.ButtonsAccordion.AddElementButton.ClickAsync();

            await _homePage!.ButtonsAccordion.ButtonWithError.ClickAsync();


            await _homePage!.ButtonsAccordion.AddElementButton.ClickAsync();
            await _homePage!.ButtonsAccordion.AddElementButton.ClickAsync();

            await _homePage!.ButtonsAccordion.RemoveElementButton.ClickAsync();
        }

    }
}