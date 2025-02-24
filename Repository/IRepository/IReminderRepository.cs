using BackTareas.Models;

    namespace BackTareas.Repository.IRepository
    {
        public interface IReminderRepository
        {
            void  SaveReminder(string chatId, string message,DateTimeOffset reminderTime);
            IEnumerable<Reminder> GetReminder();
        }
    }
