using Microsoft.Extensions.Configuration;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace MyTestAutomationFrameWork.Config
{
    public class ConfigurationManager
    {
        private static ConfigurationManager? _instance;
        private static readonly object _lock = new();
        private readonly IConfiguration _configuration;

        private ConfigurationManager()
        {
            var environment = Environment.GetEnvironmentVariable("TEST_ENVIRONMENT") ?? "Development";

            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("Config/appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"Config/appsettings.{environment}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
        }

        public static ConfigurationManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new ConfigurationManager();
                    }
                }
                return _instance;
            }
        }

        public PlaywrightSettings PlaywrightSettings =>
            _configuration.GetSection(nameof(PlaywrightSettings)).Get<PlaywrightSettings>() ?? new PlaywrightSettings();

        public BrowserContextOptions BrowserContextOptions =>
            _configuration.GetSection(nameof(BrowserContextOptions)).Get<BrowserContextOptions>() ?? new BrowserContextOptions();

        public ReportingSettings ReportingSettings =>
            _configuration.GetSection(nameof(ReportingSettings)).Get<ReportingSettings>() ?? new ReportingSettings();

        public string GetValue(string key) => _configuration[key] ?? string.Empty;
    }
}
