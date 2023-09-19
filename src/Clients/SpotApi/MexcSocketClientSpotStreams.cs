using CryptoExchange.Net.Sockets;
using Mexc.Net.Converters;
using Mexc.Net.Enums;
using Mexc.Net.Interfaces.Clients.SpotApi;
using Mexc.Net.Objects;
using Mexc.Net.Objects.Internal;
using Mexc.Net.Objects.Models;
using Mexc.Net.Objects.Models.Spot.Socket;
using Mexc.Net.Objects.Options;

namespace Mexc.Net.Clients.SpotApi;

public class MexcSocketClientSpotStreams : SocketApiClient, IMexcSocketClientSpotStreams
{
    private readonly MexcSocketOptions _options;
    private const string spotEndpoint = "spot@public.{0}.v3.api@{1}";
    private const string klineStreamEndpoint = "kline";

    /// <summary>
    /// Create a new instance of MexcSocketClientSpot with default options
    /// </summary>
    public MexcSocketClientSpotStreams(ILogger log, MexcSocketOptions options) :
        base(log, options.Environment.SocketAddress, options, options.UnifiedOptions)
    {
        _options = options;

        SetDataInterpreter((data) => string.Empty, null);
        //RateLimitPerSocketPerSecond = 4;
    }

    protected override AuthenticationProvider CreateAuthenticationProvider(ApiCredentials credentials)
        => new MexcAuthenticationProvider((MexcApiCredentials)credentials);

    #region Kline/Candlestick Streams

    public async Task<CallResult<UpdateSubscription>> SubscribeToKlineUpdatesAsync(string symbol,
        KlineInterval interval, Action<DataEvent<MexcStreamKlineData>> onMessage, CancellationToken ct = default) =>
        await SubscribeToKlineUpdatesAsync(new[] { symbol }, new[] { interval }, onMessage, ct).ConfigureAwait(false);

    public async Task<CallResult<UpdateSubscription>> SubscribeToKlineUpdatesAsync(string symbol,
        IEnumerable<KlineInterval> intervals, Action<DataEvent<MexcStreamKlineData>> onMessage, CancellationToken ct = default) =>
        await SubscribeToKlineUpdatesAsync(new[] { symbol }, intervals, onMessage, ct).ConfigureAwait(false);

    public async Task<CallResult<UpdateSubscription>> SubscribeToKlineUpdatesAsync(IEnumerable<string> symbols,
        KlineInterval interval, Action<DataEvent<MexcStreamKlineData>> onMessage, CancellationToken ct = default) =>
        await SubscribeToKlineUpdatesAsync(symbols, new[] { interval }, onMessage, ct).ConfigureAwait(false);

    public async Task<CallResult<UpdateSubscription>> SubscribeToKlineUpdatesAsync(IEnumerable<string> symbols,
        IEnumerable<KlineInterval> intervals, Action<DataEvent<MexcStreamKlineData>> onMessage, CancellationToken ct = default)
    {
        symbols.ValidateNotNull(nameof(symbols));
        //foreach (var symbol in symbols)
        //    symbol.ValidateMexcSymbol();

        var handler = new Action<DataEvent<MexcCombinedStream<MexcStreamKlineData>>>(data => onMessage(data.As(data.Data.Data, data.Data.Stream)));
        symbols = symbols.SelectMany(a =>
            intervals.Select(i => string.Format(spotEndpoint, klineStreamEndpoint, 
            $"{a.ToUpper(CultureInfo.InvariantCulture)}@{JsonConvert.SerializeObject(i, new StreamKlineIntervalConverter(false))}")).ToArray());
        return await SubscribeAsync(BaseAddress, symbols, handler, ct).ConfigureAwait(false);
    }

    public Task<CallResult<UpdateSubscription>> SubscribeToTradeUpdatesAsync(string symbol, Action<DataEvent<MexcStreamTrade>> onMessage, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<CallResult<UpdateSubscription>> SubscribeToTradeUpdatesAsync(IEnumerable<string> symbols, Action<DataEvent<MexcStreamTrade>> onMessage, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    #endregion

    internal async Task<CallResult<UpdateSubscription>> SubscribeAsync<T>(string url, IEnumerable<string> topics, Action<DataEvent<T>> onData, CancellationToken ct)
    {
        var request = new MexcSocketRequest
        {
            Method = "SUBSCRIPTION",
            Params = topics.ToArray(),
            Id = CryptoExchange.Net.ExchangeHelpers.NextId()
        };

        return await SubscribeAsync(url, request, null, false, onData, ct);
    }

    /// <inheritdoc />
    protected override bool HandleQueryResponse<T>(SocketConnection s, object request, JToken data, out CallResult<T> callResult)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    protected override bool HandleSubscriptionResponse(SocketConnection s, SocketSubscription subscription, object request, JToken message, out CallResult<object>? callResult)
    {
        callResult = null;
        if (message.Type != JTokenType.Object)
            return false;

        var id = message["id"];
        if (id == null)
            return false;

        var bRequest = (MexcSocketRequest)request;
        if ((int)id != bRequest.Id)
            return false;
        
        var result = message["msg"];
        if (result != null && result.Type != JTokenType.Null)
        {
            _logger.LogTrace($"Socket {s.SocketId} Subscription completed");
            callResult = new CallResult<object>(new object());
            return true;
        }

        var error = message["error"];
        if (error == null)
        {
            callResult = new CallResult<object>(new ServerError("Unknown error: " + message));
            return true;
        }

        callResult = new CallResult<object>(new ServerError(error["code"]!.Value<int>(), error["msg"]!.ToString()));
        return true;
    }

    /// <inheritdoc />
    protected override bool MessageMatchesHandler(SocketConnection socketConnection, JToken message, object request)
    {
        if (message.Type != JTokenType.Object)
            return false;

        var bRequest = (MexcSocketRequest)request;
        var stream = message["c"];
        if (stream == null)
            return false;

        return bRequest.Params.Contains(stream.ToString());
    }

    /// <inheritdoc />
    protected override bool MessageMatchesHandler(SocketConnection socketConnection, JToken message, string identifier)
    {
        return true;
    }

    /// <inheritdoc />
    protected override Task<CallResult<bool>> AuthenticateSocketAsync(SocketConnection s)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    protected override async Task<bool> UnsubscribeAsync(SocketConnection connection, SocketSubscription subscription)
    {
        var topics = ((MexcSocketRequest)subscription.Request!).Params;
        var unsub = new MexcSocketRequest { Method = "UNSUBSCRIPTION", Params = topics };
        var result = false;

        if (!connection.Connected)
            return true;

        await connection.SendAndWaitAsync(unsub, _options.SocketNoDataTimeout, null, 1, data =>
        {
            if (data.Type != JTokenType.Object)
                return false;

            var id = data["id"];
            if (id == null)
                return false;

            if ((int)id != unsub.Id)
                return false;

            var result = data["result"];
            if (result?.Type == JTokenType.Null)
            {
                result = true;
                return true;
            }

            return true;
        }).ConfigureAwait(false);
        return result;
    }

    
}
