using System;
using System.Threading.Tasks;
using IBApi;
using Tenatus.API.Components.AlgoTrading.Models;
using Tenatus.API.Components.AlgoTrading.Services.TradingProviders;
using Order = IBApi.Order;

namespace Samples
{
    public class InteractiveBrookerTradingClient : ITradingClient
    {
        private int orderId = 0;

        private EWrapperImpl ibClient;
        public InteractiveBrookerTradingClient()
        {
            ibClient = new EWrapperImpl();
            ibClient.ClientSocket.eConnect("127.0.0.1", 7497, 0);

        }
        public Task<bool> Buy(string stock, int quantity, decimal price)
        {
            orderId++;
            Contract contract = new Contract();
            contract.Symbol = stock.ToUpper();
            contract.SecType = "STK";
            contract.Exchange = "SMART";
            contract.Currency = "USD";

            Order orderInfo = new Order();
            orderInfo.OrderId = orderId;
            orderInfo.OrderType = "LMT";
            orderInfo.Action = "BUY";
            orderInfo.TotalQuantity = 100;
            orderInfo.LmtPrice = 185.00;
            orderInfo.Tif = "DAY";
            ibClient.ClientSocket.placeOrder(orderId, contract, orderInfo);
            return Task.FromResult(true);

        }

        public Task<bool> Sell(string stock, int quantity, decimal price)
        {
            var iOrderId = 341;
            Contract contract = new Contract();
            contract.Symbol = stock.ToUpper();
            contract.SecType = "STK";
            contract.Exchange = "SMART";
            contract.Currency = "USD";

            Order orderInfo = new Order();
            orderInfo.OrderId = iOrderId;
            orderInfo.OrderType = "LMT";
            orderInfo.Action = "SELL";
            orderInfo.TotalQuantity = 100;
            orderInfo.LmtPrice = Convert.ToDouble(price);
            orderInfo.Tif = "DAY";
            ibClient.ClientSocket.placeOrder(iOrderId, contract, orderInfo);
            return Task.FromResult(true);
        }

        public Task<OrderModel> LastOrderStatusOrDefault(string stock)
        {
            return Task.FromResult<OrderModel>(null);
        }

        public Task<Position> GetCurrentPositionOrDefault(string stock)
        {
            return Task.FromResult<Position>(null);
        }
    }
}