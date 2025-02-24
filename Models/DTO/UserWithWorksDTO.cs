using System.ComponentModel.DataAnnotations;

namespace BackTareas.Models.DTO
{
    public class UserWithWorksDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }
        public ICollection<WorkDTO>? Works { get; set; }
        public string ChatId { get; set; }
    }
}
