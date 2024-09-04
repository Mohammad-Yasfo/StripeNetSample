using ReLifeAssessment.Application.Payment.Dtos;
using ReLifeAssessment.Application.Payment.Models;

namespace ReLifeAssessment.Application.Payment.Contracts;

public interface IPaymentService
{
    Task<string> GetLinkAccountUrl(Guid venueId);
    Task<AllowedPaymentMethodsDto> GetPaymentMethodAsync(Guid Id);
    Task<AllowedPaymentMethodsDto> UpdatePaymentMethodAsync(Guid Id, AllowedPaymentMethodsDto methodsDto);
    Task<bool> AuthorizeAccount(CreatePaymentAccountDto paymentAccount);
    Task<bool> DeauthorizeAccount(Guid VenueId);
    Task<bool> GetAccountState(Guid venueId);
    Task<CompanyPaymentAccount> GetCompanyStripeAccount(Guid companyId);
    Task<bool> Pay(Guid companyId, PaymentRequestDto paymentRequestDto);
}