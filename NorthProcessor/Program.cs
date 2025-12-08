using System;
using System.Configuration;
using System.IO;
using TransTrack.Logging;

namespace NorthProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== North Warehouse Processor ===");
            Console.WriteLine();
            // Load configuration
            string incomingFolder = ConfigurationManager.AppSettings["IncomingFolder"];
            string processedFolder = ConfigurationManager.AppSettings["ProcessedFolder"];
            string errorsFolder = ConfigurationManager.AppSettings["ErrorsFolder"];
            string logFilePath = ConfigurationManager.AppSettings["LogFilePath"];

            string cloudName = ConfigurationManager.AppSettings["CloudinaryCloudName"];
            string apiKey = ConfigurationManager.AppSettings["CloudinaryApiKey"];
            string apiSecret = ConfigurationManager.AppSettings["CloudinaryApiSecret"];

            // Initialize logger
            ILogger logger = new FileLogger(logFilePath);
            logger.LogInfo("NorthProcessor started");

            try
            {
                var engine = new ProcessorEngine(
                    incomingFolder, processedFolder, errorsFolder,
                    cloudName, apiKey, apiSecret, logger);

                engine.ProcessFiles();

                logger.LogInfo("NorthProcessor completed successfully");
            }
            catch (Exception ex)
            {
                logger.LogError($"NorthProcessor failed: {ex.Message}");
                Console.WriteLine($"Error: {ex.Message}");
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}