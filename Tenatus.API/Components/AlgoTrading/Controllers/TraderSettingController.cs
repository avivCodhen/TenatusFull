using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Tenatus.API.Components.AlgoTrading.Models;
using Tenatus.API.Components.AlgoTrading.Services.TradingProviders;
using Tenatus.API.Data;
using Tenatus.API.Extensions;

namespace Tenatus.API.Components.AlgoTrading.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TraderSettingController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _dbContext;
        private readonly TraderManager _traderManager;

        public TraderSettingController(UserManager<ApplicationUser> userManager, IMapper mapper,
            ApplicationDbContext dbContext, TraderManager traderManager)
        {
            _userManager = userManager;
            _mapper = mapper;
            _dbContext = dbContext;
            _traderManager = traderManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetTraderSetting()
        {
            try
            {
                var user = await _userManager.GetApplicationUserAsync(User);

                var response = _mapper.Map<TraderSetting, TraderSettingModel>(user.TraderSetting);
                response.IsOn = _traderManager.IsOnForUser(user);
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok();
        }
    }
}