using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CryptoExchange.Net;
using CryptoExchange.Net.CommonObjects;
using CryptoExchange.Net.Objects;
using Mexc.Net;
using Mexc.Net.Clients.SpotApi;
using Mexc.Net.Enums;
using Mexc.Net.Interfaces.Clients.SpotApi;
using Mexc.Net.Objects.Models;

namespace Mexc.NET.Clients.SpotApi;

public class MexcClientSpotApiTrading : IMexcClientSpotApiTrading
{
    private readonly MexcClientSpotApi _baseClient;

    private readonly string cancelOrderEndpoint = "order";
    private readonly string newTestOrderEndpoint = "order/test";
    private readonly string newOrderEndpoint = "order";
    private readonly string openOrdersEndpoint = "openOrders";
    private readonly string queryOrderEndpoint = "order";

    internal MexcClientSpotApiTrading(MexcClientSpotApi baseClient)
    {
        _baseClient = baseClient;
    }

    public Task<WebCallResult<MexcPlacedOrder>> PlaceTestOrderAsync(string symbol,
          OrderSide side,
          SpotOrderType type,
          decimal? quantity = null,
          decimal? quoteQuantity = null,
          string newClientOrderId = null,
          decimal? price = null,
          int? receiveWindow = null,
          CancellationToken ct = default)
        => _baseClient.PlaceOrderInternal(_baseClient.GetUri(newTestOrderEndpoint),
            symbol,
            side,
            type,
            quantity,
            quoteQuantity,
            newClientOrderId,
            price,
            receiveWindow,
            weight: 1,
            ct);

    public async Task<WebCallResult<MexcPlacedOrder>> PlaceOrderAsync(string symbol,
        OrderSide side,
        SpotOrderType type,
        decimal? quantity = null,
        decimal? quoteQuantity = null,
        string newClientOrderId = null,
        decimal? price = null,
        int? receiveWindow = null,
        CancellationToken ct = default)
    {
        var result = await _baseClient.PlaceOrderInternal(_baseClient.GetUri(newOrderEndpoint),
            symbol,
            side,
            type,
            quantity,
            quoteQuantity,
            newClientOrderId,
            price,
            receiveWindow,
            weight: 1,
            ct).ConfigureAwait(false);
        if (result)
            _baseClient.InvokeOrderPlaced(new OrderId() { SourceObject = result.Data, Id = result.Data.Id.ToString(CultureInfo.InvariantCulture) });
        return result;
    }

    public async Task<WebCallResult<MexcOrderBase>> CancelOrderAsync(string symbol, long? orderId = null, string origClientOrderId = null, string newClientOrderId = null, long? receiveWindow = null, CancellationToken ct = default)
    {
        //symbol.ValidateMexcSymbol();
        if (!orderId.HasValue && string.IsNullOrEmpty(origClientOrderId))
            throw new ArgumentException("Either orderId or origClientOrderId must be sent");

        var parameters = new Dictionary<string, object>
        {
            { "symbol", symbol }
        };
        parameters.AddOptionalParameter("orderId", orderId?.ToString(CultureInfo.InvariantCulture));
        parameters.AddOptionalParameter("origClientOrderId", origClientOrderId);
        parameters.AddOptionalParameter("newClientOrderId", newClientOrderId);
        //parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? _baseClient.Options.ReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

        var result = await _baseClient.SendRequestInternal<MexcOrderBase>(_baseClient.GetUri(cancelOrderEndpoint), HttpMethod.Delete, ct, parameters, true).ConfigureAwait(false);
        if (result)
            _baseClient.InvokeOrderCanceled(new OrderId() { SourceObject = result.Data, Id = result.Data.Id.ToString(CultureInfo.InvariantCulture) });
        return result;
    }

    public async Task<WebCallResult<MexcOrder>> GetOrderAsync(string symbol, long? orderId = null, string origClientOrderId = null, long? receiveWindow = null, CancellationToken ct = default)
    {
        //symbol.ValidateMexcSymbol();
        if (orderId == null && origClientOrderId == null)
            throw new ArgumentException("Either orderId or origClientOrderId must be sent");

        var parameters = new Dictionary<string, object>
        {
            { "symbol", symbol }
        };

        parameters.AddOptionalParameter("orderId", orderId?.ToString(CultureInfo.InvariantCulture));
        parameters.AddOptionalParameter("origClientOrderId", origClientOrderId);
        parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? _baseClient.Options.ReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

        return await _baseClient.SendRequestInternal<MexcOrder>(_baseClient.GetUri(queryOrderEndpoint), HttpMethod.Get, ct, parameters, true, weight: 2).ConfigureAwait(false);
    }

    public async Task<WebCallResult<IEnumerable<MexcOrder>>> GetOpenOrdersAsync(string symbol = null, int? receiveWindow = null, CancellationToken ct = default)
    {
        symbol?.ValidateMexcSymbol();

        var parameters = new Dictionary<string, object>();
        parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? _baseClient.Options.ReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));
        parameters.AddOptionalParameter("symbol", symbol);

        return await _baseClient.SendRequestInternal<IEnumerable<MexcOrder>>(_baseClient.GetUri(openOrdersEndpoint), HttpMethod.Get, ct, parameters, true, weight: symbol == null ? 40 : 3).ConfigureAwait(false);
    }
}