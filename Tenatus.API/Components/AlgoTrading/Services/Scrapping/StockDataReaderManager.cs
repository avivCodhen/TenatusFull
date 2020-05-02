using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Tenatus.API.Components.AlgoTrading.Models;
using Tenatus.API.Components.SignalR;
using Tenatus.API.Components.SignalR.Services;
using Tenatus.API.Extensions;

namespace Tenatus.API.Components.AlgoTrading.Services.Scrapping
{
    public class StockDataReaderManager
    {
        private readonly IConfiguration _configuration;
        private readonly SignalRService _signalRService;
        private List<StockDataReaderResource> _readerResources = new List<StockDataReaderResource>();

        public StockDataReaderManager(IConfiguration configuration, SignalRService signalRService)
        {
            _configuration = configuration;
            _signalRService = signalRService;
        }

        public IStockDataReader GetStockDataReader(string userId, string stock)
        {
            var stockDataReader =
                _readerResources.SingleOrDefault(x => x.Stock.EqualsIgnoreCase(stock));
            if (stockDataReader != null)
            {
                stockDataReader.Users.Add(userId);
                return stockDataReader.Reader;
            }

            var newStockDataReader = new StockDataReaderResource()
            {
                Reader = StockDataReaderFactory.GetStockDataReader(_configuration["StockDataReader"], stock, _signalRService),
                Stock = stock
            };
            newStockDataReader.Users.Add(userId);
            _readerResources.Add(newStockDataReader);
            return newStockDataReader.Reader;
        }

        public void RemoveStockDataReader(string userId, string stock)
        {
            var stockDataReader =
                _readerResources.SingleOrDefault(x => x.Stock.EqualsIgnoreCase(stock));

            if (stockDataReader == null) return;

            stockDataReader.Users.Remove(userId);

            if (stockDataReader.Users.Any()) return;

            _readerResources.Remove(stockDataReader);
            stockDataReader.Reader.Dispose();
        }
    }
}