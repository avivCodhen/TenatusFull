using IBApi;

namespace Tenatus.API.Components.AlgoTrading.Services.TradingProviders.InteractiveBroker
{
    public class IbOrderStatus
    {
        public int OrderId { get; set; }
        public double Remaining { get; set; }
        public double BuyingPrice { get; set; }

    }
}