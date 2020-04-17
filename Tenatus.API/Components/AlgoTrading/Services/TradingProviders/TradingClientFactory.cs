using Tenatus.API.Util;

namespace Tenatus.API.Components.AlgoTrading.Services.TradingProviders
{
    public class TradingClientFactory
    {
        public static ITradingClient GetTradingClient(string type)
        {
            return type switch
            {
                AppConstants.Alpaca => new AlpacaClient(),
                _ => new AlpacaClient()
            };
        }
    }
}