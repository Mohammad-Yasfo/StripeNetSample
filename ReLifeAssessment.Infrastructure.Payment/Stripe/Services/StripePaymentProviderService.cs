using ReLifeAssessment.Application.Payment.Contracts;
using ReLifeAssessment.Application.Payment.Enums;
using ReLifeAssessment.Application.Payment.Models;
using Stripe;
using StripeConfig = ReLifeAssessment.Infrastructure.Payment.Stripe.Configuration.StripeConfiguration;

namespace ReLifeAssessment.Infrastructure.Payment.Stripe.Services
{
    /// <summary>
    /// Service class for handling Stripe payment provider operations.
    /// </summary>
    public class StripePaymentProviderService : IPaymentProviderService
    {
        #region Properties

        private readonly StripeConfig _stripeConfiguration;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="StripePaymentProviderService"/> class.
        /// </summary>
        /// <param name="stripeConfiguration">The Stripe configuration settings.</param>
        public StripePaymentProviderService(StripeConfig stripeConfiguration)
        {
            _stripeConfiguration = stripeConfiguration ?? throw new ArgumentNullException(nameof(stripeConfiguration));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Retrieves the publishable key for Stripe API.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the publishable key.</returns>
        public Task<string> GetPublishableKey()
        {
            try
            {
                return Task.FromResult(_stripeConfiguration.PublishableKey);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the publishable key.", ex);
            }
        }

        /// <summary>
        /// Generates a URL for linking a Stripe account with the provided redirect URI.
        /// </summary>
        /// <param name="companyId">The company ID.</param>
        /// <param name="redirectUri">The redirect URI after authorization.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the link account URL.</returns>
        public Task<string> GetLinkAccountUrl(Guid companyId, string redirectUri)
        {
            var url = $"{_stripeConfiguration.OAuthBaseLink}?response_type=code&client_id={_stripeConfiguration.ClientId}&scope=read_write&redirect_uri={redirectUri}&state={companyId}";
            return Task.FromResult(url);
        }

        /// <summary>
        /// Authorizes a Stripe account using the provided authorization code.
        /// </summary>
        /// <param name="paymentAccount">The payment account information.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the payment account result.</returns>
        public Task<PaymentAccountResult> AuthorizeAccount(CreatePaymentAccount paymentAccount)
        {
            try
            {
                StripeConfiguration.ApiKey = _stripeConfiguration.SecretKey;

                var options = new OAuthTokenCreateOptions
                {
                    GrantType = "authorization_code",
                    Code = paymentAccount.Code,
                };

                var service = new OAuthTokenService();
                var token = service.Create(options);

                var accountResult = new PaymentAccountResult
                {
                    Scope = token.Scope,
                    CompanyStripeAccountId = token.StripeUserId,
                };

                return Task.FromResult(accountResult);
            }
            catch (StripeException ex)
            {
                string errorMessage = StripeExceptionHandler.GetMessage(ex);
                throw new Exception($"Authorization failed: {errorMessage}");
            }
        }

        /// <summary>
        /// Deauthorizes a Stripe account for a given company.
        /// </summary>
        /// <param name="companyStripeAccountId">The Stripe account ID of the company to be deauthorized.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task DeauthorizeAccount(string companyStripeAccountId)
        {
            try
            {
                StripeConfiguration.ApiKey = _stripeConfiguration.SecretKey;

                var options = new OAuthDeauthorizeOptions
                {
                    ClientId = _stripeConfiguration.ClientId,
                    StripeUserId = companyStripeAccountId,
                };

                var service = new OAuthTokenService();
                service.Deauthorize(options);

                return Task.CompletedTask;
            }
            catch (StripeException ex)
            {
                string errorMessage = StripeExceptionHandler.GetMessage(ex);
                throw new Exception($"Failed to deauthorize the account: {errorMessage}");
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deauthorizing the account.", ex);
            }
        }

        /// <summary>
        /// Processes a payment through Stripe.
        /// </summary>
        /// <param name="model">The payment request model.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the transaction provider result.</returns>
        public Task<TransactionProviderResult> Pay(ProviderPaymentRequest model)
        {
            try
            {
                StripeConfiguration.ApiKey = _stripeConfiguration.SecretKey;

                var payOptions = new ChargeCreateOptions
                {
                    Source = model.SourceCardToken,
                    Description = model.Description,
                    Currency = model.Currency,
                    Amount = (long)model.Amount * 100, // Stripe expects the amount in cents.
                    Capture = true,
                    Metadata = new Dictionary<string, string>
                    {
                        { "BookingId", model.BookingId.ToString() },
                    }
                };

                var payService = new ChargeService();
                var requestOptions = new RequestOptions
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
                    StatusCode = charge.StripeResponse.StatusCode,
                    Status = charge.Status == "succeeded" ? PaymentStatus.Succeeded : PaymentStatus.PaymentFailed
                };

                return Task.FromResult(result);
            }
            catch (StripeException ex)
            {
                string errorMessage = $"Payment processing failed: {StripeExceptionHandler.GetMessage(ex)}";
                throw new Exception(errorMessage);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred during payment processing.", ex);
            }
        }

        #endregion
    }
}