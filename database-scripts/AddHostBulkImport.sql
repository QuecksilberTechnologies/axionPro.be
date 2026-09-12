-- Run after AddEmployeeBulkImport.sql. Transactional, non-destructive upgrade.
-- Host master values partition Host actor IDs from tenant-employee actor IDs.
BEGIN;
ALTER TABLE axionpro."BulkImportJob" DROP CONSTRAINT IF EXISTS "BulkImportJob_Master_check";
ALTER TABLE axionpro."BulkImportJob" ADD CONSTRAINT "BulkImportJob_Master_check"
    CHECK ("Master" BETWEEN 1 AND 11);
-- Match existing manual-create duplicate semantics; abort if legacy duplicates exist.
CREATE UNIQUE INDEX IF NOT EXISTS "UX_DeviceMaster_Bulk_Code"
    ON axionpro."DeviceMaster" (lower("DeviceCode")) WHERE "IsSoftDeleted" = FALSE;
CREATE UNIQUE INDEX IF NOT EXISTS "UX_DeviceMaster_Bulk_Company_Model"
    ON axionpro."DeviceMaster" (lower("CompanyName"), lower("ModelNo")) WHERE "IsSoftDeleted" = FALSE;
CREATE UNIQUE INDEX IF NOT EXISTS "UX_TenantCardMaster_Bulk_Hash"
    ON axionpro."TenantCardMaster" ("TenantId", "CardNumberLookupHash") WHERE "IsSoftDeleted" = FALSE;
COMMIT;
