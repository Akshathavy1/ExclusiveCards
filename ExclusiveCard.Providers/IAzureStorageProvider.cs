using NLog;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ExclusiveCard.Providers
{
    public interface IAzureStorageProvider
    {
        Task<string> SaveImage(string blobConnectionString, string containerName, string imageCategory, string imageName, byte[] image, bool overWriteExisting = true);

        //List<string> GetImageList(string containerName, string imageCategory);

        Task<byte[]> GetImage(string blobConnectionString, string containerName, string path, ILogger logger = null);

        Task<bool> DeleteImage(string blobConnectionString, string containerName, string path, ILogger logger = null);

        Task<bool> SaveFile(string blobConnectionString, string containerName, string sourcePath, string path, string fileName, bool overWriteExisting = true, ILogger logger = null);

        Task<List<Services.Models.DTOs.ExternalFile>> GetBlobListAsync(string blobConnectionString, string containerName, string path, string prefixFileName = null, ILogger logger = null);

        Task MoveBlobFile(string blobConnectionString, string containerName, string sourceFolder, string destinationFolder, string fileName, ILogger logger = null);

        Task<bool> BlobFileFound(string blobConnectionString, string containerName, string path, ILogger logger = null);

        Task<bool> DownloadFile(string blobConnectionString, string containerName, string sourceFolder,
            string destinationFolder, string fileName, ILogger logger = null);

        /// <summary>
        /// Upload a stream to azure blob storage
        /// </summary>
        /// <param name="blobConnectionString"></param>
        /// <param name="containerName"></param>
        /// <param name="storagePath"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        Task SaveStreamAsync(string blobConnectionString, string containerName, string storagePath, Stream stream);

        /// <summary>
        /// Download an existing stream from azure blob storage annd returns as byte array
        /// </summary>
        /// <param name="blobConnectionString"></param>
        /// <param name="containerName"></param>
        /// <param name="storagePath"></param>
        /// <returns>Downloaded byte array</returns>
        Task<byte[]> GetStreamAsync(string blobConnectionString, string containerName, string storagePath);
    }
}