using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Tenatus.API.Components.AlgoTrading.Services.Scrapping;
using Tenatus.API.Components.SignalR;
using Tenatus.API.Data;

namespace Tenatus.API.Components.AlgoTrading.Services.TradingProviders.Traders
{
    public class RangeTrader : Trader
    {
        public RangeTrader(IStockDataReader stockDataReader, ITradingClient tradingClient,
            IServiceProvider serviceProvider, ApplicationUser user, Strategy strategy, ILogger log, IHubContext<StockDataHub> hubContext)
            : base(stockDataReader, tradingClient, serviceProvider, user, strategy, log, hubContext)
        {
        }

        protected override async Task Invoke()
        {
            var strategy = (RangeStrategy) Strategy;

            var value = CurrentStockData.CurrentPrice;
            if (BuyingPrice == 0 && value <= strategy.Minimum)
            {
                await Buy(value);
            }

            if (value >= BuyingPrice && value >= strategy.Maximum && Profitable(value))
            {
                await Sell(Convert.ToDecimal(value));
            }
        }
    }
}