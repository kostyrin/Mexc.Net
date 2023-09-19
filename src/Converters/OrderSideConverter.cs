﻿using Mexc.Net.Enums;

namespace Mexc.Net.Converters;

internal class OrderSideConverter : BaseConverter<OrderSide>
{
    public OrderSideConverter() : this(true) { }
    public OrderSideConverter(bool quotes) : base(quotes) { }

    protected override List<KeyValuePair<OrderSide, string>> Mapping => new()
    {
        new KeyValuePair<OrderSide, string>(OrderSide.Buy, "BUY"),
        new KeyValuePair<OrderSide, string>(OrderSide.Sell, "SELL")
    };
}
