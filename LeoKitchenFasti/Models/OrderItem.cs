namespace LeoKitchenFasti.Models
{
    public enum ItemStatus { Pendiente, EnPreparacion, Listo, Entregado, Cancelado }

    public class OrderItem
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int Quantity { get; set; }
        public string? Notes { get; set; }

        public ItemStatus Status { get; set; } = ItemStatus.Pendiente;
    }
}