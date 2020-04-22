using System;
using Tenatus.API.Components.AlgoTrading.Services.TradingProviders.Alpaca;
using Tenatus.API.Components.AlgoTrading.Services.TradingProviders.InteractiveBroker;
using Tenatus.API.Util;

namespace Tenatus.API.Components.AlgoTrading.Services.TradingProviders
{
    public static class TradingClientFactory
    {
        public static ITradingClient GetTradingClient(string type, string apiKey, string apiSecret, string accountName)
        {
            return type.ToLower() switch
            {
                AppConstants.Alpaca => new AlpacaClient(apiKey, apiSecret),
                AppConstants.Interactive => new InteractiveBrookerTradingClient(accountName),
                _ => throw new Exception($"unknown type: {type}")
            };
        }
    }
}