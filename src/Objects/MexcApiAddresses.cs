namespace Mexc.Net.Objects;

/// <summary>
/// Mexc API addresses
/// </summary>
public class MexcApiAddresses
{
    /// <summary>
    /// Base rest address
    /// </summary>
    public string UnifiedRestAddress { get; set; } = string.Empty;
    /// <summary>
    /// Base socket address
    /// </summary>
    public string UnifiedSocketAddress { get; set; } = string.Empty;

    /// <summary>
    /// Default live addresses
    /// </summary>
    public static MexcApiAddresses Default = new MexcApiAddresses
    {
        UnifiedRestAddress = "https://api.mexc.com/api/",
        UnifiedSocketAddress = "wss://wbs.mexc.com/ws",
    };

    /// <summary>
    /// Demo addresses
    /// </summary>
    public static MexcApiAddresses Demo = new MexcApiAddresses
    {
        UnifiedRestAddress = "",
        UnifiedSocketAddress = "",
    };
}
