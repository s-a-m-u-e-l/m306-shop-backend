using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities
{
    public sealed class ProductEntity : EntityBase
    {
        /// <summary>
        /// Product label
        /// </summary>
        [Column("label")]
        public string Label { get; set; }

        /// <summary>
        /// Product release date
        /// </summary>
        [Column("release_date")]
        public DateTime ReleaseDate { get; set; }

        /// <summary>
        /// Product category id
        /// </summary>
        [Column("category_id")]
        public Guid CategoryId { get; set; }

        /// <summary>
        /// Product user / seller
        /// </summary>
        [Column("user_id")]
        public Guid UserId { get; set; }
        
        /// <summary>
        /// Product image id. 
        /// </summary>
        [Column("image_id")]
        public Guid ImageId { get; set; }

        /// <summary>
        /// Price
        /// </summary>
        [Required]
        [Column("price")]
        public decimal Price { get; set; }

        /// <summary>
        /// Product title
        /// </summary>
        [Column("title")]
        public string Title { get; set; }
        
        /// <summary>
        /// Product description
        /// </summary>
        [Column("description")]
        public string Description { get; set; }

        /// <summary>
        /// Product short description
        /// </summary>
        [Column("description_short")]
        public string DescriptionShort { get; set; }

        public UserEntity User { get; set; }

        public CategoryEntity Category { get; set; }

        public ImageEntity Image { get; set; }
    }
}