using CryptoExchange.Net.CommonObjects;
using CryptoExchange.Net.Interfaces.CommonClients;
using Mexc.Net.Converters;
using Mexc.Net.Enums;
using Mexc.Net.Interfaces.Clients.SpotApi;
using Mexc.Net.Objects;
using Mexc.Net.Objects.Internal;
using Mexc.Net.Objects.Models;
using Mexc.Net.Objects.Options;

namespace Mexc.Net.Clients.SpotApi;

/// <inheritdoc cref="IMexcClientSpotApi" />
public class MexcClientSpotApi : RestApiClient, IMexcClientSpotApi, ISpotClient
{
    //internal MexcExchangeInfo ExchangeInfo;
    internal DateTime? LastExchangeInfoUpdate;

    internal static TimeSyncState _timeSyncState = new TimeSyncState("Spot Api");

    internal new MexcRestOptions Options;

    /// <summary>
    /// Event triggered when an order is placed via this client. Only available for Spot orders
    /// </summary>
    public event Action<OrderId> OnOrderPlaced;
    /// <summary>
    /// Event triggered when an order is canceled via this client. Note that this does not trigger when using CancelAllOrdersAsync. Only available for Spot orders
    /// </summary>
    public event Action<OrderId> OnOrderCanceled;

    /// <inheritdoc />
    public string ExchangeName => "Mexc";

    /// <inheritdoc />
    public IMexcClientSpotApiAccount Account { get; }

    /// <inheritdoc />
    public IMexcClientSpotApiExchangeData ExchangeData { get; }

    /// <inheritdoc />
    public IMexcClientSpotApiTrading Trading { get; }

    /// <inheritdoc />
    public IMexcClientSpotApiProAccount ProAccount { get; }

    public MexcExchangeInfo ExchangeInfo { get; set; }

    private readonly ILogger _log;


    internal MexcClientSpotApi(ILogger logger, HttpClient? httpClient, MexcRestOptions options)
        : base(logger, httpClient, options.Environment.RestAddress, options, options.UnifiedOptions)
    {
        _log = logger;
        Options = options;

        Account = new MexcClientSpotApiAccount(this);
        ExchangeData = new MexcClientSpotApiExchangeData(this);
        //Trading = new MexcClientSpotApiTrading(this);
        ProAccount = new MexcClientSpotApiProAccount(this);

        ParameterPositions[HttpMethod.Delete] = HttpMethodParameterPosition.InUri;
    }

    /// <inheritdoc />
    protected override AuthenticationProvider CreateAuthenticationProvider(ApiCredentials credentials)
        => new MexcAuthenticationProvider((MexcApiCredentials)credentials);

    /// <summary>
    /// Return the Mexc trade symbol name from base and quote asset 
    /// </summary>
    /// <param name="baseAsset"></param>
    /// <param name="quoteAsset"></param>
    /// <returns></returns>
    public string GetSymbolName(string baseAsset, string quoteAsset) => (baseAsset + "-" + quoteAsset).ToUpperInvariant();

    /// <inheritdoc />
    public override TimeSyncInfo? GetTimeSyncInfo()
            => new TimeSyncInfo(_logger, (ApiOptions.AutoTimestamp ?? ClientOptions.AutoTimestamp), (ApiOptions.TimestampRecalculationInterval ?? ClientOptions.TimestampRecalculationInterval), _timeSyncState);

    /// <inheritdoc />
    public override TimeSpan? GetTimeOffset()
        => _timeSyncState.TimeOffset;

    public async Task<WebCallResult<OrderId>> PlaceOrderAsync(string symbol, CommonOrderSide side, CommonOrderType type, decimal quantity, decimal? price = null, string accountId = null, string clientOrderId = null, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(symbol))
            throw new ArgumentException(nameof(symbol) + " required for Mexc " + nameof(ISpotClient.PlaceOrderAsync), nameof(symbol));

        var order = await Trading.PlaceOrderAsync(symbol, GetOrderSide(side), GetOrderType(type), quantity, price: price, newClientOrderId: clientOrderId, ct: ct).ConfigureAwait(false);
        if (!order)
            return order.As<OrderId>(null);

        return order.As(new OrderId
        {
            SourceObject = order,
            Id = order.Data.Id.ToString(CultureInfo.InvariantCulture)
        });
    }

    public async Task<WebCallResult<IEnumerable<Symbol>>> GetSymbolsAsync(CancellationToken ct = default)
    {
        var exchangeInfo = await ExchangeData.GetExchangeInfoAsync(ct: ct).ConfigureAwait(false);
        if (!exchangeInfo)
            return exchangeInfo.As<IEnumerable<Symbol>>(null);

        return exchangeInfo.As(exchangeInfo.Data.Symbols.Select(s =>
            new Symbol
            {
                SourceObject = s,
                Name = s.Name,
            }));
    }

    public async Task<WebCallResult<Ticker>> GetTickerAsync(string symbol, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(symbol))
            throw new ArgumentException(nameof(symbol) + " required for Mexc " + nameof(ISpotClient.GetTickerAsync), nameof(symbol));

        var ticker = await ExchangeData.GetTickerAsync(symbol, ct: ct).ConfigureAwait(false);
        if (!ticker)
            return ticker.As<Ticker>(null);

        return ticker.As(new Ticker
        {
            SourceObject = ticker.Data,
            Symbol = ticker.Data.Symbol,
            HighPrice = ticker.Data.HighPrice,
            LowPrice = ticker.Data.LowPrice,
            Price24H = ticker.Data.PrevDayClosePrice,
            LastPrice = ticker.Data.LastPrice,
            Volume = ticker.Data.Volume
        });
    }

    public async Task<WebCallResult<IEnumerable<Ticker>>> GetTickersAsync(CancellationToken ct = default)
    {
        var tickers = await ExchangeData.GetTickersAsync(ct: ct).ConfigureAwait(false);
        if (!tickers)
            return tickers.As<IEnumerable<Ticker>>(null);

        return tickers.As(tickers.Data.Select(t => new Ticker
        {
            SourceObject = t,
            Symbol = t.Symbol,
            HighPrice = t.HighPrice,
            LowPrice = t.LowPrice,
            Price24H = t.PrevDayClosePrice,
            LastPrice = t.LastPrice,
            Volume = t.Volume
        }));
    }

    public async Task<WebCallResult<IEnumerable<Kline>>> GetKlinesAsync(string symbol, TimeSpan timespan, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default)
    {
        var symbols = await ExchangeData.GetKlinesAsync(symbol, GetKlineIntervalFromTimespan(timespan), startTime, endTime, limit, ct).ConfigureAwait(false);
        if (!symbols)
            return symbols.As(Enumerable.Empty<Kline>());

        return symbols.As(symbols.Data.Select(k => new Kline
        {
            SourceObject = k,
            ClosePrice = k.ClosePrice,
            HighPrice = k.HighPrice,
            LowPrice = k.LowPrice,
            OpenPrice = k.OpenPrice,
            OpenTime = k.OpenTime,
            Volume = k.Volume,
        }));
    }

    public async Task<WebCallResult<OrderBook>> GetOrderBookAsync(string symbol, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(symbol))
            throw new ArgumentException(nameof(symbol) + " required for Mexc " + nameof(ISpotClient.GetOrderBookAsync), nameof(symbol));

        var orderbook = await ExchangeData.GetOrderBookAsync(symbol, ct: ct).ConfigureAwait(false);
        if (!orderbook)
            return orderbook.As<OrderBook>(null);

        return orderbook.As(new OrderBook
        {
            SourceObject = orderbook.Data,
            Asks = orderbook.Data.Asks.Select(a => new OrderBookEntry { Price = a.Price, Quantity = a.Quantity }),
            Bids = orderbook.Data.Bids.Select(b => new OrderBookEntry { Price = b.Price, Quantity = b.Quantity })
        });
    }

    public Task<WebCallResult<IEnumerable<Trade>>> GetRecentTradesAsync(string symbol, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public async Task<WebCallResult<IEnumerable<Balance>>> GetBalancesAsync(string accountId = null, CancellationToken ct = default)
    {
        var balances = await Account.GetAccountInfoAsync(ct: ct).ConfigureAwait(false);
        if (!balances)
            return balances.As(Enumerable.Empty<Balance>());

        return balances.As(balances.Data.Balances.Select(t => new Balance
        {
            SourceObject = t,
            Asset = t.Asset,
            Available = t.Available,
            Total = t.Total
        }));
    }

    public async Task<WebCallResult<Order>> GetOrderAsync(string orderId, string symbol = null, CancellationToken ct = default)
    {
        if (!long.TryParse(orderId, out var id))
            throw new ArgumentException("Order id invalid", nameof(orderId));

        if (string.IsNullOrWhiteSpace(symbol))
            throw new ArgumentException(nameof(symbol) + " required for Mexc " + nameof(ISpotClient.GetOrderAsync), nameof(symbol));

        var order = await Trading.GetOrderAsync(symbol!, id, ct: ct).ConfigureAwait(false);
        if (!order)
            return order.As<Order>(null);

        return order.As(new Order
        {
            SourceObject = order,
            Id = order.Data.Id.ToString(CultureInfo.InvariantCulture),
            Symbol = order.Data.Symbol,
            Price = order.Data.Price,
            Quantity = order.Data.Quantity,
            QuantityFilled = order.Data.QuantityFilled,
            Side = order.Data.Side == Enums.OrderSide.Buy ? CommonOrderSide.Buy : CommonOrderSide.Sell,
            Type = GetOrderType(order.Data.Type),
            Status = GetOrderStatus(order.Data.Status),
            Timestamp = order.Data.CreateTime
        });
    }

    public Task<WebCallResult<IEnumerable<UserTrade>>> GetOrderTradesAsync(string orderId, string symbol = null, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public async Task<WebCallResult<IEnumerable<Order>>> GetOpenOrdersAsync(string symbol = null, CancellationToken ct = default)
    {
        var orderInfo = await Trading.GetOpenOrdersAsync(symbol, ct: ct).ConfigureAwait(false);
        if (!orderInfo)
            return orderInfo.As(Enumerable.Empty<Order>());

        return orderInfo.As(orderInfo.Data.Select(s =>
            new Order
            {
                SourceObject = s,
                Id = s.Id.ToString(CultureInfo.InvariantCulture),
                Symbol = s.Symbol,
                Side = s.Side == OrderSide.Buy ? CommonOrderSide.Buy : CommonOrderSide.Sell,
                Price = s.Price,
                Quantity = s.Quantity,
                QuantityFilled = s.QuantityFilled,
                Type = GetOrderType(s.Type),
                Status = GetOrderStatus(s.Status),
                Timestamp = s.CreateTime
            }));
    }

    public Task<WebCallResult<IEnumerable<Order>>> GetClosedOrdersAsync(string symbol = null, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public async Task<WebCallResult<OrderId>> CancelOrderAsync(string orderId, string symbol = null, CancellationToken ct = default)
    {
        if (!long.TryParse(orderId, out var id))
            throw new ArgumentException("Order id invalid", nameof(orderId));

        if (string.IsNullOrWhiteSpace(symbol))
            throw new ArgumentException(nameof(symbol) + " required for Mexc " + nameof(ISpotClient.CancelOrderAsync), nameof(symbol));

        var order = await Trading.CancelOrderAsync(symbol, id, ct: ct).ConfigureAwait(false);
        if (!order)
            return order.As<OrderId>(null);

        return order.As(new OrderId
        {
            SourceObject = order,
            Id = order.Data.Id.ToString(CultureInfo.InvariantCulture)
        });
    }

    /// <inheritdoc />
    public ISpotClient CommonSpotClient => this;

    #region internal 

    internal async Task<WebCallResult<T>> SendRequestInternal<T>(Uri uri, HttpMethod method, CancellationToken cancellationToken,
        Dictionary<string, object>? parameters = null, bool signed = false, HttpMethodParameterPosition? postPosition = null,
        ArrayParametersSerialization? arraySerialization = null, int weight = 1, bool ignoreRateLimit = false) where T : class
    {
        var result = await SendRequestAsync<T>(uri, method, cancellationToken, parameters, signed, null, postPosition, arraySerialization, weight, ignoreRatelimit: ignoreRateLimit).ConfigureAwait(false);
        if (!result && result.Error!.Code == -1021 /*&& Options.SpotApiOptions.AutoTimestamp*/)
        {
            _log.LogDebug("Received Invalid Timestamp error, triggering new time sync");
            _timeSyncState.LastSyncTime = DateTime.MinValue;
        }
        return result;
    }

    internal async Task<WebCallResult> SendRequestInternal(Uri uri, HttpMethod method, CancellationToken cancellationToken,
        Dictionary<string, object>? parameters = null, bool signed = false, HttpMethodParameterPosition? postPosition = null,
        ArrayParametersSerialization? arraySerialization = null, int weight = 1, bool ignoreRateLimit = false)
    {
        var result = await SendRequestAsync(uri, method, cancellationToken, parameters, signed, null, postPosition, arraySerialization, weight, ignoreRatelimit: ignoreRateLimit).ConfigureAwait(false);
        if (!result && result.Error!.Code == -1021 /*&& Options.SpotApiOptions.AutoTimestamp*/)
        {
            _log.LogDebug("Received Invalid Timestamp error, triggering new time sync");
            _timeSyncState.LastSyncTime = DateTime.MinValue;
        }
        return result;
    }

    internal async Task<WebCallResult<MexcPlacedOrder>> PlaceOrderInternal(Uri uri,
            string symbol,
            OrderSide side,
            SpotOrderType type,
            decimal? quantity = null,
            decimal? quoteQuantity = null,
            string newClientOrderId = null,
            decimal? price = null,
            int? receiveWindow = null,
            int weight = 1,
            CancellationToken ct = default)
    {
        //symbol.ValidateMexcSymbol();

        if (quoteQuantity != null && type != SpotOrderType.Market)
            throw new ArgumentException("quoteQuantity is only valid for market orders");

        if (quantity == null && quoteQuantity == null || quantity != null && quoteQuantity != null)
            throw new ArgumentException("1 of either should be specified, quantity or quoteOrderQuantity");

        var rulesCheck = await CheckTradeRules(symbol, quantity, quoteQuantity, price, type, ct).ConfigureAwait(false);
        if (!rulesCheck.Passed)
        {
            _log.LogWarning(rulesCheck.ErrorMessage!);
            return new WebCallResult<MexcPlacedOrder>(new ArgumentError(rulesCheck.ErrorMessage!));
        }

        quantity = rulesCheck.Quantity;
        price = rulesCheck.Price;

        var parameters = new Dictionary<string, object>
        {
            { "symbol", symbol },
            { "side", JsonConvert.SerializeObject(side, new OrderSideConverter(false)) },
            { "type", JsonConvert.SerializeObject(type, new SpotOrderTypeConverter(false)) }
        };

        parameters.AddOptionalParameter("quantity", quantity?.ToString(CultureInfo.InvariantCulture));
        parameters.AddOptionalParameter("quoteOrderQty", quoteQuantity?.ToString(CultureInfo.InvariantCulture));
        parameters.AddOptionalParameter("newClientOrderId", newClientOrderId);
        parameters.AddOptionalParameter("price", price?.ToString(CultureInfo.InvariantCulture));
        //parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? Options.ReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

        return await SendRequestInternal<MexcPlacedOrder>(uri, HttpMethod.Post, ct, parameters, true, weight: weight, postPosition: HttpMethodParameterPosition.InUri).ConfigureAwait(false);
    }

    internal async Task<MexcTradeRuleResult> CheckTradeRules(string symbol, decimal? quantity, decimal? quoteQuantity, decimal? price, SpotOrderType? type, CancellationToken ct)
    {
        throw new NotImplementedException();

        var outputQuantity = quantity;
        var outputQuoteQuantity = quoteQuantity;
        var outputPrice = price;

        //if (Options.SpotApiOptions.TradeRulesBehaviour == TradeRulesBehaviour.None)
        //    return MexcTradeRuleResult.CreatePassed(outputQuantity, outputQuoteQuantity, outputPrice, null);

        //if (ExchangeInfo == null || LastExchangeInfoUpdate == null || (DateTime.UtcNow - LastExchangeInfoUpdate.Value).TotalMinutes > Options.SpotApiOptions.TradeRulesUpdateInterval.TotalMinutes)
        //    await ExchangeData.GetExchangeInfoAsync(symbol, ct).ConfigureAwait(false);

        //if (ExchangeInfo == null)
        //    return MexcTradeRuleResult.CreateFailed("Unable to retrieve trading rules, validation failed");

        //var symbolData = ExchangeInfo.Symbols.SingleOrDefault(s => string.Equals(s.Name, symbol, StringComparison.CurrentCultureIgnoreCase));
        //if (symbolData == null)
        //    return MexcTradeRuleResult.CreateFailed($"Trade rules check failed: Symbol {symbol} not found");

        //if (type != null)
        //{
        //    if (!symbolData.OrderTypes.Contains(type.Value))
        //    {
        //        return MexcTradeRuleResult.CreateFailed(
        //            $"Trade rules check failed: {type} order type not allowed for {symbol}");
        //    }
        //}

        if (price == null)
            return MexcTradeRuleResult.CreatePassed(outputQuantity, outputQuoteQuantity, null, null);

        var currentQuantity = outputQuantity ?? quantity.Value;
        var notional = currentQuantity * outputPrice.Value;

        return MexcTradeRuleResult.CreatePassed(outputQuantity, outputQuoteQuantity, outputPrice, null);
    }

    internal Uri GetUri(string path, int apiVersion = 3)
        => new(BaseAddress.AppendPath("v" + apiVersion, path));

    internal void InvokeOrderPlaced(OrderId id)
    {
        OnOrderPlaced?.Invoke(id);
    }

    internal void InvokeOrderCanceled(OrderId id)
    {
        OnOrderCanceled?.Invoke(id);
    }

    #endregion

    #region protected

    /// <inheritdoc />
    //protected override Error ParseErrorResponse(JToken error)
    //{
    //    if (!error.HasValues)
    //    {
    //        var errorBody = error.ToString();
    //        return new ServerError(string.IsNullOrEmpty(errorBody) ? "Unknown error" : errorBody);
    //    }

    //    if (error["code"] != null && error["msg"] != null)
    //    {
    //        var result = error.ToObject<MexcResult<object>>();
    //        if (result == null)
    //            return new ServerError(error["msg"]!.ToString());

    //        return new ServerError(result.Code, result.Message!);
    //    }

    //    return new ServerError(error.ToString());
    //}

    /// <inheritdoc />
    protected override Task<WebCallResult<DateTime>> GetServerTimestampAsync()
        => ExchangeData.GetServerTimeAsync();

    #endregion

    #region private 

    private static KlineInterval GetKlineIntervalFromTimespan(TimeSpan timeSpan)
    {
        if (timeSpan == TimeSpan.FromMinutes(1)) return KlineInterval.OneMinute;
        if (timeSpan == TimeSpan.FromMinutes(5)) return KlineInterval.FiveMinutes;
        if (timeSpan == TimeSpan.FromMinutes(15)) return KlineInterval.FifteenMinutes;
        if (timeSpan == TimeSpan.FromMinutes(30)) return KlineInterval.ThirtyMinutes;
        if (timeSpan == TimeSpan.FromHours(1)) return KlineInterval.OneHour;
        if (timeSpan == TimeSpan.FromHours(4)) return KlineInterval.FourHour;
        if (timeSpan == TimeSpan.FromDays(1)) return KlineInterval.OneDay;
        if (timeSpan == TimeSpan.FromDays(30) || timeSpan == TimeSpan.FromDays(31)) return KlineInterval.OneMonth;

        throw new ArgumentException("Unsupported timespan for Mexc kline interval, check supported intervals using Kucoin.Net.Objects.KucoinKlineInterval");
    }

    private static SpotOrderType GetOrderType(CommonOrderType type)
    {
        if (type == CommonOrderType.Limit) return SpotOrderType.Limit;
        if (type == CommonOrderType.Market) return SpotOrderType.Market;

        throw new ArgumentException("Unsupported order type for Binance order: " + type);
    }

    private static OrderSide GetOrderSide(CommonOrderSide side)
    {
        if (side == CommonOrderSide.Sell) return Enums.OrderSide.Sell;
        if (side == CommonOrderSide.Buy) return Enums.OrderSide.Buy;

        throw new ArgumentException("Unsupported order side for Mexc order: " + side);
    }


    private static CommonOrderType GetOrderType(SpotOrderType orderType)
    {
        if (orderType == SpotOrderType.Limit)
            return CommonOrderType.Limit;
        if (orderType == SpotOrderType.Market)
            return CommonOrderType.Market;
        return CommonOrderType.Other;
    }

    private static CommonOrderStatus GetOrderStatus(Enums.OrderStatus orderStatus)
    {
        if (orderStatus == OrderStatus.New || orderStatus == Enums.OrderStatus.PartiallyFilled)
            return CommonOrderStatus.Active;
        if (orderStatus == OrderStatus.Filled)
            return CommonOrderStatus.Filled;
        return CommonOrderStatus.Canceled;
    }

    #endregion
}