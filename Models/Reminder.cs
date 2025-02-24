namespace BackTareas.Models
{
    public class Reminder
    {
        public string ChatId { get; set; }
        public string Message { get; set; }
        public DateTimeOffset ReminderTime { get; set; }
    }
}
