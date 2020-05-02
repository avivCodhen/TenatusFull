using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Tenatus.API.Components.SignalR
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class StockDataHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            var c = Context.User;
            return base.OnConnectedAsync();
        }
    }
}