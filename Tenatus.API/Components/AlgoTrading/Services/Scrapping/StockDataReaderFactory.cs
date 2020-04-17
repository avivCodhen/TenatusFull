using Tenatus.API.Util;

namespace Tenatus.API.Components.AlgoTrading.Services.Scrapping
{
    public static class StockDataReaderFactory
    {
        public static IStockDataReader GetStockDataReader(string type, string stock)
        {
            switch (type)
            {
                case AppConstants.StockDataReaderTypeYahoo:
                    return new YahooStockDataReader(stock);
                default:
                    return new YahooStockDataReader(stock);
                
            }
        }
    }
}