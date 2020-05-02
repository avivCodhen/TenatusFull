using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Tenatus.API.Data;
using Tenatus.API.Extensions;

namespace Tenatus.API.Components.SignalR.Services
{
    public class SignalRService
    {
        private readonly IHubContext<StockDataHub> _hubContext;

        public SignalRService(IHubContext<StockDataHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendMessageToUser(string userId,string channel, object obj)
        {
            await _hubContext.Clients.User(userId).SendAsync(channel, obj);
        }
        public async Task SendMessageToAll(string channel, object obj)
        {
            await _hubContext.Clients.All.SendAsync(channel, obj);
        }
    }
}