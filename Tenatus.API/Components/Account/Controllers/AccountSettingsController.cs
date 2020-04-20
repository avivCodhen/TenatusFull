using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tenatus.API.Components.Account.Models;
using Tenatus.API.Data;
using Tenatus.API.Extensions;

namespace Tenatus.API.Components.Account.Controllers
{
    
    public class AccountSettingsController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _dbDbContext;

        public AccountSettingsController(UserManager<ApplicationUser> userManager, ApplicationDbContext dbDbContext)
        {
            _userManager = userManager;
            _dbDbContext = dbDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAccountSettings()
        {
            try
            {
                var user = await _userManager.GetApplicationUserAsync(User);
                return Ok(new UserAccountSettingsModel
                {
                    ApiKey = user.ApiKey,
                    ApiSecret = user.ApiSecret,
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveAccountSettings([FromBody] UserAccountSettingsModel request)
        {
            try
            {
                var user = await _userManager.GetApplicationUserAsync(User);
                user.ApiKey = request.ApiKey;
                user.ApiSecret = request.ApiSecret;
                await _dbDbContext.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}