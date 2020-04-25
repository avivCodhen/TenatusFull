using System.Threading.Tasks;
using Tenatus.API.Components.AlgoTrading.Models;

namespace Tenatus.API.Components.AlgoTrading.Services.TradingProviders
{
    public interface ITradingClient
    {
        Task<OrderModel> Buy(string stock, int quantity, decimal price);
        Task<OrderModel> Sell(string orderId, string stock, int quantity, decimal price);
        Task<OrderModel> LastOrderOrDefault(string orderId);
        Task<Position> GetCurrentPositionOrDefault(string stock);
    }
}