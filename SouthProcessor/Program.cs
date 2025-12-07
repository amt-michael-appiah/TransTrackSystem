using System;
using System.Configuration;
using System.IO;
using TransTrack.Logging;

namespace SouthProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
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
            logger.LogInfo("SouthProcessor started");

            try
            {
                var engine = new ProcessorEngine(
                    incomingFolder, processedFolder, errorsFolder,
                    cloudName, apiKey, apiSecret, logger);

                engine.ProcessFiles();

                logger.LogInfo("SouthProcessor completed successfully");
            }
            catch (Exception ex)
            {
                logger.LogError($"SouthProcessor failed: {ex.Message}");
                Console.WriteLine($"Error: {ex.Message}");
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}