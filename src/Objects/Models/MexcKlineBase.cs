using System;
using CryptoExchange.Net.Converters;
using Mexc.Net.Converters;
using Mexc.Net.Enums;
using Mexc.Net.Interfaces;
using Newtonsoft.Json;

namespace Mexc.Net.Objects.Models;

/// <summary>
/// Candlestick information for symbol
/// </summary>
public abstract class MexcKlineBase : IMexcKline
{
    /// <summary>
    /// The open time of this candlestick
    /// </summary>
    [JsonProperty("t"), JsonConverter(typeof(DateTimeConverter))]
    public DateTime OpenTime { get; set; }

    /// <inheritdoc />
    [JsonProperty("a")]
    public decimal Volume { get; set; }

    /// <summary>
    /// The close time of this candlestick
    /// </summary>
    [JsonProperty("T"), JsonConverter(typeof(DateTimeConverter))]
    public DateTime CloseTime { get; set; }

    /// <summary>
    /// The interval of this candlestick
    /// </summary>
    [JsonProperty("i"), JsonConverter(typeof(KlineIntervalConverter))]
    public KlineInterval Interval { get; set; }
    /// <summary>
    /// The open price of this candlestick
    /// </summary>
    [JsonProperty("o")]
    public decimal OpenPrice { get; set; }
    /// <summary>
    /// The close price of this candlestick
    /// </summary>
    [JsonProperty("c")]
    public decimal ClosePrice { get; set; }
    /// <summary>
    /// The highest price of this candlestick
    /// </summary>
    [JsonProperty("h")]
    public decimal HighPrice { get; set; }
    /// <summary>
    /// The lowest price of this candlestick
    /// </summary>
    [JsonProperty("l")]
    public decimal LowPrice { get; set; }
}
