using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Services.Models.DTOs.StagingModels;

namespace ExclusiveCard.Services.Interfaces.Admin
{
    public interface IOfferImportAwinService
    {
        Task<OfferImportAwin> Add(OfferImportAwin offerImportAwin);

        Task<OfferImportAwin> Update(OfferImportAwin offerImportAwin);

        Task Delete(List<OfferImportAwin> awins);

        Task<List<OfferImportAwin>> AddToAwinAsync(List<OfferImportAwin> offerImportAwins);

        Task<List<OfferImportAwin>> GetAll(int? importfileId);
    }
}
