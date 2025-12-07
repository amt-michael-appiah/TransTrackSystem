using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using NorthProcessor;
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

                // Validate
                ValidationResult validation = _validator.Validate(records);

                if (validation.IsValid)
                {
                    // Upload to Cloudinary
                    string fileUrl = _uploader.UploadFile(filePath);

                    // Write processed output
                    var summary = new ProcessingSummary
                    {
                        FileName = fileName,
                        TotalRecords = records.Count,
                        FileUrl = fileUrl,
                        DateProcessed = DateTime.Now,
                        IsValid = true
                    };

                    _outputWriter.WriteProcessedFile(summary, _processedFolder);

                    // Move to processed
                    _fileOps.MoveToProcessed(filePath, _processedFolder);

                    _logger.LogInfo($"File processed successfully: {fileName}");
                    Console.WriteLine($"✓ Processed: {fileName}");
                }
                else
                {
                    // Write error file
                    _outputWriter.WriteErrorFile(fileName, validation.ErrorMessage, _errorsFolder);

                    // Move to errors
                    _fileOps.MoveToErrors(filePath, _errorsFolder);

                    _logger.LogWarning($"File validation failed: {fileName} - {validation.ErrorMessage}");
                    Console.WriteLine($"✗ Invalid: {fileName} - {validation.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing {fileName}: {ex.Message}");
                Console.WriteLine($"✗ Error: {fileName} - {ex.Message}");
            }
        }
    }
}
