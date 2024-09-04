using ReLifeAssessment.Application.Payment.Contracts;
using ReLifeAssessment.Application.Payment.Enums;
using ReLifeAssessment.Application.Payment.Models;
using Stripe;
using StripeConfig = ReLifeAssessment.Infrastructure.Payment.Stripe.Configuration.StripeConfiguration;

namespace ReLifeAssessment.Infrastructure.Payment.Stripe.Services;

public class StripePaymentProviderService : IPaymentProviderService
{
    #region Properties
    private StripeConfig stripeConfiguration;
    #endregion

    #region Constuctor
    public StripePaymentProviderService(StripeConfig StripeConfiguration)
    {
        this.stripeConfiguration = StripeConfiguration;
    }
    #endregion

    #region Methods

    public Task<string> GetPublishableKey()
    {
        try
        {
            return Task.FromResult(stripeConfiguration.PublishableKey);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public Task<string> GetLinkAccountUrl(Guid companyId, string redirectUri)
    {
        string url = stripeConfiguration.OAuthBaseLink + "?" + "response_type=code"
                       + "&" + "client_id=" + stripeConfiguration.ClientId
                       + "&" + "scope=read_write"
                       + "&" + $"redirect_uri={redirectUri}"
                       + "&" + "state=" + companyId.ToString();

        return Task.FromResult(url);
    }

    public Task<PaymentAccountResult> AuthorizeAccount(CreatePaymentAccount paymentAccount)
    {
        try
        {

            StripeConfiguration.ApiKey = stripeConfiguration.SecretKey;

            var options = new OAuthTokenCreateOptions
            {
                GrantType = "authorization_code",
                Code = paymentAccount.Code,
            };

            var service = new OAuthTokenService();

            var token = service.Create(options);

            var accountResult = new PaymentAccountResult()
            {
                Scope = token.Scope,
                CompanyStripeAccountId = token.StripeUserId,
            };
            return Task.FromResult(accountResult);

        }
        catch (StripeException ex)
        {
            string errorMessage = StripeExceptionHandler.GetMessage(ex);

            throw new Exception($"{errorMessage}");
        }
    }

    public Task DeauthorizeAccount(string companyStripeAccountId)
    {

        StripeConfiguration.ApiKey = stripeConfiguration.SecretKey;

        var options = new OAuthDeauthorizeOptions
        {
            ClientId = stripeConfiguration.ClientId,
            StripeUserId = companyStripeAccountId,
        };

        var service = new OAuthTokenService();

        service.Deauthorize(options);

        return Task.CompletedTask;
    }

    public Task<TransactionProviderResult> Pay(ProviderPaymentRequest model)
    {
        try
        {
            StripeConfiguration.ApiKey = stripeConfiguration.SecretKey;

            var payOptions = new ChargeCreateOptions()
            {
                Source = model.SourceCardToken,
                Description = model.Description,
                Currency = model.Currency,
                Amount = (long)model.Amount * 100,
                Capture = true,
                Metadata = new Dictionary<string, string>()
                    {
                        { "BookingId" ,model.BookingId.ToString() },
                    }
            };

            var payService = new ChargeService();
            var requestOptions = new RequestOptions()
            {
                StripeAccount = model.CompanyStripeAccountId
            };

            var charge = payService.Create(payOptions, requestOptions);

            var result = new TransactionProviderResult
            {
                TransactionId = charge.BalanceTransactionId,
                Captured = charge.Captured,
                Amount = charge.Amount,
                Currency = charge.Currency,
                StatusCode = charge.StripeResponse.StatusCode
            };
            if (charge.Status == "succeeded")
            {
                result.Status = PaymentStatus.Succeeded;
            }
            else
            {
                result.Status = PaymentStatus.PaymentFailed;
            }
            return Task.FromResult(result);
        }
        catch (StripeException ex)
        {
            string errorMessage = "Failed to complete the payment with error: " + StripeExceptionHandler.GetMessage(ex);
            throw new Exception(errorMessage);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    #endregion
}