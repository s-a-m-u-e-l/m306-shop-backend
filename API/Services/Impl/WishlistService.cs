using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Context;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DynamicLinq;
using Microsoft.Extensions.Logging;

namespace API.Services.Impl
{
   public class WishlistService : IWishlistService
    {
        private readonly SamadDbContext dbContext;
        private readonly ILogger logger;

        public WishlistService(SamadDbContext dbContext, ILogger<WishlistService> logger)
        {
            this.dbContext = dbContext;
            this.logger = logger;
        }

        public async Task AddAsync(WishlistEntity entity)
        {
            await dbContext.Wishlists.AddAsync(entity);

            await dbContext.SaveChangesAsync();
        }

        public async Task<bool> DoesWishlistItemExistsAsync(WishlistEntity entity)
        {
            logger.LogDebug($"Attempting to check if a wishlist already exists.");

            return await EntityFrameworkDynamicQueryableExtensions.CountAsync(dbContext.Wishlists
                       .Where(x => x.ProductId == entity.ProductId)
                       .Where(x => x.UserId == entity.UserId)) > 0;
        }

        public async Task DeleteAsync(WishlistEntity entity)
        {
            logger.LogDebug($"Attempting to remove wishlist with product id {entity.ProductId}");

            using (var transaction = await dbContext.Database.BeginTransactionAsync())
            {
                dbContext.Wishlists.Remove(entity);

                await dbContext.SaveChangesAsync();

                transaction.Commit();
            }

            logger.LogDebug($"Successfully removed wishlist with product id {entity.ProductId}");
        }

        public async Task<List<ProductEntity>> GetAllProductsAsync(Guid userId)
        {
            logger.LogDebug($"Attempting to retrieve all products of the wishlist for user with id {userId}");

            var wishlistList = await dbContext.Wishlists
                .Where(x => x.UserId == userId)
                .Include(x => x.Product)
                .ThenInclude(x => x.Image)
                .Include(x => x.Product)
                .ThenInclude(x => x.Category)
                .ToListAsync();

            var productList = new List<ProductEntity>();

            using (var transaction = await dbContext.Database.BeginTransactionAsync())
            {
                foreach (var wishlist in wishlistList)
                {
                    productList.Add(wishlist.Product);
                }

                await dbContext.SaveChangesAsync();

                transaction.Commit();
            }

            logger.LogDebug($"Successfully retrieved all products of the wishlist for user with id {userId}");
            return productList;
        }

        public async Task DeleteAll(Guid userId)
        {
            logger.LogDebug($"Attempting to remove all products of the wishlist for user with id {userId}");

            var wishlists = await dbContext.Wishlists
                .Where(x => x.UserId == userId)
                .ToListAsync();

            foreach (var wishlist in wishlists)
            {
                await DeleteAsync(wishlist);
            }

            logger.LogDebug($"Successfully removed all products of the wishlist for user with id {userId}");
        }

        public async Task DeleteByProductId(Guid productId)
        {
            logger.LogDebug($"Attempting to remove product with id {productId} from all wishlists.");

            var wishlists = await dbContext.Wishlists
                .Where(x => x.ProductId == productId)
                .ToListAsync();

            foreach (var wishlist in wishlists)
            {
                await DeleteAsync(wishlist);
            }

            logger.LogDebug($"Successfully removed product with id {productId} from all wishlists.");
        }

        public async Task DeleteWishlistsByProductsAsync(List<ProductEntity> productList)
        {
            logger.LogDebug($"Attempting to remove all wishlists with products of a given product list.");

            foreach (var product in productList)
            {
                await DeleteByProductId(product.Id);
            }

            logger.LogDebug($"Successfullyremoved all wishlists with products of a given product list.");
        }
    }
}