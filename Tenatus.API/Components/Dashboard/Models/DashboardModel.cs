using System.Collections;
using System.Collections.Generic;
using Tenatus.API.Components.AlgoTrading.Models;
using Tenatus.API.Data;

namespace Tenatus.API.Components.Dashboard.Models
{
    public class DashboardModel
    {
        public bool IsOn { get; set; }
        public IEnumerable<UserOrderModel> UserOrders { get; set; }
        public IEnumerable<StrategyModel> Strategies { get; set; }
    }
}