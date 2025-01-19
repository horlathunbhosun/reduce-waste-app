using API.Dtos.Response;
using API.Dtos.Transaction;
using API.Mappers;
using API.Repositories.Interface;
using API.Services.Stripe;

namespace API.Services.Transactions;

public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IMagicBagRepository _magicBagRepository;
    private readonly IStripeService _stripeService;
    
    public TransactionService(ITransactionRepository transactionRepository, IStripeService stripeService, IMagicBagRepository magicBagRepository)
    {
        _transactionRepository = transactionRepository;
        _stripeService = stripeService;
        _magicBagRepository = magicBagRepository;
    }
    
    
    public async Task<GenericResponse> CreateTransaction(TransactionRequestDto transactionRequestDto)
    {
        try
        {
            
            var magicBag = await _magicBagRepository.GetMagicBagById(transactionRequestDto.MagicBagId);
            if (magicBag == null)
            {
                return GenericResponse.FromError(new ErrorResponse("An Error occured MagicBag not found", "MagicBag not found",StatusCodes.Status404NotFound ), StatusCodes.Status404NotFound);
            }
            
            transactionRequestDto.Amount = magicBag.BagPrice;
            
            var createTransaction = await _transactionRepository.CreateTransaction(transactionRequestDto.ToTransactionRequestDto());
            if (createTransaction == null)
            {
                return GenericResponse.FromError(new ErrorResponse("An Error occured product not created", "Product not created",StatusCodes.Status400BadRequest ), StatusCodes.Status400BadRequest);
            }
            
            
            var metadata = new Dictionary<string, string>
            {
                
                {"UserId", createTransaction!.UserId},
                {"MagicBagId", createTransaction!.MagicBagId.ToString()},
                {"amount", createTransaction!.Amount.ToString()},
                {"TransactionID",createTransaction.Id.ToString() }
            };
            
            var stripeResponse = _stripeService.
                CreateCheckoutSession(transactionRequestDto.Amount, "Payment for Magic Bag", "USD",
                    metadata);
            
            if (stripeResponse == null)
            {
                return GenericResponse.FromError(new ErrorResponse("An Error occured payment not created", "Payment not initialed",StatusCodes.Status400BadRequest ), StatusCodes.Status400BadRequest);
            }

        
            return GenericResponse.FromSuccess(new SuccessResponse("Stripe link created successfully", stripeResponse, StatusCodes.Status201Created), StatusCodes.Status201Created);
        }catch (Exception e)
        {
            Console.WriteLine(e);
            return GenericResponse.FromError(new ErrorResponse("An Error occured product not created", "Internal Server error",StatusCodes.Status500InternalServerError ), StatusCodes.Status500InternalServerError );
        }
    }
    
    
    public async Task<GenericResponse> GetTransactionById(Guid id)
    {
        var transaction = await _transactionRepository.GetTransactionById(id);
        if (transaction == null)
        {
            return GenericResponse.FromError(new ErrorResponse("An Error occured product not found", "Product not found",StatusCodes.Status404NotFound ), StatusCodes.Status404NotFound);
        }
        
        return GenericResponse.FromSuccess(new SuccessResponse("Product found successfully", transaction.ToTransactionResponseDto(), StatusCodes.Status200OK), StatusCodes.Status200OK);
    }
    
    public async Task<GenericResponse> GetAllTransactions()
    {
        var transactions = await _transactionRepository.GetAllTransactions();
        if (transactions == null)
        {
            return GenericResponse.FromError(new ErrorResponse("An Error occured product not found", "Product not found",StatusCodes.Status404NotFound ), StatusCodes.Status404NotFound);
        }
        return GenericResponse.FromSuccess(new SuccessResponse("Product found successfully", transactions.Select(transaction => transaction.ToTransactionResponseDto()).ToList(), StatusCodes.Status200OK), StatusCodes.Status200OK);
    }
    
    
    public async Task<GenericResponse> GetAllTransactionsByUserId(string userId)
    {
        var transactions = await _transactionRepository.GetAllTransactionsByUserId(userId);
        if (transactions == null)
        {
            return GenericResponse.FromError(new ErrorResponse("An Error occured product not found", "Product not found",StatusCodes.Status404NotFound ), StatusCodes.Status404NotFound);
        }
        return GenericResponse.FromSuccess(new SuccessResponse("Product found successfully", transactions.Select(transaction => transaction.ToTransactionResponseDto()).ToList(), StatusCodes.Status200OK), StatusCodes.Status200OK);
    }
    
    
    public async Task<GenericResponse> UpdateTransaction(Guid id, TransactionRequestDto transactionRequestDto)
    {
        try
        {
            var transaction = await _transactionRepository.GetTransactionById(id);
            if (transaction == null)
            {
                return GenericResponse.FromError(new ErrorResponse("An Error occured product not found", "Product not found",StatusCodes.Status404NotFound ), StatusCodes.Status404NotFound);
            } 
            var updatedTransaction = await _transactionRepository.UpdateTransaction(transactionRequestDto.ToTransactionRequestDto(), id);
            return GenericResponse.FromSuccess(new SuccessResponse("Product updated successfully", updatedTransaction.ToTransactionResponseDto(), StatusCodes.Status200OK), StatusCodes.Status200OK);
        }catch (Exception e)
        {
            Console.WriteLine(e);
            return GenericResponse.FromError(new ErrorResponse("An Error occured product not updated", "Internal Server error",StatusCodes.Status500InternalServerError ), StatusCodes.Status500InternalServerError );
        }
    }

    public GenericResponse DeleteTransaction(Guid id)
    {
        throw new NotImplementedException();
    }
}