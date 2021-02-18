using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Context;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;

namespace API.Services.Impl
{
     public sealed class CategoryService : EntityServiceBase<CategoryEntity>, ICategoryService
    {
        private const string entityName = "category";

        public CategoryService(SamadDbContext dbContext, ILogger<CategoryService> logger) : base(dbContext, dbContext.Categories, logger, entityName)
        {

        }

        public bool DoesCategoryNameExist(string categoryName)
        {
            logger.LogDebug($"Checking whether a category with id {categoryName} exists already.");

            var exists = dbContext.Categories.Any(x => x.Title.ToLower() == categoryName.ToLower());

            logger.LogInformation($"A category with name {categoryName} does already exist: {exists}");

            return exists;
        }

        public async Task<List<(CategoryEntity categoryEntity, int productCount)>> GetAllAsync()
        {
            logger.LogDebug($"Attempting to retrieve all categories from the database.");

            var categories = dbContext.Categories
                .Include(x => x.Products)
                .OrderBy(x => x.Title)
                .AsEnumerable()
                .Select(x => (x, x.Products.Count))
                .ToList();

            logger.LogInformation($"Received {categories.Count} categories from the database.");

            return categories;
        }

        public async Task<List<ProductEntity>> GetAllProductyByCategoryIdAsync(Guid categoryId)
        {
            logger.LogDebug($"Attempting to retrieve all Products for the category id {categoryId}.");

            return await dbContext.Products
                .Where(x => x.CategoryId == categoryId)
                .ToListAsync();
        }

        public async Task DeleteAllAsync()
        {
            logger.LogDebug($"Attempting to remove all categories.");

            dbContext.Categories.RemoveRange(dbContext.Categories);
            await dbContext.SaveChangesAsync();

            logger.LogInformation($"Successfully removed all categories.");
        }
    }
}