# PostgreSQL Change Data Capture (CDC) Implementation Guide

## Overview

This implementation provides a comprehensive Change Data Capture (CDC) system for tracking all changes to the `Transactions` table in your Reducing Food Waste application. The system captures and stores detailed information about every INSERT, UPDATE, and DELETE operation.

## Features

- **Automatic Change Tracking**: Database triggers automatically capture all changes
- **Detailed Audit Logs**: Store old values, new values, and changed fields
- **Dual-Source Tracking**: Distinguish between application-level and direct database changes
- **User Attribution**: Track which application user made the change
- **RESTful API**: Comprehensive endpoints for querying audit data
- **Performance Optimized**: Proper indexing and efficient queries
- **Analytics Support**: Built-in analytics for change patterns

## Architecture Components

### 1. Database Layer
- **TransactionAuditLog Table**: Stores all change records
- **Database Triggers**: Automatically capture changes at the database level
- **PostgreSQL Functions**: Handle audit logic and utilities

### 2. Application Layer
- **TransactionCDCService**: Main service for CDC operations
- **TransactionAuditController**: REST API endpoints
- **EnhancedTransactionService**: Application-level change tracking

### 3. Data Models
- **TransactionAuditLog**: Core audit log entity
- **TransactionAuditLogDto**: Data transfer objects
- **Analytics DTOs**: Change analytics models

## Setup Instructions

### 1. Database Setup

First, ensure PostgreSQL is installed and running. Update your connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "PostgreSqlConnection": "Host=localhost;Port=5432;Database=reducing-food-waste-net;Username=postgres;Password=your_password;Include Error Detail=true;"
  }
}
```

### 2. Run Database Setup Script

Execute the setup script to configure PostgreSQL for CDC:

```bash
psql -U postgres -d reducing-food-waste-net -f Scripts/setup-postgres-cdc.sql
```

### 3. Install NuGet Packages

The required packages have been added to your project:
- `Npgsql.EntityFrameworkCore.PostgreSQL`
- `Npgsql`

### 4. Run Migrations

Create and apply Entity Framework migrations:

```bash
dotnet ef migrations add AddTransactionAuditLog
dotnet ef database update
```

### 5. Initialize CDC System

The CDC system will automatically initialize when the application starts. You can also manually initialize it using the admin endpoint:

```
POST /api/TransactionAudit/initialize
```

## API Endpoints

### Transaction History
```
GET /api/TransactionAudit/transaction/{transactionId}/history
```
Get complete change history for a specific transaction.

### Recent Changes
```
GET /api/TransactionAudit/recent?limit=50
```
Get recent changes across all transactions.

### Changes by User
```
GET /api/TransactionAudit/user/{userId}?limit=50
```
Get changes made by a specific user.

### Changes by Date Range
```
GET /api/TransactionAudit/date-range?startDate=2024-01-01&endDate=2024-01-31
```
Get changes within a specific date range.

### Change Analytics
```
GET /api/TransactionAudit/analytics?days=30
```
Get analytics about transaction changes.

### Admin Endpoints
```
POST /api/TransactionAudit/initialize        # Initialize CDC system
POST /api/TransactionAudit/setup-triggers    # Setup database triggers
DELETE /api/TransactionAudit/triggers        # Remove database triggers
```

## Usage Examples

### 1. Viewing Transaction History

```csharp
// Get all changes for a specific transaction
var response = await httpClient.GetAsync($"/api/TransactionAudit/transaction/{transactionId}/history");
var history = await response.Content.ReadFromJsonAsync<GenericResponse>();
```

### 2. Monitoring Recent Changes

```csharp
// Get the 100 most recent changes
var response = await httpClient.GetAsync("/api/TransactionAudit/recent?limit=100");
var recentChanges = await response.Content.ReadFromJsonAsync<GenericResponse>();
```

### 3. User Activity Tracking

```csharp
// Track changes made by a specific user
var response = await httpClient.GetAsync($"/api/TransactionAudit/user/{userId}");
var userActivity = await response.Content.ReadFromJsonAsync<GenericResponse>();
```

### 4. Change Analytics

```csharp
// Get analytics for the past 7 days
var response = await httpClient.GetAsync("/api/TransactionAudit/analytics?days=7");
var analytics = await response.Content.ReadFromJsonAsync<GenericResponse>();
```

## Data Structure

### Audit Log Entry
```json
{
  "id": "123e4567-e89b-12d3-a456-426614174000",
  "transactionId": "123e4567-e89b-12d3-a456-426614174001",
  "operation": "UPDATE",
  "timestamp": "2024-01-15T10:30:00Z",
  "userId": "user123",
  "applicationUser": "admin@example.com",
  "changeSource": "APPLICATION",
  "oldValues": {
    "amount": 25.50,
    "status": "Pending"
  },
  "newValues": {
    "amount": 30.00,
    "status": "Completed"
  },
  "changedFields": ["amount", "status"]
}
```

## Change Sources

The system distinguishes between different sources of changes:

- **APPLICATION**: Changes made through the application API
- **DATABASE_TRIGGER**: Direct database changes
- **SYSTEM**: System-generated changes

## Performance Considerations

### Indexing
The following indexes are automatically created:
- Transaction ID index for fast lookups
- Timestamp index for date range queries
- User ID index for user activity queries
- Compound indexes for optimized filtering

### Data Retention
Use the built-in cleanup function to manage data retention:

```sql
-- Clean up audit logs older than 365 days
SELECT audit.cleanup_old_audit_logs(365);
```

### Maintenance
Run periodic maintenance:

```sql
-- Perform maintenance operations
SELECT audit.maintenance();
```

## Security Considerations

1. **Authorization**: All endpoints require authentication
2. **Admin Operations**: Setup/teardown operations require Admin role
3. **Data Privacy**: Sensitive data in old/new values should be handled carefully
4. **Access Control**: Consider implementing field-level access control for audit data

## Monitoring and Alerting

Consider setting up monitoring for:
- High volume of changes in short periods
- Failed audit log entries
- Unusual change patterns
- Storage growth rates

## Troubleshooting

### Common Issues

1. **CDC Not Capturing Changes**
   - Verify triggers are installed: Check `pg_trigger` table
   - Ensure connection string points to correct database
   - Check application logs for CDC initialization errors

2. **Performance Issues**
   - Monitor index usage with `EXPLAIN ANALYZE`
   - Consider partitioning audit table by date
   - Implement data archiving strategy

3. **Missing Application User**
   - Ensure `IHttpContextAccessor` is registered in DI
   - Verify JWT token contains user claims
   - Check authentication middleware configuration

### Debugging Queries

```sql
-- Check if triggers exist
SELECT * FROM pg_trigger WHERE tgname = 'transactions_audit_trigger';

-- View recent audit entries
SELECT * FROM "TransactionAuditLogs" ORDER BY "Timestamp" DESC LIMIT 10;

-- Check audit statistics
SELECT "Operation", COUNT(*) FROM "TransactionAuditLogs" GROUP BY "Operation";

-- View detailed change information
SELECT * FROM audit.transaction_changes_view LIMIT 10;
```

## Migration from MySQL

If migrating from MySQL:
1. Export existing transaction data
2. Set up PostgreSQL with this CDC implementation
3. Import data to PostgreSQL
4. Update connection strings
5. Test CDC functionality

## Best Practices

1. **Regular Backups**: Include audit logs in backup strategy
2. **Data Archiving**: Implement archiving for old audit data
3. **Monitoring**: Set up alerts for audit system health
4. **Testing**: Test CDC functionality in development environment
5. **Documentation**: Keep audit schema documentation updated

## Future Enhancements

Consider implementing:
- Real-time change notifications via SignalR
- Advanced analytics dashboard
- Machine learning for anomaly detection
- Cross-table change correlation
- Export functionality for compliance reporting

## Support

For issues or questions:
1. Check application logs
2. Review PostgreSQL logs
3. Verify database connections
4. Test with smaller datasets

This implementation provides a robust foundation for change tracking that can be extended based on your specific business requirements. 