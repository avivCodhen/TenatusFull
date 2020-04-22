using System;
using System.Threading;
using System.Threading.Tasks;
using IBApi;
using Tenatus.API.Components.AlgoTrading.Models;
using Order = IBApi.Order;

namespace Tenatus.API.Components.AlgoTrading.Services.TradingProviders.InteractiveBroker
{
    public class InteractiveBrookerTradingClient : ITradingClient
    {
        private EWrapperImpl ibClient;

        public InteractiveBrookerTradingClient(string accountName)
        {
            ibClient = new EWrapperImpl();
            ibClient.ClientSocket.eConnect("127.0.0.1", 7497, 0);
            ibClient.NextOrderId++;
            var reader = new EReader(ibClient.ClientSocket, ibClient.Signal);
            reader.Start();
            new Thread(() => {
                while (ibClient.ClientSocket.IsConnected())
                {
                    ibClient.Signal.waitForSignal();
                    reader.processMsgs();
                } }) { IsBackground = true }.Start();

            ibClient.AccountValue += OnIbClientOnAccountValue; 
            ibClient.ClientSocket.reqAccountUpdates(true, "DU2020349");
            ibClient.ClientSocket.reqAccountUpdates(false, "DU2020349");
        }

        private void OnIbClientOnAccountValue(string s)
        {
        }

        public Task<bool> Buy(string stock, int quantity, decimal price)
        {
            var orderId = ibClient.NextOrderId++;
            Contract contract = new Contract();
            contract.Symbol = stock.ToUpper();
            contract.SecType = "STK";
            contract.Exchange = "SMART";
            contract.Currency = "USD";

            Order orderInfo = new Order();
            orderInfo.OrderId = orderId;
            orderInfo.OrderType = "LMT";
            orderInfo.Action = "BUY";
            orderInfo.TotalQuantity = quantity;
            orderInfo.LmtPrice = Convert.ToDouble(price+1);
            orderInfo.Tif = "DAY";
            ibClient.ClientSocket.placeOrder(orderId, contract, orderInfo);

            return Task.FromResult(true);
        }

        public Task<bool> Sell(string stock, int quantity, decimal price)
        {
            /*var iOrderId = orderId;
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
            ibClient.ClientSocket.placeOrder(iOrderId, contract, orderInfo);*/
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