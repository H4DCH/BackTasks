using BackTareas.Data;
using BackTareas.Models;
using BackTareas.Repository.IRepository;

namespace BackTareas.Repository
{
    public class UserReminderRepository : IUserReminderRepository
    {
        private readonly Context _context;
        public UserReminderRepository(Context context)
        {
            _context = context;
        }
        public async Task<User> getUserChat(int id)
        {
            var user = await _context.Users.FindAsync(id);

            return user;
        }
    }
}
