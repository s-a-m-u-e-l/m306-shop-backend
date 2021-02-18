using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Helpers;
using API.Services.Impl;
using Data.Entities;

namespace API.Services
{
   public interface IProductService : IEntityService<ProductEntity>
    {
        /// <summary>
        /// Adds a new product to the database
        /// </summary>
        /// <param name="entity">Instance of <see cref="ProductEntity"/> representing the basic product data</param>
        /// <returns></returns>
        Task AddAsync(ProductEntity entity);

        /*/// <summary>
        /// Returns a list of all available products consolidated into an instance of <see cref="ConsolidatedProductEntity"/>
        /// </summary>
        /// <param name="categoryId">Option categoryId. If supplied, only products belonging to this category will be returned</param>
        /// <returns>List of all available products consolidated into an instance of <see cref="ConsolidatedProductEntity"/></returns>
        Task<List<ConsolidatedProductEntity>> GetAllConsolidatedAsync(Guid? categoryId);

        /// <summary>
        /// Returns a paged list of all available products consolidated into an instance of <see cref="ConsolidatedProductEntity"/> while respecting the provided options
        /// </summary>
        /// <param name="options">Instance of <see cref="PagingSortingParams"/></param>
        /// <param name="categoryId">Option categoryId. If supplied, only products belonging to this category will be returned</param>
        /// <returns>Paged list of available products consolidated into an instance of <see cref="ConsolidatedProductEntity"/></returns>
        Task<PagingInformation<ConsolidatedProductEntity>> GetConsolidatedPagedAsync(PagingSortingParams options, Guid? categoryId);

        /// <summary>
        /// Returns the product with the specified id consolidated into an instance of <see cref="ConsolidatedProductEntity"/>
        /// </summary>
        /// <param name="id">Product id</param>
        /// <returns>Product with the specified id consolidated into an instance of <see cref="ConsolidatedProductEntity"/></returns>
        Task<ConsolidatedProductEntity> GetConsolidatedByIdAsync(Guid id);*/

        /// <summary>
        /// Returns the product with the specified id/>
        /// </summary>
        /// <param name="id">Product id</param>
        /// <returns>Product with the specified id/></returns>
        Task<ProductEntity> GetByIdAsync(Guid id);

        /// <summary>
        /// Returns a list of all available products/>/>
        /// </summary>
        /// <returns>List of all available products/></returns>
        Task<List<ProductEntity>> GetAllAsync();

        /// <summary>
        /// Returns a paged list of all available products/>
        /// </summary>
        /// <param name="pagingSortingOptions">Instance of <see cref="PagingSortingParams"/> to handle paging and sorting</param>
        /// <param name="filterOptions">Instance of <see cref="FilterParams"/> to handle filtering</param>
        /// <param name="filterCategoryId">Additional, optional category id. If specified, all products will be pre-filered by this category</param>
        /// <returns>Paged list of available products/></returns>
        Task<PagingInformation<ProductEntity>> GetPagedAsync(PagingSortingParams pagingSortingOptions, FilterParams filterOptions, Guid? filterCategoryId = null);

        /// <summary>
        /// Removes the all specified products, including their prices and translations
        /// </summary>
        /// <param name="productList">List of products to be removed</param>
        /// <returns></returns>
        Task DeleteAllAsync(List<ProductEntity> productList);

        /// <summary>
        /// Removes all products including their prices and translations
        /// </summary>
        /// <returns></returns>
        Task DeleteAllAsync();

        /// <summary>
        /// Returns a boolean indicating if a given product id exists in the database
        /// </summary>
        /// <param name="productId">Id of Product</param>
        /// <returns>True, if the given id exists. False otherwise</returns>
        Task<bool> DoesProductExistByIdAsync(Guid productId);
    }
}