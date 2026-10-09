using LeoKitchenFasti.Data;
using LeoKitchenFasti.Hubs;
using LeoKitchenFasti.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace LeoKitchenFasti.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<RestaurantHub> _hubContext;

        public OrderService(AppDbContext context, IHubContext<RestaurantHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public async Task<Order> CreateOrderAsync(Order nuevaOrden, int meseroId)
        {
            // 1. Completar la información de seguridad y tiempo
            nuevaOrden.WaiterId = meseroId;
            nuevaOrden.CreatedAt = DateTime.UtcNow;

            // 2. Guardar en MariaDB
            _context.Orders.Add(nuevaOrden);
            await _context.SaveChangesAsync();

            // 3. Traer la orden recién creada con todos sus detalles (Join)
            var ordenCompleta = await _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .Include(o => o.Table)
                    .ThenInclude(t => t.Area)
                .FirstOrDefaultAsync(o => o.Id == nuevaOrden.Id);

            if (ordenCompleta == null) throw new Exception("Error al registrar la orden.");

            // 4. SIGNALR: Notificar en tiempo real SOLO a la "Cocina"
            await _hubContext.Clients.Group("Cocina").SendAsync("NuevaOrdenRecibida", ordenCompleta);

            return ordenCompleta;
        }

        //Ver Ordenes 
        public async Task<List<Order>> GetActiveOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .Include(o => o.Table)
                    .ThenInclude(t => t.Area)
                .Where(o => o.Status == OrderStatus.Abierta &&
                            o.Items.Any(i => i.Status == ItemStatus.Pendiente || i.Status == ItemStatus.EnPreparacion))
                .OrderBy(o => o.CreatedAt)
                .ToListAsync();
        }
    }
}