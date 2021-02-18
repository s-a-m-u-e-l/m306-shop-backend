using System.ComponentModel.DataAnnotations;

namespace Core.Models.Requests
{
    /// <summary>
    /// Model describing a user update request
    /// </summary>
    public sealed class UserUpdateRequestModel : UserCreateUpdateRequestModelBase
    {
        /// <summary>
        /// User's password
        /// </summary>
        [StringLength(255, MinimumLength = 4, ErrorMessage = "The password must be of length between 4 and 255.")]
        public string Password { get; set; }
    }
}
