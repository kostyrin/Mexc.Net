using Mexc.Net.Enums;

namespace Mexc.Net.Converters;

internal class KlineIntervalConverter : BaseConverter<KlineInterval>
{
    public KlineIntervalConverter() : this(true) { }
    public KlineIntervalConverter(bool quotes) : base(quotes) { }
    protected override List<KeyValuePair<KlineInterval, string>> Mapping => new()
    {
        new KeyValuePair<KlineInterval, string>(KlineInterval.OneMinute, "1m"),
        new KeyValuePair<KlineInterval, string>(KlineInterval.FiveMinutes, "5m"),
        new KeyValuePair<KlineInterval, string>(KlineInterval.FifteenMinutes, "15m"),
        new KeyValuePair<KlineInterval, string>(KlineInterval.ThirtyMinutes, "30m"),
        new KeyValuePair<KlineInterval, string>(KlineInterval.OneHour, "60m"),
        new KeyValuePair<KlineInterval, string>(KlineInterval.FourHour, "4h"),
        new KeyValuePair<KlineInterval, string>(KlineInterval.OneDay, "1d"),
        new KeyValuePair<KlineInterval, string>(KlineInterval.OneMonth, "1M")
    };
}
