namespace LeoKitchenFasti.Models
{
    public enum TableStatus { Libre, Ocupada }

    public class Table
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // Ej: "Mesa 1"
        public TableStatus Status { get; set; } = TableStatus.Libre;
    }
}