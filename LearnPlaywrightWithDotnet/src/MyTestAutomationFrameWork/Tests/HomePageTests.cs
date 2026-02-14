using MyTestAutomationFramework.API;
using MyTestAutomationFramework.Core;
using MyTestAutomationFramework.PageObjects;
using static Microsoft.Playwright.Assertions;

namespace MyTestAutomationFramework.Tests
{
    [TestFixture]
    public class HomePageTests : BaseTest
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
        public async Task VerifyHomePageLoads()
        {
            var hasGetStarted = await _homePage!.GetStartedButton.IsVisibleAsync();
            Assert.That(hasGetStarted, Is.True, "Get Started button should be visible");
        }

        [Test]
        public async Task NavigateToIntroPage()
        {
            await _homePage!.GetStartedButton.ClickAsync();
            await Expect(Page).ToHaveURLAsync(new Regex(".*intro"));
        }

        [Test]
        [Description("Demonstrates API mocking by intercepting GitHub API and returning fake user data")]
        public async Task TestWithMockedGitHubApiResponse()
        {
            // Setup: Mock the GitHub API to return fake user data
            await _mockHandler!.MockApiResponseAsync(
                "**/users/taja01",
                new
                {
                    login = "taja01",
                    id = 99999,
                    name = "MOCKED Test User",
                    company = "Mocked Company",
                    blog = "https://mocked-blog.com",
                    location = "Mocked City",
                    bio = "This is FAKE data from the mock!",
                    public_repos = 999,
                    followers = 888
                }
            );

            // Navigate to a page that calls GitHub API
            await Page.GotoAsync("https://api.github.com/users/taja01");

            // The page will show our mocked data instead of real GitHub data
            var content = await Page.TextContentAsync("body");

            Assert.That(content, Does.Contain("MOCKED Test User"));
            Assert.That(content, Does.Contain("Mocked Company"));
            Assert.That(content, Does.Contain("This is FAKE data from the mock!"));
        }

        [Test]
        [Description("Mock API failure to test error handling")]
        public async Task TestWithMockedApiFailure()
        {
            // Mock an API failure (500 error)
            await _mockHandler!.MockApiFailureAsync(
                "**/users/taja01",
                statusCode: 500,
                errorMessage: "Service Unavailable - This is a mocked error!"
            );

            await Page.GotoAsync("https://api.github.com/users/taja01");

            var content = await Page.TextContentAsync("body");
            Assert.That(content, Does.Contain("Service Unavailable - This is a mocked error!"));
        }

        [Test]
        [Description("Intercept and verify request was made")]
        public async Task TestApiRequestInterception()
        {
            var requestMade = false;
            var requestUrl = "";

            // Intercept and capture the request
            await Page.RouteAsync("**/users/taja01", async route =>
            {
                requestMade = true;
                requestUrl = route.Request.Url;

                // Return mocked response
                await route.FulfillAsync(new()
                {
                    Status = 200,
                    ContentType = "application/json",
                    Body = """{"login":"taja01","name":"Intercepted User"}"""
                });
            });

            await Page.GotoAsync("https://api.github.com/users/taja01");

            Assert.That(requestMade, Is.True, "API request should have been intercepted");
            Assert.That(requestUrl, Does.Contain("/users/taja01"));
        }

        [Test]
        public async Task UserCanGetStarted()
        {
            // Test knows too much about page structure
            await _homePage!.GetStartedButton.ValidateElementVisibleAsync();
            await _homePage.GetStartedButton.AttributeValueEqualToAsync("href", "/docs/intro");
            await _homePage.GetStartedButton.ClickAsync();
            await Expect(Page).ToHaveURLAsync(new Regex(".*intro"));

            // Lots of repetition across tests
        }
    }
}