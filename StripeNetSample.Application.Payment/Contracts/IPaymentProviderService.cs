using StripeNetSample.Application.Payment.Models;

namespace StripeNetSample.Application.Payment.Contracts;

public interface IPaymentProviderService
{
    Task<TransactionProviderResult> Pay(ProviderPaymentRequest model);

    Task<string> GetLinkAccountUrl(Guid venueId, string redirectUri);

    Task<PaymentAccountResult> AuthorizeAccount(CreatePaymentAccount paymentAccount);

    Task DeauthorizeAccount(string venueStripeAccountId);
}