using System;
using Microsoft.AspNetCore.Mvc;
using ExclusiveCard.WebAdmin.ViewModels;
using ExclusiveCard.Enums;
using ExclusiveCard.WebAdmin.Helpers;
using System.Threading.Tasks;
using AutoMapper;
using ExclusiveCard.Providers;
using ExclusiveCard.Services.Interfaces.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using DTOs = ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.WebAdmin.Controllers
{
    [Authorize(Roles = "AdminUser, BackOfficeUser")]
    [SessionTimeout]
    public class MerchantController : BaseController
    {
        #region Private Members

        private readonly IMerchantService _merchantService;
        private readonly IMerchantBranchService _branchService;
        private readonly IContactDetailService _contactService;
        private readonly IMerchantSocialMediaLinkService _mediaLinkService;
        private readonly ISocialMediaCompanyService _mediaCompanyService;
        private readonly IMerchantImageService _merchantImageService;
        private readonly IAzureStorageProvider _azureStorageProvider;
        private readonly IOfferService _offerService;
        private readonly IMapper _mapper;
        private readonly IOptions<TypedAppSettings> _settings;

        public const string THUMBNAIL_SUFFIX = "__1";
        public const string MEDIUM_SUFFIX = "__2";
        public const string LARGE_SUFFIX = "__3";
        public const string FEATURE_SUFFIX = "__4";

        private IConfiguration AppSettings { get; set; }

        private readonly int _thumbnailHeight;
        private readonly int _thumbnailWidth;
        private readonly int _mediumHeight;
        private readonly int _mediumWidth;
        private readonly int _largeHeight;
        private readonly int _largeWidth;
        private readonly int _featureHeight;
        private readonly int _featureWidth;

        #endregion

        #region Constructor

        public MerchantController(
            IMerchantService merchantService,
            IMerchantBranchService branchService,
            IContactDetailService contactService,
            IMerchantSocialMediaLinkService linkService,
            ISocialMediaCompanyService mediaService,
            IMerchantImageService merchantImageService,
            IAzureStorageProvider azureStorageProvider,
            IOfferService offerService,
            IMapper mapper,
            IOptions<TypedAppSettings> settings,
            IConfiguration configuration)
        {
            _merchantService = merchantService;
            _branchService = branchService;
            _contactService = contactService;
            _mediaLinkService = linkService;
            _mediaCompanyService = mediaService;
            _merchantImageService = merchantImageService;
            _azureStorageProvider = azureStorageProvider;
            _offerService = offerService;
            _mapper = mapper;
            _settings = settings;
            AppSettings = configuration;

            if (AppSettings["Images:ThumbnailHeight"] != null)
            {
                int.TryParse(AppSettings["Images:ThumbnailHeight"], out _thumbnailHeight);
            }
            else
            {
                Logger.Error("Images:ThumbnailHeight not found in app settings");
            }

            if (AppSettings["Images:ThumbnailWidth"] != null)
            {
                int.TryParse(AppSettings["Images:ThumbnailWidth"], out _thumbnailWidth);
            }
            else
            {
                Logger.Error("Images:ThumbnailWidth not found in app settings");
            }

            if (AppSettings["Images:MediumHeight"] != null)
            {
                int.TryParse(AppSettings["Images:MediumHeight"], out _mediumHeight);
            }
            else
            {
                Logger.Error("Images:MediumHeight not found in app settings");
            }


            if (AppSettings["Images:MediumWidth"] != null)
            {
                int.TryParse(AppSettings["Images:MediumWidth"], out _mediumWidth);
            }
            else
            {
                Logger.Error("Images:MediumWidth not found in app settings");
            }

            if (AppSettings["Images:LargeHeight"] != null)
            {
                int.TryParse(AppSettings["Images:LargeHeight"], out _largeHeight);
            }
            else
            {
                Logger.Error("Images:LargeHeight not found in app settings");
            }

            if (AppSettings["Images:LargeWidth"] != null)
            {
                int.TryParse(AppSettings["Images:LargeWidth"], out _largeWidth);
            }
            else
            {
                Logger.Error("Images:LargeWidth not found in app settings");
            }
            //
            if (AppSettings["Images:FeatureHeight"] != null)
            {
                int.TryParse(AppSettings["Images:FeatureHeight"], out _featureHeight);
            }
            else
            {
                Logger.Error("Images:FeatureHeight not found in app settings");
            }

            if (AppSettings["Images:FeatureWidth"] != null)
            {
                int.TryParse(AppSettings["Images:FeatureWidth"], out _featureWidth);
            }
            else
            {
                Logger.Error("Images:FeatureWidth not found in app settings");
            }

        }

        #endregion

        [HttpGet]
        [ActionName("Index")]
        public async Task<IActionResult> Index()
        {
            //Method to Redirect To MerchantListpage From Navigation
            try
            {
                HttpContext?.Session?.SetString(Keys.Keys.SessionCustomerSearch, string.Empty);
                HttpContext?.Session?.SetString("imageCriteria", string.Empty);
                string searchText = HttpContext?.Session?.GetString("SearchCriteria");
                string pageNumber = HttpContext?.Session?.GetString("pageNumber") ?? "1";
                HttpContext?.Session?.SetString("pageNumber", "1");
                HttpContext?.Session?.SetString("SearchCriteria", string.Empty);
                MerchantSearchViewModel viewModel = new MerchantSearchViewModel();

                MerchantListRequest req = new MerchantListRequest();
                await MapToMerchantViewModel(viewModel, req, searchText, Convert.ToInt32(pageNumber));
                return View("Index", viewModel);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return View("Error");
            }
        }

        [HttpGet]
        [ActionName("Search")]
        public async Task<IActionResult> Search(string searchText = null, int pageNumber = 1, string sortField = null, string sortDirection = null)
        {  //Method to retun Search Results of Merchants From Merchants page
            try
            {
                if (!string.IsNullOrEmpty(searchText))
                {
                    searchText = searchText.Trim();
                }
                HttpContext.Session.SetString("pageNumber", pageNumber.ToString());
                HttpContext.Session.SetString("SearchCriteria", searchText ?? string.Empty);
                MerchantListRequest req = new MerchantListRequest
                {
                    MerchantSortField = (MerchantSortField)Enum.Parse(typeof(MerchantSortField), sortField, true),
                    SortDirection = sortDirection
                };
                MerchantSearchViewModel viewModel = new MerchantSearchViewModel();
                await MapToMerchantViewModel(viewModel, req, searchText, Convert.ToInt32(pageNumber));
                return View("Index", viewModel);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(JsonResponse<string>.ErrorResponse("Error searching merchants."));
            }
        }

        [HttpGet]
        [ActionName("Add")]
        public async Task<IActionResult> Add(int id = 0)
        {
            //Common Method to return Add/Edit Pages From Merchant Page
            try
            {
                MerchantViewModel merchant = new MerchantViewModel();

                if (id == 0)
                {
                    string criteriajson = HttpContext?.Session?.GetString("imageCriteria");
                    List<MerchantImageViewModel> sessionImages = JsonConvert.DeserializeObject<List<MerchantImageViewModel>>(criteriajson) ??
                                                                 new List<MerchantImageViewModel>();


                    ViewBag.Name = TypeOfRequest.Add.ToString();

                    List<Services.Models.DTOs.SocialMediaCompany> activeSocialMediaCompanies =
                        await _mediaCompanyService.GetAll();
                    foreach (Services.Models.DTOs.SocialMediaCompany item in activeSocialMediaCompanies)
                    {
                        SocialMediaItem medialink = new SocialMediaItem
                        {
                            SocialMediaCompanyId = item.Id,
                            Name = item.Name
                        };
                        merchant.SocialMediaLinks.Add(medialink);
                    }
                    merchant.MerchantImages = sessionImages;
                }
                return View("AddEditMerchant", merchant);
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
            //Common Method to return Add/Edit Pages From Merchant Page
            try
            {
                MerchantViewModel merchantModel = new MerchantViewModel();

                if (id > 0)
                {
                    //Map Manager and return view
                    DTOs.Merchant merchant = _merchantService.Get(id, true, false, false, true);
                    if (merchant.ContactDetailsId.HasValue)
                    {
                        merchant.ContactDetail = await _contactService.Get(merchant.ContactDetailsId.Value);
                    }
                    merchantModel = new MerchantViewModel
                    {
                        SocialMediaLinks = new List<SocialMediaItem>(),
                        MerchantBranchList = new MerchantBranchListViewModel(),
                        Id = merchant.Id,
                        Name = merchant.Name,
                        ShortDescription = merchant.ShortDescription,
                        LongDescription = merchant.LongDescription,
                        WebSite = merchant.WebsiteUrl,
                        ContactDetailId = merchant.ContactDetail?.Id,
                        LandlinePhone = merchant.ContactDetail?.LandlinePhone,
                        MobilePhone = merchant.ContactDetail?.MobilePhone,
                        EmailAddress = merchant.ContactDetail?.EmailAddress,
                        Terms = merchant.Terms,
                        FeatureImageOfferText = merchant.FeatureImageOfferText,
                        BrandColor = merchant.BrandColour
                    };

                    List<Services.Models.DTOs.SocialMediaCompany> activeSocialMediaCompanies = await _mediaCompanyService.GetAll();
                    foreach (Services.Models.DTOs.SocialMediaCompany item in activeSocialMediaCompanies)
                    {
                        if (merchant.MerchantSocialMediaLinks?.ToList()?.Count > 0)
                        {
                            foreach (DTOs.MerchantSocialMediaLink merchantitem in merchant.MerchantSocialMediaLinks)
                            {
                                if (merchantitem.SocialMediaCompanyId == item.Id)
                                {
                                    SocialMediaItem medialink = new SocialMediaItem
                                    {
                                        SocialMediaCompanyId = item.Id,
                                        Name = item.Name,
                                        URI = merchantitem.SocialMediaURI
                                    };
                                    merchantModel.SocialMediaLinks.Add(medialink);
                                }
                            }
                        }
                        else
                        {
                            SocialMediaItem medialink = new SocialMediaItem
                            {
                                SocialMediaCompanyId = item.Id,
                                Name = item.Name
                            };
                            merchantModel.SocialMediaLinks.Add(medialink);
                        }
                    }
                    //merchantModel.Branches = merchant.MerchantBranch.Where(x => !x.IsDeleted).OrderBy(x => x?.DisplayOrder).ToList();
                    if (id > 0)
                    {
                        merchantModel.MerchantBranchList = await MapToMerchantBranchListViewModel(merchant.Id, 1);
                    }
                    List<DTOs.MerchantImage> merchantImages = await _merchantImageService.GetAll(id, MEDIUM_SUFFIX, 0);
                    foreach (DTOs.MerchantImage merchantImage in merchantImages)
                    {
                        MerchantImageViewModel merchantImageView = new MerchantImageViewModel
                        {
                            Id = merchantImage.Id,
                            MerchantId = merchantImage.MerchantId,
                            ImagePath = merchantImage.ImagePath,
                            DisplayOrder = merchantImage.DisplayOrder,
                            ImageType = merchantImage.ImageType //?? (int)ImageType.Logo
                        };
                        switch (merchantImageView.ImageType)
                        {
                            case (int) ImageType.Logo:
                                merchantModel.MerchantImages.Add(merchantImageView);
                                break;
                            case (int) ImageType.DisabledLogo:
                                merchantModel.DisabledLogo = merchantImageView;
                                break;
                            case (int) ImageType.FeatureImage:
                                merchantModel.FeatureImage = merchantImageView;
                                break;
                        }
                    }
                    ViewBag.Name = TypeOfRequest.Edit.ToString();
                }
                return View("AddEditMerchant", merchantModel);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return View("Error");
            }
        }

        [HttpPost]
        [ActionName("Save")]
        public async Task<IActionResult> Save(MerchantViewModel request)
        {
            // Method to Save Merchant Details
            try
            {
                /*In furture, if you are required to stay back in the same screen,
                 please return the Id instead of bool and assign it in the UI*/
                List<MerchantImageViewModel> sessionImages = new List<MerchantImageViewModel>();
                if (request.Id == 0)
                {
                    string criteriajson = HttpContext?.Session?.GetString(Data.Constants.Keys.imageCriteria);
                    sessionImages = JsonConvert.DeserializeObject<List<MerchantImageViewModel>>(criteriajson) ?? new List<MerchantImageViewModel>();
                }

                //Get Disabled Logo from session
                List<MerchantImageViewModel> disabledLogo = new List<MerchantImageViewModel>();
                if(request.Id == 0)
                {
                    string jsonData = HttpContext?.Session?.GetString(Data.Constants.Keys.imageDisabled);
                    disabledLogo = !string.IsNullOrEmpty(jsonData) ? JsonConvert.DeserializeObject<List<MerchantImageViewModel>>(jsonData) : new List<MerchantImageViewModel>();
                }

                //Get Feature image from session
                List<MerchantImageViewModel> featureImage = new List<MerchantImageViewModel>();
                if (request.Id == 0)
                {
                    string jsonData = HttpContext?.Session?.GetString(Data.Constants.Keys.imageFeature);
                    featureImage = !string.IsNullOrEmpty(jsonData) ? JsonConvert.DeserializeObject<List<MerchantImageViewModel>>(jsonData) :
                                   new List<MerchantImageViewModel>();
                }

                DTOs.Merchant response = new DTOs.Merchant();
                DTOs.Merchant merchant = new DTOs.Merchant
                {
                    Id = request.Id,
                    Name = request.Name,
                    ShortDescription = request.ShortDescription,
                    LongDescription = request.LongDescription,
                    WebsiteUrl = request.WebSite,
                    Terms = request.Terms,
                    FeatureImageOfferText = request.FeatureImageOfferText,
                    BrandColour = request.BrandColor
                };

                if (request.Id == 0)
                {
                    if (!string.IsNullOrEmpty(request.MobilePhone) || !string.IsNullOrEmpty(request.LandlinePhone) ||
                        !string.IsNullOrEmpty(request.EmailAddress))
                    {
                        merchant.ContactDetail = new DTOs.ContactDetail
                        {
                            MobilePhone = request.MobilePhone,
                            LandlinePhone = request.LandlinePhone,
                            EmailAddress = request.EmailAddress
                        };
                    }
                    else
                    {
                        merchant.ContactDetailsId = null;
                    }
                    response = await _merchantService.Add(merchant);
                }
                else
                {
                    if (!string.IsNullOrEmpty(request.MobilePhone) || !string.IsNullOrEmpty(request.LandlinePhone) ||
                        !string.IsNullOrEmpty(request.EmailAddress))
                    {
                        DTOs.ContactDetail merchantContactDetail = new DTOs.ContactDetail
                        {
                            MobilePhone = request.MobilePhone,
                            LandlinePhone = request.LandlinePhone,
                            EmailAddress = request.EmailAddress
                        };
                        if (request.ContactDetailId.HasValue)
                        {
                            merchant.ContactDetailsId = request.ContactDetailId.Value;
                            merchantContactDetail.Id = Convert.ToInt32(request.ContactDetailId);
                            response.ContactDetail = await _contactService.Update(merchantContactDetail);
                        }
                        else
                        {
                            response.ContactDetail = await _contactService.Add(merchantContactDetail);
                            merchantContactDetail.Id = response.ContactDetail.Id;
                        }
                        merchant.ContactDetail = merchantContactDetail;
                    }
                    response = await _merchantService.Update(merchant);
                }
                foreach (SocialMediaItem item in request.SocialMediaLinks)
                {
                    if (request.Id == 0)
                    {
                        DTOs.MerchantSocialMediaLink merchantLinks = new DTOs.MerchantSocialMediaLink
                        {
                            MerchantId = response.Id,
                            SocialMediaCompanyId = item.SocialMediaCompanyId,
                            SocialMediaURI = item.URI
                        };
                        await _mediaLinkService.Add(merchantLinks);
                    }
                    else
                    {
                        DTOs.MerchantSocialMediaLink merchantSocialMediaLink =
                            _mediaLinkService.Get(response.Id, item.SocialMediaCompanyId);
                        if (merchantSocialMediaLink != null)
                        {
                            if (merchantSocialMediaLink.SocialMediaURI != item.URI)
                            {
                                merchantSocialMediaLink.SocialMediaURI = item.URI;
                                await _mediaLinkService.Update(merchantSocialMediaLink);
                            }
                        }
                    }
                }
                //get images add them to MerchantImages
                if (request.Id == 0)
                {
                    for (int i = 0; i < sessionImages.Count(); i++)
                    {
                        foreach (string imagePathString in sessionImages[i].ImagePaths)
                        {
                            DTOs.MerchantImage merchantImage = new DTOs.MerchantImage
                            {
                                MerchantId = response.Id,
                                ImagePath = imagePathString,
                                DisplayOrder = (short)(i + 1),
                                ImageType = (int) ImageType.Logo
                            };
                            await _merchantImageService.Add(merchantImage);
                        }
                    }

                    //Add Disabled Logo
                    foreach (var image in disabledLogo)
                    {
                        DTOs.MerchantImage disabled = new DTOs.MerchantImage
                        {
                            MerchantId = response.Id,
                            ImagePath = image.ImagePath,
                            DisplayOrder = 1,
                            ImageType = (int) ImageType.DisabledLogo
                        };
                        await _merchantImageService.Add(disabled);
                    }
                    //Add Feature Image
                    foreach (var image in featureImage)
                    {
                        DTOs.MerchantImage feature = new DTOs.MerchantImage
                        {
                            MerchantId = response.Id,
                            ImagePath = image.ImagePath,
                            DisplayOrder = 1,
                            ImageType = (int)ImageType.FeatureImage
                        };
                        await _merchantImageService.Add(feature);
                    }
                }

                HttpContext?.Session?.SetString("imageCriteria", string.Empty);
                return Json(JsonResponse<int>.SuccessResponse(response.Id));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(JsonResponse<string>.ErrorResponse("Error saving merchant details."));
            }
        }

        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            //method to delete Merchant
            try
            {
                DTOs.Merchant merchant = _merchantService.Get(id, true, true, false, true);
                merchant.IsDeleted = true;
                if (merchant.ContactDetailsId.HasValue)
                {
                    merchant.ContactDetail = await _contactService.Get(merchant.ContactDetailsId.Value);
                    merchant.ContactDetail.IsDeleted = true;
                }

                if (merchant.MerchantBranches != null && merchant.MerchantBranches.ToList().Count != 0)
                {
                    foreach (DTOs.MerchantBranch branch in merchant.MerchantBranches.ToList())
                    {
                        branch.IsDeleted = true;
                        if (branch.ContactDetail != null)
                        {
                            branch.ContactDetail.IsDeleted = true;
                        }
                    }
                }

                //GET OFFERS BASED ON MERCHANT ID
                List<DTOs.Offer> offersDto = await _offerService.GetByMerchant(id);
                int statusIdDeleted = (int) OfferStatus.Deleted; //status.FirstOrDefault(x => x.Name == Data.Constants.Status.Deleted).Id;
                if (offersDto != null && offersDto.Count > 0)
                {
                    List<DTOs.Offer> reqOffers = offersDto.Select(off => new DTOs.Offer
                    {
                        Id = off.Id,
                        MerchantId = off.MerchantId,
                        AffiliateId = off.AffiliateId,
                        OfferTypeId = off.OfferTypeId,
                        StatusId = statusIdDeleted,
                        ValidFrom = off.ValidFrom,
                        ValidTo = off.ValidTo,
                        Validindefinately = off.Validindefinately,
                        ShortDescription = off.ShortDescription,
                        LongDescription = off.LongDescription,
                        Instructions = off.Instructions,
                        Terms = off.Terms,
                        Exclusions = off.Exclusions,
                        LinkUrl = off.LinkUrl,
                        OfferCode = off.OfferCode,
                        Reoccuring = off.Reoccuring,
                        SearchRanking = off.SearchRanking,
                        Datecreated = off.Datecreated,
                        Headline = off.Headline,
                        AffiliateReference = off.AffiliateReference,
                        DateUpdated = off.DateUpdated
                    })
                        .ToList();

                    await _offerService.BulkUpdateAsync(reqOffers);
                }

                //Delete Merchant Image and Blob Storage
                List<DTOs.MerchantImage> responseMerchantImages = await _merchantImageService.GetAll(id, "", 0);
                if (responseMerchantImages != null && responseMerchantImages.Count > 0)
                {
                    foreach (DTOs.MerchantImage merchantImage in responseMerchantImages)
                    {
                        await _azureStorageProvider.DeleteImage(_settings.Value.BlobConnectionString, _settings.Value.ContainerName,
                            merchantImage.ImagePath);
                    }
                    await _merchantImageService.Delete(id, 0);
                }

                merchant.IsDeleted = true;
                await _merchantService.Update(merchant);

                return Json(JsonResponse<bool>.SuccessResponse(true));
            }
            catch (Exception)
            {
                return Json(JsonResponse<string>.ErrorResponse("Error deleting merchant."));
            }
        }

        [HttpGet]
        [ActionName("AddBranch")]
        public async Task<IActionResult> AddBranch(int merchantId, int branchId = 0)
        {
            //Common Method to return Add / Edit Pages From Merchant branch Page
            try
            {
                if (merchantId <= 0)
                {
                    return Json(JsonResponse<string>.ErrorResponse("Merchant not found."));
                }
                BranchViewModel branchViewModel = new BranchViewModel();
                await MapToBranchViewModel(branchViewModel, merchantId, branchId);
                return View("AddEditBranch", branchViewModel);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return View("Error");
            }
        }

        [HttpGet]
        [ActionName("EditBranch")]
        public async Task<IActionResult> EditBranch(int merchantId, int branchId)
        {
            //Common Method to return Add / Edit Pages From Merchant branch Page
            try
            {
                if (merchantId <= 0 || branchId <= 0)
                {
                    return Json(JsonResponse<string>.ErrorResponse("Merchant or branch not found."));
                }
                BranchViewModel branchViewModel = new BranchViewModel();
                await MapToBranchViewModel(branchViewModel, merchantId, branchId);
                return View("AddEditBranch", branchViewModel);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return View("Error");
            }
        }

        [HttpPost]
        [ActionName("SaveBranch")]
        public async Task<IActionResult> SaveBranch(BranchViewModel request)
        {
            //Method to save Merchant branch details and return MerchantId for redirect to Edit Merchant
            try
            {
                DTOs.MerchantBranch merchantBranch = new DTOs.MerchantBranch
                {
                    MerchantId = request.MerchantId,
                    Name = request.Name,
                    Mainbranch = request.Mainbranch,
                    ShortDescription = request.ShortDescription,
                    LongDescription = request.LongDescription,
                    DisplayOrder = request.DisplayOrder,
                    Notes = request.Notes
                };
                if (request.ContactDetail != null)
                    merchantBranch.ContactDetail = _mapper.Map<DTOs.ContactDetail>(request.ContactDetail);

                if (request.Id > 0)
                {
                    merchantBranch.Id = request.Id;
                    if (request.ContactDetailsId.HasValue)
                    {
                        merchantBranch.ContactDetailsId = request.ContactDetailsId.Value;
                        merchantBranch.ContactDetail.Id = request.ContactDetailsId.Value;
                    }
                    await _branchService.Update(merchantBranch);
                }
                else
                {
                    await _branchService.Add(merchantBranch);
                }

                return Json(JsonResponse<int>.SuccessResponse(request.MerchantId));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(JsonResponse<string>.ErrorResponse("Error saving merchant branch."));
            }
        }

        [HttpPost]
        [ActionName("DeleteBranch")]
        public async Task<IActionResult> DeleteBranch(int id)
        {
            //Method to delete Merchant branch
            try
            {
                DTOs.MerchantBranch merchantBranch = await _branchService.Get(id, true);
                merchantBranch.IsDeleted = true;
                if (merchantBranch.ContactDetail != null)
                {
                    merchantBranch.ContactDetail.IsDeleted = true;
                }

                await _branchService.Update(merchantBranch);

                return Json(JsonResponse<bool>.SuccessResponse(true));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(JsonResponse<string>.ErrorResponse("Error deleting branch."));
            }
        }

        [HttpGet]
        [ActionName("SearchBranch")]
        public async Task<IActionResult> SearchBranch(int merchantId, int pageNumber = 1)
        {
            //Method to retun Merchant branch details with pagination
            try
            {
                MerchantBranchListViewModel model = new MerchantBranchListViewModel();
                model = await MapToMerchantBranchListViewModel(merchantId, pageNumber);

                return PartialView("_merchantbranchList", model);
            }

            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(JsonResponse<string>.ErrorResponse("Error getting branches."));
            }
        }

        [HttpPost]
        [ActionName("SaveMerchantImage")]
        public async Task<IActionResult> SaveMerchantImage(int id, IFormFile imageFile)
        {
            try
            {
                string fileName = Guid.NewGuid().ToString("N").ToUpper();
                Bitmap image = Image.FromStream(imageFile.OpenReadStream()) as Bitmap;

                if (!string.IsNullOrEmpty(image?.ToString()))
                {
                    List<MerchantImageViewModel> sessionImages = new List<MerchantImageViewModel>();
                    if (id == 0)
                    {
                        string criteriajson = HttpContext?.Session?.GetString(Data.Constants.Keys.imageCriteria);
                        sessionImages = JsonConvert.DeserializeObject<List<MerchantImageViewModel>>(criteriajson) ?? new List<MerchantImageViewModel>();
                    }
                    List<MerchantImageViewModel> merchantImageViews = await MapImageToBlobStorage(id, image, fileName, sessionImages, ImageType.Logo);
                    if (id == 0)
                    {
                        var criteriajson = JsonConvert.SerializeObject(merchantImageViews);
                        HttpContext?.Session?.SetString(Data.Constants.Keys.imageCriteria, criteriajson);
                    }
                    return PartialView("_carouselView", merchantImageViews.Where(x => x.ImageType == (int)ImageType.Logo).ToList());
                }
                return Json(JsonResponse<string>.ErrorResponse("Error while adding image please try again."));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(JsonResponse<string>.ErrorResponse("Error while adding image please try again."));
            }
        }

        [HttpGet]
        [ActionName("Cancel")]
        public async Task<IActionResult> Cancel()
        {
            try
            {
                string criteriajson = HttpContext?.Session?.GetString("imageCriteria");
                List<MerchantImageViewModel> sessionImages = JsonConvert.DeserializeObject<List<MerchantImageViewModel>>(criteriajson) ??
                                                             new List<MerchantImageViewModel>();

                await Task.WhenAll(sessionImages.Select(async image =>
                {
                    await _azureStorageProvider.DeleteImage(_settings.Value.BlobConnectionString,
                        _settings.Value.ContainerName, image.ImagePath);
                }));

                return Json(JsonResponse<bool>.SuccessResponse(true));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(JsonResponse<string>.ErrorResponse("Error deleting images."));
            }
        }

        [HttpPost]
        [ActionName("DeleteMerchantImage")]
        public async Task<IActionResult> DeleteMerchantImage(MerchantImageViewModel viewModel)
        {
            try
            {
                List<MerchantImageViewModel> sessionImages = new List<MerchantImageViewModel>();
                if (viewModel.Id == 0 && viewModel.MerchantId == 0)
                {
                    //Add - Delete Image
                    string criteriajson = HttpContext?.Session?.GetString(Data.Constants.Keys.imageCriteria);
                    sessionImages = JsonConvert.DeserializeObject<List<MerchantImageViewModel>>(criteriajson) ?? new List<MerchantImageViewModel>();
                }
                List<MerchantImageViewModel> merchantImageViewModels = new List<MerchantImageViewModel>();

                if (viewModel.Id == 0 && viewModel.MerchantId == 0)
                {
                    //Add - delete Image case
                    for (int i = 0; i < sessionImages.Count(); i++)
                    {
                        if ((short)i == viewModel.DisplayOrder)
                        {
                            await Task.WhenAll(sessionImages.Select(async image =>
                            {
                                await _azureStorageProvider.DeleteImage(_settings.Value.BlobConnectionString, _settings.Value.ContainerName, image.ImagePath);
                            }));
                        }
                        else
                        {
                            MerchantImageViewModel merchantImage = new MerchantImageViewModel
                            {
                                Id = sessionImages[i].Id,
                                ImagePath = sessionImages[i].ImagePath,
                                ImagePaths = sessionImages[i].ImagePaths,
                                MerchantId = sessionImages[i].MerchantId,
                                DisplayOrder = (short)merchantImageViewModels.Count(),
                                ImageType = (int) ImageType.Logo
                            };
                            merchantImageViewModels.Add(merchantImage);
                        }
                    }
                }
                else
                {
                    //Edit - delete image case
                    List<DTOs.MerchantImage> responseMerchantImages = await _merchantImageService.GetAll(viewModel.MerchantId, "", viewModel.DisplayOrder);
                    responseMerchantImages = responseMerchantImages.Where(x => x.ImageType == (int) ImageType.Logo).ToList();

                    if (responseMerchantImages?.Count > 0)
                    {
                        await Task.WhenAll(responseMerchantImages.Select(async image =>
                        {
                            await _azureStorageProvider.DeleteImage(_settings.Value.BlobConnectionString, _settings.Value.ContainerName, image.ImagePath);
                        }));
                    }
                    await _merchantImageService.Delete(viewModel.MerchantId, viewModel.DisplayOrder);

                    responseMerchantImages = await _merchantImageService.GetAll(viewModel.MerchantId, MEDIUM_SUFFIX, 0);
                    merchantImageViewModels.AddRange(responseMerchantImages.Select(merchantImage => new MerchantImageViewModel
                    {
                        Id = merchantImage.Id,
                        MerchantId = merchantImage.MerchantId,
                        ImagePath = merchantImage.ImagePath,
                        DisplayOrder = merchantImage.DisplayOrder,
                        ImageType = (int) merchantImage.ImageType
                    }));
                }

                if (viewModel.Id == 0 && viewModel.MerchantId == 0)
                {
                    var criteriajson = JsonConvert.SerializeObject(merchantImageViewModels);
                    HttpContext?.Session?.SetString(Data.Constants.Keys.imageCriteria, criteriajson);
                }
                return PartialView("_carouselView", merchantImageViewModels.Where(x => x.ImageType == (int) ImageType.Logo).ToList());
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(JsonResponse<string>.ErrorResponse("Error deleting image."));
            }
        }

        [HttpPost]
        [ActionName("DeleteFeatureImage")]
        public async Task<IActionResult> DeleteFeatureImage(MerchantImageViewModel viewModel)
        {
            try
            {
                List<MerchantImageViewModel> sessionImages = new List<MerchantImageViewModel>();
                if (viewModel.Id == 0 && viewModel.MerchantId == 0)
                {
                    //Add - Delete Image
                    string criteriajson = HttpContext?.Session?.GetString(Data.Constants.Keys.imageFeature);
                    sessionImages = JsonConvert.DeserializeObject<List<MerchantImageViewModel>>(criteriajson) ?? new List<MerchantImageViewModel>();
                }
                List<MerchantImageViewModel> merchantImageViewModels = new List<MerchantImageViewModel>();

                if (viewModel.Id == 0 && viewModel.MerchantId == 0)
                {
                    //Add - delete Image case
                    for (int i = 0; i < sessionImages.Count(); i++)
                    {
                        if ((short)i == viewModel.DisplayOrder)
                        {
                            await Task.WhenAll(sessionImages.Select(async image =>
                            {
                                await _azureStorageProvider.DeleteImage(_settings.Value.BlobConnectionString, _settings.Value.ContainerName, image.ImagePath);
                            }));
                        }
                        else
                        {
                            MerchantImageViewModel merchantImage = new MerchantImageViewModel
                            {
                                Id = sessionImages[i].Id,
                                ImagePath = sessionImages[i].ImagePath,
                                ImagePaths = sessionImages[i].ImagePaths,
                                MerchantId = sessionImages[i].MerchantId,
                                DisplayOrder = (short)merchantImageViewModels.Count(),
                                ImageType = (int)ImageType.FeatureImage
                            };
                            merchantImageViewModels.Add(merchantImage);
                        }
                    }
                }
                else
                {
                    //Edit - delete image case
                    List<DTOs.MerchantImage> responseMerchantImages = await _merchantImageService.GetAll(viewModel.MerchantId, "", viewModel.DisplayOrder);
                    responseMerchantImages = responseMerchantImages.Where(x => x.ImageType == (int)ImageType.FeatureImage).ToList();
                    if (responseMerchantImages?.Count > 0)
                    {
                        await Task.WhenAll(responseMerchantImages.Select(async image =>
                        {
                            await _azureStorageProvider.DeleteImage(_settings.Value.BlobConnectionString, _settings.Value.ContainerName, image.ImagePath);
                        }));
                    }
                    await _merchantImageService.DeleteByMerchantIdAndType(viewModel.MerchantId,
                        (int)ImageType.FeatureImage);

                    responseMerchantImages = await _merchantImageService.GetAll(viewModel.MerchantId, MEDIUM_SUFFIX, 0);
                    merchantImageViewModels.AddRange(responseMerchantImages.Select(merchantImage => new MerchantImageViewModel
                    {
                        Id = merchantImage.Id,
                        MerchantId = merchantImage.MerchantId,
                        ImagePath = merchantImage.ImagePath,
                        DisplayOrder = merchantImage.DisplayOrder,
                        ImageType = (int)merchantImage.ImageType
                    }));
                }

                if (viewModel.Id == 0 && viewModel.MerchantId == 0)
                {
                    var criteriajson = JsonConvert.SerializeObject(merchantImageViewModels);
                    HttpContext?.Session?.SetString(Data.Constants.Keys.imageFeature, criteriajson);
                }
                return PartialView("_featureImage", merchantImageViewModels.FirstOrDefault(x => x.ImageType == (int) ImageType.FeatureImage));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(JsonResponse<string>.ErrorResponse("Error deleting image."));
            }
        }

        [HttpPost]
        [ActionName("DeleteDisabledLogo")]
        public async Task<IActionResult> DeleteDisabledLogo(MerchantImageViewModel viewModel)
        {
            try
            {
                List<MerchantImageViewModel> sessionImages = new List<MerchantImageViewModel>();
                if (viewModel.Id == 0 && viewModel.MerchantId == 0)
                {
                    //Add - Delete Image
                    string criteriajson = HttpContext?.Session?.GetString(Data.Constants.Keys.imageDisabled);
                    sessionImages = JsonConvert.DeserializeObject<List<MerchantImageViewModel>>(criteriajson) ?? new List<MerchantImageViewModel>();
                }
                List<MerchantImageViewModel> merchantImageViewModels = new List<MerchantImageViewModel>();

                if (viewModel.Id == 0 && viewModel.MerchantId == 0)
                {
                    //Add - delete Image case
                    for (int i = 0; i < sessionImages.Count(); i++)
                    {
                        if ((short)i == viewModel.DisplayOrder)
                        {
                            await Task.WhenAll(sessionImages.Select(async image =>
                            {
                                await _azureStorageProvider.DeleteImage(_settings.Value.BlobConnectionString, _settings.Value.ContainerName, image.ImagePath);
                            }));
                        }
                        else
                        {
                            MerchantImageViewModel merchantImage = new MerchantImageViewModel
                            {
                                Id = sessionImages[i].Id,
                                ImagePath = sessionImages[i].ImagePath,
                                ImagePaths = sessionImages[i].ImagePaths,
                                MerchantId = sessionImages[i].MerchantId,
                                DisplayOrder = (short)merchantImageViewModels.Count(),
                                ImageType = (int)ImageType.DisabledLogo
                            };
                            merchantImageViewModels.Add(merchantImage);
                        }
                    }
                }
                else
                {
                    //Edit - delete image case
                    List<DTOs.MerchantImage> responseMerchantImages = await _merchantImageService.GetAll(viewModel.MerchantId, "", viewModel.DisplayOrder);
                    responseMerchantImages = responseMerchantImages.Where(x => x.ImageType == (int)ImageType.DisabledLogo).ToList();

                    if (responseMerchantImages?.Count > 0)
                    {
                        await Task.WhenAll(responseMerchantImages.Select(async image =>
                        {
                            await _azureStorageProvider.DeleteImage(_settings.Value.BlobConnectionString, _settings.Value.ContainerName, image.ImagePath);
                        }));
                    }
                    await _merchantImageService.DeleteByMerchantIdAndType(viewModel.MerchantId,
                        (int)ImageType.DisabledLogo);

                    responseMerchantImages = await _merchantImageService.GetAll(viewModel.MerchantId, MEDIUM_SUFFIX, 0);
                    merchantImageViewModels.AddRange(responseMerchantImages.Select(merchantImage => new MerchantImageViewModel
                    {
                        Id = merchantImage.Id,
                        MerchantId = merchantImage.MerchantId,
                        ImagePath = merchantImage.ImagePath,
                        DisplayOrder = merchantImage.DisplayOrder,
                        ImageType = (int)merchantImage.ImageType
                    }));
                }

                if (viewModel.Id == 0 && viewModel.MerchantId == 0)
                {
                    var criteriajson = JsonConvert.SerializeObject(merchantImageViewModels);
                    HttpContext?.Session?.SetString(Data.Constants.Keys.imageDisabled, criteriajson);
                }
                return PartialView("_logo", merchantImageViewModels.FirstOrDefault(x => x.ImageType == (int) ImageType.DisabledLogo));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(JsonResponse<string>.ErrorResponse("Error deleting disabled logo."));
            }
        }

        [HttpPost]
        [ActionName("SaveDisabledLogo")]
        public async Task<IActionResult> SaveDisabledLogo(int id, IFormFile imageFile)
        {
            try
            {
                string fileName = Guid.NewGuid().ToString("N").ToUpper();
                Bitmap image = Image.FromStream(imageFile.OpenReadStream()) as Bitmap;

                if (!string.IsNullOrEmpty(image?.ToString()))
                {
                    List<MerchantImageViewModel> sessionImages = new List<MerchantImageViewModel>();
                    if (id == 0)
                    {
                        string criteriajson = HttpContext?.Session?.GetString(Data.Constants.Keys.imageDisabled);
                        if(!string.IsNullOrEmpty(criteriajson))
                        { sessionImages = JsonConvert.DeserializeObject<List<MerchantImageViewModel>>(criteriajson) ?? new List<MerchantImageViewModel>();}
                    }
                    List<MerchantImageViewModel> merchantImageViews = await MapImageToBlobStorage(id, image, fileName, sessionImages, ImageType.DisabledLogo);
                    if (id == 0)
                    {
                        var criteriajson = JsonConvert.SerializeObject(merchantImageViews);
                        HttpContext?.Session?.SetString(Data.Constants.Keys.imageDisabled, criteriajson);
                    }
                    return PartialView("_logo", merchantImageViews.FirstOrDefault(x => x.ImageType == (int) ImageType.DisabledLogo));
                }
                return Json(JsonResponse<string>.ErrorResponse("Error while adding disabled logo. Please try again."));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(JsonResponse<string>.ErrorResponse("Error while adding disabled logo. Please try again."));
            }
        }

        [HttpPost]
        [ActionName("SaveFeatureImage")]
        public async Task<IActionResult> SaveFeatureImage(int id, IFormFile imageFile)
        {
            try
            {
                string fileName = Guid.NewGuid().ToString("N").ToUpper();
                Bitmap image = Image.FromStream(imageFile.OpenReadStream()) as Bitmap;

                if (!string.IsNullOrEmpty(image?.ToString()))
                {
                    List<MerchantImageViewModel> sessionImages = new List<MerchantImageViewModel>();
                    if (id == 0)
                    {
                        string criteriajson = HttpContext?.Session?.GetString(Data.Constants.Keys.imageFeature);
                        if (!string.IsNullOrEmpty(criteriajson))
                        {
                            sessionImages = JsonConvert.DeserializeObject<List<MerchantImageViewModel>>(criteriajson) ??
                                            new List<MerchantImageViewModel>();
                        }
                    }
                    List<MerchantImageViewModel> merchantImageViews = await MapImageToBlobStorage(id, image, fileName, sessionImages, ImageType.FeatureImage);
                    if (id == 0)
                    {
                        var criteriajson = JsonConvert.SerializeObject(merchantImageViews);
                        HttpContext?.Session?.SetString(Data.Constants.Keys.imageFeature, criteriajson);
                    }
                    return PartialView("_featureImage", merchantImageViews.FirstOrDefault(x => x.ImageType == (int) ImageType.FeatureImage));
                }
                return Json(JsonResponse<string>.ErrorResponse("Error while adding feature image. Please try again."));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(JsonResponse<string>.ErrorResponse("Error while adding feature image. Please try again."));
            }
        }

        #region Private Methods

        private async Task MapToMerchantViewModel(MerchantSearchViewModel viewModel,
            MerchantListRequest req, string searchText, int pageNumber)
        {
            int pageSize = 20;
            int.TryParse(_settings.Value.PageSize, out pageSize);
            MerchantsSortOrder sortOrder = GetSortOrder(req);

            List<DTOs.Merchant> response = await _merchantService.GetAll();
            if (response != null)
            {
                DTOs.PagedResult<DTOs.Merchant> paging = await _merchantService.GetPagedResult(searchText, pageNumber, pageSize, sortOrder);
                List<SelectListItem> listofMerchant = response.Select(merchant => new SelectListItem()
                {
                    Text = merchant.Name,
                    Value = merchant.Id.ToString()
                }).ToList();
                viewModel.SearchText = searchText;
                viewModel.ListofMerchants = listofMerchant;
                viewModel.MerchantsList = new MerchantsList(req)
                {
                    ListofMerchants = paging.Results.ToList(),
                    PagingModel =
                {
                    CurrentPage = paging.CurrentPage,
                    PageCount = paging.PageCount,
                    PageSize = paging.PageSize,
                    RowCount = paging.RowCount
                },
                    CurrentPageNumber = pageNumber,
                };
                if (req.MerchantSortField == MerchantSortField.MerchantName)
                {
                    viewModel.MerchantsList.SortIcon = req.SortDirection == "desc" ? "fa fa-sort-alpha-desc" : "fa fa-sort-alpha-asc";
                }
            }



        }

        private MerchantsSortOrder GetSortOrder(MerchantListRequest req)
        {
            MerchantsSortOrder sortOrder;

            switch (req.MerchantSortField)
            {
                case MerchantSortField.MerchantName:
                    sortOrder = req.SortDirection == "asc" ? MerchantsSortOrder.MerchantNameAsc : MerchantsSortOrder.MerchantNameDesc;
                    break;

                default:
                    sortOrder = MerchantsSortOrder.MerchantNameAsc;
                    break;
            }

            return sortOrder;
        }

        private async Task<MerchantBranchListViewModel> MapToMerchantBranchListViewModel(int merchantId, int pageNumber)
        {
            var pageSize = 20;
            int.TryParse(_settings.Value.PageSize, out pageSize);
            DTOs.PagedResult<DTOs.MerchantBranch> paging =
                await _branchService.GetPagedResult(merchantId, pageNumber, pageSize);
            return new MerchantBranchListViewModel
            {
                ListofBranches = paging.Results.ToList(),
                PagingModel =
                {
                    CurrentPage = paging.CurrentPage,
                    PageCount = paging.PageCount,
                    PageSize = paging.PageSize,
                    RowCount = paging.RowCount
                },
                CurrentPageNumber = pageNumber
            };
        }

        private List<SelectListItem> MapToCountrySelectList(List<CustomCountryList> customCountryList)
        {
            return customCountryList.Select(country => new SelectListItem()
            {
                Text = country.Name,
                Value = country.Value
            })
                .ToList();
        }

        private async Task MapToBranchViewModel(BranchViewModel model, int merchantId, int branchId)
        {
            List<CustomCountryList> country = new List<CustomCountryList>
            {
                new CustomCountryList {Name = "CZ", Value = "CZ"},
                new CustomCountryList {Name = "GB", Value = "GB"},
                new CustomCountryList {Name = "PL", Value = "PL"},
                new CustomCountryList {Name = "SC", Value = "SC"},
                new CustomCountryList {Name = "SK", Value = "SK"}
            };
            if (branchId != 0)
            {
                DTOs.MerchantBranch branchDetails = await _branchService.Get(branchId, true, true);//getBranch
                if (branchDetails == null) return;
                model.MerchantName = branchDetails.Merchant.Name;
                model.MerchantId = branchDetails.MerchantId;
                model.Id = branchDetails.Id;
                if (branchDetails.ContactDetailsId.HasValue)
                {
                    model.ContactDetailsId = branchDetails.ContactDetailsId.Value;
                    model.ContactDetail = _mapper.Map<ContactsViewModel>(await _contactService.Get(branchDetails.ContactDetailsId.Value));
                }
                model.Name = branchDetails.Name;
                model.Mainbranch = branchDetails.Mainbranch;
                model.ShortDescription = branchDetails.ShortDescription;
                model.LongDescription = branchDetails.LongDescription;
                model.DisplayOrder = branchDetails.DisplayOrder;
                model.Notes = branchDetails.Notes;
                model.SelectCountry = MapToCountrySelectList(country);
            }
            else
            {
                if (merchantId == 0) return;
                DTOs.Merchant merchantDetails = _merchantService.Get(merchantId, true, false, false, true);//getMerchant
                if (merchantDetails == null) return;
                model.MerchantName = merchantDetails.Name;
                model.MerchantId = merchantId;
                model.Mainbranch = true;
                model.SelectCountry = MapToCountrySelectList(country);
                model.ContactDetail = new ContactsViewModel()
                {
                    CountryCode = "GB"
                };
            }
        }

        private async Task<List<MerchantImageViewModel>> MapImageToBlobStorage(int id, Bitmap bitmapImage,
            string imageName, List<MerchantImageViewModel> sessionMerchantImage, ImageType type)
        {
            string blobConnectionString = _settings.Value.BlobConnectionString;
            List<MerchantImageViewModel> merchantImages = new List<MerchantImageViewModel>();
            string imagePath = string.Empty;
            if (blobConnectionString != null)
            {
                if (id == 0)
                {
                    List<string> imagePaths = new List<string>();
                    //Add Case
                    RotateImage(bitmapImage);
                    foreach (string imageSize in Enum.GetNames(typeof(ImageSize)))
                    {
                        //#528, Feature images are needed in more than just the new format
                        imagePath = await ResizeImage(imageName, _settings.Value.ImageCategory, bitmapImage,
                                (ImageSize)Enum.Parse(typeof(ImageSize), imageSize), _azureStorageProvider);

                        imagePaths.Add(imagePath);
                    }
                    foreach (string imageString in imagePaths)
                    {
                        //#528 Should this be changed to the new feature format or not?
                        //if (imageString.Contains(MEDIUM_SUFFIX))
                        if ((imageString.Contains(FEATURE_SUFFIX) && type == ImageType.FeatureImage) ||
                            (imageString.Contains(MEDIUM_SUFFIX) && type != ImageType.FeatureImage))
                        {
                            MerchantImageViewModel merchantImageView = new MerchantImageViewModel
                            {
                                DisplayOrder = (short)sessionMerchantImage.Count(),
                                ImagePath = imageString,
                                ImagePaths = imagePaths,
                                ImageType = (int)type
                            };
                            sessionMerchantImage.Add(merchantImageView);
                        }
                    }
                    merchantImages = sessionMerchantImage;
                }
                else
                {
                    //Edit Case
                    int displayOrder = _merchantImageService.Get(id).DisplayOrder + 1;
                    RotateImage(bitmapImage);
                    foreach (string imageSize in Enum.GetNames(typeof(ImageSize)))
                    {
                        //#528, Feature images are needed in more than just the new format
                        imagePath = await ResizeImage(imageName, _settings.Value.ImageCategory, bitmapImage,
                        (ImageSize)Enum.Parse(typeof(ImageSize), imageSize), _azureStorageProvider);

                        if (imagePath != null)
                        {
                            DTOs.MerchantImage response = new DTOs.MerchantImage();
                            DTOs.MerchantImage merchantImage = new DTOs.MerchantImage
                            {
                                MerchantId = id,
                                ImagePath = imagePath,
                                DisplayOrder = (short)(displayOrder),
                                ImageType = (int)type
                            };
                            await _merchantImageService.Add(merchantImage);
                        }
                    }
                    //#528 Should this be changed to the new feature format or not?
                    //List<DTOs.MerchantImage> responseMerchantImages = await _merchantImageService.GetAll(id, MEDIUM_SUFFIX, 0);
                    if (type != ImageType.FeatureImage)
                    {
                        List<DTOs.MerchantImage> responseMerchantImages = await _merchantImageService.GetAll(id, MEDIUM_SUFFIX, 0);
                        merchantImages.AddRange(responseMerchantImages.Select(merchantImage => new MerchantImageViewModel
                        {
                            Id = merchantImage.Id,
                            MerchantId = merchantImage.MerchantId,
                            ImagePath = merchantImage.ImagePath,
                            DisplayOrder = merchantImage.DisplayOrder,
                            ImageType = (int)merchantImage.ImageType
                        }));
                    }
                    else
                    {
                        List<DTOs.MerchantImage> responseMerchantImages = await _merchantImageService.GetAll(id, FEATURE_SUFFIX, 0);
                        merchantImages.AddRange(responseMerchantImages.Select(merchantImage => new MerchantImageViewModel
                        {
                            Id = merchantImage.Id,
                            MerchantId = merchantImage.MerchantId,
                            ImagePath = merchantImage.ImagePath,
                            DisplayOrder = merchantImage.DisplayOrder,
                            ImageType = (int)merchantImage.ImageType
                        }));
                    }

                }
            }
            return merchantImages;
        }

        private async Task<string> ResizeImage(string imageName, string imageCategory,
            Bitmap image, ImageSize size, IAzureStorageProvider azureStorageProvider)
        {
            int imageHeight = 0, imageWidth = 0;
            string suffix = "";
            string path = "";
            byte[] result = null;

            switch (size)
            {
                case ImageSize.Thumbnail:
                    suffix = THUMBNAIL_SUFFIX;
                    if (image.Width > image.Height)
                    {
                        imageWidth = _thumbnailWidth;
                        imageHeight = (image.Height * imageWidth) / image.Width;
                    }
                    else
                    {
                        imageHeight = _thumbnailHeight;
                        imageWidth = (image.Width * imageHeight) / image.Height;
                    }
                    break;

                case ImageSize.Medium:
                    suffix = MEDIUM_SUFFIX;
                    if (image.Width > image.Height)
                    {
                        imageWidth = _mediumWidth;
                        imageHeight = (image.Height * imageWidth) / image.Width;
                    }
                    else
                    {
                        imageHeight = _mediumHeight;
                        imageWidth = (image.Width * imageHeight) / image.Height;
                    }
                    break;

                case ImageSize.Large:
                    suffix = LARGE_SUFFIX;
                    if (image.Width > image.Height)
                    {
                        imageWidth = _largeWidth;
                        imageHeight = (image.Height * imageWidth) / image.Width;
                    }
                    else
                    {
                        imageHeight = _largeHeight;
                        imageWidth = (image.Width * imageHeight) / image.Height;
                    }
                    break;
                case ImageSize.Feature:
                    suffix = FEATURE_SUFFIX;
                    //#528 Fixed size or replicate others?...
                    //imageWidth = _featureWidth;
                    //imageHeight = _featureHeight;
                    if (image.Width > image.Height)
                    {
                        imageWidth = _featureWidth;
                        imageHeight = (image.Height * imageWidth) / image.Width;
                    }
                    else
                    {
                        imageHeight = _featureHeight;
                        imageWidth = (image.Width * imageHeight) / image.Height;
                    }
                    break;
            }
            if (image != null)
            {
                Bitmap newImage = new Bitmap(image, imageWidth, imageHeight);

                MemoryStream stream = new MemoryStream();
                newImage.Save(stream, ImageFormat.Jpeg);
                result = stream.ToArray();
            }

            path = await azureStorageProvider.SaveImage(_settings.Value.BlobConnectionString,
                _settings.Value.ContainerName, _settings.Value.ImageCategory,
                Path.GetFileNameWithoutExtension(imageName) + suffix + Path.GetExtension(imageName), result);
            return path;
        }

        private bool RotateImage(Bitmap img)
        {

            if (img == null) return false;

            // The EXIF tag for orientation always has ID 0x0112 which is HEX of 274
            if (Array.IndexOf(img.PropertyIdList, 274) <= -1)
            {
                // no resize but no failure
                return true;
            }

            RotateFlipType rotateFlipType = RotateFlipType.RotateNoneFlipNone;

            rotateFlipType = GetImageRotation(img);

            if (rotateFlipType != RotateFlipType.RotateNoneFlipNone)
            {
                try
                {
                    img.RotateFlip(rotateFlipType);
                }
                catch (Exception)
                {
                    // exception here is not the end of the world
                    // we should log (but no logger)
                    return false;
                }
            }

            // The EXIF data is now invalid (specifies rotation ontop of our above rotation) so needs to be removed.
            img.RemovePropertyItem(274);

            return true;
        }

        private RotateFlipType GetImageRotation(Bitmap img)
        {
            // There are 8 possible value for orientation, find and rotate image using relevant transformation
            int orientation = img.GetPropertyItem(274).Value[0];

            RotateFlipType rotateFlipType = RotateFlipType.RotateNoneFlipNone;

            switch (orientation)
            {
                case 1:
                    // No rotation required.
                    break;
                case 2:
                    rotateFlipType = RotateFlipType.RotateNoneFlipX;
                    break;
                case 3:
                    rotateFlipType = RotateFlipType.Rotate180FlipNone;
                    break;
                case 4:
                    rotateFlipType = RotateFlipType.Rotate180FlipX;
                    break;
                case 5:
                    rotateFlipType = RotateFlipType.Rotate90FlipX;
                    break;
                case 6:
                    rotateFlipType = RotateFlipType.Rotate90FlipNone;
                    break;
                case 7:
                    rotateFlipType = RotateFlipType.Rotate270FlipX;
                    break;
                case 8:
                    rotateFlipType = RotateFlipType.Rotate270FlipNone;
                    break;
            }

            return rotateFlipType;
        }

        #endregion
    }
}
