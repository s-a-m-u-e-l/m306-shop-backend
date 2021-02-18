using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities
{
    public sealed class CategoryEntity : EntityBase
    {
        /// <summary>
        /// Category title
        /// </summary>
        [Column("title")]
        public string Title { get; set; }

        public List<ProductEntity> Products { get; set; }
    }
}