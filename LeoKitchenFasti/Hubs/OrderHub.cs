using Microsoft.AspNetCore.SignalR;

namespace LeoKitchenFasti.Hubs
{
    // El "Hub" es el punto donde se conectan todas las pantallas (meseros y cocina)
    public class OrderHub : Hub
    {
        // Los clientes (React/Tauri) pueden llamar a este método al abrir la app
        // para unirse al grupo exclusivo de la cocina y recibir las notificaciones.
        public async Task JoinKitchen()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Cocina");
        }
    }
}