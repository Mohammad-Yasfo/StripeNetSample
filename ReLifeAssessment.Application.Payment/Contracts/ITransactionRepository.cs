using ReLifeAssessment.Application.Payment.Models;

namespace ReLifeAssessment.Application.Payment.Contracts;

public interface ITransactionRepository
{
    Task<Transaction> AddAsync(Transaction transaction);

    Task UpdateAsync(Transaction transaction);
}