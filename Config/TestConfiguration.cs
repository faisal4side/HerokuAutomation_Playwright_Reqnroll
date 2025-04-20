using System.IO;
using System.Reflection;

namespace HerokuAutomation_Playwright_Reqnroll.Config
{
    public static class TestConfiguration
    {
        private static readonly string ProjectRoot = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..", "..", ".."));
        public static readonly string ArtifactsRoot = Path.Combine(ProjectRoot, "Artifacts");
        
        public static string BaseUrl => "https://the-internet.herokuapp.com";
        public static int DefaultTimeout => 5000; // 5 seconds
        
        // Artifact directories
        public static string ScreenshotsDirectory => Path.Combine(ArtifactsRoot, "Screenshots");
        public static string VideoDirectory => Path.Combine(ArtifactsRoot, "Videos");
        public static string LogsDirectory => Path.Combine(ArtifactsRoot, "Logs");
        public static string TestDataDirectory => Path.Combine(ArtifactsRoot, "TestData");
        public static string ReportsDirectory => Path.Combine(ArtifactsRoot, "Reports");
        public static string TracesDirectory => Path.Combine(ArtifactsRoot, "Traces");

        static TestConfiguration()
        {
            try
            {
                // Ensure directories exist
                if (!Directory.Exists(ScreenshotsDirectory))
                    Directory.CreateDirectory(ScreenshotsDirectory);
                if (!Directory.Exists(VideoDirectory))
                    Directory.CreateDirectory(VideoDirectory);
                if (!Directory.Exists(LogsDirectory))
                    Directory.CreateDirectory(LogsDirectory);
                if (!Directory.Exists(TestDataDirectory))
                    Directory.CreateDirectory(TestDataDirectory);
                if (!Directory.Exists(ReportsDirectory))
                    Directory.CreateDirectory(ReportsDirectory);
                if (!Directory.Exists(TracesDirectory))
                    Directory.CreateDirectory(TracesDirectory);

                Console.WriteLine($"Project root directory: {ProjectRoot}");
                Console.WriteLine($"Artifacts root directory: {ArtifactsRoot}");
                Console.WriteLine($"Screenshots directory: {ScreenshotsDirectory}");
                Console.WriteLine($"Logs directory: {LogsDirectory}");
                Console.WriteLine($"Traces directory: {TracesDirectory}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating artifact directories: {ex.Message}");
                throw;
            }
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