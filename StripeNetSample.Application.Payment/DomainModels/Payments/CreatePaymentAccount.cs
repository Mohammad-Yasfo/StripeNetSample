namespace StripeNetSample.Application.Payment.Models
{
    public class CreatePaymentAccount
    {
        public string Code { get; set; }

        public string Scope { get; set; }

        public string State { get; set; }
    }
}
