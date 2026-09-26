using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using NRules.Fluent.Dsl;

namespace Nop.Services.Orders.Rules;

[Name("An attribute combination with an overridden price is priced from that price and the item's own quantity"), Tag(UnitPricing.Tag)]
public class CombinationOverriddenPriceRule : Rule
{
    public override void Define()
    {
        ProductAttributeCombination combination = default;
        UnitPriceRequest request = default;

        When()
            .Match(() => combination,
                c => c.OverriddenPrice.HasValue)
            .Match(() => request);

        Then()
            .Do(ctx => ctx.Insert(new FinalPriceInput
            {
                OverriddenPrice = combination.OverriddenPrice,
                AdditionalCharge = decimal.Zero,
                Quantity = request.Quantity
            }));
    }
}

[Name("A product the customer does not price, without an overridden combination price, already in the cart while tier prices are grouped, is priced from its attributes and the cart quantity"), Tag(UnitPricing.Tag)]
public class AttributesPriceForCartQuantityRule : Rule
{
    public override void Define()
    {
        Product product = default;
        UnitPriceRequest request = default;

        When()
            .Match(() => product,
                p => !p.CustomerEntersPrice)
            .Not<ProductAttributeCombination>(c => c.OverriddenPrice.HasValue)
            .Match<ShoppingCartSettings>(s => s.GroupTierPricesForDistinctShoppingCartItems)
            .Match(() => request,
                r => r.CartQuantity > 0);

        Then()
            .Do(ctx => ctx.Insert(new FinalPriceInput
            {
                OverriddenPrice = null,
                AdditionalCharge = request.AttributesTotalPrice,
                Quantity = request.CartQuantity
            }));
    }
}

[Name("A product the customer does not price, without an overridden combination price, is otherwise priced from its attributes and the item's own quantity"), Tag(UnitPricing.Tag)]
public class AttributesPriceForItemQuantityRule : Rule
{
    public override void Define()
    {
        Product product = default;
        ShoppingCartSettings settings = default;
        UnitPriceRequest request = default;

        When()
            .Match(() => product,
                p => !p.CustomerEntersPrice)
            .Not<ProductAttributeCombination>(c => c.OverriddenPrice.HasValue)
            .Match(() => settings)
            .Match(() => request,
                r => !settings.GroupTierPricesForDistinctShoppingCartItems || r.CartQuantity == 0);

        Then()
            .Do(ctx => ctx.Insert(new FinalPriceInput
            {
                OverriddenPrice = null,
                AdditionalCharge = request.AttributesTotalPrice,
                Quantity = request.Quantity
            }));
    }
}