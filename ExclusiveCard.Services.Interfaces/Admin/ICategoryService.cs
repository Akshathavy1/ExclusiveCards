using System.Collections.Generic;
using ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Services.Interfaces.Admin
{
    public interface ICategoryService
    {
        #region Write

        Category Update(Category category);
        CategoryFeatureDetail AddCategoryFeatureDetail(CategoryFeatureDetail feature);
        CategoryFeatureDetail UpdateCategoryFeatureDetail(CategoryFeatureDetail feature);

        #endregion

        #region Reads  

        CategoryFeatureDetail GetFeatureDetail(int categoryId, string countryCode);
        Category GetById(int id);
        Category Get(string name);
        List<Category> GetAll();
        List<TreeItem<Category>> GetTree();

        #endregion

    }
}
