using AutoMapper;
using Microsoft.Extensions.Logging;
using StripeNetSample.Application.Payment.Configuration;
using StripeNetSample.Application.Payment.Contracts;
using StripeNetSample.Application.Payment.Dtos;
using StripeNetSample.Application.Payment.Enums;
using StripeNetSample.Application.Payment.Models;

namespace StripeNetSample.Application.Payment.Services
{
    /// <summary>
    /// Service class for managing payment operations including account linking, payment method updates, and transactions.
    /// </summary>
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentProviderService _paymentProviderService;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IPaymentAccountsRepository _paymentRepository;
        private readonly WebAppsConfiguration _webAppsConfiguration;
        private readonly ILogger<PaymentService> _logger;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentService"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="paymentProviderService">The payment provider service.</param>
        /// <param name="transactionRepository">The transaction repository.</param>
        /// <param name="paymentRepository">The payment accounts repository.</param>
        /// <param name="webAppsConfiguration">The web apps configuration.</param>
        public PaymentService(
            ILogger<PaymentService> logger,
            IMapper mapper,
            IPaymentProviderService paymentProviderService,
            ITransactionRepository transactionRepository,
            IPaymentAccountsRepository paymentRepository,
            WebAppsConfiguration webAppsConfiguration)
        {
            _paymentProviderService = paymentProviderService;
            _transactionRepository = transactionRepository;
            _paymentRepository = paymentRepository;
            _webAppsConfiguration = webAppsConfiguration;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Generates a URL for linking a payment account for a given company.
        /// </summary>
        /// <param name="companyId">The ID of the company.</param>
        /// <returns>A task representing the asynchronous operation, with a URL as the result.</returns>
        /// <exception cref="Exception">Thrown if the company already has a connected Stripe account or if the URL creation fails.</exception>
        public async Task<string> GetLinkAccountUrl(Guid companyId)
        {
            var accountInfo = await _paymentRepository.GetAsync(companyId);

            if (accountInfo != null && !string.IsNullOrEmpty(accountInfo.CompanyStripeAccountId))
            {
                throw new Exception("The company already has a connected Stripe account.");
            }

            try
            {
                string redirectUri = $"{_webAppsConfiguration.CompaniesAppUrl}/payments/link-company-payment-account";
                var url = await _paymentProviderService.GetLinkAccountUrl(companyId, redirectUri);
                return url;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to create authorization link.", ex);
            }
        }

        /// <summary>
        /// Retrieves the allowed payment methods for a company.
        /// </summary>
        /// <param name="id">The ID of the company.</param>
        /// <returns>A task representing the asynchronous operation, with an <see cref="AllowedPaymentMethodsDto"/> as the result.</returns>
        public async Task<AllowedPaymentMethodsDto> GetPaymentMethodAsync(Guid id)
        {
            try
            {
                var allowedPaymentMethods = new AllowedPaymentMethodsDto();
                var companyPayment = await _paymentRepository.GetCompanyPaymentAsync(id, PaymentMethodType.BankAccount);

                if (companyPayment != null && companyPayment.Details != null)
                {
                    allowedPaymentMethods.HasBankDetails = companyPayment.HasDetails;
                    allowedPaymentMethods.MethodDetails = _mapper.Map<PaymentMethodsDetailsDto>(companyPayment.Details);
                }
                else
                {
                    allowedPaymentMethods.HasBankDetails = false;
                }

                return allowedPaymentMethods;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve payment method.");
                throw;
            }
        }

        /// <summary>
        /// Updates the payment methods for a company.
        /// </summary>
        /// <param name="id">The ID of the company.</param>
        /// <param name="methodsDto">The payment methods details to update.</param>
        /// <returns>A task representing the asynchronous operation, with an <see cref="AllowedPaymentMethodsDto"/> as the result.</returns>
        /// <exception cref="Exception">Thrown if no payment methods are specified or if the company does not have a connected Stripe account.</exception>
        public async Task<AllowedPaymentMethodsDto> UpdatePaymentMethodAsync(Guid id, AllowedPaymentMethodsDto methodsDto)
        {
            if (!methodsDto.CanPayLater && !methodsDto.CanPayNow)
                throw new Exception("At least one payment method must be specified.");

            var accountInfo = await _paymentRepository.GetAsync(id);

            if ((accountInfo == null || string.IsNullOrEmpty(accountInfo.CompanyStripeAccountId)) && methodsDto.CanPayNow)
            {
                throw new Exception("The company does not have a connected Stripe account.");
            }

            await AddOrUpdateCompanyPaymentAsync(id, methodsDto);
            var result = await GetPaymentMethodAsync(id);

            return result;
        }

        /// <summary>
        /// Adds or updates the company payment details.
        /// </summary>
        /// <param name="id">The ID of the company.</param>
        /// <param name="methodsDto">The payment methods details.</param>
        private async Task AddOrUpdateCompanyPaymentAsync(Guid id, AllowedPaymentMethodsDto methodsDto)
        {
            var companyPayment = await _paymentRepository.GetCompanyPaymentAsync(id, PaymentMethodType.BankAccount);

            if (companyPayment == null)
            {
                if (methodsDto.MethodDetails != null)
                {
                    companyPayment = new CompanyPayment
                    {
                        Id = Guid.NewGuid(),
                        CompanyId = id,
                        MethodType = PaymentMethodType.BankAccount,
                        HasDetails = methodsDto.HasBankDetails,
                        Details = _mapper.Map<PaymentMethodsDetails>(methodsDto.MethodDetails)
                    };

                    await _paymentRepository.AddCompanyPaymentAsync(companyPayment);
                }
            }
            else
            {
                if (companyPayment.HasDetails == methodsDto.HasBankDetails && methodsDto.MethodDetails == null)
                    return;

                companyPayment.HasDetails = methodsDto.HasBankDetails;

                if (methodsDto.MethodDetails != null)
                {
                    if (companyPayment.Details == null)
                        companyPayment.Details = new PaymentMethodsDetails();

                    companyPayment.Details.BankName = string.IsNullOrEmpty(methodsDto.MethodDetails.BankName) ?
                        companyPayment.Details.BankName : methodsDto.MethodDetails.BankName;

                    companyPayment.Details.AccountHolder = string.IsNullOrEmpty(methodsDto.MethodDetails.AccountHolder) ?
                        companyPayment.Details.AccountHolder : methodsDto.MethodDetails.AccountHolder;

                    companyPayment.Details.AccountNumber = string.IsNullOrEmpty(methodsDto.MethodDetails.AccountNumber) ?
                        companyPayment.Details.AccountNumber : methodsDto.MethodDetails.AccountNumber;

                    companyPayment.Details.SortCode = string.IsNullOrEmpty(methodsDto.MethodDetails.SortCode) ?
                        companyPayment.Details.SortCode : methodsDto.MethodDetails.SortCode;

                    companyPayment.Details.IBAN = string.IsNullOrEmpty(methodsDto.MethodDetails.IBAN) ?
                        companyPayment.Details.IBAN : methodsDto.MethodDetails.IBAN;

                    companyPayment.Details.Swift = string.IsNullOrEmpty(methodsDto.MethodDetails.Swift) ?
                        companyPayment.Details.Swift : methodsDto.MethodDetails.Swift;

                    companyPayment.Details.Address = string.IsNullOrEmpty(methodsDto.MethodDetails.Address) ?
                        companyPayment.Details.Address : methodsDto.MethodDetails.Address;
                }

                await _paymentRepository.UpdateCompanyPaymentAsync(companyPayment);
            }
        }

        /// <summary>
        /// Authorizes a payment account for a company.
        /// </summary>
        /// <param name="paymentAccount">The payment account details to authorize.</param>
        /// <returns>A task representing the asynchronous operation, with a boolean indicating success or failure.</returns>
        /// <exception cref="Exception">Thrown if the company already has a connected Stripe account or if authorization fails.</exception>
        public async Task<bool> AuthorizeAccount(CreatePaymentAccountDto paymentAccount)
        {
            PaymentAccountResult accountResult;
            CompanyPaymentAccount accountInfo;

            try
            {
                accountInfo = await _paymentRepository.GetAsync(paymentAccount.CompanyId);

                if (accountInfo != null && !string.IsNullOrEmpty(accountInfo.CompanyStripeAccountId))
                {
                    throw new Exception("The company already has a connected Stripe account.");
                }

                var providerPaymentAccount = new CreatePaymentAccount
                {
                    Code = paymentAccount.Code,
                    State = paymentAccount.CompanyId.ToString(),
                    Scope = paymentAccount.Scope
                };

                accountResult = await _paymentProviderService.AuthorizeAccount(providerPaymentAccount);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to authorize account with the payment provider.", ex);
            }

            try
            {
                if (string.IsNullOrEmpty(accountResult.CompanyStripeAccountId))
                {
                    throw new Exception("Cannot link this account because the company Stripe Account ID is null.");
                }
                if (string.IsNullOrEmpty(accountResult.Scope))
                {
                    throw new Exception("Cannot link this account because the company Stripe Account Scope is null.");
                }

                if (accountInfo == null)
                {
                    var companyPaymentAccount = new CompanyPaymentAccount
                    {
                        CompanyId = paymentAccount.CompanyId,
                        CompanyStripeAccountId = accountResult.CompanyStripeAccountId,
                        Scope = accountResult.Scope,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = Guid.NewGuid() // GetCurrentEmployeeId()
                    };

                    await _paymentRepository.AddAsync(companyPaymentAccount);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Failed to authorize Stripe account: {ex}");
                throw;
            }
        }

        /// <summary>
        /// Deauthorizes a payment account for a company.
        /// </summary>
        /// <param name="companyId">The ID of the company.</param>
        /// <returns>A task representing the asynchronous operation, with a boolean indicating success or failure.</returns>
        /// <exception cref="Exception">Thrown if the company does not have a Stripe account or if deauthorization fails.</exception>
        public async Task<bool> DeauthorizeAccount(Guid companyId)
        {
            try
            {
                var accountInfo = await _paymentRepository.GetAsync(companyId);

                if (accountInfo == null || string.IsNullOrEmpty(accountInfo.CompanyStripeAccountId))
                {
                    throw new Exception("The company does not have a Stripe account.");
                }
                if (!accountInfo.IsActive)
                {
                    throw new Exception("The company account is already deactivated.");
                }

                await _paymentProviderService.DeauthorizeAccount(accountInfo.CompanyStripeAccountId);
                await _paymentRepository.DeActivateAccountAsync(accountInfo.Id);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to deauthorize company account: {ex.Message}");
                throw new Exception("Failed to deauthorize account.", ex);
            }
        }

        /// <summary>
        /// Checks the activation state of a company's payment account.
        /// </summary>
        /// <param name="companyId">The ID of the company.</param>
        /// <returns>A task representing the asynchronous operation, with a boolean indicating whether the account is active.</returns>
        public async Task<bool> GetAccountState(Guid companyId)
        {
            var accountInfo = await _paymentRepository.GetAsync(companyId);
            return accountInfo != null &&
                   !string.IsNullOrEmpty(accountInfo.CompanyStripeAccountId) &&
                   !string.IsNullOrEmpty(accountInfo.Scope);
        }

        /// <summary>
        /// Retrieves the Stripe account information for a company.
        /// </summary>
        /// <param name="companyId">The ID of the company.</param>
        /// <returns>A task representing the asynchronous operation, with the <see cref="CompanyPaymentAccount"/> as the result.</returns>
        /// <exception cref="Exception">Thrown if the company does not have a Stripe account.</exception>
        public async Task<CompanyPaymentAccount> GetCompanyStripeAccount(Guid companyId)
        {
            var accountInfo = await _paymentRepository.GetAsync(companyId);
            if (accountInfo == null || string.IsNullOrEmpty(accountInfo.CompanyStripeAccountId))
            {
                throw new Exception("The company does not have a Stripe account.");
            }

            return accountInfo;
        }

        /// <summary>
        /// Processes a payment request for a company.
        /// </summary>
        /// <param name="companyId">The ID of the company.</param>
        /// <param name="paymentRequestDto">The payment request details.</param>
        /// <returns>A task representing the asynchronous operation, with a boolean indicating whether the payment was successful.</returns>
        public async Task<bool> Pay(Guid companyId, PaymentRequestDto paymentRequestDto)
        {
            try
            {
                var companyAccount = await GetCompanyStripeAccount(companyId);

                var paymentRequest = new PaymentRequest
                {
                    SourceCardToken = paymentRequestDto.SourceCardToken,
                    TargetCompanyId = companyId,
                    Reason = PaymentReason.BookingDeposit,
                    Description = "Payment for booking deposit",
                    CompanyStripeAccountId = companyAccount.CompanyStripeAccountId,
                    Amount = 100, // Replace with actual amount
                    CompanyPaymentAccountId = companyAccount.Id,
                    NameOnCard = paymentRequestDto.NameOnCard,
                    CardNumber = paymentRequestDto.CardNumber
                };

                var transaction = await Pay(paymentRequest);
                return transaction.PaymentStatus == PaymentStatus.Succeeded;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Payment processing failed: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Executes a payment request and updates the transaction status.
        /// </summary>
        /// <param name="request">The payment request details.</param>
        /// <returns>A task representing the asynchronous operation, with the <see cref="Transaction"/> as the result.</returns>
        /// <exception cref="Exception">Thrown if transaction creation or payment processing fails.</exception>
        private async Task<Transaction> Pay(PaymentRequest request)
        {
            var newTransaction = new Transaction
            {
                Id = Guid.NewGuid(),
                Amount = request.Amount,
                TransactionStatus = TransactionStatus.Initiated,
                CompanyId = request.TargetCompanyId,
                Description = $"{request.CompanyPaymentAccountId}-{PaymentReason.BookingFees}",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = Guid.NewGuid(), // GetCurrentEmployeeId()
                CompanyPaymentAccountId = request.CompanyPaymentAccountId,
                Currency = request.Currency,
                NameOnCard = request.NameOnCard,
                CardNumber = request.CardNumber
            };

            var transaction = await _transactionRepository.AddAsync(newTransaction) ??
                              throw new Exception("Failed to create transaction.");

            var providerPaymentRequest = new ProviderPaymentRequest
            {
                SourceCardToken = request.SourceCardToken,
                Description = transaction.Description,
                Amount = request.Amount,
                Currency = request.Currency,
                CompanyStripeAccountId = request.CompanyStripeAccountId
            };

            TransactionProviderResult paymentResult;

            try
            {
                paymentResult = await _paymentProviderService.Pay(providerPaymentRequest);
            }
            catch (Exception ex)
            {
                transaction.TransactionStatus = TransactionStatus.Failed;
                await _transactionRepository.UpdateAsync(transaction);
                throw new Exception("Payment processing failed with the payment provider.", ex);
            }

            try
            {
                if (paymentResult == null)
                {
                    transaction.TransactionStatus = TransactionStatus.Failed;
                    transaction.PaymentStatus = PaymentStatus.PaymentFailed;
                }
                else if (paymentResult.Status == PaymentStatus.PaymentFailed)
                {
                    transaction.TransactionStatus = TransactionStatus.Failed;
                    transaction.PaymentStatus = paymentResult.Status;
                    _logger.LogInformation("Payment process failed.");
                }
                else if (paymentResult.Status == PaymentStatus.Succeeded)
                {
                    transaction.ProviderTransactionId = paymentResult.TransactionId;
                    transaction.TransactionStatus = TransactionStatus.Succeeded;
                    transaction.PaymentStatus = paymentResult.Status;
                    _logger.LogInformation("Payment process succeeded.");
                }
                else
                {
                    transaction.TransactionStatus = TransactionStatus.Failed;
                    transaction.PaymentStatus = PaymentStatus.PaymentFailed;
                    _logger.LogInformation("Payment process failed for an unknown reason.");
                }

                transaction.UpdatedAt = DateTime.UtcNow;
                transaction.UpdatedBy = Guid.NewGuid(); // GetCurrentEmployeeId()
                await _transactionRepository.UpdateAsync(transaction);

                return transaction;
            }
            catch (Exception ex)
            {
                transaction.TransactionStatus = TransactionStatus.Failed;
                transaction.UpdatedAt = DateTime.UtcNow;
                transaction.UpdatedBy = Guid.NewGuid(); // GetCurrentEmployeeId()
                await _transactionRepository.UpdateAsync(transaction);

                _logger.LogError($"Transaction update failed: {ex.Message}");
                throw;
            }
        }
    }
}