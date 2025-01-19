namespace StripeNetSample.Application.Payment.Models
{
    public class PaymentMethodsDetails
    {
        public string BankName { get; set; }
        public string AccountHolder { get; set; }
        public string AccountNumber { get; set; }
        public string SortCode { get; set; }
        public string IBAN { get; set; }
        public string Swift { get; set; }
        public string Address { get; set; }
    }
}
