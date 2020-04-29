using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Tenatus.API.Components.AlgoTrading.Models;
using Tenatus.API.Components.AlgoTrading.Services.Scrapping;
using Tenatus.API.Data;

namespace Tenatus.API.Components.AlgoTrading.Services.TradingProviders.Traders
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

        public Task StartTrader(ApplicationUser user)
        {
            try
            {
                if (IsOnForUser(user))
                    throw new Exception("Trader has already started");
                
                if(user.Strategies.Any(x=>x.Active))
                    throw new Exception("No active strategies available.");
                
                var tasks = new List<Task>();
                foreach (var strategy in user.Strategies.Where(x=>x.Active))
                {
                    var trader =
                        _traderResources.SingleOrDefault(x => x.Strategy.Id == strategy.Id);

                    if (trader != null)
                    {
                        if (trader.Trader.IsOn) continue;

                        trader.Trader.IsOn = true;
                        tasks.Add(Task.Run(() => trader.Trader.Start()));
                    }
                    else
                    {
                        var newTrader = GetTrader(strategy, user);
                        _traderResources.Add(new TraderResource()
                        {
                            Trader = newTrader,
                            UserId = user.Id
                        });
                        tasks.Add(Task.Run(() => newTrader.Start()));
                        strategy.LastActive = DateTimeOffset.Now;
                    }
                }

                Task.WhenAll(tasks);
            }
            catch (Exception e)
            {
                foreach (var strategy in user.Strategies)
                {
                    _stockDataReaderManager.RemoveStockDataReader(user.Id, strategy.Stock);
                }

                StopTrader(user);
                Console.WriteLine($"Error: {e.Message}");
                throw;
            }
            return Task.CompletedTask;
        }

        private Trader GetTrader(Strategy strategy, ApplicationUser user)
        {
            var stockDataReader = _stockDataReaderManager.GetStockDataReader(user.Id, strategy.Stock);
            var tradingClient = _tradingClientFactory.GetTradingClient(user);
            switch (strategy)
            {
                case RangeStrategy _:
                    return new RangeTrader(stockDataReader, tradingClient, _serviceProvider, user, strategy, _logger);
                case PercentStrategy _:
                    return new PercentTrader(stockDataReader, tradingClient, _serviceProvider, user, strategy, _logger);
                default:
                    throw new Exception($"unknown exception: {strategy.GetType()}");
            }
        }

        public void StopTrader(ApplicationUser user)
        {
            foreach (var strategy in user.Strategies)
            {
                var trader =
                    _traderResources.SingleOrDefault(x => strategy.Id == x.Strategy.Id);

                if (trader == null) return;

                trader.Trader.IsOn = false;
                _traderResources.Remove(trader);
                _stockDataReaderManager.RemoveStockDataReader(user.Id, strategy.Stock);
            }
        }

        public bool IsOnForUser(ApplicationUser user)
        {
            var traders =
                user.Strategies.Select(x => x.Id).Intersect(_traderResources.Select(x => x.Strategy.Id)).Any();
            return _traderResources.Any() && traders;
        }
    }
}