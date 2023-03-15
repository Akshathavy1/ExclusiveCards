using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using ExclusiveCard.Enums;
using ExclusiveCard.Providers;
using ExclusiveCard.Services.Interfaces.Admin;
using ExclusiveCard.Services.Models.DTOs;
using ExclusiveCard.WebAdmin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ExclusiveCard.WebAdmin.Controllers
{
    [Authorize(Roles = "AdminUser, BackOfficeUser")]
    [SessionTimeout]
    public class TalkSportController : BaseController
    {

        #region Private Members
        private readonly ITalkSportService _talkSportService;
        private readonly IOptions<TypedAppSettings> _settings;
        private readonly IAzureStorageProvider _azureStorageProvider;

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

        public TalkSportController(ITalkSportService talkSportService, IOptions<TypedAppSettings> settings, IAzureStorageProvider azureStorageProvider,IConfiguration configuration)
        {
            _talkSportService = talkSportService;
            _settings = settings;
            _azureStorageProvider = azureStorageProvider;
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
       
        #region Public Methods

        public async Task<IActionResult> Index()
        {
            var talkSportViewModel = await MapToTalkSportViewModel();
            talkSportViewModel.SiteCategoryId = 1;
            return View("Index", talkSportViewModel);
        }

        [HttpGet]
        [ActionName("SearchClub")]
        public async Task<IActionResult> SearchClub(int leagueId,int siteCategoryId,int pageNumber)
        {
            var talkSportViewModel = await MapToTalkSportViewModel();
            int pageSize = 10;
            talkSportViewModel.SiteClan = await _talkSportService.SearchClub(leagueId, siteCategoryId,pageNumber, pageSize);
            talkSportViewModel.LeagueId = leagueId;
            talkSportViewModel.SiteCategoryId = siteCategoryId;

            talkSportViewModel.Paging = new PagingViewModel
            {
                CurrentPage = talkSportViewModel.SiteClan.CurrentPage,
                PageCount = talkSportViewModel.SiteClan.PageCount,
                PageSize = talkSportViewModel.SiteClan.PageSize,
                RowCount = talkSportViewModel.SiteClan.RowCount
            };

            talkSportViewModel.Page = pageNumber;
            return View("Index", talkSportViewModel);
        }

        [HttpPost]
        [ActionName("UpdateSiteClan")]
        public async Task<IActionResult> UpdateSiteClan(int id, int league,int charity)
        {
            try
            {
                if (id > 0)
                {
                    var siteClan = await _talkSportService.GetSiteClanById(id);
                    siteClan.Id = id;
                    siteClan.LeagueId = league;
                    siteClan.CharityId = charity;
                    var result = await _talkSportService.UpdateSiteClan(siteClan);
                }

                return Json(JsonResponse<string>.SuccessResponse("Updated successfully."));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(JsonResponse<string>.ErrorResponse("Some error occurred. Please try again."));
            }
        }

        [HttpPost]
        [ActionName("UpdateSiteClanImage")]
        public async Task<IActionResult> UpdateSiteClanImage(int id, IFormFile imageFile)
        {
            try
            {
                string fileName = Guid.NewGuid().ToString("N").ToUpper();
                Bitmap image = Image.FromStream(imageFile.OpenReadStream()) as Bitmap;

                if (!string.IsNullOrEmpty(image?.ToString()) && id > 0)
                {
                    SiteClan clan = await MapImageToBlobStorage(id, image, fileName);
                    if (clan!=null)
                    {
                        return Json(JsonResponse<string>.SuccessResponse(clan.ImagePath));
                    }
                }
                return Json(JsonResponse<string>.ErrorResponse("Error while adding image please try again."));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(JsonResponse<string>.ErrorResponse("Error while adding image please try again."));
            }
        }



        [HttpPost]
        [ActionName("DeleteSiteClanImage")]
        public async Task<IActionResult> DeleteSiteClanImage(int id)
        {
            try
            {
               
                SiteClan clan = await MapImageToBlobStorage(id, null, null);
                if (clan.ImagePath == null)
                {
                    return Json(JsonResponse<string>.SuccessResponse("Deleted Successfully."));
                }
                return Json(JsonResponse<string>.ErrorResponse("Error while deleting image please try again."));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(JsonResponse<string>.ErrorResponse("Error while deleting image please try again."));
            }
        }
        #endregion



        #region Private Methods

        private async Task<SiteClan> MapImageToBlobStorage(int id, Bitmap bitmapImage,
          string imageName)
        {
            string blobConnectionString = _settings.Value.BlobConnectionString;
            SiteClan clan = new SiteClan();
            string imagePath = string.Empty;
            if (blobConnectionString != null)
            {
                var siteClan = await _talkSportService.GetSiteClanById(id);
                if (!string.IsNullOrEmpty(siteClan.ImagePath))
                {
                    //deleting existing image
                    await _azureStorageProvider.DeleteImage(_settings.Value.BlobConnectionString, _settings.Value.ContainerName,
                    siteClan.ImagePath);
                }

                if (bitmapImage !=null)
                {
                    RotateImage(bitmapImage);
                    foreach (string imageSize in Enum.GetNames(typeof(ImageSize)))
                    {
                        imagePath = await ResizeImage(imageName, _settings.Value.ImageCategory, bitmapImage,
                            (ImageSize)Enum.Parse(typeof(ImageSize), imageSize), _azureStorageProvider);
                    }
                }

                siteClan.Id = id;
                if (!string.IsNullOrEmpty(imagePath))
                {
                    siteClan.ImagePath = imagePath;
                }
                else
                {
                    siteClan.ImagePath = null;
                }
                clan = await _talkSportService.UpdateSiteClan(siteClan);

            }
            return clan;
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
                _settings.Value.ContainerName, imageCategory,
                Path.GetFileNameWithoutExtension(imageName) + suffix + Path.GetExtension(imageName), result);
            return path;
        }

        private async Task<TalkSportViewModel> MapToTalkSportViewModel()
        {
            try
            {
                TalkSportViewModel viewModel = new TalkSportViewModel();
                var leagues = await _talkSportService.GetAllLeagues();
                var siteCategories = await _talkSportService.GetAllSiteCategory();
                var charities = await _talkSportService.GetAllCharity();
                if (leagues != null)
                {
                    viewModel.Leagues = new List<SelectListItem>();
                    foreach (var item in leagues)
                    {
                        SelectListItem value = new SelectListItem()
                        {
                            Text = item.Description,
                            Value = item.Id.ToString()
                        };
                        viewModel.Leagues.Add(value);
                    }
                }

                if (siteCategories != null)
                {
                    viewModel.SiteCategories = new List<SelectListItem>();
                    foreach (var item in siteCategories)
                    {
                        SelectListItem value = new SelectListItem()
                        {
                            Text = item.Description,
                            Value = item.Id.ToString()
                        };
                        viewModel.SiteCategories.Add(value);
                    }
                }

                if (charities != null)
                {
                    viewModel.Charity = new List<SelectListItem>();
                    foreach (var item in charities)
                    {
                        SelectListItem value = new SelectListItem()
                        {
                            Text = item.CharityName,
                            Value = item.Id.ToString()
                        };
                        viewModel.Charity.Add(value);
                    }
                }

                return viewModel;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
          
        }

        #endregion
    }
}