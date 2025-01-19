using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StripeNetSample.Application.Payment.Contracts;
using StripeNetSample.Repositories.Payment.Transaction.Entities;

namespace StripeNetSample.Repositories.Payment.Transaction.Repositories
{
    /// <summary>
    /// Repository class for managing transactions.
    /// </summary>
    public class TransactionRepository : ITransactionRepository
    {
        private readonly PaymentDbContext _context;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        public TransactionRepository(PaymentDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Adds a new transaction to the repository.
        /// </summary>
        /// <param name="transaction">The transaction to add.</param>
        /// <returns>The added transaction with its updated details.</returns>
        public async Task<Application.Payment.Models.Transaction> AddAsync(Application.Payment.Models.Transaction transaction)
        {
            var transactionEntity = _mapper.Map<TransactionEntity>(transaction);
            _context.Transactions.Add(transactionEntity);
            await _context.SaveChangesAsync();

            // Map back to the application model to return with updated details
            var addedTransaction = _mapper.Map<Application.Payment.Models.Transaction>(transactionEntity);
            return addedTransaction;
        }

        /// <summary>
        /// Updates an existing transaction in the repository.
        /// </summary>
        /// <param name="transaction">The transaction with updated information.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="Exception">Thrown when the transaction is not found.</exception>
        public async Task UpdateAsync(Application.Payment.Models.Transaction transaction)
        {
            var transactionDb = await _context.Transactions
                .FirstOrDefaultAsync(a => a.Id == transaction.Id)
                ?? throw new Exception("Transaction not found");

            // Update transaction details
            transactionDb.Amount = transaction.Amount;
            transactionDb.ProviderTransactionId = transaction.ProviderTransactionId;
            transactionDb.TransactionStatus = transaction.TransactionStatus;
            transactionDb.PaymentStatus = transaction.PaymentStatus;
            transactionDb.Currency = transaction.Currency;
            transactionDb.CompanyId = transaction.CompanyId;

            await _context.SaveChangesAsync();
        }
    }
}