using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using AventStack.ExtentReports.Gherkin.Model;
using System;
using System.IO;
using System.Linq;
using HerokuAutomation_Playwright_Reqnroll.Config;
using NUnit.Framework;

namespace HerokuAutomation_Playwright_Reqnroll.Utilities
{
    public class ReportingHelper
    {
        private static ExtentReports _extent;
        private static ExtentTest _feature;
        private static ExtentTest _scenario;
        private static string _reportPath;
        private static ExtentHtmlReporter _htmlReporter;

        public static void InitializeReport()
        {
            try
            {
                if (_extent == null)
                {
                    // Create a unique report file name
                    var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    var projectDir = Directory.GetCurrentDirectory();
                    var reportDirectory = Path.Combine(projectDir, "TestResults", "Reports");
                    Directory.CreateDirectory(reportDirectory);
                    
                    _reportPath = Path.Combine(reportDirectory, $"TestReport_{timestamp}.html");
                    Console.WriteLine($"Creating report at: {_reportPath}");
                    
                    // Initialize the HTML reporter
                    _htmlReporter = new ExtentHtmlReporter(_reportPath);
                    _htmlReporter.Config.Theme = AventStack.ExtentReports.Reporter.Configuration.Theme.Dark;
                    _htmlReporter.Config.DocumentTitle = "Automation Test Report";
                    _htmlReporter.Config.ReportName = "Playwright Test Automation Report";
                    
                    // Initialize ExtentReports
                    _extent = new ExtentReports();
                    _extent.AttachReporter(_htmlReporter);
                    
                    // Add system info
                    _extent.AddSystemInfo("Environment", "Test");
                    _extent.AddSystemInfo("Browser", "Chromium");
                    _extent.AddSystemInfo("Execution Time", DateTime.Now.ToString());
                    
                    Console.WriteLine("Report initialized successfully");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing report: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        public static void StartFeature(string featureName, string description = "")
        {
            try
            {
                if (_extent == null)
                {
                    InitializeReport();
                }
                _feature = _extent.CreateTest<Feature>(featureName, description);
                Console.WriteLine($"Started feature: {featureName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting feature: {ex.Message}");
            }
        }

        public static void StartScenario(string scenarioName)
        {
            try
            {
                _scenario = _feature.CreateNode<Scenario>(scenarioName);
                Console.WriteLine($"Started scenario: {scenarioName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting scenario: {ex.Message}");
            }
        }

        public static void LogStep(StepType stepType, string stepName, string details = "", MediaEntityModelProvider media = null)
        {
            try
            {
                var node = _scenario.CreateNode(new GherkinKeyword(stepType.ToString()), stepName);
                if (!string.IsNullOrEmpty(details))
                {
                    node.Info(details);
                }
                if (media != null)
                {
                    node.Log(Status.Info, media);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error logging step: {ex.Message}");
            }
        }

        public static void LogPass(string stepName, string details = "", MediaEntityModelProvider media = null)
        {
            try
            {
                var node = _scenario.Pass(stepName);
                if (!string.IsNullOrEmpty(details))
                {
                    node.Info(details);
                }
                if (media != null)
                {
                    node.Log(Status.Pass, media);
                }
                Console.WriteLine($"Logged pass: {stepName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error logging pass: {ex.Message}");
            }
        }

        public static void LogFail(string stepName, string details = "", MediaEntityModelProvider media = null)
        {
            try
            {
                var node = _scenario.Fail(stepName);
                if (!string.IsNullOrEmpty(details))
                {
                    node.Info(details);
                }
                if (media != null)
                {
                    node.Log(Status.Fail, media);
                }
                
                // Add screenshot on failure if available
                var screenshot = CaptureScreenshot();
                if (screenshot != null)
                {
                    node.Log(Status.Fail, screenshot);
                }
                Console.WriteLine($"Logged failure: {stepName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error logging failure: {ex.Message}");
            }
        }

        public static void LogSkip(string stepName, string details = "")
        {
            try
            {
                var node = _scenario.Skip(stepName);
                if (!string.IsNullOrEmpty(details))
                {
                    node.Info(details);
                }
                Console.WriteLine($"Logged skip: {stepName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error logging skip: {ex.Message}");
            }
        }

        public static MediaEntityModelProvider CaptureScreenshot()
        {
            try
            {
                var screenshotFiles = Directory.GetFiles(TestConfiguration.ScreenshotsDirectory, "*.png");
                var latestScreenshot = screenshotFiles.OrderByDescending(f => new FileInfo(f).CreationTime).FirstOrDefault();
                
                if (latestScreenshot != null)
                {
                    Console.WriteLine($"Captured screenshot: {latestScreenshot}");
                    return MediaEntityBuilder.CreateScreenCaptureFromPath(latestScreenshot).Build();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to capture screenshot: {ex.Message}");
            }
            return null;
        }

        public static void FinalizeReport()
        {
            try
            {
                if (_extent != null)
                {
                    _extent.Flush();
                    Console.WriteLine($"Report finalized at: {_reportPath}");
                    
                    // Verify the report file exists
                    if (File.Exists(_reportPath))
                    {
                        Console.WriteLine($"Report file successfully created at: {_reportPath}");
                        // Try to open the report
                        try
                        {
                            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                            {
                                FileName = _reportPath,
                                UseShellExecute = true
                            });
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Could not open report automatically: {ex.Message}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Warning: Report file not found at expected location: {_reportPath}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error finalizing report: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }
    }

    public enum StepType
    {
        Given,
        When,
        Then,
        And,
        But
    }
} 