using Mexc.Net.Objects.Models;

namespace Mexc.Net.Interfaces;

/// <summary>
/// The order book for a asset
/// </summary>
public interface IMexcOrderBook
{
    /// <summary>
    /// The symbol of the order book (only filled from stream updates)
    /// </summary>
    string Symbol { get; set; }

    /// <summary>
    /// The ID of the last update
    /// </summary>
    long LastUpdateId { get; set; }

    /// <summary>
    /// The list of bids
    /// </summary>
    IEnumerable<MexcOrderBookEntry> Bids { get; set; }

    /// <summary>
    /// The list of asks
    /// </summary>
    IEnumerable<MexcOrderBookEntry> Asks { get; set; }
}

/// <summary>
/// Order book update event
/// </summary>
public interface IMexcEventOrderBook : IMexcOrderBook
{
    /// <summary>
    /// The ID of the first update
    /// </summary>
    long? FirstUpdateId { get; set; }
    /// <summary>
    /// Timestamp of the event
    /// </summary>
    DateTime EventTime { get; set; }
}

/// <summary>
/// Futures order book update event
/// </summary>
public interface IMexcFuturesEventOrderBook : IMexcEventOrderBook
{
    /// <summary>
    /// Transaction time
    /// </summary>
    DateTime TransactionTime { get; set; }
    /// <summary>
    /// Last update id of the previous update
    /// </summary>
    public long LastUpdateIdStream { get; set; }
}