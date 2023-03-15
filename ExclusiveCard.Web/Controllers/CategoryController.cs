using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ExclusiveCard.Enums;
using ExclusiveCard.Providers;
using ExclusiveCard.Services.Interfaces.Admin;
using ExclusiveCard.Services.Models.DTOs;
using ExclusiveCard.WebAdmin.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace ExclusiveCard.WebAdmin.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "AdminUser, BackOfficeUser")]
    public class CategoryController : BaseController
    {
        #region Private Members and Constructor

        private readonly IMemoryCache _cache;
        private readonly ICategoryService _categoryService;
        private readonly IMerchantService _merchantService;
        private readonly IAzureStorageProvider _azureStorageProvider;
        private readonly IOptions<TypedAppSettings> _settings;

        private const string THUMBNAIL_SUFFIX = "__1";
        private const string MEDIUM_SUFFIX = "__2";
        private const string LARGE_SUFFIX = "__3";
        private const string FEATURE_SUFFIX = "__4";

        private IConfiguration AppSettings { get; set; }

        private readonly int _thumbnailHeight;
        private readonly int _thumbnailWidth;
        private readonly int _mediumHeight;
        private readonly int _mediumWidth;
        private readonly int _largeHeight;
        private readonly int _largeWidth;
        private readonly int _featureHeight;
        private readonly int _featureWidth;

        public CategoryController(IMemoryCache cache,
            ICategoryService categoryService,
            IMerchantService merchantService,
            IAzureStorageProvider azureStorageProvider,
            IOptions<TypedAppSettings> settings,
            IConfiguration configuration)
        {
            _cache = cache;
            _categoryService = categoryService;
            _merchantService = merchantService;
            _azureStorageProvider = azureStorageProvider;
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
        [ActionName("List")]
        public IActionResult List()
        {
            List<TreeItem<Category>> categories;
            try
            {
                categories = _categoryService.GetTree();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
            return View(categories);
        }


        [HttpGet]
        [ActionName("Edit")]
        public async Task<IActionResult> Edit(int id)
        {
            CategoryViewModel model = new CategoryViewModel();
            try
            {
                //Get category and category
                
                var categories = _categoryService.GetAll();

                if (categories.Count > 0)
                {
                    var category = categories.FirstOrDefault(x => x.Id == id);

                    if (category != null)
                    {
                        model.Id = category.Id;
                        model.Code = category.Code;
                        model.Name = category.Name;
                        model.ParentId = category.ParentId;
                        model.IsActive = category.IsActive;
                        model.DisplayOrder = category.DisplayOrder;
                        model.UrlSlug = category.UrlSlug;

                        if (category.CategoryFeatureDetails != null && category.CategoryFeatureDetails.Count > 0)
                        {
                            var item = category.CategoryFeatureDetails.FirstOrDefault(x => x.CountryCode == "GB") ??
                                       category.CategoryFeatureDetails.FirstOrDefault();
                            if (item != null)
                            {
                                model.Feature = new CategoryFeatureViewModel
                                {
                                    CategoryId = item.CategoryId,
                                    FeatureMerchantId = item.FeatureMerchantId,
                                    CountryCode = item.CountryCode,
                                    SelectedImage = new MerchantImageViewModel
                                    {
                                        Id = category.Id,
                                        MerchantId = item.FeatureMerchantId,
                                        ImagePath = item.SelectedImage,
                                        DisplayOrder = 1
                                    },
                                    UnselectedImage = new MerchantImageViewModel
                                    {
                                        Id = category.Id,
                                        MerchantId = item.FeatureMerchantId,
                                        ImagePath = item.UnselectedImage,
                                        DisplayOrder = 1
                                    },
                                };
                            }
                            else //Ideally this shouldn't be hit
                            {
                                model.Feature = new CategoryFeatureViewModel
                                {
                                    CategoryId = category.Id,
                                    CountryCode = "GB"
                                };
                            }
                        }
                        else
                        {
                            model.Feature = new CategoryFeatureViewModel
                            {
                                CategoryId = category.Id,
                                CountryCode = "GB"
                            };
                        }
                    }

                    categories = categories.Where(x => x.ParentId == 0).ToList();
                    await Task.WhenAll(categories.Select(async item =>
                    {
                        model.Parents.Add(new SelectListItem
                        {
                            Text = item.Name,
                            Value = item.Id.ToString()
                        });
                        await Task.CompletedTask;
                    }));
                }

                //Get List of merchants
                List<Merchant> response = await _merchantService.GetAll();
                await Task.WhenAll(response.Select(async merchant =>
                {
                    model.Feature.Merchants.Add(new SelectListItem()
                    {
                        Text = merchant.Name,
                        Value = merchant.Id.ToString()
                    });
                    await Task.CompletedTask;
                }));
                _cache.Set(Data.Constants.Keys.MerchantSelectList, model.Feature.Merchants);
                model.Feature.Countries = GetCountrySelectList();
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return View("Error");
            }

            return View(model);
        }

        [HttpPost]
        [ActionName("Save")]
        public IActionResult Save(CategoryViewModel model)
        {
            try
            {
                Category existingCategory = _cache.Get<Category>($"{Data.Constants.Keys.Category}_{model.Id}") ??
                                            _categoryService.GetById(model.Id);

                if (existingCategory == null)
                {
                    return Json(JsonResponse<string>.ErrorResponse("Category not found."));
                }

                existingCategory.Name = model.Name;
                existingCategory.ParentId = model.ParentId;

                _categoryService.Update(existingCategory);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return Json(JsonResponse<string>.ErrorResponse("Error saving category."));
            }

            return Json(JsonResponse<bool>.SuccessResponse(true));
        }

        [HttpPost]
        [ActionName("DeleteSelectedImage")]
        public async Task<IActionResult> DeleteSelectedImage(MerchantImageViewModel viewModel, string countryCode)
        {
            try
            {
                //get category
                if (viewModel.Id > 0)
                {
                    var category = _categoryService.GetById(viewModel.Id);
                    var feature = category.CategoryFeatureDetails?.FirstOrDefault(x => x.CountryCode == countryCode);

                    if (feature != null && !string.IsNullOrEmpty(feature.SelectedImage))
                    {
                        try
                        {
                            await _azureStorageProvider.DeleteImage(_settings.Value.BlobConnectionString,
                                _settings.Value.ContainerName, feature.SelectedImage);
                        }
                        catch
                        {
                            Logger.Error($"Error deleting image from the blob for category - {viewModel.Id}");
                        }

                        feature.SelectedImage = null;
                        _categoryService.UpdateCategoryFeatureDetail(feature);
                    }
                }
                else
                {
                    return Json(JsonResponse<string>.ErrorResponse("Could not find the category"));
                }

                
                var model = new MerchantImageViewModel
                {
                    Id = viewModel.Id,
                    MerchantId = viewModel.MerchantId,
                    ImagePath = null,
                    DisplayOrder = 1
                };

                
                return PartialView("_featureImage", model);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(JsonResponse<string>.ErrorResponse("Error deleting selected image."));
            }
        }

        [HttpPost]
        [ActionName("SaveSelectedImage")]
        public async Task<IActionResult> SaveSelectedImage(int id, int merchantId, IFormFile imageFile, string countryCode)
        {
            try
            {
                string fileName = Guid.NewGuid().ToString("N").ToUpper();
                Bitmap image = Image.FromStream(imageFile.OpenReadStream()) as Bitmap;

                if (!string.IsNullOrEmpty(image?.ToString()))
                {
                    MerchantImageViewModel model = new MerchantImageViewModel();
                    string imagePath = await MapImageToBlobStorage(image, fileName);
                    //Add this to category
                    var category = _categoryService.GetById(id);
                    if (category != null)
                    {
                        var feature = category.CategoryFeatureDetails?.FirstOrDefault(x => x.CountryCode == countryCode);

                        if (feature == null)
                        {
                            CategoryFeatureDetail detail = new CategoryFeatureDetail
                            {
                                CategoryId = id,
                                CountryCode = countryCode,
                                FeatureMerchantId = merchantId,
                                SelectedImage = imagePath
                            };
                            _categoryService.AddCategoryFeatureDetail(detail);
                        }
                        else {
                            if (!string.IsNullOrEmpty(feature.SelectedImage))
                            {
                                //delete image from blob
                                try
                                {
                                    await _azureStorageProvider.DeleteImage(_settings.Value.BlobConnectionString,
                                        _settings.Value.ContainerName, feature.SelectedImage);
                                }
                                catch
                                {
                                    Logger.Error($"Selected image not found in Blob for category - {category.Id}");
                                }
                            }
                            feature.FeatureMerchantId = merchantId;
                            feature.SelectedImage = imagePath;
                            _categoryService.UpdateCategoryFeatureDetail(feature);
                        }

                        model = new MerchantImageViewModel
                        {
                            Id = id,
                            MerchantId = merchantId,
                            ImagePath = imagePath
                        };
                    }

                    return PartialView("_featureImage", model);
                }
                return Json(JsonResponse<string>.ErrorResponse("Error while adding selected image. Please try again."));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(JsonResponse<string>.ErrorResponse("Error while adding selected image. Please try again."));
            }
        }

        [HttpPost]
        [ActionName("SaveUnselectedImage")]
        public async Task<IActionResult> SaveUnselectedImage(int id, int merchantId, IFormFile imageFile, string countryCode)
        {
            try
            {
                string fileName = Guid.NewGuid().ToString("N").ToUpper();
                Bitmap image = Image.FromStream(imageFile.OpenReadStream()) as Bitmap;

                if (!string.IsNullOrEmpty(image?.ToString()))
                {
                    MerchantImageViewModel model = new MerchantImageViewModel();
                    string imagePath = await MapImageToBlobStorage(image, fileName);
                    //Add this to category
                    var category = _categoryService.GetById(id);
                    if (category != null)
                    {
                        var feature = category.CategoryFeatureDetails?.FirstOrDefault(x => x.CountryCode == countryCode);

                        if (feature == null)
                        {
                            CategoryFeatureDetail detail = new CategoryFeatureDetail
                            {
                                CategoryId = id,
                                CountryCode = countryCode,
                                FeatureMerchantId = merchantId,
                                UnselectedImage = imagePath
                            };
                            _categoryService.AddCategoryFeatureDetail(detail);
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(feature.UnselectedImage))
                            {
                                //delete image from blob
                                try
                                {
                                    await _azureStorageProvider.DeleteImage(_settings.Value.BlobConnectionString,
                                        _settings.Value.ContainerName, feature.UnselectedImage);
                                }
                                catch
                                {
                                    Logger.Error($"Unselected image not found in Blob for category - {category.Id}");
                                }
                            }
                            feature.FeatureMerchantId = merchantId;
                            feature.UnselectedImage = imagePath;
                            _categoryService.UpdateCategoryFeatureDetail(feature);
                        }

                        model = new MerchantImageViewModel
                        {
                            Id = id,
                            MerchantId = merchantId,
                            ImagePath = imagePath
                        };
                    }
                    return PartialView("_logo", model);
                }
                return Json(JsonResponse<string>.ErrorResponse("Error while adding unselected image. Please try again."));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(JsonResponse<string>.ErrorResponse("Error while adding unselected image. Please try again."));
            }
        }

        [HttpPost]
        [ActionName("DeleteUnselectedImage")]
        public async Task<IActionResult> DeleteUnselectedImage(MerchantImageViewModel viewModel, string countryCode)
        {
            try
            {
                //get category
                if (viewModel.Id > 0)
                {
                    var category = _categoryService.GetById(viewModel.Id);
                    var feature = category.CategoryFeatureDetails?.FirstOrDefault(x => x.CountryCode == countryCode);

                    if (feature != null && !string.IsNullOrEmpty(feature.UnselectedImage))
                    {
                        try
                        {
                            await _azureStorageProvider.DeleteImage(_settings.Value.BlobConnectionString,
                                _settings.Value.ContainerName, feature.UnselectedImage);
                        }
                        catch
                        {
                            Logger.Error($"Error deleting image from the blob for category - {viewModel.Id}");
                        }

                        feature.UnselectedImage = null;
                        _categoryService.UpdateCategoryFeatureDetail(feature);
                    }
                }
                else
                {
                    return Json(JsonResponse<string>.ErrorResponse("Could not find the category"));
                }
                
                var model = new MerchantImageViewModel
                {
                    Id = viewModel.Id,
                    MerchantId = viewModel.MerchantId,
                    ImagePath = null,
                    DisplayOrder = 1
                };

                return PartialView("_logo", model);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(JsonResponse<string>.ErrorResponse("Error deleting selected image."));
            }
        }

        [HttpGet]
        [ActionName("GetFeature")]
        public async Task<IActionResult> GetFeature(int id, string countryCode)
        {
            try
            {
                CategoryFeatureViewModel model;

                //Get category feature
                var feature = _categoryService.GetFeatureDetail(id, countryCode);
                if (feature != null)
                {
                    model = new CategoryFeatureViewModel
                    {
                        CategoryId = feature.CategoryId,
                        CountryCode = feature.CountryCode,
                        FeatureMerchantId = feature.FeatureMerchantId,
                        SelectedImage = new MerchantImageViewModel
                        {
                            Id = feature.CategoryId,
                            MerchantId = feature.FeatureMerchantId,
                            ImagePath = feature.SelectedImage,
                            DisplayOrder = 1
                        },
                        UnselectedImage = new MerchantImageViewModel
                        {
                            Id = feature.CategoryId,
                            MerchantId = feature.FeatureMerchantId,
                            ImagePath = feature.UnselectedImage,
                            DisplayOrder = 1
                        }
                    };
                }
                else
                {
                    model = new CategoryFeatureViewModel
                    {
                        CountryCode = countryCode,
                        CategoryId = id
                    };
                }

                model.Countries = GetCountrySelectList();
                model.Merchants = _cache.Get<List<SelectListItem>>(Data.Constants.Keys.MerchantSelectList);
                if (model.Merchants == null || model.Merchants.Count == 0)
                {
                    List<Merchant> response = await _merchantService.GetAll();
                    await Task.WhenAll(response.Select(async merchant =>
                    {
                        model.Merchants.Add(new SelectListItem()
                        {
                            Text = merchant.Name,
                            Value = merchant.Id.ToString()
                        });
                        await Task.CompletedTask;
                    }));
                }

                return PartialView("_feature", model);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return Json(
                    JsonResponse<string>.ErrorResponse(
                        "Error occurred while trying to fetch details for the selected country."));
            }
        }

        #region Private Methods

        private async Task<string> MapImageToBlobStorage(Bitmap bitmapImage,
            string imageName)
        {
            string blobConnectionString = _settings.Value.BlobConnectionString;
            string imagePath = string.Empty;
            if (blobConnectionString != null)
            {
                //Edit Case
                RotateImage(bitmapImage);
                foreach (string imageSize in Enum.GetNames(typeof(ImageSize)))
                {
                    //#528 - Should we be filtering out the new 'ImageSize.Feature' here?
                    //if(imageSize != ImageSize.Feature.ToString())
                    imagePath = await ResizeImage(imageName, _settings.Value.ImageCategory, bitmapImage,
                        (ImageSize)Enum.Parse(typeof(ImageSize), imageSize), _azureStorageProvider);
                }
            }
            return imagePath;
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
                    //Fixed size or replicate others?...
                    imageWidth = _featureWidth;
                    imageHeight = _featureHeight;
                    //if (image.Width > image.Height)
                    //{
                    //    imageWidth = _featureWidth;
                    //    imageHeight = (image.Height * imageWidth) / image.Width;
                    //}
                    //else
                    //{
                    //    imageHeight = _featureHeight;
                    //    imageWidth = (image.Width * imageHeight) / image.Height;
                    //}
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

        private List<SelectListItem> GetCountrySelectList()
        {
            return new List<SelectListItem>
                {
                    new SelectListItem{ Text = "CZ", Value = "CZ"},
                    new SelectListItem{ Text = "GB", Value = "GB"},
                    new SelectListItem{ Text = "PL", Value = "PL"},
                    new SelectListItem{ Text = "SC", Value = "SC"},
                    new SelectListItem{ Text = "SK", Value = "SK"},
                }.ToList();
        }
        
        #endregion
    }
}
