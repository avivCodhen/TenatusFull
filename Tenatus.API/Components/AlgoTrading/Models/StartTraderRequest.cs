using System.Collections.Generic;

namespace Tenatus.API.Components.AlgoTrading.Models
{
    public class StartTraderRequest
    {
        public List<string> Stocks { get; set; }
        public decimal BuyingValue { get; set; }
        public decimal SellingValue { get; set; }
    }
}