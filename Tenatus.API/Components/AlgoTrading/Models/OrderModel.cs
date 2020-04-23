using Tenatus.API.EnumTypes;

namespace Tenatus.API.Components.AlgoTrading.Models
{
    public class OrderModel
    {
        public string ExternalId { get; set; }
        public int Quantity { get; set; }
        public decimal BuyingPrice { get; set; }
        public UserOrderActionType UserOrderActionType { get; set; }
        public UserOrderType UserOrderType { get; set; }

    }
}