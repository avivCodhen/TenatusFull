using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IBApi;
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


        public async Task StartTrader(ApplicationUser user)
        {
            try
            {
                var traderClient = TradingClientFactory.GetTradingClient(_configuration["TradingClient"],
                    user.ApiKey, user.ApiSecret);
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

                        var newTrader = new Trader(stockDataReader, traderClient, stock.Name,
                            user.TraderSetting.BuyingValue, user.TraderSetting.SellingValue);
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
            }
        }
    }
}