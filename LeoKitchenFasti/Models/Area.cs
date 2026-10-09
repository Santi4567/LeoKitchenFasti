namespace LeoKitchenFasti.Models
{
    public class Area
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // Ej: "Terraza", "Palapa", "Interior"

        // Relación inversa: Un área puede tener muchas mesas
        public List<Table> Tables { get; set; } = new();
    }
}