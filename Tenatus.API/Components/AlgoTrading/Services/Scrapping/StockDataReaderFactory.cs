using Microsoft.AspNetCore.SignalR;
using Tenatus.API.Components.SignalR;
using Tenatus.API.Components.SignalR.Services;
using Tenatus.API.Data;
using Tenatus.API.Util;

namespace Tenatus.API.Components.AlgoTrading.Services.Scrapping
{
    public static class StockDataReaderFactory
    {
        
        public static IStockDataReader GetStockDataReader(string type, string stock, SignalRService signalRService)
        {
            switch (type)
            {
                case AppConstants.StockDataReaderTypeYahoo:
                    return new YahooStockDataReader(stock, signalRService);
                default:
                    return new YahooStockDataReader(stock, signalRService);
                
            }
        }
    }
}