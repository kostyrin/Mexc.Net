using System;
using CryptoExchange.Net.Converters;
using Mexc.Net.Converters;
using Mexc.Net.Enums;
using Mexc.Net.Interfaces;
using Newtonsoft.Json;

namespace Mexc.Net.Objects.Models.Spot.Socket;

/// <summary>
/// Wrapper for kline information for a symbol
/// </summary>
public class MexcStreamKlineData : IMexcStreamKlineData
{
    /// <summary>
    /// The data of stream
    /// </summary>
    [JsonProperty("k")]
    [JsonConverter(typeof(InterfaceConverter<MexcStreamKline>))]
    public IMexcStreamKline Data { get; set; }
}

/// <summary>
/// The kline data
/// </summary>
public class MexcStreamKline : MexcKlineBase, IMexcStreamKline
{
    /// <summary>
    /// The open time of this candlestick
    /// </summary>
    [JsonProperty("t"), JsonConverter(typeof(DateTimeConverter))]
    public new DateTime OpenTime { get; set; }

    /// <summary>
    /// The close time of this candlestick
    /// </summary>
    [JsonProperty("T"), JsonConverter(typeof(DateTimeConverter))]
    public new DateTime CloseTime { get; set; }

    /// <summary>
    /// The interval of this candlestick
    /// </summary>
    [JsonProperty("i"), JsonConverter(typeof(KlineIntervalConverter))]
    public KlineInterval Interval { get; set; }
    /// <summary>
    /// The open price of this candlestick
    /// </summary>
    [JsonProperty("o")]
    public new decimal OpenPrice { get; set; }
    /// <summary>
    /// The close price of this candlestick
    /// </summary>
    [JsonProperty("c")]
    public new decimal ClosePrice { get; set; }
    /// <summary>
    /// The highest price of this candlestick
    /// </summary>
    [JsonProperty("h")]
    public new decimal HighPrice { get; set; }
    /// <summary>
    /// The lowest price of this candlestick
    /// </summary>
    [JsonProperty("l")]
    public new decimal LowPrice { get; set; }

    /// <summary>
    /// Casts this object to a <see cref="MexcSpotKline"/> object
    /// </summary>
    /// <returns></returns>
    public MexcSpotKline ToKline()
    {
        return new MexcSpotKline
        {
            OpenPrice = OpenPrice,
            ClosePrice = ClosePrice,
            Volume = Volume,
            CloseTime = CloseTime,
            HighPrice = HighPrice,
            LowPrice = LowPrice,
            OpenTime = OpenTime,
        };
    }
}
