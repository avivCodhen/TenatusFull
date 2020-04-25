using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tenatus.API.Components.AlgoTrading.Models;
using Tenatus.API.Components.AlgoTrading.Services.Scrapping;
using Tenatus.API.Data;
using Tenatus.API.EnumTypes;
using Tenatus.API.Util;

namespace Tenatus.API.Components.AlgoTrading.Services.TradingProviders
{
    public class Trader
    {
        private readonly decimal _buyingValue;
        private readonly decimal _sellingValue;
        private decimal _buyingPrice = new decimal(0);

        private readonly string _stock;
        private readonly IStockDataReader _stockDataReader;
        private readonly ITradingClient _tradingClient;
        private readonly IServiceProvider _serviceProvider;
        private readonly ApplicationUser _user;

        private readonly ILogger _log;
        private int _quantity = 1;

        public bool IsOn = true;

        public Trader(IStockDataReader stockDataReader, ITradingClient tradingClient, IServiceProvider serviceProvider,
            ApplicationUser user, string stock, ILogger log)
        {
            _stockDataReader = stockDataReader;
            _tradingClient = tradingClient;
            _serviceProvider = serviceProvider;
            _user = user;
            _stock = stock;
            _log = log;
            _buyingValue = user.TraderSetting.BuyingValue;
            _sellingValue = user.TraderSetting.SellingValue;
        }

        public async Task Start()
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var stocks = new List<StockData>();
                var temp = new decimal(0);

                var index = 0;

                var order = await _tradingClient.LastOrderOrDefault(_stock);
                if (order != null && order.UserOrderActionType == UserOrderActionType.Buy)
                    _buyingPrice = order.BuyingPrice;

                var position = await _tradingClient.GetCurrentPositionOrDefault(_stock);
                if (position != null)
                {
                    _quantity = position.Quantity;
                    _buyingPrice = position.BuyingPrice;
                }

                while (IsOn)
                {
                    var stockData = _stockDataReader.ReadStockValue();
                    var value = Convert.ToDecimal(stockData.Value);
                    Console.WriteLine($"Read value {_stock}: {value}");
                    if (index > 0)
                    {
                        var prevValue = Convert.ToDecimal(stocks[index - 1].Value);

                        var trend = value > prevValue;

                        if (trend)
                        {
                            temp = 0;
                            if (_buyingPrice != 0 && value / _buyingPrice >= _sellingValue)
                            {
                                await Sell(value, dbContext);
                            }
                        }
                        else
                        {
                            temp = new[] {temp, value, prevValue}.Max();
                            if (_buyingPrice == 0 && value / temp <= _buyingValue)
                            {
                                await Buy(value, dbContext);
                                _buyingPrice = value;
                            }
                        }
                    }

                    Thread.Sleep(1000);
                    stocks.Add(stockData);
                    index++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        private async Task Buy(decimal value, ApplicationDbContext dbContext)
        {
            try
            {
                _log.LogInformation($"Buying {_quantity} {_stock} share(s). Price: {_buyingPrice}.");
                var orderModel = await _tradingClient.Buy(_stock, _quantity, value);

                if (orderModel == null) throw new Exception("failed to buy");

                _log.LogInformation(
                    $"BOUGHT {_quantity} {_stock} share(s). Price: {_buyingPrice}.");
                _buyingPrice = value;
                await SaveUserOrder(orderModel, dbContext);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                _log.LogError(
                    $"FAILED TO BUY {_quantity} {_stock} share(s). Price: {_buyingPrice}. Error: {e.Message}");
            }
        }

        private async Task SaveUserOrder(OrderModel orderModel, ApplicationDbContext dbContext)
        {
            dbContext.UserOrders.Add(new UserOrder
            {
                UserId = _user.Id,
                BuyingPrice = orderModel.BuyingPrice,
                ExternalId = orderModel.ExternalId,
                UserOrderActionType = orderModel.UserOrderActionType,
                TradingClient = "Interactive",
                UserOrderType = orderModel.UserOrderType
            });
            await dbContext.SaveChangesAsync();
        }

        private async Task Sell(decimal value, ApplicationDbContext dbContext)
        {
            _log.LogInformation($"Selling {_quantity} {_stock} share(s). Price: {value}...");
            var orderModel = await _tradingClient.Sell(_stock, _quantity, value);
            if (orderModel != null)
            {
                _log.LogInformation($"SOLD {_quantity} {_stock} share(s). Price: {value}");
                _buyingPrice = 0;
                await SaveUserOrder(orderModel, dbContext);
            }
            else
                _log.LogError(
                    $"FAILED TO SELL {_quantity} {_stock} share(s). Price: {value}");
        }

        private UserOrder GetLastOrder(UserOrderActionType orderActionType, ApplicationDbContext dbContext)
        {
            return dbContext.UserOrders
                .Where(x => x.ApplicationUser.Id == _user.Id && x.UserOrderActionType == orderActionType &&
                            x.TradingClient == "Interactive")
                .OrderByDescending(x => x.Created).FirstOrDefault();
        }
    }
}