using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Tenatus.API.Components.AlgoTrading.Services.Scrapping;
using Tenatus.API.Components.SignalR;
using Tenatus.API.Data;

namespace Tenatus.API.Components.AlgoTrading.Services.TradingProviders.Traders
{
    public class HftTrader : Trader
    {
        private decimal _temp = new decimal(0);

        public HftTrader(IStockDataReader stockDataReader, ITradingClient tradingClient,
            IServiceProvider serviceProvider,
            ApplicationUser user, Strategy strategy, ILogger log, IHubContext<StockDataHub> hubContext) : base(stockDataReader, tradingClient, serviceProvider,
            user, strategy, log, hubContext)
        {
        }

        protected override Task Invoke()
        {
            while (IsOn)
            {
               
                /*var stocks = new List<StockData>();

                var stockData = StockDataReader.ReadStockValue();
                var value = Convert.ToDecimal(stockData.CurrentPrice);
                Console.WriteLine($"Read value {Stock}: {value}");
                var prevValue = stocks.LastOrDefault()?.CurrentPrice;
                if (prevValue == null) continue;

                var prevDecimalValue = Convert.ToDecimal(prevValue);


                var trend = value > prevDecimalValue;

                if (trend)
                {
                    _temp = 0;
                    if (BuyingPrice != 0 && value / BuyingPrice >= _sellingValue)
                    {
                        await Sell(value);
                    }
                }
                else
                {
                    _temp = new[] {_temp, value, prevDecimalValue}.Max();
                    if (BuyingPrice == 0 && value / _temp <= _buyingValue)
                    {
                        await Buy(value);
                        BuyingPrice = value;
                    }
                }

                Thread.Sleep(1000);
                stocks.Add(stockData); */
            }

            return Task.CompletedTask;
        }
    }
}