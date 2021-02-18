using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Helpers;
using Data.Context;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace API.Services.Impl
{
    public sealed class ProductService : IProductService
    {
        private readonly SamadDbContext dbContext;
        private readonly ILogger logger;
        private readonly IImageService imageService;
        private readonly IWishlistService wishlistService;

        public ProductService(SamadDbContext dbContext, ILogger<ProductService> logger,
            IImageService imageService, IWishlistService wishlistService)
        {
            this.dbContext = dbContext;
            this.logger = logger;
            this.imageService = imageService;
            this.wishlistService = wishlistService;
        }

        public async Task AddAsync(ProductEntity entity)
        {
            logger.LogDebug($"Attempting to add new product");

            using (var transaction = await dbContext.Database.BeginTransactionAsync())
            {
                // Persist ProductEntity first
                dbContext.Products.Add(entity);
                await dbContext.SaveChangesAsync();

                transaction.Commit();
            }
        }

        public async Task DeleteAsync(ProductEntity entity)
        {
            await DeleteAsync(entity.Id);
        }

        public async Task DeleteAsync(Guid entityId)
        {
            logger.LogDebug($"Attempting to remove product with id {entityId}");

            using (var transaction = await dbContext.Database.BeginTransactionAsync())
            {
                dbContext.Products.Remove(await dbContext.Products.FindAsync(entityId));

                await dbContext.SaveChangesAsync();

                transaction.Commit();
            }

            logger.LogInformation($"Sucessfully removed product with id {entityId}");
        }

        public async Task<PagingInformation<ProductEntity>> GetPagedAsync(PagingSortingParams pagingSortingOptions,
            FilterParams filterParams, Guid? filterCategoryId = null)
        {
            logger.LogDebug(
                $"Attempting to retrieve products from database: Page: {pagingSortingOptions.Page}, items per page: {pagingSortingOptions.ItemsPerPage}, sort by: {pagingSortingOptions.SortBy}, sort direction: {pagingSortingOptions.SortDirection}.");

            var productsQuery = dbContext.Products
                .Include(x => x.Image) as IQueryable<ProductEntity>;

            if (filterCategoryId != null)
            {
                productsQuery = productsQuery.Where(x => x.CategoryId == filterCategoryId);
            }

            productsQuery = productsQuery.ProductsAdvancedFiltered(filterParams);

            var totalProducts = await productsQuery.CountAsync();

            var products = await productsQuery.PagedAndSorted(pagingSortingOptions)
                .ToListAsync();

            var totalPages = (int)Math.Ceiling(totalProducts / (double)pagingSortingOptions.ItemsPerPage);

            logger.LogInformation($"Retrieved {products.Count} products from the database.");

            return new PagingInformation<ProductEntity>
            {
                PageCount = totalPages,
                CurrentPage = pagingSortingOptions.Page,
                ItemsPerPage = pagingSortingOptions.ItemsPerPage,
                TotalItems = totalProducts,
                Items = products
            };
        }

        public async Task<ProductEntity> GetByIdAsync(Guid entityId)
        {
            logger.LogDebug($"Attempting to retrieve the product with the id {entityId}");

            var product = await dbContext.Products
                .Where(x => x.Id == entityId)
                .Include(x => x.Image)
                .Include(x => x.User)
                .Include(x => x.Category)
                .FirstOrDefaultAsync();

            return product;
        }

        public async Task UpdateAsync(ProductEntity entity)
        {
            logger.LogDebug($"Attempting to update product with id {entity.Id}");

            using (var transaction = await dbContext.Database.BeginTransactionAsync())
            {
                // Update product entity itself

                dbContext.Products.Update(entity);
                await dbContext.SaveChangesAsync();

                transaction.Commit();
            }
        }

        public async Task<List<ProductEntity>> GetAllAsync()
        {
            logger.LogDebug($"Attempting to retrieve all available products from the database.");

            var products = await dbContext.Products
                .Include(x => x.Image)
                .Include(x => x.User)
                .Include(x => x.Category)
                .ToListAsync();

            logger.LogInformation($"Retrieved {products.Count} products from the database.");

            return products;
        }

        public async Task DeleteAllAsync(List<ProductEntity> productList)
        {
            logger.LogDebug($"Attempting to remove {productList.Count} products by category");

            using (var transaction = await dbContext.Database.BeginTransactionAsync())
            {
                dbContext.Products.RemoveRange(productList);
                await dbContext.SaveChangesAsync();
                transaction.Commit();
            }

            logger.LogInformation(
                $"Successfully removed {productList.Count} products by category.");
        }

        public async Task DeleteAllAsync()
        {
            logger.LogDebug($"Attempting to remove all products.");

            using (var transaction = await dbContext.Database.BeginTransactionAsync())
            {
                dbContext.Products.RemoveRange(dbContext.Products);
                await dbContext.SaveChangesAsync();
                transaction.Commit();
            }

            logger.LogInformation($"Successfully removed all products");
        }

        public async Task<bool> DoesProductExistByIdAsync(Guid productId)
        {
            logger.LogDebug($"Attempting to check if a product with id {productId} exists or not.");

            return await dbContext.Products
                       .Where(x => x.Id == productId)
                       .CountAsync() > 0;
        }
    }
}