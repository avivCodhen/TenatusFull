using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using Serilog.Core;
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
        private readonly ApplicationDbContext _dbContext;
        private readonly ApplicationUser _user;

        private readonly Logger _log;
        private int _quantity = 1;
        private decimal _profit = new decimal(1);

        public bool IsOn;

        public Trader(IStockDataReader stockDataReader, ITradingClient tradingClient, ApplicationDbContext dbContext,
            ApplicationUser user, string stock)
        {
            _stockDataReader = stockDataReader;
            _tradingClient = tradingClient;
            _dbContext = dbContext;
            _user = user;
            _stock = stock;
            _buyingValue = user.TraderSetting.BuyingValue;
            _sellingValue = user.TraderSetting.SellingValue;

            try
            {
                _profit = Convert.ToDecimal(System.IO.File.ReadAllText("{AppConstants.FilePath}profit.txt"));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            _log = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File($"{AppConstants.FilePath}log.txt")
                .CreateLogger();
        }

        public async Task Start()
        {
            var stocks = new List<StockData>();
            var temp = new decimal(0);

            var index = 0;

            var order = await _tradingClient.LastOrderStatusOrDefault(_stock);
            if (order != null && order.UserOrderActionType == UserOrderActionType.Buy)
                _buyingPrice = order.BuyingPrice;

            var position = await _tradingClient.GetCurrentPositionOrDefault(_stock);
            if (position != null)
            {
                _quantity = position.Quantity;
                _buyingPrice = position.BuyingPrice;
            }

            while (true)
            {
                var stockData = _stockDataReader.ReadStockValue();
                var value = Convert.ToDecimal(stockData.Value);

                if (index > 0)
                {
                    var prevValue = Convert.ToDecimal(stocks[index - 1].Value);

                    var trend = value > prevValue;

                    if (trend)
                    {
                        temp = 0;
                        if (_buyingPrice != 0 && value / _buyingPrice >= _sellingValue)
                        {
                            await Sell(value);
                        }
                    }
                    else
                    {
                        temp = new[] {temp, value, prevValue}.Max();
                        if (_buyingPrice == 0 && value / temp <= _buyingValue)
                        {
                            await Buy(_buyingPrice);
                            _buyingPrice = value;
                        }
                    }
                }

                stocks.Add(stockData);
                index++;
            }
        }

        private async Task Buy(decimal value)
        {
            var lastBuyOrder = GetLastOrder(UserOrderActionType.Buy);
            var newOrderId = Convert.ToInt16(lastBuyOrder.ExternalId);
            newOrderId++;
            _log.Information($"Buying {_quantity} {_stock} share(s). Price: {_buyingPrice}.");
            var orderModel = await _tradingClient.Buy(newOrderId.ToString(), _stock, _quantity, _buyingPrice);
            if (orderModel != null)
            {
                _log.Information(
                    $"BOUGHT {_quantity} {_stock} share(s). Price: {_buyingPrice}.");
                _buyingPrice = value;
                await SaveUserOrder(orderModel);
            }
            else
                _log.Information(
                    $"FAILED TO BUY {_quantity} {_stock} share(s). Price: {_buyingPrice}.");
        }

        private async Task SaveUserOrder(OrderModel orderModel)
        {
            _dbContext.UserOrders.Add(new UserOrder
            {
                UserId = _user.Id,
                BuyingPrice = orderModel.BuyingPrice,
                ExternalId = orderModel.ExternalId,
                UserOrderActionType = orderModel.UserOrderActionType,
                TradingClient = "Interactive",
                UserOrderType = orderModel.UserOrderType
            });
            await _dbContext.SaveChangesAsync();
        }

        private async Task Sell(decimal value)
        {
            var lastBuyOrder = GetLastOrder(UserOrderActionType.Buy);
            _log.Information($"Selling {_quantity} {_stock} share(s). Price: {value}...");
            var orderModel = await _tradingClient.Sell(lastBuyOrder.ExternalId, _stock, _quantity, value);
            if (orderModel != null)
            {
                _log.Information($"SOLD {_quantity} {_stock} share(s). Price: {value}");
                _buyingPrice = 0;
                await SaveUserOrder(orderModel);
            }
            else
                _log.Information(
                    $"FAILED TO SELL {_quantity} {_stock} share(s). Price: {value}");
        }

        private UserOrder GetLastOrder(UserOrderActionType orderActionType)
        {
            return _dbContext.UserOrders
                .Where(x => x.ApplicationUser.Id == _user.Id && x.UserOrderActionType == orderActionType &&
                            x.TradingClient == "Interactive")
                .OrderByDescending(x => x.Created).First();
        }
    }
}