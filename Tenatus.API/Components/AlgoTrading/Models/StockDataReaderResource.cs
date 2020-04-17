using System.Collections.Generic;
using Tenatus.API.Components.AlgoTrading.Services.Scrapping;

namespace Tenatus.API.Components.AlgoTrading.Models
{
    public class StockDataReaderResource
    {
        public IStockDataReader Reader { get; set; }
        public List<string> Users { get; set; } = new List<string>();
        public string Stock { get; set; }
    }
}