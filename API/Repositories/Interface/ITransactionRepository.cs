using System.Transactions;
using API.Models;

namespace API.Repositories.Interface;

public interface ITransactionRepository
{
    Task<Transactions?> GetTransactionById(Guid id);
    
    
    
    Task<List<Transactions>> GetAllTransactions();
    
    Task<List<Transactions>> GetAllTransactionsByUserId(string userId);
    
    Task<Transactions> CreateTransaction(Transactions transaction);
    
    Task<Transactions> UpdateTransaction(Transactions transaction, Guid id);
    
}