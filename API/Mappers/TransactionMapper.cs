using API.Dtos.Transaction;
using API.Models;

namespace API.Mappers;

public static class TransactionMapper
{
    public static TransactionResponseDto ToTransactionResponseDto(this Transactions transactions)
    {
        return new TransactionResponseDto
        {
            Id = transactions.Id,
            Amount = transactions.Amount,
            Users = transactions.Users!.ToUserResponseDto(),
            MagicBag = transactions.MagicBag!.ToMagicBagResponseDto(),
            Status = transactions.Status,
            TransactionDate = transactions.TransactionDate,
            PickUpdateDate = transactions.PickUpdateDate,
            CreatedAt = transactions.CreatedAt,
            UpdatedAt = transactions.UpdatedAt
      
        };
    }
    
    
    public static Transactions ToTransactionRequestDto(this TransactionRequestDto transactionRequestDto)
    {
        return new Transactions
        {
            UserId = transactionRequestDto.UserId,
            MagicBagId = transactionRequestDto.MagicBagId,
            Amount = transactionRequestDto.Amount,
            TransactionDate = DateTime.Now,
            PickUpdateDate = DateTime.Now.AddDays(2),
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
        };
    }
}