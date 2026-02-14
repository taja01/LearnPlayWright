using Microsoft.Playwright;
using MyTestAutomationFramework.Core;
using MyTestAutomationFramework.Helpers;
using MyTestAutomationFrameWork.Config;

namespace MyTestAutomationFrameWork.Tests
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
        private readonly List<string> _consoleErrors = new();
        private readonly List<string> _consoleWarnings = new();

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

            // Clear console errors/warnings for this test
            _consoleErrors.Clear();
            _consoleWarnings.Clear();

            Context = await BrowserManager.CreateContextAsync();
            Page = await Context.NewPageAsync();
            SessionManager = new SessionManager(Page);

            Page.SetDefaultTimeout(Config.PlaywrightSettings.Timeout);

            // Listen to console messages with error tracking
            if (Config.PlaywrightSettings.TrackConsoleErrors)
            {
                Page.Console += OnConsoleMessage;
            }

            // Listen to page errors
            Page.PageError += (_, error) =>
            {
                var errorMessage = $"Page Error: {error}";
                TestLogger.Error(errorMessage);
                _consoleErrors.Add(errorMessage);
            };

            // Listen to request failures
            Page.RequestFailed += (_, request) =>
            {
                var failureMessage = $"{request.Url} - {request.Failure}";
                TestLogger.RequestFailed(request.Url, request.Failure);

                // Only track as error if not in ignore patterns
                if (!ShouldIgnoreError(failureMessage))
                {
                    _consoleWarnings.Add($"Request Failed: {failureMessage}");
                }
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

            // Show console error summary
            if (_consoleErrors.Count > 0 || _consoleWarnings.Count > 0)
            {
                TestLogger.Section("BROWSER CONSOLE ISSUES DETECTED");

                if (_consoleErrors.Count > 0)
                {
                    TestLogger.ConsoleErrorSummary(_consoleErrors);
                }

                if (_consoleWarnings.Count > 0)
                {
                    TestLogger.Warning($"Console Warnings: {_consoleWarnings.Count}");
                    foreach (var warning in _consoleWarnings)
                    {
                        TestLogger.Warning($"  - {warning}");
                    }
                }

                // Optionally fail test if console errors detected
                if (Config.PlaywrightSettings.FailTestOnConsoleError && _consoleErrors.Count > 0)
                {
                    testFailed = true;
                    Assert.Fail($"Test failed due to {_consoleErrors.Count} console error(s). See logs for details.");
                }
            }
            else
            {
                TestLogger.Success("No console errors detected");
            }

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

        private void OnConsoleMessage(object? sender, IConsoleMessage msg)
        {
            var message = msg.Text;
            var type = msg.Type;

            TestLogger.ConsoleMessage(type, message);

            // Track errors and warnings
            if (type.ToLower() == "error" && !ShouldIgnoreError(message))
            {
                _consoleErrors.Add(message);
            }
            else if (type.ToLower() == "warning" && !ShouldIgnoreError(message))
            {
                _consoleWarnings.Add(message);
            }
        }

        private bool ShouldIgnoreError(string errorMessage)
        {
            var ignorePatterns = Config.PlaywrightSettings.IgnoreConsoleErrorPatterns;

            if (ignorePatterns == null || ignorePatterns.Count == 0)
                return false;

            return ignorePatterns.Any(pattern =>
                errorMessage.Contains(pattern, StringComparison.OrdinalIgnoreCase));
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

            // Save console errors to file
            if (_consoleErrors.Count > 0)
            {
                var errorLogPath = await SaveConsoleErrorsAsync($"{testName}_{timestamp}");
                _testArtifacts.Add(errorLogPath);
            }
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

        protected async Task<string> SaveConsoleErrorsAsync(string fileName)
        {
            var errorLogPath = Path.Combine(
                Config.ReportingSettings.OutputFolder,
                "console-errors",
                $"{fileName}.txt");

            Directory.CreateDirectory(Path.GetDirectoryName(errorLogPath)!);

            var content = new System.Text.StringBuilder();
            content.AppendLine($"Console Errors for Test: {TestContext.CurrentContext.Test.Name}");
            content.AppendLine($"Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            content.AppendLine(new string('=', 80));
            content.AppendLine();

            content.AppendLine($"ERRORS ({_consoleErrors.Count}):");
            foreach (var error in _consoleErrors)
            {
                content.AppendLine($"  - {error}");
            }

            if (_consoleWarnings.Count > 0)
            {
                content.AppendLine();
                content.AppendLine($"WARNINGS ({_consoleWarnings.Count}):");
                foreach (var warning in _consoleWarnings)
                {
                    content.AppendLine($"  - {warning}");
                }
            }

            await File.WriteAllTextAsync(errorLogPath, content.ToString());
            TestContext.AddTestAttachment(errorLogPath);

            TestLogger.Artifact("Console Errors", errorLogPath);

            return errorLogPath;
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
                Config.ReportingSettings.TracesFolder,
                Path.Combine(Config.ReportingSettings.OutputFolder, "console-errors")
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