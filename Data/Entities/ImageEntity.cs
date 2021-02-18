using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities
{
    public sealed class ImageEntity : EntityBase
    {
        /// <summary>
        /// Description of the image
        /// </summary>
        [Column("description")]
        [Required] public string Description { get; set; }

        /// <summary>
        /// Base64 representation of the image
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [Column("base_64_string")]
        public string Base64String { get; set; }

        /// <summary>
        /// The type of the image. (Must be png or jpg)
        /// </summary>
        [Required]
        [RegularExpression("(png|jpg|jpeg)")]
        [Column("image_type")]
        public string ImageType { get; set; }
    }
}