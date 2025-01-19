using API.Data;
using API.Exceptions;
using API.Models;
using API.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories;

public class TransactionRepository: ITransactionRepository
{
    private readonly ApplicationDbContext _context;
    
    public TransactionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    
    public async Task<Transactions?> GetTransactionById(Guid id)
    {
      return await _context.Transactions.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<Transactions>> GetAllTransactions()
    {
        return await  _context.Transactions.Include("Users").Include("MagicBag").ToListAsync();
    }

    public async Task<List<Transactions>> GetAllTransactionsByUserId(string userId)
    {
        return await  _context.Transactions.Include("Users").Include("MagicBag").Where(x => x.UserId == userId).ToListAsync();
    }

    public async Task<Transactions> CreateTransaction(Transactions transaction)
    {
        await _context.Transactions.AddAsync(transaction);
        await _context.SaveChangesAsync();
        Console.WriteLine($"Transaction Created Successfully: {transaction.Id}");
        return transaction;
    }

    public async Task<Transactions> UpdateTransaction(Transactions transaction, Guid id)
    {
        var transactionExist = await _context.Transactions.FirstOrDefaultAsync(p => p.Id == transaction.Id);
        if (transactionExist == null)
        {
            throw new NotFoundException("Transaction does not exist");
        }

        _context.Entry(transactionExist).CurrentValues.SetValues(transaction);
        var data =   _context.Transactions.Update(transactionExist);
        Console.WriteLine($"Transaction Updated Successfully: {transaction.Id}");
        Console.WriteLine($"Transaction Updated ENtity Successfully: {data.Entity.Id}" );
        await _context.SaveChangesAsync();
        return transactionExist;
    }
}