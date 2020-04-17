using Tenatus.API.Components.AlgoTrading.Services.TradingProviders;

namespace Tenatus.API.Components.AlgoTrading.Models
{
    public class TraderResource
    {
        public Trader Trader { get; set; }
        public string Stock { get; set; }
        public string UserId { get; set; }
        
    }
}