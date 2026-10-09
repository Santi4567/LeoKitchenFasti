using LeoKitchenFasti.Models;

namespace LeoKitchenFasti.Services
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(Order nuevaOrden, int meseroId);
    }
}