using System;
using System.Collections.Generic;
using System.IO;
using NLog;
using System.Net;
using System.Text;
using ExclusiveCard.Data.Constants;
using ExclusiveCard.Services.Interfaces;

namespace ExclusiveCard.Providers
{
    public class FTPProvider : ISFTPProvider
    {
        #region Private Members and Constructor

        private readonly ILogger _logger;

        public FTPProvider()
        {
            _logger = LogManager.GetCurrentClassLogger();
        }

        #endregion


        public bool UploadFile(string serverUri, string username, string password,
            string destinationFolderPath, Services.Models.DTOs.ExternalFile file, string adminEmail)
        {
            bool result = false;
            
                //check if directory exists
                //Calling the method:
                string ftpDirectory = $"{serverUri}{destinationFolderPath}/"; //Note: backslash at the last position of the path.
                bool dirExists = DoesFtpDirectoryExist(ftpDirectory, username, password);
                if (!dirExists)
                {
                    //Create Directory
                    WebRequest createReq = WebRequest.Create($"{serverUri}{destinationFolderPath}");
                    createReq.Method = WebRequestMethods.Ftp.MakeDirectory;
                    createReq.Credentials = new NetworkCredential(username.Normalize(), password.Normalize());
                    using (var resp = (FtpWebResponse)createReq.GetResponse())
                    {
                        _logger.Error(resp.StatusCode);
                    }
                }

                // Create a Uri instance with the specified URI string.
                // If the URI is not correctly formed, the Uri constructor
                // will throw an exception.
                Uri target = new Uri($"{serverUri}{destinationFolderPath}/{file.FileName}");
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(target);
                request.EnableSsl = true;
                request.Credentials = new NetworkCredential(username.Normalize(), password.Normalize());
                request.Method = WebRequestMethods.Ftp.UploadFile;

                request.UseBinary = true;

                var currentPath = Path.Combine(
                    Directory.GetCurrentDirectory(), TemporaryFilePath.TempFileIN);

                // Copy the contents of the file to the request stream.
                byte[] fileContents;
                using (StreamReader sourceStream = new StreamReader(Path.Combine(currentPath, file.FileName)))
                {
                    fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
                }

                request.ContentLength = fileContents.Length;

                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(fileContents, 0, fileContents.Length);
                }

                var myFtpWebResponse = (FtpWebResponse)request.GetResponse();

                //_logger.Info("Upload File Complete, status: " + myFtpWebResponse.StatusDescription);

                myFtpWebResponse.Close();
                result = true;
            
            

            return result;
        }

        public bool DeleteFile(Uri serverUri, string username, string password, string destinationFolderPath, string fileName, string adminEmail)
        {
            bool result = false;

            
                if (serverUri.Scheme != Uri.UriSchemeFtp)
                {
                    return false;
                }
                // Get the object used to communicate with the server.
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create($"{serverUri}{destinationFolderPath}/{fileName}");
                request.EnableSsl = true;
                request.Credentials = new NetworkCredential(username.Normalize(), password.Normalize());
                request.Method = WebRequestMethods.Ftp.DeleteFile;

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                //_logger.Error("Delete status: {0}", response.StatusDescription);
                response.Close();
                result = true;
            

            return result;
        }

        public List<Services.Models.DTOs.ExternalFile> GetFilesFromFolder(string serverUri, string username, string password, string localPath, string adminEmail, string partnerName)
        {
            List<Services.Models.DTOs.ExternalFile> externalFiles = new List<Services.Models.DTOs.ExternalFile>();
            
                FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(serverUri);
                ftpRequest.Credentials = new NetworkCredential(username.Normalize(), password.Normalize());
                ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;
                ftpRequest.EnableSsl = true;
                
                FtpWebResponse response = null;
                try
                {
                    response = (FtpWebResponse)ftpRequest.GetResponse();
                }
                catch
                {
                }
                
                if (response == null)
                    return null;

                StreamReader streamReader = new StreamReader(response.GetResponseStream());
                List<string> files = new List<string>();

                string line = streamReader.ReadLine();
                while (!string.IsNullOrEmpty(line))
                {
                    files.Add(line);
                    line = streamReader.ReadLine();
                }
                streamReader.Close();

                //Create local folder if not exists
                if (!Directory.Exists(localPath))
                {
                    Directory.CreateDirectory(localPath);
                }

                if (files?.Count > 0)
                {
                    using (WebClient ftpClient = new WebClient())
                    {
                        ftpClient.Credentials = new NetworkCredential(username, password);

                        foreach (var file in files)
                        {
                            if (file.Contains("."))
                            {
                                string path = $"{serverUri}/{file}";
                                ftpClient.DownloadFile(path, $"{localPath}{file}");
                                externalFiles.Add(new Services.Models.DTOs.ExternalFile
                                {
                                    FileName = file
                                });
                            }
                        }
                    }
                }
                else
                {
                    //STOPPED SENDING email sent for error in processing file.  TAM are not providing these files automatically
                    //var res = _emailService.SendEmail(new Services.Models.RequestModels.Email
                    //{
                    //    EmailTo = new List<string>() { adminEmail },
                    //    Subject = $"No Balance file was found for partner {partnerName}",
                    //    BodyHtml = $"Dear Admin,<br/><p>No Balance files were found for partner {partnerName}</p>",
                    //    BodyPlainText = $"Dear Admin, No Balance files were found for partner {partnerName}",
                    //}).Result;
                    //if (res != true.ToString())
                    //{
                    //    _logger.Error("Error sending email when failed to retrieve files from FTPS.");
                    //}
                }
            
            return externalFiles;
        }

        #region Private Methods

        private bool DoesFtpDirectoryExist(string dirPath, string username, string password)
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(dirPath);
                request.EnableSsl = true;
                request.Credentials = new NetworkCredential(username.Normalize(), password.Normalize());
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                return true;
            }
            catch (WebException)
            {
                return false;
            }
        }

        #endregion
    }
}
