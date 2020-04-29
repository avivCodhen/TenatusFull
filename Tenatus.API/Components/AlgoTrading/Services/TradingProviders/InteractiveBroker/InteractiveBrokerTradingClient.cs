using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IBApi;
using Tenatus.API.Components.AlgoTrading.Models;
using Tenatus.API.Extensions;
using Tenatus.API.Types;
using Tenatus.API.Util;
using Order = IBApi.Order;

namespace Tenatus.API.Components.AlgoTrading.Services.TradingProviders.InteractiveBroker
{
    public class InteractiveBrokerTradingClient : ITradingClient
    {
        private readonly string _accountName;
        private EWrapperImpl ibClient;

        public InteractiveBrokerTradingClient(string accountName)
        {
            _accountName = accountName;
            ibClient = new EWrapperImpl();
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

            ibClient.ClientSocket.reqIds(-1);
            /*
            var contract = new Contract()
            {
                ConId = 0, Symbol = "MSFT", SecType = "STK",
                Exchange = "SMART", Currency = "USD", LocalSymbol = "MSFT"
            };

            List<TagValue> mktDataOptions = new List<TagValue>();


            ibClient.ClientSocket.reqMktData(10, contract, "", false, false, mktDataOptions);
            */
        }

        public Task<OrderModel> Buy(string stock, int quantity, decimal price)
        {
            var contract = GetDefaultContract(stock);
            var id = ibClient.NextOrderId;
            var orderInfo = GetOrder(quantity, price, id, true);
            ibClient.ClientSocket.placeOrder(id, contract, orderInfo);

            WaitForOrderComplete(id);
            ibClient.NextOrderId++;
            return Task.FromResult(new OrderModel
            {
                Quantity = quantity, BuyingPrice = price, ExternalId = id.ToString(),
                UserOrderType = AppConstants.Lmt, UserOrderActionType = UserOrderActionType.Buy
            });
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


        public Task<OrderModel> Sell(string stock, int quantity, decimal price)
        {
            var contract = GetDefaultContract(stock);
            var id = ibClient.NextOrderId;
            var orderInfo = GetOrder(quantity, price, id, false);

            ibClient.ClientSocket.placeOrder(id, contract, orderInfo);

            WaitForOrderComplete(id);
            ibClient.NextOrderId++;
            return Task.FromResult(new OrderModel
            {
                Quantity = quantity, BuyingPrice = price, ExternalId = id.ToString(),
                UserOrderType = AppConstants.Lmt, UserOrderActionType = UserOrderActionType.Buy
            });
        }

        public Task<bool> CancelOrder(string lastOrderId)
        {
            return Task.FromResult<bool>(false);
        }

        public Task CancelAllOrders()
        {
            ibClient.ClientSocket.reqGlobalCancel();
            return Task.CompletedTask;
        }

        public Task<bool> RequestBudget(decimal amount)
        {
            var success = false;
            ibClient.AccountValue += s =>
            {
                if (Convert.ToDecimal(s) >= amount)
                {
                    success = true;
                }
            };
            ibClient.ClientSocket.reqAccountUpdates(true, _accountName);
            ibClient.ClientSocket.reqAccountUpdates(false, _accountName);
            var waitTime = DateTime.Now.AddMinutes(1);
            while (!success && DateTime.Now < waitTime)
            {
                return Task.FromResult<bool>(true);
            }

            return Task.FromResult(false);
        }

        public Task<OrderModel> ActiveOrderOrDefault(string stock)
        {
            ibClient.ClientSocket.reqOpenOrders();
            Order openOrder = null;
            ibClient.IbOpenOrder += order =>
            {
                if (order.Contract.Symbol.EqualsIgnoreCase(stock))
                {
                    openOrder = order.Order;
                }
            };
            var success = false;
            ibClient.IbOpenOrderEnd += () => { success = true; };

            var waitTime = DateTime.Now.AddMinutes(1);
            while (!success && DateTime.Now < waitTime)
            {
            }

            return Task.FromResult(new OrderModel
            {
                BuyingPrice = Convert.ToDecimal(openOrder.LmtPrice),
                ExternalId = openOrder.OrderId.ToString(),
                Quantity = Convert.ToInt16(openOrder.TotalQuantity - openOrder.FilledQuantity),
                UserOrderActionType = openOrder.Action == "BUY"
                    ? UserOrderActionType.Buy
                    : UserOrderActionType.Sell
            });
        }

        public Task<Position> CurrentPositionOrDefault(string stock)
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
            var waitTime = DateTime.Now.AddMinutes(1);
            while (pos == null && DateTime.Now < waitTime)
            {
            }

            return Task.FromResult(pos);
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