using System.Collections.Generic;

namespace API.Helpers
{
    public sealed class PagingInformation<T>
    {
        /// <summary>
        /// Specifies how many pages exist respecting the currently set <see cref="ItemsPerPage"/>
        /// </summary>
        public int PageCount { get; set; }

        /// <summary>
        /// Specifies how many items should be served per page
        /// </summary>
        public int ItemsPerPage { get; set; }

        /// <summary>
        /// Specifies how many items exist at total
        /// </summary>
        public int TotalItems { get; set; }

        /// <summary>
        /// Specifies the current page
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Specifies the actual page payload
        /// </summary>
        public List<T> Items { get; set; } = new List<T>();
    }
}