using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;
using NRules.Fluent.Dsl;

namespace Nop.Services.Common.Rules;

[Name("An address with an enabled, required field left empty is invalid"), Tag(AddressValidation.Tag)]
public class RequiredFieldMissingRule : Rule
{
    public override void Define()
    {
        When()
            .Match<RequiredField>(f => f.Enabled,
                f => f.Required,
                f => string.IsNullOrWhiteSpace(f.Value));

        Then()
            .Do(ctx => ctx.Insert(new AddressInvalid()));
    }
}

[Name("An address without a known country is invalid when the country is enabled"), Tag(AddressValidation.Tag)]
public class CountryMissingRule : Rule
{
    public override void Define()
    {
        When()
            .Match<AddressSettings>(s => s.CountryEnabled)
            .Not<Country>();

        Then()
            .Do(ctx => ctx.Insert(new AddressInvalid()));
    }
}

[Name("An address whose country has states is invalid when the state is enabled and none of those states is selected"), Tag(AddressValidation.Tag)]
public class StateProvinceMissingRule : Rule
{
    public override void Define()
    {
        Address address = default;

        When()
            .Match<AddressSettings>(s => s.CountryEnabled,
                s => s.StateProvinceEnabled)
            .Match<Country>()
            .Exists<StateProvince>()
            .Match(() => address)
            .Not<StateProvince>(s => s.Id == address.StateProvinceId);

        Then()
            .Do(ctx => ctx.Insert(new AddressInvalid()));
    }
}

[Name("An address with a required custom attribute left empty is invalid"), Tag(AddressValidation.Tag)]
public class RequiredAttributeMissingRule : Rule
{
    public override void Define()
    {
        When()
            .Match<RequiredAttributeValue>(v => string.IsNullOrEmpty(v.Value));

        Then()
            .Do(ctx => ctx.Insert(new AddressInvalid()));
    }
}