using Microsoft.Playwright;
using MyTestAutomationFramework.Config;
using MyTestAutomationFramework.Helpers;

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
        protected SessionManager SessionManager { get; private set; } = null!;

        private readonly List<string> _testArtifacts = new();

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            await BrowserManager.GetBrowserAsync();
        }

        [SetUp]
        public async Task BaseSetUp()
        {
            Context = await BrowserManager.CreateContextAsync();
            Page = await Context.NewPageAsync();
            SessionManager = new SessionManager(Page);

            Page.SetDefaultTimeout(Config.PlaywrightSettings.Timeout);

            // Listen to console messages
            Page.Console += (_, msg) =>
            {
                TestContext.WriteLine($"[BROWSER {msg.Type.ToUpper()}]: {msg.Text}");
            };

            // Listen to page errors
            Page.PageError += (_, error) =>
            {
                TestContext.WriteLine($"[PAGE ERROR]: {error}");
            };

            if (!string.IsNullOrEmpty(BaseUrl))
            {
                await Page.GotoAsync(BaseUrl);
            }
        }

        [TearDown]
        public async Task BaseTearDown()
        {
            var testFailed = TestContext.CurrentContext.Result.Outcome.Status
                == NUnit.Framework.Interfaces.TestStatus.Failed;

            if (testFailed)
            {
                await CaptureFailureArtifactsAsync();
            }

            if (Context != null)
            {
                await Context.CloseAsync();
                await Context.DisposeAsync();
            }

            CleanupTestArtifacts();
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            await BrowserManager.DisposeAsync();
        }

        protected async Task CaptureFailureArtifactsAsync()
        {
            var testName = TestContext.CurrentContext.Test.Name;
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");

            // Screenshot
            var screenshotPath = await CaptureScreenshotAsync($"{testName}_{timestamp}");
            _testArtifacts.Add(screenshotPath);

            // Trace
            if (Config.PlaywrightSettings.Trace != "off")
            {
                var tracePath = await CaptureTraceAsync($"{testName}_{timestamp}");
                _testArtifacts.Add(tracePath);
            }

            // Page HTML
            var htmlPath = await CapturePageHtmlAsync($"{testName}_{timestamp}");
            _testArtifacts.Add(htmlPath);
        }

        protected async Task<string> CaptureScreenshotAsync(string screenshotName)
        {
            var screenshotPath = Path.Combine(
                Config.ReportingSettings.ScreenshotFolder,
                $"{screenshotName}.png");

            Directory.CreateDirectory(Config.ReportingSettings.ScreenshotFolder);
            await Page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotPath, FullPage = true });
            TestContext.AddTestAttachment(screenshotPath);

            return screenshotPath;
        }

        protected async Task<string> CaptureTraceAsync(string traceName)
        {
            var tracePath = Path.Combine(
                Config.ReportingSettings.TracesFolder,
                $"{traceName}.zip");

            Directory.CreateDirectory(Config.ReportingSettings.TracesFolder);
            await Context.Tracing.StopAsync(new TracingStopOptions { Path = tracePath });

            return tracePath;
        }

        protected async Task<string> CapturePageHtmlAsync(string htmlName)
        {
            var htmlPath = Path.Combine(
                Config.ReportingSettings.OutputFolder,
                "html",
                $"{htmlName}.html");

            Directory.CreateDirectory(Path.GetDirectoryName(htmlPath)!);
            var html = await Page.ContentAsync();
            await File.WriteAllTextAsync(htmlPath, html);
            TestContext.AddTestAttachment(htmlPath);

            return htmlPath;
        }

        protected void CleanupTestArtifacts()
        {
            // Optional: Clean up old artifacts older than X days
            var daysToKeep = 7;
            var cutoffDate = DateTime.Now.AddDays(-daysToKeep);

            foreach (var folder in new[]
            {
                Config.ReportingSettings.ScreenshotFolder,
                Config.ReportingSettings.VideoFolder,
                Config.ReportingSettings.TracesFolder
            })
            {
                if (Directory.Exists(folder))
                {
                    var files = Directory.GetFiles(folder);
                    foreach (var file in files)
                    {
                        var fileInfo = new FileInfo(file);
                        if (fileInfo.CreationTime < cutoffDate)
                        {
                            try { File.Delete(file); } catch { }
                        }
                    }
                }
            }
        }
    }
}