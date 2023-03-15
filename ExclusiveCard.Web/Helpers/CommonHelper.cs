using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using ExclusiveCard.Enums;
using ExclusiveCard.WebAdmin.ViewModels;
using System.Drawing;
using System.Threading.Tasks;
using ExclusiveCard.Providers;
using System.Drawing.Imaging;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Reflection;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExclusiveCard.WebAdmin.Helpers
{
    public class CommonHelper
    {
        public static IConfiguration AppSettings { get; set; }
        public static int PageSize { get; set; }

        public static int ThumbnailHeight { get; set; }
        public static int ThumbnailWidth { get; set; }
        public static int MediumHeight { get; set; }
        public static int MediumWidth { get; set; }
        public static int LargeHeight { get; set; }
        public static int LargeWidth { get; set; }
        public static int FeatureHeight { get; set; }
        public static int FeatureWidth { get; set; }

        public static string ContainerName { get; set; }
        public static string ImageCategory { get; set; }
        public static string BlobConnectionString { get; set; }
        public static string StackerAPIId { get; set; }
        public static string StackerAPIKey { get; set; }

        public static string PayPalLink { get; set; }

        public const string THUMBNAIL_SUFFIX = "__1";
        public const string MEDIUM_SUFFIX = "__2";
        public const string LARGE_SUFFIX = "__3";
        public const string FEATURE_SUFFIX = "__4";

        public static void Initialize()
        {
            AppSettings = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            PageSize = Convert.ToInt32(AppSettings["PageSize"]);

            ThumbnailHeight = Convert.ToInt32(AppSettings["Images:ThumbnailHeight"]);
            ThumbnailWidth = Convert.ToInt32(AppSettings["Images:ThumbnailWidth"]);
            MediumHeight = Convert.ToInt32(AppSettings["Images:MediumHeight"]);
            MediumWidth = Convert.ToInt32(AppSettings["Images:MediumWidth"]);
            LargeHeight = Convert.ToInt32(AppSettings["Images:LargeHeight"]);
            LargeWidth = Convert.ToInt32(AppSettings["Images:LargeWidth"]);
            FeatureHeight = Convert.ToInt32(AppSettings["Images:FeatureHeight"]);
            FeatureWidth = Convert.ToInt32(AppSettings["Images:FeatureWidth"]);

            ContainerName = AppSettings["ContainerName"];
            ImageCategory = AppSettings["ImageCategory"];
            BlobConnectionString = AppSettings["BlobConnectionString"];
            StackerAPIId = AppSettings["StrackrAPI_id"];
            StackerAPIKey = AppSettings["StrackrAPI_key"];
            PayPalLink = AppSettings["PayPalLink"];
        }

        public static List<CustomCountryList> GetCountrySelectList()
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

        public static List<SelectListItem> CountryList()
        {
            List<SelectListItem> country = new List<SelectListItem>
            {
                new SelectListItem {Text = "Czech Republic", Value = "CZ"},
                new SelectListItem {Text = "United Kingdom", Value = "GB"},
                new SelectListItem {Text= "Poland", Value= "PL"},
                new SelectListItem {Text= "Seychelles", Value= "SC"},
                new SelectListItem {Text= "Slovakia", Value= "SK"}
            };

            return country;
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

        public static List<SelectListItem> GetCultureInfoCountryList()
        {
            List<SelectListItem> countryCultureInfo = new List<SelectListItem>
            {
                new SelectListItem {Text = "CZ", Value = "cs-CZ"},
                new SelectListItem {Text = "GB", Value = "en-GB"},
                new SelectListItem {Text = "PL", Value = "pl-PL"},
                new SelectListItem {Text = "SC", Value = "en-SC"},
                new SelectListItem {Text = "SK", Value = "sk-SK"}
            };

            return countryCultureInfo;
        }

        public static async Task<string> ResizeImage(string imageName, string imageCategory, 
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
                        imageWidth = ThumbnailWidth;
                        imageHeight = (image.Height * imageWidth) / image.Width;
                    }
                    else
                    {
                        imageHeight = ThumbnailHeight;
                        imageWidth = (image.Width * imageHeight) / image.Height;
                    }
                    break;

                case ImageSize.Medium:
                    suffix = MEDIUM_SUFFIX;
                    if (image.Width > image.Height)
                    {
                        imageWidth = MediumWidth;
                        imageHeight = (image.Height * imageWidth) / image.Width;
                    }
                    else
                    {
                        imageHeight = MediumHeight;
                        imageWidth = (image.Width * imageHeight) / image.Height;
                    }
                    break;

                case ImageSize.Large:
                    suffix = LARGE_SUFFIX;
                    if (image.Width > image.Height)
                    {
                        imageWidth = LargeWidth;
                        imageHeight = (image.Height * imageWidth) / image.Width;
                    }
                    else
                    {
                        imageHeight = LargeHeight;
                        imageWidth = (image.Width * imageHeight) / image.Height;
                    }
                    break;
                case ImageSize.Feature:
                    suffix = FEATURE_SUFFIX;
                    //Fixed size or replicate others?...
                    imageWidth = FeatureWidth;
                    imageHeight = FeatureHeight;
                    //if (image.Width > image.Height)
                    //{
                    //    imageWidth = FeatureWidth;
                    //    imageHeight = (image.Height * imageWidth) / image.Width;
                    //}
                    //else
                    //{
                    //    imageHeight = FeatureHeight;
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
            path = await azureStorageProvider.SaveImage(BlobConnectionString, ContainerName, ImageCategory, Path.GetFileNameWithoutExtension(imageName) + suffix + Path.GetExtension(imageName), result);
            return path;
        }

        public static bool RotateImage(Bitmap img)
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

        public static RotateFlipType GetImageRotation(Bitmap img)
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

        public static object GetPropertyValue(Object obj, String propertyName)
        {
            object fieldValue = null;
            if (obj != null && !string.IsNullOrEmpty(propertyName))
            {
                Type objType = obj.GetType();

                PropertyInfo field = objType.GetProperty(propertyName);
                if (field != null)
                {
                    fieldValue = field.GetValue(obj);
                }
            }
            return fieldValue;
        }

        public static void SetPropertyValue(Object obj, String propertyName, object value)
        {
            if (obj != null && !string.IsNullOrEmpty(propertyName))
            {
                Type objType = obj.GetType();

                PropertyInfo field = objType.GetProperty(propertyName);
                if (field != null)
                {
                    field.SetValue(obj, value);
                }
            }
        }

        public static bool MatchTableAndSchema(Type t, string tableNameWithSchema)
        {
            // Get instance of the attribute.
            TableAttribute myAttribute = (TableAttribute)Attribute.GetCustomAttribute(t, typeof(TableAttribute));

            if (myAttribute == null)
            {
                return false;
            }
            else
            {
                if (tableNameWithSchema == $"{myAttribute.Schema}.{myAttribute.Name}")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        //Convert Raw data into json array
        public static Dictionary<string, string> ParsePaypalIPN(string postedRaw)
        {
            var result = new Dictionary<string, string>();
            var keyValuePairs = postedRaw.Split('&');
            foreach (var kvp in keyValuePairs)
            {
                var keyvalue = kvp.Split('=');
                var key = keyvalue[0];
                var value = keyvalue[1];
                result.Add(key, value);
            }
            return result;
        }
    }
}
