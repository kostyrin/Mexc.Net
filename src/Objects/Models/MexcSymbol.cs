using Mexc.Net.Converters;
using Mexc.Net.Enums;

namespace Mexc.Net.Objects.Models;

/// <summary>
/// Symbol info
/// </summary>
public class MexcSymbol
{
    /// <summary>
    /// The symbol
    /// </summary>
    [JsonProperty("symbol")]
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// The status of the symbol
    /// </summary>
    [JsonConverter(typeof(SymbolStatusConverter))]
    public SymbolStatus Status { get; set; }
    /// <summary>
    /// The base asset
    /// </summary>
    public string BaseAsset { get; set; } = string.Empty;
    /// <summary>
    /// The precision of the base asset
    /// </summary>
    public int BaseAssetPrecision { get; set; }
    /// <summary>
    /// The quote asset
    /// </summary>
    public string QuoteAsset { get; set; } = string.Empty;
    /// <summary>
    /// The precision of the quote asset
    /// </summary>
    [JsonProperty("quotePrecision")]
    public int QuoteAssetPrecision { get; set; }

    /// <summary>
    /// Allowed order types
    /// </summary>
    [JsonProperty(ItemConverterType = typeof(SpotOrderTypeConverter))]
    public IEnumerable<SpotOrderType> OrderTypes { get; set; } = Array.Empty<SpotOrderType>();
    /// <summary>
    /// Ice berg orders allowed
    /// </summary>
    public bool IceBergAllowed { get; set; }
    /// <summary>
    /// Cancel replace allowed
    /// </summary>
    public bool CancelReplaceAllowed { get; set; }
    /// <summary>
    /// Spot trading orders allowed
    /// </summary>
    public bool IsSpotTradingAllowed { get; set; }
    /// <summary>
    /// Trailling stop orders are allowed
    /// </summary>
    public bool AllowTrailingStop { get; set; }
    /// <summary>
    /// Margin trading orders allowed
    /// </summary>
    public bool IsMarginTradingAllowed { get; set; }
    /// <summary>
    /// If OCO(One Cancels Other) orders are allowed
    /// </summary>
    public bool OCOAllowed { get; set; }
    /// <summary>
    /// Whether or not it is allowed to specify the quantity of a market order in the quote asset
    /// </summary>
    [JsonProperty("quoteOrderQtyMarketAllowed")]
    public bool QuoteOrderQuantityMarketAllowed { get; set; }
    /// <summary>
    /// The precision of the base asset fee
    /// </summary>
    [JsonProperty("baseCommissionPrecision")]
    public int BaseFeePrecision { get; set; }
    /// <summary>
    /// The precision of the quote asset fee
    /// </summary>
    [JsonProperty("quoteCommissionPrecision")]
    public int QuoteFeePrecision { get; set; }
    /// <summary>
    /// Permissions types
    /// </summary>
    [JsonProperty(ItemConverterType = typeof(EnumConverter))]
    public IEnumerable<AccountType> Permissions { get; set; } = Array.Empty<AccountType>();
    /// <summary>
    /// Filters for order on this symbol
    /// </summary>
    public IEnumerable<MexcSymbolFilter> Filters { get; set; } = Array.Empty<MexcSymbolFilter>();
}
