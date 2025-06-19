using API.Data;
using API.Models;
using API.Services.CDC;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;

namespace API.Services.Transactions;

public class EnhancedTransactionService : ITransactionService
{
    private readonly ApplicationDbContext _context;
    private readonly ITransactionCDCService _cdcService;
    private readonly ILogger<EnhancedTransactionService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public EnhancedTransactionService(
        ApplicationDbContext context,
        ITransactionCDCService cdcService,
        ILogger<EnhancedTransactionService> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _cdcService = cdcService;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    // Example implementation of update method with CDC integration
    public async Task<bool> UpdateTransactionAsync(Guid id, Transactions updatedTransaction)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var existingTransaction = await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == id);

            if (existingTransaction == null)
                return false;

            // Store the old values for auditing
            var oldValues = CreateTransactionSnapshot(existingTransaction);
            var changedFields = new List<string>();

            // Track changes
            if (existingTransaction.Amount != updatedTransaction.Amount)
            {
                existingTransaction.Amount = updatedTransaction.Amount;
                changedFields.Add(nameof(Transactions.Amount));
            }

            if (existingTransaction.Status != updatedTransaction.Status)
            {
                existingTransaction.Status = updatedTransaction.Status;
                changedFields.Add(nameof(Transactions.Status));
            }

            if (existingTransaction.PickUpdateDate != updatedTransaction.PickUpdateDate)
            {
                existingTransaction.PickUpdateDate = updatedTransaction.PickUpdateDate;
                changedFields.Add(nameof(Transactions.PickUpdateDate));
            }

            if (existingTransaction.DynamicColumns != updatedTransaction.DynamicColumns)
            {
                existingTransaction.DynamicColumns = updatedTransaction.DynamicColumns;
                changedFields.Add(nameof(Transactions.DynamicColumns));
            }

            // Update timestamp
            existingTransaction.UpdatedAt = DateTime.UtcNow;
            changedFields.Add(nameof(Transactions.UpdatedAt));

            // Save the changes
            await _context.SaveChangesAsync();

            // Create audit log entry if there were changes
            if (changedFields.Any())
            {
                var newValues = CreateTransactionSnapshot(existingTransaction);
                var currentUser = GetCurrentUserId();

                var auditLog = new TransactionAuditLog
                {
                    Id = Guid.NewGuid(),
                    TransactionId = existingTransaction.Id,
                    Operation = "UPDATE",
                    Timestamp = DateTime.UtcNow,
                    UserId = existingTransaction.UserId,
                    ApplicationUser = currentUser,
                    ChangeSource = "APPLICATION"
                };

                auditLog.SetOldValues(oldValues);
                auditLog.SetNewValues(newValues);
                auditLog.SetChangedFields(changedFields);

                await _cdcService.CreateAuditLogAsync(auditLog);
            }

            await transaction.CommitAsync();
            return true;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error updating transaction {TransactionId}", id);
            return false;
        }
    }

    public async Task<Transactions?> CreateTransactionAsync(Transactions newTransaction)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            newTransaction.Id = Guid.NewGuid();
            newTransaction.CreatedAt = DateTime.UtcNow;
            newTransaction.UpdatedAt = DateTime.UtcNow;

            _context.Transactions.Add(newTransaction);
            await _context.SaveChangesAsync();

            // Create audit log for INSERT operation
            var newValues = CreateTransactionSnapshot(newTransaction);
            var currentUser = GetCurrentUserId();

            var auditLog = new TransactionAuditLog
            {
                Id = Guid.NewGuid(),
                TransactionId = newTransaction.Id,
                Operation = "INSERT",
                Timestamp = DateTime.UtcNow,
                UserId = newTransaction.UserId,
                ApplicationUser = currentUser,
                ChangeSource = "APPLICATION"
            };

            auditLog.SetNewValues(newValues);
            auditLog.SetChangedFields(new List<string>());

            await _cdcService.CreateAuditLogAsync(auditLog);

            await transaction.CommitAsync();
            return newTransaction;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error creating transaction");
            return null;
        }
    }

    public async Task<bool> DeleteTransactionAsync(Guid id)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var existingTransaction = await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == id);

            if (existingTransaction == null)
                return false;

            // Store the old values for auditing
            var oldValues = CreateTransactionSnapshot(existingTransaction);
            var currentUser = GetCurrentUserId();

            // Create audit log for DELETE operation
            var auditLog = new TransactionAuditLog
            {
                Id = Guid.NewGuid(),
                TransactionId = existingTransaction.Id,
                Operation = "DELETE",
                Timestamp = DateTime.UtcNow,
                UserId = existingTransaction.UserId,
                ApplicationUser = currentUser,
                ChangeSource = "APPLICATION"
            };

            auditLog.SetOldValues(oldValues);
            auditLog.SetChangedFields(new List<string>());

            await _cdcService.CreateAuditLogAsync(auditLog);

            _context.Transactions.Remove(existingTransaction);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
            return true;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error deleting transaction {TransactionId}", id);
            return false;
        }
    }

    public async Task<Transactions?> GetTransactionByIdAsync(Guid id)
    {
        return await _context.Transactions
            .Include(t => t.Users)
            .Include(t => t.MagicBag)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<List<Transactions>> GetTransactionsByUserIdAsync(string userId)
    {
        return await _context.Transactions
            .Where(t => t.UserId == userId)
            .Include(t => t.Users)
            .Include(t => t.MagicBag)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Transactions>> GetAllTransactionsAsync()
    {
        return await _context.Transactions
            .Include(t => t.Users)
            .Include(t => t.MagicBag)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    private Dictionary<string, object?> CreateTransactionSnapshot(Transactions transaction)
    {
        return new Dictionary<string, object?>
        {
            [nameof(transaction.Id)] = transaction.Id,
            [nameof(transaction.UserId)] = transaction.UserId,
            [nameof(transaction.MagicBagId)] = transaction.MagicBagId,
            [nameof(transaction.TransactionDate)] = transaction.TransactionDate,
            [nameof(transaction.Amount)] = transaction.Amount,
            [nameof(transaction.Status)] = transaction.Status,
            [nameof(transaction.PickUpdateDate)] = transaction.PickUpdateDate,
            [nameof(transaction.CreatedAt)] = transaction.CreatedAt,
            [nameof(transaction.UpdatedAt)] = transaction.UpdatedAt,
            [nameof(transaction.DynamicColumns)] = transaction.DynamicColumns
        };
    }

    private string? GetCurrentUserId()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
} 