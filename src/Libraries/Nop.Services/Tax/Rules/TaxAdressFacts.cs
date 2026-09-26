using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using NRules.Fluent.Dsl;

namespace Nop.Services.Tax.Rules;

[Name("When tax is based on the pickup point and pickup in store is allowed, the selected pickup point is the tax address"), Tag(TaxAddressing.Tag), Priority(1)]
public class PickupPointTaxAddressRule : Rule
{
    public override void Define()
    {
        TaxAddressCandidates candidates = default;

        When()
            .Match<TaxSettings>(s => s.TaxBasedOnPickupPointAddress)
            .Match<ShippingSettings>(s => s.AllowPickupInStore)
            .Match(() => candidates,
                c => c.PickupPointAddress != null);

        Then()
            .Do(ctx => ctx.Insert(new TaxAddress { Address = candidates.PickupPointAddress }));
    }
}

[Name("When tax is based on the billing address and the customer has one, it is the tax address"), Tag(TaxAddressing.Tag)]
public class BillingTaxAddressRule : Rule
{
    public override void Define()
    {
        TaxAddressCandidates candidates = default;

        When()
            .Match<TaxSettings>(s => s.TaxBasedOn == TaxBasedOn.BillingAddress)
            .Match<Customer>(c => c.BillingAddressId != null)
            .Match(() => candidates)
            .Not<TaxAddress>();

        Then()
            .Do(ctx => ctx.Insert(new TaxAddress { Address = candidates.BillingAddress }));
    }
}

[Name("When tax is based on the shipping address and the customer has one, it is the tax address"), Tag(TaxAddressing.Tag)]
public class ShippingTaxAddressRule : Rule
{
    public override void Define()
    {
        TaxAddressCandidates candidates = default;

        When()
            .Match<TaxSettings>(s => s.TaxBasedOn == TaxBasedOn.ShippingAddress)
            .Match<Customer>(c => c.ShippingAddressId != null)
            .Match(() => candidates)
            .Not<TaxAddress>();

        Then()
            .Do(ctx => ctx.Insert(new TaxAddress { Address = candidates.ShippingAddress }));
    }
}

[Name("When tax is based on a billing address the customer does not have, the automatically detected country is the tax address"), Tag(TaxAddressing.Tag)]
public class DetectedCountryForBillingTaxAddressRule : Rule
{
    public override void Define()
    {
        TaxAddressCandidates candidates = default;

        When()
            .Match<TaxSettings>(s => s.TaxBasedOn == TaxBasedOn.BillingAddress,
                s => s.AutomaticallyDetectCountry)
            .Match<Customer>(c => c.BillingAddressId == null)
            .Match(() => candidates,
                c => c.DetectedCountry != null)
            .Not<TaxAddress>();

        Then()
            .Do(ctx => ctx.Insert(new TaxAddress { Address = new Address { CreatedOnUtc = DateTime.UtcNow, CountryId = candidates.DetectedCountry.Id } }));
    }
}

[Name("When tax is based on a shipping address the customer does not have, the automatically detected country is the tax address"), Tag(TaxAddressing.Tag)]
public class DetectedCountryForShippingTaxAddressRule : Rule
{
    public override void Define()
    {
        TaxAddressCandidates candidates = default;

        When()
            .Match<TaxSettings>(s => s.TaxBasedOn == TaxBasedOn.ShippingAddress,
                s => s.AutomaticallyDetectCountry)
            .Match<Customer>(c => c.ShippingAddressId == null)
            .Match(() => candidates,
                c => c.DetectedCountry != null)
            .Not<TaxAddress>();

        Then()
            .Do(ctx => ctx.Insert(new TaxAddress { Address = new Address { CreatedOnUtc = DateTime.UtcNow, CountryId = candidates.DetectedCountry.Id } }));
    }
}

[Name("Otherwise the default tax address is the tax address"), Tag(TaxAddressing.Tag), Priority(-1)]
public class DefaultTaxAddressRule : Rule
{
    public override void Define()
    {
        TaxAddressCandidates candidates = default;

        When()
            .Match(() => candidates)
            .Not<TaxAddress>();

        Then()
            .Do(ctx => ctx.Insert(new TaxAddress { Address = candidates.DefaultAddress }));
    }
}