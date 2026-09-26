namespace Nop.Services.Common.Rules;

public sealed class RequiredField
{
    public bool Enabled;
    public bool Required;
    public string Value;
}

public sealed class RequiredAttributeValue
{
    public string Value;
}

public sealed class AddressInvalid
{
}

public static class AddressValidation
{
    public const string Tag = "AddressValidation";
}

