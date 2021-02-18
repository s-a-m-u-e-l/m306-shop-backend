using System.ComponentModel.DataAnnotations;

namespace Core.Models.Requests
{
    /// <summary>
    /// Model describing a paged request
    /// </summary>
    public abstract class PagingSortingFilteringRequestModelBase
    {
        /// <summary>
        /// Specifies the page which should be retrieved
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// Specifies how many items should be returned per page
        /// </summary>
        [Range(5, 50, ErrorMessage = "Value from 5-50 are supported only")]
        public int ItemsPerPage { get; set; } = 10;

        /// <summary>
        /// Specifies the sorting column
        /// </summary>
        public abstract string SortBy { get; set; }

        /// <summary>
        /// Specifies the sorting direction
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Sort direction is required")]
        [RegularExpression("(ASC|DESC)", ErrorMessage = "Sort direction can only be ascending (ASC) or descending (DESC)")]
        public string SortDirection { get; set; } = "ASC";

        /// <summary>
        /// Specifies the filter column
        /// </summary>
        public abstract string FilterBy { get; set; }

        /// <summary>
        /// Specifies the filter string
        /// </summary>
        public string FilterQuery { get; set; }

        /// <summary>
        /// Specifies the language code to select the correct l10n for searching and filtering
        /// </summary>
        // [Required(AllowEmptyStrings = false, ErrorMessage = "Please specify a language code")]
        // public string LanguageCode { get; set; }
    }
}
