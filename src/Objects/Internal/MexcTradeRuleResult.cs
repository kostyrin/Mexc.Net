namespace Mexc.Net.Objects.Internal;

internal class MexcTradeRuleResult
{
    public bool Passed { get; set; }
    public decimal? Quantity { get; set; }
    public decimal? QuoteQuantity { get; set; }
    public decimal? Price { get; set; }
    public decimal? StopPrice { get; set; }
    public string ErrorMessage { get; set; }

    public static MexcTradeRuleResult CreatePassed(decimal? quantity, decimal? quoteQuantity, decimal? price, decimal? stopPrice)
    {
        return new MexcTradeRuleResult
        {
            Passed = true,
            Quantity = quantity,
            Price = price,
            StopPrice = stopPrice,
            QuoteQuantity = quoteQuantity
        };
    }

    public static MexcTradeRuleResult CreateFailed(string message)
    {
        return new MexcTradeRuleResult
        {
            Passed = false,
            ErrorMessage = message
        };
    }
}
