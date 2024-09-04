namespace ReLifeAssessment.Application.Payment.Models
{
    public class CompanyPaymentAccount
    {
        public Guid CompanyId { get; set; }
        public Guid Id { get; set; }
        public string Scope { get; set; }
        public string CompanyStripeAccountId { get; set; }
        public bool IsActive { get; set; }

        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}