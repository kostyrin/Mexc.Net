using CryptoExchange.Net.Objects.Options;
using Mexc.Net;

namespace Mexc.Net.Objects.Options;

/// <summary>
/// Socket client options
/// </summary>
public class MexcSocketOptions : SocketExchangeOptions<MexcEnvironment>
{
    /// <summary>
    /// Default options for new MexcRestClients
    /// </summary>
    public static MexcSocketOptions Default { get; set; } = new MexcSocketOptions()
    {
        SocketSubscriptionsCombineTarget = 10,
        Environment = MexcEnvironment.Live
    };

    /// <summary>
    /// Options for the Unified API
    /// </summary>
    public SocketApiOptions UnifiedOptions { get; private set; } = new SocketApiOptions();

    internal MexcSocketOptions Copy()
    {
        var options = Copy<MexcSocketOptions>();
        options.UnifiedOptions = UnifiedOptions.Copy<SocketApiOptions>();
        return options;
    }
}
