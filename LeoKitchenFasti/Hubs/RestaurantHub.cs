using Microsoft.AspNetCore.SignalR;

namespace LeoKitchenFasti.Hubs
{
    public class RestaurantHub : Hub
    {
        // Cuando la tablet de la cocina abra la app, llamará a este método
        public async Task UnirseACocina()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Cocina");
        }

        // Cuando la tablet del mesero abra la app, llamará a este método
        public async Task UnirseAMeseros()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Meseros");
        }
    }
}