using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Tenatus.API.Components.AlgoTrading.Models;
using Tenatus.API.Components.AlgoTrading.Services.TradingProviders.Traders;
using Tenatus.API.Components.Dashboard.Models;
using Tenatus.API.Data;
using Tenatus.API.Extensions;

namespace Tenatus.API.Components.Dashboard.Controllers
{
    public class DashboardController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _dbContext;
        private readonly TraderManager _traderManager;

        public DashboardController(UserManager<ApplicationUser> userManager, IMapper mapper, ApplicationDbContext dbContext, TraderManager traderManager)
        {
            _userManager = userManager;
            _mapper = mapper;
            _dbContext = dbContext;
            _traderManager = traderManager;
        }
        [HttpGet]
        public async Task<IActionResult> GetDashboard()
        {
            var user = await _userManager.GetApplicationUserAsync(User);
            var userOrders = _dbContext.UserOrders.Where(x => x.UserId == user.Id).ToList();
            var userStrategies = _dbContext.Strategies.Where(x => x.UserId == user.Id).ToList();
            var isOn = _traderManager.IsOnForUser(user);
            var response = new DashboardModel()
            {
                UserOrders = _mapper.Map<IEnumerable<UserOrder>, IEnumerable<UserOrderModel>>(userOrders),
                Strategies = _mapper.Map<IEnumerable<Strategy>, IEnumerable<StrategyModel>>(userStrategies),
                IsOn = isOn
            };
            return Ok(response);
        }
    }
}