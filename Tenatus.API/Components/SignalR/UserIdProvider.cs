using System;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace Tenatus.API.Components.SignalR
{
    public class UserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            return connection.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}