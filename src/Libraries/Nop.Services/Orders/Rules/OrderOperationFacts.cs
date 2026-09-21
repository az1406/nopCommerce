using NRules;
using NRules;

namespace Nop.Services.Orders.Rules;

public enum OrderOperation
{
    Cancel,
    MarkAsAuthorized,
    MarkAsPaid,
    Capture
}

public sealed class OperationAllowed(OrderOperation operation)
{
    public readonly OrderOperation Operation = operation;
}

public sealed class GatewayProbeRequired(OrderOperation operation)
{
    public readonly OrderOperation Operation = operation;
}

public sealed class GatewaySupports(OrderOperation operation)
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

public static class GatewayProbes
{
    public static async Task<bool> AllowsOnceGatewayAnswersAsync(this ISession session,
        OrderOperation operation, Func<Task<bool>> gatewaySupportsIt)
    {
        if (session.Query<GatewayProbeRequired>().Any(probe => probe.Operation == operation)
            && await gatewaySupportsIt())
        {
            session.Insert(new GatewaySupports(operation));
            session.Fire();
        }

        return session.Allows(operation);
    }
}