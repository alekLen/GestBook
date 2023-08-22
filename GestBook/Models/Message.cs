namespace GestBook.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string? MessageDate { get; set; }
        public User? user { get; set; }
    }
}
