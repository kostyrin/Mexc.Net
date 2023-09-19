namespace Mexc.Net.Objects.Models;

public class MexcCombinedStream<T>
{
    /// <summary>
    /// The stream combined
    /// </summary>
    [JsonProperty("c")]
    public string Stream { get; set; } = string.Empty;

    /// <summary>
    /// The data of stream
    /// </summary>
    [JsonProperty("d")]
    public T Data { get; set; } = default!;

    [JsonProperty("s")]
    public string Symbol { get; set; } = default!;

    [JsonProperty("t"), JsonConverter(typeof(DateTimeConverter))]
    public DateTime EventTime { get; set; } = default!;
}
