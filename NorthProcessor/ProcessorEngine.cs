using System;
using System.Collections.Generic;
using System.IO;
using TransTrack.Common.Models;
using TransTrack.FileHandling;
using TransTrack.Logging;

namespace NorthProcessor
{
    public class ProcessorEngine
    {
        private readonly string _incomingFolder;
        private readonly string _processedFolder;
        private readonly string _errorsFolder;
        private readonly CloudinaryUploader _uploader;
        private readonly ILogger _logger;
        private readonly CsvParser _parser;
        private readonly NorthValidator _validator;
        private readonly FileOperations _fileOps;
        private readonly OutputWriter _outputWriter;

        public ProcessorEngine(string incomingFolder, string processedFolder,
            string errorsFolder, string cloudName, string apiKey,
            string apiSecret, ILogger logger)
        {
            _incomingFolder = incomingFolder;
            _processedFolder = processedFolder;
            _errorsFolder = errorsFolder;
            _uploader = new CloudinaryUploader(cloudName, apiKey, apiSecret);
            _logger = logger;
            _parser = new CsvParser();
            _validator = new NorthValidator();
            _fileOps = new FileOperations();
            _outputWriter = new OutputWriter();
        }

        public void ProcessFiles()
        {
            var files = Directory.GetFiles(_incomingFolder, "*.csv");

            if (files.Length == 0)
            {
                _logger.LogInfo("No files to process");
                Console.WriteLine("No CSV files found in incoming folder");
                return;
            }

            _logger.LogInfo($"Found {files.Length} file(s) to process");

            foreach (var file in files)
            {
                ProcessFile(file);
            }
        }

        private void ProcessFile(string filePath)
        {
            string fileName = Path.GetFileName(filePath);
            _logger.LogInfo($"Processing file: {fileName}");

            try
            {
                // Parse file
                List<ShipmentRecord> records = _parser.Parse(filePath);
                _logger.LogInfo($"Parsed {records.Count} record(s) from {fileName}");

                // Validate
                ValidationResult validation = _validator.Validate(records);

                if (validation.IsValid)
                {
                    // Upload to Cloudinary
                    _logger.LogInfo($"Uploading {fileName} to Cloudinary...");
                    string fileUrl = _uploader.UploadFile(filePath);
                    _logger.LogInfo($"File uploaded successfully: {fileUrl}");

                    // Write processed output to Processed folder
                    var summary = new ProcessingSummary
                    {
                        FileName = fileName,
                        TotalRecords = records.Count,
                        FileUrl = fileUrl,
                        DateProcessed = DateTime.Now,
                        IsValid = true
                    };

                    _outputWriter.WriteProcessedFile(summary, _processedFolder);

                    // DELETE the original file from Incoming folder
                    _fileOps.DeleteProcessedFile(filePath);

                    _logger.LogInfo($"File processed successfully and deleted from incoming: {fileName}");
                    Console.WriteLine($"✓ Processed: {fileName} ({records.Count} records)");
                    Console.WriteLine($"  Original file deleted from incoming folder");
                }
                else
                {
                    // Write error report to Errors folder (only the report file)
                    _outputWriter.WriteErrorFile(fileName, validation.ErrorMessage, _errorsFolder);

                    // DELETE the invalid file from Incoming folder
                    _fileOps.DeleteInvalidFile(filePath);

                    _logger.LogWarning($"File validation failed and deleted from incoming: {fileName} - {validation.ErrorMessage}");
                    Console.WriteLine($"✗ Invalid: {fileName}");
                    Console.WriteLine($"  Reason: {validation.ErrorMessage}");
                    Console.WriteLine($"  File deleted from incoming folder");
                    Console.WriteLine($"  Error report saved to Errors folder");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing {fileName}: {ex.Message}");
                _logger.LogError($"Stack trace: {ex.StackTrace}");
                Console.WriteLine($"✗ Error: {fileName}");
                Console.WriteLine($"  Message: {ex.Message}");

                try
                {
                    // Write error report to Errors folder
                    _outputWriter.WriteErrorFile(fileName, $"System error: {ex.Message}", _errorsFolder);

                    // DELETE the file that caused the error
                    _fileOps.DeleteInvalidFile(filePath);

                    _logger.LogError($"File deleted due to processing error: {fileName}");
                    Console.WriteLine($"  File deleted from incoming folder");
                    Console.WriteLine($"  Error report saved to Errors folder");
                }
                catch (Exception deleteEx)
                {
                    _logger.LogError($"Failed to delete error file: {deleteEx.Message}");
                }
            }
        }
    }
}