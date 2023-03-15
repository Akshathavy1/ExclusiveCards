//using System.Threading.Tasks;
//using AutoMapper;
//using ExclusiveCard.Data.Managers;
//using ST = ExclusiveCard.Data.StagingModels;
//using ExclusiveCard.Services.Interfaces.Admin;

//namespace ExclusiveCard.Services.Admin
//{
//   public class StagingOfferCategoryService : IStagingOfferCategoryService
//    {
//        #region Private Members

//        private readonly IMapper _mapper;
//        private readonly IStagingOfferCategoryManager _offerCategoryManager;

//        #endregion

//        #region Constructor

//        public StagingOfferCategoryService(IMapper mapper, IStagingOfferCategoryManager stagingOfferCategoryManager)
//        {
//            _mapper = mapper;
//            _offerCategoryManager = stagingOfferCategoryManager;
//        }

//        #endregion

//        #region Writes

        
//        public async Task<Models.DTOs.StagingModels.OfferCategory> Add(Models.RequestModels.StagingModels.OfferCategory offerCategory)
//        {
//            ST.OfferCategory req = _mapper.Map<ST.OfferCategory>(offerCategory);
//            return _mapper.Map<Models.DTOs.StagingModels.OfferCategory>(
//               await _offerCategoryManager.Add(req));
//        }

        
//        public async Task<Models.DTOs.StagingModels.OfferCategory> Update(Models.RequestModels.StagingModels.OfferCategory offerCategory)
//        {
//            ST.OfferCategory req = _mapper.Map<ST.OfferCategory>(offerCategory);
//            return _mapper.Map<Models.DTOs.StagingModels.OfferCategory>(
//                await _offerCategoryManager.Update(req));
//        }

//        #endregion
//    }
//}
