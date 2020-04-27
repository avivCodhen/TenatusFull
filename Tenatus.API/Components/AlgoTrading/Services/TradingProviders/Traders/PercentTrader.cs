using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tenatus.API.Components.AlgoTrading.Models;
using Tenatus.API.Components.AlgoTrading.Services.Scrapping;
using Tenatus.API.Data;

namespace Tenatus.API.Components.AlgoTrading.Services.TradingProviders.Traders
{
    public class PercentTrader : Trader
    {
        decimal roofValue = new decimal(0.0);

        public PercentTrader(IStockDataReader stockDataReader, ITradingClient tradingClient,
            IServiceProvider serviceProvider, ApplicationUser user, Strategy strategy, ILogger log)
            : base(stockDataReader, tradingClient, serviceProvider, user, strategy, log)
        {
        }

        protected override async Task Invoke()
        {
            var strategy = (PercentStrategy) Strategy;

            var value = CurrentStockData.CurrentPrice;
            var computedValue = Convert.ToDecimal(value) * strategy.Percent;
            roofValue = StockValues.Where(x => x.CurrentPrice >= computedValue).Min(x => x.CurrentPrice);
            if (BuyingPrice == 0 && roofValue > 0)
            {
                await Buy(value);
            }

            var computedBuyingPrice = Convert.ToDecimal(BuyingPrice) * strategy.Percent;
            if (computedBuyingPrice <= value && Profitable(value))
            {
                await Sell(value);
            }
        }

    }
}