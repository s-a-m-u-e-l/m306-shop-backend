using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Entities;

namespace API.Services
{
   public interface IWishlistService
    {
        
        /// <summary>
        /// Adds a new Wishlist item to the database
        /// </summary>
        /// <param name="entity">Entity of a wishlist</param>
        /// <returns></returns>
        Task AddAsync(WishlistEntity entity);

        /// <summary>
        /// Checks wheather an item already exists in the database or not
        /// </summary>
        /// <param name="entity">Entity of a wishlist</param>
        /// <returns>Boolean, indicating if the item already exists</returns>
        Task<bool> DoesWishlistItemExistsAsync(WishlistEntity entity);
        
        /// <summary>
        /// Removes a given entity from the database
        /// </summary>
        /// <param name="entity">Entity of wishlist</param>
        /// <returns></returns>
        Task DeleteAsync(WishlistEntity entity);

        /// <summary>
        /// Returns all Products in from the wishlist of a given user
        /// </summary>
        /// <param name="userId">The id of the given user</param>
        /// <returns>List of products, which are on the wishlist</returns>
        Task<List<ProductEntity>> GetAllProductsAsync(Guid userId);

        /// <summary>
        /// Removes all wishlists for a given user
        /// </summary>
        /// <param name="userId">Id of the user</param>
        /// <returns></returns>
        Task DeleteAll(Guid userId);

        /// <summary>
        /// Removes the wishlist of a given product Id 
        /// </summary>
        /// <param name="productId">The id of the Product</param>
        /// <returns></returns>
        Task DeleteByProductId(Guid productId);
        
        /// <summary>
        /// Removes all wishlists which contain a product from the given list
        /// </summary>
        /// <param name="productList">The product list which will be removed aftwerwards</param>
        /// <returns></returns>
        Task DeleteWishlistsByProductsAsync(List<ProductEntity> productList);
    } 
}