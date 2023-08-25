using GestBook.Models;
namespace GestBook.Repository
{
    public interface IRepository
    {
        Task<Salt> GetSalt(User user);      
        Task<User> GetUser(string name);
        Task AddUser(User user);
        Task AddSalt(Salt s);
        Task AddMessage(Message mess);
        Task Save();
    }
} 
