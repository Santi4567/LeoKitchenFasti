using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LeoKitchenFasti.Models
{
    [Table("users")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        [JsonIgnore]
        public string Passwd { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        // Relación con la tabla Roles
        public int RolId { get; set; }

        [ForeignKey("RolId")]
        public Rol Rol { get; set; }

        public bool IsActive { get; set; } = true;

        // Relación con la tabla de UserSession
        public ICollection<UserSession> Sessions { get; set; } = new List<UserSession>();
    }
}