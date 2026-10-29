// DTOs/TableDtos.cs
using System.ComponentModel.DataAnnotations;

namespace LeoKitchenFasti.DTOs
{
    public class AreaCreateDto
    {
        [Required(ErrorMessage = "El nombre del área es obligatorio")]
        public string Name { get; set; } = string.Empty;
    }

    public class TableCreateDto
    {
        [Required(ErrorMessage = "El nombre de la mesa es obligatorio")]
        public string Name { get; set; } = string.Empty;

        [Required]
        public int AreaId { get; set; }
    }
}