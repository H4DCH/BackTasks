using BackTareas.Repository.IRepository;
using Quartz;
using Telegram.Bot;

namespace BackTareas.Jobs
{
    public class ReminderJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                // Obtener las dependencias desde el JobDataMap
                var jobData = context.JobDetail.JobDataMap;

                if (!jobData.ContainsKey("ChatId") || !jobData.ContainsKey("Message") || !jobData.ContainsKey("ReminderTime") || !jobData.ContainsKey("BotClient") || !jobData.ContainsKey("ReminderRepository"))
                {
                    throw new InvalidOperationException("Faltan datos necesarios en JobDataMap.");
                }

                var chatId = jobData.GetString("ChatId");
                var message = jobData.GetString("Message");
                var reminderTimeString = jobData.GetString("ReminderTime");
                var botClient = (ITelegramBotClient)jobData["BotClient"]; // Obtener el bot client
                var reminderRepository = (IReminderRepository)jobData["ReminderRepository"]; // Obtener el repositorio

                if (string.IsNullOrEmpty(chatId) || string.IsNullOrEmpty(message) || string.IsNullOrEmpty(reminderTimeString))
                {
                    throw new InvalidOperationException("ChatId, Message o ReminderTime no pueden ser nulos o vacíos.");
                }

                var reminderTime = DateTimeOffset.Parse(reminderTimeString);

                if (DateTimeOffset.Now >= reminderTime)
                {
                    await botClient.SendMessage(chatId: chatId, text: message);
                }
                else
                {
                    Console.WriteLine($"El recordatorio se enviará en: {reminderTime}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al enviar el mensaje: {ex.Message}");
                throw; // Relanzar la excepción para que Quartz la maneje
            }
        }
    }
}