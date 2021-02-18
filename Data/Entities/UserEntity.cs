using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities
{
    public sealed class UserEntity : EntityBase
    {
        /// <summary>
        /// The user's first name
        /// </summary>
        [Required]
        [Column("first_name")]
        public string FirstName { get; set; }

        /// <summary>
        /// The user's last name
        /// </summary>
        [Required]
        [Column("last_name")]
        public string LastName { get; set; }

        /// <summary>
        /// The user's billing address' id
        /// </summary>
        //[Required]
        //[Column("billing_address_id")]
        //public Guid BillingAddressId { get; set; }

        /// <summary>
        /// The users's address' id
        /// </summary>
        //[Required]
        //[Column("address_id")]
        //public Guid AddressId { get; set; }

        /// <summary>
        /// The user's email
        /// </summary>
        [Required]
        [Column("email")]
        public string Email { get; set; }

        /// <summary>
        /// The user's password
        /// </summary>
        [Required]
        [Column("password")]
        public string Password { get; set; }

        /// <summary>
        /// Indicates if the user is admin
        /// </summary>
        [Required]
        [Column("is_admin")]
        public bool IsAdmin { get; set; }
    }
}