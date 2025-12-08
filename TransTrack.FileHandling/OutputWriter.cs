using System;
using System.IO;
using TransTrack.Common.Models;

namespace TransTrack.FileHandling
{
    public class OutputWriter
    {
        public void WriteProcessedFile(ProcessingSummary summary, string outputFolder)
        {
            EnsureDirectoryExists(outputFolder);

            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(summary.FileName);
            string extension = Path.GetExtension(summary.FileName);
            string outputFile = Path.Combine(outputFolder, fileNameWithoutExt + "_processed" + extension);

            string content = $@"ProcessedFile: {summary.FileName}
TotalRecords: {summary.TotalRecords}
FileUrl:
{summary.FileUrl}
DateProcessed: {summary.DateProcessed:yyyy-MM-dd HH:mm:ss}";

            File.WriteAllText(outputFile, content);
        }

        public void WriteErrorFile(string fileName, string reason, string errorFolder)
        {
            EnsureDirectoryExists(errorFolder);

            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
            string extension = Path.GetExtension(fileName);
            string errorFile = Path.Combine(errorFolder, fileNameWithoutExt + "_error" + extension);

            string content = $@"FileName: {fileName}
Reason: {reason}
Timestamp: {DateTime.Now:yyyy-MM-dd HH:mm:ss}";

            File.WriteAllText(errorFile, content);
        }

        private void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}