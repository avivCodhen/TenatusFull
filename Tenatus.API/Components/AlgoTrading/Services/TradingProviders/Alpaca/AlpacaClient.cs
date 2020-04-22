using System;
using System.Linq;
using System.Threading.Tasks;
using Alpaca.Markets;
using Tenatus.API.Components.AlgoTrading.Models;
using Tenatus.API.Util;
using static Alpaca.Markets.Environments;

namespace Tenatus.API.Components.AlgoTrading.Services.TradingProviders.Alpaca
{
    public class AlpacaClient : ITradingClient
    {
        private readonly AlpacaTradingClient _alpacaTradingClient;
        private AlpacaDataClient _alpacaDataClient;
        private Guid _lastTradeId;

        public AlpacaClient()
        {
            _alpacaTradingClient = Paper
                .GetAlpacaTradingClient(new SecretKey("PKCD5HTGM3YXOSDKK9IQ",
                    "t79OEYnzAqQixSYrWyWXGwMCGo5jmOWQzFSGOeP8"));
            _alpacaDataClient = Paper.GetAlpacaDataClient(new SecretKey("PKCD5HTGM3YXOSDKK9IQ",
                "t79OEYnzAqQixSYrWyWXGwMCGo5jmOWQzFSGOeP8"));
        }

        public AlpacaClient(string apiKey, string apiSecret)
        {
            _alpacaTradingClient = Paper
                .GetAlpacaTradingClient(new SecretKey(apiKey,
                    apiSecret));
        }

        public async Task<bool> Buy(string stock, int quantity, decimal price)
        {
            await SubmitOrder(stock, quantity, price, OrderSide.Buy);
            return await OrderCompletedOrDefault(stock) != null;
        }

        public async Task<bool> Sell(string stock, int quantity, decimal price)
        {
            await SubmitOrder(stock, quantity, price, OrderSide.Sell);
            return await OrderCompletedOrDefault(stock) != null;
        }

        public async Task<OrderModel> LastOrderStatusOrDefault(string stock)
        {
            return await OrderCompletedOrDefault(stock);
        }

        public async Task<Position> GetCurrentPositionOrDefault(string stock)
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
                    .OrderByDescending(x => x.CreatedAt).First();

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

        private async Task SubmitOrder(string stock, int quantity, decimal price, OrderSide side)
        {
            if (quantity == 0)
            {
                Console.WriteLine("Quantity is 0. No order submitted.");
                return;
            }

            Console.WriteLine($"Submitting {side} order for {quantity} shares at ${price}.");
            var order = await _alpacaTradingClient.PostOrderAsync(
                new NewOrderRequest(
                    stock.ToUpper(), quantity, side, OrderType.Limit, TimeInForce.Day)
                {
                    LimitPrice = price
                });
            _lastTradeId = order.OrderId;
            System.IO.File.WriteAllText($"{AppConstants.FilePath}{stock}-{GetType().Name}.txt",
                _lastTradeId.ToString());
        }

        private async Task<OrderModel> OrderCompletedOrDefault(string stock)
        {
            try
            {
                var lastTradeId =
                    Guid.Parse(System.IO.File.ReadAllText($"{AppConstants.FilePath}{stock}-{GetType().Name}.txt"));
                while (true)
                {
                    var order = await _alpacaTradingClient.GetOrderAsync(lastTradeId);
                    if (order == null)
                        return null;

                    switch (order.OrderStatus)
                    {
                        case OrderStatus.Filled:
                            return new OrderModel() {Buy = order.OrderSide == OrderSide.Buy, Price = order.LimitPrice.Value};
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
            catch (Exception e)
            {
                Console.WriteLine($"Error from orderCompleted: {e.Message}");
                return null;
            }
        }
    }
}