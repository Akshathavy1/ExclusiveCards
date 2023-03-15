using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;

using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using LogLevel = NLog.LogLevel;

namespace ExclusiveCard.Providers
{
    public class AzureStorageProvider : IAzureStorageProvider
    {
        #region Private Members

        private const string APPSETTINGS_IMAGE_ROOT = "Images:RootFolder";
        private const string NO_IMAGE = "NoImage__2.png";
        private readonly ILogger _logger;

        public AzureStorageProvider()
        {
            _logger = LogManager.GetCurrentClassLogger();
        }

        #endregion Private Members

        #region Public Methods

        /// <see cref="IAzureStorageProvider.SaveStreamAsync(string, string, string, Stream)"/>
        public async Task SaveStreamAsync(string blobConnectionString, string containerName, string storagePath, Stream stream)
        {
            var container = GetBlobContainer(containerName, blobConnectionString);
            var blob = container.GetBlockBlobReference(storagePath);
            await blob.UploadFromStreamAsync(stream);
        }

        /// <see cref="IAzureStorageProvider.GetStreamAsync(string, string, string)"/>
        public async Task<byte[]> GetStreamAsync(string blobConnectionString, string containerName, string storagePath)
        {
            if (!string.IsNullOrEmpty(storagePath))
            {
                try
                {
                    byte[] downloadStream = null;
                    var container = GetBlobContainer(containerName, blobConnectionString);
                    var blob = container.GetBlockBlobReference(storagePath);
                    using (var stream = new MemoryStream())
                    {
                        await blob.DownloadToStreamAsync(stream);
                        downloadStream = stream.ToArray();
                    }
                    return downloadStream;
                }
                catch (Exception ex)
                {
                    _logger?.Error(ex, $"Missing stream: {containerName} - {storagePath}");
                    return null;
                }
            }
            return null;
        }

        public async Task<string> SaveImage(string blobConnectionString, string containerName, string imageCategory, string imageName, byte[] image, bool overWriteExisting = true)
        {
            string blobName = imageCategory + "/" + imageName;

            var container = GetBlobContainer(containerName, blobConnectionString);
            var blob = container.GetBlockBlobReference(blobName);

            AccessCondition condition = null;
            if (!overWriteExisting)
                condition = AccessCondition.GenerateIfNotExistsCondition();

            await blob.UploadFromByteArrayAsync(image, 0, image.Length);//condition

            return blobName;
        }

        //public List<string> GetImageList(int tenantId, string imageCategory)
        //{
        //    List<string> imageNames = new List<string>();

        //    CloudBlobContainer container = GetBlobContainer(tenantId);
        //    var items = container.ListBlobsSegmentedAsync(imageCategory + "/", true);

        //    foreach (var item in items)
        //    {
        //        var itemName = item.Uri.LocalPath.Substring(item.Container.Name.Length + 2);
        //        imageNames.Add(itemName);
        //    }

        //    return imageNames;
        //}

        public async Task<byte[]> GetImage(string blobConnectionString, string containerName, string path,
            ILogger logger = null)
        {
            byte[] image = null;

            if (!string.IsNullOrEmpty(path))
            {
                try
                {
                    var container = GetBlobContainer(containerName, blobConnectionString);
                    var blob = container.GetBlockBlobReference(path);
                    using (var stream = new MemoryStream())
                    {
                        await blob.DownloadToStreamAsync(stream);
                        image = stream.ToArray();
                    }
                }
                catch (Exception ex)
                {
                    logger?.Error(ex, $"Missing image: {containerName} - {path}");
                    return image ?? GetEmptyImage();
                }
            }

            return image ?? GetEmptyImage();
        }

        public async Task<bool> DeleteImage(string blobConnectionString, string containerName, string path, ILogger logger = null)
        {
            try
            {
                var container = GetBlobContainer(containerName, blobConnectionString);
                var blob = container.GetBlockBlobReference(path);
                return await blob.DeleteIfExistsAsync();
            }
            catch (Exception ex)
            {
                logger?.Log(LogLevel.Error, ex,
                    $"Error while deleting image containerName={containerName}, path={path}");
            }

            return false;
        }

        public async Task<bool> SaveFile(string blobConnectionString, string containerName, string sourcePath, string path, string fileName, bool overWriteExisting = true, ILogger logger = null)
        {
            try
            {
                var container = GetBlobContainer(containerName, blobConnectionString);
                var blob = container.GetBlockBlobReference($"{path}/{fileName}");
                AccessCondition condition = null;
                if (!overWriteExisting)
                    condition = AccessCondition.GenerateIfNotExistsCondition();
                await blob.UploadFromFileAsync(sourcePath + fileName);
                return true;
            }
            catch (Exception ex)
            {
                logger?.Log(LogLevel.Error, ex, $"Error while saving Partner file, fileName: {fileName}");
            }
            return false;
        }

        public async Task<List<Services.Models.DTOs.ExternalFile>> GetBlobListAsync(string blobConnectionString, string containerName, string path, string prefixFileName = null, ILogger logger = null)
        {
            var list = new List<IListBlobItem>();
            List<Services.Models.DTOs.ExternalFile> externalFiles = new List<Services.Models.DTOs.ExternalFile>();
            try
            {
                BlobContinuationToken continuationToken = null;
                var container = GetBlobContainer(containerName, blobConnectionString);
                var blob = container.GetDirectoryReference(path);
                var segments = await blob.ListBlobsSegmentedAsync(continuationToken);
                foreach (var item in segments.Results)
                {
                    var cloudblob = (CloudBlockBlob)item;
                    //bool isFileFound = true;
                    //if (!string.IsNullOrEmpty(prefixFileName))
                    //{
                    //    isFileFound = cloudblob.Name.Contains(prefixFileName);
                    //}

                    //if (isFileFound)
                    //{
                    var memStream = new MemoryStream();
                    await cloudblob.DownloadToStreamAsync(memStream).ConfigureAwait(false);
                    Services.Models.DTOs.ExternalFile externalFile = new Services.Models.DTOs.ExternalFile
                    {
                        FileName = Path.GetFileName(cloudblob.Name),
                        FileMemoryContent = memStream
                    };
                    externalFiles.Add(externalFile);
                    list.Add(item);
                    //}
                }
            }
            catch (Exception ex)
            {
                logger?.Log(LogLevel.Error, ex,
                    $"Error while getting files");
            }
            return externalFiles;
        }

        public async Task MoveBlobFile(string blobConnectionString, string containerName, string sourceFolder, string destinationFolder, string fileName, ILogger logger = null)
        {
            try
            {
                var container = GetBlobContainer(containerName, blobConnectionString);
                var blob = container.GetBlockBlobReference($"{sourceFolder}/{fileName}");
                var blob2 = container.GetBlockBlobReference($"{destinationFolder}/{fileName}");
                var exists = await BlobFileFound(blobConnectionString, containerName, $"{sourceFolder}/{fileName}", logger);
                if (exists)
                {
                    await blob2.StartCopyAsync(blob);
                    await blob.DeleteIfExistsAsync();
                }
            }
            catch (Exception ex)
            {
                logger?.Log(LogLevel.Error, ex, $"Error while moving file in azure blob storage. FileName : {fileName} , sourceFolder : {sourceFolder} and destinationFolder : {destinationFolder}");
            }
        }

        public async Task<bool> BlobFileFound(string blobConnectionString, string containerName, string path, ILogger logger = null)
        {
            try
            {
                var container = GetBlobContainer(containerName, blobConnectionString);
                var blob = container.GetBlockBlobReference(path);
                return await blob.ExistsAsync();
            }
            catch (Exception ex)
            {
                logger?.Log(LogLevel.Error, ex, $"Error while checking file in azure blob storage. FilePath : {path}");
            }
            return false;
        }

        public async Task<bool> DownloadFile(string blobConnectionString, string containerName, string sourceFolder, string destinationFolder, string fileName, ILogger logger = null)
        {
            try
            {
                var container = GetBlobContainer(containerName, blobConnectionString);
                var blob = container.GetBlockBlobReference($"{sourceFolder}/{fileName}");
                await blob.DownloadToFileAsync($"{destinationFolder}/{fileName}", FileMode.OpenOrCreate);
                return true;
            }
            catch (Exception e)
            {
                logger?.Error(e);
                return false;
            }
        }

        #endregion Public Methods

        #region Private Methods

        private CloudBlobContainer GetBlobContainer(string containerName, string blobConnectionString)
        {
            var storageAccount = GetStorageAccount(blobConnectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer container = blobClient.GetContainerReference(containerName);
            container.CreateIfNotExistsAsync();

            return container;
        }

        private CloudStorageAccount GetStorageAccount(string blobConnectionString)
        {
            //var setting = CloudConfigurationManager.GetSetting(blobConnectionString);
            string setting = blobConnectionString;
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(setting);
            return storageAccount;
        }

        private byte[] GetEmptyImage()
        {
            return null;
            //string imageRoot = ConfigurationManager.AppSettings[APPSETTINGS_IMAGE_ROOT];
            //if (!imageRoot.EndsWith("\\"))
            //    imageRoot += "\\";
            //string fileName = imageRoot + NO_IMAGE;

            //byte[] file = null;
            //using (FileStream fileStream = System.IO.File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            //{
            //    file = new byte[fileStream.Length];
            //    fileStream.Read(file, 0, (int)fileStream.Length);
            //}
            //return file;
        }

        #endregion Private Methods
    }
}