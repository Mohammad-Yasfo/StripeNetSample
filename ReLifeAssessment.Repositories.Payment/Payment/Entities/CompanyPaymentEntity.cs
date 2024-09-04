namespace ReLifeAssessment.Repositories.Payment.Entities;

public class CompanyPaymentEntity
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public byte MethodType { get; set; }
    public bool HasDetails { get; set; }
    public string Details { get; set; }
}