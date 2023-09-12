using System.ComponentModel.DataAnnotations;

namespace GestBook.Models
{
    public class RegisterModel
    {
        [Required]
        [Display(Name = "логин: ")]
        public string? Login { get; set; }

        [Required]
        [Display(Name = "пароль: ")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required]
        [Display(Name = "подтвердите пароль: ")]
        [Compare("Password", ErrorMessage = "пароли не совпадают")]
        [DataType(DataType.Password)]
        public string? PasswordConfirm { get; set; }
    }
}
