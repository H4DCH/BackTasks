using BackTareas.Models;

namespace BackTareas.Repository.IRepository
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetById(int id);
        Task Update(User model);
        Task<User> Create(User model);
        Task<User> GetByEmail(string email);   
        Task<string>Login(User model);
        Task<User> GetUserByEmailAndPasswordAsync(string email, string password);
    }
}
