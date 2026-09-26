using Nop.Core.Domain.Directory;

namespace Nop.Services.Catalog.Rules;

public sealed class RoundingRequest
{
    public decimal Price;
    public decimal Cents;
    public RoundingType Type;
}

public sealed class RoundedPrice
{
    public decimal Value;
}

public static class CashRounding
{
    public const string Tag = "CashRounding";
}