using Mexc.Net.Interfaces.Clients.SpotApi;

namespace Mexc.Net.Clients.SpotApi;

public class MexcClientSpotApiProAccount : IMexcClientSpotApiProAccount
{
    private readonly MexcClientSpotApi _baseClient;

    internal MexcClientSpotApiProAccount(MexcClientSpotApi baseClient)
    {
        _baseClient = baseClient;
    }
}
