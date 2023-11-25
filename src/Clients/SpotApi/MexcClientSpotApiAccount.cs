using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CryptoExchange.Net.Objects;
using Mexc.Net.Interfaces.Clients.SpotApi;
using Mexc.Net.Objects.Models;

namespace Mexc.Net.Clients.SpotApi;

public class MexcClientSpotApiAccount : IMexcClientSpotApiAccount
{
    private readonly string accountInfoEndpoint = "account";

    private readonly MexcClientSpotApi _baseClient;

    internal MexcClientSpotApiAccount(MexcClientSpotApi baseClient)
    {
        _baseClient = baseClient;
    }

    public async Task<WebCallResult<MexcAccountInfo>> GetAccountInfoAsync(long? receiveWindow = null, CancellationToken ct = default)
    {
        var parameters = new Dictionary<string, object>();
        //parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? _baseClient.Options.ReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

        return await _baseClient.SendRequestInternal<MexcAccountInfo>(_baseClient.GetUri(accountInfoEndpoint), HttpMethod.Get, ct, parameters, true, weight: 10).ConfigureAwait(false);
    }
}