using AutoMapper;
using ExclusiveCard.Data.Constants;
using ExclusiveCard.Enums;
using DTOs = ExclusiveCard.Services.Models.DTOs;
using ExclusiveCard.Services.Interfaces.Admin;
using ExclusiveCard.Services.Models.DTOs.Admin;
using ExclusiveCard.WebAdmin.Helpers;
using ExclusiveCard.WebAdmin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ExclusiveCard.Data.Models;
using ExclusiveCard.Services.Interfaces;

namespace ExclusiveCard.WebAdmin.Controllers
{
    [Authorize(Roles = "AdminUser, BackOfficeUser")]
    [SessionTimeout]
    public class OfferController : BaseController
    {
        #region Private Members

        private readonly IMemoryCache _cache;
        private readonly IMerchantService _merchantService;
        private readonly IStatusServices _statusServices;
        private readonly IOfferService _offerService;
        private readonly IOfferTypeService _offertypeService;
        private readonly IOfferTagService _offerTagService;
        private readonly IOfferCountryService _offerCountryService;
        private readonly IOfferCategoryService _offerCategoryService;
        private readonly ITagService _tagService;

        //private readonly IOfferImportFileService _OLDofferImportService;
        private readonly IOfferImportService _newOfferImportService;

        private readonly IAffiliateService _affiliateService;
        private readonly IAffiliateFileService _affiliateFileService;
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;
        private readonly IOptions<TypedAppSettings> _settings;
        private readonly IMerchantBranchService _merchantBranchService;
        private readonly Services.Interfaces.Public.IOfferBranchServices _offerBranchServices;
        private readonly ISSOService _ssoService;

        #endregion Private Members

        #region Constructor

        public OfferController(IMerchantService merchantService, IStatusServices statusServices,
            IOfferTypeService offerTypeService, IOfferService offerService,
            IOfferTagService offerTagService, IOfferCountryService offerCountryService, IOfferCategoryService offerCategoryService,
            ITagService tagService,
            //IOfferImportFileService OLDofferImportService,
            IOfferImportService newOfferImportService,
            IAffiliateService affiliateService,
            IAffiliateFileService affiliateFileService, IMapper mapper, IMemoryCache cache,
            ICategoryService categoryService, IOptions<TypedAppSettings> settings, IMerchantBranchService merchantBranchService, Services.Interfaces.Public.IOfferBranchServices offerBranchServices,
            ISSOService ssoservice)
        {
            _merchantService = merchantService;
            _statusServices = statusServices;
            _offertypeService = offerTypeService;
            _offerService = offerService;
            _offerTagService = offerTagService;
            _offerCategoryService = offerCategoryService;
            _offerCountryService = offerCountryService;
            _tagService = tagService;
            //_OLDofferImportService = OLDofferImportService;
            _newOfferImportService = newOfferImportService;
            _affiliateService = affiliateService;
            _affiliateFileService = affiliateFileService;
            _mapper = mapper;
            _cache = cache;
            _categoryService = categoryService;
            _settings = settings;
            _merchantBranchService = merchantBranchService;
            _offerBranchServices = offerBranchServices;
            _ssoService = ssoservice;
        }

        #endregion Constructor

        [HttpGet]
        [ActionName("Index")]
        public async Task<IActionResult> Index()
        {
            try
            {
                OfferListRequest req = new OfferListRequest();
                OfferSearchViewModel searchCriteria = new OfferSearchViewModel();
                OfferSearchViewModel offerSearchViewModel = new OfferSearchViewModel();
                HttpContext?.Session?.SetString(Keys.Keys.SessionCustomerSearch, string.Empty);

                await MapToOfferSearchViewModel(offerSearchViewModel, searchCriteria, req, false);

                return View("Search", offerSearchViewModel);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return View("Error");
            }
        }

        [HttpGet]
        [ActionName("Search")]
        public async Task<IActionResult> Search(int? merchantId, int? affiliateId, int? typeId, int? statusId,
            string keyword, DateTime? validFrom, DateTime? validTo, int page, string sortField, string sortDirection)
        {
            try
            {
                OfferSearchViewModel searchCriteria = new OfferSearchViewModel
                {
                    MerchantId = merchantId,
                    AffiliateId = affiliateId,
                    OfferType = typeId,
                    OfferStatus = statusId,
                    Keyword = keyword,
                    ValidFrom = validFrom,
                    ValidTo = validTo,
                    PageNumber = page
                };
                if (searchCriteria.ValidTo.HasValue)
                {
                    searchCriteria.ValidTo = searchCriteria.ValidTo.Value.AddDays(1);
                }
                OfferListRequest req = new OfferListRequest
                {
                    SortField = (OfferSortField)Enum.Parse(typeof(OfferSortField), sortField, true),
                    SortDirection = sortDirection
                };
                await MapToOfferSearchViewModel(searchCriteria, searchCriteria, req, true);
                return View("Search", searchCriteria);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(JsonResponse<string>.ErrorResponse("Some error occurred. Please try again."));
            }
        }

        [HttpGet]
        [ActionName("Add")]
        public async Task<IActionResult> Add(int id = 0)
        {
            try
            {
                OfferViewModel offerViewModel = await MapToOfferViewModel(id);
                ViewData["Categories"] = JsonConvert.SerializeObject(offerViewModel.ListofCategory, Formatting.None);
                ViewData["Tags"] = JsonConvert.SerializeObject(offerViewModel.TagLists, Formatting.None);
                ViewData["Merchants"] = JsonConvert.SerializeObject(offerViewModel.ListofMerchants, Formatting.None);
                ViewData["ThirdPartySites"] = JsonConvert.SerializeObject(offerViewModel.ListofSSOThirdPartySites, Formatting.None);
                ViewBag.Name = TypeOfRequest.Add.ToString();
                return View("AddEditOffer", offerViewModel);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return View("Error");
            }
        }

        [HttpGet]
        [ActionName("Edit")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                OfferViewModel offerViewModel = await MapToOfferViewModel(id);
                ViewData["Categories"] = JsonConvert.SerializeObject(offerViewModel.ListofCategory, Formatting.None);
                ViewData["Tags"] = JsonConvert.SerializeObject(offerViewModel.TagLists, Formatting.None);
                ViewData["Merchants"] = JsonConvert.SerializeObject(offerViewModel.ListofMerchants, Formatting.None);
                ViewData["ThirdPartySites"] = JsonConvert.SerializeObject(offerViewModel.ListofSSOThirdPartySites, Formatting.None);
                ViewBag.Name = TypeOfRequest.Edit.ToString();
                return View("AddEditOffer", offerViewModel);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return View("Error");
            }
        }

        [HttpPost]
        [ActionName("Save")]
        public async Task<IActionResult> Save(OfferViewModel request)
        {
            // Method to Save Offer Details
            try
            {
                if (request.ValidFrom > request.ValidTo)
                {
                    return Json(JsonResponse<string>.ErrorResponse("Always To date must greater then or equal to From date"));
                }
                /*In future, if you are required to stay back in the same screen,
                 please return the Id instead of bool and assign it in the UI*/
                //Map to request model from Offer View Model and save
                DTOs.Offer response = new DTOs.Offer();
                DTOs.Offer offer = new DTOs.Offer
                {
                    Id = request.Id,
                    MerchantId = request.MerchantId,
                    AffiliateId = request.AffiliateId,
                    OfferTypeId = request.OfferTypeId,
                    StatusId = request.StatusId,
                    Validindefinately = request.ValidIndefintely,
                    ShortDescription = request.ShortDescription,
                    LongDescription = request.LongDescription,
                    Instructions = request.Instructions,
                    Terms = request.Terms,
                    Exclusions = request.Exclusions,
                    LinkUrl = request.LinkURL,
                    OfferCode = request.OfferCode,
                    Reoccuring = request.Reoccuring,
                    SearchRanking = request.SearchRanking,
                    Headline = request.Headline,
                    AffiliateReference = request.AffiliateReference,
                    RedemptionAccountNumber = request.RedemptionAccountNumber,
                    RedemptionProductCode = request.RedemptionProductCode,
                    SSOConfigId = request.SSOThirdpartySiteId,
                    ProductCode = request.ProductCode
                };
                if (request.ValidFrom.HasValue)
                {
                    offer.ValidFrom = TimeZoneInfo.ConvertTimeToUtc(request.ValidFrom.Value, TimeZoneInfo.Local);
                    double diff = (request.ValidFrom.Value - offer.ValidFrom.Value).TotalDays;
                    if (diff > 0)
                    {
                        offer.ValidFrom = offer.ValidFrom.Value.AddDays(diff);
                    }
                }
                if (request.ValidTo.HasValue)
                {
                    offer.ValidTo = TimeZoneInfo.ConvertTimeToUtc(request.ValidTo.Value, TimeZoneInfo.Local);
                    double diff = (request.ValidTo.Value - offer.ValidTo.Value).TotalDays;
                    if (diff > 0)
                    {
                        offer.ValidTo = offer.ValidTo.Value.AddDays(diff);
                    }
                }

                if (request.Id == 0)
                {
                    offer.Datecreated = DateTime.Now;
                    response = await _offerService.Add(offer);
                }
                else
                {
                    offer.Id = request.Id;
                    offer.Datecreated = request.DateAdded;
                    response = await _offerService.Update(offer);
                    if (response != null)
                    {
                        await _offerCategoryService.Delete(request.Id);
                        await _offerCountryService.Delete(request.Id);
                        await _offerTagService.Delete(request.Id);
                        await _offerService.DeleteOfferBranch(request.Id);
                    }
                }
                if (response != null)
                {
                    foreach (CategoryTreeView categoryList in request.ListofCategory)
                    {
                        DTOs.OfferCategory category = new DTOs.OfferCategory
                        {
                            OfferId = response.Id,
                            CategoryId = categoryList.Id
                        };
                        await _offerCategoryService.Add(category);
                    }

                    foreach (CustomCountryList countryList in request.ListofCountries)
                    {
                        DTOs.OfferCountry offerCountry = new DTOs.OfferCountry
                        {
                            OfferId = response.Id,
                            CountryCode = countryList.Name,
                            IsActive = countryList.Accepted
                        };
                        await _offerCountryService.Add(offerCountry);
                    }

                    foreach (CustomTagList tagList in request.ListofTag)
                    {
                        if (tagList.Id == 0)
                        {
                            DTOs.Tag tag = new DTOs.Tag
                            {
                                Tags = tagList.Tags,
                                TagType = TypeOfTag.Offer.ToString(),
                                IsActive = true
                            };
                            DTOs.Tag responseTag = await _tagService.Add(tag);
                            DTOs.OfferTag offerTag = new DTOs.OfferTag
                            {
                                OfferId = response.Id,
                                TagId = responseTag.Id
                            };
                            await _offerTagService.Add(offerTag);
                        }
                        else
                        {
                            DTOs.OfferTag offerTag = new DTOs.OfferTag
                            {
                                OfferId = response.Id,
                                TagId = tagList.Id
                            };
                            await _offerTagService.Add(offerTag);
                        }
                    }

                    foreach (SelectListItem offerBranch in request.ListofBranches)
                    {
                        DTOs.OfferMerchantBranch branch = new DTOs.OfferMerchantBranch
                        {
                            OfferId = response.Id,
                            MerchantBranchId = int.Parse(offerBranch.Value)
                        };
                        await _offerService.AddOfferMerchantBranch(branch);
                    }
                }
                return Json(JsonResponse<bool>.SuccessResponse(true));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(JsonResponse<string>.ErrorResponse("Error Saving Offer"));
            }
        }

        /// <summary>
        /// Displays the screen when user selects Import from the Offers menu
        /// This provides option to choose which affiliate and filetype they wish to use,
        /// and a button to choose a file to upload.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ActionName("ImportFile")]
        public async Task<ActionResult> ImportFile()
        {
            FileImportModel model = !string.IsNullOrEmpty(HttpContext.Session.GetString(Keys.Keys.SessionFileImport))
                ? JsonConvert.DeserializeObject<FileImportModel>(HttpContext.Session.GetString(Keys.Keys.SessionFileImport))
                : new FileImportModel();
            if (model?.AffiliateId > 0)
            {
                return RedirectToAction("GetImport",
                     new
                     {
                         affiliateId = model.AffiliateId,
                         fileId = model.FileTypeId,
                         countryCode = model.ImportCountryCode
                     });
            }
            await InitializeFileImport(_affiliateService, model);
            ViewData["Affiliates"] = JsonConvert.SerializeObject(model?.Affiliates, Formatting.None);

            return View("Import", model);
        }

        [HttpGet]
        [ActionName("GetImport")]
        public async Task<ActionResult> GetImport(int affiliateId, int fileId, string countryCode)
        {
            FileImportModel model = new FileImportModel
            {
                AffiliateId = affiliateId,
                FileTypeId = fileId,
                ImportCountryCode = countryCode
            };

            await GetAndMapOfferImports(model);

            ViewData["Affiliates"] = JsonConvert.SerializeObject(model.Affiliates, Formatting.None);
            return View("Import", model);
        }

        /// <summary>
        /// 1)
        /// This method is fired when user clicks the Import button on the Offers - Import page
        /// It saves the imported file to a temp folder on the webserver and creates a record
        /// in the Staging.OfferImportFile table with status of new.
        /// It then copies the records from the file into the staging table
        /// and finally copies valid staging offers into the offers table
        /// Fired by the ImportFile Function in Offer.js (662)
        /// </summary>
        /// <param name="affiliateId">Affiliate selected from the Affiliates dropdown</param>
        /// <param name="fileId">This should be named FileTypeId and is the value from the fileType dropdown. It has a cunning disguise of FileId to confuse everyone </param>
        /// <param name="file">The file to be imported - uploaded by the browser from the user's PC and then received as postdata to this method call</param>
        /// <param name="countryCode"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("SaveFile")]
        public async Task<ActionResult> SaveFile(int affiliateId, int fileId, IFormFile file, string countryCode)
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString(Keys.Keys.SessionFileImport)))
            {
                HttpContext.Session.SetString(Keys.Keys.SessionFileImport, string.Empty);
            }
            FileImportModel model = new FileImportModel
            {
                AffiliateId = affiliateId,
                FileTypeId = fileId,
                //FileSelected = file,
                ImportCountryCode = countryCode
            };
            var fileImportView = JsonConvert.SerializeObject(model);
            HttpContext?.Session?.SetString(Keys.Keys.SessionFileImport, fileImportView);
            model.FileSelected = file;
            try
            {
                if (model.FileSelected != null && model.FileSelected.Length > 0)
                {
                    string path = Path.Combine(Directory.GetCurrentDirectory(), @"Affiliate\");

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    model.File = Path.Combine(path, model.FileSelected.FileName);
                    model.ErrorFilePath = Path.Combine(path, $"Error_ {model.FileSelected.FileName}");

                    // Copies the file to a temp location on the web server
                    using (FileStream stream = new FileStream(model.File, FileMode.Create))
                    {
                        await model.FileSelected.CopyToAsync(stream);
                    }

                    DTOs.StagingModels.OfferImportFile request = new DTOs.StagingModels.OfferImportFile()
                    {
                        AffiliateFileId = model.FileTypeId,
                        DateImported = DateTime.UtcNow,
                        FilePath = model.File,
                        ErrorFilePath = model.ErrorFilePath,
                        ImportStatus = (int)Enums.Import.New,
                        Staged = 0,
                        TotalRecords = 0,
                        Failed = 0,
                        Imported = 0,
                        Duplicates = 0,
                        Updates = 0,
                        CountryCode = model.ImportCountryCode
                    };

                    _newOfferImportService.AddImportFile(request);

                    await _newOfferImportService.Import();

                    await GetAndMapOfferImports(model);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(JsonResponse<string>.ErrorResponse("Error occurred while importing file."));
            }
            return Ok(JsonResponse<bool>.SuccessResponse(true));
        }

        /// <summary>
        /// This marks the current file import record as complete ready for the next file to be imported
        /// </summary>
        /// <param name="id"></param>
        /// <param name="affiliateId"></param>
        /// <param name="fileTypeId"></param>
        /// <param name="countryCode"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("CompleteImport")]
        public async Task<IActionResult> UpdateImport(int affiliateId, int fileTypeId)
        {
            try
            {
                await _newOfferImportService.CompleteImport(affiliateId, fileTypeId);

                return Ok(JsonResponse<bool>.SuccessResponse(true));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(JsonResponse<string>.ErrorResponse("Error occurred while completing import."));
            }
        }

        ///// <summary>
        ///// 2)
        ///// Is called by clientside javascript code on the Offer Import page of the admin tool
        ///// to start the actual import of data from an offer file into the staging datatable.
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("Import")]
        //public async Task<IActionResult> Import()
        //{
        //    try
        //    {
        //        await _newOfferImportService.UploadToStaging();
        //        return Ok(JsonResponse<bool>.SuccessResponse(true));
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error(ex);
        //        if (ex.Message == "No new records found to migrate offers")
        //        {
        //            return Ok(JsonResponse<string>.ErrorResponse(ex.Message));
        //        }
        //        return Ok(JsonResponse<string>.ErrorResponse("Error occurred while transfer file."));
        //    }
        //}

        /// <summary>
        /// 3)
        /// This is used to transafer records from staging.migrated to offers
        /// </summary>
        /// <param name="id"></param>
        /// <param name="affiliateId"></param>
        /// <param name="fileTypeId"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("UpdateImport")]
        public async Task<IActionResult> UpdateImport(int id, int affiliateId, int fileTypeId, string countryCode)
        {
            try
            {
                await _newOfferImportService.MigrateFromStaging();
                return Ok(JsonResponse<bool>.SuccessResponse(true));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(JsonResponse<string>.ErrorResponse("Error occurred while completing import."));
            }
        }

        //[HttpGet]
        //[ActionName("MigrateOfferFile")]
        //public async Task<IActionResult> MigrateOfferFile()
        //{
        //    try
        //    {
        // ## AL: OfferHelper.GetImportedFilesAndMigrate used to do everything, so is this actually called?

        //        //await OfferHelper.GetImportedFilesAndMigrate(Logger);
        //        await _newOfferImportService.Import();
        //        return Json(JsonResponse<bool>.SuccessResponse(true));
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error(ex);
        //        if (ex.Message == "No new records found to migrate offers")
        //        {
        //            return Json(JsonResponse<string>.ErrorResponse(ex.Message));
        //        }
        //        return Json(JsonResponse<string>.ErrorResponse("Error occurred while transfer file."));
        //    }
        //}

        [HttpGet]
        [ActionName("DownloadFile")]
        public FileResult DownloadFile(string path)
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);
            var fileName = Path.GetFileName(path);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        [HttpGet]
        [ActionName("Offerlist")]
        public async Task<IActionResult> Offerlist()
        {
            MaintainOfferListsModel model = new MaintainOfferListsModel { ListofCountries = GetCountryList() };

            var listOfItems = await _offerService.GetAllOfferList();
            model.ListofOfferListItems = new List<SelectListItem>();
            foreach (var item in listOfItems)
            {
                SelectListItem value = new SelectListItem()
                {
                    Text = item.ListName,
                    Value = item.Id.ToString()
                };
                model.ListofOfferListItems.Add(value);
            }
            model.ListofMerchants = await MapToMerchantList();
            MapToAffiliateList(await _affiliateService.GetAll(), model.ListofAffiliate, null);
            model.ListofOfferType = await MapToOfferTypeList();
            model.ListofStatus = MapToStatusList();
            model.CountryCode = "GB";

            return View("List", model);
        }

        [HttpGet]
        [ActionName("OfferlistSearch")]
        public async Task<IActionResult> OfferlistSearch(int? offerlistId, string countryCode, int? merchantId, int? affiliateId, int? typeId, int? statusId,
           string keyword, DateTime? validFrom, DateTime? validTo, int? OfferPage = 1, int? OfferListPage = 1, bool isPartialView = false)
        //public async Task<IActionResult> OfferlistSearch(MaintainOfferListsModel searchCriteria)
        {
            try
            {
                MaintainOfferListsModel searchCriteria = new MaintainOfferListsModel
                {
                    MerchantId = merchantId,
                    AffiliateId = affiliateId,
                    OfferType = typeId,
                    OfferStatus = statusId,
                    Keyword = keyword,
                    ValidFrom = validFrom,
                    ValidTo = validTo,
                    OfferListPage = (int)OfferListPage,
                    offerPage = (int)OfferPage,
                    OfferListItemId = (int)offerlistId,
                    CountryCode = countryCode
                };

                if (searchCriteria.ValidTo.HasValue)
                {
                    searchCriteria.ValidTo = searchCriteria.ValidTo.Value.AddDays(1);
                }

                if (!ModelState.IsValid)
                {
                    StringBuilder errorBuilder = new StringBuilder();
                    foreach (ModelStateEntry modelState in ViewData.ModelState.Values)
                    {
                        foreach (ModelError error in modelState.Errors)
                        {
                            errorBuilder.Append(error.ErrorMessage);
                            errorBuilder.Append("<br/>");
                        }
                    }

                    return Json(JsonResponse<string>.ErrorResponse(errorBuilder.ToString()));
                }

                if (isPartialView == false)
                {
                    searchCriteria.ListofMerchants = await MapToMerchantList();
                    MapToAffiliateList(await _affiliateService.GetAll(), searchCriteria.ListofAffiliate, null);
                    searchCriteria.ListofOfferType = await MapToOfferTypeList();
                    searchCriteria.ListofStatus = MapToStatusList();
                    searchCriteria.ListofCountries = GetCountryList();
                    var listOfItems = await _offerService.GetAllOfferList();
                    searchCriteria.ListofOfferListItems = new List<SelectListItem>();

                    searchCriteria.ListofOfferListItems.AddRange(listOfItems.Select(item => new SelectListItem
                    {
                        Text = item.ListName,
                        Value = item.Id.ToString()
                    }));
                }

                searchCriteria.ListtOfOffers = new OfferListMaintainViewModel();

                DTOs.OfferSearchCriteria criteria = new DTOs.OfferSearchCriteria()
                {
                    MerchantId = searchCriteria.MerchantId,
                    AffiliateId = searchCriteria.AffiliateId,
                    OfferType = searchCriteria.OfferType,
                    OfferStatus = searchCriteria.OfferStatus,
                    KeyWord = searchCriteria.Keyword,
                    ValidFrom = searchCriteria.ValidFrom,
                    ValidTo = searchCriteria.ValidTo,
                    CountryCode = searchCriteria.CountryCode,
                    PageSize = Convert.ToInt32(_settings.Value.PageSize),
                    CurrentPage = searchCriteria.offerPage
                };
                DTOs.PagedResult<OfferSummary> paging = await _offerService.PagedSearchOffersandOfferList(criteria);
                searchCriteria.ListtOfOffers.CurrentPageNumber = searchCriteria.offerPage;
                if (isPartialView == false)
                {
                    searchCriteria.MerchantId = searchCriteria.MerchantId;
                    searchCriteria.Keyword = searchCriteria.Keyword;
                    searchCriteria.AffiliateId = searchCriteria.AffiliateId;
                    searchCriteria.OfferType = searchCriteria.OfferType;
                    searchCriteria.OfferStatus = searchCriteria.OfferStatus;
                    searchCriteria.ValidFrom = searchCriteria.ValidFrom;
                    searchCriteria.ValidTo = searchCriteria.ValidTo;
                    searchCriteria.CountryCode = searchCriteria.CountryCode;
                    searchCriteria.OfferListItemId = searchCriteria.OfferListItemId;
                }
                searchCriteria.ListtOfOffers.ListofOffers = paging.Results.ToList();
                searchCriteria.ListtOfOffers.PagingModel.CurrentPage = paging.CurrentPage;
                searchCriteria.ListtOfOffers.PagingModel.PageCount = paging.PageCount;
                searchCriteria.ListtOfOffers.PagingModel.PageSize = paging.PageSize;
                searchCriteria.ListtOfOffers.PagingModel.RowCount = paging.RowCount;
                if (isPartialView == false)
                {
                    DTOs.PagedResult<OfferSummary> pagingOfferListItems =
                        new DTOs.PagedResult<OfferSummary>();
                    pagingOfferListItems = await _offerService.PagedSearchOfferListitems(searchCriteria.OfferListItemId,
                        searchCriteria.CountryCode, searchCriteria.OfferListPage,
                        Convert.ToInt32(_settings.Value.PageSize));

                    searchCriteria.ListtOfOffersListitems =
                        new OfferListMaintainViewModel
                        {
                            ListofOffers = pagingOfferListItems.Results.ToList(),
                            PagingModel =
                            {
                                CurrentPage = pagingOfferListItems.CurrentPage,
                                PageCount = pagingOfferListItems.PageCount,
                                PageSize = pagingOfferListItems.PageSize,
                                RowCount = pagingOfferListItems.RowCount
                            }
                        };

                    if (string.IsNullOrEmpty(searchCriteria.CountryCode))
                    {
                        searchCriteria.CountryCode = "GB";
                    }

                    ViewData["SourceData"] =
                        JsonConvert.SerializeObject(searchCriteria.ListtOfOffers.ListofOffers, Formatting.None);
                    ViewData["DestData"] =
                        JsonConvert.SerializeObject(searchCriteria.ListtOfOffersListitems.ListofOffers,
                            Formatting.None);

                    return View("List", searchCriteria);
                }
                else
                {
                    return PartialView("_offersSelectItems", searchCriteria.ListtOfOffers);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(JsonResponse<string>.ErrorResponse("Some error occurred. Please try again."));
            }
        }

        //for partial new offer list with offer data
        [HttpGet]
        [ActionName("GetNewOfferList")]
        public IActionResult GetNewOfferList(int offerId, string offerMerchantName, string shortDescription, bool isSingleRow, string validFrom = null, string validTo = null)
        {
            try
            {
                var offers = new OfferListMaintainViewModel
                {
                    ListofOffers = new List<OfferSummary>
                    {
                        new OfferSummary
                        {
                            OfferId = offerId,
                            MerchantName = offerMerchantName,
                            OfferShortDescription = shortDescription,
                            ValidFrom = !string.IsNullOrEmpty(validFrom)?Convert.ToDateTime(validFrom,new CultureInfo("en-GB")).ToString("yyyy-MM-dd"):null,
                            ValidTo = !string.IsNullOrEmpty(validTo)?Convert.ToDateTime(validTo,new CultureInfo("en-GB")).ToString("yyyy-MM-dd"):null
                        }
                    },
                    IsSingleRow = isSingleRow
                };
                return PartialView("_offersListSelectItems", offers);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(JsonResponse<string>.ErrorResponse("Some error occurred. Please try again."));
            }
        }

        [HttpPost]
        [ActionName("UpdateOfferListItem")]
        public async Task<IActionResult> UpdateOfferListItem(UpdateOfferListItemViewModel model)
        {
            try
            {
                //Get offerList all and delete with no changes
                List<DTOs.OfferListItem> existingRecordsForOfferList = await _offerService.GetByListIdAndCountry(model.OfferListItemId, model.CountryCode);
                //offerListItems count > 0 then saveChanges called after new record save.
                //bool saveChanges = model.OfferListItems.Count != 0 && existingRecordsForOfferList.Count >0;
                bool saveChanges = existingRecordsForOfferList != null;
                if (existingRecordsForOfferList != null)
                    foreach (var item in existingRecordsForOfferList)
                    {
                        DTOs.OfferListItem reqItem = new DTOs.OfferListItem
                        {
                            OfferId = item.OfferId,
                            OfferListId = model.OfferListItemId,
                            CountryCode = model.CountryCode
                        };
                        await _offerService.DeleteOfferListItem(reqItem, saveChanges);
                    }
                string[] dateFormats =
                    {"dd-MM-yyyy hh:mm:ss", "dd/MM/yyyy hh:mm:ss", "dd/MM/yyyy", "dd-MM-yyyy","yyyy-MM-dd","yyyy/MM/dd"};
                var reqItemList = new List<DTOs.OfferListItem>();
                foreach (var offer in model.OfferListItems)
                {
                    //var existingRecordsForOfferListN = await _offerService.GetAllByOfferId(offer.OfferId);

                    //if (existingRecordsForOfferListN == null)
                    //{
                    var reqItem = new DTOs.OfferListItem
                    {
                        OfferListId = model.OfferListItemId,
                        CountryCode = model.CountryCode,
                        OfferId = offer.OfferId,
                        DisplayOrder = offer.DisplayOrder
                    };
                    if (!string.IsNullOrEmpty(offer.ValidFrom))
                    {
                        DateTime startDate;
                        DateTime.TryParseExact(offer.ValidFrom, dateFormats, CultureInfo.InvariantCulture,
                            DateTimeStyles.None, out startDate);
                        reqItem.DisplayFrom = startDate;
                    }

                    if (!string.IsNullOrEmpty(offer.ValidTo))
                    {
                        DateTime endDate;
                        DateTime.TryParseExact(offer.ValidTo, dateFormats, CultureInfo.InvariantCulture,
                            DateTimeStyles.None, out endDate);
                        reqItem.DisplayTo = endDate;
                    }

                    if (reqItem.DisplayFrom != null && reqItem.DisplayTo != null && reqItem.DisplayFrom > reqItem.DisplayTo)
                    {
                        return Json(JsonResponse<string>.ErrorResponse(
                            $"End date should be greater than start date. Please check in row : {reqItem.DisplayOrder}"));
                    }

                    reqItemList.Add(reqItem);
                    //}
                }
                //save new offerList and saveChanges.
                if (reqItemList.Count > 0)
                {
                    await _offerService.AddOfferListItemList(reqItemList);
                }
                return Json(JsonResponse<string>.SuccessResponse("Successfully added offer list items"));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(JsonResponse<string>.ErrorResponse("Some error occurred while processing the request. Please try again."));
            }
        }

        [HttpPost]
        [ActionName("AddDeleteOfferlistItem")]
        public async Task<IActionResult> AddDeleteOfferlistItem(AddEditOfferListItemModel requestModel, int[] ids)
        {
            try
            {
                int requestCount = _cache.Get<int>(Keys.Keys.RequestCount);
                //check if request count is available in cach
                requestCount++;
                _cache.Set(requestCount, Keys.Keys.RequestCount);

                List<DTOs.OfferListItem> existngRecordsForOfferList = await _offerService.GetByListIdAndCountry(requestModel.OfferListId, requestModel.Countrycode);
                var displayOrder = 1;
                if (existngRecordsForOfferList != null && existngRecordsForOfferList.Count > 0)
                {
                    displayOrder = existngRecordsForOfferList.Max(x => x.DisplayOrder) + 1;
                }
                if (requestModel.SelectedOrder.HasValue)
                {
                    displayOrder = requestModel.SelectedOrder.Value;
                }
                //Get OfferIds from request string
                List<string> offerIds = requestModel.OfferIds.Split(',').ToList();

                if (requestModel.ItemsSelected == 0)
                {
                    requestModel.ItemsSelected = 1;
                }

                //Update Order of offer list
                if (requestModel.SelectedOrder.HasValue)
                {
                    await _offerService.ReorderOffersOnSelectedPostition(requestModel.OfferListId,
                        requestModel.Countrycode, requestModel.SelectedOrder.Value, requestModel.ItemsSelected);
                }
                for (int i = 0; i < offerIds.Count; i++)
                {
                    DTOs.OfferListItem item = new DTOs.OfferListItem
                    {
                        OfferId = Convert.ToInt32(offerIds[i]),
                        OfferListId = requestModel.OfferListId,
                        CountryCode = requestModel.Countrycode,
                        DisplayOrder = Convert.ToInt16(displayOrder + i)
                    };

                    if (requestModel.AddTolist)
                    {
                        var record = await _offerService.GetByOfferIdandCountry(item.OfferId, item.OfferListId, item.CountryCode);
                        if (record == null)
                        {
                            await _offerService.AddOfferListItem(item);
                        }
                    }
                    else
                    {
                        await _offerService.DeleteOfferListItem(item);
                    }
                }

                if (requestModel.AddTolist)
                {
                    requestCount--;
                    CacheHelper.Set(_cache, requestCount, Keys.Keys.RequestCount);
                    return Json(JsonResponse<int>.SuccessResponse(requestCount));
                    //return PartialView("_offersSelectItems", responseModel);
                }
                requestCount--;
                CacheHelper.Set(_cache, requestCount, Keys.Keys.RequestCount);
                return Json(JsonResponse<int>.SuccessResponse(requestCount));
                //return PartialView("_offersSelectItems", responseModel);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(JsonResponse<string>.ErrorResponse("Some error occurred while processing the request. Please try again."));
            }
        }

        [HttpPost]
        [ActionName("UpdateOfferListOrder")]
        public async Task<IActionResult> UpdateOfferListOrder(List<UpdateOfferListOrderModel> requestModel)
        {
            try
            {
                OfferListMaintainViewModel responseModel = new OfferListMaintainViewModel();

                DTOs.PagedResult<OfferSummary> pagingOfferListItems =
                    await _offerService.PagedSearchOfferListitems(requestModel[0].OfferListId,
                        requestModel[0].CountryCode, requestModel[0].PageNumber,
                        Convert.ToInt32(_settings.Value.PageSize));

                responseModel.ListofOffers = pagingOfferListItems.Results.ToList();
                responseModel.PagingModel.CurrentPage = pagingOfferListItems.CurrentPage;
                responseModel.PagingModel.PageCount = pagingOfferListItems.PageCount;
                responseModel.PagingModel.PageSize = pagingOfferListItems.PageSize;
                responseModel.PagingModel.RowCount = pagingOfferListItems.RowCount;

                return PartialView("_offersListSelectItems", responseModel);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(JsonResponse<string>.ErrorResponse("Some error occurred while processing the request. Please try again."));
            }
        }

        [HttpGet]
        [ActionName("GetProcessingFile")]
        public async Task<IActionResult> GetProcessingFile(int affiliateId, int fileId, string countryCode)
        {
            FileImportModel model = new FileImportModel
            {
                AffiliateId = affiliateId,
                FileTypeId = fileId,
                ImportCountryCode = countryCode
            };
            await GetAndMapOfferImports(model);
            return Json(JsonResponse<Object>.SuccessResponse(model));
        }

        [HttpGet]
        [ActionName("GetMerchantBranches")]
        public async Task<IActionResult> GetMerchantBranches(int merchantId)
        {
            try
            {
                var MerchantBranches = await MapToMerchantBranchList(merchantId);
                //   List<MerchantBranch> ListofBranches = MerchantBranches;
                JsonResult jsonResult = Json(JsonResponse<List<MerchantBranch>>.SuccessResponse(MerchantBranches));
                return jsonResult;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return View("Error");
            }
        }

        #region Private Methods

        //Map to Offer View Model
        private async Task<OfferViewModel> MapToOfferViewModel(int id)
        {
            try
            {
                OfferViewModel model = new OfferViewModel();
                if (id > 0)
                {
                    DTOs.Offer offer = await _offerService.Get(id, true, true, true);
                    if (offer != null)
                    {
                        model.Id = offer.Id;
                        model.DateAdded = offer.Datecreated;
                        model.ListofMerchants = await MapToMerchantList();
                        model.RefMerchantId = offer.MerchantId;
                        model.MerchantId = offer.MerchantId;
                        model.ListofOfferType = await MapToOfferTypeList();
                        model.AffiliateId = offer.AffiliateId;
                        MapToAffiliateList(await _affiliateService.GetAll(), model.ListofAffiliate, null);
                        model.OfferTypeId = offer.OfferTypeId;
                        model.ListofStatus = MapToStatusList();
                        model.StatusId = offer.StatusId;
                        model.ListofRanking = MapToSearchRanking(new List<SelectListItem>());
                        model.SearchRanking = offer.SearchRanking;
                        model.Reoccuring = offer.Reoccuring;
                        model.ValidIndefintely = offer.Validindefinately;
                        model.Headline = offer.Headline;
                        model.ShortDescription = offer.ShortDescription;
                        model.LongDescription = offer.LongDescription;
                        model.Instructions = offer.Instructions;
                        model.Terms = offer.Terms;
                        model.Exclusions = offer.Exclusions;
                        model.LinkURL = offer.LinkUrl;
                        model.OfferCode = offer.OfferCode;
                        model.RedemptionAccountNumber = offer.RedemptionAccountNumber;
                        model.RedemptionProductCode = offer.RedemptionProductCode;
                        model.AffiliateReference = offer.AffiliateReference;
                        model.ListofTag = await MapToListofOfferTag(offer.Id);
                        model.ListofCountries = await MapToEditCountryList(GetCountrySelectList(), offer.Id);
                        model.TagLists = await MapToTagLists();
                        model.ListofCategory = await MapToEditCategoryList(await MapToCategoryList(), offer.Id);
                        model.ListofBranches = await MapToEditMerchantBranch(await MapToOfferMerchantBranchList(offer.MerchantId), offer.Id);
                        model.SSOThirdpartySiteId = offer.SSOConfigId ?? 0;
                        model.ProductCode = offer.ProductCode;
                        model.ListofSSOThirdPartySites = await MapToSSOThirdPartySiteList();
                        if (offer.ValidFrom.HasValue)
                        {
                            model.ValidFrom =
                                TimeZoneInfo.ConvertTimeFromUtc(offer.ValidFrom.Value, TimeZoneInfo.Local);
                        }
                        if (offer.ValidTo.HasValue)
                        {
                            model.ValidTo = TimeZoneInfo.ConvertTimeFromUtc(offer.ValidTo.Value, TimeZoneInfo.Local);
                        }
                    }
                }
                else if (id == 0)
                {
                    model.ListofMerchants = await MapToMerchantList();
                    MapToAffiliateList(await _affiliateService.GetAll(), model.ListofAffiliate, null);
                    model.ListofOfferType = await MapToOfferTypeList();
                    model.ListofStatus = MapToStatusList();
                    model.ListofRanking = MapToSearchRanking(new List<SelectListItem>());
                    model.ListofCountries = MapToAddCountryList(GetCountrySelectList(), "GB");
                    model.ListofBranches = new List<SelectListItem>();
                    model.ListofCategory = await MapToCategoryList();
                    model.TagLists = await MapToTagLists();
                    model.SearchRanking = 5;
                    model.DateAdded = DateTime.UtcNow;
                    model.ListofSSOThirdPartySites = await MapToSSOThirdPartySiteList();
                }
                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<List<SelectListItem>> MapToEditMerchantBranch(List<MerchantBranch> merchantBranches, int offerId)
        {
            var branch = await _offerBranchServices.GetofferBranch(offerId);
            List<SelectListItem> merchantBranchSelectedList = new List<SelectListItem>();

            foreach (var id in merchantBranches)
            {
                bool isofferbranch = false;
                foreach (var v in branch)
                {
                    if (id.Id == v)
                    {
                        isofferbranch = true;
                    }
                }
                merchantBranchSelectedList.Add(new SelectListItem()
                {
                    Text = id.Name,
                    Value = id.Id.ToString(),
                    Selected = isofferbranch
                });
            }
            return merchantBranchSelectedList;
        }

        private async Task MapToOfferSearchViewModel(OfferSearchViewModel model, OfferSearchViewModel searchCriteria,
            OfferListRequest req, bool setfromSession)
        {
            OffersSortOrder sortOrder = GetSortOrder(req);
            model.ListofMerchants = await MapToMerchantList();
            MapToAffiliateList(await _affiliateService.GetAll(), model.ListofAffiliate, null);
            model.ListofOfferType = await MapToOfferTypeList();
            model.ListofStatus = MapToStatusList();
            model.OffersList = new OfferListViewModel(req);
            DTOs.PagedResult<OfferSummary> paging = new DTOs.PagedResult<OfferSummary>();
            if (searchCriteria != null && setfromSession)
            {
                paging = await _offerService.PagedSearch(searchCriteria.MerchantId,
                    searchCriteria.AffiliateId,
                    searchCriteria.OfferType, searchCriteria.OfferStatus, searchCriteria.Keyword,
                    searchCriteria.ValidFrom,
                    searchCriteria.ValidTo, searchCriteria.PageNumber, Convert.ToInt32(_settings.Value.PageSize), sortOrder);
                model.OffersList.CurrentPageNumber = searchCriteria.PageNumber;
                model.MerchantId = searchCriteria.MerchantId;
                model.Keyword = searchCriteria.Keyword;
                model.AffiliateId = searchCriteria.AffiliateId;
                model.OfferType = searchCriteria.OfferType;
                model.OfferStatus = searchCriteria.OfferStatus;
                model.ValidFrom = searchCriteria.ValidFrom;
                model.ValidTo = searchCriteria.ValidTo;
            }
            else
            {
                paging = await _offerService.GetAllPagedSearch(1, Convert.ToInt32(_settings.Value.PageSize));
                model.OffersList.CurrentPageNumber = 1;
            }

            model.OffersList.ListofOffers = paging.Results.ToList();
            model.OffersList.PagingModel.CurrentPage = paging.CurrentPage;
            model.OffersList.PagingModel.PageCount = paging.PageCount;
            model.OffersList.PagingModel.PageSize = paging.PageSize;
            model.OffersList.PagingModel.RowCount = paging.RowCount;
            if (req.SortField == OfferSortField.ValidFrom || req.SortField == OfferSortField.ValidTo)
            {
                model.OffersList.SortIcon = req.SortDirection == "desc" ? "fa fa-sort-numeric-desc" : "fa fa-sort-numeric-asc";
            }
            else
            {
                model.OffersList.SortIcon = req.SortDirection == "desc" ? "fa fa-sort-alpha-desc" : "fa fa-sort-alpha-asc";
            }
        }

        /// <summary>
        /// Called when no data found in the current user's browser session for previous imports
        /// Fetches the list of affiliates and their mapping rules, plus list of countries.
        ///
        /// </summary>
        /// <param name="service"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        private async Task InitializeFileImport(IAffiliateService service, FileImportModel model)
        {
            var affiliates = await service.GetAll();
            model.Affiliates = MapToAffiliateAndFiles(affiliates);
            MapToAffiliateList(affiliates, model.AffiliateList, model.FileTypes);
            //model.CurrentStatus = Enums.Import.New.ToString(); //Status.New;
            model.Migrated = (int)Enums.Import.Processed;
            model.ListofCountries = GetCountryList();
            if (string.IsNullOrEmpty(model.ImportCountryCode))
            {
                model.ImportCountryCode = "GB";
            }
        }

        //Get and Map to import file view model and save file to folder
        private async Task GetAndMapOfferImports(FileImportModel model)
        {
            try
            {
                //DTOs.StagingModels.OfferImportFile reqModel = _OLDofferImportService.Get(model.AffiliateId, model.FileTypeId, (int)Import.Complete);
                //DTOs.StagingModels.OfferImportFile reqModel = _newOfferImportService.GetImportFile(model.AffiliateId, model.FileTypeId, (int)Enums.Import.Processed);
                DTOs.StagingModels.OfferImportFile reqModel = _newOfferImportService.GetLatestImportFile(model.AffiliateId, model.FileTypeId);
                if (reqModel != null)
                {
                    model.Id = reqModel.Id;
                    model.Success = reqModel.Imported;
                    model.TotalRecords = reqModel.TotalRecords;
                    model.ImportStatus = reqModel.ImportStatus;
                    model.File = reqModel.FilePath;
                    model.ErrorFilePath = reqModel.ErrorFilePath;
                    //model.CurrentStatus = Enum.GetName(typeof(MembershipCardStatus), model.ImportStatus); //status?.FirstOrDefault(x => x.IsActive && x.Type == StatusType.Import && x.Id == model.ImportStatus)?.Name;
                    model.CurrentStatus = Enum.GetName(typeof(Enums.Import), model.ImportStatus);
                    model.Failed = reqModel.Failed;
                    model.Staged = reqModel.Staged;
                    model.Duplicates = reqModel.Duplicates;
                    model.Updates = reqModel.Updates;
                }

                //This is always the Import.Processed int value, its used in the scripts
                //for comparison with Model.ImportStatus to identify of file processing is complete (I think)
                model.Migrated = (int)Enums.Import.Processed;

                // This section is a repeat of fetching data in InitializeFileImport()
                // Too much to expect common code to be put in reusable methods???

                List<DTOs.Affiliate> affiliates = await _affiliateService.GetAll();
                model.Affiliates = MapToAffiliateAndFiles(affiliates);
                MapToAffiliateList(affiliates, model.AffiliateList, model.FileTypes);
                model.ListofCountries = GetCountryList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<List<SelectListItem>> MapToMerchantList()
        {
            List<DTOs.Merchant> response = await _merchantService.GetAll();
            if (response != null)
            {
                return response.Select(merchant => new SelectListItem()
                {
                    Text = merchant.Name,
                    Value = merchant.Id.ToString()
                })
.ToList();
            }
            return new List<SelectListItem>();
        }

        //Map to Affiliate SelectListItem
        private void MapToAffiliateList(List<DTOs.Affiliate> affiliates, List<SelectListItem> affiliateList, List<SelectListItem> fileTypes)
        {
            if (affiliates != null && affiliates.Count > 0)
            {
                foreach (DTOs.Affiliate affiliate in affiliates)
                {
                    affiliateList.Add(new SelectListItem
                    {
                        Text = affiliate.Name,
                        Value = affiliate.Id.ToString()
                    });
                    if (affiliate.AffiliateFiles == null || fileTypes == null) continue;
                    foreach (DTOs.AffiliateFile fileType in affiliate.AffiliateFiles)
                    {
                        if (fileTypes.All(x => x.Text != fileType.FileName))
                        {
                            fileTypes.Add(new SelectListItem
                            {
                                Text = fileType.FileName,
                                Value = fileType.Id.ToString()
                            });
                        }
                    }
                }
            }
        }

        //Map to OfferType SelectListItem
        private async Task<List<SelectListItem>> MapToOfferTypeList()
        {
            List<DTOs.OfferType> response = await _offertypeService.GetAll();
            if (response != null)
            {
                return response.Select(offerType => new SelectListItem()
                {
                    Text = offerType.Description,
                    Value = offerType.Id.ToString()
                })
.ToList();
            }
            return new List<SelectListItem>();
        }

        //Map to MerchantBranch
        private async Task<List<MerchantBranch>> MapToMerchantBranchList(int merchantId)
        {
            List<DTOs.MerchantBranch> response = await _merchantBranchService.GetAll(merchantId);

            return response.Select(branch => new MerchantBranch()
            {
                Name = branch.Name,
                Id = branch.Id
            })
            .ToList();
        }

        //Map to Offer Status SelectListItem
        private List<SelectListItem> MapToStatusList()
        {
            return Enum.GetValues(typeof(OfferStatus)).Cast<OfferStatus>().Select(v => new SelectListItem
            {
                Text = Regex.Replace(v.ToString(), "(\\B[A-Z]+?(?=[A-Z][^A-Z])|\\B[A-Z]+?(?=[^A-Z]))", " $1"),
                Value = ((int)v).ToString()
            }).ToList();
        }

        //Map to Offer Search ranking SelectListItem
        private List<SelectListItem> MapToSearchRanking(List<SelectListItem> ranking)
        {
            for (int i = 1; i <= 5; i++)
            {
                ranking.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
            }
            return ranking;
        }

        //Map to Offer Category
        private async Task<List<CategoryTreeView>> MapToCategoryList()
        {
            List<DTOs.Category> categories = _categoryService.GetAll();

            await Task.CompletedTask;

            return categories.Select(category => new CategoryTreeView()
            {
                Id = category.Id,
                Name = category.Name,
                ParentId = category.ParentId
            })
            .ToList();
        }

        private async Task<List<MerchantBranch>> MapToOfferMerchantBranchList(int merchantid)
        {
            List<DTOs.MerchantBranch> branches = await _merchantBranchService.GetAll(merchantid);

            await Task.CompletedTask;

            return branches.Select(category => new MerchantBranch()
            {
                Id = category.Id,
                Name = category.Name
            })
            .ToList();
        }

        //Map to Offer Category with selections
        private async Task<List<CategoryTreeView>> MapToEditCategoryList(List<CategoryTreeView> categoryTrees, int id)
        {
            List<DTOs.OfferCategory> offerCategories = await _offerCategoryService.GetAll(id);
            if (offerCategories?.Count > 0)
            {
                foreach (DTOs.OfferCategory offerCategory in offerCategories)
                {
                    foreach (CategoryTreeView categoryTreeView in categoryTrees)
                    {
                        if (categoryTreeView.Id == offerCategory.CategoryId)
                        {
                            categoryTreeView.IsChecked = true;
                        }
                    }
                }
            }

            return categoryTrees;
        }

        //Map to Tag Lists
        private async Task<List<CustomTagList>> MapToTagLists()
        {
            List<DTOs.Tag> tags = await _tagService.GetAll();
            if (tags == null || tags.Count == 0)
                return null;
            List<CustomTagList> data = new List<CustomTagList>();
            data.AddRange(tags.Select(tag => new CustomTagList
            {
                Id = tag.Id,
                Tags = tag.Tags
            }));
            return data;
        }

        //Map to Offer Tags
        private async Task<List<CustomTagList>> MapToListofOfferTag(int id)
        {
            List<DTOs.OfferTag> offerTags = await _offerTagService.GetAll(id);
            if (offerTags == null || offerTags.Count == 0)
                return null;
            List<CustomTagList> data = new List<CustomTagList>();

            data.AddRange(offerTags.Select(tag => new CustomTagList
            {
                Id = tag.TagId,
                Tags = tag.Tag?.Tags
            }));

            return data;
        }

        //Map to Country seletion in List
        private async Task<List<CustomCountryList>> MapToEditCountryList(List<CustomCountryList> customCountryLists, int id)
        {
            List<DTOs.OfferCountry> offerCountries = await _offerCountryService.GetAll(id);
            if (offerCountries != null)
                foreach (DTOs.OfferCountry offerCountry in offerCountries)
                {
                    foreach (CustomCountryList customCountryList in customCountryLists)
                    {
                        if (customCountryList.Name == offerCountry.CountryCode)
                        {
                            customCountryList.Accepted = true;
                        }
                    }
                }
            return customCountryLists;
        }

        //Map to Country seletion for default case
        private static List<CustomCountryList> MapToAddCountryList(List<CustomCountryList> customCountryLists, string defaultValue)
        {
            foreach (CustomCountryList customCountryList in customCountryLists)
            {
                if (customCountryList.Name == defaultValue)
                {
                    customCountryList.Accepted = true;
                }
            }
            return customCountryLists;
        }

        //Map Third party sites list to selectitems list
        private async Task<List<SelectListItem>> MapToSSOThirdPartySiteList()
        {
            var ssoList = new List<SelectListItem>() { new SelectListItem { Text = "None Selected", Value = "0" }, };
            var ssoSites = await _ssoService.GetAllSSOConfigurations();
            if (ssoSites != null)
            {
                var listItems = ssoSites.Select(site => new SelectListItem()
                {
                    Text = site.Name,
                    Value = site.Id.ToString()
                }).ToList();
                ssoList.AddRange(listItems);
            }
            return ssoList;
        }

        private List<CustomCountryList> GetCountrySelectList()
        {
            List<CustomCountryList> country = new List<CustomCountryList>
            {
                new CustomCountryList {Name = "CZ", Value = "CZ"},
                new CustomCountryList {Name = "GB", Value = "GB"},
                new CustomCountryList {Name = "PL", Value = "PL"},
                new CustomCountryList {Name = "SC", Value = "SC"},
                new CustomCountryList {Name = "SK", Value = "SK"}
            };

            return country;
        }

        private OffersSortOrder GetSortOrder(OfferListRequest req)
        {
            OffersSortOrder sortOrder;

            switch (req.SortField)
            {
                case OfferSortField.MerchantName:
                    sortOrder = req.SortDirection == "asc" ? OffersSortOrder.MerchantNameAsc : OffersSortOrder.MerchantNameDesc;
                    break;

                case OfferSortField.OfferShortDescription:
                    sortOrder = req.SortDirection == "asc" ? OffersSortOrder.OfferShortDescriptionAsc : OffersSortOrder.OfferShortDescriptionDesc;
                    break;

                case OfferSortField.ValidFrom:
                    sortOrder = req.SortDirection == "asc" ? OffersSortOrder.ValidFromAsc : OffersSortOrder.ValidFromDesc;
                    break;

                case OfferSortField.ValidTo:
                    sortOrder = req.SortDirection == "asc" ? OffersSortOrder.ValidToAsc : OffersSortOrder.ValidToDesc;
                    break;

                default:
                    sortOrder = OffersSortOrder.MerchantNameAsc;
                    break;
            }

            return sortOrder;
        }

        private List<AffiliateToAffiliateFileMapping> MapToAffiliateAndFiles(List<DTOs.Affiliate> affiliates)
        {
            List<AffiliateToAffiliateFileMapping> mappings = new List<AffiliateToAffiliateFileMapping>();
            foreach (DTOs.Affiliate affiliate in affiliates)
            {
                if (affiliate.AffiliateFiles == null) continue;

                AffiliateToAffiliateFileMapping aff = new AffiliateToAffiliateFileMapping
                {
                    AffiliateName = affiliate.Name,
                    AffiliateId = affiliate.Id
                };

                foreach (DTOs.AffiliateFile fileType in affiliate.AffiliateFiles)
                {
                    aff.FileTypes.Add(new SelectListItem
                    {
                        Text = fileType.FileName,
                        Value = fileType.Id.ToString()
                    });
                }
                mappings.Add(aff);
            }
            return mappings;
        }

        public static List<SelectListItem> GetCountryList()
        {
            List<SelectListItem> country = new List<SelectListItem>
            {
                new SelectListItem {Text = "CZ", Value = "CZ"},
                new SelectListItem {Text = "GB", Value = "GB"},
                new SelectListItem {Text = "PL", Value = "PL"},
                new SelectListItem {Text = "SC", Value = "SC"},
                new SelectListItem {Text = "SK", Value = "SK"}
            };

            return country;
        }

        #endregion Private Methods
    }
}