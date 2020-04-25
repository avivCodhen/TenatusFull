using IBApi;

namespace Tenatus.API.Components.AlgoTrading.Services.TradingProviders.InteractiveBroker
{
    public class IbOpenOrder
    {
        public Contract Contract { get; set; }
        public Order Order { get; set; }
        public int OrderId { get; set; }
    }
}