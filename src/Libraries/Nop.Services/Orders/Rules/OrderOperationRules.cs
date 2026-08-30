using Nop.Core.Domain.Orders;
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