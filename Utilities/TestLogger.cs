using System;
using System.IO;
using System.Text;
using HerokuAutomation_Playwright_Reqnroll.Config;

namespace HerokuAutomation_Playwright_Reqnroll.Utilities
{
    public static class TestLogger
    {
        private static string _logFilePath;
        private static readonly object _lock = new object();

        static TestLogger()
        {
            Directory.CreateDirectory(TestConfiguration.LogsDirectory);
            _logFilePath = Path.Combine(TestConfiguration.LogsDirectory, $"test_log_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
            Log("INFO", "Logger initialized");
        }

        public static void LogInfo(string message)
        {
            Log("INFO", message);
        }

        public static void LogError(string message, Exception ex = null)
        {
            var errorMessage = new StringBuilder();
            errorMessage.AppendLine($"ERROR: {message}");
            
            if (ex != null)
            {
                errorMessage.AppendLine("Stack Trace:");
                errorMessage.AppendLine(ex.ToString());
            }

            Log("ERROR", errorMessage.ToString());
        }

        private static void Log(string level, string message)
        {
            lock (_lock)
            {
                try
                {
                    var logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}{Environment.NewLine}";
                    File.AppendAllText(_logFilePath, logEntry);
                }
                catch (Exception ex)
                {
                    // If logging fails, write to console as fallback
                    Console.WriteLine($"Failed to write to log file: {ex.Message}");
                    Console.WriteLine($"Original message: [{level}] {message}");
                }
            }
        }

        public static string GetCurrentLogFile()
        {
            return _logFilePath;
        }
    }
} 