using StripeNetSample.Application.Payment.Enums;

namespace StripeNetSample.Repositories.Payment.Transaction.Entities
{
    public class TransactionEntity
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public TransactionStatus TransactionStatus { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }
        public string ProviderTransactionId { get; set; }
        public string Currency { get; set; }
        public Guid CompanyId { get; set; }
        public Guid CompanyPaymentAccountId { get; set; }
        public string Description { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string NameOnCard { get; set; }
        public string CardNumber { get; set; }
    }
}