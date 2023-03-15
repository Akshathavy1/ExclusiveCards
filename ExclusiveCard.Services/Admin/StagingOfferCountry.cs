//using System.Threading.Tasks;
//using AutoMapper;
//using ExclusiveCard.Data.Managers;
//using ST = ExclusiveCard.Data.StagingModels;
//using ExclusiveCard.Services.Interfaces.Admin;

//namespace ExclusiveCard.Services.Admin
//{
//   public class StagingOfferCountry : IStagingOfferCountryService
//    {
//        #region Private Members

//        private readonly IMapper _mapper;
//        private readonly IStagingOfferCountryManager _offerCountryManager;

//        #endregion

//        #region Constructor

//        public StagingOfferCountry(IMapper mapper, IStagingOfferCountryManager stagingOfferCountryManager)
//        {
//            _mapper = mapper;
//            _offerCountryManager = stagingOfferCountryManager;
//        }

//        #endregion

//        #region Writes


//        public async Task<Models.DTOs.StagingModels.OfferCountry> Add(Models.RequestModels.StagingModels.OfferCountry offerCountry)
//        {
//            ST.OfferCountry req = _mapper.Map<ST.OfferCountry>(offerCountry);
//            return _mapper.Map<Models.DTOs.StagingModels.OfferCountry>(
//               await _offerCountryManager.Add(req));
//        }


//        public async Task<Models.DTOs.StagingModels.OfferCountry> Update(Models.RequestModels.StagingModels.OfferCountry offerCountry)
//        {
//            ST.OfferCountry req = _mapper.Map<ST.OfferCountry>(offerCountry);
//            return _mapper.Map<Models.DTOs.StagingModels.OfferCountry>(
//                await _offerCountryManager.Update(req));
//        }

//        #endregion
//    }
//}
