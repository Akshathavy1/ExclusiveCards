using ExclusiveCard.Services.Interfaces.Public;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExclusiveCard.Managers;

namespace ExclusiveCard.Services.Public
{
    public class CategoryService : ICategoryService
    {
        #region Private Members
        
        private readonly ICategoryManager _categoryManager;

        #endregion

        #region Contructor

        public CategoryService(ICategoryManager categoryManager)
        {
            _categoryManager = categoryManager;
        }

        #endregion

        #region Reads

        //Get category by category name
        public Models.DTOs.Category Get(string categoryName)
        {

            return _categoryManager.Get(categoryName);
        }

        //Get category by urlSlug
        public Models.DTOs.Category GetByUrlSlug(string urlSlug)
        {
            return _categoryManager.GetByUrlSlug(urlSlug);
        }

        //Get all categories
        public List<Models.DTOs.Category> GetAll()
        {
            return _categoryManager.GetAll();
        }


        public async Task<List<Models.DTOs.Category>> GetAllparentcategories()
        {
            return await _categoryManager.GetAllParents();
        }

        //Get all categories by parentId
        public List<Models.DTOs.Category> GetByParentId(int parentId)
        {
            return _categoryManager.GetByParentId(parentId);
        }

        #endregion

        private List<Models.DTOs.Category> MapToList(List<Data.Models.Category> data)
        {
            if (data == null || data.Count == 0)
                return null;

            List<Models.DTOs.Category> models = new List<Models.DTOs.Category>();

            models.AddRange(data.Select(MapToDto));

            return models;
        }

        private Models.DTOs.Category MapToDto(Data.Models.Category data)
        {
            if (data == null)
                return null;
            var model = new Models.DTOs.Category
            {
                Id = data.Id,
                Name = data.Name,
                Code = data.Code,
                ParentId = data.ParentId,
                IsActive = data.IsActive,
                DisplayOrder = data.DisplayOrder,
                UrlSlug = data.UrlSlug
            };

            if (data.CategoryFeatureDetails?.Count > 0)
            {
                model.CategoryFeatureDetails = new List<Models.DTOs.CategoryFeatureDetail>();
                foreach (var item in data.CategoryFeatureDetails)
                {
                    var feature = new Models.DTOs.CategoryFeatureDetail
                    {
                        CategoryId = item.CategoryId,
                        CountryCode = item.CountryCode,
                        FeatureMerchantId = item.FeatureMerchantId,
                        SelectedImage = item.SelectedImage,
                        UnselectedImage = item.UnselectedImage
                    };

                    if (item.Merchant != null)
                    {
                        feature.Merchant = new Models.DTOs.Merchant
                        {
                            Id = item.Merchant.Id,
                            Name = item.Merchant.Name,
                            ContactDetailsId = item.Merchant.ContactDetailsId,
                            ContactName = item.Merchant.ContactName,
                            ShortDescription = item.Merchant.ShortDescription,
                            LongDescription = item.Merchant.LongDescription,
                            Terms = item.Merchant.Terms,
                            WebsiteUrl = item.Merchant.WebsiteUrl,
                            IsDeleted = item.Merchant.IsDeleted,
                            FeatureImageOfferText = item.Merchant.FeatureImageOfferText,
                            BrandColour = item.Merchant.BrandColour
                        };
                    }

                    model.CategoryFeatureDetails.Add(feature);
                }
            }

            return model;
        }
    }
}
