using API.Dtos.Response;
using API.Dtos.Transaction;

namespace API.Services.Transactions;

public interface ITransactionService
{
    Task<GenericResponse> CreateTransaction(TransactionRequestDto transactionRequestDto);
    
    Task<GenericResponse> GetTransactionById(Guid id);
    
    Task<GenericResponse> GetAllTransactions();
    
    Task<GenericResponse> GetAllTransactionsByUserId(string userId);
    
    Task<GenericResponse> UpdateTransaction(Guid id, TransactionRequestDto transactionRequestDto);
    
    GenericResponse DeleteTransaction(Guid id);
}