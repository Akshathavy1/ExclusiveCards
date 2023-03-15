using ExclusiveCard.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NLog;
using ExclusiveCard.Data.Constants;

namespace ExclusiveCard.Services
{
    public class SFTPProvider
    {
        #region PrivateMembers

        private IConfiguration AppSettings { get; set; }
        private readonly string _host;
        private readonly string _username;
        private readonly string _password;
        private readonly int _port;
        private readonly ILogger _logger;

        #endregion

        #region Initialize

        public SftpClient GetSFTPConnection()
        {
            SftpClient sftpClient = null;
            try
            {
                sftpClient = new SftpClient(_host, _port, _username, _password);
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }
            return sftpClient;
        }

        #endregion

        #region Constructor

        public SFTPProvider()
        {
            AppSettings = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            _host = AppSettings["SFTP_Host"];
            _username = AppSettings["SFTP_UserName"];
            _password = AppSettings["SFTP_Password"];
            _port = Convert.ToInt32(AppSettings["SFTP_Port"]);
            _logger = LogManager.GetCurrentClassLogger();
        }

        #endregion
        
        public bool UploadFile(string destinationFolderPath, Models.DTOs.ExternalFile file)
        {
            bool result = false;
            try
            {
                using (var sftpClient = GetSFTPConnection())
                {
                    sftpClient.Connect();
                    if (sftpClient.IsConnected && !string.IsNullOrEmpty(file.FileName) && file.FileContent.Length > 0)
                    {
                        if (!sftpClient.Exists(destinationFolderPath)) //in folder created for UploadFile
                        {
                            sftpClient.Create(destinationFolderPath);
                        }
                        sftpClient.ChangeDirectory(destinationFolderPath);
                        //sftpClient.BufferSize = 4 * 1024;
                        sftpClient.UploadFile(file.FileContent, file.FileName);
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return result;
        }

        public bool DeleteFile(string destinationFolderPath, string fileName)
        {
            bool result = false;
            try
            {
                using (var sftpClient = GetSFTPConnection())
                {
                    sftpClient.Connect();
                    if (sftpClient.IsConnected && !string.IsNullOrEmpty(fileName))
                    {
                        sftpClient.DeleteFile($"{destinationFolderPath}/{fileName}");
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return result;
        }

        public async Task<List<Models.DTOs.ExternalFile>> GetTAMPositionList(string sourceFolder, ILogger logger = null)
        {
            List<Models.DTOs.ExternalFile> externalFiles = new List<Models.DTOs.ExternalFile>();
            try
            {
                using (var sftpClient = GetSFTPConnection())
                {
                    sftpClient.Connect();
                    var files = sftpClient.ListDirectory(sourceFolder);
                    var currentPath = Path.Combine(
                    Directory.GetCurrentDirectory(), TemporaryFilePath.TempFilePosition);

                    if (!Directory.Exists(currentPath))
                    {
                        Directory.CreateDirectory(currentPath);
                    }
                    foreach (var file in files)
                    {
                        if (!file.IsDirectory && !file.IsSymbolicLink)
                        {
                            using (var newFile = new FileStream(TemporaryFilePath.TempFilePosition + file.Name, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                            {
                                sftpClient.DownloadFile(file.FullName, newFile);
                            }
                            externalFiles.Add(new Models.DTOs.ExternalFile
                            {
                                FileName = currentPath + file.Name
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger?.Error(ex);
            }
            await Task.CompletedTask;

            return externalFiles;
        }

        public bool TestConnection()
        {
            bool connect = false;
            try
            {
                using (var sftpClient = GetSFTPConnection())
                {
                    sftpClient.Connect();
                    connect = true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return connect;
        }
    }
}
