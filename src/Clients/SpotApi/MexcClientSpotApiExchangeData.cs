using Mexc.Net.Converters;
using Mexc.Net.Enums;
using Mexc.Net.Interfaces.Clients;
using Mexc.Net.Interfaces.Clients.SpotApi;
using Mexc.Net.Objects.Models;

namespace Mexc.Net.Clients.SpotApi;

public class MexcClientSpotApiExchangeData : IMexcClientSpotApiExchangeData
{
    private const string klinesEndpoint = "klines";
    private const string exchangeInfoEndpoint = "exchangeInfo";
    private const string checkTimeEndpoint = "time";
    private const string orderBookEndpoint = "depth";
    private const string averagePriceEndpoint = "avgPrice";
    private const string price24HEndpoint = "ticker/24hr";

    private readonly MexcClientSpotApi _baseClient;

    internal MexcClientSpotApiExchangeData(MexcClientSpotApi baseClient)
    {
        _baseClient = baseClient;
    }

    public async Task<WebCallResult<DateTime>> GetServerTimeAsync(CancellationToken ct = default)
    {
        var result = await _baseClient.SendRequestInternal<MexcCheckTime>(_baseClient.GetUri(checkTimeEndpoint), HttpMethod.Get, ct, ignoreRateLimit: true).ConfigureAwait(false);
        return result.As(result.Data?.ServerTime ?? default);
    }

    public async Task<WebCallResult<IEnumerable<MexcKline>>> GetKlinesAsync(string symbol, KlineInterval interval, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default)
    {
        //symbol.ValidateMexcSymbol();

        var parameters = new Dictionary<string, object>
        {
            { "symbol", symbol },
            { "interval", JsonConvert.SerializeObject(interval, new KlineIntervalConverter(false)) }
        };

        parameters.AddOptionalParameter("startTime", DateTimeConverter.ConvertToSeconds(startTime));
        parameters.AddOptionalParameter("endTime", DateTimeConverter.ConvertToSeconds(endTime));
        parameters.AddOptionalParameter("limit", limit);

        return await _baseClient.SendRequestInternal<IEnumerable<MexcKline>>(_baseClient.GetUri(klinesEndpoint), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
    }

    public async Task<WebCallResult<MexcOrderBook>> GetOrderBookAsync(string symbol, int? limit = null, CancellationToken ct = default)
    {
        //symbol.ValidateMexcSymbol();
        limit?.ValidateIntBetween(nameof(limit), 1, 5000);
        var parameters = new Dictionary<string, object> { { "symbol", symbol } };
        parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
        var requestWeight = limit == null ? 1 : limit <= 100 ? 1 : limit <= 500 ? 5 : limit <= 1000 ? 10 : 50;
        var result = await _baseClient.SendRequestInternal<MexcOrderBook>(_baseClient.GetUri(orderBookEndpoint), HttpMethod.Get, ct, parameters, weight: requestWeight).ConfigureAwait(false);
        if (result)
            result.Data.Symbol = symbol;
        return result;
    }

    public Task<WebCallResult<MexcExchangeInfo>> GetExchangeInfoAsync(CancellationToken ct = default)
        => GetExchangeInfoAsync(Array.Empty<string>(), ct);

    public Task<WebCallResult<MexcExchangeInfo>> GetExchangeInfoAsync(string symbol, CancellationToken ct = default)
        => GetExchangeInfoAsync(new string[] { symbol }, ct);

    public async Task<WebCallResult<MexcExchangeInfo>> GetExchangeInfoAsync(string[] symbols, CancellationToken ct = default)
    {
        var parameters = new Dictionary<string, object>();

        if (symbols.Length > 1)
        {
            parameters.Add("symbols", string.Join(",", symbols));
        }
        else if (symbols.Any())
        {
            parameters.Add("symbol", symbols.First());
        }

        var exchangeInfoResult = await _baseClient.SendRequestInternal<MexcExchangeInfo>(_baseClient.GetUri(exchangeInfoEndpoint), HttpMethod.Get, ct, parameters: parameters, arraySerialization: ArrayParametersSerialization.Array, weight: 10).ConfigureAwait(false);
        if (!exchangeInfoResult)
            return exchangeInfoResult;

        _baseClient.ExchangeInfo = exchangeInfoResult.Data;
        _baseClient.LastExchangeInfoUpdate = DateTime.UtcNow;
        return exchangeInfoResult;
    }

    public async Task<WebCallResult<MexcAveragePrice>> GetCurrentAvgPriceAsync(string symbol, CancellationToken ct = default)
    {
        //symbol.ValidateMexcSymbol();
        var parameters = new Dictionary<string, object> { { "symbol", symbol } };

        return await _baseClient.SendRequestInternal<MexcAveragePrice>(_baseClient.GetUri(averagePriceEndpoint), HttpMethod.Get, ct, parameters).ConfigureAwait(false);
    }

    public async Task<WebCallResult<IMexcTick>> GetTickerAsync(string symbol, CancellationToken ct = default)
    {
        //symbol.ValidateMexcSymbol();
        var parameters = new Dictionary<string, object> { { "symbol", symbol } };

        var result = await _baseClient.SendRequestInternal<Mexc24HPrice>(_baseClient.GetUri(price24HEndpoint), HttpMethod.Get, ct, parameters, weight: 1).ConfigureAwait(false);
        return result.As<IMexcTick>(result.Data);
    }

    public async Task<WebCallResult<IEnumerable<IMexcTick>>> GetTickersAsync(IEnumerable<string> symbols, CancellationToken ct = default)
    {
        //foreach (var symbol in symbols)
        //    symbol.ValidateMexcSymbol();

        var parameters = new Dictionary<string, object> { { "symbols", string.Join(",", symbols) } };
        var symbolCount = symbols.Count();
        var weight = symbolCount <= 20 ? 1 : symbolCount <= 100 ? 20 : 40;
        var result = await _baseClient.SendRequestInternal<IEnumerable<Mexc24HPrice>>(_baseClient.GetUri(price24HEndpoint), HttpMethod.Get, ct, parameters, weight: weight).ConfigureAwait(false);
        return result.As<IEnumerable<IMexcTick>>(result.Data);
    }

    public async Task<WebCallResult<IEnumerable<IMexcTick>>> GetTickersAsync(CancellationToken ct = default)
    {
        var result = await _baseClient.SendRequestInternal<IEnumerable<Mexc24HPrice>>(_baseClient.GetUri(price24HEndpoint), HttpMethod.Get, ct, weight: 40).ConfigureAwait(false);
        return result.As<IEnumerable<IMexcTick>>(result.Data);
    }
}
