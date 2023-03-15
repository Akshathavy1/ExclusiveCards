using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ExclusiveCard.Data.CRUDS;
using ExclusiveCard.Data.Models;
using ExclusiveCard.Services.Interfaces.Admin;

namespace ExclusiveCard.Services.Admin
{
    [Obsolete("DO NOT write any new code or add references to this, use Exclusivecard.Services.Admin.MembershipService instead")]
    public class OLD_MembershipRegistrationCodeService: IOLD_MembershipRegistrationCodeService
    {
        #region Private Members

        private readonly IMapper _mapper;
        private readonly IMembershipRegistrationCodeManager _membershipRegistrationCodeManager;

        #endregion

        #region Constructor

        public OLD_MembershipRegistrationCodeService(IMapper mapper, IMembershipRegistrationCodeManager membershipRegistrationCodeManager)
        {
            _mapper = mapper;
            _membershipRegistrationCodeManager = membershipRegistrationCodeManager;
        }

        #endregion

        #region Writes

        //Add Customer
        public async Task<Models.DTOs.MembershipRegistrationCode> Add(Models.DTOs.MembershipRegistrationCode membershipRegistrationCode)
        {
            MembershipRegistrationCode req = _mapper.Map<MembershipRegistrationCode>(membershipRegistrationCode);
            return _mapper.Map<Models.DTOs.MembershipRegistrationCode>(
                await _membershipRegistrationCodeManager.Add(req));
        }

        //Update Customer
        public async Task<Models.DTOs.MembershipRegistrationCode> Update(Models.DTOs.MembershipRegistrationCode membershipRegistrationCode)
        {
            MembershipRegistrationCode req = _mapper.Map<MembershipRegistrationCode>(membershipRegistrationCode);
            return _mapper.Map<Models.DTOs.MembershipRegistrationCode>(
                await _membershipRegistrationCodeManager.Update(req));
        }

        #endregion

        public async Task<Models.DTOs.MembershipRegistrationCode> Get(string code)
        {
            return _mapper.Map<Models.DTOs.MembershipRegistrationCode>(await _membershipRegistrationCodeManager.GetAsync(code));
        }

        public async Task<List<Models.DTOs.MembershipRegistrationCode>> GetAllAsync()
        {
            return MapToDtos(await _membershipRegistrationCodeManager
                .GetAllAsync());
        }

        private List<Models.DTOs.MembershipRegistrationCode> MapToDtos(List<MembershipRegistrationCode> data)
        {
            if (data == null || data?.Count == 0)
                return null;

            List<Models.DTOs.MembershipRegistrationCode> dtos = new List<Models.DTOs.MembershipRegistrationCode>();

            dtos.AddRange(data.Select(MapToDto));

            return dtos;
        }

        private Models.DTOs.MembershipRegistrationCode MapToDto(MembershipRegistrationCode data)
        {
            if (data == null)
                return null;

            return new Models.DTOs.MembershipRegistrationCode
            {
                Id = data.Id,
                MembershipPlanId = data.MembershipPlanId,
                RegistartionCode = data.RegistartionCode,
                ValidFrom = data.ValidFrom,
                ValidTo = data.ValidTo,
                NumberOfCards = data.NumberOfCards,
                EmailHostName = data.EmailHostName,
                IsActive = data.IsActive,
                IsDeleted = data.IsDeleted
            };
        }
    }
}
