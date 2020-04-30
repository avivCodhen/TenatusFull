using Microsoft.AspNetCore.SignalR;
using Tenatus.API.Components.SignalR;
using Tenatus.API.Data;
using Tenatus.API.Util;

namespace Tenatus.API.Components.AlgoTrading.Services.Scrapping
{
    public static class StockDataReaderFactory
    {
        public static IStockDataReader GetStockDataReader(string type, string stock, IHubContext<StockDataHub> hubContext)
        {
            switch (type)
            {
                case AppConstants.StockDataReaderTypeYahoo:
                    return new YahooStockDataReader(stock, hubContext);
                default:
                    return new YahooStockDataReader(stock, hubContext);
                
            }
        }
    }
}