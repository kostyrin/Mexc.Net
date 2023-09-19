using Mexc.Net.Enums;

namespace Mexc.Net.Converters;

internal class StreamKlineIntervalConverter : BaseConverter<KlineInterval>
{
    public StreamKlineIntervalConverter() : this(true) { }
    public StreamKlineIntervalConverter(bool quotes) : base(quotes) { }
    protected override List<KeyValuePair<KlineInterval, string>> Mapping => new()
{
    new KeyValuePair<KlineInterval, string>(KlineInterval.OneMinute, "Min1"),
    new KeyValuePair<KlineInterval, string>(KlineInterval.FiveMinutes, "Min5"),
    new KeyValuePair<KlineInterval, string>(KlineInterval.FifteenMinutes, "Min15"),
    new KeyValuePair<KlineInterval, string>(KlineInterval.ThirtyMinutes, "Min30"),
    new KeyValuePair<KlineInterval, string>(KlineInterval.OneHour, "Min60"),
    new KeyValuePair<KlineInterval, string>(KlineInterval.FourHour, "Hour4"),
    new KeyValuePair<KlineInterval, string>(KlineInterval.OneDay, "Day1"),
    new KeyValuePair<KlineInterval, string>(KlineInterval.OneMonth, "Month1")
};
}
