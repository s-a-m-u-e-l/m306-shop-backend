using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace API.Helpers
{
    /// <summary>
    /// Extensions regarding <see cref="ModelStateDictionary"/>
    /// </summary>
    public static class ModelStateDictionaryExtensions
    {
        /// <summary>
        /// Converts the errors within a <see cref="ModelStateDictionary"/> into an instance of <see cref="ErrorResponseModel"/>
        /// </summary>
        /// <param name="modelStateDictionary"></param>
        /// <returns>Instance of <see cref="ErrorResponseModel"/></returns>
        public static ErrorResponseModel ToErrorResponseModel(this ModelStateDictionary modelStateDictionary)
        {
            var validationErrors = modelStateDictionary.Keys
                 .SelectMany(x => modelStateDictionary[x].Errors.Select(y => new ErrorResponseModel.ValidationError(x, y.ErrorMessage)))
                 .ToList();

            return new ErrorResponseModel
            {
                ErrorMessage = "Validation failed.",
                ValidationErrors = validationErrors
            };
        }
    }
}
