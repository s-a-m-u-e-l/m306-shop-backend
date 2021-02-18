using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Entities;

namespace API.Services
{

    public interface ICategoryService : IEntityService<CategoryEntity>
    {
        /// <summary>
        /// Returns whether a category with the provided name does already exist
        /// </summary>
        /// <param name="categoryName">Category name</param>
        /// <returns>Boolean indicating whether a category with this name exists already.</returns>
        bool DoesCategoryNameExist(string categoryName);

        /// <summary>
        /// Returns a list of all categories associated with the count of products belonging to them
        /// </summary>
        /// <returns>List of all categories associated with the count of products belonging to them</returns>
        Task<List<(CategoryEntity categoryEntity, int productCount)>> GetAllAsync();

        /// <summary>
        /// Returns a list of all products which belong to a given category
        /// </summary>
        /// <param name="categoryId">The Id of the category</param>
        /// <returns>List of all Products for a given category</returns>
        Task<List<ProductEntity>> GetAllProductyByCategoryIdAsync(Guid categoryId);

        /// <summary>
        /// Removes all categories
        /// </summary>
        /// <returns></returns>
        Task DeleteAllAsync();
    }
}