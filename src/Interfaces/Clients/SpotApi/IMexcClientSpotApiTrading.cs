using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CryptoExchange.Net.Objects;
using Mexc.Net.Enums;
using Mexc.Net.Objects.Models;

namespace Mexc.Net.Interfaces.Clients.SpotApi;

public interface IMexcClientSpotApiTrading
{
    Task<WebCallResult<MexcPlacedOrder>> PlaceTestOrderAsync(string symbol,
        OrderSide side,
        SpotOrderType type,
        decimal? quantity = null,
        decimal? quoteQuantity = null,
        string newClientOrderId = null,
        decimal? price = null,
        int? receiveWindow = null,
        CancellationToken ct = default);

    Task<WebCallResult<MexcPlacedOrder>> PlaceOrderAsync(string symbol,
        OrderSide side,
        SpotOrderType type,
        decimal? quantity = null,
        decimal? quoteQuantity = null,
        string newClientOrderId = null,
        decimal? price = null,
        int? receiveWindow = null,
        CancellationToken ct = default);

    Task<WebCallResult<MexcOrderBase>> CancelOrderAsync(string symbol, long? orderId = null, string origClientOrderId = null, string newClientOrderId = null, long? receiveWindow = null, CancellationToken ct = default);

    Task<WebCallResult<IEnumerable<MexcOrder>>> GetOpenOrdersAsync(string symbol = null, int? receiveWindow = null, CancellationToken ct = default);

    Task<WebCallResult<MexcOrder>> GetOrderAsync(string symbol, long? orderId = null, string origClientOrderId = null, long? receiveWindow = null, CancellationToken ct = default);
}
