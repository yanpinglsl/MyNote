using System.ComponentModel.DataAnnotations;

namespace GlobalizationLocalizationDemo.Dto
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "UserNameIsRequired")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "PasswordIsRequired")]
        [StringLength(8, ErrorMessage = "PasswordLeastCharactersLong", MinimumLength = 6)]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "PasswordDoNotMatch")]
        public string ConfirmPassword { get; set; }
    }
}
