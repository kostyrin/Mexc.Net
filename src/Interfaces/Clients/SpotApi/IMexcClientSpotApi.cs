using CryptoExchange.Net.Interfaces.CommonClients;
using Mexc.Net.Objects.Models;

namespace Mexc.Net.Interfaces.Clients.SpotApi;

public interface IMexcClientSpotApi : IRestApiClient, IDisposable
{  
    /// <summary>
    /// Endpoints related to account settings, info or actions
    /// </summary>
    IMexcClientSpotApiAccount Account { get; }

    /// <summary>
    /// Endpoints related to retrieving market and system data
    /// </summary>
    IMexcClientSpotApiExchangeData ExchangeData { get; }

    /// <summary>
    /// Endpoints related to orders and trades
    /// </summary>
    IMexcClientSpotApiTrading Trading { get; }

    /// <summary>
    /// Endpoints related to orders and trades from Pro Account (High Frequency)
    /// </summary>
    IMexcClientSpotApiProAccount ProAccount { get; }

    /// <summary>
    /// Get the ISpotClient for this client. This is a common interface which allows for some basic operations without knowing any details of the exchange.
    /// </summary>
    /// <returns></returns>
    public ISpotClient CommonSpotClient { get; }

    MexcExchangeInfo ExchangeInfo { get; set; }
}
