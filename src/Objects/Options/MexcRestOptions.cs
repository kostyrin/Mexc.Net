using System;
using CryptoExchange.Net.Objects.Options;

namespace Mexc.Net.Objects.Options;

/// <summary>
/// Rest client options
/// </summary>
public class MexcRestOptions : RestExchangeOptions<MexcEnvironment, MexcApiCredentials>
{
    /// <summary>
    /// Default options for new MexcRestClients
    /// </summary>
    public static MexcRestOptions Default { get; set; } = new MexcRestOptions()
    {
        Environment = MexcEnvironment.Live
    };

    /// <summary>
    /// The default receive window for requests
    /// </summary>
    public TimeSpan ReceiveWindow { get; set; } = TimeSpan.FromSeconds(5);
    
    /// <summary>
    /// Whether or not to sign public requests
    /// </summary>
    public bool SignPublicRequests { get; set; }

    /// <summary>
    /// Options for the  unified API
    /// </summary>
    public RestApiOptions UnifiedOptions { get; private set; } = new RestApiOptions();

    internal MexcRestOptions Copy()
    {
        var options = Copy<MexcRestOptions>();
        options.SignPublicRequests = SignPublicRequests;
        options.UnifiedOptions = UnifiedOptions.Copy<RestApiOptions>();
        return options;
    }
}
