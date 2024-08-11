namespace Services.API.Gateway
{
    using Microsoft.AspNetCore.SignalR;
    using System.Threading.Tasks;

    public class NotificationHub : Hub
    {
        public async Task SendNotification(string notification)
        {
            if (Clients != null && Clients.All != null)
            {
                await Clients.All.SendAsync("ReceiveNotification", notification);
            }
            else
            {
                Console.WriteLine("No hay clientes conectados para enviar la notificación.");
            }
        }
    }

}
