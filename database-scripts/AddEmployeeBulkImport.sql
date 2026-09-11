-- Employee import uses the existing durable queue. Fail on conflicting legacy
-- identities rather than merging, overwriting or deleting employee accounts.
BEGIN;
ALTER TABLE axionpro."BulkImportJob" DROP CONSTRAINT IF EXISTS "BulkImportJob_Master_check";
ALTER TABLE axionpro."BulkImportJob" ADD CONSTRAINT "BulkImportJob_Master_check"
    CHECK ("Master" BETWEEN 1 AND 5);
CREATE UNIQUE INDEX IF NOT EXISTS "UX_Employee_Tenant_Code"
    ON axionpro."Employee" ("TenantId", lower(btrim("EmployementCode")))
    WHERE "EmployementCode" IS NOT NULL AND btrim("EmployementCode") <> '';
CREATE UNIQUE INDEX IF NOT EXISTS "UX_LoginCredential_NormalizedLogin"
    ON axionpro."LoginCredential" (lower(btrim("LoginId")));
COMMIT;
