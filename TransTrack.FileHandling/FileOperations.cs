using System;
using System.IO;

namespace TransTrack.FileHandling
{
    public class FileOperations
    {
        public void MoveToProcessed(string sourceFile, string processedFolder)
        {
            EnsureDirectoryExists(processedFolder);
            string fileName = Path.GetFileName(sourceFile);
            string destPath = Path.Combine(processedFolder, fileName);
            File.Move(sourceFile, destPath);
        }

        public void MoveToErrors(string sourceFile, string errorsFolder)
        {
            EnsureDirectoryExists(errorsFolder);
            string fileName = Path.GetFileName(sourceFile);
            string destPath = Path.Combine(errorsFolder, fileName);
            File.Move(sourceFile, destPath);
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