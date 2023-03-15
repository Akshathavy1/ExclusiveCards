using System;
using System.Globalization;
using System.Threading.Tasks;
using ExclusiveCard.Providers;
using ExclusiveCard.Services.Interfaces.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

namespace ExclusiveCard.Website.Controllers
{
    public class ImageController : BaseController
    {
        private readonly IMerchantImageService _imageService;
        private readonly IOptions<TypedAppSettings> _setting;

        public ImageController(IMerchantImageService imageService,
            IOptions<TypedAppSettings> setting)
        {
            _imageService = imageService;
            _setting = setting;
        }

        public async Task<ActionResult> GetImage(string path, int id)
        {
            try
            {
                Services.Models.DTOs.MerchantImage image = new Services.Models.DTOs.MerchantImage();
                if (path.Contains(_setting.Value.ImageCategory))
                {
                    image = await _imageService.GetByIdAsync(id);
                }

                try
                {
                    if (!string.IsNullOrEmpty(Request.Headers["If-Modified-Since"]))
                    {
                        CultureInfo provider = CultureInfo.InvariantCulture;
                        var lastMod = DateTime.ParseExact(Request.Headers["If-Modified-Since"], "r", provider)
                            .ToLocalTime();
                        if (image != null)
                        {
                            if (lastMod == image.TimeStamp.AddMilliseconds(-image.TimeStamp.Millisecond))
                            {
                                Response.StatusCode = 304;
                                return Content(string.Empty);
                            }
                        }
                        else
                        {
                            //This is a temporary hack for adverts
                            Response.StatusCode = 304;
                            return Content(string.Empty);
                        }
                    }
                }
                catch (Exception ex)
                {
                    //we are going to ignore this exception
                    Logger.Warn(ex);
                }

                AzureStorageProvider azureProvider = new AzureStorageProvider();
                if (!string.IsNullOrEmpty(path))
                {
                    byte[] imgData = await azureProvider.GetImage(_setting.Value.BlobConnectionString,
                        _setting.Value.ContainerName, path, Logger);
                    if (imgData != null)
                    {
                        DateTime dtDateModified = DateTime.UtcNow;
                        if (image?.TimeStamp > DateTime.MinValue)
                        {
                            dtDateModified = image.TimeStamp;
                        }

                        string cacheControl = "max-age=" + 31536000 + ",immutable";
                        Response.Headers.Add("Cache-Control", new StringValues(cacheControl));

                        return File(imgData, "image/jpeg", dtDateModified, EntityTagHeaderValue.Any);
                    }
                    else
                    {
                        await _imageService.DeleteByMerchantImagePath(path);
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return null;
            }
        }
    }
}
