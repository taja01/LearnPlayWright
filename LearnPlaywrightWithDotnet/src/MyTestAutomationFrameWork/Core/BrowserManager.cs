using Microsoft.Playwright;
using MyTestAutomationFrameWork.Config;

namespace MyTestAutomationFramework.Core
{
    public class BrowserManager
    {
        private IPlaywright? _playwright;
        private IBrowser? _browser;
        private readonly ConfigurationManager _config;

        public BrowserManager()
        {
            _config = ConfigurationManager.Instance;
        }

        public async Task<IBrowser> GetBrowserAsync()
        {
            if (_browser == null)
            {
                await InitializeBrowserAsync();
            }
            return _browser!;
        }

        private async Task InitializeBrowserAsync()
        {
            var config = _config.PlaywrightSettings;
            _playwright = await Playwright.CreateAsync();

            var launchOptions = new BrowserTypeLaunchOptions
            {
                Headless = config.Headless,
                SlowMo = config.SlowMo,
                Timeout = config.Timeout,
                Args = ["--start-maximized", "--disable-blink-features=AutomationControlled"]
            };

            _browser = config.BrowserType.ToLower() switch
            {
                "firefox" => await _playwright.Firefox.LaunchAsync(launchOptions),
                "webkit" => await _playwright.Webkit.LaunchAsync(launchOptions),
                _ => await _playwright.Chromium.LaunchAsync(launchOptions)
            };
        }

        public async Task<IBrowserContext> CreateContextAsync(BrowserNewContextOptions? options = null)
        {
            var browser = await GetBrowserAsync();
            var config = _config.PlaywrightSettings;
            var contextConfig = _config.BrowserContextOptions;
            var reportingConfig = _config.ReportingSettings;

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

        public async Task DisposeAsync()
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