using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExclusiveCard.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using NLog;
using dto = ExclusiveCard.Services.Models.DTOs;
using data = ExclusiveCard.Data.Models;

namespace ExclusiveCard.Managers
{
    public class CategoryManager : ICategoryManager
    {
        #region Private Members and Constructor

        private readonly IRepository<data.Category> _repository;
        private readonly IRepository<data.CategoryFeatureDetail> _featureRepository;
        private readonly ILogger _logger;

        public CategoryManager(IRepository<data.Category> repository,
            IRepository<data.CategoryFeatureDetail> featureRepository)
        {
            _repository = repository;
            _featureRepository = featureRepository;
            _logger = LogManager.GetCurrentClassLogger();
        }

        #endregion

        public dto.Category Update(dto.Category category)
        {
            try
            {
                var req = MapToData(category);
                _repository.Update(req);
                category = MapToDto(req);
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return null;
            }

            return category;
        }

        public dto.CategoryFeatureDetail AddCategoryFeatureDetail(dto.CategoryFeatureDetail feature) 
        {
            try
            {
                var req = MapToReq(feature);
                _featureRepository.Create(req);
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return null;
            }

            return feature;
        }

        public dto.CategoryFeatureDetail UpdateCategoryFeatureDetail(dto.CategoryFeatureDetail feature)
        {
            try
            {
                var req = MapToReq(feature);
                _featureRepository.Update(req);
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return null;
            }

            return feature;
        }

        public dto.CategoryFeatureDetail GetFeatureDetail(int categoryId, string countryCode)
        {
            return MapToDto(_featureRepository
                .FilterNoTrackAsync(x => x.CategoryId == categoryId && x.CountryCode == countryCode).Result.FirstOrDefault());
        }

        public dto.Category GetById(int id)
        {
            return MapToDto(_repository
                .IncludeAndThenInclude(x => x.IsActive && x.Id == id, x => x.Include(y => y.CategoryFeatureDetails))
                .FirstOrDefault());
        }

        public dto.Category Get(string str)
        {
            return MapToDto(_repository
                .FilterNoTrackAsync(x => x.IsActive && x.Name == str).Result
                .FirstOrDefault());
        }

        public dto.Category GetByUrlSlug(string urlSlug)
        {
            return MapToDto(_repository
                .FilterNoTrackAsync(x => x.IsActive && x.UrlSlug == urlSlug).Result
                .FirstOrDefault());
        }

        public List<dto.Category> GetAll()
        {
            return MapToDtos(_repository
                .IncludeAndThenInclude(x => x.IsActive, x => x.Include(y => y.CategoryFeatureDetails))
                .OrderBy(x => x.DisplayOrder).ToList());
        }

        public async Task<List<dto.Category>> GetAllParents()
        { 
            var data = await _repository.FilterNoTrackAsync(x => x.IsActive && x.ParentId == 0);
            return MapToDtos(data.OrderBy(x => x.DisplayOrder).ToList());
        }

        public List<dto.Category> GetByParentId(int parentId)
        {
            return MapToDtos(_repository.FilterNoTrackAsync(x => x.IsActive && x.ParentId == parentId).Result.OrderBy(x => x.DisplayOrder).ToList());
        }

        #region Private Methods

        private data.Category MapToData(dto.Category category)
        {
            if (category == null)
                return null;

            var cat = new data.Category
            {
                Id = category.Id,
                Name = category.Name,
                Code = category.Code,
                ParentId = category.ParentId,
                DisplayOrder = category.DisplayOrder,
                IsActive = category.IsActive,
                UrlSlug = category.UrlSlug
            };

            if (category.CategoryFeatureDetails?.Count > 0)
            {
                cat.CategoryFeatureDetails = new List<data.CategoryFeatureDetail>();
                foreach (var item in category.CategoryFeatureDetails)
                {
                    cat.CategoryFeatureDetails.Add(
                        new data.CategoryFeatureDetail
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

        private dto.Category MapToDto(data.Category category)
        {
            if (category == null)
                return null;

            var cat = new dto.Category
            {
                Id = category.Id,
                Name = category.Name,
                Code = category.Code,
                ParentId = category.ParentId,
                DisplayOrder = category.DisplayOrder,
                IsActive = category.IsActive,
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

        private List<dto.Category> MapToDtos(List<data.Category> data)
        {
            if (data == null || data.Count == 0)
                return null;

            List<dto.Category> dtos = new List<dto.Category>();

            dtos.AddRange(data.Select(MapToDto));

            return dtos;
        }

        private data.CategoryFeatureDetail MapToReq(dto.CategoryFeatureDetail data)
        {
            if (data == null)
                return null;
            return new data.CategoryFeatureDetail
            {
                CategoryId = data.CategoryId,
                CountryCode = data.CountryCode,
                FeatureMerchantId = data.FeatureMerchantId,
                SelectedImage = data.SelectedImage,
                UnselectedImage = data.UnselectedImage
            };
        }

        private dto.CategoryFeatureDetail MapToDto(data.CategoryFeatureDetail data)
        {
            if (data == null)
                return null;
            return new dto.CategoryFeatureDetail
            {
                CategoryId = data.CategoryId,
                CountryCode = data.CountryCode,
                FeatureMerchantId = data.FeatureMerchantId,
                SelectedImage = data.SelectedImage,
                UnselectedImage = data.UnselectedImage
            };
        }

        #endregion
    }
}
