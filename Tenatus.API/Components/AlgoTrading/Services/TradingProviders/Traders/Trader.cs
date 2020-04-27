using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tenatus.API.Components.AlgoTrading.Models;
using Tenatus.API.Components.AlgoTrading.Services.Scrapping;
using Tenatus.API.Data;
using Tenatus.API.Extensions;
using Tenatus.API.Types;

namespace Tenatus.API.Components.AlgoTrading.Services.TradingProviders.Traders
{
    public abstract class Trader
    {
        protected decimal BuyingPrice = new decimal(0);

        private readonly ITradingClient _tradingClient;
        private readonly IServiceProvider _serviceProvider;
        private readonly ApplicationUser _user;
        private ApplicationDbContext _dbContext;
        private readonly ILogger _log;
        private int _quantity = 1;
        private decimal _budget = new decimal(0.0);
        private readonly IStockDataReader _stockDataReader;

        public bool IsOn = true;
        protected readonly Strategy Strategy;
        protected List<StockData> StockValues;
        protected StockData CurrentStockData;

        protected Trader(IStockDataReader stockDataReader, ITradingClient tradingClient,
            IServiceProvider serviceProvider,
            ApplicationUser user, Strategy strategy, ILogger log)
        {
            _stockDataReader = stockDataReader;
            _tradingClient = tradingClient;
            _serviceProvider = serviceProvider;
            _user = user;
            Strategy = strategy;
            _log = log;
        }

        public async Task Start()
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                _dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                StockValues = _dbContext.StocksData.Where(x => x.Stock.EqualsIgnoreCase(Strategy.Stock))
                    .OrderByDescending(x => x.Time).ToList();

                await UpdateBuyingPrice();
                await UpdateBudget();
                while (IsOn)
                {
                    CurrentStockData = _stockDataReader.ReadStockValue();
                    StockValues.Add(CurrentStockData);
                    _dbContext.StocksData.Add(CurrentStockData);
                    _dbContext.SaveChanges();
                    
                    await Invoke();
                    
                    Thread.Sleep(1000);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        private async Task UpdateBudget()
        {
            var amount = await _tradingClient.RequestBudget(Strategy.Budget);
            if (amount)
                _budget = Strategy.Budget;
        }


        protected abstract Task Invoke();

        protected async Task Buy(decimal value)
        {
            try
            {
                var quantity = (int) (_budget / value);
                if (quantity <= 0) throw new Exception($"Quantity is less or equals to zero.");

                _log.LogInformation($"Buying {_quantity} {Strategy.Stock} share(s). Price: {BuyingPrice}.");
                var orderModel = await _tradingClient.Buy(Strategy.Stock, quantity, value);

                if (orderModel == null) throw new Exception("failed to buy");

                _log.LogInformation(
                    $"BOUGHT {_quantity} {Strategy.Stock} share(s). Price: {BuyingPrice}.");
                BuyingPrice = value;
                _quantity = quantity;
                await SaveUserOrder(orderModel);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                _log.LogError(
                    $"FAILED TO BUY {_quantity} {Strategy.Stock} share(s). Price: {BuyingPrice}. Error: {e.Message}");
            }
        }

        private async Task SaveUserOrder(OrderModel orderModel)
        {
            _dbContext.UserOrders.Add(new UserOrder
            {
                UserId = _user.Id,
                BuyingPrice = orderModel.BuyingPrice,
                ExternalId = orderModel.ExternalId,
                UserOrderActionType = orderModel.UserOrderActionType,
                TradingClient = _user.TradingClientType,
                UserOrderType = orderModel.UserOrderType,
                Stock = Strategy.Stock
            });
            await _dbContext.SaveChangesAsync();
        }

        protected async Task Sell(decimal value)
        {
            _log.LogInformation($"Selling {_quantity} {Strategy.Stock} share(s). Price: {value}...");
            var orderModel = await _tradingClient.Sell(Strategy.Stock, _quantity, value);
            if (orderModel != null)
            {
                _log.LogInformation($"SOLD {_quantity} {Strategy.Stock} share(s). Price: {value}");
                BuyingPrice = 0;
                await SaveUserOrder(orderModel);
            }
            else
                _log.LogError(
                    $"FAILED TO SELL {_quantity} {Strategy.Stock} share(s). Price: {value}");
        }

        private UserOrder GetLastOrder(UserOrderActionType orderActionType)
        {
            return _dbContext.UserOrders
                .Where(x => x.ApplicationUser.Id == _user.Id && x.UserOrderActionType == orderActionType &&
                            x.TradingClient.EqualsIgnoreCase(_user.TradingClientType))
                .OrderByDescending(x => x.Created).FirstOrDefault();
        }

        private async Task UpdateBuyingPrice()
        {
            var activeOrder = await _tradingClient.ActiveOrderOrDefault(Strategy.Stock);
            if (activeOrder != null)
            {
                //await _tradingClient.CancelOrder(activeOrder.ExternalId);
                await _tradingClient.CancelAllOrders();
            }

            var position = await _tradingClient.CurrentPositionOrDefault(Strategy.Stock);
            if (position != null)
            {
                BuyingPrice = position.BuyingPrice;
                _quantity = position.Quantity;
            }
        }
        
        protected bool Profitable(decimal value)
        {
            var results = (value * _quantity) - BuyingPrice * _quantity;
            return results > _user.MinimumFee;
        }
    }
}