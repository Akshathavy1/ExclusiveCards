using ExclusiveCard.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

using dto = ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Managers
{
    public interface IPartnerManager
    {
        dto.PartnerDto GetProvider(string Id);

        /// <summary>
        /// Creates a new partner record
        /// </summary>
        /// <param name="partner">the new partner's details</param>
        /// <returns>The newly created partner</returns>
        Task<dto.PartnerDto> CreatePartnerAsync(dto.PartnerDto partner);

        /// <summary>
        /// Updates an existing partner record
        /// </summary>
        /// <param name="partner">the new details</param>
        /// <returns>Has updated</returns>
        Task<bool> UpdatePartnerAsync(dto.PartnerDto partner);

        /// <summary>
        /// Get all partner records
        /// </summary>
        /// <param name="partnerType">partner Type</param>
        /// <returns>list of all partner records</returns>
        Task<List<dto.PartnerDto>> GetAllPartnersAsync(PartnerType partnerType = PartnerType.CardProvider);

        /// <summary>
        /// Get the default partner record
        /// </summary>
        /// <returns>The default partner</returns>
        Task<dto.PartnerDto> GetDefaultPartnerAsync();
    }
}