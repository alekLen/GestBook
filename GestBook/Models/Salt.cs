namespace GestBook.Models
{
    public class Salt
    {
        public int Id { get; set; }
        public string? salt { get; set; }
        public User user { get; set; }
    }
}
