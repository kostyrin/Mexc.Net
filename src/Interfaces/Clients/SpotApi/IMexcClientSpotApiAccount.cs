using System.Threading;
using System.Threading.Tasks;
using CryptoExchange.Net.Objects;
using Mexc.Net.Objects.Models;

namespace Mexc.Net.Interfaces.Clients.SpotApi;

public interface IMexcClientSpotApiAccount
{
    Task<WebCallResult<MexcAccountInfo>> GetAccountInfoAsync(long? receiveWindow = null, CancellationToken ct = default);
}
