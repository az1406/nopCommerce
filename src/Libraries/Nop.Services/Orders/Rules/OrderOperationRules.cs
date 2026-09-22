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

[Name("An order that cost something, was never refunded before, is paid and whose payment method supports refunding can be refunded"), Tag(OrderOperations.Tag)]
public class OrderCanBeRefundedRule : Rule
{
    public override void Define()
    {
        Order order = default;

        When()
            .Match(() => order,
                o => o.OrderTotal != decimal.Zero,
                o => o.RefundedAmount == decimal.Zero,
                o => o.PaymentStatus == PaymentStatus.Paid)
            .Match<IPaymentMethod>(paymentMethod => paymentMethod.SupportRefund);

        Then()
            .Do(ctx => ctx.Insert(new OperationAllowed(OrderOperation.Refund)));
    }
}

[Name("An order that cost something, still has an unrefunded balance covering the requested amount, is paid or partially refunded and whose payment method supports partial refunds can be partially refunded"), Tag(OrderOperations.Tag)]
public class OrderCanBePartiallyRefundedRule : Rule
{
    public override void Define()
    {
        Order order = default;

        When()
            .Match(() => order,
                o => o.OrderTotal != decimal.Zero,
                o => o.OrderTotal - o.RefundedAmount > decimal.Zero,
                o => o.PaymentStatus == PaymentStatus.Paid || o.PaymentStatus == PaymentStatus.PartiallyRefunded)
            .Match<RefundRequest>(request => request.Amount <= order.OrderTotal - order.RefundedAmount)
            .Match<IPaymentMethod>(paymentMethod => paymentMethod.SupportPartiallyRefund);

        Then()
            .Do(ctx => ctx.Insert(new OperationAllowed(OrderOperation.PartialRefund)));
    }
}

[Name("An order that cost something, was never refunded before and is paid can be refunded offline"), Tag(OrderOperations.Tag)]
public class OrderCanBeRefundedOfflineRule : Rule
{
    public override void Define()
    {
        Order order = default;

        When()
            .Match(() => order,
                o => o.OrderTotal != decimal.Zero,
                o => o.RefundedAmount == decimal.Zero,
                o => o.PaymentStatus == PaymentStatus.Paid);

        Then()
            .Do(ctx => ctx.Insert(new OperationAllowed(OrderOperation.RefundOffline)));
    }
}

[Name("An order that cost something, still has an unrefunded balance covering the requested amount and is paid or partially refunded can be partially refunded offline"), Tag(OrderOperations.Tag)]
public class OrderCanBePartiallyRefundedOfflineRule : Rule
{
    public override void Define()
    {
        Order order = default;

        When()
            .Match(() => order,
                o => o.OrderTotal != decimal.Zero,
                o => o.OrderTotal - o.RefundedAmount > decimal.Zero,
                o => o.PaymentStatus == PaymentStatus.Paid || o.PaymentStatus == PaymentStatus.PartiallyRefunded)
            .Match<RefundRequest>(request => request.Amount <= order.OrderTotal - order.RefundedAmount);

        Then()
            .Do(ctx => ctx.Insert(new OperationAllowed(OrderOperation.PartialRefundOffline)));
    }
}