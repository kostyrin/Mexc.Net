﻿namespace Mexc.Net.Objects.Models.Spot.Socket;

/// <summary>
/// Candlestick information for symbol
/// </summary>
[JsonConverter(typeof(ArrayConverter))]
public class MexcSpotKline : MexcKlineBase
{ }