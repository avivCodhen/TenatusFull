using System;
using Tenatus.API.Data;
using Tenatus.API.Types;

namespace Tenatus.API.Components.Dashboard.Models
{
    public class UserOrderModel
    {
        public DateTimeOffset Created{ get; set; }
        public int Id { get; set; }
        public string ExternalId { get; set; }
        public UserOrderType UserOrderType { get; set; }
        public int Quantity { get; set; }
        public decimal BuyingPrice { get; set; }
        public string UserOrderActionType { get; set; }
        public bool Filled { get; set; }
        public string Stock { get; set; }
    }
}