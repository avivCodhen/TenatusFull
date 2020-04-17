using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
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
        private List<TraderResource> _traderResources = new List<TraderResource>();

        public TraderManager(StockDataReaderManager stockDataReaderManager, IConfiguration configuration)
        {
            _stockDataReaderManager = stockDataReaderManager;
            _configuration = configuration;
        }


        public async Task StartTrader(ApplicationUser user, List<string> stocks)
        {
            try
            {
                var tasks = new List<Task>();
                foreach (var stock in stocks)
                {
                    var trader =
                        _traderResources.SingleOrDefault(x => x.Stock.EqualsIgnoreCase(stock) && x.UserId == user.Id);

                    if (trader != null)
                    {
                        if (trader.Trader.IsOn) continue;

                        trader.Trader.IsOn = true;
                        tasks.Add(Task.Run(() => trader.Trader.Start()));
                    }
                    else
                    {
                        var stockDataReader =
                            _stockDataReaderManager.GetStockDataReader(user.Id, stock);
                        var traderClient = TradingClientFactory.GetTradingClient(_configuration["TradingClient"]);
                        var newTrader = new Trader(stockDataReader, traderClient, stock);
                        _traderResources.Add(new TraderResource()
                        {
                            Stock = stock,
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
                foreach (var stock in stocks)
                {
                    _stockDataReaderManager.RemoveStockDataReader(user.Id, stock);
                }

                StopTrader(user, stocks);
                throw;
            }

          
        }

        public void StopTrader(ApplicationUser user, List<string> stocks)
        {
            foreach (var stock in stocks)
            {
                var trader =
                    _traderResources.SingleOrDefault(x => x.Stock.EqualsIgnoreCase(stock) && x.UserId == user.Id);

                if (trader == null) return;

                trader.Trader.IsOn = false;
                _traderResources.Remove(trader);
            }
        }
    }
}