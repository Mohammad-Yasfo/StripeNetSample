using Microsoft.EntityFrameworkCore;
using StripeNetSample.Application.Payment.Configuration;
using StripeNetSample.Infrastructure.Payment.Stripe.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace StripeNetSample.Api.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class ConfigurationExtension
    {
        public static IServiceCollection AddDbContext<T>(this IServiceCollection services, string connectionString, bool useAzureIdentity) where T : DbContext
        {
            var venuesBuilder =
                GetDbContextWithoutAzureIdentity(connectionString, typeof(T).Assembly.GetName().Name);

            return services.AddDbContext<T>(venuesBuilder);
        }

        public static Action<DbContextOptionsBuilder> GetDbContextWithoutAzureIdentity(string connectionString, string migrationAssembly)
        {
            return builder => builder.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationAssembly));
        }

        public static StripeConfiguration GetStripeConfiguration(this IConfiguration configuration)
        {
            return configuration.GetSection("StripeConfiguration").Get<StripeConfiguration>();
        }

        public static WebAppsConfiguration GetWebAppsConfiguration(this IConfiguration configuration)
        {
            return configuration.GetSection("WebAppsConfiguration").Get<WebAppsConfiguration>();
        }

        public static DbConnectionConfiguration GetDbConnectionConfiguration(this IConfiguration configuration)
        {
            return new DbConnectionConfiguration
            {
                IntegrationConnectionString = configuration.GetConnectionString("PaymentConnectionString"),
                AzureIdentity = configuration.GetValue<bool>("ConnectionStrings:AzureIdentity"),
            };
        }
    }
}