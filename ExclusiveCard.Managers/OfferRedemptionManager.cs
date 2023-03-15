using System;
using ExclusiveCard.Data.Models;
using ExclusiveCard.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using ExclusiveCard.Data.Context;
using NLog;
using dto = ExclusiveCard.Services.Models.DTOs;


namespace ExclusiveCard.Managers
{
    public class OfferRedemptionManager : IOfferRedemptionManager
    {
        #region Private members and constructor

        private readonly IRepository<Category> _categoryRepo;
        private readonly IRepository<OfferRedemption> _redemptionRepo;
        private readonly IRepository<Offer> _offerRepository;
        private readonly ExclusiveContext _context;
        private readonly ILogger _logger;

        public OfferRedemptionManager(IRepository<Category> categoryRepo,
        IRepository<OfferRedemption> redemptionRepo,
        IRepository<Offer> offerRepository,
        ExclusiveContext context)
        {
            _context = context;
            _categoryRepo = categoryRepo;
            _redemptionRepo = redemptionRepo;
            _offerRepository = offerRepository;
            _logger = LogManager.GetCurrentClassLogger();
        }

        #endregion

        #region Writes

        public dto.OfferRedemption AddRedemption(dto.OfferRedemption redeem)
        {
            try
            {
                var req = MapToData(redeem);
                _redemptionRepo.Create(req);
                redeem = MapToDto(req);
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return null;
            }

            return redeem;
        }

        public dto.OfferRedemption UpdateRedemption(dto.OfferRedemption redeem)
        {
            try
            {
                var req = MapToData(redeem);
                _redemptionRepo.Update(req);
                redeem = MapToDto(req);
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return null;
            }

            return redeem;
        }

        public async Task<bool> RevertPickedOfferRedemption(int fileId)
        {
            try
            {
                var data = _redemptionRepo.FilterNoTrackAsync(x => x.FileId == fileId)?.Result?.ToList();
                if ( data?.Count > 0)
                {
                    await Task.WhenAll(data.Select(async item =>
                    {
                        item.FileId = null;
                        item.UpdatedDate = null;
                        _redemptionRepo.Update(item);

                        await Task.CompletedTask;
                    }));
                }

                return true;
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return false;
            }
        }

        #endregion

        #region Reads
        
        //Get Parent Categories for shop by category menu
        public List<dto.Category> GetParentCategories()
        {
            var data = _categoryRepo.IncludeAndThenInclude(x => x.IsActive && x.ParentId == 0,
                x => x.Include(y => y.CategoryFeatureDetails)).ToList();
            if (data.Count == 0)
            {
                return null;
            }

            List<dto.Category> categories = new List<dto.Category>();
            categories.AddRange(data.Select(MapCategoryToDto));
            return categories;
        }

        public async Task<List<dto.OfferRedemption>> GetOfferRedemptions(int? statusId)
        {
            return MapToDtos(await _redemptionRepo.IncludeAndThenInclude(x => statusId.HasValue ? x.State == statusId.Value : x.State > 0,
                    y => y.Include(z => z.Status).Include(z => z.MembershipCard).ThenInclude(a => a.PartnerReward))
                .AsNoTracking().ToListAsync());
        }

        public async Task<List<RedemptionDataModel>> GetRedemptionRequestAsync(string blobFolder)
        {
            List<RedemptionDataModel> data = new List<RedemptionDataModel>();
            try
            {
                string sqlQuery = "EXEC [Exclusive].[SP_GetRedemptionsForFile] @BlobLove2Shop";

                data = await _context.Query<RedemptionDataModel>().FromSql(sqlQuery, new SqlParameter("@BlobLove2Shop", blobFolder)).ToListAsync();
                if(data != null)
                    _logger.Trace($"Data is not null, records: {data.Count()}");
                else _logger.Trace($"Data is null");

            }
            catch (Exception e)
            {
                _logger.Error(e);
            }
            finally
            {
                _context.Database.CloseConnection();
            }
            return data;
        }

        public dto.OfferRedemption GetOfferRedemption(int membershipCardId, int offerId)
        {
            return MapToDto(_redemptionRepo
                .FilterNoTrackAsync(x => x.MembershipCardId == membershipCardId && x.OfferId == offerId)?.Result
                ?.FirstOrDefault());
        }

        //public async Task<dto.Public.OfferSummary> GetOfferByKeyword(string keyword)
        //{
        //    return MapToOfferSummary(await _offerRepository.IncludeAndThenInclude(
        //        x => !x.Merchant.IsDeleted && x.OfferType.Description != Data.Constants.Keys.DiamondOffer &&
        //             (x.Validindefinately || (x.ValidFrom.HasValue && x.ValidFrom <= DateTime.UtcNow &&
        //                                      x.ValidTo.HasValue && x.ValidTo >= DateTime.UtcNow)) &&
        //             (x.ShortDescription.ToLower().Contains(keyword.ToLower())
        //              || x.LongDescription.ToLower().Contains(keyword.ToLower())
        //              || x.Merchant.Name.ToLower().Contains(keyword.ToLower())
        //              || x.OfferTags.Any(t => t.Tag.Tags.ToLower().Contains(keyword.ToLower()))),
        //        x => x.Include(y => y.Merchant)).FirstOrDefaultAsync());
        //}

        public async Task<List<dto.Public.OfferSummary>> GetOfferByKeyword(string keyword)
        {
            return MapToOfferSummaries(await _offerRepository.IncludeAndThenInclude(
                x => !x.Merchant.IsDeleted && x.OfferType.Description != Data.Constants.Keys.DiamondOffer &&
                     (x.Validindefinately || (x.ValidFrom.HasValue && x.ValidFrom <= DateTime.UtcNow && x.ValidTo.HasValue && x.ValidTo >= DateTime.UtcNow)) &&

                     (x.ShortDescription.ToLower().Contains(keyword.ToLower())
                       || x.LongDescription.ToLower().Contains(keyword.ToLower())
                       || x.Merchant.Name.ToLower().Contains(keyword.ToLower())
                       || x.Merchant.ShortDescription.ToLower().Contains(keyword.ToLower())
                       || x.Merchant.LongDescription.ToLower().Contains(keyword.ToLower())
                       || x.Headline.ToLower().Contains(keyword.ToLower())
                       || x.ShortDescription.ToLower().Contains(keyword.ToLower())
                       || x.LongDescription.ToLower().Contains(keyword.ToLower())
                       || x.OfferTags.Any(t => t.Tag.Tags.ToLower().Contains(keyword.ToLower()) )),
                       x => x.Include(y => y.Merchant))
                       .OrderBy(x=>x.SearchRanking)
                       .ThenBy(x => x.OfferType.SearchRanking)
                       .ThenBy(x=>x.ValidTo)
                       .ThenBy(x => x.ValidFrom)
                       .ThenBy(x => x.Datecreated).Take(5).ToListAsync());
        }

        #endregion

        #region Private Methods

        private dto.Category MapCategoryToDto(Category category)
        {
            if (category == null)
                return null;

            var cat = new dto.Category
            {
                Id = category.Id,
                Name = category.Name,
                Code = category.Code,
                ParentId = category.ParentId,
                IsActive = category.IsActive,
                DisplayOrder = category.DisplayOrder,
                UrlSlug = category.UrlSlug
            };

            if (category.CategoryFeatureDetails?.Count > 0)
            {
                cat.CategoryFeatureDetails = new List<dto.CategoryFeatureDetail>();
                foreach (var item in category.CategoryFeatureDetails)
                {
                    cat.CategoryFeatureDetails.Add(
                        new dto.CategoryFeatureDetail
                        {
                            CategoryId = item.CategoryId,
                            CountryCode = item.CountryCode,
                            FeatureMerchantId = item.FeatureMerchantId,
                            SelectedImage = item.SelectedImage,
                            UnselectedImage = item.UnselectedImage
                        });
                }
            }

            return cat;
        }

        private OfferRedemption MapToData(dto.OfferRedemption dto)
        {
            return new OfferRedemption
            {
                MembershipCardId = dto.MembershipCardId,
                OfferId = dto.OfferId,
                State = dto.State,
                FileId = dto.FileId,
                CreatedDate = dto.CreatedDate,
                UpdatedDate = dto.UpdatedDate,
                CustomerRef = dto.CustomerRef
            };
        }

        private dto.OfferRedemption MapToDto(OfferRedemption data)
        {
            if (data == null)
                return null;

            var dto = new dto.OfferRedemption
            {
                MembershipCardId = data.MembershipCardId,
                OfferId = data.OfferId,
                State = data.State,
                FileId = data.FileId,
                CreatedDate = data.CreatedDate,
                UpdatedDate = data.UpdatedDate,
                CustomerRef = data.CustomerRef
            };

            if (data.MembershipCard != null)
            {
                dto.MembershipCard = new dto.MembershipCard
                {
                    Id = data.MembershipCard.Id,
                    CustomerId = data.MembershipCard.CustomerId,
                    MembershipPlanId = data.MembershipCard.MembershipPlanId,
                    CardNumber = data.MembershipCard.CardNumber,
                    ValidFrom = data.MembershipCard.ValidFrom,
                    ValidTo = data.MembershipCard.ValidTo,
                    DateIssued = data.MembershipCard.DateIssued,
                    StatusId = data.MembershipCard.StatusId,
                    PhysicalCardRequested = data.MembershipCard.PhysicalCardRequested,
                    CustomerPaymentProviderId = data.MembershipCard.CustomerPaymentProviderId,
                    IsActive = data.MembershipCard.IsActive,
                    IsDeleted = data.MembershipCard.IsDeleted,
                    PhysicalCardStatusId = data.MembershipCard.PhysicalCardStatusId,
                    RegistrationCode = data.MembershipCard.RegistrationCode,
                    PartnerRewardId = data.MembershipCard.PartnerRewardId,
                    TermsConditionsId = data.MembershipCard.TermsConditionsId
                };
                if (data.MembershipCard.PartnerReward != null)
                {
                    dto.MembershipCard.PartnerReward = new dto.PartnerRewards
                    {
                        Id = data.MembershipCard.PartnerReward.Id,
                        RewardKey = data.MembershipCard.PartnerReward.RewardKey,
                        PartnerId = data.MembershipCard.PartnerReward.PartnerId,
                        CreatedDate = data.MembershipCard.PartnerReward.CreatedDate,
                        LatestValue = data.MembershipCard.PartnerReward.LatestValue,
                        ValueDate = data.MembershipCard.PartnerReward.ValueDate,
                        TotalConfirmedWithdrawn = data.MembershipCard.PartnerReward.TotalConfirmedWithdrawn
                    };
                }
            }

            if (data.Offer != null)
            {
                dto.Offer = new dto.Offer
                {
                    Id = data.Offer.Id,
                    MerchantId = data.Offer.MerchantId,
                    AffiliateId = data.Offer.AffiliateId,
                    OfferTypeId = data.Offer.OfferTypeId,
                    StatusId = data.Offer.StatusId,
                    ValidFrom = data.Offer.ValidFrom,
                    ValidTo = data.Offer.ValidTo,
                    Validindefinately = data.Offer.Validindefinately,
                    ShortDescription = data.Offer.ShortDescription,
                    LongDescription = data.Offer.LongDescription,
                    Instructions = data.Offer.Instructions,
                    Terms = data.Offer.Terms,
                    Exclusions = data.Offer.Exclusions,
                    LinkUrl = data.Offer.LinkUrl,
                    OfferCode = data.Offer.OfferCode,
                    Reoccuring = data.Offer.Reoccuring,
                    SearchRanking = data.Offer.SearchRanking,
                    Datecreated = data.Offer.Datecreated,
                    Headline = data.Offer.Headline,
                    AffiliateReference = data.Offer.AffiliateReference,
                    DateUpdated = data.Offer.DateUpdated,
                    RedemptionAccountNumber = data.Offer.RedemptionAccountNumber,
                    RedemptionProductCode = data.Offer.RedemptionProductCode,
                    
                };
            }

            if (data.Status != null)
            {
                dto.Status = new dto.Status
                {
                    Id = data.Status.Id,
                    Name = data.Status.Name,
                    Type = data.Status.Type,
                    IsActive = data.Status.IsActive
                };
            }

            if (data.File != null)
            {
                dto.File = new dto.Files
                {
                    Id = data.File.Id,
                    Name = data.File.Name,
                    PartnerId = data.File.PartnerId,
                    Type = data.File.Type,
                    StatusId = data.File.StatusId,
                    PaymentStatusId = data.File.PaymentStatusId,
                    TotalAmount = data.File.TotalAmount,
                    CreatedDate = data.File.CreatedDate,
                    ChangedDate = data.File.ChangedDate,
                    PaidDate = data.File.PaidDate,
                    UpdatedBy = data.File.UpdatedBy,
                    ConfirmedAmount = data.File.ConfirmedAmount
                };
            }

            return dto;
        }

        private List<dto.OfferRedemption> MapToDtos(List<OfferRedemption> data)
        {
            if (data == null || data.Count == 0)
                return null;

            List<dto.OfferRedemption> dtos = new List<dto.OfferRedemption>();

            dtos.AddRange(data.Select(MapToDto));

            return dtos;
        }

        private dto.Offer MapToOfferDto(Offer data)
        {
            if (data == null)
                return null;

            var offer = new dto.Offer
            {
                Id = data.Id,
                MerchantId = data.MerchantId,
                AffiliateId = data.AffiliateId,
                OfferTypeId = data.OfferTypeId,
                StatusId = data.StatusId,
                ValidFrom = data.ValidFrom,
                ValidTo = data.ValidTo,
                Validindefinately = data.Validindefinately,
                ShortDescription = data.ShortDescription,
                LongDescription = data.LongDescription,
                Instructions = data.Instructions,
                Terms = data.Terms,
                Exclusions = data.Exclusions,
                LinkUrl = data.LinkUrl,
                OfferCode = data.OfferCode,
                Reoccuring = data.Reoccuring,
                SearchRanking = data.SearchRanking,
                Datecreated = data.Datecreated,
                Headline = data.Headline,
                AffiliateReference = data.AffiliateReference,
                DateUpdated = data.DateUpdated,
                RedemptionAccountNumber = data.RedemptionAccountNumber,
                RedemptionProductCode = data.RedemptionProductCode
            };

            if (data.Merchant != null)
            {
                offer.Merchant = new dto.Merchant
                {
                    Id = data.Merchant.Id,
                    Name = data.Merchant.Name,
                    ContactDetailsId = data.Merchant.ContactDetailsId,
                    ContactName = data.Merchant.ContactName,
                    ShortDescription = data.Merchant.ShortDescription,
                    LongDescription = data.Merchant.LongDescription,
                    Terms = data.Merchant.Terms,
                    WebsiteUrl = data.Merchant.WebsiteUrl,
                    IsDeleted = data.Merchant.IsDeleted,
                    FeatureImageOfferText = data.Merchant.FeatureImageOfferText,
                    BrandColour = data.Merchant.BrandColour
                };
            }

            return offer;
        }

        private List<dto.Public.OfferSummary> MapToOfferSummaries(List<Offer> data)
        {
            List<dto.Public.OfferSummary> summary = new List<dto.Public.OfferSummary>();

            if (data != null && data.Count > 0)
            {
                summary.AddRange(data.Select(MapToOfferSummary));
            }

            return summary;
        }

        private dto.Public.OfferSummary MapToOfferSummary(Offer data)
        {
            if (data == null)
                return null;
            return new dto.Public.OfferSummary
            {
                OfferTypeId = data.OfferTypeId,
                OfferId = data.Id,
                OfferHeadline = data.Headline,
                OfferShortDescription = data.ShortDescription,
                OfferLongDescription = data.LongDescription,
                OfferTerms = data.Terms,
                OfferInstructions = data.Instructions,
                OfferExclusions = data.Exclusions,
                OfferCode = data.OfferCode,
                MerchantId = data.MerchantId,
                MerchantName = data.Merchant?.Name,
                DeepLinkAvailable = !string.IsNullOrEmpty(data.LinkUrl),
                Rank = data.SearchRanking
            };
        }

        #endregion

        public static DbCommand WithSqlParam(DbCommand cmd, string paramName, object paramValue)
        {
            if (string.IsNullOrEmpty(cmd.CommandText))
                throw new InvalidOperationException("Call LoadStoredProc before using this method");

            var param = cmd.CreateParameter();
            param.ParameterName = paramName;
            param.Value = paramValue;
            cmd.Parameters.Add(param);
            return cmd;
        }
    }
}
