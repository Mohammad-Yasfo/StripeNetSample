using Microsoft.EntityFrameworkCore;
using StripeNetSample.Repositories.Payment.Entities;
using StripeNetSample.Repositories.Payment.Transaction.Entities;

namespace StripeNetSample.Repositories.Payment
{
    public class PaymentDbContext : DbContext
    {
        const string CommonSchema = "cmn";

        public PaymentDbContext(DbContextOptions<PaymentDbContext> options)
            : base(options)
        {
        }

        public DbSet<TransactionEntity> Transactions { get; set; }
        public DbSet<CompanyPaymentAccountEntity> CompaniesPaymentAccount { get; set; }
        public DbSet<CompanyPaymentEntity> CompaniesPayment { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<TransactionEntity>(a =>
            {
                a.Property(p => p.Amount).HasPrecision(8, 2);
            });

            builder.Entity<CompanyPaymentAccountEntity>(a =>
            {
            });
        }
    }
}