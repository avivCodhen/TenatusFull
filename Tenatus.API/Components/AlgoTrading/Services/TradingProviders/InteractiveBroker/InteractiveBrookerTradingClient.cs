using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IBApi;
using Tenatus.API.Components.AlgoTrading.Models;
using Tenatus.API.EnumTypes;
using Tenatus.API.Extensions;
using Order = IBApi.Order;

namespace Tenatus.API.Components.AlgoTrading.Services.TradingProviders.InteractiveBroker
{
    public class InteractiveBrookerTradingClient : ITradingClient
    {
        private EWrapperImpl ibClient;

        public InteractiveBrookerTradingClient(string accountName, string lastOrderId)
        {
            ibClient = new EWrapperImpl();
            ibClient.NextOrderId = Convert.ToInt16(lastOrderId);
            ibClient.ClientSocket.eConnect("127.0.0.1", 7497, 0);
            var reader = new EReader(ibClient.ClientSocket, ibClient.Signal);
            reader.Start();
            new Thread(() =>
            {
                while (ibClient.ClientSocket.IsConnected())
                {
                    ibClient.Signal.waitForSignal();
                    reader.processMsgs();
                }
            }) {IsBackground = true}.Start();
            /*
            var contract = new Contract()
            {
                ConId = 0, Symbol = "MSFT", SecType = "STK",
                Exchange = "SMART", Currency = "USD", LocalSymbol = "MSFT"
            };

            List<TagValue> mktDataOptions = new List<TagValue>();


            ibClient.ClientSocket.reqMktData(10, contract, "", false, false, mktDataOptions);
            */

            //ibClient.ClientSocket.reqAccountUpdates(true, accountName);
            //ibClient.ClientSocket.reqAccountUpdates(false, accountName);
        }

        public async Task<OrderModel> Buy(string stock, int quantity, decimal price)
        {
            var contract = GetDefaultContract(stock);
            var id = ibClient.NextOrderId++;
            var orderInfo = GetOrder(quantity, price, id, true);
            ibClient.ClientSocket.placeOrder(id, contract, orderInfo);

            WaitForOrderComplete(id);
            return new OrderModel
            {
                Quantity = quantity, BuyingPrice = price, ExternalId = id.ToString(),
                UserOrderType = UserOrderType.Limit, UserOrderActionType = UserOrderActionType.Buy
            };
        }

        private void WaitForOrderComplete(int id)
        {
            var success = false;
            ibClient.IbOrderStatus += status =>
            {
                if (status.OrderId == id && status.Remaining == 0)
                {
                    success = true;
                }
            };

            while (!success)
            {
            }
        }


        public async Task<OrderModel> Sell(string orderId, string stock, int quantity, decimal price)
        {
            var contract = GetDefaultContract(stock);
            var id = Convert.ToInt16(orderId);

            var orderInfo = GetOrder(quantity, price, id, false);

            ibClient.ClientSocket.placeOrder(id, contract, orderInfo);

            WaitForOrderComplete(id);
            return new OrderModel
            {
                Quantity = quantity, BuyingPrice = price, ExternalId = id.ToString(),
                UserOrderType = UserOrderType.Limit, UserOrderActionType = UserOrderActionType.Buy
            };
        }

        public async Task<OrderModel> LastOrderOrDefault(string stock)
        {
            var openOrders = new List<IbOpenOrder>();
            ibClient.ClientSocket.reqOpenOrders();
            ibClient.IbOpenOrder += order => { openOrders.Add(order); };
            var success = false;
            ibClient.IbOpenOrderEnd += () => { success = true; };

            var waitTime = DateTime.Now.AddMinutes(1);
            while (!success && DateTime.Now < waitTime )
            {
            }

            var openOrder = openOrders.SingleOrDefault(x => x.Contract.Symbol.EqualsIgnoreCase(stock));
            if (openOrder != null)
                return new OrderModel
                {
                    BuyingPrice = Convert.ToDecimal(openOrder.Order.LmtPrice),
                    ExternalId = openOrder.OrderId.ToString(),
                    Quantity = Convert.ToInt16(openOrder.Order.TotalQuantity - openOrder.Order.FilledQuantity),
                    UserOrderActionType = openOrder.Order.Action == "BUY"
                        ? UserOrderActionType.Buy
                        : UserOrderActionType.Sell
                };
            return null;
        }

        public async Task<Position> GetCurrentPositionOrDefault(string stock)
        {
            Position pos = null;
            ibClient.IbPositions += position =>
            {
                if (position.Symbol.EqualsIgnoreCase(stock) && position.Position != 0)
                {
                    pos = new Position
                    {
                        BuyingPrice = Convert.ToDecimal(position.BuyingPrice),
                        Quantity = Convert.ToInt16(position.Position)
                    };
                }
            };
            ibClient.ClientSocket.reqPositions();
            var waitTime = DateTime.Now;
            while (pos == null && DateTime.Now < waitTime)
            {
            }

            return pos;
        }

        private static Order GetOrder(int quantity, decimal price, int id, bool buy)
        {
            var orderInfo = new Order
            {
                OrderId = id,
                OrderType = "LMT",
                Action = buy ? "BUY" : "SELL",
                TotalQuantity = quantity,
                LmtPrice = Convert.ToDouble(price),
                Tif = "DAY"
            };
            return orderInfo;
        }

        private Contract GetDefaultContract(string stock)
        {
            return new Contract
            {
                Symbol = stock.ToUpper(), SecType = "STK", Exchange = "SMART", Currency = "USD"
            };
        }
    }
}