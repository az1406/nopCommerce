namespace Nop.Services.Orders.Rules;

public sealed class UnitPriceRequest
{
    public int Quantity;
    public int CartQuantity;
    public decimal AttributesTotalPrice;
}

public sealed class FinalPriceInput
{
    public decimal? OverriddenPrice;
    public decimal AdditionalCharge;
    public int Quantity;
}

public static class UnitPricing
{
    public const string Tag = "UnitPricing";
}