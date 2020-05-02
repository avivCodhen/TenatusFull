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
using Tenatus.API.Components.SignalR.Models;
using Tenatus.API.Data;
using Tenatus.API.Extensions;
using Tenatus.API.Util;

namespace Tenatus.API.Components.Dashboard.Controllers
{
    public class DashboardController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _dbContext;

        public DashboardController(UserManager<ApplicationUser> userManager, IMapper mapper, ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _mapper = mapper;
            _dbContext = dbContext;
        }
        [HttpGet]
        public async Task<IActionResult> GetDashboard()
        {
            var user = await _userManager.GetApplicationUserAsync(User);
            var userOrders = _dbContext.UserOrders.Where(x => x.UserId == user.Id).OrderByDescending(x=>x.Created).ToList();
            var userStrategies = _dbContext.Strategies.Where(x => x.UserId == user.Id).ToList();
            var isTraderOn = user.IsTraderOn;
            
            var stocks = user.Strategies.Select(x => x.Stock.ToUpper());
            var userStocks = GetUserStocks(stocks);
            var response = new DashboardModel()
            {
                UserOrders = _mapper.Map<IEnumerable<UserOrder>, IEnumerable<UserOrderModel>>(userOrders),
                Strategies = _mapper.Map<IEnumerable<Strategy>, IEnumerable<StrategyModel>>(userStrategies),
                IsTraderOn = isTraderOn,
                Stocks = userStocks,
                MarketOpen = MarketHelper.IsMarketOpen()
            };
            return Ok(response);
        }

        private IEnumerable<StockDataModel> GetUserStocks(IEnumerable<string> stocks)
        {
            var stockDatas = new List<StockDataModel>();
            foreach (var stock in stocks)
            {
                var data = _dbContext.StocksData
                    .OrderByDescending(x => x.Time).FirstOrDefault(x => x.Stock.ToUpper() == stock);
                if(data != null)
                    stockDatas.Add(_mapper.Map<StockData, StockDataModel>(data));
            }

            return stockDatas;
        }
    }
}