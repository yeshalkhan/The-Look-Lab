using Microsoft.AspNetCore.SignalR; 

namespace Infrastructure.Hubs
{
    public class InventoryHub : Hub
    {
        public async Task UpdateInventory(int productId, int newStock)
        {
            await Clients.All.SendAsync("ReceiveInventoryUpdate", productId, newStock);
        }
    }
}
