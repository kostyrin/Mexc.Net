using System;
using System.Collections.Generic;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Mexc.Net.Objects.Models;

/// <summary>
/// Exchange info
/// </summary>
public class MexcExchangeInfo
{
    /// <summary>
    /// The timezone the server uses
    /// </summary>
    public string TimeZone { get; set; } = string.Empty;
    /// <summary>
    /// The current server time
    /// </summary>
    [JsonConverter(typeof(DateTimeConverter))]
    public DateTime ServerTime { get; set; }
    /// <summary>
    /// The rate limits used
    /// </summary>
    public IEnumerable<MexcRateLimit> RateLimits { get; set; } = Array.Empty<MexcRateLimit>();

    public IEnumerable<MexcSymbol> Symbols { get; set; } = Array.Empty<MexcSymbol>();

    /// <summary>
    /// Filters
    /// </summary>
    public IEnumerable<object> ExchangeFilters { get; set; } = Array.Empty<object>();
}