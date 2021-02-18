using System.ComponentModel.DataAnnotations;

namespace Core.Models.Requests
{
    /// <summary>
    /// Model describing a category create/update request
    /// </summary>
    public abstract class CategoryCreateUpdateRequestModelBase
    {
        /// <summary>
        /// Category title
        /// </summary>
        [Required(ErrorMessage = "A category title must be provided", AllowEmptyStrings = false)]
        public string Title { get; set; }
    }
}
