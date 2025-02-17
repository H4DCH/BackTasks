using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackTareas.Models
{
    public class Work
    {
        [Key]
       public int Id { get; set; }
        [Required(ErrorMessage = "El titulo es requerida")]  
        public string Title { get;set; }
        [Required(ErrorMessage ="La descripcion es requerida")]
        public string Description { get; set; }

        [Required(ErrorMessage ="El estado es requerido")]  
        public WorkState Status { get; set; }

        [Required(ErrorMessage = "La hora de recordatorio es requerida")]
        public DateTime DateTime { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }  
    }
}
