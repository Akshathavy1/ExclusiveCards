using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Services.Interfaces.Admin
{
    public interface IMerchantImageService
    {
        #region Writes

        Task<MerchantImage> Add(MerchantImage merchantImage);

        Task<MerchantImage> Update(MerchantImage merchantImage);

        Task<int> Delete(int merchantId, short? displayOrder);

        Task<int> DeleteByMerchantIdAndType(int merchantId, int type);
        Task<string> DeleteByMerchantImagePath(string path);

        #endregion

        #region Reads

        MerchantImage Get(int merchantId);

        Task<List<MerchantImage>> GetAll(int merchantId, string imageSize, short? displayOrder);

        Task<MerchantImage> GetByIdAsync(int id);

        #endregion
    }
}
