using System.ComponentModel.DataAnnotations;

namespace GestBook.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Поле должно быть заполнено")]
        [Display(Name = "Имя пользователя: ")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Поле должно быть заполнено")]
        [Display(Name = "Пароль: ")]
        public string Password { get; set; }
        public ICollection<Message>? Messages { get; set; }
    }
}
