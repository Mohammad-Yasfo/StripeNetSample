namespace ReLifeAssessment.Application.Payment.Dtos;

public class PaymentRequestDto
{
    public string SourceCardToken { get; set; }

    public Guid BookingId { get; set; }

    public string NameOnCard { get; set; }

    public string CardNumber { get; set; }
}