using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Text.Json;

namespace API.Services.CDC;

public class TransactionCDCService : ITransactionCDCService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<TransactionCDCService> _logger;

    public TransactionCDCService(
        ApplicationDbContext context, 
        IConfiguration configuration,
        ILogger<TransactionCDCService> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<bool> InitializeCDCAsync()
    {
        try
        {
            // Create the audit log table if it doesn't exist
            await _context.Database.EnsureCreatedAsync();
            
            // Setup database triggers for automatic CDC
            await SetupDatabaseTriggersAsync();
            
            _logger.LogInformation("CDC initialized successfully");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize CDC");
            return false;
        }
    }

    public async Task<List<TransactionAuditLog>> GetTransactionHistoryAsync(Guid transactionId)
    {
        return await _context.TransactionAuditLogs
            .Where(log => log.TransactionId == transactionId)
            .OrderByDescending(log => log.Timestamp)
            .ToListAsync();
    }

    public async Task<List<TransactionAuditLog>> GetRecentChangesAsync(int limit = 50)
    {
        return await _context.TransactionAuditLogs
            .Include(log => log.Transaction)
            .OrderByDescending(log => log.Timestamp)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<List<TransactionAuditLog>> GetChangesByUserAsync(string userId, int limit = 50)
    {
        return await _context.TransactionAuditLogs
            .Where(log => log.UserId == userId || log.ApplicationUser == userId)
            .Include(log => log.Transaction)
            .OrderByDescending(log => log.Timestamp)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<List<TransactionAuditLog>> GetChangesByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.TransactionAuditLogs
            .Where(log => log.Timestamp >= startDate && log.Timestamp <= endDate)
            .Include(log => log.Transaction)
            .OrderByDescending(log => log.Timestamp)
            .ToListAsync();
    }

    public async Task<TransactionAuditLog?> GetChangeByIdAsync(Guid changeId)
    {
        return await _context.TransactionAuditLogs
            .Include(log => log.Transaction)
            .FirstOrDefaultAsync(log => log.Id == changeId);
    }

    public async Task<bool> CreateAuditLogAsync(TransactionAuditLog auditLog)
    {
        try
        {
            _context.TransactionAuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create audit log");
            return false;
        }
    }

    public async Task<bool> SetupDatabaseTriggersAsync()
    {
        try
        {
            var connectionString = _configuration.GetConnectionString("PostgreSqlConnection");
            using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            // Create the audit trigger function
            var createFunctionSql = @"
                CREATE OR REPLACE FUNCTION audit_transactions_trigger()
                RETURNS TRIGGER AS $$
                DECLARE
                    old_values JSONB;
                    new_values JSONB;
                    changed_fields TEXT[] := '{}';
                    field_name TEXT;
                BEGIN
                    -- Handle different operations
                    IF TG_OP = 'DELETE' THEN
                        old_values := to_jsonb(OLD);
                        new_values := NULL;
                        
                        INSERT INTO ""TransactionAuditLogs"" (
                            ""Id"", ""TransactionId"", ""Operation"", ""Timestamp"", 
                            ""UserId"", ""OldValues"", ""NewValues"", ""ChangedFields"", ""ChangeSource""
                        ) VALUES (
                            gen_random_uuid(), OLD.""Id"", 'DELETE', NOW(), 
                            OLD.""UserId"", old_values::TEXT, NULL, '[]'::TEXT, 'DATABASE_TRIGGER'
                        );
                        
                        RETURN OLD;
                    ELSIF TG_OP = 'UPDATE' THEN
                        old_values := to_jsonb(OLD);
                        new_values := to_jsonb(NEW);
                        
                        -- Detect changed fields
                        IF OLD.""UserId"" IS DISTINCT FROM NEW.""UserId"" THEN
                            changed_fields := array_append(changed_fields, 'UserId');
                        END IF;
                        IF OLD.""MagicBagId"" IS DISTINCT FROM NEW.""MagicBagId"" THEN
                            changed_fields := array_append(changed_fields, 'MagicBagId');
                        END IF;
                        IF OLD.""Amount"" IS DISTINCT FROM NEW.""Amount"" THEN
                            changed_fields := array_append(changed_fields, 'Amount');
                        END IF;
                        IF OLD.""Status"" IS DISTINCT FROM NEW.""Status"" THEN
                            changed_fields := array_append(changed_fields, 'Status');
                        END IF;
                        IF OLD.""PickUpdateDate"" IS DISTINCT FROM NEW.""PickUpdateDate"" THEN
                            changed_fields := array_append(changed_fields, 'PickUpdateDate');
                        END IF;
                        IF OLD.""DynamicColumns"" IS DISTINCT FROM NEW.""DynamicColumns"" THEN
                            changed_fields := array_append(changed_fields, 'DynamicColumns');
                        END IF;
                        
                        -- Only log if there are actual changes
                        IF array_length(changed_fields, 1) > 0 THEN
                            INSERT INTO ""TransactionAuditLogs"" (
                                ""Id"", ""TransactionId"", ""Operation"", ""Timestamp"", 
                                ""UserId"", ""OldValues"", ""NewValues"", ""ChangedFields"", ""ChangeSource""
                            ) VALUES (
                                gen_random_uuid(), NEW.""Id"", 'UPDATE', NOW(), 
                                NEW.""UserId"", old_values::TEXT, new_values::TEXT, 
                                array_to_json(changed_fields)::TEXT, 'DATABASE_TRIGGER'
                            );
                        END IF;
                        
                        RETURN NEW;
                    ELSIF TG_OP = 'INSERT' THEN
                        new_values := to_jsonb(NEW);
                        
                        INSERT INTO ""TransactionAuditLogs"" (
                            ""Id"", ""TransactionId"", ""Operation"", ""Timestamp"", 
                            ""UserId"", ""OldValues"", ""NewValues"", ""ChangedFields"", ""ChangeSource""
                        ) VALUES (
                            gen_random_uuid(), NEW.""Id"", 'INSERT', NOW(), 
                            NEW.""UserId"", NULL, new_values::TEXT, '[]'::TEXT, 'DATABASE_TRIGGER'
                        );
                        
                        RETURN NEW;
                    END IF;
                    
                    RETURN NULL;
                END;
                $$ LANGUAGE plpgsql;";

            using var functionCommand = new NpgsqlCommand(createFunctionSql, connection);
            await functionCommand.ExecuteNonQueryAsync();

            // Drop existing trigger if it exists
            var dropTriggerSql = @"DROP TRIGGER IF EXISTS transactions_audit_trigger ON ""Transactions"";";
            using var dropCommand = new NpgsqlCommand(dropTriggerSql, connection);
            await dropCommand.ExecuteNonQueryAsync();

            // Create the trigger
            var createTriggerSql = @"
                CREATE TRIGGER transactions_audit_trigger
                AFTER INSERT OR UPDATE OR DELETE ON ""Transactions""
                FOR EACH ROW EXECUTE FUNCTION audit_transactions_trigger();";

            using var triggerCommand = new NpgsqlCommand(createTriggerSql, connection);
            await triggerCommand.ExecuteNonQueryAsync();

            _logger.LogInformation("Database triggers setup successfully");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to setup database triggers");
            return false;
        }
    }

    public async Task<bool> RemoveDatabaseTriggersAsync()
    {
        try
        {
            var connectionString = _configuration.GetConnectionString("PostgreSqlConnection");
            using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            // Drop the trigger
            var dropTriggerSql = @"DROP TRIGGER IF EXISTS transactions_audit_trigger ON ""Transactions"";";
            using var dropTriggerCommand = new NpgsqlCommand(dropTriggerSql, connection);
            await dropTriggerCommand.ExecuteNonQueryAsync();

            // Drop the function
            var dropFunctionSql = @"DROP FUNCTION IF EXISTS audit_transactions_trigger();";
            using var dropFunctionCommand = new NpgsqlCommand(dropFunctionSql, connection);
            await dropFunctionCommand.ExecuteNonQueryAsync();

            _logger.LogInformation("Database triggers removed successfully");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove database triggers");
            return false;
        }
    }
} 