using System;

namespace Core.Models.Requests
{
    /// <summary>
    /// Model describing a product update request
    /// </summary>
    public sealed class ProductUpdateRequestModel : ProductCreateUpdateRequestModelBase
    {
        /// <summary>
        /// Product id
        /// </summary>
        public Guid Id { get; set; }
    }
}
