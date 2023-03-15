using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ExclusiveCard.Data.CRUDS;
using ExclusiveCard.Data.Models;
using ExclusiveCard.Providers;
using ExclusiveCard.Services.Interfaces.Public;
using ExclusiveCard.Enums;
using dto = ExclusiveCard.Services.Models.DTOs;
using NLog;
using System.Text;

namespace ExclusiveCard.Services.Public
{
    public class OffersService : IOffersService
    {
        #region Private Members

        private readonly IMapper _mapper;
        private readonly IOfferListManager _manager;
        private readonly IOfferListItemManager _offerListItemManager;
        private readonly IAzureStorageProvider _azureStorageProvider;
        private readonly IOfferManager _offerManager;
        private readonly IOfferTypeManager _offerTypeManager;
        private readonly Managers.IOfferRedemptionManager _offersManager;
        private readonly Managers.IEmailManager _emailManager;

        private readonly ILogger _logger;

        #endregion Private Members

        #region Constructor

        public OffersService(IMapper mapper,
            IAzureStorageProvider azureStorageProvider,
            IOfferManager offerManager,
            Managers.IOfferRedemptionManager offersManager,
            IOfferListManager offerListManager,
            IOfferListItemManager offerListItemManager,
            IOfferTypeManager offerTypeManager,
            Managers.IEmailManager emailService)
        {
            _mapper = mapper;
            _offerManager = offerManager;
            _manager = offerListManager;
            _azureStorageProvider = azureStorageProvider;
            _offersManager = offersManager;
            _offerTypeManager = offerTypeManager;
            _offerListItemManager = offerListItemManager;
            _emailManager = emailService;
            _logger = LogManager.GetCurrentClassLogger();
        }

        #endregion Constructor

        #region Logic

        public async Task<string> CreateLove2ShopRequestFile(string adminEmail, string connectionString, string containerName, string blobFolder)
        {
            var respFile = new dto.Files();
            var statuses = new List<dto.Status>();
            try
            {
                //Get requests
                var data = await GetRedemptionRequestAsync(blobFolder);

                StringBuilder stream = new StringBuilder();
                //Return no data found
                if (data == null || data.Count == 0)
                {
                    _logger.Trace("No requests found to create Love2Shop file.");
                    return "No requests found to create file.";
                }

                //Generate File stream
                _logger.Trace("Generating stream for Love2Shop file.");
                string streamFileName = data.FirstOrDefault()?.FileName;
                GenerateFileDataStream(stream, data);
                _logger.Trace("Generating stream for Love2Shop file succeeded.");

                //Check for the folder
                var currentPath = Path.Combine(
                    Directory.GetCurrentDirectory(), Data.Constants.TemporaryFilePath.TempFileLove2Shop);

                if (!Directory.Exists(currentPath))
                {
                    Directory.CreateDirectory(currentPath);
                }

                if (!Directory.Exists(currentPath))
                {
                    _logger.Trace($"{currentPath} - directory not created");
                }

                _logger.Trace("Writing Love2Shop file on the server.");
                File.WriteAllText(Path.Combine(currentPath, streamFileName), stream.ToString());
                _logger.Trace("Writing Love2Shop file on the server succeeded.");

                _logger.Trace("Writing Love2Shop file to the blob.");
                //write to blob
                //var azureSaveWorked = await _azureStorageProvider.SaveFile(connectionString,
                //    containerName, Data.Constants.TemporaryFilePath.TempFileLove2Shop,
                //    blobFolder, streamFileName, true, _logger);
                var azureSaveWorked = await _azureStorageProvider.SaveFile(connectionString,
                    containerName, currentPath,
                    blobFolder, streamFileName, true, _logger);
                _logger.Trace($"Writing Love2Shop file to the blob succeeded? {azureSaveWorked}");

                if (azureSaveWorked)
                {
                    //string[] files = System.IO.Directory.GetFiles(currentPath);

                    ////Why is this deleting all files in the temp directory, shouldn't it just delete the one it created?
                    //foreach (string s in files)
                    //{
                    //    // Use static Path methods to extract only the file name from the path.
                    //    var fileName = Path.GetFileName(s);
                    //    File.Delete(Path.Combine(currentPath, s));
                    //}
                    File.Delete(Path.Combine(currentPath, streamFileName));
                }

                return azureSaveWorked.ToString(); //true.ToString();
            }
            catch (Exception e)
            {
                _logger.Error(e);

                //email will be sent to admin since we got exception while creating file
                var res = await _emailManager.SendEmailAsync(new dto.Email
                {
                    EmailTo = new List<string>() { adminEmail },
                    Subject = $"Love2Shop Requests File creation failed",
                    BodyHtml = $"Dear Admin, <br/><p>Failed to create Love2Shop request file - {e.StackTrace}",
                    BodyPlainText = $"Dear Admin, Failed to create Love2Shop request file - {e.StackTrace}",
                });
                if (res != true.ToString())
                {
                    _logger.Error("Error sending email when failed to create Love2Shop request file.");
                }
                return e.ToString();
            }
        }

        #endregion Logic

        #region Writes

        public async Task<dto.OfferList> Add(dto.OfferList offerList)
        {
            OfferList list = _mapper.Map<OfferList>(offerList);
            return _mapper.Map<dto.OfferList>(await _manager.Add(list));
        }

        public async Task<dto.OfferList> Update(dto.OfferList offerList)
        {
            OfferList list = _mapper.Map<OfferList>(offerList);
            return _mapper.Map<dto.OfferList>(await _manager.Update(list));
        }

        public dto.OfferRedemption AddRedemption(dto.OfferRedemption redeem)
        {
            return _offersManager.AddRedemption(redeem);
        }

        public dto.OfferRedemption UpdateRedemption(dto.OfferRedemption redeem)
        {
            return _offersManager.UpdateRedemption(redeem);
        }

        public async Task<bool> RevertPickedOfferRedemption(int fileId)
        {
            return await _offersManager.RevertPickedOfferRedemption(fileId);
        }

        #endregion Writes

        #region Reads

        public async Task<dto.Offer> Get(int id, bool includeMerchant, bool includeOfferTypes, bool includeStatus, bool includeAffiliate, bool includeCategories)
        {
            return MapToOfferDto(
                await _offerManager.Get(id, includeMerchant, includeOfferTypes, includeStatus, includeAffiliate, includeCategories),
                includeMerchant, includeOfferTypes, includeStatus, includeAffiliate, includeCategories);
        }

        //As per requirement. Get OfferSummaries based on OfferListName and countryCode
        public async Task<List<dto.Public.OfferSummary>> GetOffersByListName(string listName, string countryCode)
        {
            return await GetOfferSummaries(await _manager.Get(listName, countryCode));
        }

        //As per the requirement, this will return image in byte[]. Could use Provider and get this directly
        public async Task<byte[]> GetImage(string blobConnectionString, string containerName, string path)
        {
            return await _azureStorageProvider.GetImage(blobConnectionString, containerName, path);
        }

        //Get OfferSummaries for search criteria
        public async Task<List<dto.Public.OfferSummary>> Search(dto.OfferSearchCriteria criteria)
        {
            var list = await _offerManager.GetPublicPagedResult(criteria.MerchantId, criteria.OfferType,
                criteria.CountryCode,
                criteria.ValidFrom, criteria.ValidTo, criteria.SortOrder, criteria.Categories, criteria.KeyWord,
                criteria.MerchantName, criteria.OfferTypes, criteria.PageSize, criteria.CurrentPage, criteria.ExcludedMerchantId);
            return GetOfferSummary(list.Results.ToList());
        }

        //Get Paged OfferSummaries for search criteria
        public async Task<dto.PagedResult<dto.Public.OfferSummary>> PagedSearch(dto.OfferSearchCriteria criteria)
        {
            return Map(await _offerManager.GetPublicPagedResult(criteria.MerchantId, criteria.OfferType,
                criteria.CountryCode,
                criteria.ValidFrom, criteria.ValidTo, criteria.SortOrder, criteria.Categories, criteria.KeyWord,
                criteria.MerchantName, criteria.OfferTypes, criteria.PageSize, criteria.CurrentPage,
                criteria.ExcludedMerchantId));
        }

        //Get OfferList and offers based on countryCode
        public async Task<List<dto.OfferList>> GetOffersForCountry(string countryCode)
        {
            return _mapper.Map<List<dto.OfferList>>(await _manager.GetAll(countryCode));
        }

        //Get all OfferLists
        public async Task<List<dto.OfferList>> GetAllOfferLists()
        {
            return _mapper.Map<List<dto.OfferList>>(await _manager.GetAll());
        }

        //To Get offerSummary for offerlistId, countryCode, PageSize
        public async Task<List<dto.Public.OfferSummary>> GetByOfferListCountry(int offerListId, string countryCode, int pageSize, int? merchantId, bool biggerImage = false)
        {
            var data = await _offerListItemManager.GetByOfferListIdForCountry(offerListId, countryCode, pageSize, merchantId);

            return await GetOfferSummaries(data, biggerImage);
        }

        //Get PagedOfferSummary for merchantId, offerType, country, pagesize and currentPage
        public async Task<dto.PagedResult<dto.Public.OfferSummary>> GetMerchantOffersByType(int merchantId, int offerType, string countryCode, int pageSize, int currentPage)
        {
            var resp = await _offerManager.GetPublicPagedResult(merchantId, offerType, countryCode, null, null, null, null, null, null, null, pageSize, currentPage, null);
            return GetPagedOfferSummary(resp);
        }

        //Get list of OfferSummary for merchantId, offerType, country, pagesize and currentPage
        public async Task<List<dto.Public.OfferSummary>> GetListofMerchantOffersByTypeAsync(int? merchantId, int? offerType, string countryCode)
        {
            return GetOfferSummary(await _offerManager.GetPublicListofOffersAsync(
                merchantId, offerType, countryCode, null, null,
                null, null, null, null, null));
        }

        //Get All OfferType
        public async Task<List<dto.OfferType>> GetAllOfferType()
        {
            return MapToOfferTypes(await _offerTypeManager.GetAll());
        }

        //Get PagedOfferSummary for main search terms
        public async Task<dto.PagedResult<dto.Public.OfferSummary>> PagedMainSearch(dto.OfferSearchCriteria criteria)
        {
            var resp = await _offerManager.GetPublicMainSearchResult(
                criteria.OfferId,
                criteria.OfferType,
                criteria.CountryCode,
                criteria.KeyWord,
                criteria.PageSize,
                criteria.CurrentPage,
                criteria.SortOrder,
                criteria.Categories,
                criteria.OfferTypes);

            var mapped = Map(resp);

            return mapped;
        }

        //Get offerType by offerType Name
        public async Task<dto.OfferType> GetOfferType(string offerTypeName)
        {
            return _mapper.Map<dto.OfferType>(await _offerTypeManager.Get(offerTypeName));
        }

        public List<dto.Category> GetParentCategories()
        {
            return _offersManager.GetParentCategories();
        }

        public async Task<List<dto.OfferListDataModel>> GetOfferListDataModels(string countryCode, List<int> categories, int pageSize, string listName)
        {
            var resp = await _offerListItemManager.GetOfferListDataModels(countryCode, categories, pageSize, listName);
            var dtoResult = _mapper.Map<List<dto.OfferListDataModel>>(resp);
            return dtoResult;
        }
        public async Task<List<dto.OfferListDataModel>> GetLocalOfferListDataModels(string countryCode, int regionalId)
        {
            var resp = await _offerListItemManager.GetLocalOfferListDataModels(countryCode, regionalId);
            var dtoResult = _mapper.Map<List<dto.OfferListDataModel>>(resp);
            return dtoResult;
        }

        //Get top 5 OfferList

        public async Task<List<dto.OfferRedemption>> GetOfferRedemptions(int? statusId)
        {
            return await _offersManager.GetOfferRedemptions(statusId);
        }

        public async Task<List<dto.RedemptionDataModel>> GetRedemptionRequestAsync(string blobFolder)
        {
            var resp = await _offersManager.GetRedemptionRequestAsync(blobFolder);
            var dtoResult = _mapper.Map<List<dto.RedemptionDataModel>>(resp);
            return dtoResult;
        }

        public dto.OfferRedemption GetOfferRedemption(int membershipCardId, int offerId)
        {
            return _offersManager.GetOfferRedemption(membershipCardId, offerId);
        }

        public async Task<List<dto.Public.OfferSummary>> GetOfferByKeyword(string keyword)
        {
            return await _offersManager.GetOfferByKeyword(keyword);
        }

        #endregion Reads

        #region Private Methods

        private async Task<List<dto.Public.OfferSummary>> GetOfferSummaries(OfferList list)
        {
            dto.Public.OfferSummary[] result = await Task.WhenAll(list.OfferListItems.Select(async offer =>
            {
                if (offer.Offer?.Merchant != null)
                {
                    return await MapToSummary(offer);
                }
                return null;
            }));

            return result.Where(x => x != null).ToList();
            //return (from item in list.OfferListItems where item.Offer?.Merchant != null select MapToSummary(item)).ToList();
        }

        private async Task<List<dto.Public.OfferSummary>> GetOfferSummaries(List<OfferListItem> list, bool biggerImage = false)
        {
            if (list.Count == 0) return new List<dto.Public.OfferSummary>();


            dto.Public.OfferSummary[] result = await Task.WhenAll(list.Select(async offer =>
            {
                if (offer.Offer?.Merchant != null)
                {
                    return await MapToSummary(offer, biggerImage);
                }
                return null;
            }));

            return result.Where(x => x != null).ToList();
        }

        private async Task<dto.Public.OfferSummary> MapToSummary(OfferListItem item, bool biggerImage = false)
        {
            await Task.CompletedTask;
            var images = item.Offer?.Merchant?.MerchantImages?.ToList();
            string image;
            if (biggerImage)
            {
                image = images?.FirstOrDefault(x => x.ImagePath.Contains("__3"))
                    ?.ImagePath;
            }
            else
            {
                image = item.Offer?.Merchant?.MerchantImages?.FirstOrDefault()
                    ?.ImagePath;
            }

            return new dto.Public.OfferSummary
            {
                OfferId = item.OfferId,
                OfferHeadline = item?.Offer?.Headline,
                OfferShortDescription = item?.Offer?.ShortDescription,
                OfferTerms = item.Offer?.Terms,
                OfferExclusions = item.Offer?.Exclusions,
                OfferCode = item.Offer?.OfferCode,
                MerchantId = item.Offer.Merchant.Id,
                MerchantName = item.Offer?.Merchant?.Name,
                MerchantLogoPath = image,
                DeepLinkAvailable = string.IsNullOrEmpty(item.Offer.LinkUrl),
                OfferTypeDescription = item.Offer?.OfferType?.Description,
                OfferLongDescription = item.Offer?.LongDescription,
                OfferInstructions = item.Offer?.Instructions,
                TimePending = item.Offer.ValidTo.HasValue ? (item.Offer.ValidTo.Value - DateTime.UtcNow).Minutes : 0,
                Rank = item.Offer.SearchRanking,
                RedemptionAccountNumber = item.Offer.RedemptionAccountNumber,
                RedemptionProductCode = item.Offer.RedemptionProductCode,
                IsSSOConfigured = item.Offer.SSOConfigId != null
            };
        }

        private List<dto.Public.OfferSummary> GetOfferSummary(List<Offer> offerpagedResult)
        {
            List<dto.Public.OfferSummary> offerSummaries = new List<dto.Public.OfferSummary>();
            
            foreach (Offer offer in offerpagedResult)
            {
                offerSummaries.Add(MapOfferToSummary(offer));
            }
            return offerSummaries;
        }

        private dto.PagedResult<dto.Public.OfferSummary> GetPagedOfferSummary(PagedResult<Offer> offerPagedResult)
        {
            List<dto.Public.OfferSummary> offerSummaries = new List<dto.Public.OfferSummary>();
            foreach (Offer offer in offerPagedResult.Results)
            {
                offerSummaries.Add(MapOfferToSummary(offer));
            }

            dto.PagedResult<dto.Public.OfferSummary> pagedResult = new dto.PagedResult<dto.Public.OfferSummary>
            {
                Results = offerSummaries,
                PageCount = offerPagedResult.PageCount,
                CurrentPage = offerPagedResult.CurrentPage,
                PageSize = offerPagedResult.PageSize,
                RowCount = offerPagedResult.RowCount
            };

            return pagedResult;
        }

        private dto.Public.OfferSummary MapOfferToSummary(Offer offer)
        {
            List<MerchantImage> images = offer.Merchant?.MerchantImages?.ToList();
            var returnEntity = new dto.Public.OfferSummary
            {
                OfferTypeId = offer.OfferTypeId,
                OfferId = offer.Id,
                OfferShortDescription = offer.ShortDescription,
                OfferHeadline = offer.Headline,
                OfferTerms = offer.Terms,
                OfferExclusions = offer.Exclusions,
                OfferCode = offer.OfferCode,
                MerchantId = offer.MerchantId,
                MerchantName = offer.Merchant?.Name,
                MerchantLogoPath = images?.Where(x => x.ImageType == (int)ImageType.Logo)?.OrderBy(x => x.DisplayOrder)?.FirstOrDefault(x => x.ImagePath.Contains("__2"))?.ImagePath,
                DeepLinkAvailable = !string.IsNullOrEmpty(offer.LinkUrl),
                OfferTypeDescription = offer.OfferType?.Description,
                OfferLongDescription = offer.LongDescription,
                OfferInstructions = offer.Instructions,
                TimePending = offer.ValidTo.HasValue ? (offer.ValidTo.Value - DateTime.UtcNow).Minutes : 0,
                Rank = offer.SearchRanking,
                RedemptionAccountNumber = offer.RedemptionAccountNumber,
                RedemptionProductCode = offer.RedemptionProductCode,
                IsSSOConfigured = offer.SSOConfigId != null
            };
            //If short description matches Long description we need to hide short description
            //if (!string.IsNullOrEmpty(offer.LongDescription) && !string.IsNullOrEmpty(offer.ShortDescription))
            //{
            //    var htmlStripedLongDesription = Regex.Replace(offer.LongDescription, "<.*?>", string.Empty).Trim();
            //    if (string.Equals(offer.ShortDescription, htmlStripedLongDesription,
            //        StringComparison.InvariantCultureIgnoreCase))
            //    {
            //        returnEntity.OfferShortDescription = string.Empty;
            //    }
            //}
            return returnEntity;

        }

        private dto.PagedResult<dto.Public.OfferSummary> Map(PagedResult<Offer> offerPagedResult)
        {
            dto.PagedResult<dto.Public.OfferSummary> result = new dto.PagedResult<dto.Public.OfferSummary>();
            foreach (Offer item in offerPagedResult.Results)
            {
                result.Results.Add(MapOfferToSummary(item));
            }

            result.CurrentPage = offerPagedResult.CurrentPage;
            result.PageCount = offerPagedResult.PageCount;
            result.PageSize = offerPagedResult.PageSize;
            result.RowCount = offerPagedResult.RowCount;
            return result;
        }

        private List<dto.OfferType> MapToOfferTypes(List<OfferType> offerTypes)
        {
            List<dto.OfferType> dtos = new List<dto.OfferType>();
            if (offerTypes == null)
                return null;
            dtos.AddRange(offerTypes.Select(MapToOfferType));
            return dtos;
        }

        private dto.OfferType MapToOfferType(OfferType offerType)
        {
            if (offerType == null)
                return null;
            dto.OfferType resOfferType = new dto.OfferType
            {
                Id = offerType.Id,
                Description = offerType.Description,
                IsActive = offerType.IsActive,
                SearchRanking = offerType.SearchRanking,
                ActionTextLocalisationId = offerType.ActionTextLocalisationId,
                TitleLocalisationId = offerType.TitleLocalisationId
            };
            return resOfferType;
        }

        private void GenerateFileDataStream(StringBuilder fileStream, List<dto.RedemptionDataModel> data)
        {
            try
            {
                //Create Header
                var infos = typeof(dto.RedemptionDataModel).GetProperties();

                for (var i = 1; i < infos.Length; i++)
                {
                    if (i == 1)
                    {
                        fileStream.Append(infos[i].Name);
                    }
                    else
                    {
                        fileStream.Append(",");
                        fileStream.Append(infos[i].Name);
                    }

                    if (i == infos.Length - 1)
                    {
                        fileStream.Append("\r\n");
                    }
                }
                //build data line
                foreach (var rowData in data)
                {
                    fileStream.Append(
                        $"{rowData.AccountNumber},{rowData.CustomerRef},{rowData.Name}," +
                        $"{rowData.Add1},{rowData.Add2},{rowData.Add3},{rowData.Add4}," +
                        $"{rowData.Postcode},{rowData.Country},{rowData.ProductCode}," +
                        $"{rowData.Quantity},{rowData.Value},{rowData.Total},{rowData.ActivationPIN},{rowData.CustomerNotes}\r\n");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private dto.Offer MapToOfferDto(Offer data, bool includeMerchant, bool includeOfferTypes, bool includeStatus, bool includeAffiliate, bool includeCategories)
        {
            if (data == null)
                return null;

            var offerDto = new dto.Offer
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
                RedemptionProductCode = data.RedemptionProductCode,
                SSOConfigId = data.SSOConfigId,
                ProductCode = data.ProductCode
            };

            if (includeMerchant && data.Merchant != null)
            {
                offerDto.Merchant = new dto.Merchant();
                offerDto.Merchant = MapToMerchantDto(data.Merchant);
            }

            if (includeOfferTypes && data.OfferType != null)
            {
                offerDto.OfferType = new dto.OfferType();
                offerDto.OfferType = MapToOfferType(data.OfferType);
            }

            if (includeStatus && data.Status != null)
            {
                offerDto.Status = new dto.Status();
                offerDto.Status = MapToStatusDto(data.Status);
            }

            if (includeAffiliate && data.Affiliate != null)
            {
                offerDto.Affiliate = new dto.Affiliate();
                offerDto.Affiliate = MapToAffiliateDto(data.Affiliate);
            }

            if (includeCategories && data.OfferCategories?.Count > 0)
            {
                offerDto.OfferCategories = new List<dto.OfferCategory>();
                offerDto.OfferCategories = MapOfferCategoryDtoList(data.OfferCategories.ToList());
            }

            return offerDto;
        }

        private dto.Merchant MapToMerchantDto(Merchant data)
        {
            if (data == null)
                return null;
            return new dto.Merchant
            {
                Id = data.Id,
                Name = data.Name,
                ContactDetailsId = data.ContactDetailsId,
                ContactName = data.ContactName,
                ShortDescription = data.ShortDescription,
                LongDescription = data.LongDescription,
                Terms = data.Terms,
                WebsiteUrl = data.WebsiteUrl,
                IsDeleted = data.IsDeleted,
                FeatureImageOfferText = data.FeatureImageOfferText,
                BrandColour = data.BrandColour
            };
        }

        private dto.Status MapToStatusDto(Status data)
        {
            if (data == null)
                return null;

            return new dto.Status
            {
                Id = data.Id,
                Name = data.Name,
                Type = data.Type,
                IsActive = data.IsActive
            };
        }

        private dto.Affiliate MapToAffiliateDto(Affiliate data)
        {
            if (data == null)
                return null;
            return new dto.Affiliate
            {
                Id = data.Id,
                Name = data.Name
            };
        }

        private List<dto.OfferCategory> MapOfferCategoryDtoList(List<OfferCategory> data)
        {
            if (data == null || data.Count == 0)
                return null;

            List<dto.OfferCategory> list = new List<dto.OfferCategory>();

            list.AddRange(data.Select(MapToOfferCategoryDto));

            return list;
        }

        private dto.OfferCategory MapToOfferCategoryDto(OfferCategory data)
        {
            if (data == null)
                return null;

            return new dto.OfferCategory
            {
                OfferId = data.OfferId,
                CategoryId = data.CategoryId
            };
        }

        #endregion Private Methods
    }
}