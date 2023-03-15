using System.Collections.Generic;
using System.Threading.Tasks;
using dto = ExclusiveCard.Services.Models.DTOs;
using db = ExclusiveCard.Data.Models;

namespace ExclusiveCard.Managers
{
    public interface IOfferRedemptionManager
    {
        dto.OfferRedemption AddRedemption(dto.OfferRedemption redeem);
        dto.OfferRedemption UpdateRedemption(dto.OfferRedemption redeem);
        Task<bool> RevertPickedOfferRedemption(int fileId);

        List<dto.Category> GetParentCategories();
        Task<List<dto.OfferRedemption>> GetOfferRedemptions(int? statusId);
        Task<List<db.RedemptionDataModel>> GetRedemptionRequestAsync(string blobFolder);
        dto.OfferRedemption GetOfferRedemption(int membershipCardId, int offerId);
        Task<List<dto.Public.OfferSummary>> GetOfferByKeyword(string keyword);
    }
}