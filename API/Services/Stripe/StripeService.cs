using Stripe.Checkout;
using Stripe;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;

namespace API.Services.Stripe;

public class StripeService : IStripeService
{
    private readonly StripeSettings _stripeSettings;

    public StripeService(IOptions<StripeSettings> stripeSettings)
    {
        _stripeSettings = stripeSettings.Value;
    }

    
    public Session CreateCheckoutSession(double localAmount, string paymentDescription, string currency = "USD", Dictionary<string, string> metaData = null)
    {
        // Set your secret key (Replace this with your actual secret key)
        StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
 
        // Build the line items for the checkout session
        var options = new SessionCreateOptions
        {
            SuccessUrl = $"{_stripeSettings.DashboardUrl}/core/success?session_id={{CHECKOUT_SESSION_ID}}",
            CancelUrl = $"{_stripeSettings.DashboardUrl}/cancel",
            PaymentMethodTypes = new List<string> { "card" },
            Mode = "payment",
            Metadata = metaData,
            LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    Quantity = 1,
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = string.IsNullOrWhiteSpace(currency) ? "USD" : currency,
                        UnitAmount = (long)Math.Round(localAmount * 100), // Stripe expects amount in cents
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = "Transaction Payment",
                            Description = paymentDescription
                        }
                    }
                }
            }
        };

        // Create the checkout session
        var service = new SessionService();
        var session = service.Create(options);

        return session;
    }
}