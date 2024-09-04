using ReLifeAssessment.Application.Payment.Enums;

namespace ReLifeAssessment.Application.Payment.Models
{
    public class PaymentRequest
    {
        public string SourceCardToken { get; set; }
        public PaymentReason Reason { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string CompanyStripeAccountId { get; set; }
        public string Currency { get; set; }
        public Guid TargetCompanyId { get; set; }
        public Guid CompanyPaymentAccountId { get; set; }
        public string NameOnCard { get; set; }
        public string CardNumber { get; set; }
    }
}