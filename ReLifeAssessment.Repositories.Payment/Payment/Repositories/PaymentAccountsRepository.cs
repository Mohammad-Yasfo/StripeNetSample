using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ReLifeAssessment.Application.Payment.Contracts;
using ReLifeAssessment.Application.Payment.Models;
using ReLifeAssessment.Repositories.Payment.Entities;

namespace ReLifeAssessment.Repositories.Payment.Repositories;

/// <summary>
/// Repository class for managing payment accounts and company payments.
/// </summary>
public class PaymentAccountsRepository : IPaymentAccountsRepository
{
    private readonly PaymentDbContext _context;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="PaymentAccountsRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="mapper">The AutoMapper instance.</param>
    public PaymentAccountsRepository(PaymentDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// Adds a new payment account to the repository.
    /// </summary>
    /// <param name="paymentAccount">The payment account to add.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task AddAsync(CompanyPaymentAccount paymentAccount)
    {
        var paymentAccountEntity = _mapper.Map<CompanyPaymentAccountEntity>(paymentAccount);
        _context.CompaniesPaymentAccount.Add(paymentAccountEntity);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Removes a payment account from the repository by its ID.
    /// </summary>
    /// <param name="id">The ID of the payment account to remove.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task RemoveAsync(Guid id)
    {
        var companyAccountInfoEntity = await _context.CompaniesPaymentAccount
            .FirstAsync(a => a.CompanyId == id);

        _context.CompaniesPaymentAccount.Remove(companyAccountInfoEntity);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Retrieves a payment account by its ID.
    /// </summary>
    /// <param name="id">The ID of the payment account to retrieve.</param>
    /// <returns>The payment account, or null if not found.</returns>
    public async Task<CompanyPaymentAccount> GetAsync(Guid id)
    {
        var companyAccountInfoEntity = await _context.CompaniesPaymentAccount
            .FirstOrDefaultAsync(a => a.CompanyId == id && a.IsActive);

        var accountInfo = _mapper.Map<CompanyPaymentAccount>(companyAccountInfoEntity);
        return accountInfo;
    }

    /// <summary>
    /// Deactivates a payment account by its ID.
    /// </summary>
    /// <param name="id">The ID of the payment account to deactivate.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task DeActivateAccountAsync(Guid id)
    {
        var companyAccountInfoEntity = await _context.CompaniesPaymentAccount
            .FirstAsync(a => a.Id == id);

        companyAccountInfoEntity.IsActive = false;
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Activates a payment account by its ID and updates related information.
    /// </summary>
    /// <param name="id">The ID of the payment account to activate.</param>
    /// <param name="companyStripeAccountId">The Stripe account ID to associate with the payment account.</param>
    /// <param name="createdBy">The ID of the user who created the account.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task ActivateAccountAsync(Guid id, string companyStripeAccountId, Guid createdBy)
    {
        var companyAccountInfoEntity = await _context.CompaniesPaymentAccount
            .FirstAsync(a => a.Id == id);

        companyAccountInfoEntity.CompanyStripeAccountId = companyStripeAccountId;
        companyAccountInfoEntity.UpdatedAt = DateTime.UtcNow;
        companyAccountInfoEntity.CreatedBy = createdBy;

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Retrieves company payment information by company ID and payment method type.
    /// </summary>
    /// <param name="companyId">The ID of the company.</param>
    /// <param name="methodType">The type of payment method.</param>
    /// <returns>The company payment, or null if not found.</returns>
    public async Task<CompanyPayment> GetCompanyPaymentAsync(Guid companyId, PaymentMethodType methodType)
    {
        var companyPaymentEntity = await _context.CompaniesPayment
            .FirstOrDefaultAsync(p => p.CompanyId == companyId && p.MethodType == (byte)methodType);

        if (companyPaymentEntity == null)
            return null;

        var companyPayment = _mapper.Map<CompanyPayment>(companyPaymentEntity);
        return companyPayment;
    }

    /// <summary>
    /// Adds a new company payment to the repository.
    /// </summary>
    /// <param name="companyPayment">The company payment to add.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task AddCompanyPaymentAsync(CompanyPayment companyPayment)
    {
        var companyPaymentEntity = _mapper.Map<CompanyPaymentEntity>(companyPayment);
        await _context.CompaniesPayment.AddAsync(companyPaymentEntity);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Updates an existing company payment in the repository.
    /// </summary>
    /// <param name="companyPayment">The company payment with updated information.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task UpdateCompanyPaymentAsync(CompanyPayment companyPayment)
    {
        var companyPaymentEntity = await _context.CompaniesPayment
            .FirstAsync(p => p.CompanyId == companyPayment.CompanyId && p.MethodType == (byte)companyPayment.MethodType);

        companyPaymentEntity.HasDetails = companyPayment.HasDetails;
        companyPaymentEntity.Details = JsonConvert.SerializeObject(companyPayment.Details);

        await _context.SaveChangesAsync();
    }
}