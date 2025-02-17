using BackTareas.Data;
using BackTareas.Models;
using BackTareas.Repository.IRepository;
using BackTareas.Utilities;
using Microsoft.EntityFrameworkCore;

namespace BackTareas.Repository
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly Context context;
        private readonly CreatedToken createdToken;
        public UserRepository(Context context, CreatedToken createdToken) : base(context)
        {
            this.context = context;
            this.createdToken = createdToken;   
        }

        public async Task<User> Create(User model)
        {
            model.Password = createdToken.EncryptSHA256(model.Password);
            context.Users.Add(model);   
            await Save();
            return model;
        }

        public async Task<User> GetByEmail(string email)
        {
            var model = await context.Users.FirstOrDefaultAsync(i => i.Email == email);

            return model;
        }

        public async Task<User> GetById(int id)
        {
            var model = await context.Users.Include(w=>w.Works).FirstOrDefaultAsync(i=>i.Id==id);

            return model;
        }

        public Task<string> Login(User model)
        {
            var token = createdToken.GenereatedJWT(model);
            return Task.FromResult(token);
        }

        public async Task Update(User model)
        {
            context.Update(model);
            await Save();
        }

        public async Task<User> GetUserByEmailAndPasswordAsync(string email, string password)
        {
            var encryptedPassword = createdToken.EncryptSHA256(password);
            return await context.Users
                .Where(u => u.Email == email && u.Password == encryptedPassword)
                .FirstOrDefaultAsync();
        }
    }
}
