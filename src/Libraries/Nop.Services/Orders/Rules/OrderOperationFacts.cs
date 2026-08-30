using NRules;

namespace Nop.Services.Orders.Rules;

public enum OrderOperation
{
    Cancel
}

public sealed class OperationAllowed(OrderOperation operation)
{
    public readonly OrderOperation Operation = operation;
}

public static class OrderOperations
{
    public const string Tag = "OrderOperations";

    public static bool Allows(this ISession session, OrderOperation operation)
    {
        return session.Query<OperationAllowed>().Any(allowed => allowed.Operation == operation);
    }
}