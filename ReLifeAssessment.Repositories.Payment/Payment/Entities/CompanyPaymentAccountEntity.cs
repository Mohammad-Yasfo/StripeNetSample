namespace ReLifeAssessment.Repositories.Payment.Entities;

public class CompanyPaymentAccountEntity
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public string Scope { get; set; }
    public string CompanyStripeAccountId { get; set; }
    public bool IsActive { get; set; }

    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid UpdatedBy { get; set; }
}