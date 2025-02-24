using BackTareas.Data;
using BackTareas.Models;
using BackTareas.Repository.IRepository;
using BackTareas.Utilities;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;

namespace BackTareas.Repository
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly Context context;
        private readonly CreatedToken createdToken;
        private readonly ITelegramBotClient _telegramBotClient;
        public UserRepository(Context context, CreatedToken createdToken, ITelegramBotClient telegramBotClient) : base(context)
        {
            this.context = context;
            this.createdToken = createdToken;
            _telegramBotClient = telegramBotClient;
        }

        public async Task<User> Create(User model)
        {

            // Obtener las actualizaciones del bot
            var updates = await _telegramBotClient.GetUpdates();

            // Buscar el ChatId más reciente
            foreach (var update in updates)
            {
                if (update.Message != null && update.Message.Chat != null)
                {
                    model.ChatId = update.Message.Chat.Id.ToString();
                    break; // Tomamos el primer ChatId válido y salimos del bucle
                }
            }

            // Validar que se haya obtenido un ChatId
            if (string.IsNullOrEmpty(model.ChatId))
            {
                throw new InvalidOperationException("No se pudo obtener un ChatId válido.");
            }

            // Encriptar la contraseña
            model.Password = createdToken.EncryptSHA256(model.Password);

            // Guardar el usuario en la base de datos
            context.Users.Add(model);
            await context.SaveChangesAsync();

            await _telegramBotClient.DeleteWebhook(dropPendingUpdates: true);

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
