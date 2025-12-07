using System;
using System.IO;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace NorthProcessor
{
    public class CloudinaryUploader
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryUploader(string cloudName, string apiKey, string apiSecret)
        {
            Account account = new Account(cloudName, apiKey, apiSecret);
            _cloudinary = new Cloudinary(account);
        }

        public string UploadFile(string filePath)
        {
            try
            {
                var uploadParams = new RawUploadParams()
                {
                    File = new FileDescription(filePath),
                    PublicId = Path.GetFileNameWithoutExtension(filePath)
                };

                var uploadResult = _cloudinary.Upload(uploadParams);

                if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return uploadResult.SecureUrl.ToString();
                }
                else
                {
                    throw new Exception($"Upload failed: {uploadResult.Error?.Message}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error uploading file to Cloudinary: {ex.Message}", ex);
            }
        }
    }
}