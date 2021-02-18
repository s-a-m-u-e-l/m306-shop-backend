using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities
{
    public sealed class ConsolidatedProductEntity
    {
        /// <summary>
        /// Product id
        /// </summary>
        [Column("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Product artist
        /// </summary>
        [Column("artist")]
        public string Artist { get; set; }

        /// <summary>
        /// Product currency
        /// </summary>
        [Column("currency")]
        public string Currency { get; set; }

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

        /// <summary>
        /// Product image id. 
        /// </summary>
        [Column("image_id")]
        public Guid ImageId { get; set; }

        /// <summary>
        /// Product label
        /// </summary>
        [Column("label")]
        public string Label { get; set; }

        /// <summary>
        /// Product language
        /// </summary>
        [Column("language")]
        public string Language { get; set; }

        /// <summary>
        /// Product price
        /// </summary>
        [Column("price")]
        public decimal Price { get; set; }

        /// <summary>
        /// Product release date
        /// </summary>
        [Column("release_date")]
        public DateTime ReleaseDate { get; set; }

        /// <summary>
        /// Product's category id
        /// </summary>
        [Column("category_id")]
        public Guid CategoryId { get; set; }

        /// <summary>
        /// Product title
        /// </summary>
        [Column("title")]
        public string Title { get; set; }
    }
}