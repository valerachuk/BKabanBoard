using System.ComponentModel.DataAnnotations;

namespace BKabanApi.Models
{
    public class UserCredentials
    {
        [Required(ErrorMessage = "Email required")]
        [MaxLength(30, ErrorMessage = "Max email length is {1}")]
        [MinLength(6, ErrorMessage = "Min email length is {1}")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password required")]
        [MaxLength(30, ErrorMessage = "Max password length is {1}")]
        [MinLength(6, ErrorMessage = "Min password length is {1}")]
        public string Password { get; set; }
    }
}
