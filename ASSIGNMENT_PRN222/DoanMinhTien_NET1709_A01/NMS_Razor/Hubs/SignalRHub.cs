using Microsoft.AspNetCore.SignalR;

namespace NMS_Razor.Hubs
{
    public class SignalRHub : Hub
    {


        public async Task NotifyChange()
        {
            await Clients.All.SendAsync("Change");
        }


    }
}
