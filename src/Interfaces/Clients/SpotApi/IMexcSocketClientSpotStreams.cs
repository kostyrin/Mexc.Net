using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Sockets;
using Mexc.Net.Enums;
using Mexc.Net.Objects.Models.Spot.Socket;

namespace Mexc.Net.Interfaces.Clients.SpotApi;

public interface IMexcSocketClientSpotStreams : ISocketApiClient, IDisposable
{
    Task<CallResult<UpdateSubscription>> SubscribeToKlineUpdatesAsync(string symbol, KlineInterval interval, Action<DataEvent<MexcStreamKlineData>> onMessage, CancellationToken ct = default);

    Task<CallResult<UpdateSubscription>> SubscribeToKlineUpdatesAsync(string symbol, IEnumerable<KlineInterval> intervals, Action<DataEvent<MexcStreamKlineData>> onMessage, CancellationToken ct = default);

    Task<CallResult<UpdateSubscription>> SubscribeToKlineUpdatesAsync(IEnumerable<string> symbols, KlineInterval interval, Action<DataEvent<MexcStreamKlineData>> onMessage, CancellationToken ct = default);

    Task<CallResult<UpdateSubscription>> SubscribeToKlineUpdatesAsync(IEnumerable<string> symbols, IEnumerable<KlineInterval> intervals, Action<DataEvent<MexcStreamKlineData>> onMessage, CancellationToken ct = default);

    Task<CallResult<UpdateSubscription>> SubscribeToTradeUpdatesAsync(string symbol,
        Action<DataEvent<MexcStreamTrade>> onMessage, CancellationToken ct = default);

    Task<CallResult<UpdateSubscription>> SubscribeToTradeUpdatesAsync(IEnumerable<string> symbols,
        Action<DataEvent<MexcStreamTrade>> onMessage, CancellationToken ct = default);
}
