using BackTareas.Jobs;
using BackTareas.Repository.IRepository;
using Quartz;
using Telegram.Bot;

namespace BackTareas.Service
{
    public class ReminderService
    {
        private readonly IScheduler _scheduler;
        private readonly ITelegramBotClient _botClient;
        private readonly IReminderRepository _reminderRepository;

        public ReminderService(IScheduler scheduler, ITelegramBotClient botClient, IReminderRepository reminderRepository)
        {
            _scheduler = scheduler;
            _botClient = botClient;
            _reminderRepository = reminderRepository;
        }

        public async Task ScheduleReminder(string chatId, string message, DateTimeOffset reminderTime)
        {
            // Verificar si el scheduler está iniciado
            if (!_scheduler.IsStarted)
            {
                await _scheduler.Start();
            }

            // Crear el JobDataMap y agregar los objetos complejos
            var jobDataMap = new JobDataMap
            {
                { "ChatId", chatId },
                { "Message", message },
                { "ReminderTime", reminderTime.ToString("o") }, // Guardar la fecha como string
                { "BotClient", _botClient }, // Agregar el bot client
                { "ReminderRepository", _reminderRepository } // Agregar el repositorio
            };

            // Crear el job y asignar el JobDataMap
            var job = JobBuilder.Create<ReminderJob>()
                .SetJobData(jobDataMap) // Asignar el JobDataMap
                .Build();

            // Crear el trigger
            var trigger = TriggerBuilder.Create()
                .StartAt(reminderTime) // Programar el trigger para la fecha específica
                .Build();

            // Programar el job
            await _scheduler.ScheduleJob(job, trigger);
            Console.WriteLine($"Recordatorio programado para {reminderTime}.");
        }
    }
}