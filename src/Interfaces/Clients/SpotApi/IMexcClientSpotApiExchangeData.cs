using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CryptoExchange.Net.Objects;
using Mexc.Net.Enums;
using Mexc.Net.Objects.Models;

namespace Mexc.Net.Interfaces.Clients.SpotApi;

public interface IMexcClientSpotApiExchangeData
{
    Task<WebCallResult<DateTime>> GetServerTimeAsync(CancellationToken ct = default);

    Task<WebCallResult<IEnumerable<MexcKline>>> GetKlinesAsync(string symbol, KlineInterval interval, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default);

    Task<WebCallResult<MexcOrderBook>> GetOrderBookAsync(string symbol, int? limit = null, CancellationToken ct = default);

    Task<WebCallResult<MexcExchangeInfo>> GetExchangeInfoAsync(CancellationToken ct = default);

    Task<WebCallResult<MexcExchangeInfo>> GetExchangeInfoAsync(string symbol, CancellationToken ct = default);

    Task<WebCallResult<MexcExchangeInfo>> GetExchangeInfoAsync(string[] symbols, CancellationToken ct = default);

    Task<WebCallResult<IMexcTick>> GetTickerAsync(string symbol, CancellationToken ct = default);

    Task<WebCallResult<IEnumerable<IMexcTick>>> GetTickersAsync(IEnumerable<string> symbols, CancellationToken ct = default);

    Task<WebCallResult<IEnumerable<IMexcTick>>> GetTickersAsync(CancellationToken ct = default);
}
