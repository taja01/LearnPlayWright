using Microsoft.Playwright;
using MyTestAutomationFramework.Config;

namespace MyTestAutomationFramework.Core
{
    [Parallelizable(ParallelScope.Self)]
    [TestFixture]
    public class BaseTest
    {
        protected ConfigurationManager Config => ConfigurationManager.Instance;
        protected string BaseUrl => Config.PlaywrightSettings.BaseUrl;

        protected IPage Page { get; private set; } = null!;
        protected IBrowserContext Context { get; private set; } = null!;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            // Browser is initialized once per test class
            await BrowserManager.GetBrowserAsync();
        }

        [SetUp]
        public async Task BaseSetUp()
        {
            // Create new context and page for each test
            Context = await BrowserManager.CreateContextAsync();
            Page = await Context.NewPageAsync();

            // Set default timeout
            Page.SetDefaultTimeout(Config.PlaywrightSettings.Timeout);

            // Navigate to base URL if configured
            if (!string.IsNullOrEmpty(BaseUrl))
            {
                await Page.GotoAsync(BaseUrl);
            }
        }

        [TearDown]
        public async Task BaseTearDown()
        {
            // Capture screenshot on failure
            if (TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Failed)
            {
                await CaptureScreenshotAsync($"{TestContext.CurrentContext.Test.Name}_failure");

                if (Config.PlaywrightSettings.Trace != "off")
                {
                    await CaptureTraceAsync($"{TestContext.CurrentContext.Test.Name}_trace");
                }
            }

            // Close context after each test
            if (Context != null)
            {
                await Context.CloseAsync();
                await Context.DisposeAsync();
            }
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            await BrowserManager.DisposeAsync();
        }

        protected async Task CaptureScreenshotAsync(string screenshotName)
        {
            var screenshotPath = Path.Combine(
                Config.ReportingSettings.ScreenshotFolder,
                $"{screenshotName}_{DateTime.Now:yyyyMMdd_HHmmss}.png");

            Directory.CreateDirectory(Config.ReportingSettings.ScreenshotFolder);
            await Page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotPath, FullPage = true });
            TestContext.AddTestAttachment(screenshotPath);
        }

        protected async Task CaptureTraceAsync(string traceName)
        {
            var tracePath = Path.Combine(
                Config.ReportingSettings.TracesFolder,
                $"{traceName}_{DateTime.Now:yyyyMMdd_HHmmss}.zip");

            Directory.CreateDirectory(Config.ReportingSettings.TracesFolder);
            await Context.Tracing.StopAsync(new TracingStopOptions { Path = tracePath });
        }
    }
}