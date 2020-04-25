using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Tenatus.API.Components.AlgoTrading.Services.TradingProviders.Alpaca;
using Tenatus.API.Components.AlgoTrading.Services.TradingProviders.InteractiveBroker;
using Tenatus.API.Data;
using Tenatus.API.EnumTypes;
using Tenatus.API.Util;

namespace Tenatus.API.Components.AlgoTrading.Services.TradingProviders
{
    public class TradingClientFactory
    {
        public ITradingClient GetTradingClient(ApplicationUser user)
        {
            return user.TradingClientType.ToLower() switch
            {
                AppConstants.TradingClientAlpaca => new AlpacaClient(user.ApiKey, user.ApiSecret),
                AppConstants.TradingClientInteractive =>
                new InteractiveBrokerTradingClient(user.AccountName),
                _ => throw new Exception($"unknown type: {user.TradingClientType}")
            };
        }
    }
}