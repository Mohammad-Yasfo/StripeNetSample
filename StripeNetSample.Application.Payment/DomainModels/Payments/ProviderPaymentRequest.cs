namespace StripeNetSample.Application.Payment.Models
{
    public class ProviderPaymentRequest
    {
        public string SourceCardToken { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string CompanyStripeAccountId { get; set; }
        public string Currency { get; set; }
        public Guid BookingId { get; set; }
    }
}
