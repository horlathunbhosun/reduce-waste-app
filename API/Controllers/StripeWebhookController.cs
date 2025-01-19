using API.Repositories.Interface;
using API.Services.MagicBag;
using API.Services.Stripe;
using API.Services.Transactions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace API.Controllers;


[Route("api/stripe-webhook")]
[ApiController]
public class StripeWebhookController  : ControllerBase
{
    
    private readonly StripeSettings _stripeSettings;

    private readonly ITransactionRepository _transactionRepository;
    
    private readonly IMagicBagRepository _magicBagRepository;
    
    public StripeWebhookController(IOptions<StripeSettings> stripeSettings, ITransactionRepository transactionRepository, IMagicBagRepository magicBagRepository)
    {
        _stripeSettings = stripeSettings.Value;
        _transactionRepository = transactionRepository;
        _magicBagRepository = magicBagRepository;
    }
    
    [HttpPost]
    public async Task<IActionResult> StripeWebhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], _stripeSettings.WebhookSecret,
        throwOnApiVersionMismatch: false // Disables the mismatch exception
        
        );
        var eventType = stripeEvent.Type;
        var session = stripeEvent.Data.Object as Session;
        switch (eventType)
        {
            case "checkout.session.completed":
                // Payment is successful and the subscription is created.
                // You should provision the subscription and save the customer ID to your database.
                Console.WriteLine($"{session} Payment is successful and the subscription is created.");
                
                if (session != null)
                {
                    var metadata = session.Metadata;
                    var amount = metadata["amount"];
                    var transactionId = Guid.Parse(metadata["TransactionID"]);
                    var userId = metadata["UserId"];
                    var magicBagId = Guid.Parse(metadata["MagicBagId"]);
                    
                    
                    var transaction = await _transactionRepository.GetTransactionById(transactionId);
                    
                    if (transaction == null)
                    {
                        return Ok();
                    }
                    
                    transaction.Status = "Completed";
                    
                    await _transactionRepository.UpdateTransaction(transaction, transactionId);
                    
                    var magicBag = await _magicBagRepository.GetMagicBagById(magicBagId);
                    
                    if (magicBag == null)
                    {
                        return Ok();
                    }
                    magicBag.Status = "Pending";
                    
                    await _magicBagRepository.UpdateMagicBag(magicBag,magicBagId);
                    

                    Console.WriteLine($"Amount: {amount}");
                    Console.WriteLine($"TransactionID: {transactionId}");
                    Console.WriteLine($"UserId: {userId}");
                    Console.WriteLine($"MagicBagId: {magicBagId}");
                    Console.WriteLine($"Metadata: {metadata}");
                }

                Console.WriteLine(json);
                break;
            case "checkout.session.async_payment_succeeded":
                // Handle async payment
                
                break;
            case "checkout.session.async_payment_failed":
                // Handle async payment
                break;
            default:
                // Unexpected event type
                return BadRequest();
        }
        return Ok();
    }
}