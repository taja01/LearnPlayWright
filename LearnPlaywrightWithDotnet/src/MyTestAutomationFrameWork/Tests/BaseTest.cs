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
            TestLogger.Info("Initializing browser for test suite");
            await BrowserManager.GetBrowserAsync();
        }

        [SetUp]
        public async Task BaseSetUp()
        {
            TestLogger.TestStart(TestContext.CurrentContext.Test.Name);

            Context = await BrowserManager.CreateContextAsync();
            Page = await Context.NewPageAsync();
            SessionManager = new SessionManager(Page);

            Page.SetDefaultTimeout(Config.PlaywrightSettings.Timeout);

            // Listen to console messages
            Page.Console += (_, msg) =>
            {
                TestLogger.ConsoleMessage(msg.Type, msg.Text);
            };

            // Listen to page errors
            Page.PageError += (_, error) =>
            {
                TestLogger.Error($"Page Error: {error}");
            };

            // Listen to request failures
            Page.RequestFailed += (_, request) =>
            {
                TestLogger.Warning($"Request Failed: {request.Url} - {request.Failure}");
            };

            if (!string.IsNullOrEmpty(BaseUrl))
            {
                TestLogger.Info($"Navigating to base URL: {BaseUrl}");
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
                TestLogger.Warning("Test failed - capturing artifacts");
                await CaptureFailureArtifactsAsync();
            }

            if (Context != null)
            {
                await Context.CloseAsync();
                await Context.DisposeAsync();
            }

            CleanupTestArtifacts();

            var status = testFailed ? "FAILED" : "PASSED";
            TestLogger.TestEnd(TestContext.CurrentContext.Test.Name, status);
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            TestLogger.Info("Disposing browser for test suite");
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

            TestLogger.Screenshot(screenshotPath);

            return screenshotPath;
        }

        protected async Task<string> CaptureTraceAsync(string traceName)
        {
            var tracePath = Path.Combine(
                Config.ReportingSettings.TracesFolder,
                $"{traceName}.zip");

            Directory.CreateDirectory(Config.ReportingSettings.TracesFolder);
            await Context.Tracing.StopAsync(new TracingStopOptions { Path = tracePath });

            TestLogger.Artifact("Trace", tracePath);

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

            TestLogger.Artifact("HTML", htmlPath);

            return htmlPath;
        }

        protected void CleanupTestArtifacts()
        {
            var daysToKeep = 7;
            var cutoffDate = DateTime.Now.AddDays(-daysToKeep);
            var deletedCount = 0;

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
                            try
                            {
                                File.Delete(file);
                                deletedCount++;
                            }
                            catch (Exception ex)
                            {
                                TestLogger.Warning($"Failed to delete old artifact: {file} - {ex.Message}");
                            }
                        }
                    }
                }
            }

            if (deletedCount > 0)
            {
                TestLogger.Debug($"Cleaned up {deletedCount} old artifact(s) older than {daysToKeep} days");
            }
        }
    }
}