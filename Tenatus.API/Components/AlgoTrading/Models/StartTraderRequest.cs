using System.Collections.Generic;
using Tenatus.API.Data;

namespace Tenatus.API.Components.AlgoTrading.Controllers
{
    public class StartTraderRequest
    {
        public List<string> Stocks { get; set; }
        public decimal BuyingValue { get; set; }
        public decimal SellingValue { get; set; }
    }
}