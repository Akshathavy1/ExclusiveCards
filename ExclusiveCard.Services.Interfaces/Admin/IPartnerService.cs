using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExclusiveCard.Services.Interfaces.Admin
{
    public interface IPartnerService
    {
        Task<string> SendPartnerReport(int partnerId, string adminEmail, string inFolder, string blobConnectionString,
            string container, string ftpUrl, string ftpUsername, string ftpPassword);

        Task<string> SendPartnerWithdrawalReport(string adminEmail, string inFolder, string blobConnectionString,
            string container, string ftpUrl, string ftpUsername, string ftpPassword);
        Task<string> ProcessPartnerReport(int partnerId, string adminEmail, string tamFolderOut,
            string blobConnectionString, string partnerContainerName, string blobProcessing, string blobError,
            string blobProcessed, string serverUri, string username, string password);
        Task<string> ProcessPartnerPositionFile(string adminEmail, string blobConnectionString,
            string partnerContainerName, string blobProcessing, string tamFolderPosition, string blobProcessed,
            string blobError, string serverUri, string username, string password);

        #region Writes  

        Task<Models.DTOs.PartnerDto> Add(Models.DTOs.PartnerDto partner);
        Task<Models.DTOs.PartnerDto> Update(Models.DTOs.PartnerDto partner);

        #endregion

        #region Reads

        Task<Models.DTOs.PartnerDto> GetByIdAsync(int id);
        Task<Models.DTOs.PartnerDto> GetByNameAsync(string name);
        Task<List<Models.DTOs.PartnerDto>> GetAllAsync(int? type);
        #endregion
    }
}
