using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Models.Requests
{
    /// <summary>
    /// Model describing a paged product request
    /// </summary>
    public sealed class ProductPagingSortingFilteringRequestModel : PagingSortingFilteringRequestModelBase
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "The parameter sort by is required.")]
        [RegularExpression("^.*(Label|ReleaseDate)$", ErrorMessage = "The parameter sort by must match one of 'Label', 'ReleaseDate'")]
        public override string SortBy { get; set; } = "Artist";

        [RegularExpression("^.*(Label|ReleaseDate|Description|DescriptionShort|Title)$", ErrorMessage = "The parameter filter by must match one of 'Label', 'ReleaseDate', 'Description', 'DescriptionShort' and 'Title'")]
        public override string FilterBy { get; set; }

        /// <summary>
        /// Specifies an optional category id. Products will be pre-filtered by this id before applying the other filters
        /// </summary>
        public Guid? FilterByCategory { get; set; }
    }
}
