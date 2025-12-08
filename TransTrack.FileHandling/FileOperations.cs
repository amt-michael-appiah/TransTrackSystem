using System;
using System.IO;

namespace TransTrack.FileHandling
{
    public class FileOperations
    {
        public void DeleteProcessedFile(string sourceFile)
        {
            try
            {
                if (File.Exists(sourceFile))
                {
                    File.Delete(sourceFile);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to delete file {sourceFile}: {ex.Message}", ex);
            }
        }


        public void DeleteInvalidFile(string sourceFile)
        {
            try
            {
                if (File.Exists(sourceFile))
                {
                    File.Delete(sourceFile);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to delete invalid file {sourceFile}: {ex.Message}", ex);
            }
        }



    }
}