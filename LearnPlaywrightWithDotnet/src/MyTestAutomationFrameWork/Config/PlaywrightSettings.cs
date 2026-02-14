namespace MyTestAutomationFrameWork.Config
{
    public class PlaywrightSettings
    {
        public bool Headless { get; set; }
        public float SlowMo { get; set; }
        public int Timeout { get; set; }
        public string BrowserType { get; set; } = "chromium";
        public int ViewportWidth { get; set; }
        public int ViewportHeight { get; set; }
        public string Screenshot { get; set; } = "only-on-failure";
        public string Video { get; set; } = "retain-on-failure";
        public string Trace { get; set; } = "retain-on-failure";
        public string BaseUrl { get; set; } = string.Empty;
        public int MaxRetries { get; set; }
        public int Workers { get; set; }
        public bool IgnoreHTTPSErrors { get; set; }
        public bool AcceptDownloads { get; set; }

        public bool TrackConsoleErrors { get; set; } = true;
        public bool FailTestOnConsoleError { get; set; } = false;
        public List<string> IgnoreConsoleErrorPatterns { get; set; } = new();
    }

    public class BrowserContextOptions
    {
        public string? UserAgent { get; set; }
        public string Locale { get; set; } = "en-US";
        public string TimezoneId { get; set; } = "America/New_York";
        public List<string> Permissions { get; set; } = new();
        public Geolocation? Geolocation { get; set; }
        public string ColorScheme { get; set; } = "light";
    }

    public class Geolocation
    {
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public float Accuracy { get; set; }
    }

    public class ReportingSettings
    {
        public string OutputFolder { get; set; } = "test-results";
        public string ScreenshotFolder { get; set; } = "screenshots";
        public string VideoFolder { get; set; } = "videos";
        public string TracesFolder { get; set; } = "traces";
    }
}
