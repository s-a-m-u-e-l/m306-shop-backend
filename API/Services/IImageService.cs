using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Entities;

namespace API.Services
{
    public interface IImageService
    {
        /// <summary>
        /// Adds a new image to the database
        /// </summary>
        /// <param name="entity">Instance of ImageEntity</param>
        /// <returns></returns>
        Task AddAsync(ImageEntity entity);

        /// <summary>
        /// Get the image for a given id
        /// </summary>
        /// <param name="imageId">The image id</param>
        /// <returns></returns>
        Task<ImageEntity> GetByIdAsync(Guid imageId);

        /// <summary>
        /// Deletes the image with the corresponding ID
        /// </summary>
        /// <param name="imageId">The id of the image</param>
        /// <returns></returns>
        Task DeleteAsync(Guid imageId);

        /// <summary>
        /// Deletes all images
        /// </summary>
        /// <returns></returns>
        Task DeleteAllAsync();

        /// <summary>
        /// Removes all images by a given list of products
        /// </summary>
        /// <param name="productList">The list of products which will be removed</param>
        /// <returns></returns>
        Task DeleteImagesByCategoryAsync(List<ProductEntity> productList);
    }
}