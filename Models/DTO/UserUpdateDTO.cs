using System.ComponentModel.DataAnnotations;

namespace BackTareas.Models.DTO
{
    public class UserUpdateDTO
    {
        [Required(ErrorMessage = "El nombre es necesario")]
        public string Name { get; set; }

        [Required(ErrorMessage = "El correo es requerido")]
        [EmailAddress(ErrorMessage = "El formato del correo es incorrecto")]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
