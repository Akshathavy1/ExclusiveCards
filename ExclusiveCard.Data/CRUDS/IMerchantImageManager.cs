using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Data.Models;

namespace ExclusiveCard.Data.CRUDS
{
    public interface IMerchantImageManager
    {
        Task<MerchantImage> Add(MerchantImage merchantImage);
        Task<MerchantImage> Update(MerchantImage merchantImage);
        Task<int> Delete(int merchantId, short? displayOrder);
        Task<int> DeleteByMerchantIdAndType(int merchantId, int type);
        Task<string> DeleteByMerchantImagePath(string path);
        MerchantImage Get(int merchantId);
        Task<List<MerchantImage>> GetAll(int merchantId, string imageSize, short? displayOrder);
        Task<MerchantImage> GetByIdAsync(int id);
    }
}