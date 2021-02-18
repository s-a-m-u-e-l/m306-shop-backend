using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Context;
using Data.Entities;
using Microsoft.EntityFrameworkCore.DynamicLinq;
using Microsoft.Extensions.Logging;

namespace API.Services.Impl
{
  public sealed class ImageService : IImageService
    {
        private readonly SamadDbContext dbContext;

        private readonly ILogger logger;

        /// <summary>
        /// Implementation of <see cref="IImageService"/>
        /// </summary>
        /// <param name="dbContext">Database Context</param>
        public ImageService(SamadDbContext dbContext, ILogger<ImageService> logger)
        {
            this.dbContext = dbContext;
            this.logger = logger;
        }

        public async Task AddAsync(ImageEntity entity)
        {
            logger.LogDebug($"Attempting to Add a new image with id {entity.Id}.");

            using (var transaction = await dbContext.Database.BeginTransactionAsync())
            {
                await dbContext.Images.AddAsync(entity);
                await dbContext.SaveChangesAsync();
                transaction.Commit();
            }

            logger.LogInformation($"Sucessfully added Image with id {entity.Id}");
        }

        public async Task<ImageEntity> GetByIdAsync(Guid imageId)
        {
            logger.LogDebug($"Attempting to get an image with id {imageId}.");

            return await EntityFrameworkDynamicQueryableExtensions.FirstOrDefaultAsync(dbContext.Images
                .Where(x => x.Id.Equals(imageId)));
        }

        public async Task DeleteAsync(Guid imageId)
        {
            logger.LogDebug($"Attempting to remove Image with id with id {imageId}");

            using (var transaction = await dbContext.Database.BeginTransactionAsync())
            {
                dbContext.Images.Remove(await dbContext.Images.FindAsync(imageId));

                await dbContext.SaveChangesAsync();

                transaction.Commit();
            }

            logger.LogInformation($"Sucessfully removed Image with id {imageId}");
        }

        public async Task DeleteAllAsync()
        {
            logger.LogDebug($"Attempting to remove all images.");

            dbContext.Images.RemoveRange(dbContext.Images);

            await dbContext.SaveChangesAsync();

            logger.LogInformation($"Successfully removed all images.");
        }

        public async Task DeleteImagesByCategoryAsync(List<ProductEntity> productList)
        {
            logger.LogDebug($"Attempting to remove all images for a given product list");

            foreach (var product in productList)
            {
                await DeleteAsync(product.ImageId);
            }
        }
    }
}