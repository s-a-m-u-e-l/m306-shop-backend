using Core.Models;
using Data.Entities;

namespace API.Helpers
{
    /// <summary>
    /// Product entity extension methods
    /// </summary>
    public static class ProductEntityExtensions
    {
        /// <summary>
        /// Converts an instance of <see cref="ProductEntity"/>/>
        /// </summary>
        /// <param name="productEntity">Instance of <see cref="ProductEntity"/> to convert</param>
        /// <returns>Instance of <see cref="ProductReponseModel"/></returns>
        public static ProductReponseModel ToProductResponseModel(this ProductEntity productEntity)
        {
            var productResponse = new ProductReponseModel
            {
                Id = productEntity.Id,
                CategoryId = productEntity.CategoryId,
                Price = productEntity.Price,
                Title = productEntity.Title,
                Description = productEntity.Description,
                DescriptionShort = productEntity.DescriptionShort,
                Category = new ProductReponseModel.CategoryResponseModel
                {
                    Title = productEntity.Category.Title
                },
                User = new ProductReponseModel.UserResponseModel
                {
                    FirstName = productEntity.User.FirstName,
                    LastName = productEntity.User.LastName,
                    Email = productEntity.User.Email
                },
                Image = new ProductReponseModel.ImageResponseModel
                {
                    Base64String = productEntity.Image.Base64String,
                    Description = productEntity.Image.Description,
                    ImageType = productEntity.Image.ImageType
                },
                Label = productEntity.Label,
                ReleaseDate = productEntity.ReleaseDate
            };
            return productResponse;
        }
    }
}
