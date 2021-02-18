using System;
using System.Linq;
using System.Threading.Tasks;
using API.Filters;
using API.Helpers;
using API.Services;
using Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    [Route("api/v1/wishlist")]
    [Authorize]
    [ApiV1ExceptionFilter]
    public sealed class WishlistController : Controller
    {
        private readonly IProductService productService;
        private readonly ILoginService loginService;
        private readonly ILogger logger;
        private readonly IWishlistService WishlistService;

        public WishlistController(ILogger<WishlistController> logger, IProductService productService,
            ILoginService loginService, IWishlistService wishlistService)
        {
            this.logger = logger;
            this.productService = productService;
            this.loginService = loginService;
            WishlistService = wishlistService;
        }

        /// <summary>
        /// Adds a product to the wishlist of the current logged in user
        /// </summary>
        /// <param name="productId">The id of the Product</param>
        /// <response code="200">Product added to wishlist successfully</response>
        /// <response code="400">Invalid model</response>
        /// <response code="409">Product already on the wishlist</response>
        /// <response code="500">An internal error occurred</response>
        [HttpPost("{productId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        public async Task<IActionResult> AddProduct([FromRoute] Guid productId)
        {
            if (ModelState.IsValid)
            {
                logger.LogDebug($"Attempting to add new wishlist with productId {productId}.");

                if (!await productService.DoesProductExistByIdAsync(productId))
                {
                    logger.LogDebug($"Failed to add product with id {productId} to a wishlist. The given Product id doesn't exist at all.");
                    return BadRequest("The product Id doesn't exist");
                }

                var userId = loginService.GetLoggedInUserId();

                var wishlistEntity = new WishlistEntity
                {
                    UserId = userId,
                    ProductId = productId
                };

                if (await WishlistService.DoesWishlistItemExistsAsync(wishlistEntity))
                {
                    logger.LogDebug($"Failed to add product with id {productId} to a wishlist. The item already is on the wishlist");
                    return Conflict("The item already is on the wishlist");
                }

                await WishlistService.AddAsync(wishlistEntity);

                logger.LogInformation($"Successfully added new product to the wishlist.");

                return Ok(wishlistEntity);
            }
            else
            {
                logger.LogWarning($"Erorr while adding product. Validation failed");

                return BadRequest(ModelState.ToErrorResponseModel());
            }
        }

        /// <summary>
        /// Removes a product from the wishlist of the current logged in user
        /// </summary>
        /// <param name="productId">Id of the product</param>
        /// <response code="200">Product removed from wishlist successfully</response>
        /// <response code="400">Invalid model</response>
        /// <response code="500">An internal error occurred</response>
        [HttpDelete("{productId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        public async Task<IActionResult> Delete([FromRoute] Guid productId)
        {
            if (ModelState.IsValid)
            {
                logger.LogDebug($"Attempting to remove product with productId {productId} from wishlist.");

                var userId = loginService.GetLoggedInUserId();

                var wishlistEntity = new WishlistEntity
                {
                    UserId = userId,
                    ProductId = productId
                };

                if (!await WishlistService.DoesWishlistItemExistsAsync(wishlistEntity))
                {
                    logger.LogDebug($"Failed to remove product with id {productId} from wishlist. There is no such product on the wishlist");
                    return BadRequest("The item already is not on the wishlist");
                }

                await WishlistService.DeleteAsync(wishlistEntity);

                logger.LogInformation($"Successfully removed product from the wishlist.");

                return Ok(wishlistEntity);
            }
            else
            {
                logger.LogWarning($"Erorr while removing product from Wishlist. Validation failed");

                return BadRequest(ModelState.ToErrorResponseModel());
            }
        }

        /// <summary>
        /// Gets a list of products which are on the wishlist of the current logged in user.
        /// </summary>
        /// <response code="200">Products returned successfully</response>
        /// <response code="400">Invalid model</response>
        /// <response code="500">An internal error occurred</response>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        public async Task<IActionResult> GetAllProducts()
        {
            if (ModelState.IsValid)
            {
                logger.LogDebug($"Attempting to retrieve all products from the wishlist.");

                var userId = loginService.GetLoggedInUserId();

                var products = await WishlistService.GetAllProductsAsync(userId);

                logger.LogInformation($"Successfully retrieved all products from the wishlist.");

                return Json(products.Select(x => x.ToProductResponseModel()).ToList());
            }
            else
            {
                logger.LogWarning($"Erorr while getting products. Validation failed");

                return BadRequest(ModelState.ToErrorResponseModel());
            }
        }

        /// <summary>
        /// Deletes all products on the wishlist of the current logged in user.
        /// </summary>
        /// <response code="200">Products removed successfully</response>
        /// <response code="400">Invalid model</response>
        /// <response code="500">An internal error occurred</response>
        [HttpDelete]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(typeof(ErrorResponseModel), 500)]
        public async Task<IActionResult> DeleteAll()
        {
            if (ModelState.IsValid)
            {

                var userId = loginService.GetLoggedInUserId();

                await WishlistService.DeleteAll(userId);

                logger.LogInformation($"Successfully removed all products from the wishlist.");

                return Ok();
            }
            else
            {
                return BadRequest(ModelState.ToErrorResponseModel());
            }
        }
    }
}