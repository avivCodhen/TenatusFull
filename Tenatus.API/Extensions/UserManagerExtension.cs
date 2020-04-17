﻿using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Tenatus.API.Data;

namespace Tenatus.API.Extensions
{
    public static class UserManagerExtension
    {
        public static async Task<ApplicationUser> GetApplicationUserAsync(this UserManager<ApplicationUser> userManager, ClaimsPrincipal user)
        {
            return await userManager.FindByEmailAsync(user.FindFirst(ClaimTypes.Email)?.Value);
        }
    }
}