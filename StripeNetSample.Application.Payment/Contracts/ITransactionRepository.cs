using StripeNetSample.Application.Payment.Models;

namespace StripeNetSample.Application.Payment.Contracts;

public interface ITransactionRepository
{
    Task<Transaction> AddAsync(Transaction transaction);

    Task UpdateAsync(Transaction transaction);
}