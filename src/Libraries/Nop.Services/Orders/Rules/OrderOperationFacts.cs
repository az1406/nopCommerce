using NRules;
using Nop.Services.Payments;

namespace Nop.Services.Orders.Rules;

public enum OrderOperation
{
    Cancel,
    MarkAsAuthorized,
    MarkAsPaid,
    Capture,
    Void,
    Refund,
    PartialRefund
}

public sealed class OperationAllowed(OrderOperation operation)
{
    public readonly OrderOperation Operation = operation;
}

public sealed class RefundRequest(decimal amount)
{
    public readonly decimal Amount = amount;
}

public static class OrderOperations
{
    public const string Tag = "OrderOperations";

    public static bool Allows(this ISession session, OrderOperation operation)
    {
        return session.Query<OperationAllowed>().Any(allowed => allowed.Operation == operation);
    }
}
