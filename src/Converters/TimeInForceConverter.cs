﻿using System.Collections.Generic;
using CryptoExchange.Net.Converters;
using Mexc.Net.Enums;

namespace Mexc.Net.Converters;

internal class TimeInForceConverter : BaseConverter<TimeInForce>
{
    public TimeInForceConverter() : this(true) { }
    public TimeInForceConverter(bool quotes) : base(quotes) { }

    protected override List<KeyValuePair<TimeInForce, string>> Mapping => new List<KeyValuePair<TimeInForce, string>>
    {
        new KeyValuePair<TimeInForce, string>(TimeInForce.GoodTillCanceled, "GTC"),
        new KeyValuePair<TimeInForce, string>(TimeInForce.ImmediateOrCancel, "IOC"),
        new KeyValuePair<TimeInForce, string>(TimeInForce.FillOrKill, "FOK"),
    };
}