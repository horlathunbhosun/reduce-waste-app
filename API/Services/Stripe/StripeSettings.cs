namespace API.Services.Stripe;

public class StripeSettings
{
    public string SecretKey { get; set; }
 //   public string PublishableKey { get; set; }
    public string WebhookSecret { get; set; }
    
    public string DashboardUrl { get; set; }
}