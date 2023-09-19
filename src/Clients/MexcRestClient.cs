using Mexc.Net.Clients.SpotApi;
using Mexc.Net.Interfaces.Clients;
using Mexc.Net.Interfaces.Clients.SpotApi;
using Mexc.Net.Objects;
using Mexc.Net.Objects.Options;
using Microsoft.Extensions.Options;

namespace Mexc.Net.Clients;

public class MexcRestClient : BaseRestClient, IMexcRestClient
{
    /// <inheritdoc />
    public IMexcClientSpotApi SpotApi { get; }

    /// <summary>
    /// Create a new instance of the OKXRestClient using provided options
    /// </summary>
    /// <param name="optionsDelegate">Option configuration delegate</param>
    public MexcRestClient(Action<MexcRestOptions> optionsDelegate) : this(null, null, optionsDelegate)
    {
    }

    /// <summary>
    /// Create a new instance of the OKXRestClient using default options
    /// </summary>
    public MexcRestClient(ILoggerFactory? loggerFactory = null, HttpClient? httpClient = null) : this(httpClient, loggerFactory, null)
    {
    }

    public MexcRestClient(HttpClient? httpClient, ILoggerFactory? loggerFactory, Action<MexcRestOptions>? optionsDelegate = null)
        : base(loggerFactory, "Mexc")
    {
        var options = MexcRestOptions.Default.Copy();
        if (optionsDelegate != null)
            optionsDelegate(options);
        Initialize(options);

        SpotApi = AddApiClient(new MexcClientSpotApi(_logger, httpClient, options));
    }

    #region Common Methods
    /// <summary>
    /// Sets the default options to use for new clients
    /// </summary>
    /// <param name="optionsDelegate">Callback for setting the options</param>
    public static void SetDefaultOptions(Action<MexcRestOptions> optionsDelegate)
    {
        var options = MexcRestOptions.Default.Copy();
        optionsDelegate(options);
        MexcRestOptions.Default = options;
    }

    /// <summary>
    /// Sets the API Credentials
    /// </summary>
    /// <param name="credentials">API Credentials Object</param>
    public void SetApiCredentials(MexcApiCredentials credentials)
    {
        SpotApi.SetApiCredentials(credentials);
    }
    #endregion
}
