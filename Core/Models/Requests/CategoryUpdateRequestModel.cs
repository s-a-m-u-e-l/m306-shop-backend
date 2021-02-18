using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Models.Requests
{
    /// <summary>
    /// Model describing a category update request
    /// </summary>
    public sealed class CategoryUpdateRequestModel : CategoryCreateUpdateRequestModelBase
    {
        /// <summary>
        /// Category id
        /// </summary>
        [Required(ErrorMessage = "A id of a category is required in order to update it")]
        public Guid Id { get; set; }
    }
}
