using System.ComponentModel.DataAnnotations;

namespace Core.Models.Requests
{
    /// <summary>
    /// Class representing an product image create/update request
    /// </summary>
    public abstract class ProductImageCreateUpdateRequestModelBase
    {
        /// <summary>
        /// Image description
        /// </summary>
        [Required]
        public string Description { get; set; }

        /// <summary>
        /// Image data base64-encoded
        /// </summary>
        [Required]
        public string Base64String { get; set; }

        /// <summary>
        /// Image type
        /// </summary>
        [Required]
        [RegularExpression("(png)|(jpg)|(jpeg)")]
        public string ImageType { get; set; }
    }
}
