using System;
using CryptoExchange.Net;
using Mexc.Net.Clients.SpotApi;
using Mexc.Net.Interfaces.Clients;
using Mexc.Net.Interfaces.Clients.SpotApi;
using Mexc.Net.Objects;
using Mexc.Net.Objects.Options;
using Microsoft.Extensions.Logging;

namespace Mexc.Net.Clients;

public class MexcSocketClient : BaseSocketClient, IMexcSocketClient
{
    public IMexcSocketClientSpotStreams SpotStreams { get; set; }

    /// <summary>
    /// Create a new instance of MexcSocketClientSpot with default options
    /// </summary>
    public MexcSocketClient() : this(MexcSocketOptions.Default)
    {
    }

    /// <summary>
    /// Create a new instance of MexcSocketClientSpot using provided options
    /// </summary>
    /// <param name="options">The options to use for this client</param>
    public MexcSocketClient(MexcSocketOptions options, ILoggerFactory loggerFactory = null) : base(loggerFactory, "Mexc")
    {
        SpotStreams = AddApiClient(new MexcSocketClientSpotStreams(loggerFactory.CreateLogger<MexcSocketClient>(), options));
    }

    #region Common Methods
    /// <summary>
    /// Set default options
    /// </summary>
    /// <param name="optionsDelegate"></param>
    public static void SetDefaultOptions(Action<MexcSocketOptions> optionsDelegate)
    {
        var options = MexcSocketOptions.Default.Copy();
        optionsDelegate(options);
        MexcSocketOptions.Default = options;
    }

    /// <inheritdoc />
    public virtual void SetApiCredentials(MexcApiCredentials credentials)
    {
        SpotStreams.SetApiCredentials(credentials.Copy());
    }

    #endregion
}
