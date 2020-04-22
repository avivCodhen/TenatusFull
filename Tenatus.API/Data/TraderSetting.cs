using System.Collections.Generic;

namespace Tenatus.API.Data
{
    public class TraderSetting
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public decimal BuyingValue { get; set; } = new decimal(0.9995);
        public decimal SellingValue { get; set; } = new decimal(1.0001);
        public virtual List<Stock> Stocks { get; set; }

    }
}