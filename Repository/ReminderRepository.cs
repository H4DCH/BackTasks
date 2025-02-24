using BackTareas.Models;
using BackTareas.Repository.IRepository;
using System.Collections.Concurrent;

namespace BackTareas.Repository
{
    public class ReminderRepository : IReminderRepository
    {
        private readonly ConcurrentDictionary<string, Reminder> _reminders = new();   

        public void SaveReminder(string chatId, string message, DateTimeOffset reminderTime)
        {
            var reminder = new Reminder
            {
                ChatId = chatId,
                Message = message,
                ReminderTime = reminderTime.ToUniversalTime()
            };
            _reminders[chatId]= reminder;
        }

        public  IEnumerable<Reminder> GetReminder()
        {
            return _reminders.Values;
        }
    }
}
