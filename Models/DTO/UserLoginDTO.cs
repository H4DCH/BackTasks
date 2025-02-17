using System.ComponentModel.DataAnnotations;

namespace BackTareas.Models.DTO
{
    public class UserLoginDTO
    {
        [Required(ErrorMessage = "El correo es requerido")]
        [EmailAddress(ErrorMessage = "El formato del correo es incorrecto")]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }

}
