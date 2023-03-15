using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using ExclusiveCard.Data.CRUDS;
using ExclusiveCard.Data.Models;
using ExclusiveCard.Services.Interfaces.Admin;
using DTOs = ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Services.Admin
{
    public class SocialMediaCompanyService : ISocialMediaCompanyService
    {
        #region Private members

        private readonly IMapper _mapper;
        private readonly ISocialMediaCompanyManager _manager;

        #endregion

        #region Constuctor

        public SocialMediaCompanyService(IMapper mapper, ISocialMediaCompanyManager socialMediaCompanyManager)
        {
            _mapper = mapper;
            _manager = socialMediaCompanyManager;
        }

        #endregion

        #region Writes

        public async Task<Models.DTOs.SocialMediaCompany> Add(DTOs.SocialMediaCompany company)
        {
            SocialMediaCompany req = _mapper.Map<SocialMediaCompany>(company);
            return _mapper.Map<Models.DTOs.SocialMediaCompany>(
                await _manager.Add(req));
        }

        #endregion

        #region Reads

        public async Task<List<Models.DTOs.SocialMediaCompany>> GetAll()
        {
            return ManualMappings.MapSocialMediaCompanies(await _manager.GetAll());
        }

        #endregion
    }
}
