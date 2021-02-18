using System;

namespace Core.Models
{
    public class CategoryResponseModel
    {
        /// <summary>
        /// The category's id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The category's title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Specifies how many products are associated with this category
        /// </summary>
        public long ProductCount { get; set; }
    }
}