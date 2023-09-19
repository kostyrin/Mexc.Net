using System.Text;

namespace Mexc.Net.Objects;

/// <summary>
/// Credentials for the Mexc API
/// </summary>
public class MexcApiCredentials : ApiCredentials
{
    /// <summary>
    /// Passphrase
    /// </summary>
    public SecureString PassPhrase { get; }

    /// <summary>
    /// Creates new api credentials. Keep this information safe.
    /// </summary>
    /// <param name="apiKey">The API key</param>
    /// <param name="apiSecret">The API secret</param>
    public MexcApiCredentials(string apiKey, string apiSecret) : base(apiKey, apiSecret)
    {  }

    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="apiKey"></param>
    /// <param name="apiSecret"></param>
    /// <param name="apiPassPhrase"></param>
    public MexcApiCredentials(string apiKey, string apiSecret, string apiPassPhrase) : base(apiKey, apiSecret)
    {
        PassPhrase = apiPassPhrase.ToSecureString();
    }

    /// <summary>
    /// Create Api credentials providing a stream containing json data. The json data should include three values: apiKey, apiSecret and apiPassPhrase
    /// </summary>
    /// <param name="inputStream">The stream containing the json data</param>
    /// <param name="identifierKey">A key to identify the credentials for the API. For example, when set to `binanceKey` the json data should contain a value for the property `binanceKey`. Defaults to 'apiKey'.</param>
    /// <param name="identifierSecret">A key to identify the credentials for the API. For example, when set to `binanceSecret` the json data should contain a value for the property `binanceSecret`. Defaults to 'apiSecret'.</param>
    /// <param name="identifierPassPhrase">A key to identify the credentials for the API. For example, when set to `MexcPass` the json data should contain a value for the property `MexcPass`. Defaults to 'apiPassPhrase'.</param>
    public MexcApiCredentials(Stream inputStream, string? identifierKey = null, string? identifierSecret = null) : base(inputStream, identifierKey, identifierSecret)
    {
        string? pass;
        using (var reader = new StreamReader(inputStream, Encoding.ASCII, false, 512, true))
        {
            var stringData = reader.ReadToEnd();
            var jsonData = JToken.Parse(stringData);
            pass = TryGetValue(jsonData, identifierKey ?? "apiPassPhrase");
        }

        inputStream.Seek(0, SeekOrigin.Begin);
    }

    /// <inheritdoc />
    public override ApiCredentials Copy()
    {
        return new MexcApiCredentials(Key!.GetString(), Secret!.GetString());
    }
}
