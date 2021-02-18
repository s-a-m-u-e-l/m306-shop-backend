using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities
{
    public sealed class WishlistEntity
    {
        /// <summary>
        /// The user's Id
        /// </summary>
        [Required]
        [Column("user_id")]
        public Guid UserId{ get; set; }
        
        /// <summary>
        /// The product's Id
        /// </summary>
        [Required]
        [Column("product_id")]
        public Guid ProductId { get; set; }
        
        public ProductEntity Product{ get; set; }
    }
}