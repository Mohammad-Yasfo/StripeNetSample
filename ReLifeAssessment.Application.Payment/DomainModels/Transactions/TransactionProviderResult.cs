using ReLifeAssessment.Application.Payment.Enums;
using System.Net;

namespace ReLifeAssessment.Application.Payment.Models;

public class TransactionProviderResult
{
    public PaymentStatus Status { get; set; }
    public string Currency { get; set; }
    public string TransactionId { get; set; }
    public bool Captured { get; set; }
    public long Amount { get; set; }
    public HttpStatusCode StatusCode { get; set; }
}