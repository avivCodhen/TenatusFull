using Tenatus.API.Components.AlgoTrading.Services.TradingProviders;
using Tenatus.API.Components.AlgoTrading.Services.TradingProviders.Traders;
using Tenatus.API.Data;

namespace Tenatus.API.Components.AlgoTrading.Models
{
    public class TraderResource
    {
        public Trader Trader { get; set; }
        public Strategy Strategy { get; set; }
        public string UserId { get; set; }
        
    }
}