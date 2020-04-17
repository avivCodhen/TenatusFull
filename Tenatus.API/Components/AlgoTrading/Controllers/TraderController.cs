using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Tenatus.API.Components.AlgoTrading.Services.TradingProviders;
using Tenatus.API.Data;
using Tenatus.API.Extensions;

namespace Tenatus.API.Components.AlgoTrading.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TraderController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TraderManager _traderManager;

        public TraderController(UserManager<ApplicationUser> userManager, TraderManager traderManager)
        {
            _userManager = userManager;
            _traderManager = traderManager;
        }

        [Route("start")]
        [HttpPost]
        public async Task<IActionResult> StartTrader([FromBody] StartTraderRequest request)
        {
            try
            {
                var user = await _userManager.GetApplicationUserAsync(User);
                await _traderManager.StartTrader(user, request.Stocks);

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Route("stop")]
        [HttpPost]
        public async Task<IActionResult> StopTrader([FromBody] StartTraderRequest request)
        {
            try
            {
                var user = await _userManager.GetApplicationUserAsync(User);
                _traderManager.StopTrader(user, request.Stocks);

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}