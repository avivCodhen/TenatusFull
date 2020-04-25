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
        private readonly IServiceProvider _serviceProvider;

        public TradingClientFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ITradingClient GetTradingClient(ApplicationUser user)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var lastOrder = dbContext.UserOrders.Where(x =>
                    x.ApplicationUser.Id == user.Id && x.UserOrderActionType == UserOrderActionType.Buy &&
                    x.TradingClient == "Interactive")
                .OrderByDescending(x => x.Created).FirstOrDefault();

            return user.TradingClientType.ToLower() switch
            {
                AppConstants.TradingClientAlpaca => new AlpacaClient(user.ApiKey, user.ApiSecret),
                AppConstants.TradingClientInteractive =>
                new InteractiveBrookerTradingClient(user.AccountName, lastOrder?.ExternalId),
                _ => throw new Exception($"unknown type: {user.TradingClientType}")
            };
        }
    }
}