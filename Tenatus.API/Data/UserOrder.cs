using System;
using Tenatus.API.Types;

namespace Tenatus.API.Data
{
    public class UserOrder
    {
        public string TradingClient { get; set; }
        public DateTimeOffset Created { get; set; } = DateTimeOffset.Now;
        public int Id { get; set; }
        public string ExternalId { get; set; }
        public string UserOrderType { get; set; }
        public int Quantity { get; set; }
        public decimal BuyingPrice { get; set; }
        public UserOrderActionType UserOrderActionType { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public string UserId { get; set; }
        public bool Filled { get; set; }
        public string Stock { get; set; }
    }
}