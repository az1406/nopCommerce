using Nop.Core.Domain.Catalog;

namespace Nop.Services.Orders.Rules;

public sealed class SelectedAttribute
{
    public ProductAttributeMapping Mapping;
    public int Index;
}

public sealed class CheckedAttribute
{
    public ProductAttributeMapping Mapping;
    public ProductAttributeValue[] Values;
    public string[] SelectedValues;
    public int[] SelectedValueIds;
    public string Name;
    public string Prompt;
    public int Index;
}

public sealed class BundledProduct
{
    public ProductAttributeValue Value;
    public ProductAttributeMapping Mapping;
    public Product Product;
    public int Index;
}

public sealed class BundleOptions
{
    public bool Ignore;
    public bool IncludeNonCombinable;
}

public sealed class BundledProductIncluded
{
    public BundledProduct Bundle;
}

public sealed class BundledProductWarning
{
    public BundledProduct Bundle;
    public string AttributeName;
    public string ValueName;
    public string Warning;
    public int Index;
}

public sealed class AttributeWarningTexts
{
    public string SelectAttribute;
    public string MinimumLength;
    public string MaximumLength;
    public string AssociatedProductWarning;
}

public sealed class AttributeWarning
{
    public int Order;
    public string Message;
}

public sealed class AttributeErrorStop
{
    public int Order;
}

public static class AttributeWarnings
{
    public const string Tag = "AttributeWarnings";
}