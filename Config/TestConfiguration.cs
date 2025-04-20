using System.IO;
using System.Reflection;

namespace HerokuAutomation_Playwright_Reqnroll.Config
{
    public static class TestConfiguration
    {
        private static readonly string OutputDirectory = AppDomain.CurrentDomain.BaseDirectory;
        
        public static string BaseUrl => "https://the-internet.herokuapp.com";
        public static int DefaultTimeout => 5000; // 5 seconds
        
        // Artifact directories
        public static string ScreenshotsDirectory => Path.Combine(OutputDirectory, "Screenshots");
        public static string VideoDirectory => Path.Combine(OutputDirectory, "Videos");
        public static string LogsDirectory => Path.Combine(OutputDirectory, "Logs");
        public static string TestDataDirectory => Path.Combine(OutputDirectory, "TestData");
        public static string ReportsDirectory => Path.Combine(OutputDirectory, "Reports");

        static TestConfiguration()
        {
            // Ensure directories exist
            Directory.CreateDirectory(ScreenshotsDirectory);
            Directory.CreateDirectory(VideoDirectory);
            Directory.CreateDirectory(LogsDirectory);
            Directory.CreateDirectory(TestDataDirectory);
            Directory.CreateDirectory(ReportsDirectory);
        }

        // Video recording settings
        public static BrowserRecordingOptions RecordingOptions => new()
        {
            Dir = VideoDirectory,
            Size = new() { Width = 1920, Height = 1080 }
        };

        // Screenshot settings
        public static class ScreenshotSettings
        {
            public static bool FullPage => true;
            public static string Format => "png";
        }
    }

    public class BrowserRecordingOptions
    {
        public string Dir { get; set; }
        public string Format => "webm";
        public VideoSize Size { get; set; }
    }

    public class VideoSize
    {
        public int Width { get; set; }
        public int Height { get; set; }
    }
} 