using System.IO;

namespace HerokuAutomation_Playwright_Reqnroll.Config
{
    public static class TestConfiguration
    {
        private static readonly string ProjectRoot = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
        
        public static string BaseUrl => "https://the-internet.herokuapp.com";
        public static int DefaultTimeout => 5000; // 5 seconds
        
        // Artifact directories
        public static string ScreenshotsDirectory => Path.Combine(ProjectRoot, "Screenshots");
        public static string VideoDirectory => Path.Combine(ProjectRoot, "Videos");
        public static string LogsDirectory => Path.Combine(ProjectRoot, "Logs");
        public static string TestDataDirectory => Path.Combine(ProjectRoot, "TestData");

        static TestConfiguration()
        {
            // Ensure directories exist
            Directory.CreateDirectory(ScreenshotsDirectory);
            Directory.CreateDirectory(VideoDirectory);
            Directory.CreateDirectory(LogsDirectory);
            Directory.CreateDirectory(TestDataDirectory);
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
        public VideoSize Size { get; set; }
    }

    public class VideoSize
    {
        public int Width { get; set; }
        public int Height { get; set; }
    }
} 