using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using TransTrack.Logging;

namespace TransTrack.Tests
{
    [TestClass]
    public class IntegrationTests
    {
        private string _testBaseFolder;
        private string _incomingFolder;
        private string _processedFolder;
        private string _errorsFolder;
        private string _logFilePath;
        private ILogger _logger;

        [TestInitialize]
        public void Setup()
        {
            // Create test folder structure
            _testBaseFolder = Path.Combine(Path.GetTempPath(), "TransTrack_IntegrationTests_" + Guid.NewGuid().ToString("N"));
            _incomingFolder = Path.Combine(_testBaseFolder, "Incoming");
            _processedFolder = Path.Combine(_testBaseFolder, "Processed");
            _errorsFolder = Path.Combine(_testBaseFolder, "Errors");
            _logFilePath = Path.Combine(_testBaseFolder, "test.log");

            Directory.CreateDirectory(_incomingFolder);
            Directory.CreateDirectory(_processedFolder);
            Directory.CreateDirectory(_errorsFolder);

            _logger = new FileLogger(_logFilePath);
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists(_testBaseFolder))
            {
                Directory.Delete(_testBaseFolder, true);
            }
        }

        [TestMethod]
        public void NorthProcessor_ValidCsvFile_ProcessesSuccessfully()
        {
            // Arrange
            string csvFile = Path.Combine(_incomingFolder, "valid_north.csv");
            string csvContent = "SH1001,Accra,Tema,2024-10-12,150.5\nSH1002,Accra,Kumasi,2024-09-20,90";
            File.WriteAllText(csvFile, csvContent);

            var parser = new TransTrack.FileHandling.CsvParser();
            var validator = new NorthProcessor.NorthValidator();
            var fileOps = new TransTrack.FileHandling.FileOperations();
            var outputWriter = new TransTrack.FileHandling.OutputWriter();

            // Act
            var records = parser.Parse(csvFile);
            var validation = validator.Validate(records);

            if (validation.IsValid)
            {
                var summary = new TransTrack.Common.Models.ProcessingSummary
                {
                    FileName = Path.GetFileName(csvFile),
                    TotalRecords = records.Count,
                    FileUrl = "https://test.cloudinary.com/test.csv",
                    DateProcessed = DateTime.Now,
                    IsValid = true
                };

                outputWriter.WriteProcessedFile(summary, _processedFolder);
                fileOps.DeleteProcessedFile(csvFile);
            }

            // Assert
            Assert.IsTrue(validation.IsValid);
            Assert.IsFalse(File.Exists(csvFile), "Original file should be deleted");

            var processedFiles = Directory.GetFiles(_processedFolder, "*_processed.csv");
            Assert.AreEqual(1, processedFiles.Length, "Should have one processed file");

            string processedContent = File.ReadAllText(processedFiles[0]);
            Assert.IsTrue(processedContent.Contains("TotalRecords: 2"));
            Assert.IsTrue(processedContent.Contains("valid_north.csv"));
        }

        [TestMethod]
        public void NorthProcessor_InvalidCsvFile_MovesToErrorsFolder()
        {
            // Arrange - Invalid file with zero weight
            string csvFile = Path.Combine(_incomingFolder, "invalid_north.csv");
            string csvContent = "SH1001,Accra,Tema,2024-10-12,0";
            File.WriteAllText(csvFile, csvContent);

            var parser = new TransTrack.FileHandling.CsvParser();
            var validator = new NorthProcessor.NorthValidator();
            var fileOps = new TransTrack.FileHandling.FileOperations();
            var outputWriter = new TransTrack.FileHandling.OutputWriter();

            // Act
            var records = parser.Parse(csvFile);
            var validation = validator.Validate(records);

            if (!validation.IsValid)
            {
                outputWriter.WriteErrorFile(Path.GetFileName(csvFile), validation.ErrorMessage, _errorsFolder);
                fileOps.DeleteInvalidFile(csvFile);
            }

            // Assert
            Assert.IsFalse(validation.IsValid);
            Assert.IsTrue(validation.ErrorMessage.Contains("weight"));
            Assert.IsFalse(File.Exists(csvFile), "Original file should be deleted");

            var errorFiles = Directory.GetFiles(_errorsFolder, "*_error.csv");
            Assert.AreEqual(1, errorFiles.Length, "Should have one error file");

            string errorContent = File.ReadAllText(errorFiles[0]);
            Assert.IsTrue(errorContent.Contains("invalid_north.csv"));
            Assert.IsTrue(errorContent.Contains("weight"));
        }

        [TestMethod]
        public void SouthProcessor_ValidTxtFile_ProcessesSuccessfully()
        {
            // Arrange
            string txtFile = Path.Combine(_incomingFolder, "valid_south.txt");
            string txtContent = "S-55221|North|Takoradi|2024-11-13|Bulk\nS-55222|East|Tema|2024-11-11|Fragile";
            File.WriteAllText(txtFile, txtContent);

            var parser = new TransTrack.FileHandling.TxtParser();
            var validator = new SouthProcessor.SouthValidator();
            var fileOps = new TransTrack.FileHandling.FileOperations();
            var outputWriter = new TransTrack.FileHandling.OutputWriter();

            // Act
            var records = parser.Parse(txtFile);
            var validation = validator.Validate(records);

            if (validation.IsValid)
            {
                var summary = new TransTrack.Common.Models.ProcessingSummary
                {
                    FileName = Path.GetFileName(txtFile),
                    TotalRecords = records.Count,
                    FileUrl = "https://test.cloudinary.com/test.txt",
                    DateProcessed = DateTime.Now,
                    IsValid = true
                };

                outputWriter.WriteProcessedFile(summary, _processedFolder);
                fileOps.DeleteProcessedFile(txtFile);
            }

            // Assert
            Assert.IsTrue(validation.IsValid);
            Assert.IsFalse(File.Exists(txtFile), "Original file should be deleted");

            var processedFiles = Directory.GetFiles(_processedFolder, "*_processed.txt");
            Assert.AreEqual(1, processedFiles.Length, "Should have one processed file");

            string processedContent = File.ReadAllText(processedFiles[0]);
            Assert.IsTrue(processedContent.Contains("TotalRecords: 2"));
            Assert.IsTrue(processedContent.Contains("valid_south.txt"));
        }

        [TestMethod]
        public void SouthProcessor_InvalidTxtFile_WeekendDate_MovesToErrorsFolder()
        {
            // Arrange - Invalid file with weekend date (2024-11-17 is Sunday)
            string txtFile = Path.Combine(_incomingFolder, "invalid_south.txt");
            string txtContent = "S-90019|West|CapeCoast|2024-11-17|Bulk";
            File.WriteAllText(txtFile, txtContent);

            var parser = new TransTrack.FileHandling.TxtParser();
            var validator = new SouthProcessor.SouthValidator();
            var fileOps = new TransTrack.FileHandling.FileOperations();
            var outputWriter = new TransTrack.FileHandling.OutputWriter();

            // Act
            var records = parser.Parse(txtFile);
            var validation = validator.Validate(records);

            if (!validation.IsValid)
            {
                outputWriter.WriteErrorFile(Path.GetFileName(txtFile), validation.ErrorMessage, _errorsFolder);
                fileOps.DeleteInvalidFile(txtFile);
            }

            // Assert
            Assert.IsFalse(validation.IsValid);
            Assert.IsTrue(validation.ErrorMessage.Contains("weekend"));
            Assert.IsFalse(File.Exists(txtFile), "Original file should be deleted");

            var errorFiles = Directory.GetFiles(_errorsFolder, "*_error.txt");
            Assert.AreEqual(1, errorFiles.Length, "Should have one error file");

            string errorContent = File.ReadAllText(errorFiles[0]);
            Assert.IsTrue(errorContent.Contains("invalid_south.txt"));
            Assert.IsTrue(errorContent.Contains("weekend"));
        }

        [TestMethod]
        public void MultipleFiles_ProcessedCorrectly()
        {
            // Arrange
            string validCsv = Path.Combine(_incomingFolder, "valid.csv");
            string invalidCsv = Path.Combine(_incomingFolder, "invalid.csv");

            File.WriteAllText(validCsv, "SH1001,Accra,Tema,2024-10-12,150.5");
            File.WriteAllText(invalidCsv, "SH1002,Accra,Tema,2024-10-12,0");

            var parser = new TransTrack.FileHandling.CsvParser();
            var validator = new NorthProcessor.NorthValidator();
            var fileOps = new TransTrack.FileHandling.FileOperations();
            var outputWriter = new TransTrack.FileHandling.OutputWriter();

            // Act - Process both files
            var files = Directory.GetFiles(_incomingFolder, "*.csv");
            foreach (var file in files)
            {
                var records = parser.Parse(file);
                var validation = validator.Validate(records);

                if (validation.IsValid)
                {
                    var summary = new TransTrack.Common.Models.ProcessingSummary
                    {
                        FileName = Path.GetFileName(file),
                        TotalRecords = records.Count,
                        FileUrl = "https://test.cloudinary.com/test.csv",
                        DateProcessed = DateTime.Now,
                        IsValid = true
                    };
                    outputWriter.WriteProcessedFile(summary, _processedFolder);
                    fileOps.DeleteProcessedFile(file);
                }
                else
                {
                    outputWriter.WriteErrorFile(Path.GetFileName(file), validation.ErrorMessage, _errorsFolder);
                    fileOps.DeleteInvalidFile(file);
                }
            }

            // Assert
            Assert.AreEqual(0, Directory.GetFiles(_incomingFolder).Length, "Incoming folder should be empty");
            Assert.AreEqual(1, Directory.GetFiles(_processedFolder, "*_processed.csv").Length, "Should have 1 processed file");
            Assert.AreEqual(1, Directory.GetFiles(_errorsFolder, "*_error.csv").Length, "Should have 1 error file");
        }

        [TestMethod]
        public void LoggingIntegration_LogsAllActivities()
        {
            // Arrange
            string csvFile = Path.Combine(_incomingFolder, "test.csv");
            File.WriteAllText(csvFile, "SH1001,Accra,Tema,2024-10-12,150.5");

            // Act
            _logger.LogInfo("Starting processing");
            _logger.LogInfo($"Processing file: {Path.GetFileName(csvFile)}");
            _logger.LogInfo("File processed successfully");

            // Assert
            Assert.IsTrue(File.Exists(_logFilePath));
            string logContent = File.ReadAllText(_logFilePath);

            Assert.IsTrue(logContent.Contains("Starting processing"));
            Assert.IsTrue(logContent.Contains("Processing file: test.csv"));
            Assert.IsTrue(logContent.Contains("File processed successfully"));
            Assert.IsTrue(logContent.Contains("[INFO]"));
        }

        [TestMethod]
        public void OutputWriter_CreatesCorrectOutputFormat()
        {
            // Arrange
            var summary = new TransTrack.Common.Models.ProcessingSummary
            {
                FileName = "test_shipment.csv",
                TotalRecords = 5,
                FileUrl = "https://res.cloudinary.com/demo/test_shipment.csv",
                DateProcessed = new DateTime(2024, 12, 8, 10, 30, 45),
                IsValid = true
            };

            var outputWriter = new TransTrack.FileHandling.OutputWriter();

            // Act
            outputWriter.WriteProcessedFile(summary, _processedFolder);

            // Assert
            var outputFiles = Directory.GetFiles(_processedFolder, "*_processed.csv");
            Assert.AreEqual(1, outputFiles.Length);

            string content = File.ReadAllText(outputFiles[0]);
            Assert.IsTrue(content.Contains("ProcessedFile: test_shipment.csv"));
            Assert.IsTrue(content.Contains("TotalRecords: 5"));
            Assert.IsTrue(content.Contains("FileUrl:"));
            Assert.IsTrue(content.Contains("https://res.cloudinary.com/demo/test_shipment.csv"));
            Assert.IsTrue(content.Contains("DateProcessed: 2024-12-08 10:30:45"));
        }
    }
}