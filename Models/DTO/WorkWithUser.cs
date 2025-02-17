using System.ComponentModel.DataAnnotations.Schema;

namespace BackTareas.Models.DTO
{
    public class WorkWithUser
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public WorkState Status { get; set; }

        public DateTime DateTime { get; set; }

        public UserDTO User { get; set; }
    }
}
