using System;
using System.Collections.Generic;

namespace Core.Models
{
     public sealed class ProductReponseModel
    {
        /// <summary>
        /// Product id
        /// </summary>
        public Guid Id { get; set; }
        
        
        /// <summary>
        /// Product seller
        /// </summary>
        public UserResponseModel User { get; set; }

        /// <summary>
        /// Product label
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Product release date
        /// </summary>
        public DateTime ReleaseDate { get; set; }

        /// <summary>
        /// Product category id
        /// </summary>
        public Guid CategoryId { get; set; }
        public CategoryResponseModel Category { get; set; }

        /// <summary>
        /// Associated product image
        /// </summary>
        public ImageResponseModel Image { get; set; }

        /// <summary>
        /// price
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Product title
        /// </summary>
        public string Title { get; set; }
        
        /// <summary>
        /// Product description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Product short description
        /// </summary>
        public string DescriptionShort { get; set; }
        
        public sealed class UserResponseModel
        {
            
            /// <summary>
            /// user id
            /// </summary>
            public Guid Id { get; set; }
            
            /// <summary>
            /// The user's first name
            /// </summary>
            public string FirstName { get; set; }

            /// <summary>
            /// The user's last name
            /// </summary>
            public string LastName { get; set; }

            /// <summary>
            /// The user's email
            /// </summary>
            public string Email { get; set; }
        }
        public sealed class ImageResponseModel
        {
            /// <summary>
            /// Image description
            /// </summary>
            public string Description { get; set; }

            /// <summary>
            /// Base64 image data
            /// </summary>
            public string Base64String { get; set; }

            /// <summary>
            /// Image type
            /// </summary>
            public string ImageType { get; set; }
        }
        public sealed class CategoryResponseModel
        {
            public string Title { get; set; }
        }
    }
}