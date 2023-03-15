using System;
using System.Collections.Generic;

namespace ExclusiveCard.Services.Interfaces
{
    public interface ISFTPProvider
    {
        //SftpClient GetSFTPConnection();
        //bool TestConnection();
        bool UploadFile(string serverUri, string username, string password, string destinationFolderPath,
            Models.DTOs.ExternalFile file, string adminEmail);
        List<Models.DTOs.ExternalFile> GetFilesFromFolder(string serverUri, string username, string password, string localPath, string adminEmail, string partnerName);
        bool DeleteFile(Uri serverUri, string username, string password, string destinationFolderPath, string fileName, string adminEmail);
    }
}
