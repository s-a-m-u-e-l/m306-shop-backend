using System.ComponentModel.DataAnnotations;

namespace Core.Models.Requests
{
    public sealed class UserCreateRequestModel : UserCreateUpdateRequestModelBase
    {
        [Required(ErrorMessage = "A password is required.", AllowEmptyStrings = false)]
        [StringLength(255, MinimumLength = 4, ErrorMessage = "The password must be of length between 4 and 255.")]
        public string Password { get; set; }
    }
}
