using System.Threading.Tasks;
using Tenatus.API.Components.AlgoTrading.Models;

namespace Tenatus.API.Components.AlgoTrading.Services.TradingProviders
{
    public interface ITradingClient
    {
        Task<bool> Buy(string stock, int quantity, decimal price);
        Task<bool> Sell(string stock, int quantity, decimal price);
        Task<OrderModel> LastOrderStatusOrDefault(string stock);
        Task<Position> GetCurrentPositionOrDefault(string stock);
    }
}