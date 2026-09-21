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

[Name("An order that is not cancelled and whose payment is not yet settled can be marked as paid"), Tag(OrderOperations.Tag)]
public class OrderCanBeMarkedAsPaidRule : Rule
{
    public override void Define()
    {
        Order order = default;

        When()
            .Match(() => order,
                o => o.OrderStatus != OrderStatus.Cancelled,
                o => o.PaymentStatus != PaymentStatus.Paid,
                o => o.PaymentStatus != PaymentStatus.Refunded,
                o => o.PaymentStatus != PaymentStatus.Voided);

        Then()
            .Do(ctx => ctx.Insert(new OperationAllowed(OrderOperation.MarkAsPaid)));
    }
}

[Name("Capturing needs the gateway asked when the order is neither cancelled nor pending and its payment is authorized"), Tag(OrderOperations.Tag)]
public class CaptureNeedsGatewayAskedRule : Rule
{
    public override void Define()
    {
        Order order = default;

        When()
            .Match(() => order,
                o => o.OrderStatus != OrderStatus.Cancelled,
                o => o.OrderStatus != OrderStatus.Pending,
                o => o.PaymentStatus == PaymentStatus.Authorized);

        Then()
            .Do(ctx => ctx.Insert(new GatewayProbeRequired(OrderOperation.Capture)));
    }
}

[Name("An order whose capture was worth asking about and whose gateway supports capturing can be captured"), Tag(OrderOperations.Tag)]
public class OrderCanBeCapturedRule : Rule
{
    public override void Define()
    {
        When()
            .Exists<GatewayProbeRequired>(probe => probe.Operation == OrderOperation.Capture)
            .Exists<GatewaySupports>(support => support.Operation == OrderOperation.Capture);

        Then()
            .Do(ctx => ctx.Insert(new OperationAllowed(OrderOperation.Capture)));
    }
}