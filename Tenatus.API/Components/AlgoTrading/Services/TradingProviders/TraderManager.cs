using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IBApi;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Tenatus.API.Components.AlgoTrading.Models;
using Tenatus.API.Components.AlgoTrading.Services.Scrapping;
using Tenatus.API.Data;
using Tenatus.API.Extensions;

namespace Tenatus.API.Components.AlgoTrading.Services.TradingProviders
{
    public class TraderManager
    {
        private readonly StockDataReaderManager _stockDataReaderManager;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;
        private readonly TradingClientFactory _tradingClientFactory;
        private readonly ILogger<TraderManager> _logger;
        private List<TraderResource> _traderResources = new List<TraderResource>();

        public TraderManager(StockDataReaderManager stockDataReaderManager, IConfiguration configuration,
            IServiceProvider serviceProvider, TradingClientFactory tradingClientFactory,
            ILogger<TraderManager> logger)
        {
            _stockDataReaderManager = stockDataReaderManager;
            _configuration = configuration;
            _serviceProvider = serviceProvider;
            _tradingClientFactory = tradingClientFactory;
            _logger = logger;
        }

        public async Task StartTrader(ApplicationUser user)
        {
            try
            {
                if (IsOnForUser(user))
                    throw new Exception("Trader has already started");

                var traderClient = _tradingClientFactory.GetTradingClient(user);
                var tasks = new List<Task>();
                foreach (var stock in user.TraderSetting.Stocks)
                {
                    var trader =
                        _traderResources.SingleOrDefault(x =>
                            x.Stock.EqualsIgnoreCase(stock.Name) && x.UserId == user.Id);

                    if (trader != null)
                    {
                        if (trader.Trader.IsOn) continue;

                        trader.Trader.IsOn = true;
                        tasks.Add(Task.Run(() => trader.Trader.Start()));
                    }
                    else
                    {
                        var stockDataReader =
                            _stockDataReaderManager.GetStockDataReader(user.Id, stock.Name);

                        var newTrader = new Trader(stockDataReader, traderClient, _serviceProvider, user, stock.Name, _logger);
                        _traderResources.Add(new TraderResource()
                        {
                            Stock = stock.Name,
                            Trader = newTrader,
                            UserId = user.Id
                        });
                        tasks.Add(Task.Run(() => newTrader.Start()));
                    }
                }

                Task.WhenAll(tasks);
            }
            catch (Exception e)
            {
                foreach (var stock in user.TraderSetting.Stocks)
                {
                    _stockDataReaderManager.RemoveStockDataReader(user.Id, stock.Name);
                }

                StopTrader(user);
                Console.WriteLine($"Error: {e.Message}");
                throw;
            }
        }

        public void StopTrader(ApplicationUser user)
        {
            foreach (var stock in user.TraderSetting.Stocks)
            {
                var trader =
                    _traderResources.SingleOrDefault(x => x.Stock.EqualsIgnoreCase(stock.Name) && x.UserId == user.Id);

                if (trader == null) return;

                trader.Trader.IsOn = false;
                _traderResources.Remove(trader);
                _stockDataReaderManager.RemoveStockDataReader(user.Id, stock.Name);
            }
        }

        public bool IsOnForUser(ApplicationUser user)
        {
            var ts = user.TraderSetting;
            var traders =
                _traderResources.Where(x => ts.Stocks.Select(s => s.Name).Contains(x.Stock) && x.UserId == user.Id)
                    .ToList();
            return _traderResources.Any() && traders.Any();
        }
    }
}