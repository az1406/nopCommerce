using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using NRules.Fluent.Dsl;

namespace Nop.Services.Orders.Rules;

[Name("An order that is not cancelled can be cancelled"), Tag(OrderOperations.Tag)]
public class OrderCanBeCancelledRule : Rule
{
    public override void Define()
    {
        Order order = default;

        When()
            .Match(() => order,
                o => o.OrderStatus != OrderStatus.Cancelled);

        Then()
            .Do(ctx => ctx.Insert(new OperationAllowed(OrderOperation.Cancel)));
    }
}

[Name("An order that is not cancelled and still awaits payment can be marked as authorized"), Tag(OrderOperations.Tag)]
public class OrderCanBeMarkedAsAuthorizedRule : Rule
{
    public override void Define()
    {
        Order order = default;

        When()
            .Match(() => order,
                o => o.OrderStatus != OrderStatus.Cancelled,
                o => o.PaymentStatus == PaymentStatus.Pending);

        Then()
            .Do(ctx => ctx.Insert(new OperationAllowed(OrderOperation.MarkAsAuthorized)));
    }
}