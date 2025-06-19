-- PostgreSQL CDC Setup Script for Reducing Food Waste Application
-- This script sets up the database for Change Data Capture operations

-- Create database if it doesn't exist (run this as superuser)
-- CREATE DATABASE "reducing-food-waste-net" WITH ENCODING 'UTF8' LC_COLLATE='en_US.UTF-8' LC_CTYPE='en_US.UTF-8';

-- Connect to the database
\c "reducing-food-waste-net";

-- Enable necessary extensions
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pgcrypto";

-- Set up tablespace for audit logs (optional, for performance)
-- CREATE TABLESPACE audit_logs_tablespace LOCATION '/var/lib/postgresql/audit_logs';

-- Grant necessary permissions (adjust user as needed)
-- GRANT ALL PRIVILEGES ON DATABASE "reducing-food-waste-net" TO postgres;

-- Create a dedicated schema for audit functions (optional)
CREATE SCHEMA IF NOT EXISTS audit;

-- Function to get current application user from session variables
CREATE OR REPLACE FUNCTION audit.get_application_user()
RETURNS TEXT AS $$
BEGIN
    RETURN COALESCE(current_setting('myapp.current_user', true), 'SYSTEM');
EXCEPTION
    WHEN OTHERS THEN
        RETURN 'SYSTEM';
END;
$$ LANGUAGE plpgsql;

-- Enhanced audit trigger function with better error handling
CREATE OR REPLACE FUNCTION audit.audit_transactions_trigger()
RETURNS TRIGGER AS $$
DECLARE
    old_values JSONB;
    new_values JSONB;
    changed_fields TEXT[] := '{}';
    field_name TEXT;
    audit_user TEXT;
BEGIN
    -- Get the application user
    audit_user := audit.get_application_user();
    
    -- Handle different operations
    IF TG_OP = 'DELETE' THEN
        old_values := to_jsonb(OLD);
        new_values := NULL;
        
        INSERT INTO "TransactionAuditLogs" (
            "Id", "TransactionId", "Operation", "Timestamp", 
            "UserId", "OldValues", "NewValues", "ChangedFields", 
            "ChangeSource", "ApplicationUser"
        ) VALUES (
            gen_random_uuid(), 
            OLD."Id", 
            'DELETE', 
            NOW(), 
            OLD."UserId", 
            old_values::TEXT, 
            NULL, 
            '[]'::TEXT, 
            'DATABASE_TRIGGER',
            audit_user
        );
        
        RETURN OLD;
        
    ELSIF TG_OP = 'UPDATE' THEN
        old_values := to_jsonb(OLD);
        new_values := to_jsonb(NEW);
        
        -- Detect changed fields with null handling
        IF OLD."UserId" IS DISTINCT FROM NEW."UserId" THEN
            changed_fields := array_append(changed_fields, 'UserId');
        END IF;
        IF OLD."MagicBagId" IS DISTINCT FROM NEW."MagicBagId" THEN
            changed_fields := array_append(changed_fields, 'MagicBagId');
        END IF;
        IF OLD."TransactionDate" IS DISTINCT FROM NEW."TransactionDate" THEN
            changed_fields := array_append(changed_fields, 'TransactionDate');
        END IF;
        IF OLD."Amount" IS DISTINCT FROM NEW."Amount" THEN
            changed_fields := array_append(changed_fields, 'Amount');
        END IF;
        IF OLD."Status" IS DISTINCT FROM NEW."Status" THEN
            changed_fields := array_append(changed_fields, 'Status');
        END IF;
        IF OLD."PickUpdateDate" IS DISTINCT FROM NEW."PickUpdateDate" THEN
            changed_fields := array_append(changed_fields, 'PickUpdateDate');
        END IF;
        IF OLD."CreatedAt" IS DISTINCT FROM NEW."CreatedAt" THEN
            changed_fields := array_append(changed_fields, 'CreatedAt');
        END IF;
        IF OLD."UpdatedAt" IS DISTINCT FROM NEW."UpdatedAt" THEN
            changed_fields := array_append(changed_fields, 'UpdatedAt');
        END IF;
        IF OLD."DynamicColumns" IS DISTINCT FROM NEW."DynamicColumns" THEN
            changed_fields := array_append(changed_fields, 'DynamicColumns');
        END IF;
        
        -- Only log if there are actual changes (excluding UpdatedAt auto-updates)
        IF array_length(changed_fields, 1) > 0 AND 
           NOT (array_length(changed_fields, 1) = 1 AND 'UpdatedAt' = ANY(changed_fields)) THEN
            
            INSERT INTO "TransactionAuditLogs" (
                "Id", "TransactionId", "Operation", "Timestamp", 
                "UserId", "OldValues", "NewValues", "ChangedFields", 
                "ChangeSource", "ApplicationUser"
            ) VALUES (
                gen_random_uuid(), 
                NEW."Id", 
                'UPDATE', 
                NOW(), 
                NEW."UserId", 
                old_values::TEXT, 
                new_values::TEXT, 
                array_to_json(changed_fields)::TEXT, 
                'DATABASE_TRIGGER',
                audit_user
            );
        END IF;
        
        RETURN NEW;
        
    ELSIF TG_OP = 'INSERT' THEN
        new_values := to_jsonb(NEW);
        
        INSERT INTO "TransactionAuditLogs" (
            "Id", "TransactionId", "Operation", "Timestamp", 
            "UserId", "OldValues", "NewValues", "ChangedFields", 
            "ChangeSource", "ApplicationUser"
        ) VALUES (
            gen_random_uuid(), 
            NEW."Id", 
            'INSERT', 
            NOW(), 
            NEW."UserId", 
            NULL, 
            new_values::TEXT, 
            '[]'::TEXT, 
            'DATABASE_TRIGGER',
            audit_user
        );
        
        RETURN NEW;
    END IF;
    
    RETURN NULL;
EXCEPTION
    WHEN OTHERS THEN
        -- Log the error but don't fail the main transaction
        RAISE WARNING 'Audit trigger failed: %', SQLERRM;
        RETURN COALESCE(NEW, OLD);
END;
$$ LANGUAGE plpgsql;

-- Function to set application user context
CREATE OR REPLACE FUNCTION audit.set_application_user(user_id TEXT)
RETURNS VOID AS $$
BEGIN
    PERFORM set_config('myapp.current_user', user_id, false);
END;
$$ LANGUAGE plpgsql;

-- Create indexes for better performance on audit logs
-- These will be created by Entity Framework migrations, but listed here for reference
/*
CREATE INDEX CONCURRENTLY IF NOT EXISTS idx_transactionauditlogs_transactionid 
ON "TransactionAuditLogs"("TransactionId");

CREATE INDEX CONCURRENTLY IF NOT EXISTS idx_transactionauditlogs_timestamp 
ON "TransactionAuditLogs"("Timestamp" DESC);

CREATE INDEX CONCURRENTLY IF NOT EXISTS idx_transactionauditlogs_userid 
ON "TransactionAuditLogs"("UserId");

CREATE INDEX CONCURRENTLY IF NOT EXISTS idx_transactionauditlogs_operation 
ON "TransactionAuditLogs"("Operation");

CREATE INDEX CONCURRENTLY IF NOT EXISTS idx_transactionauditlogs_compound 
ON "TransactionAuditLogs"("TransactionId", "Timestamp" DESC);
*/

-- Create a view for easy querying of transaction changes
CREATE OR REPLACE VIEW audit.transaction_changes_view AS
SELECT 
    tal."Id" as audit_id,
    tal."TransactionId",
    tal."Operation",
    tal."Timestamp",
    tal."UserId",
    tal."ApplicationUser",
    tal."ChangeSource",
    tal."ChangedFields"::jsonb as changed_fields,
    tal."OldValues"::jsonb as old_values,
    tal."NewValues"::jsonb as new_values,
    t."Amount" as current_amount,
    t."Status" as current_status,
    t."CreatedAt" as transaction_created,
    u."Email" as user_email
FROM "TransactionAuditLogs" tal
LEFT JOIN "Transactions" t ON tal."TransactionId" = t."Id"
LEFT JOIN "AspNetUsers" u ON tal."UserId" = u."Id"
ORDER BY tal."Timestamp" DESC;

-- Grant permissions on the audit schema
GRANT USAGE ON SCHEMA audit TO postgres;
GRANT EXECUTE ON ALL FUNCTIONS IN SCHEMA audit TO postgres;
GRANT SELECT ON audit.transaction_changes_view TO postgres;

-- Function to clean up old audit logs (optional)
CREATE OR REPLACE FUNCTION audit.cleanup_old_audit_logs(retention_days INTEGER DEFAULT 365)
RETURNS INTEGER AS $$
DECLARE
    deleted_count INTEGER;
BEGIN
    DELETE FROM "TransactionAuditLogs" 
    WHERE "Timestamp" < NOW() - INTERVAL '1 day' * retention_days;
    
    GET DIAGNOSTICS deleted_count = ROW_COUNT;
    
    RETURN deleted_count;
END;
$$ LANGUAGE plpgsql;

-- Create a maintenance function to be run periodically
CREATE OR REPLACE FUNCTION audit.maintenance()
RETURNS TEXT AS $$
DECLARE
    result TEXT;
BEGIN
    -- Reindex audit tables for performance
    REINDEX TABLE "TransactionAuditLogs";
    
    -- Update table statistics
    ANALYZE "TransactionAuditLogs";
    
    result := 'Maintenance completed successfully at ' || NOW();
    RETURN result;
END;
$$ LANGUAGE plpgsql;

-- Log the setup completion
DO $$
BEGIN
    RAISE NOTICE 'PostgreSQL CDC setup completed successfully at %', NOW();
END $$; 