using System;
using System.Linq;
using System.Threading.Tasks;
using API.Filters;
using API.Helpers;
using API.Services;
using Core.Models;
using Core.Models.Requests;
using Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    
    [Route("api/v1/categories")]
    [Authorize]
    [ApiV1ExceptionFilter]
    public sealed class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly ILogger _logger;
        private readonly IWishlistService _wishlistService;
        private readonly IImageService _imageService;

        public CategoryController(ICategoryService categoryService, IProductService productService, ILogger<CategoryController> logger, IWishlistService wishlistService, IImageService imageService)
        {
            _categoryService = categoryService;
            _productService = productService;
            _logger = logger;
            _wishlistService = wishlistService;
            _imageService = imageService;
        }

        /// <summary>
        /// Returns a list of all available categories including the amount of products associated with each category
        /// </summary>
        /// <response code="200">Categories received successfully</response>
        /// <response code="500">An internal error occurred</response>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogDebug($"Attempting to get all categories.");

            var categories = await _categoryService.GetAllAsync();

            _logger.LogInformation($"Got {categories.Count} categories from the database.");

            return Json(categories.Select(x => new CategoryResponseModel
            {
                Id = x.categoryEntity.Id,
                Title = x.categoryEntity.Title,
                ProductCount = x.productCount
            }));
        }

        /// <summary>
        /// Creates a new category
        /// </summary>
        /// <response code="200">Category successfully created</response> 
        /// <response code="400">Invalid model or a category with the specified title exists already</response>  
        /// <response code="403">No permission to create categories</response>  
        /// <response code="500">An internal error occurred</response>
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateNew([FromBody]CategoryCreateRequestModel model)
        {
            _logger.LogDebug($"Attempting to create new category.");

            if (model != null && ModelState.IsValid)
            {
                // Check if a category with the requested name exists already
                if (_categoryService.DoesCategoryNameExist(model.Title))
                {
                    _logger.LogWarning($"A category with the name {model.Title} exists already. Aborting request.");
                    return BadRequest(new ErrorResponseModel("A category with this name exists already."));
                }

                var categoryEntity = new CategoryEntity
                {
                    Title = model.Title
                };

                await _categoryService.AddAsync(categoryEntity);

                _logger.LogInformation($"Successfully added new category with title {model.Title}.");

                return Json(new
                {
                    categoryEntity.Id,
                    categoryEntity.Title
                });
            }
            else
            {
                _logger.LogWarning($"Error while creating new category. Validation failed.");

                return BadRequest(ModelState.ToErrorResponseModel());
            }
        }

        /// <summary>
        /// Updates the specified category
        /// </summary>
        /// <response code="200">Category successfully updated</response> 
        /// <response code="400">Invalid model or a category with the specified title exists already</response>  
        /// <response code="403">No permission to update categories</response>  
        /// <response code="500">An internal error occurred</response>
        [HttpPut("{categoryId}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Update([FromRoute] Guid categoryId, [FromBody]CategoryUpdateRequestModel model)
        {
            _logger.LogDebug($"Attempting to update category with id {categoryId}.");

            if (model != null && ModelState.IsValid && categoryId == model.Id)
            {
                // Check if category exists
                var categoryEntity = await _categoryService.GetByIdAsync(categoryId);

                if (categoryEntity == null)
                {
                    _logger.LogWarning($"Cannot update category. A category with id {categoryId} does not exist.");
                    return BadRequest(new ErrorResponseModel($"A category with id {categoryId} does not exist."));
                }

                // Check if a category with the requested (new) name exists already -> be sure to let a model with a non-changed title pass
                if (_categoryService.DoesCategoryNameExist(model.Title) && categoryEntity.Title.ToLower() != model.Title.ToLower())
                {
                    _logger.LogWarning($"A category with the name {model.Title} exists already. Aborting request.");
                    return BadRequest(new ErrorResponseModel("A category with this name exists already."));
                }

                categoryEntity.Title = model.Title;

                await _categoryService.UpdateAsync(categoryEntity);

                _logger.LogInformation($"Successfully updated category with title {model.Title}.");

                return Json(new
                {
                    categoryEntity.Id,
                    categoryEntity.Title
                });
            }
            else
            {
                _logger.LogWarning($"Error while updating category. Validation failed.");

                return BadRequest(ModelState.ToErrorResponseModel());
            }
        }

        /// <summary>
        /// Removes the specified category.
        /// All products with the specified category will be removed including corresponding images and products in wishlists
        /// </summary>
        /// <response code="200">Category successfully deleted</response> 
        /// <response code="400">Invalid model or a category with the specified id does not exist</response>  
        /// <response code="403">No permission to delete categories</response>  
        /// <response code="500">An internal error occurred</response> 
        [HttpDelete("{categoryId}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete([FromRoute] Guid categoryId)
        {
            _logger.LogDebug($"Attempting to delete category with id {categoryId}.");

            if (categoryId != null)
            {
                // Check if category exits
                var category = await _categoryService.GetByIdAsync(categoryId);

                if (category == null)
                {
                    _logger.LogWarning($"A category with id {categoryId} does not exist.");
                    return BadRequest(new ErrorResponseModel($"A category with with id {categoryId} does not exist."));
                }

                var productList = await _categoryService.GetAllProductyByCategoryIdAsync(categoryId);

                if (productList == null)
                {              
                    _logger.LogWarning($"The category with id {categoryId} has no products.");
                    return BadRequest(new ErrorResponseModel($"The category with id {categoryId} has no products at all."));
                }
                
                // Remove all Wishlists with the products of this category first
                await _wishlistService.DeleteWishlistsByProductsAsync(productList);
                
                // Remove all products with the specified category
                await _productService.DeleteAllAsync(productList);

                //Remove all images of the products of this category
                await _imageService.DeleteImagesByCategoryAsync(productList);
                
                // Remove the category itself
                await _categoryService.DeleteAsync(category);

                return Ok();
            }
            else
            {
                _logger.LogDebug($"Error while deleting category. Validation failed.");

                return BadRequest(new ErrorResponseModel("Please specify a valid category id."));
            }
        }
    }
    
    
}