using System.Diagnostics.CodeAnalysis;

namespace ReLifeAssessment.Application.Payment.Configuration
{
    [ExcludeFromCodeCoverage]
    public class DbConnectionConfiguration
    {
        public string IntegrationConnectionString { get; set; }
        public bool AzureIdentity { get; set; }
    }
}