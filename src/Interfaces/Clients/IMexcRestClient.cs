using CryptoExchange.Net.Interfaces;
using Mexc.Net.Interfaces.Clients.SpotApi;
using Mexc.Net.Objects;

namespace Mexc.Net.Interfaces.Clients;

public interface IMexcRestClient : IRestClient
{
    /// <summary>
    /// Unified API endpoints
    /// </summary>
    IMexcClientSpotApi SpotApi { get; }

    /// <summary>
    /// Set the API credentials for this client. All Api clients in this client will use the new credentials, regardless of earlier set options.
    /// </summary>
    /// <param name="credentials">The credentials to set</param>
    void SetApiCredentials(MexcApiCredentials credentials);
}
