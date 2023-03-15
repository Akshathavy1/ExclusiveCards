using System.Threading.Tasks;
using AutoMapper;
using ExclusiveCard.Data.CRUDS;
using ExclusiveCard.Data.Models;
using ExclusiveCard.Services.Interfaces.Admin;

namespace ExclusiveCard.Services.Admin
{
    public class AffiliateFileMappingService : IAffiliateFileMappingService
    {
        #region Private members

        private readonly IMapper _mapper;
        private readonly IAffiliateFileMappingManager _affiliateFileMappingManager;

        #endregion

        #region Constuctor

        public AffiliateFileMappingService(IMapper mapper, IAffiliateFileMappingManager affiliateFileMappingManager)
        {
            _affiliateFileMappingManager = affiliateFileMappingManager;
            _mapper = mapper;
        }

        #endregion

        #region Writes

        public async Task<Models.DTOs.AffiliateFileMapping> Add(Models.DTOs.AffiliateFileMapping affiliateFileMapping)
        {
            AffiliateFileMapping req = _mapper.Map<AffiliateFileMapping>(affiliateFileMapping);
            return _mapper.Map<Models.DTOs.AffiliateFileMapping>(
                await _affiliateFileMappingManager.Add(req));
        }

        public async Task<Models.DTOs.AffiliateFileMapping> Update(Models.DTOs.AffiliateFileMapping affiliateFileMapping)
        {
            AffiliateFileMapping req = _mapper.Map<AffiliateFileMapping>(affiliateFileMapping);
            return _mapper.Map<Models.DTOs.AffiliateFileMapping>(
                await _affiliateFileMappingManager.Update(req));
        }

        #endregion

        #region Reads

        //public async Task<Models.DTOs.AffiliateFileMapping> Get(int id)
        //{
        //    return _mapper.Map<Models.DTOs.AffiliateFileMapping>(await _affiliateFileMappingManager.Get(id));
        //}

        #endregion
    }
}
