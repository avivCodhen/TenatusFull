using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Tenatus.API.Components.AlgoTrading.Models;
using Tenatus.API.Components.AlgoTrading.Services.TradingProviders;
using Tenatus.API.Components.AlgoTrading.Services.TradingProviders.Traders;
using Tenatus.API.Components.SignalR.Models;
using Tenatus.API.Components.SignalR.Services;
using Tenatus.API.Data;
using Tenatus.API.Extensions;
using Tenatus.API.Util;

namespace Tenatus.API.Components.AlgoTrading.Controllers
{
    public class TraderController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TraderManager _traderManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly SignalRService _signalRService;

        public TraderController(UserManager<ApplicationUser> userManager, TraderManager traderManager,
            ApplicationDbContext dbContext, SignalRService signalRService)
        {
            _userManager = userManager;
            _traderManager = traderManager;
            _dbContext = dbContext;
            _signalRService = signalRService;
        }

        [Route("start")]
        [HttpPost]
        public async Task<IActionResult> StartTrader([FromBody] StartTraderRequest request)
        {
            try
            {
                var user = await _userManager.GetApplicationUserAsync(User);
                user.IsTraderOn = true;
                var marketOpen = MarketHelper.IsMarketOpen();
                //if (marketOpen)
                    await _traderManager.ManageTrader(user);
                await _dbContext.SaveChangesAsync();

                return Ok(new {marketOpen, user.IsTraderOn});
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
                user.IsTraderOn = false;
                _traderManager.StopTrader(user);
                await _dbContext.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}