using System.ComponentModel.DataAnnotations;

namespace UI.Models
{
    public class UserRegistrationModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }

        [StringLength(48, MinimumLength = 4, ErrorMessage = "The Username Must Be Between {2} And {1} Characters Long")]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(48, MinimumLength = 8, ErrorMessage = "The Password Must Be Between {2} And {1} Characters Long")]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The Passwords Do Not Match")]
        [Display(Name = "Confirm Password")]
        public string ConfirmationPassword { get; set; }
    }

    public class UserLoginModel
    {
    }
}