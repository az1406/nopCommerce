using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;

namespace Nop.Services.Tax.Rules;

public sealed class TaxAddressCandidates
{
    public Address PickupPointAddress;
    public Country DetectedCountry;
    public Address BillingAddress;
    public Address ShippingAddress;
    public Address DefaultAddress;
}

public sealed class TaxAddress
{
    public Address Address;
}

public static class TaxAddressing
{
    public const string Tag = "TaxAddressing";
}