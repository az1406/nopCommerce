using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using NRules.Fluent.Dsl;
using Nop.Services.Payments;

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

[Name("An order that is neither cancelled nor pending, whose payment is authorized and whose payment method supports capturing can be captured"), Tag(OrderOperations.Tag)]
public class OrderCanBeCapturedRule : Rule
{
    public override void Define()
    {
        Order order = default;

        When()
            .Match(() => order,
                o => o.OrderStatus != OrderStatus.Cancelled,
                o => o.OrderStatus != OrderStatus.Pending,
                o => o.PaymentStatus == PaymentStatus.Authorized)
            .Match<IPaymentMethod>(paymentMethod => paymentMethod.SupportCapture);

        Then()
            .Do(ctx => ctx.Insert(new OperationAllowed(OrderOperation.Capture)));
    }
}

[Name("An order that cost something, whose payment is authorized and whose payment method supports voiding can be voided"), Tag(OrderOperations.Tag)]
public class OrderCanBeVoidedRule : Rule
{
    public override void Define()
    {
        Order order = default;

        When()
            .Match(() => order,
                o => o.OrderTotal != decimal.Zero,
                o => o.PaymentStatus == PaymentStatus.Authorized)
            .Match<IPaymentMethod>(paymentMethod => paymentMethod.SupportVoid);

        Then()
            .Do(ctx => ctx.Insert(new OperationAllowed(OrderOperation.Void)));
    }
}