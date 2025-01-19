namespace StripeNetSample.Application.Payment.Dtos;

public class AllowedPaymentMethodsDto
{
    public bool CanPayNow { get; set; }
    public bool CanPayLater { get; set; }
    public bool HasBankDetails { get; set; }
    public PaymentMethodsDetailsDto MethodDetails { get; set; }
}