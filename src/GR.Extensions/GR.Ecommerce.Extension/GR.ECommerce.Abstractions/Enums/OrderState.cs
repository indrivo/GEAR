namespace GR.ECommerce.Abstractions.Enums
{
    public enum OrderState
    {
        New, 
        OnHold,
        PendingPayment,
        PaymentReceived,
        PaymentFailed,
        Invoiced,
        Shipping,
        Shipped,
        Complete,
        Canceled,
        Refunded,
        Closed
    }
}
