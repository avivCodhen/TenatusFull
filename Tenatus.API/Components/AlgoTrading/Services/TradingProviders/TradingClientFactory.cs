using Samples;
using Tenatus.API.Util;

namespace Tenatus.API.Components.AlgoTrading.Services.TradingProviders
{
    public static class TradingClientFactory
    {
        public static ITradingClient GetTradingClient(string type, string apiKey, string apiSecret)
        {
            return type switch
            {
                AppConstants.Alpaca => new AlpacaClient(apiKey, apiSecret),
                AppConstants.Interactive => new InteractiveBrookerTradingClient(),
                _ => new AlpacaClient()
            };
        }
    }
}