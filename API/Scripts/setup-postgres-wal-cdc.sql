-- PostgreSQL WAL-Level CDC Setup Script
-- This script configures PostgreSQL for WAL-based Change Data Capture

-- =============================================================================
-- STEP 1: Configure PostgreSQL for Logical Replication
-- =============================================================================

-- First, these settings need to be added to postgresql.conf and PostgreSQL restarted:
-- wal_level = logical
-- max_wal_senders = 10
-- max_replication_slots = 10
-- max_logical_replication_workers = 4

-- Check current configuration
SELECT name, setting, unit, context, short_desc 
FROM pg_settings 
WHERE name IN ('wal_level', 'max_wal_senders', 'max_replication_slots', 'max_logical_replication_workers');

-- =============================================================================
-- STEP 2: Create Replication User (if not exists)
-- =============================================================================

-- Create a dedicated replication user
DO $$
BEGIN
    IF NOT EXISTS (SELECT FROM pg_catalog.pg_roles WHERE rolname = 'cdc_user') THEN
        CREATE ROLE cdc_user WITH REPLICATION LOGIN PASSWORD 'cdc_password_2024!';
    END IF;
END
$$;

-- Grant necessary permissions
GRANT CONNECT ON DATABASE "reducing-food-waste-net" TO cdc_user;
GRANT USAGE ON SCHEMA public TO cdc_user;
GRANT SELECT ON ALL TABLES IN SCHEMA public TO cdc_user;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT SELECT ON TABLES TO cdc_user;

-- Grant replication privileges
ALTER ROLE cdc_user REPLICATION;

-- =============================================================================
-- STEP 3: Create Publication for Transaction Changes
-- =============================================================================

-- Drop existing publication if it exists
DROP PUBLICATION IF EXISTS transactions_changes;

-- Create publication for the Transactions table
CREATE PUBLICATION transactions_changes FOR TABLE "Transactions";

-- Verify publication was created
SELECT pubname, puballtables, pubinsert, pubupdate, pubdelete, pubtruncate
FROM pg_publication 
WHERE pubname = 'transactions_changes';

-- View tables in the publication
SELECT schemaname, tablename 
FROM pg_publication_tables 
WHERE pubname = 'transactions_changes';

-- =============================================================================
-- STEP 4: Create Logical Replication Slot
-- =============================================================================

-- Create a logical replication slot for CDC
SELECT pg_create_logical_replication_slot('transactions_cdc_slot', 'pgoutput');

-- Verify replication slot was created
SELECT slot_name, plugin, slot_type, database, active, restart_lsn, confirmed_flush_lsn
FROM pg_replication_slots 
WHERE slot_name = 'transactions_cdc_slot';

-- =============================================================================
-- STEP 5: Create WAL Decoder Functions
-- =============================================================================

-- Function to decode WAL changes and convert to JSON
CREATE OR REPLACE FUNCTION decode_transaction_changes()
RETURNS TABLE(
    lsn pg_lsn,
    operation text,
    transaction_id bigint,
    table_name text,
    old_values jsonb,
    new_values jsonb,
    change_timestamp timestamp with time zone
) AS $$
DECLARE
    rec record;
BEGIN
    -- This is a simplified example - in practice, you'd use pg_logical_slot_get_changes
    -- or integrate with tools like Debezium for proper WAL decoding
    
    FOR rec IN 
        SELECT * FROM pg_logical_slot_peek_changes('transactions_cdc_slot', NULL, NULL)
    LOOP
        -- Parse the WAL data (this is a simplified example)
        lsn := rec.lsn;
        operation := 'UNKNOWN';
        transaction_id := rec.xid;
        table_name := 'Transactions';
        old_values := '{}'::jsonb;
        new_values := '{}'::jsonb;
        change_timestamp := now();
        
        RETURN NEXT;
    END LOOP;
END;
$$ LANGUAGE plpgsql;

-- =============================================================================
-- STEP 6: Create Real-time Change Tracking View
-- =============================================================================

-- Create a view that combines WAL changes with application audit logs
CREATE OR REPLACE VIEW real_time_transaction_changes AS
SELECT 
    'WAL' as source,
    lsn::text as change_id,
    operation,
    transaction_id::text as transaction_ref,
    table_name,
    old_values,
    new_values,
    change_timestamp as timestamp,
    null as application_user
FROM decode_transaction_changes()
UNION ALL
SELECT 
    'AUDIT' as source,
    "Id"::text as change_id,
    "Operation" as operation,
    "TransactionId"::text as transaction_ref,
    'Transactions' as table_name,
    "OldValues"::jsonb as old_values,
    "NewValues"::jsonb as new_values,
    "Timestamp" as timestamp,
    "ApplicationUser" as application_user
FROM "TransactionAuditLogs"
ORDER BY timestamp DESC;

-- =============================================================================
-- STEP 7: Create Functions for WAL Management
-- =============================================================================

-- Function to advance replication slot (consume processed changes)
CREATE OR REPLACE FUNCTION advance_cdc_slot(target_lsn pg_lsn)
RETURNS pg_lsn AS $$
BEGIN
    RETURN pg_replication_slot_advance('transactions_cdc_slot', target_lsn);
END;
$$ LANGUAGE plpgsql;

-- Function to get current WAL position
CREATE OR REPLACE FUNCTION get_current_wal_position()
RETURNS pg_lsn AS $$
BEGIN
    RETURN pg_current_wal_lsn();
END;
$$ LANGUAGE plpgsql;

-- Function to check replication lag
CREATE OR REPLACE FUNCTION get_replication_lag()
RETURNS TABLE(
    slot_name text,
    lag_bytes bigint,
    lag_seconds interval
) AS $$
BEGIN
    RETURN QUERY
    SELECT 
        s.slot_name::text,
        pg_wal_lsn_diff(pg_current_wal_lsn(), s.restart_lsn) as lag_bytes,
        CASE 
            WHEN s.active THEN now() - s.confirmed_flush_lsn::text::timestamp
            ELSE null
        END as lag_seconds
    FROM pg_replication_slots s
    WHERE s.slot_name = 'transactions_cdc_slot';
END;
$$ LANGUAGE plpgsql;

-- =============================================================================
-- STEP 8: Create Subscription (for separate CDC database)
-- =============================================================================

-- If you want to replicate to a separate CDC database, create a subscription
-- Note: This should be run on the CDC target database, not the source

/*
-- Example subscription creation (run on target database)
CREATE SUBSCRIPTION transactions_cdc_subscription
CONNECTION 'host=localhost port=5432 user=cdc_user password=cdc_password_2024! dbname=reducing-food-waste-net'
PUBLICATION transactions_changes
WITH (copy_data = false, create_slot = false, slot_name = 'transactions_cdc_slot');
*/

-- =============================================================================
-- STEP 9: Create Monitoring Functions
-- =============================================================================

-- Function to monitor WAL generation rate
CREATE OR REPLACE FUNCTION monitor_wal_activity()
RETURNS TABLE(
    current_lsn pg_lsn,
    wal_segments_generated bigint,
    wal_bytes_generated bigint,
    last_checkpoint timestamp with time zone
) AS $$
BEGIN
    RETURN QUERY
    SELECT 
        pg_current_wal_lsn() as current_lsn,
        pg_stat_get_wal_senders() as wal_segments_generated,
        pg_wal_lsn_diff(pg_current_wal_lsn(), '0/0'::pg_lsn) as wal_bytes_generated,
        pg_stat_get_bgwriter_checkpoint_write_time()::timestamp as last_checkpoint;
END;
$$ LANGUAGE plpgsql;

-- Function to get CDC statistics
CREATE OR REPLACE FUNCTION get_cdc_statistics()
RETURNS TABLE(
    total_changes bigint,
    changes_last_hour bigint,
    changes_last_day bigint,
    oldest_unprocessed_change timestamp with time zone,
    replication_slot_active boolean
) AS $$
BEGIN
    RETURN QUERY
    SELECT 
        COUNT(*)::bigint as total_changes,
        COUNT(*) FILTER (WHERE "Timestamp" > now() - interval '1 hour')::bigint as changes_last_hour,
        COUNT(*) FILTER (WHERE "Timestamp" > now() - interval '1 day')::bigint as changes_last_day,
        MIN("Timestamp") as oldest_unprocessed_change,
        COALESCE((SELECT active FROM pg_replication_slots WHERE slot_name = 'transactions_cdc_slot'), false) as replication_slot_active
    FROM "TransactionAuditLogs";
END;
$$ LANGUAGE plpgsql;

-- =============================================================================
-- STEP 10: Create Cleanup and Maintenance Functions
-- =============================================================================

-- Function to cleanup old WAL files
CREATE OR REPLACE FUNCTION cleanup_wal_files()
RETURNS text AS $$
BEGIN
    -- Advance the replication slot to allow WAL cleanup
    PERFORM pg_replication_slot_advance('transactions_cdc_slot', pg_current_wal_lsn());
    
    -- Request checkpoint to clean up WAL files
    CHECKPOINT;
    
    RETURN 'WAL cleanup completed at ' || now();
END;
$$ LANGUAGE plpgsql;

-- Function to reset CDC system
CREATE OR REPLACE FUNCTION reset_cdc_system()
RETURNS text AS $$
BEGIN
    -- Drop and recreate replication slot
    PERFORM pg_drop_replication_slot('transactions_cdc_slot');
    PERFORM pg_create_logical_replication_slot('transactions_cdc_slot', 'pgoutput');
    
    RETURN 'CDC system reset completed at ' || now();
END;
$$ LANGUAGE plpgsql;

-- =============================================================================
-- STEP 11: Grant Permissions
-- =============================================================================

-- Grant permissions on CDC functions
GRANT EXECUTE ON FUNCTION decode_transaction_changes() TO cdc_user;
GRANT EXECUTE ON FUNCTION get_current_wal_position() TO cdc_user;
GRANT EXECUTE ON FUNCTION get_replication_lag() TO cdc_user;
GRANT EXECUTE ON FUNCTION monitor_wal_activity() TO cdc_user;
GRANT EXECUTE ON FUNCTION get_cdc_statistics() TO cdc_user;

-- Grant permissions on views
GRANT SELECT ON real_time_transaction_changes TO cdc_user;

-- =============================================================================
-- STEP 12: Create Alerts and Notifications
-- =============================================================================

-- Function to check CDC health
CREATE OR REPLACE FUNCTION check_cdc_health()
RETURNS TABLE(
    check_name text,
    status text,
    details text
) AS $$
BEGIN
    -- Check if replication slot exists and is active
    IF NOT EXISTS (SELECT 1 FROM pg_replication_slots WHERE slot_name = 'transactions_cdc_slot') THEN
        RETURN QUERY SELECT 'Replication Slot'::text, 'ERROR'::text, 'Replication slot does not exist'::text;
    ELSIF NOT (SELECT active FROM pg_replication_slots WHERE slot_name = 'transactions_cdc_slot') THEN
        RETURN QUERY SELECT 'Replication Slot'::text, 'WARNING'::text, 'Replication slot is not active'::text;
    ELSE
        RETURN QUERY SELECT 'Replication Slot'::text, 'OK'::text, 'Replication slot is active'::text;
    END IF;
    
    -- Check WAL level
    IF (SELECT setting FROM pg_settings WHERE name = 'wal_level') != 'logical' THEN
        RETURN QUERY SELECT 'WAL Level'::text, 'ERROR'::text, 'WAL level is not set to logical'::text;
    ELSE
        RETURN QUERY SELECT 'WAL Level'::text, 'OK'::text, 'WAL level is set to logical'::text;
    END IF;
    
    -- Check replication lag
    IF (SELECT lag_bytes FROM get_replication_lag()) > 1024*1024*100 THEN -- 100MB
        RETURN QUERY SELECT 'Replication Lag'::text, 'WARNING'::text, 'Replication lag is high'::text;
    ELSE
        RETURN QUERY SELECT 'Replication Lag'::text, 'OK'::text, 'Replication lag is normal'::text;
    END IF;
END;
$$ LANGUAGE plpgsql;

-- =============================================================================
-- VERIFICATION AND STATUS
-- =============================================================================

-- Display setup status
DO $$
BEGIN
    RAISE NOTICE '=== PostgreSQL WAL-Level CDC Setup Status ===';
    RAISE NOTICE 'Current WAL Level: %', (SELECT setting FROM pg_settings WHERE name = 'wal_level');
    RAISE NOTICE 'Max WAL Senders: %', (SELECT setting FROM pg_settings WHERE name = 'max_wal_senders');
    RAISE NOTICE 'Max Replication Slots: %', (SELECT setting FROM pg_settings WHERE name = 'max_replication_slots');
    RAISE NOTICE 'Publication Created: %', (SELECT COUNT(*) FROM pg_publication WHERE pubname = 'transactions_changes');
    RAISE NOTICE 'Replication Slot Created: %', (SELECT COUNT(*) FROM pg_replication_slots WHERE slot_name = 'transactions_cdc_slot');
    RAISE NOTICE 'Setup completed at: %', NOW();
END $$;

-- Show current CDC status
SELECT * FROM check_cdc_health();
SELECT * FROM get_cdc_statistics(); 