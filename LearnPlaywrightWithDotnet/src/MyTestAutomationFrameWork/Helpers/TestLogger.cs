using Serilog;
using Serilog.Events;

namespace MyTestAutomationFramework.Helpers
{
    public static class TestLogger
    {
        private static ILogger? _logger;

        public static ILogger Instance
        {
            get
            {
                if (_logger == null)
                {
                    _logger = new LoggerConfiguration()
                        .MinimumLevel.Debug()
                        .WriteTo.Console(
                            outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                        .WriteTo.File(
                            path: "logs/test-log-.txt",
                            rollingInterval: RollingInterval.Day,
                            outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                        .CreateLogger();
                }
                return _logger;
            }
        }

        public static void Info(string message) => Instance.Information(message);
        public static void Debug(string message) => Instance.Debug(message);
        public static void Warning(string message) => Instance.Warning(message);
        public static void Error(string message, Exception? ex = null) => Instance.Error(ex, message);
        public static void TestStart(string testName) => Instance.Information("▶️ TEST STARTED: {TestName}", testName);
        public static void TestEnd(string testName, string status) => Instance.Information("✅ TEST {Status}: {TestName}", status, testName);
        public static void Step(string stepDescription) => Instance.Information("🔹 STEP: {Step}", stepDescription);
    }
}