using API.Models;
using API.Dtos.Response;

namespace API.Services.CDC;

public interface ITransactionCDCService
{
    Task<bool> InitializeCDCAsync();
    Task<List<TransactionAuditLog>> GetTransactionHistoryAsync(Guid transactionId);
    Task<List<TransactionAuditLog>> GetRecentChangesAsync(int limit = 50);
    Task<List<TransactionAuditLog>> GetChangesByUserAsync(string userId, int limit = 50);
    Task<List<TransactionAuditLog>> GetChangesByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<TransactionAuditLog?> GetChangeByIdAsync(Guid changeId);
    Task<bool> CreateAuditLogAsync(TransactionAuditLog auditLog);
    Task<bool> SetupDatabaseTriggersAsync();
    Task<bool> RemoveDatabaseTriggersAsync();
} 