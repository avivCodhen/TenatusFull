using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Tenatus.API.Components.AlgoTrading.Models;
using Tenatus.API.Components.AlgoTrading.Services.TradingProviders;
using Tenatus.API.Components.AlgoTrading.Services.TradingProviders.Traders;
using Tenatus.API.Data;
using Tenatus.API.Extensions;

namespace Tenatus.API.Components.AlgoTrading.Controllers
{
    public class TraderController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TraderManager _traderManager;
        private readonly ApplicationDbContext _dbContext;

        public TraderController(UserManager<ApplicationUser> userManager, TraderManager traderManager,
            ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _traderManager = traderManager;
            _dbContext = dbContext;
        }

        [Route("start")]
        [HttpPost]
        public async Task<IActionResult> StartTrader([FromBody] StartTraderRequest request)
        {
            try
            {
                var user = await _userManager.GetApplicationUserAsync(User);
               
                await _traderManager.StartTrader(user);
                await _dbContext.SaveChangesAsync();
                
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Route("stop")]
        [HttpDelete]
        public async Task<IActionResult> StopTrader()
        {
            try
            {
                var user = await _userManager.GetApplicationUserAsync(User);
                _traderManager.StopTrader(user);

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}