namespace Mexc.Net.Objects.Models;

public class MexcPlacedOrder : MexcOrderBase
{
    /// <summary>
    /// The time the order was placed
    /// </summary>
    [JsonProperty("transactTime"), JsonConverter(typeof(DateTimeConverter))]
    public DateTime CreateTime { get; set; }
}
