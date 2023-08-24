using System.ComponentModel.DataAnnotations;

namespace GestBook.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public ICollection<Message>? Messages { get; set; }
    }
}
