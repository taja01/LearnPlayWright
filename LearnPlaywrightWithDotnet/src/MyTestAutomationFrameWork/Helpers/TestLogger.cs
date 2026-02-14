using Serilog;
using Serilog.Events;
using System.Diagnostics;

namespace MyTestAutomationFramework.Helpers
{
    public static class TestLogger
    {
        private static ILogger? _logger;
        private static int _stepCounter = 0;

        public static ILogger Instance
        {
            get
            {
                if (_logger == null)
                {
                    _logger = new LoggerConfiguration()
                        .MinimumLevel.Debug()
                        .WriteTo.Console(
                            outputTemplate: "[{Timestamp:HH:mm:ss.fff} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                        .WriteTo.File(
                            path: "logs/test-log-.txt",
                            rollingInterval: RollingInterval.Day,
                            outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {Message:lj}{NewLine}{Exception}",
                            restrictedToMinimumLevel: LogEventLevel.Debug)
                        .CreateLogger();
                }
                return _logger;
            }
        }

        // Basic Logging
        public static void Info(string message) => Instance.Information(message);
        public static void Debug(string message) => Instance.Debug(message);
        public static void Warning(string message) => Instance.Warning(message);
        public static void Error(string message, Exception? ex = null) => Instance.Error(ex, message);

        // Test Lifecycle
        public static void TestStart(string testName)
        {
            _stepCounter = 0;
            var separator = new string('=', 80);
            Instance.Information(separator);
            Instance.Information("🚀 TEST STARTED: {TestName}", testName);
            Instance.Information("   Time: {Time}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            Instance.Information(separator);
        }

        public static void TestEnd(string testName, string status)
        {
            var separator = new string('=', 80);
            var emoji = status.ToUpper() == "PASSED" ? "✅" : "❌";
            Instance.Information(separator);
            Instance.Information("{Emoji} TEST {Status}: {TestName}", emoji, status.ToUpper(), testName);
            Instance.Information(separator);
        }

        // Test Steps
        public static void Step(string stepDescription)
        {
            _stepCounter++;
            Instance.Information("📋 STEP {StepNumber}: {Step}", _stepCounter, stepDescription);
        }

        public static void Success(string message)
        {
            Instance.Information("✅ SUCCESS: {Message}", message);
        }

        // Actions & Verifications
        public static void Action(string action, string target)
        {
            Instance.Information("🎬 ACTION: {Action} on '{Target}'", action, target);
        }

        public static void Verify(string verification)
        {
            Instance.Information("🔍 VERIFY: {Verification}", verification);
        }

        // API Logging
        public static void ApiRequest(string method, string url)
        {
            Instance.Information("🌐 API REQUEST: {Method} {Url}", method, url);
        }

        public static void ApiResponse(int statusCode, string? body = null)
        {
            if (string.IsNullOrEmpty(body))
            {
                Instance.Information("📥 API RESPONSE: {StatusCode}", statusCode);
            }
            else
            {
                var truncatedBody = body.Length > 200 ? body.Substring(0, 200) + "..." : body;
                Instance.Information("📥 API RESPONSE: {StatusCode} - {Body}", statusCode, truncatedBody);
            }
        }

        // Screenshots & Artifacts
        public static void Screenshot(string path)
        {
            Instance.Information("📸 SCREENSHOT: {Path}", path);
        }

        public static void Artifact(string type, string path)
        {
            Instance.Information("📎 ARTIFACT ({Type}): {Path}", type, path);
        }

        // Test Data
        public static void TestData(string key, object value)
        {
            Instance.Information("📊 TEST DATA: {Key} = {Value}", key, value);
        }

        // Sections
        public static void Section(string sectionName)
        {
            var separator = new string('-', 60);
            Instance.Information("");
            Instance.Information(separator);
            Instance.Information("📦 {SectionName}", sectionName);
            Instance.Information(separator);
        }

        // Console Messages - Enhanced
        public static void ConsoleMessage(string type, string message)
        {
            var logLevel = type.ToLower() switch
            {
                "error" => LogEventLevel.Error,
                "warning" => LogEventLevel.Warning,
                _ => LogEventLevel.Debug
            };

            var emoji = type.ToLower() switch
            {
                "error" => "🔴",
                "warning" => "🟡",
                "log" => "💬",
                _ => "ℹ️"
            };

            if (logLevel == LogEventLevel.Error)
            {
                Instance.Write(logLevel, "{Emoji} BROWSER CONSOLE ERROR: {Message}", emoji, message);
            }
            else if (logLevel == LogEventLevel.Warning)
            {
                Instance.Write(logLevel, "{Emoji} BROWSER CONSOLE WARNING: {Message}", emoji, message);
            }
            else
            {
                Instance.Debug("{Emoji} BROWSER CONSOLE [{Type}]: {Message}", emoji, type.ToUpper(), message);
            }
        }

        // Console Error Summary
        public static void ConsoleErrorSummary(List<string> errors)
        {
            if (errors.Count == 0) return;

            var separator = new string('!', 80);
            Instance.Error(separator);
            Instance.Error("🔴 CONSOLE ERRORS DETECTED: {Count} error(s)", errors.Count);
            Instance.Error(separator);

            for (int i = 0; i < errors.Count; i++)
            {
                Instance.Error("  {Index}. {Error}", i + 1, errors[i]);
            }

            Instance.Error(separator);
        }

        // Performance Tracking
        public static void Performance(string operation, long milliseconds)
        {
            Instance.Information("⏱️  PERFORMANCE: {Operation} took {Milliseconds}ms", operation, milliseconds);
        }

        // Element Interactions
        public static void ElementAction(string element, string action)
        {
            Instance.Debug("🖱️  {Action} on {Element}", action, element);
        }

        public static void ElementVerification(string element, string verification, bool passed)
        {
            var emoji = passed ? "✅" : "❌";
            Instance.Information("{Emoji} {Element}: {Verification}", emoji, element, verification);
        }

        // Browser Events
        public static void BrowserEvent(string eventType, string details)
        {
            Instance.Debug("🌐 BROWSER {EventType}: {Details}", eventType, details);
        }

        public static void RequestFailed(string url, string? failure)
        {
            Instance.Warning("⚠️  REQUEST FAILED: {Url} - {Failure}", url, failure ?? "Unknown reason");
        }

        // Timed Section
        public static IDisposable TimedSection(string sectionName)
        {
            return new TimedSectionLogger(sectionName);
        }

        // Cleanup
        public static void Close()
        {
            (_logger as IDisposable)?.Dispose();
            _logger = null;
        }

        // Private helper class for timed sections
        private class TimedSectionLogger : IDisposable
        {
            private readonly string _sectionName;
            private readonly Stopwatch _stopwatch;

            public TimedSectionLogger(string sectionName)
            {
                _sectionName = sectionName;
                _stopwatch = Stopwatch.StartNew();
                TestLogger.Section($"START: {_sectionName}");
            }

            public void Dispose()
            {
                _stopwatch.Stop();
                TestLogger.Performance(_sectionName, _stopwatch.ElapsedMilliseconds);
            }
        }
    }
}