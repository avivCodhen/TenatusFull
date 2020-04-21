using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using Serilog.Core;
using Tenatus.API.Components.AlgoTrading.Models;
using Tenatus.API.Components.AlgoTrading.Services.Scrapping;
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

        private readonly Logger _log;
        private int _quantity = 1;
        private decimal _profit = new decimal(1);

        public bool IsOn;

        public Trader(IStockDataReader stockDataReader, ITradingClient tradingClient,
            string stock, decimal buyingValue,
            decimal sellingValue)
        {
            _stockDataReader = stockDataReader;
            _tradingClient = tradingClient;
            _stock = stock;
            _buyingValue = buyingValue;
            _sellingValue = sellingValue;

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
            if (order != null && order.Buy)
                _buyingPrice = order.Price;

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
                            _log.Information($"Selling {_quantity} {_stock} share(s). Price: {value}...");
                            var results = await _tradingClient.Sell(_stock, _quantity, value);
                            if (results)
                                _log.Information($"SOLD {_quantity} {_stock} share(s). Price: {value}");
                            else
                                _log.Information(
                                    $"FAILED TO SELL {_quantity} {_stock} share(s). Price: {value}");

                            _profit = _profit * (value / _buyingPrice);
                            try
                            {
                                System.IO.File.WriteAllText($"{AppConstants.FilePath}profit.txt",
                                    _profit.ToString(CultureInfo.InvariantCulture));
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }

                            _buyingPrice = 0;
                        }
                    }
                    else
                    {
                        temp = new[] {temp, value, prevValue}.Max();
                        if (_buyingPrice == 0 && value / temp <= _buyingValue)
                        {
                            _log.Information($"Buying {_quantity} {_stock} share(s). Price: {_buyingPrice}.");
                            _buyingPrice = value;
                            var results = await _tradingClient.Buy(_stock, _quantity, _buyingPrice);
                            if (results)
                                _log.Information(
                                    $"BOUGHT {_quantity} {_stock} share(s). Price: {_buyingPrice}. results is {results}.");
                            else
                                _log.Information(
                                    $"FAILED TO BUY {_quantity} {_stock} share(s). Price: {_buyingPrice}.");
                        }
                    }
                }

                stocks.Add(stockData);
                index++;
            }
        }
    }
}