using System;
using System.Collections.Generic;
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
    [Route("api/v1/products")]
    [Authorize]
    [ApiV1ExceptionFilter]
    public sealed class ProductController : Controller
    {
        private readonly IImageService _imageService;
        private readonly IProductService _productService;
        private readonly IWishlistService _wishlistService;
        private readonly ILoginService _loginService;

        private readonly ILogger _logger;

        public ProductController(IImageService imageService, IProductService productService, IWishlistService wishlistService,
            ILogger<ProductController> logger, ILoginService loginService)
        {
            _imageService = imageService;
            _productService = productService;
            _wishlistService = wishlistService;
            _loginService = loginService;
            _logger = logger;
        }

        /// <summary>
        /// Returns all products
        /// </summary>
        /// <response code="200">Products returned successfully</response>
        /// <response code="403">No permissions to get raw products</response>
        /// <response code="500">An internal error occurred</response>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(List<ProductReponseModel>), 200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        public async Task<IActionResult> Get()
        {
            _logger.LogDebug($"Attempting to get all products.");

            var products = await _productService.GetAllAsync();

            _logger.LogInformation($"Received {products.Count} products from the database.");

            return Json(products.Select(x => x.ToProductResponseModel()).ToList());
        }

        /// <summary>
        /// Returns all products with paging
        /// </summary>
        /// <param name="page">Page to retrieve</param>
        /// <param name="model">Paging and sorting options</param>
        /// <response code="200">Products returned successfully</response>
        /// <response code="400">Invalid model</response>
        /// <response code="403">No permissions to get raw products</response>
        /// <response code="500">An internal error occurred</response>
        [HttpGet("paged")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(PagingInformation<ProductReponseModel>), 200)]
        [ProducesResponseType(typeof(ErrorResponseModel), 400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        public async Task<IActionResult> GetPaged([FromQuery] ProductPagingSortingFilteringRequestModel model)
        {
            _logger.LogDebug($"Attempting to get paged products: Page: {model.Page}, items per page: {model.ItemsPerPage}.");

            if (ModelState.IsValid)
            {
                // Check if the requested language exists
                // if (!i18nService.SupportedLanguages.Any(x => x.Code == model.LanguageCode))
                // {
                //     return BadRequest(new ErrorResponseModel("The requested language does not exist."));
                // }

                var pagingSortingOptions = new PagingSortingParams
                {
                    ItemsPerPage = model.ItemsPerPage,
                    Page = model.Page,
                    SortBy = model.SortBy,
                    SortDirection = model.SortDirection
                };

                var filterOptions = new FilterParams
                {
                    FilterBy = model.FilterBy,
                    FilterQuery = model.FilterQuery,
                    // FilterLanguage = model.LanguageCode
                };

                var productPagingInformation = await _productService.GetPagedAsync(pagingSortingOptions, filterOptions, model.FilterByCategory);

                var productPagingInformationResponse = new PagingInformation<ProductReponseModel>
                {
                    CurrentPage = productPagingInformation.CurrentPage,
                    Items = productPagingInformation.Items.Select(x => x.ToProductResponseModel()).ToList(),
                    ItemsPerPage = productPagingInformation.ItemsPerPage,
                    PageCount = productPagingInformation.PageCount,
                    TotalItems = productPagingInformation.TotalItems
                };

                _logger.LogInformation(
                    $"Received {productPagingInformationResponse.Items.Count} products from the database.");

                return Json(productPagingInformationResponse);
            }
            else
            {
                _logger.LogWarning($"Error while performing paged request. Validation failed.");
                return BadRequest(ModelState.ToErrorResponseModel());
            }
        }

        /// <summary>
        /// Returns the product with the specified id
        /// </summary>
        /// <param name="id">Product id</param>
        /// <response code="200">Products returned successfully</response>
        /// <response code="403">No permissions to get raw products</response>
        /// <response code="404">Product with the specified id not found</response>
        /// <response code="500">An internal error occurred</response>
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ProductReponseModel), 200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            _logger.LogDebug($"Attempting to get product with id {id}.");

            var product = await _productService.GetByIdAsync(id);

            if (product == null)
            {
                _logger.LogWarning($"Product with id {id} could not be found.");

                return NotFound();
            }

            return Json(product.ToProductResponseModel());
        }

        /// <summary>
        /// Creates a new product
        /// </summary>
        /// <param name="model">Product to create</param>
        /// <response code="200">Product created successfully</response>
        /// <response code="400">Invalid model</response>
        /// <response code="403">No permissions to create new products</response>
        /// <response code="500">An internal error occurred</response>
        [HttpPost]
        [ProducesResponseType(typeof(ProductReponseModel), 200)]
        [ProducesResponseType(typeof(ErrorResponseModel), 400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        public async Task<IActionResult> CreateNew([FromBody] ProductCreateRequestModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                _logger.LogDebug(
                    $"Attempting to add new product.");

                var imageEntity = new ImageEntity
                {
                    Base64String = model.Image.Base64String,
                    Description = model.Image.Description,
                    ImageType = model.Image.ImageType
                };

                // Add image to database
                await _imageService.AddAsync(imageEntity);
                var productEntity = new ProductEntity
                {
                    UserId = model.UserId,
                    CategoryId = model.CategoryId,
                    ImageId = imageEntity.Id,
                    Label = model.Label,
                    ReleaseDate = model.ReleaseDate,
                    Price = model.Price,
                    Title = model.Title,
                    Description = model.Description,
                    DescriptionShort = model.DescriptionShort
                };

                // Add product with prices and translations to database
                await _productService.AddAsync(productEntity);

                _logger.LogInformation($"Successfully added new product.");

                // Get product
                var newProduct = await _productService.GetByIdAsync(productEntity.Id);

                return Ok(newProduct.ToProductResponseModel());
            }
            else
            {
                _logger.LogWarning($"Erorr while adding product. Validation failed");

                return BadRequest(ModelState.ToErrorResponseModel());
            }
        }

        /// <summary>
        /// Updates an existing product
        /// </summary>
        /// <param name="productId">Id of the product to update</param>
        /// <param name="model">Product to update</param>
        /// <response code="200">Product updated successfully</response>
        /// <response code="400">Invalid model</response>
        /// <response code="403">No permission to update products</response>
        /// <response code="500">An internal error occurred</response>
        [HttpPut("{productId}")]
        [ProducesResponseType(typeof(ProductEntity), 200)]
        [ProducesResponseType(typeof(ErrorResponseModel), 400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        public async Task<IActionResult> Update([FromRoute] Guid productId, [FromBody] ProductUpdateRequestModel model)
        {
            // Check if model is valid
            if (model != null && ModelState.IsValid && productId == model.Id)
            {
                _logger.LogDebug($"Attempting to update product with id {productId}.");

                // Get product
                var product = await _productService.GetByIdAsync(productId);

                if (product == null)
                {
                    _logger.LogWarning($"A product with id {productId} does not exist.");
                    return BadRequest(ModelState.ToErrorResponseModel());
                }

                _loginService.GetLoggedInUserId();
                product.CategoryId = model.CategoryId;
                product.Label = model.Label;
                product.ReleaseDate = model.ReleaseDate;
                product.Price = model.Price;
                product.Title = model.Title;
                product.Description = model.Description;
                product.DescriptionShort = model.DescriptionShort;

                var oldImageId = product.ImageId;

                // If the image got updated
                if (product.Image.Base64String != model.Image.Base64String ||
                    product.Image.Description != model.Image.Description ||
                    product.Image.ImageType != model.Image.ImageType)
                {
                    var newImage = new ImageEntity
                    {
                        Base64String = model.Image.Base64String,
                        Description = model.Image.Description,
                        ImageType = model.Image.ImageType
                    };

                    await _imageService.AddAsync(newImage);
                    product.ImageId = newImage.Id;
                }

                await _productService.UpdateAsync(product);

                //Delete old Image
                if (oldImageId != product.ImageId)
                {
                    await _imageService.DeleteAsync(oldImageId);
                }

                _logger.LogInformation($"Successfully updated product with id {model.Id}.");

                var newProduct = await _productService.GetByIdAsync(productId);

                return Ok(newProduct);
            }
            else
            {
                _logger.LogWarning($"Error while updating the product. Validation failed.");

                return BadRequest(ModelState.ToErrorResponseModel());
            }
        }

        /// <summary>
        /// Deletes the product with the specified id including all related prices and translations
        /// </summary>
        /// <param name="productId">Id of the product to delete</param>
        /// <response code="200">Product deleted successfully</response>
        /// <response code="400">Invalid model</response>
        /// <response code="403">No permissions to delete products</response>
        /// <response code="500">An internal error occurred</response>
        [HttpDelete("{productId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ErrorResponseModel), 400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        public async Task<IActionResult> Delete([FromRoute] Guid productId)
        {
            if (ModelState.IsValid)
            {
                _logger.LogDebug($"Attempting to delete product with id {productId}.");

                // Check if product exists
                var product = await _productService.GetByIdAsync(productId);

                if (product == null)
                {
                    _logger.LogWarning(
                        $"Error while deleting the product with id {productId}. The product does not exist.");

                    return NotFound();
                }

                // Delete corresponding wishlists
                await _wishlistService.DeleteByProductId(product.Id);

                await _productService.DeleteAsync(product);

                // Delete corresponding Images
                await _imageService.DeleteAsync(product.ImageId);

                _logger.LogInformation($"Product with id {productId} has been deleted successfully.");

                return Ok();
            }
            else
            {
                _logger.LogWarning($"Error while deleting product with id {productId}.");

                return BadRequest(ModelState.ToErrorResponseModel());
            }
        }
    }
}