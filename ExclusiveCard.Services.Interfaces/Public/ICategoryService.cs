using System.Collections.Generic;
using System.Threading.Tasks;
using ExclusiveCard.Services.Models.DTOs;

namespace ExclusiveCard.Services.Interfaces.Public
{
    public interface ICategoryService
    {
        #region Reads  

        Category Get(string categoryName);

        Category GetByUrlSlug(string urlSlug);

        List<Category> GetAll();

        Task<List<Category>> GetAllparentcategories();

        List<Category> GetByParentId(int parentId);

        #endregion
    }
}