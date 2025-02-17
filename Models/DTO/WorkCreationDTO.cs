using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BackTareas.Models.DTO
{
    public class WorkCreationDTO
    {
        [Required(ErrorMessage = "El titulo es requerido")]
        public string Title { get; set; }

        [Required(ErrorMessage = "La descripcion es requerida")]
        public string Description { get; set; }

        [Required(ErrorMessage = "La hora de recordatorio es requerida")]
        public DateTime DateTime { get; set; }

        public int UserId { get; set; }
    }
}
