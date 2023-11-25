using Newtonsoft.Json;

namespace Mexc.Net.Objects.Core;

internal class MexcSocketResponse
{
    public bool Success
    {
        get
        {
            return
                string.IsNullOrEmpty(ErrorCode)
                || ErrorCode?.Trim() == "0";
        }
    }

    [JsonProperty("event")]
    public string? Event { get; set; }

    [JsonProperty("code")]
    public string? ErrorCode { get; set; }

    [JsonProperty("msg")]
    public string? ErrorMessage { get; set; }
}

internal class MexcSocketUpdateResponse<T> : MexcSocketResponse
{
    [JsonProperty("data")]
    public T Data { get; set; } = default!;
}

//internal class MexcOrderBookUpdate
//{
//    [JsonProperty("action")]
//    public string? Action { get; set; }

//    [JsonProperty("data")]
//    public IEnumerable<MexcOrderBook> Data { get; set; } = default!;
//}
