namespace StripeNetSample.Application.Payment.Enums
{
    public enum PaymentStatus : byte
    {
        Succeeded,  // Customer’s payment succeeded
        PaymentFailed,  // Customer’s payment was declined by card network or otherwise expired
        Processing,  // The customer’s payment was submitted to Stripe successfully. Only applicable to payment methods with delayed success confirmation.
        AmountCapturableUpdated  // Customer’s payment is authorized and ready for capture
    }
}