using System.ComponentModel.DataAnnotations;

namespace BackTareas.Models.DTO
{
    public class UserDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }
        public string ChatId { get; set; }
    }
}
