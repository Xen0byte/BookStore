using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class UserDTO
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string EmailAddress { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(48, MinimumLength = 8, ErrorMessage = "The Password Must Be Between {2} And {1} Characters Long")]
        public string Password { get; set; }

        [StringLength(48, MinimumLength = 4, ErrorMessage = "The Username Must Be Between {2} And {1} Characters Long")]
        public string Username { get; set; }
    }
}