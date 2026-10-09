namespace LeoKitchenFasti.Models
{
    public enum OrderStatus { Abierta, Pagada, Cancelada }

    public class Order
    {
        public int Id { get; set; }

        public int TableId { get; set; }
        public Table? Table { get; set; } 

        public int WaiterId { get; set; }
        public User? Waiter { get; set; } 

        public OrderStatus Status { get; set; } = OrderStatus.Abierta;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<OrderItem> Items { get; set; } = new();
    }
}