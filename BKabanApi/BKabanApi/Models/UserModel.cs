using System.ComponentModel.DataAnnotations;

namespace BKabanApi.Models
{
    public class UserModel
    {
        [Required(ErrorMessage = "Username required")]
        [MaxLength(20, ErrorMessage = "Max email length is {1}")]
        [MinLength(2, ErrorMessage = "Min email length is {1}")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password required")]
        [MaxLength(100, ErrorMessage = "Max password length is {1}")]
        [MinLength(6, ErrorMessage = "Min password length is {1}")]
        public string Password { get; set; }
    }
}
