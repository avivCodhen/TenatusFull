namespace Tenatus.API.Components.AlgoTrading.Services.TradingProviders.InteractiveBroker
{
    public class IbPosition
    {
        public string Symbol { get; set; }
        public double Position { get; set; }
        public double BuyingPrice { get; set; }
    }
}