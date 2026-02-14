using Microsoft.Playwright;
using MyTestAutomationFrameWork.Config;

namespace MyTestAutomationFramework.Core
{
    public class BrowserManager
    {
        private static IPlaywright? _playwright;
        private static IBrowser? _browser;
        private static readonly object _lock = new();

        public static async Task<IBrowser> GetBrowserAsync()
        {
            if (_browser == null)
            {
                lock (_lock)
                {
                    if (_browser == null)
                    {
                        InitializeBrowserAsync().Wait();
                    }
                }
            }
            return _browser!;
        }

        private static async Task InitializeBrowserAsync()
        {
            var config = ConfigurationManager.Instance.PlaywrightSettings;
            _playwright = await Playwright.CreateAsync();

            var launchOptions = new BrowserTypeLaunchOptions
            {
                Headless = config.Headless,
                SlowMo = config.SlowMo,
                Timeout = config.Timeout,
                Args = ["--start-maximized"]
            };

            _browser = config.BrowserType.ToLower() switch
            {
                "firefox" => await _playwright.Firefox.LaunchAsync(launchOptions),
                "webkit" => await _playwright.Webkit.LaunchAsync(launchOptions),
                _ => await _playwright.Chromium.LaunchAsync(launchOptions)
            };
        }

        public static async Task<IBrowserContext> CreateContextAsync(BrowserNewContextOptions? options = null)
        {
            var browser = await GetBrowserAsync();
            var config = ConfigurationManager.Instance.PlaywrightSettings;
            var contextConfig = ConfigurationManager.Instance.BrowserContextOptions;
            var reportingConfig = ConfigurationManager.Instance.ReportingSettings;

            options ??= new BrowserNewContextOptions
            {
                ViewportSize = new ViewportSize
                {
                    Width = config.ViewportWidth,
                    Height = config.ViewportHeight
                },
                IgnoreHTTPSErrors = config.IgnoreHTTPSErrors,
                AcceptDownloads = config.AcceptDownloads,
                Locale = contextConfig.Locale,
                TimezoneId = contextConfig.TimezoneId,
                ColorScheme = contextConfig.ColorScheme.ToLower() switch
                {
                    "dark" => ColorScheme.Dark,
                    "light" => ColorScheme.Light,
                    _ => ColorScheme.NoPreference
                },
                RecordVideoDir = config.Video != "off" ? reportingConfig.VideoFolder : null,
                RecordVideoSize = new RecordVideoSize { Width = 1920, Height = 1080 }
            };

            var context = await browser.NewContextAsync(options);

            // Enable tracing if configured
            if (config.Trace != "off")
            {
                await context.Tracing.StartAsync(new TracingStartOptions
                {
                    Screenshots = true,
                    Snapshots = true,
                    Sources = true
                });
            }

            return context;
        }

        public static async Task DisposeAsync()
        {
            if (_browser != null)
            {
                await _browser.CloseAsync();
                await _browser.DisposeAsync();
                _browser = null;
            }

            _playwright?.Dispose();
            _playwright = null;
        }
    }
}