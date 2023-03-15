using AutoMapper;
using ExclusiveCard.Data.CRUDS;
using ExclusiveCard.Services.Interfaces.Public;

namespace ExclusiveCard.Services.Public
{
    public class MerchantService : IMerchantService
    {
        #region Private Members

        private readonly IMapper _mapper;
        private readonly IMerchantManager _manager;

        #endregion

        #region Constructor

        public MerchantService(IMapper mapper, IMerchantManager merchantManager)
        {
            _mapper = mapper;
            _manager = merchantManager;
        }

        #endregion

        #region Reads

        public Models.DTOs.Merchant Get(int id, bool includeBranch = false, bool includeBranchContact = false, bool includeImage = false, bool includeSocialMedia = false)
        {
            return _mapper.Map<Models.DTOs.Merchant>(_manager.Get(id, includeBranch, includeBranchContact, includeImage, includeSocialMedia));
        }

        #endregion
    }
}
