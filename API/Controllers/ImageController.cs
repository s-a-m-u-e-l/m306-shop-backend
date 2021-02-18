using System;
using System.Threading.Tasks;
using API.Filters;
using API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    [Microsoft.AspNetCore.Components.Route("api/v1/images")]
    [ApiV1ExceptionFilter]
    public sealed class ImageController : Controller
    {
        private readonly ILogger logger;
        private readonly IImageService imageService;

        public ImageController(ILogger<ImageController> logger, IImageService imageService)
        {
            this.logger = logger;
            this.imageService = imageService;
        }

        [HttpGet("{imageId}")]
        public async Task<IActionResult> Get([FromRoute] Guid imageId)
        {
            try
            {
                logger.LogDebug($"Attempting to get image with id {imageId}.");

                var image = await imageService.GetByIdAsync(imageId);

                // Check if image exists at all
                if (image == null)
                {
                    logger.LogWarning($"Error while getting image: Image with id {imageId} does not exist.");

                    return BadRequest(new ErrorResponseModel($"A Image with id {imageId} does not exist."));
                }

                return Json(new
                {
                    image.Id,
                    image.Description,
                    image.Base64String,
                    image.ImageType
                });
            }
            catch (Exception ex)
            {
                logger.LogError($"Error while getting image with id {imageId}.");
                logger.LogError(ex.Message);
                return StatusCode(500, new ErrorResponseModel("Error while getting image."));
            }
        }
    }
}