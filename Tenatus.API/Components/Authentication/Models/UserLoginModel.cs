using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Tenatus.API.Components.Authentication.Models
{
    public class UserLoginModel
    {
        public string Password { get; set; }
        public string Email { get; set; }
    }
}