using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using TransTrack.Logging;

namespace TransTrack.Tests
{
    [TestClass]
    public class FileLoggerTests
    {
        private string _testLogPath;
        private FileLogger _logger;

        [TestInitialize]
        public void Setup()
        {
            _testLogPath = Path.Combine(Path.GetTempPath(), "test_log.txt");
            _logger = new FileLogger(_testLogPath);
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (File.Exists(_testLogPath))
            {
                File.Delete(_testLogPath);
            }
        }

        [TestMethod]
        public void LogInfo_WritesInfoMessageToFile()
        {
            // Arrange
            string message = "Test info message";

            // Act
            _logger.LogInfo(message);

            // Assert
            Assert.IsTrue(File.Exists(_testLogPath));
            string logContent = File.ReadAllText(_testLogPath);
            Assert.IsTrue(logContent.Contains("[INFO]"));
            Assert.IsTrue(logContent.Contains(message));
        }

        [TestMethod]
        public void LogError_WritesErrorMessageToFile()
        {
            // Arrange
            string message = "Test error message";

            // Act
            _logger.LogError(message);

            // Assert
            Assert.IsTrue(File.Exists(_testLogPath));
            string logContent = File.ReadAllText(_testLogPath);
            Assert.IsTrue(logContent.Contains("[ERROR]"));
            Assert.IsTrue(logContent.Contains(message));
        }

        [TestMethod]
        public void LogWarning_WritesWarningMessageToFile()
        {
            // Arrange
            string message = "Test warning message";

            // Act
            _logger.LogWarning(message);

            // Assert
            Assert.IsTrue(File.Exists(_testLogPath));
            string logContent = File.ReadAllText(_testLogPath);
            Assert.IsTrue(logContent.Contains("[WARNING]"));
            Assert.IsTrue(logContent.Contains(message));
        }

        [TestMethod]
        public void Log_CreatesLogFileIfNotExists()
        {
            // Arrange
            string newLogPath = Path.Combine(Path.GetTempPath(), "new_test_log.txt");
            var newLogger = new FileLogger(newLogPath);

            try
            {
                // Act
                newLogger.LogInfo("Creating new log file");

                // Assert
                Assert.IsTrue(File.Exists(newLogPath));
            }
            finally
            {
                if (File.Exists(newLogPath))
                {
                    File.Delete(newLogPath);
                }
            }
        }

        [TestMethod]
        public void Log_AppendsToExistingFile()
        {
            // Arrange
            _logger.LogInfo("First message");

            // Act
            _logger.LogInfo("Second message");

            // Assert
            string logContent = File.ReadAllText(_testLogPath);
            Assert.IsTrue(logContent.Contains("First message"));
            Assert.IsTrue(logContent.Contains("Second message"));
        }

        [TestMethod]
        public void Log_IncludesTimestamp()
        {
            // Arrange
            string message = "Message with timestamp";

            // Act
            _logger.LogInfo(message);

            // Assert
            string logContent = File.ReadAllText(_testLogPath);

            // Check for date format (YYYY-MM-DD)
            Assert.IsTrue(System.Text.RegularExpressions.Regex.IsMatch(
                logContent,
                @"\d{4}-\d{2}-\d{2}"
            ));

            // Check for time format (HH:MM:SS)
            Assert.IsTrue(System.Text.RegularExpressions.Regex.IsMatch(
                logContent,
                @"\d{2}:\d{2}:\d{2}"
            ));
        }

        [TestMethod]
        public void Log_MultipleMessages_AllWrittenCorrectly()
        {
            // Arrange & Act
            _logger.LogInfo("Info message");
            _logger.LogWarning("Warning message");
            _logger.LogError("Error message");

            // Assert
            string logContent = File.ReadAllText(_testLogPath);
            string[] lines = logContent.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            Assert.AreEqual(3, lines.Length);
            Assert.IsTrue(lines[0].Contains("[INFO]") && lines[0].Contains("Info message"));
            Assert.IsTrue(lines[1].Contains("[WARNING]") && lines[1].Contains("Warning message"));
            Assert.IsTrue(lines[2].Contains("[ERROR]") && lines[2].Contains("Error message"));
        }

        [TestMethod]
        public void Log_CreatesDirectoryIfNotExists()
        {
            // Arrange
            string newDirectory = Path.Combine(Path.GetTempPath(), "TestLogDirectory");
            string newLogPath = Path.Combine(newDirectory, "test.log");

            // Ensure directory doesn't exist
            if (Directory.Exists(newDirectory))
            {
                Directory.Delete(newDirectory, true);
            }

            try
            {
                // Act
                var newLogger = new FileLogger(newLogPath);
                newLogger.LogInfo("Test message");

                // Assert
                Assert.IsTrue(Directory.Exists(newDirectory));
                Assert.IsTrue(File.Exists(newLogPath));
            }
            finally
            {
                if (Directory.Exists(newDirectory))
                {
                    Directory.Delete(newDirectory, true);
                }
            }
        }
    }
}