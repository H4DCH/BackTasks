using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BackTareas.Models.DTO
{
    public class WorkDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public WorkState Status { get; set; }

        public DateTime DateTime { get; set; }
    }
}
