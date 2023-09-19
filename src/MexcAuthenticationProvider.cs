using Mexc.Net.Objects;

namespace Mexc.Net;

internal class MexcAuthenticationProvider : AuthenticationProvider
{
    public string ApiKey => _credentials.Key!.GetString();

    public string Passphrase => _credentials.Secret.GetString();

    public MexcAuthenticationProvider(MexcApiCredentials credentials) : base(credentials)
    {
        if (credentials == null || credentials.Secret == null)
            throw new ArgumentException("No valid API credentials provided. Key/Secret needed.");
    }

    public override void AuthenticateRequest(RestApiClient apiClient, Uri uri, HttpMethod method, Dictionary<string, object> providedParameters, bool auth, ArrayParametersSerialization arraySerialization, HttpMethodParameterPosition parameterPosition, out SortedDictionary<string, object> uriParameters, out SortedDictionary<string, object> bodyParameters, out Dictionary<string, string> headers)
    {
        uriParameters = parameterPosition == HttpMethodParameterPosition.InUri ? new SortedDictionary<string, object>(providedParameters) : new SortedDictionary<string, object>();
        bodyParameters = parameterPosition == HttpMethodParameterPosition.InBody ? new SortedDictionary<string, object>(providedParameters) : new SortedDictionary<string, object>();
        headers = new Dictionary<string, string>() { { "X-MEXC-APIKEY", _credentials.Key!.GetString() } };

        if (!auth)
            return;

        var parameters = parameterPosition == HttpMethodParameterPosition.InUri ? uriParameters : bodyParameters;
        parameters.Add("timestamp", GetMillisecondTimestamp(apiClient));

        uri = uri.SetParameters(uriParameters, arraySerialization);
        var signature = SignHMACSHA256(parameterPosition == HttpMethodParameterPosition.InUri ? uri.Query.Replace("?", "") : parameters.ToFormData());
        parameters.Add("signature", signature);
    }

    public string SignWebsocket(string timestamp)
    {
        throw new NotImplementedException();
        //var signtext = timestamp + "GET" + "/users/self/verify";
        //return SignHMACSHA256(signtext, SignOutputType.Base64);
    }
}
