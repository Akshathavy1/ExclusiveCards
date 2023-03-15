using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ExclusiveCard.Data.CRUDS;
using ExclusiveCard.Data.Models;
using ExclusiveCard.Services.Interfaces.Admin;
using dto = ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Services.Admin
{
   public class AffiliateService : IAffiliateService
    {
        #region Private members

        private readonly IMapper _mapper;
        private readonly IAffiliateManager _affiliateManager;

        #endregion

        #region Constuctor

        public AffiliateService(IMapper mapper, IAffiliateManager affiliateManager)
        {
            _affiliateManager = affiliateManager;
            _mapper = mapper;
        }

        #endregion

        #region Writes

        public async Task<Models.DTOs.Affiliate> Add(Models.DTOs.Affiliate affiliate)
        {
            Affiliate req = _mapper.Map<Affiliate>(affiliate);
            return _mapper.Map<Models.DTOs.Affiliate>(
                await _affiliateManager.Add(req));
        }

        public async Task<Models.DTOs.Affiliate> Update(Models.DTOs.Affiliate affiliate)
        {
            Affiliate req = _mapper.Map<Affiliate>(affiliate);
            return _mapper.Map<Models.DTOs.Affiliate>(
                await _affiliateManager.Update(req));
        }

        public async Task<Models.DTOs.Affiliate> DeleteAsync(Models.DTOs.Affiliate affiliate)
        {
            Affiliate req = _mapper.Map<Affiliate>(affiliate);
            return _mapper.Map<Models.DTOs.Affiliate>(
                await _affiliateManager.DeleteAsync(req));
        }

        #endregion

        #region Reads

        public async Task<Models.DTOs.Affiliate> Get(int id)
        {
            return _mapper.Map<Models.DTOs.Affiliate>(await _affiliateManager.Get(id));
        }

        public async Task<List<Models.DTOs.Affiliate>> GetAll()
        {
            return await MapToAffiliates(await _affiliateManager.GetAll());
        }

        #endregion

        public async Task<List<dto.Affiliate>> MapToAffiliates(List<Affiliate> affiliates)
        {
            if (affiliates == null || affiliates.Count == 0)
                return null;

            List<dto.Affiliate> dtos = new List<dto.Affiliate>();
            await Task.WhenAll(affiliates.Select(async affiliate => {
                dtos.Add(MapToAffiliate(affiliate));
                await Task.CompletedTask;
            }));
            return dtos;
        }

        private dto.Affiliate MapToAffiliate(Affiliate affiliate)
        {
            if (affiliate == null)
                return null;

            dto.Affiliate model = new dto.Affiliate
            {
                Id = affiliate.Id,
                Name = affiliate.Name,
            };

            if (affiliate.AffiliateFiles != null && affiliate.AffiliateFiles.Count > 0)
            {
                model.AffiliateFiles = new List<dto.AffiliateFile>();
                foreach (var file in affiliate.AffiliateFiles)
                {
                    model.AffiliateFiles.Add(new dto.AffiliateFile
                    {
                        Id = file.Id,
                        AffiliateId = file.AffiliateId,
                        FileName = file.FileName,
                        Description = file.Description,
                        StagingTable = file.StagingTable,
                        AffiliateFileMappingId = file.AffiliateFileMappingId
                    });
                }
            }

            return model;
        }
    }
}
