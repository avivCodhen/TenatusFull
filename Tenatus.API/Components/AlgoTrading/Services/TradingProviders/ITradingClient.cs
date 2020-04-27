using System.Threading.Tasks;
using Tenatus.API.Components.AlgoTrading.Models;

namespace Tenatus.API.Components.AlgoTrading.Services.TradingProviders
{
    public interface ITradingClient
    {
        Task<OrderModel> Buy(string stock, int quantity, decimal price);
        Task<OrderModel> Sell(string stock, int quantity, decimal price);
        Task<bool> CancelOrder(string lastOrderId);
        Task CancelAllOrders();
        Task<bool> RequestBudget(decimal amount);
        Task<OrderModel> ActiveOrderOrDefault(string stock);
        Task<Position> CurrentPositionOrDefault(string stock);
    }
}