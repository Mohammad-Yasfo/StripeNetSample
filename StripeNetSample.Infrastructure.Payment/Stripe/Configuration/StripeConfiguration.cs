namespace StripeNetSample.Infrastructure.Payment.Stripe.Configuration;

public class StripeConfiguration
{
    public string ClientId { get; set; }
    public string PublishableKey { get; set; }
    public string SecretKey { get; set; }
    public string OAuthBaseLink { get; set; }
}
