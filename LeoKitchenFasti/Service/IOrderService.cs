using LeoKitchenFasti.Models;

namespace LeoKitchenFasti.Services
{
    public interface IOrderService
    {
        // Crear Orden
        Task<Order> CreateOrderAsync(Order nuevaOrden, int meseroId);
        //Ver ordenes 
        Task<List<Order>> GetActiveOrdersAsync();
    }
}