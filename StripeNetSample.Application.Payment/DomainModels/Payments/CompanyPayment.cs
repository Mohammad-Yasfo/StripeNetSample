namespace StripeNetSample.Application.Payment.Models
{
    public class CompanyPayment
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public PaymentMethodType MethodType { get; set; }
        public bool HasDetails { get; set; }
        public PaymentMethodsDetails Details { get; set; }
    }
}