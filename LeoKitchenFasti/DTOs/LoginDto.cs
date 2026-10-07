using System.ComponentModel.DataAnnotations;

namespace LeoKitchenFasti.DTOs
{
    public class LoginDto
    {
        [Required]
        public string UsuarioOCorreo { get; set; } // Acepta user o email

        [Required]
        public string Password { get; set; }
    }
}
