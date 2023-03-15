using ExclusiveCard.Enums;
using ExclusiveCard.Providers;
using ExclusiveCard.Services.Interfaces.Admin;
using ExclusiveCard.WebAdmin.ViewModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DTOs = ExclusiveCard.Services.Models.DTOs;
using services = ExclusiveCard.WebAdmin.App.ServiceHelper;

namespace ExclusiveCard.WebAdmin.Helpers
{
    public class MerchantHelper : CommonHelper
    {
        //TODO::These two methods will not be used anymore as these were used to migrate images from Stackr
        //TODO:: Need to check with Ian and delete these
        public static async Task<List<MerchantImageViewModel>> MapToBlobStorage(IMerchantImageService merchantImageService,
            IAzureStorageProvider azureStorageProvider, int id, Bitmap bitmapImage, string imageName, List<MerchantImageViewModel> sessionMerchantImage)
        {
            List<MerchantImageViewModel> merchantImages = new List<MerchantImageViewModel>();
            string imagePath = string.Empty;
            if (BlobConnectionString != null)
            {
                if (id == 0)
                {
                    List<string> imagePaths = new List<string>();
                    //Add Case
                    RotateImage(bitmapImage);
                    foreach (string imageSize in Enum.GetNames(typeof(ImageSize)))
                    {
                        imagePath = await ResizeImage(imageName, ImageCategory, bitmapImage,
                            (ImageSize)Enum.Parse(typeof(ImageSize), imageSize), azureStorageProvider);

                        imagePaths.Add(imagePath);
                    }
                    foreach (string imageString in imagePaths)
                    {
                        if (imageString.Contains(MEDIUM_SUFFIX))
                        {
                            MerchantImageViewModel merchantImageView = new MerchantImageViewModel
                            {
                                DisplayOrder = (short)sessionMerchantImage.Count(),
                                ImagePath = imageString,
                                ImagePaths = imagePaths,
                                ImageType = (int) ImageType.Logo
                            };
                            sessionMerchantImage.Add(merchantImageView);
                        }
                    }
                    merchantImages = sessionMerchantImage;
                }
                else
                {
                    //Edit Case
                    int displayOrder = merchantImageService.Get(id).DisplayOrder + 1;
                    RotateImage(bitmapImage);
                    foreach (string imageSize in Enum.GetNames(typeof(ImageSize)))
                    {
                        imagePath = await ResizeImage(imageName, ImageCategory, bitmapImage,
                            (ImageSize)Enum.Parse(typeof(ImageSize), imageSize), azureStorageProvider);

                        if (imagePath != null)
                        {
                            DTOs.MerchantImage response = new DTOs.MerchantImage();
                            DTOs.MerchantImage merchantImage = new DTOs.MerchantImage
                            {
                                MerchantId = id,
                                ImagePath = imagePath,
                                DisplayOrder = (short)(displayOrder),
                                ImageType = (int) ImageType.Logo
                            };
                            await merchantImageService.Add(merchantImage);
                        }
                    }
                    List<DTOs.MerchantImage> responseMerchantImages = await merchantImageService.GetAll(id, MEDIUM_SUFFIX, 0);
                    merchantImages.AddRange(responseMerchantImages.Select(merchantImage => new MerchantImageViewModel
                    {
                        Id = merchantImage.Id,
                        MerchantId = merchantImage.MerchantId,
                        ImagePath = merchantImage.ImagePath,
                        DisplayOrder = merchantImage.DisplayOrder,
                        ImageType = merchantImage.ImageType
                    }));
                }
            }
            return merchantImages;
        }

        //Method to import Merchant Images from StrackerAPI
        public static async Task<bool> GetJsonAndMapToImportMerchantImage(IMerchantService merchantService, IMerchantImageService merchantImageService,
           IAffiliateMappingRuleService affiliateMappingRuleService, IAffiliateMappingService affiliateMappingService, IAzureStorageProvider azureStorageProvider)
        {
            bool results = false;
            try
            {
                JArray errorMerchantJsonFile = new JArray();
                string path;
                DTOs.AffiliateMappingRule affiliateMappingRule = await affiliateMappingRuleService.GetByDesc("AWIN Merchants");
                if (affiliateMappingRule != null)
                {
                    string url = $"https://api.strackr.com/v3/advertisers?api_id={StackerAPIId}&api_key={StackerAPIKey}";
                    var response = "";
                    JObject json = new JObject();

                    //Download Json File and Create Json File
                    using (WebClient wc = new WebClient())
                    {
                        wc.Encoding = System.Text.Encoding.UTF8;
                        byte[] b = wc.DownloadData(url);
                        MemoryStream output = new MemoryStream();
                        using (GZipStream g = new GZipStream(new MemoryStream(b), CompressionMode.Decompress))
                        {
                            g.CopyTo(output);
                        }
                        response = System.Text.Encoding.UTF8.GetString(output.ToArray());
                        json = JObject.Parse(response);

                        path = Path.Combine(
                             Directory.GetCurrentDirectory(), "JsonFile/");
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        if (string.IsNullOrEmpty(response) || Convert.ToInt32(json["results_count"]) == 0)
                        {
                            throw new Exception("Empty json result.");
                        }
                        else if (File.Exists(path + "Merchant" + string.Format("{0:ddMMyyyy}", DateTime.UtcNow) + ".json"))
                        {
                            //For testing jsonFile download with current Date (not in requirement)
                            results = false;
                        }
                        else
                        {
                            File.WriteAllText(path + "Merchant" + string.Format("{0:ddMMyyyy}", DateTime.UtcNow) + ".json", json.ToString());
                        }
                    }

                    //Mapping Json File to MerchantImages
                    int recordCount = Convert.ToInt32(json["results_count"]);
                    List<DTOs.SocialMediaCompany> socialMediaCompanies = await App.ServiceHelper.Instance.SocialMediaCompanyService.GetAll();
                    //foreach (JObject result in json["results"])
                    await Task.WhenAll(json["results"].Select(async result =>
                     {

                         try
                         {
                             string merchantName = result["name"].ToString();
                             string imagePath = result["logo"].ToString();
                             if (!string.IsNullOrEmpty(merchantName))
                             {
                                 DTOs.Merchant responseMerchant = merchantService.GetByName(merchantName);
                                 if (responseMerchant == null)
                                 {
                                     DTOs.Merchant request = new DTOs.Merchant
                                     {
                                         Name = merchantName,
                                         MerchantSocialMediaLinks = new List<DTOs.MerchantSocialMediaLink>()
                                     };
                                     foreach (DTOs.SocialMediaCompany socialMediaCompany in socialMediaCompanies)
                                     {
                                         DTOs.MerchantSocialMediaLink merchantSocialMediaLink = new DTOs.MerchantSocialMediaLink
                                         {
                                             SocialMediaCompanyId = socialMediaCompany.Id
                                         };
                                         request.MerchantSocialMediaLinks.Add(merchantSocialMediaLink);
                                     }
                                     responseMerchant = await merchantService.Add(request);
                                 }
                                 else
                                 {
                                     if (responseMerchant.MerchantSocialMediaLinks.Count == 0)
                                     {
                                         List<DTOs.MerchantSocialMediaLink> merchantSocialMediaLinks = new List<DTOs.MerchantSocialMediaLink>();
                                         foreach (DTOs.SocialMediaCompany socialMediaCompany in socialMediaCompanies)
                                         {
                                             DTOs.MerchantSocialMediaLink merchantSocialMediaLink = new DTOs.MerchantSocialMediaLink
                                             {
                                                 MerchantId = responseMerchant.Id,
                                                 SocialMediaCompanyId = socialMediaCompany.Id
                                             };
                                             merchantSocialMediaLinks.Add(merchantSocialMediaLink);
                                         }
                                         await App.ServiceHelper.Instance.MerchantSocialMediaLinkService.AddListAsync(merchantSocialMediaLinks);
                                     }
                                 }
                                 DTOs.AffiliateMapping affiliateMapping = await affiliateMappingService.GetByAffiliateValue(affiliateMappingRule.Id, merchantName);
                                 if (affiliateMapping == null)
                                 {
                                     DTOs.AffiliateMapping affiliateMappingRequest = new DTOs.AffiliateMapping
                                     {
                                         AffiliateMappingRuleId = affiliateMappingRule.Id,
                                         AffilateValue = merchantName,
                                         ExclusiveValue = responseMerchant.Id.ToString()
                                     };
                                     await affiliateMappingService.Add(affiliateMappingRequest);
                                 }
                                 if (!string.IsNullOrEmpty(imagePath))
                                 {
                                     if (responseMerchant != null)
                                     {
                                         Bitmap image;
                                         string fileName;
                                         using (WebClient wcImg = new WebClient())
                                         {
                                             Stream stream = wcImg.OpenRead(imagePath);
                                             image = new Bitmap(stream);
                                             fileName = Guid.NewGuid().ToString("N").ToUpper();
                                         }
                                         if (!string.IsNullOrEmpty(image?.ToString()))
                                         {
                                             //MapToBlobStorage function Edit case process to save 3 images in Blob Storage and DB
                                             List<MerchantImageViewModel> sessionImages = new List<MerchantImageViewModel>();
                                             await MapToBlobStorage(merchantImageService, azureStorageProvider, responseMerchant.Id, image, fileName, sessionImages);
                                         }
                                     }
                                 }
                             }
                         }
                         catch (Exception ex)
                         {
                             services.Instance.Logger.Error(ex);
                             JObject res = new JObject();
                             res.Add(result);
                             res.Add("errorMessage", ex.Message + ex.StackTrace);
                             errorMerchantJsonFile.Add(res.ToString());
                         }
                     }));
                    if (errorMerchantJsonFile?.Count > 0)
                    {
                        File.WriteAllText(path + "ErrorMerchantImportFiles" + string.Format("{0:ddMMyyyyHHmmss}", DateTime.UtcNow) + ".json", errorMerchantJsonFile.ToString());
                        throw new Exception("Error while importing files");
                    }
                    results = true;
                }
                else
                {
                    throw new Exception("Awin Merchants can't found in AffiliateMappingRule Table");
                }
            }
            catch (Exception ex)
            {
                services.Instance.Logger.Error(ex);
            }

            return results;
        }
    }
}
