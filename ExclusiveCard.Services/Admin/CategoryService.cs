using System;
using System.Collections.Generic;
using System.Linq;
using ExclusiveCard.Services.Models.DTOs;
using ExclusiveCard.Managers;
using ExclusiveCard.Services.Interfaces.Admin;

namespace ExclusiveCard.Services.Admin
{
    public class CategoryService : ICategoryService
    {
        #region Private Members

        private readonly ICategoryManager _manager;

        #endregion

        #region Contructor

        public CategoryService(ICategoryManager manager)
        {
            _manager = manager;
        }

        #endregion

        #region Write

        public Category Update(Category category)
        {
            return _manager.Update(category);
        }

        public CategoryFeatureDetail AddCategoryFeatureDetail(CategoryFeatureDetail feature)
        {
            return _manager.AddCategoryFeatureDetail(feature);
        }

        public CategoryFeatureDetail UpdateCategoryFeatureDetail(CategoryFeatureDetail feature)
        {
            return _manager.UpdateCategoryFeatureDetail(feature);
        }

        #endregion

        #region Reads

        public CategoryFeatureDetail GetFeatureDetail(int categoryId, string countryCode)
        {
            return _manager.GetFeatureDetail(categoryId, countryCode);
        }

        public Category GetById(int id)
        {
            return _manager.GetById(id);
        }

        //Get category by name
        public Category Get(string name)
        {
            return _manager.Get(name);
        }

        //Get all categories
        public List<Category> GetAll()
        {
            return _manager.GetAll();
        }

        public List<TreeItem<Category>> GetTree()
        {
            var data = _manager.GetAll();
            
            return GenerateTree(data, x => x.Id, x => x.ParentId).ToList();
        }

        #endregion

        #region Private Methods

        private IEnumerable<TreeItem<T>> GenerateTree<T, K>(
            IEnumerable<T> collection,
            Func<T, K> idSelector,
            Func<T, K> parentIdSelector,
            K rootId = default(K))
        {
            var enumerable = collection.ToList();
            foreach (var c in enumerable.Where(c => parentIdSelector(c).Equals(rootId)))
            {
                yield return new TreeItem<T>
                {
                    Item = c,
                    Children = GenerateTree(enumerable, idSelector, parentIdSelector, idSelector(c))
                };
            }
        }

        #endregion
    }
}
