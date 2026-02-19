using Microsoft.Playwright;
using MyTestAutomationFramework.Core;
using MyTestAutomationFramework.Helpers;
using MyTestAutomationFrameWork.Config;
using Reqnroll;
using Reqnroll.BoDi;

namespace ReqnRollExample.Hooks
{
    [Binding]
    public class PlaywrightHooks
    {
        private readonly IObjectContainer _objectContainer;
        private readonly ScenarioContext _scenarioContext;
        private BrowserManager? _browserManager;
        private IBrowserContext? _context;
        private IPage? _page;
        private CancellationTokenSource? _timeoutCancellationTokenSource;

        // Default scenario timeout (2 minutes)
        private const int DefaultScenarioTimeoutMs = 120000;

        public PlaywrightHooks(IObjectContainer objectContainer, ScenarioContext scenarioContext)
        {
            _objectContainer = objectContainer;
            _scenarioContext = scenarioContext;
        }

        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            TestLogger.Info("========================================");
            TestLogger.Info("Starting Reqnroll Test Run");
            TestLogger.Info("========================================");
        }

        [BeforeScenario]
        public async Task BeforeScenario()
        {
            var scenarioTitle = _scenarioContext.ScenarioInfo.Title;
            TestLogger.TestStart(scenarioTitle);
            TestLogger.Info($"Scenario: {scenarioTitle}");
            TestLogger.Info($"Tags: {string.Join(", ", _scenarioContext.ScenarioInfo.Tags)}");

            // Create timeout cancellation token
            var timeoutMs = GetScenarioTimeout();
            _timeoutCancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMilliseconds(timeoutMs));
            TestLogger.Info($"Scenario timeout: {timeoutMs}ms");

            // Start timeout monitoring task
            _ = MonitorScenarioTimeoutAsync(_timeoutCancellationTokenSource.Token);

            // Initialize browser for this scenario
            _browserManager = new BrowserManager();
            _context = await _browserManager.CreateContextAsync();
            _page = await _context.NewPageAsync();

            var config = ConfigurationManager.Instance;

            // Set Playwright default timeout
            _page.SetDefaultTimeout(config.PlaywrightSettings.Timeout);
            _page.SetDefaultNavigationTimeout(config.PlaywrightSettings.Timeout);

            // Listen to console messages
            _page.Console += (_, msg) =>
            {
                TestLogger.ConsoleMessage(msg.Type, msg.Text);
            };

            // Listen to page errors
            _page.PageError += (_, error) =>
            {
                TestLogger.Error($"Page Error: {error}");
            };

            // Register instances for DI
            _objectContainer.RegisterInstanceAs(_page);
            _objectContainer.RegisterInstanceAs(_context);
            _objectContainer.RegisterInstanceAs(new SessionManager(_page));

            // Navigate to base URL if configured
            var baseUrl = config.PlaywrightSettings.BaseUrl;
            if (!string.IsNullOrEmpty(baseUrl))
            {
                TestLogger.Info($"Navigating to: {baseUrl}");
                await _page.GotoAsync(baseUrl);
            }
        }

        [AfterScenario]
        public async Task AfterScenario()
        {
            // Cancel timeout monitoring
            _timeoutCancellationTokenSource?.Cancel();
            _timeoutCancellationTokenSource?.Dispose();

            var scenarioTitle = _scenarioContext.ScenarioInfo.Title;
            var status = _scenarioContext.TestError == null ? "PASSED" : "FAILED";

            if (_scenarioContext.TestError != null)
            {
                TestLogger.Error($"Scenario failed: {_scenarioContext.TestError.Message}");

                // Capture screenshot on failure
                if (_page != null)
                {
                    var screenshotPath = Path.Combine(
                        "screenshots",
                        $"{scenarioTitle.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd_HHmmss}.png");

                    Directory.CreateDirectory("screenshots");
                    await _page.ScreenshotAsync(new PageScreenshotOptions
                    {
                        Path = screenshotPath,
                        FullPage = true
                    });

                    TestLogger.Screenshot(screenshotPath);
                }
            }

            // Cleanup
            if (_context != null)
            {
                await _context.CloseAsync();
                await _context.DisposeAsync();
            }

            if (_browserManager != null)
            {
                await _browserManager.DisposeAsync();
            }

            TestLogger.TestEnd(scenarioTitle, status);
        }

        [AfterTestRun]
        public static void AfterTestRun()
        {
            TestLogger.Info("========================================");
            TestLogger.Info("Reqnroll Test Run Completed");
            TestLogger.Info("========================================");
        }

        /// <summary>
        /// Get timeout for scenario based on tags
        /// </summary>
        private int GetScenarioTimeout()
        {
            // Check for timeout tag like @timeout:180000 (3 minutes)
            var timeoutTag = _scenarioContext.ScenarioInfo.Tags
                .FirstOrDefault(tag => tag.StartsWith("timeout:", StringComparison.OrdinalIgnoreCase));

            if (timeoutTag != null)
            {
                var timeoutValue = timeoutTag.Split(':')[1];
                if (int.TryParse(timeoutValue, out var timeout))
                {
                    return timeout;
                }
            }

            // Check for predefined tags
            if (_scenarioContext.ScenarioInfo.Tags.Contains("slow"))
            {
                return 300000; // 5 minutes for slow tests
            }

            if (_scenarioContext.ScenarioInfo.Tags.Contains("quick"))
            {
                return 30000; // 30 seconds for quick tests
            }

            return DefaultScenarioTimeoutMs; // Default: 2 minutes
        }

        /// <summary>
        /// Monitor scenario timeout
        /// </summary>
        private async Task MonitorScenarioTimeoutAsync(CancellationToken cancellationToken)
        {
            try
            {
                await Task.Delay(Timeout.Infinite, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                // Timeout occurred
                if (!cancellationToken.IsCancellationRequested)
                {
                    TestLogger.Error($"Scenario timeout exceeded!");
                    throw new TimeoutException($"Scenario '{_scenarioContext.ScenarioInfo.Title}' exceeded timeout");
                }
            }
        }
    }
}