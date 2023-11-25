using System;
using System.Collections.Generic;
using CryptoExchange.Net.Converters;
using Mexc.Net.Converters;
using Mexc.Net.Enums;
using Newtonsoft.Json;

namespace Mexc.Net.Objects.Core;

internal class MexcSocketMessage
{
    [JsonProperty("op")]
    public string Operation { get; set; } = string.Empty;

    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("args")]
    public IEnumerable<object> Args { get; set; } = Array.Empty<object>();
}

internal class MexcSocketRequest
{
    [JsonProperty("op"), JsonConverter(typeof(MexcSocketOperationConverter))]
    public MexcSocketOperation Operation { get; set; }

    [JsonProperty("args")]
    public List<MexcSocketRequestArgument> Arguments { get; set; } = new List<MexcSocketRequestArgument>();

    public MexcSocketRequest(MexcSocketOperation op, MexcSocketRequestArgument argument)
    {
        Operation = op;
        Arguments.Add(argument);
    }
}

internal class MexcSocketRequestArgument
{
    [JsonProperty("channel")]
    public string Channel { get; set; } = string.Empty;

    [JsonProperty("instFamily", NullValueHandling = NullValueHandling.Ignore)]
    public string? InstrumentFamily { get; set; }

    [JsonProperty("instId", NullValueHandling = NullValueHandling.Ignore)]
    public string? Symbol { get; set; }

    [JsonProperty("instType", NullValueHandling = NullValueHandling.Ignore), JsonConverter(typeof(InstrumentTypeConverter))]
    public MexcInstrumentType? InstrumentType { get; set; }

    [JsonProperty("ccy", NullValueHandling = NullValueHandling.Ignore)]
    public string? Asset { get; set; }

    [JsonProperty("algoId", NullValueHandling = NullValueHandling.Ignore)]
    public string? AlgoId { get; set; }

    [JsonProperty("extraParams", NullValueHandling = NullValueHandling.Ignore)]
    public string? ExtraParams { get; set; }
}

internal class MexcSocketAuthRequest
{
    [JsonProperty("op"), JsonConverter(typeof(MexcSocketOperationConverter))]
    public MexcSocketOperation Operation { get; set; }

    [JsonProperty("args")]
    public List<MexcSocketAuthRequestArgument> Arguments { get; set; } = new List<MexcSocketAuthRequestArgument>();

    public MexcSocketAuthRequest(MexcSocketOperation op, MexcSocketAuthRequestArgument argument)
    {
        Operation = op;
        Arguments.Add(argument);
    }
}

internal class MexcSocketAuthRequestArgument
{
    [JsonProperty("apiKey", NullValueHandling = NullValueHandling.Ignore)]
    public string? ApiKey { get; set; }

    [JsonProperty("passphrase", NullValueHandling = NullValueHandling.Ignore)]
    public string? Passphrase { get; set; }

    [JsonProperty("timestamp", NullValueHandling = NullValueHandling.Ignore)]
    public string? Timestamp { get; set; }

    [JsonProperty("sign", NullValueHandling = NullValueHandling.Ignore)]
    public string? Signature { get; set; }
}

internal enum MexcSocketOperation
{
    Subscribe,
    Unsubscribe,
    Login,
}

internal class MexcSocketOperationConverter : BaseConverter<MexcSocketOperation>
{
    public MexcSocketOperationConverter() : this(true) { }
    public MexcSocketOperationConverter(bool quotes) : base(quotes) { }

    protected override List<KeyValuePair<MexcSocketOperation, string>> Mapping => new List<KeyValuePair<MexcSocketOperation, string>>
    {
        new KeyValuePair<MexcSocketOperation, string>(MexcSocketOperation.Subscribe, "subscribe"),
        new KeyValuePair<MexcSocketOperation, string>(MexcSocketOperation.Unsubscribe, "unsubscribe"),
        new KeyValuePair<MexcSocketOperation, string>(MexcSocketOperation.Login, "login"),
    };
}
