using BackTareas.Models;

namespace BackTareas.Repository.IRepository
{
    public interface IUserReminderRepository
    {
        Task<User> getUserChat(int id);
    }
}
