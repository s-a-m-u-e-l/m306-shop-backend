using System.ComponentModel.DataAnnotations;

namespace Core.Models.Requests
{
    /// <summary>
    /// Model describing a login request
    /// </summary>
    public sealed class LoginRequestModel
    {
        /// <summary>
        /// The user's email
        /// </summary>
        [EmailAddress]
        [Required(AllowEmptyStrings = false, ErrorMessage = "An email must be supplied")]
        public string EMail { get; set; }

        /// <summary>
        /// The user's password
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "A password must be supplied")]
        public string Password { get; set; }
    }
}
