# PostgreSQL WAL-Level CDC Setup Guide

## Overview

This guide explains how to set up PostgreSQL's native Write-Ahead Log (WAL) based Change Data Capture for real-time transaction monitoring. Unlike trigger-based CDC, WAL-level CDC captures changes directly from PostgreSQL's transaction log, providing:

- **Near-zero performance impact** on your application
- **Guaranteed capture** of all changes (even direct database modifications)
- **Real-time streaming** of changes as they occur
- **Better scalability** for high-volume environments

## Architecture

```
┌─────────────────┐    WAL Stream    ┌──────────────────┐    Process    ┌─────────────────┐
│   PostgreSQL    │ ────────────────▶│  WAL Consumer    │ ─────────────▶│  Audit Logs     │
│   (WAL/LSN)     │                  │  (Logical Rep)   │               │  (JSON Storage) │
└─────────────────┘                  └──────────────────┘               └─────────────────┘
```

## Prerequisites

### 1. PostgreSQL Configuration

Edit your `postgresql.conf` file and restart PostgreSQL:

```ini
# Enable logical replication
wal_level = logical

# Set replication parameters
max_wal_senders = 10
max_replication_slots = 10
max_logical_replication_workers = 4

# Optional: For better performance
wal_compression = on
wal_buffers = 16MB
checkpoint_completion_target = 0.9
```

### 2. Verify Configuration

Check if settings are applied:
```sql
SELECT name, setting, context 
FROM pg_settings 
WHERE name IN ('wal_level', 'max_wal_senders', 'max_replication_slots');
```

Expected output:
```
       name        | setting |  context  
-------------------+---------+-----------
 wal_level         | logical | postmaster
 max_wal_senders   | 10      | postmaster
 max_replication_slots | 10  | postmaster
```

## Setup Steps

### Step 1: Configure PostgreSQL for CDC

Run the WAL CDC setup script:

```bash
psql -U postgres -d reducing-food-waste-net -f Scripts/setup-postgres-wal-cdc.sql
```

This script will:
- Create a replication user (`cdc_user`)
- Set up publication (`transactions_changes`)
- Create logical replication slot (`transactions_cdc_slot`)
- Configure monitoring functions

### Step 2: Verify Setup

Check if everything is configured correctly:

```sql
-- Check publication
SELECT pubname, puballtables, pubinsert, pubupdate, pubdelete 
FROM pg_publication WHERE pubname = 'transactions_changes';

-- Check replication slot
SELECT slot_name, plugin, slot_type, active, restart_lsn 
FROM pg_replication_slots WHERE slot_name = 'transactions_cdc_slot';

-- Check permissions
SELECT rolname, rolreplication FROM pg_roles WHERE rolname = 'cdc_user';
```

### Step 3: Configure Application Settings

Enable WAL consumer in `appsettings.json`:

```json
{
  "CDC": {
    "StartWALConsumerOnStartup": true,
    "WALConsumerSettings": {
      "ReplicationSlotName": "transactions_cdc_slot",
      "PublicationName": "transactions_changes",
      "ReplicationUser": "cdc_user",
      "ReplicationPassword": "cdc_password_2024!",
      "MemoryBufferSize": 1000,
      "ProcessingIntervalMs": 1000,
      "EnableRealTimeStream": true
    }
  }
}
```

### Step 4: Start the Application

The WAL consumer will automatically start when the application starts:

```bash
dotnet run
```

Look for these log messages:
```
info: Program[0] Trigger-based CDC system initialized successfully
info: Program[0] WAL-based CDC consumer started successfully
```

## API Endpoints

### WAL Consumer Management

```bash
# Start WAL consumer
POST /api/WALCDC/start

# Stop WAL consumer  
POST /api/WALCDC/stop

# Get consumer status
GET /api/WALCDC/status

# Test configuration
POST /api/WALCDC/test
```

### Real-time Change Monitoring

```bash
# Get recent WAL changes
GET /api/WALCDC/recent-changes?limit=50

# Real-time change stream (Server-Sent Events)
GET /api/WALCDC/stream

# Health monitoring
GET /api/WALCDC/health
```

## Usage Examples

### 1. Monitor WAL Consumer Status

```bash
curl -X GET "https://localhost:5001/api/WALCDC/status" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

Response:
```json
{
  "success": true,
  "data": {
    "isConsumerRunning": true,
    "totalWALChanges": 1250,
    "changesLastHour": 45,
    "changesLastDay": 892,
    "recentChangesCount": 50,
    "lastChangeTimestamp": "2024-01-15T14:30:22Z"
  }
}
```

### 2. Real-time Change Stream (JavaScript)

```javascript
const eventSource = new EventSource('/api/WALCDC/stream');

eventSource.onmessage = function(event) {
    const change = JSON.parse(event.data);
    console.log('Transaction changed:', {
        id: change.transactionId,
        operation: change.operation,
        timestamp: change.timestamp,
        lsn: change.lsn
    });
};

eventSource.onerror = function(event) {
    console.error('WAL stream error:', event);
};
```

### 3. Health Monitoring

```bash
curl -X GET "https://localhost:5001/api/WALCDC/health" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

Response:
```json
{
  "success": true,
  "data": {
    "isHealthy": true,
    "consumerRunning": true,
    "lastChangeAge": "00:02:15",
    "changesPerHour": 45,
    "changesPerDay": 892,
    "memoryBufferSize": 50,
    "recommendations": [
      "WAL CDC system is operating normally"
    ]
  }
}
```

## Monitoring and Maintenance

### 1. Monitor Replication Lag

```sql
-- Check replication lag
SELECT * FROM get_replication_lag();

-- Monitor WAL activity
SELECT * FROM monitor_wal_activity();
```

### 2. View Recent Changes

```sql
-- View combined changes from WAL and triggers
SELECT * FROM real_time_transaction_changes 
ORDER BY timestamp DESC 
LIMIT 10;
```

### 3. Cleanup Old Data

```sql
-- Clean up audit logs older than 365 days
SELECT audit.cleanup_old_audit_logs(365);

-- Clean up WAL files (advances replication slot)
SELECT cleanup_wal_files();
```

### 4. Performance Monitoring

```sql
-- Check CDC health
SELECT * FROM check_cdc_health();

-- Get CDC statistics  
SELECT * FROM get_cdc_statistics();
```

## Performance Considerations

### WAL Size Management

Monitor WAL disk usage:
```sql
SELECT 
    pg_size_pretty(pg_wal_lsn_diff(pg_current_wal_lsn(), restart_lsn)) as lag_size,
    pg_size_pretty(pg_current_wal_lsn() - '0/0'::pg_lsn) as total_wal_size
FROM pg_replication_slots 
WHERE slot_name = 'transactions_cdc_slot';
```

### Memory Usage

The WAL consumer keeps recent changes in memory. Monitor usage:
- Default buffer size: 1000 changes
- Adjust `MemoryBufferSize` in configuration
- Monitor heap usage in application logs

### Network Bandwidth

WAL streaming uses network bandwidth. For high-volume systems:
- Monitor network I/O
- Consider compression settings
- Use dedicated network for replication if needed

## Troubleshooting

### Common Issues

1. **WAL Consumer Won't Start**
   ```bash
   # Check PostgreSQL settings
   SELECT setting FROM pg_settings WHERE name = 'wal_level';
   
   # Should return 'logical'
   ```

2. **No Changes Being Captured**
   ```sql
   -- Check if publication includes your table
   SELECT * FROM pg_publication_tables WHERE pubname = 'transactions_changes';
   
   -- Check replication slot activity
   SELECT active FROM pg_replication_slots WHERE slot_name = 'transactions_cdc_slot';
   ```

3. **High Replication Lag**
   ```sql
   -- Check lag
   SELECT * FROM get_replication_lag();
   
   -- Advance slot manually if needed
   SELECT pg_replication_slot_advance('transactions_cdc_slot', pg_current_wal_lsn());
   ```

4. **Connection Errors**
   ```sql
   -- Verify replication user permissions
   SELECT rolname, rolreplication, rolcanlogin FROM pg_roles WHERE rolname = 'cdc_user';
   ```

### Debug Mode

Enable debug logging in `appsettings.json`:
```json
{
  "Logging": {
    "LogLevel": {
      "API.Services.CDC": "Debug",
      "Npgsql": "Debug"
    }
  }
}
```

### Reset CDC System

If you need to reset the WAL CDC system:
```sql
-- Reset the CDC system (drops and recreates replication slot)
SELECT reset_cdc_system();
```

## Security Considerations

### User Permissions

The replication user (`cdc_user`) has minimal required permissions:
- `REPLICATION` privilege
- `SELECT` on tracked tables
- No write permissions

### Network Security

- Use SSL for replication connections in production
- Restrict replication user to specific IP addresses
- Monitor replication connections

### Data Privacy

WAL changes contain full row data:
- Implement field-level filtering if needed
- Consider encryption for sensitive data
- Audit access to change logs

## Production Deployment

### High Availability

- Set up standby replication slots
- Monitor slot lag continuously  
- Implement automatic failover for consumer

### Backup Strategy

- Include replication slots in backup procedures
- Test restore procedures with active slots
- Document recovery procedures

### Monitoring Alerts

Set up alerts for:
- Replication lag > 10MB
- Consumer down for > 5 minutes  
- WAL disk usage > 80%
- Failed change processing

## Integration Examples

### With Kafka

Stream changes to Kafka for distributed processing:
```csharp
// In PostgresWALCDCService.ProcessWALMessage
await _kafkaProducer.ProduceAsync("transaction-changes", changeEvent);
```

### With SignalR

Real-time UI updates:
```csharp
// In PostgresWALCDCService.ProcessWALMessage  
await _hubContext.Clients.All.SendAsync("TransactionChanged", changeEvent);
```

### With ElasticSearch

Index changes for search:
```csharp
// In PostgresWALCDCService.ProcessWALMessage
await _elasticClient.IndexAsync(changeEvent, idx => idx.Index("transaction-changes"));
```

This WAL-level CDC implementation provides a robust, scalable foundation for real-time transaction monitoring with minimal performance impact on your application. 