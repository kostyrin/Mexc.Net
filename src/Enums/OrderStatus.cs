namespace Mexc.Net.Enums;

/// <summary>
/// The status of an orderн
/// </summary>
public enum OrderStatus
{
    /// <summary>
    /// Order is new
    /// </summary>
    New,
    /// <summary>
    /// Order is partly filled, still has quantity left to fill
    /// </summary>
    PartiallyFilled,
    /// <summary>
    /// The order has been filled and completed
    /// </summary>
    Filled,
    /// <summary>
    /// The order has been canceled
    /// </summary>
    Canceled,
    /// <summary>
    /// The order has been partially canceled
    /// </summary>
    PartiallyCanceled
}