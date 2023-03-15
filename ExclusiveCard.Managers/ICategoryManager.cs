using System.Collections.Generic;
using System.Threading.Tasks;
using dto = ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Managers
{
    public interface ICategoryManager
    {
        dto.Category Update(dto.Category category);
        dto.CategoryFeatureDetail AddCategoryFeatureDetail(dto.CategoryFeatureDetail feature);
        dto.CategoryFeatureDetail UpdateCategoryFeatureDetail(dto.CategoryFeatureDetail feature);

        dto.CategoryFeatureDetail GetFeatureDetail(int categoryId, string countryCode);
        dto.Category GetById(int id);
        dto.Category Get(string str);
        dto.Category GetByUrlSlug(string urlSlug);
        List<dto.Category> GetAll();
        Task<List<dto.Category>> GetAllParents();
        List<dto.Category> GetByParentId(int parentId);
    }
}