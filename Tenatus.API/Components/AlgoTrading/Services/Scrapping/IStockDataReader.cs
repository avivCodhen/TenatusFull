using System.Collections.Generic;
using System.Threading.Tasks;
using Tenatus.API.Components.AlgoTrading.Models;

namespace Tenatus.API.Components.AlgoTrading.Services.Scrapping
{
    public interface IStockDataReader
    {
     Task<List<StockData>> WriteStocksValue();
     StockData ReadStockValue();
     void Dispose();
    }
}