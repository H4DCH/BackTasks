using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BackTareas.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage ="El nombre es necesario")]  
        public string Name { get; set; }

        [Required (ErrorMessage ="El correo es requerido")]
        [EmailAddress(ErrorMessage ="El formato del correo es incorrecto")]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }   
        public ICollection<Work>? Works { get; set; }
        [Required]
        public string ChatId { get; set; }
    }
}
