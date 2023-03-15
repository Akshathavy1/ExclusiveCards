using AutoMapper;
using ExclusiveCard.Data.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

using db = ExclusiveCard.Data.Models;
using dto = ExclusiveCard.Services.Models.DTOs;

using ExclusiveCard.Enums;
using System.Linq;

namespace ExclusiveCard.Managers
{
    public class PartnerManager : IPartnerManager
    {
        #region Private Fields and Constructor

        private readonly IRepository<db.Partner> _partnerRepo;
        private readonly IMapper _mapper;

        public PartnerManager(IRepository<db.Partner> partnerRepo, IMapper mapper)
        {
            _partnerRepo = partnerRepo;
            _mapper = mapper;
        }

        #endregion Private Fields and Constructor

        /// <summary>
        /// Part of the white label partner, customer login validation
        /// This is used to find a partner record with a matching <paramref name="Id"/> in it's AspNetUserId field
        /// </summary>
        /// <param name="Id">The AspNetUserId that's expected to be on the partner record</param>
        /// <returns>A Partner model</returns>
        public dto.PartnerDto GetProvider(string Id)
        {
            dto.PartnerDto dtoPartner = null;

            if (Id != null)
            {
                var dbPartner = _partnerRepo.Get(P => P.AspNetUserId == Id && !P.IsDeleted);

                if (dbPartner != null)
                    dtoPartner = _mapper.Map<dto.PartnerDto>(dbPartner);
            }
            return dtoPartner;
        }

        /// <see cref="IPartnerManager.CreatePartnerAsync(PartnerDto)"/>
        public async Task<dto.PartnerDto> CreatePartnerAsync(dto.PartnerDto partner)
        {
            var dbModel = _mapper.Map<db.Partner>(partner);
            _partnerRepo.Create(dbModel);
            await _partnerRepo.SaveChangesAsync();
            var dtoPartner = _mapper.Map<dto.PartnerDto>(dbModel);
            return dtoPartner;
        }

        /// <see cref="IPartnerManager.UpdatePartnerAsync(dto.PartnerDto)"/>
        public async Task<bool> UpdatePartnerAsync(dto.PartnerDto partner)
        {
            var dbModel = _mapper.Map<db.Partner>(partner);
            _partnerRepo.Update(dbModel);
            var updated = await _partnerRepo.SaveChangesAsync();
            return updated != 0;
        }

        /// <see cref="IPartnerManager.GetAllPartnersAsync(PartnerType)"/>
        public async Task<List<dto.PartnerDto>> GetAllPartnersAsync(PartnerType partnerType = PartnerType.CardProvider)
        {
            var dbPartners = await _partnerRepo.FilterNoTrackAsync(p => p.Type == (int)partnerType && !p.IsDeleted);
            var dtoPartners = _mapper.Map<List<dto.PartnerDto>>(dbPartners);
            return dtoPartners;
        }

        /// <see cref="IPartnerManager.GetDefaultPartnerAsync"/>
        public async Task<dto.PartnerDto> GetDefaultPartnerAsync()
        {
            var dbPartners = await _partnerRepo.FilterNoTrackAsync(p => p.Name == "Exclusive Media"
                                && p.Type == (int)PartnerType.CardProvider && !p.IsDeleted);
            var dtoPartner = _mapper.Map<dto.PartnerDto>(dbPartners.FirstOrDefault());
            return dtoPartner;
        }
    }
}