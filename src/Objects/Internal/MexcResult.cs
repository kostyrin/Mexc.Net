using Newtonsoft.Json;

namespace Mexc.Net.Objects.Internal;

internal class MexcResult<T>
{
    [JsonProperty("code")]
    public int Code { get; set; }

    [JsonProperty("msg")]
    public string Message { get; set; }

    public T Data { get; set; } = default!;
}
