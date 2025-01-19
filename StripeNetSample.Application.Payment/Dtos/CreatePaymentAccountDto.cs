namespace StripeNetSample.Application.Payment.Dtos;

public class CreatePaymentAccountDto
{
    public string Code { get; set; }
    public string Scope { get; set; }
    public Guid CompanyId { get; set; }
}