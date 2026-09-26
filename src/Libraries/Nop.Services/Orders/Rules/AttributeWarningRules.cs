using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using NRules.Fluent.Dsl;

namespace Nop.Services.Orders.Rules;

[Name("A selected attribute that belongs to another product is an attribute error"), Tag(AttributeWarnings.Tag)]
public class ForeignAttributeRule : Rule
{
    public override void Define()
    {
        Product product = default;
        SelectedAttribute attribute = default;

        When()
            .Match(() => product)
            .Match(() => attribute,
                a => a.Mapping.ProductId != 0,
                a => a.Mapping.ProductId != product.Id);

        Then()
            .Do(ctx => ctx.Insert(new AttributeWarning { Order = attribute.Index, Message = "Attribute error" }));
    }
}

[Name("A selected attribute that belongs to no product is an attribute error after which nothing else is checked"), Tag(AttributeWarnings.Tag)]
public class AbortingAttributeRule : Rule
{
    public override void Define()
    {
        SelectedAttribute attribute = default;

        When()
            .Match(() => attribute,
                a => a.Mapping.ProductId == 0);

        Then()
            .Do(ctx => ctx.Insert(new AttributeWarning { Order = attribute.Index, Message = "Attribute error" }))
            .Do(ctx => ctx.Insert(new AttributeErrorStop { Order = attribute.Index }));
    }
}

[Name("A required attribute without a non-blank selected value is missing"), Tag(AttributeWarnings.Tag)]
public class RequiredValueBlankRule : Rule
{
    public override void Define()
    {
        CheckedAttribute attribute = default;

        When()
            .Match(() => attribute,
                a => a.Mapping.IsRequired,
                a => !a.SelectedValues.Any(value => !string.IsNullOrEmpty(value.Trim())));

        Then()
            .Do(ctx => ctx.Insert(new AttributeWarning { Order = 1000 + attribute.Index * 10, Message = attribute.Prompt }));
    }
}

[Name("A required attribute with predefined values is missing when none of its values was selected"), Tag(AttributeWarnings.Tag)]
public class RequiredValueUnknownRule : Rule
{
    public override void Define()
    {
        CheckedAttribute attribute = default;

        When()
            .Match(() => attribute,
                a => a.Mapping.IsRequired,
                a => a.SelectedValues.Any(value => !string.IsNullOrEmpty(value.Trim())),
                a => a.Mapping.ShouldHaveValues(),
                a => a.Values.Any(),
                a => !a.Values.Any(value => a.SelectedValues.Contains(value.Id.ToString())));

        Then()
            .Do(ctx => ctx.Insert(new AttributeWarning { Order = 1000 + attribute.Index * 10, Message = attribute.Prompt }));
    }
}

[Name("A read-only checkbox attribute whose selected values differ from the pre-selected ones was changed"), Tag(AttributeWarnings.Tag)]
public class ReadOnlyValuesChangedRule : Rule
{
    public override void Define()
    {
        CheckedAttribute attribute = default;

        When()
            .Match(() => attribute,
                a => a.Mapping.AttributeControlType == AttributeControlType.ReadonlyCheckboxes,
                a => !CommonHelper.ArraysEqual(a.Values.Where(value => value.IsPreSelected).Select(value => value.Id).ToArray(), a.SelectedValueIds));

        Then()
            .Do(ctx => ctx.Insert(new AttributeWarning { Order = 1000 + attribute.Index * 10 + 1, Message = "You cannot change read-only values" }));
    }
}

[Name("A text attribute whose entered text is shorter than its minimum length is too short"), Tag(AttributeWarnings.Tag)]
public class TextTooShortRule : Rule
{
    public override void Define()
    {
        CheckedAttribute attribute = default;
        AttributeWarningTexts texts = default;

        When()
            .Match(() => texts)
            .Match(() => attribute,
                a => a.Mapping.ValidationRulesAllowed(),
                a => a.Mapping.AttributeControlType == AttributeControlType.TextBox || a.Mapping.AttributeControlType == AttributeControlType.MultilineTextbox,
                a => a.Mapping.ValidationMinLength > a.SelectedValues.Take(1).Sum(value => value.Length));

        Then()
            .Do(ctx => ctx.Insert(new AttributeWarning
            {
                Order = 2000 + attribute.Index * 10,
                Message = string.Format(texts.MinimumLength, attribute.Name, attribute.Mapping.ValidationMinLength.Value)
            }));
    }
}

[Name("A text attribute whose entered text is longer than its maximum length is too long"), Tag(AttributeWarnings.Tag)]
public class TextTooLongRule : Rule
{
    public override void Define()
    {
        CheckedAttribute attribute = default;
        AttributeWarningTexts texts = default;

        When()
            .Match(() => texts)
            .Match(() => attribute,
                a => a.Mapping.ValidationRulesAllowed(),
                a => a.Mapping.AttributeControlType == AttributeControlType.TextBox || a.Mapping.AttributeControlType == AttributeControlType.MultilineTextbox,
                a => a.Mapping.ValidationMaxLength < a.SelectedValues.Take(1).Sum(value => value.Length));

        Then()
            .Do(ctx => ctx.Insert(new AttributeWarning
            {
                Order = 2000 + attribute.Index * 10 + 1,
                Message = string.Format(texts.MaximumLength, attribute.Name, attribute.Mapping.ValidationMaxLength.Value)
            }));
    }
}

[Name("A bundled product selected through a combinable attribute is checked when nothing else is wrong and bundles are not ignored"), Tag(AttributeWarnings.Tag), Priority(-1)]
public class IncludedCombinableBundleRule : Rule
{
    public override void Define()
    {
        BundledProduct bundle = default;

        When()
            .Match(() => bundle,
                b => b.Mapping != null,
                b => !b.Mapping.IsNonCombinable())
            .Match<BundleOptions>(o => !o.Ignore)
            .Not<AttributeWarning>(warning => warning.Order < 3000);

        Then()
            .Do(ctx => ctx.Insert(new BundledProductIncluded { Bundle = bundle }));
    }
}

[Name("A bundled product selected through a non-combinable attribute is checked when such attributes are not ignored, nothing else is wrong and bundles are not ignored"), Tag(AttributeWarnings.Tag), Priority(-1)]
public class IncludedNonCombinableBundleRule : Rule
{
    public override void Define()
    {
        BundledProduct bundle = default;

        When()
            .Match(() => bundle,
                b => b.Mapping != null,
                b => b.Mapping.IsNonCombinable())
            .Match<BundleOptions>(o => !o.Ignore,
                o => o.IncludeNonCombinable)
            .Not<AttributeWarning>(warning => warning.Order < 3000);

        Then()
            .Do(ctx => ctx.Insert(new BundledProductIncluded { Bundle = bundle }));
    }
}

[Name("A checked bundled product that cannot be loaded is reported"), Tag(AttributeWarnings.Tag)]
public class BundledProductMissingRule : Rule
{
    public override void Define()
    {
        BundledProductIncluded included = default;

        When()
            .Match(() => included,
                i => i.Bundle.Product == null);

        Then()
            .Do(ctx => ctx.Insert(new AttributeWarning
            {
                Order = 3000 + included.Bundle.Index * 100,
                Message = string.Format("Associated product cannot be loaded - {0}", included.Bundle.Value.AssociatedProductId)
            }));
    }
}

[Name("A warning of a checked bundled product is reported under its attribute and value"), Tag(AttributeWarnings.Tag)]
public class BundledProductWarningRule : Rule
{
    public override void Define()
    {
        BundledProductWarning warning = default;
        AttributeWarningTexts texts = default;

        When()
            .Match(() => texts)
            .Match(() => warning);

        Then()
            .Do(ctx => ctx.Insert(new AttributeWarning
            {
                Order = 3000 + warning.Bundle.Index * 100 + 1 + warning.Index,
                Message = string.Format(texts.AssociatedProductWarning, warning.AttributeName, warning.ValueName, warning.Warning)
            }));
    }
}