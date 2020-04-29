using System;
using System.Linq;
using System.Threading.Tasks;
using Alpaca.Markets;
using Tenatus.API.Components.AlgoTrading.Models;
using Tenatus.API.Extensions;
using Tenatus.API.Types;
using Tenatus.API.Util;
using static Alpaca.Markets.Environments;

namespace Tenatus.API.Components.AlgoTrading.Services.TradingProviders.Alpaca
{
    public class AlpacaClient : ITradingClient
    {
        private readonly AlpacaTradingClient _alpacaTradingClient;

        public AlpacaClient()
        {
            _alpacaTradingClient = Paper
                .GetAlpacaTradingClient(new SecretKey("PKCD5HTGM3YXOSDKK9IQ",
                    "t79OEYnzAqQixSYrWyWXGwMCGo5jmOWQzFSGOeP8"));
        }

        public AlpacaClient(string apiKey, string apiSecret)
        {
            _alpacaTradingClient = Paper
                .GetAlpacaTradingClient(new SecretKey(apiKey,
                    apiSecret));
        }

        public async Task<OrderModel> Buy(string stock, int quantity, decimal price)
        {
            var newOrderId = await SubmitOrder(stock, quantity, price, OrderSide.Buy);
            return await OrderCompletedOrDefault(newOrderId);
        }

        public async Task<OrderModel> Sell(string stock, int quantity, decimal price)
        {
            var newOrderId = await SubmitOrder(stock, quantity, price, OrderSide.Sell);
            return await OrderCompletedOrDefault(newOrderId);
        }

        public Task<bool> CancelOrder(string lastOrderId)
        {
            throw new NotImplementedException();
        }

        public async Task CancelAllOrders()
        {
            await _alpacaTradingClient.DeleteAllOrdersAsync();
        }

        public async Task<bool> RequestBudget(decimal amount)
        {
            var account = await _alpacaTradingClient.GetAccountAsync();
            var buyingPower = account.BuyingPower;
            return buyingPower >= amount;
        }

        public async Task<OrderModel> ActiveOrderOrDefault(string stock)
        {
            return await OrderCompletedOrDefault(new Guid());
        }

        public async Task<Position> CurrentPositionOrDefault(string stock)
        {
            try
            {
                var position = await _alpacaTradingClient.GetPositionAsync(stock.ToUpper());

                if (position == null) return null;

                var orders = await _alpacaTradingClient.ListOrdersAsync(new ListOrdersRequest()
                {
                    OrderListSorting = SortDirection.Descending,
                    OrderStatusFilter = OrderStatusFilter.Closed,
                });
                var order = orders
                    .Where(x => x.OrderStatus != OrderStatus.Canceled &&
                                string.Equals(stock, x.Symbol, StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(x => x.CreatedAt).FirstOrDefault();

                if (order.OrderStatus != OrderStatus.Filled)
                    return null;

                return new Position()
                    {BuyingPrice = order.LimitPrice.Value, Quantity = position.Quantity};
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return null;
        }

        private async Task<Guid> SubmitOrder(string stock, int quantity, decimal price, OrderSide side)
        {
            Console.WriteLine($"Submitting {side} order for {quantity} shares at ${price}.");
            var order = await _alpacaTradingClient.PostOrderAsync(
                new NewOrderRequest(
                    stock.ToUpper(), quantity, side, OrderType.Limit, TimeInForce.Day)
                {
                    LimitPrice = price
                });
            return order.OrderId;
        }

        private async Task<OrderModel> OrderCompletedOrDefault(Guid orderId)
        {
            try
            {
                while (true)
                {
                    var orders = await _alpacaTradingClient.ListOrdersAsync(
                        new ListOrdersRequest
                        {
                            OrderListSorting = SortDirection.Descending,
                            LimitOrderNumber = 10,
                            OrderStatusFilter = OrderStatusFilter.Closed
                        });
                    var order = orders.SingleOrDefault(x => x.OrderId == orderId);
                    if (order != null)
                    {
                        var userOrderType = order.OrderType switch
                        {
                            OrderType.Limit => AppConstants.Lmt,
                            OrderType.StopLimit => AppConstants.StopLimit,
                            _ => throw new ArgumentOutOfRangeException()
                        };

                        switch (order.OrderStatus)
                        {
                            case OrderStatus.Filled:
                                return new OrderModel()
                                {
                                    UserOrderActionType = order.OrderSide == OrderSide.Buy
                                        ? UserOrderActionType.Buy
                                        : UserOrderActionType.Sell,
                                    BuyingPrice = order.LimitPrice.Value,
                                    Quantity = Convert.ToInt16(order.Quantity),
                                    UserOrderType = userOrderType,
                                    ExternalId = order.OrderId.ToString(),
                                    Filled = true
                                };
                            case OrderStatus.Suspended:
                            case OrderStatus.Canceled:
                            case OrderStatus.Expired:
                            case OrderStatus.Stopped:
                            case OrderStatus.Rejected:
                                return null;
                            default:
                                continue;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error from orderCompleted: {e.Message}");
                return null;
            }
        }
    }
}