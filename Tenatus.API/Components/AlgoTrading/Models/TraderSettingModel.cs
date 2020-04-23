using System.Collections.Generic;

namespace Tenatus.API.Components.AlgoTrading.Models
{
    public class TraderSettingModel
    {
        public decimal BuyingValue { get; set; } = new decimal(0.9995);
        public decimal SellingValue { get; set; } = new decimal(1.0001);
        public List<string> Stocks { get; set; }
        public bool IsOn { get; set; }
    }
}