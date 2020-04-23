using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Tenatus.API.Components.AlgoTrading.Models;
using Tenatus.API.Extensions;

namespace Tenatus.API.Components.AlgoTrading.Services.Scrapping
{
    public class StockDataReaderManager
    {
        private readonly IConfiguration _configuration;
        private List<StockDataReaderResource> _readerResources = new List<StockDataReaderResource>();

        public StockDataReaderManager(IConfiguration configuration)
        {
            _configuration = configuration;
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
                Reader = StockDataReaderFactory.GetStockDataReader(_configuration["StockDataReader"], stock),
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