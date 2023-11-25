using System.Collections.Generic;
using CryptoExchange.Net.Converters;
using Mexc.Net.Enums;

namespace Mexc.Net.Converters;

internal class InstrumentTypeConverter : BaseConverter<MexcInstrumentType>
{
    public InstrumentTypeConverter() : this(true) { }
    public InstrumentTypeConverter(bool quotes) : base(quotes) { }

    protected override List<KeyValuePair<MexcInstrumentType, string>> Mapping => new List<KeyValuePair<MexcInstrumentType, string>>
    {
        new KeyValuePair<MexcInstrumentType, string>(MexcInstrumentType.Any, "ANY"),
        new KeyValuePair<MexcInstrumentType, string>(MexcInstrumentType.Spot, "SPOT"),
        new KeyValuePair<MexcInstrumentType, string>(MexcInstrumentType.Margin, "MARGIN"),
        new KeyValuePair<MexcInstrumentType, string>(MexcInstrumentType.Swap, "SWAP"),
        new KeyValuePair<MexcInstrumentType, string>(MexcInstrumentType.Futures, "FUTURES"),
        new KeyValuePair<MexcInstrumentType, string>(MexcInstrumentType.Option, "OPTION"),
        new KeyValuePair<MexcInstrumentType, string>(MexcInstrumentType.Contracts, "CONTRACTS"),
    };
}