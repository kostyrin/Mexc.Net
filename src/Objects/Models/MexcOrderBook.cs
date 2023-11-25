using System;
using System.Collections.Generic;
using Mexc.Net.Interfaces;
using Newtonsoft.Json;

namespace Mexc.Net.Objects.Models;

/// <summary>
/// The order book for a asset
/// </summary>
public class MexcOrderBook : IMexcOrderBook
{
    /// <summary>
    /// The symbol of the order book 
    /// </summary>
    [JsonProperty("s")]
    public string Symbol { get; set; } = string.Empty;

    /// <summary>
    /// The ID of the last update
    /// </summary>
    [JsonProperty("lastUpdateId")]
    public long LastUpdateId { get; set; }

    /// <summary>
    /// The list of bids
    /// </summary>
    public IEnumerable<MexcOrderBookEntry> Bids { get; set; } = Array.Empty<MexcOrderBookEntry>();

    /// <summary>
    /// The list of asks
    /// </summary>
    public IEnumerable<MexcOrderBookEntry> Asks { get; set; } = Array.Empty<MexcOrderBookEntry>();
}
