using ReLifeAssessment.Application.Payment.Models;

namespace ReLifeAssessment.Application.Payment.Contracts;

public interface IPaymentAccountsRepository
{
    Task AddAsync(CompanyPaymentAccount paymentAccount);
    Task<CompanyPaymentAccount> GetAsync(Guid Id);
    Task RemoveAsync(Guid Id);
    Task DeActivateAccountAsync(Guid Id);
    Task ActivateAccountAsync(Guid accountId, string venueStripeAccountId, Guid createdBy);

    Task<CompanyPayment> GetCompanyPaymentAsync(Guid venueId, PaymentMethodType methodType);
    Task AddCompanyPaymentAsync(CompanyPayment venuePayment);
    Task UpdateCompanyPaymentAsync(CompanyPayment venuePayment);
}