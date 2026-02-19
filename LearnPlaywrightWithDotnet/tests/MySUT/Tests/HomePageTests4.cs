using MySUT.PageObjects;
using MyTestAutomationFramework.API;
using MyTestAutomationFramework.Helpers;
using MyTestAutomationFrameWork.Tests;

namespace MySUT.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.Fixtures)]
    public class HomePageTests4 : BaseTest
    {
        private HomePage? _homePage;
        private MockResponseHandler? _mockHandler;

        [SetUp]
        public async Task SetUp()
        {
            _homePage = new HomePage(Page);
            _mockHandler = new MockResponseHandler(Page);
            await _homePage.IsLoadedAsync();
        }

        [Test]
        public async Task ElementsAddRemoveAndCounts()
        {
            TestLogger.Step("Check lists");
            await _homePage!.ButtonsAccordion.AddElementButton.ClickAsync();

            await _homePage!.ButtonsAccordion.ButtonWithError.ClickAsync();


            await _homePage!.ButtonsAccordion.AddElementButton.ClickAsync();
            await _homePage!.ButtonsAccordion.AddElementButton.ClickAsync();
            await _homePage!.ButtonsAccordion.AddElementButton.ClickAsync();
            await _homePage!.ButtonsAccordion.AddElementButton.ClickAsync();
            await _homePage!.ButtonsAccordion.AddElementButton.ClickAsync();
            await _homePage!.ButtonsAccordion.AddElementButton.ClickAsync();

            var counter = await _homePage!.ButtonsAccordion.Entry.CountAsync();

            Assert.That(counter, Is.EqualTo(7));

            await _homePage!.ButtonsAccordion.RemoveElementButton.ClickAsync();

            counter = await _homePage!.ButtonsAccordion.Entry.CountAsync();
            Assert.That(counter, Is.EqualTo(6));
        }


        [Test]
        public async Task ElementsAddRemoveAndCounts2()
        {
            TestLogger.Step("Check lists");
            await _homePage!.ButtonsAccordion.AddElementButton.ClickAsync();

            await _homePage!.ButtonsAccordion.ButtonWithError.ClickAsync();



            var counter = await _homePage!.ButtonsAccordion.Entry.CountAsync();

            Assert.That(counter, Is.EqualTo(1));

            await _homePage!.ButtonsAccordion.RemoveElementButton.ClickAsync();

            counter = await _homePage!.ButtonsAccordion.Entry.CountAsync();
            Assert.That(counter, Is.EqualTo(0));
        }
    }
}