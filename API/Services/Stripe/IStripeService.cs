using Stripe.Checkout;

namespace API.Services.Stripe;

public interface IStripeService
{
    Session CreateCheckoutSession(double localAmount, string paymentDescription, string currency = "USD", Dictionary<string, string> metaData = null);
}