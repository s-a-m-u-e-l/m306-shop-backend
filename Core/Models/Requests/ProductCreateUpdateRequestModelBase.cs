﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Core.Models.Requests
{
    /// <summary>
    /// Model describing a product create/update request
    /// </summary>
    public abstract class ProductCreateUpdateRequestModelBase
    {
        /// <summary>
        /// Product user / seller
        /// </summary>
        [Required]
        public Guid UserId { get; set; }

        /// <summary>
        /// Product category's id
        /// </summary>
        [Required]
        public Guid CategoryId { get; set; }

        /// <summary>
        /// Product label
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string Label { get; set; }

        /// <summary>
        /// Product relase date
        /// </summary>
        [Required]
        public DateTime ReleaseDate { get; set; }

        /// <summary>
        /// Product image
        /// </summary>
        [Required]
        public ProductImageCreateRequestModel Image { get; set; }

        /// <summary>
        /// price
        /// </summary>
        [Required]
        public decimal Price { get; set; }

        /// <summary>
        /// Product title
        /// </summary>
        [Required]
        public string Title { get; set; }
        
        /// <summary>
        /// Product description
        /// </summary>
        [Required]
        public string Description { get; set; }

        /// <summary>
        /// Product short description
        /// </summary>
        [Required]
        public string DescriptionShort { get; set; }
    }
}
