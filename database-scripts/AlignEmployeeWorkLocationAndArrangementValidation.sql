BEGIN;

-- GiST exclusion constraints make the API's overlap rule concurrency-safe.
CREATE EXTENSION IF NOT EXISTS btree_gist;

-- Date-window conflicts are validated before writes. These former unique indexes
-- prevented valid non-overlapping history and future schedules from coexisting.
DROP INDEX IF EXISTS axionpro."UX_EmployeeLocationAssignment_Employee_Location";
DROP INDEX IF EXISTS axionpro."UX_EmployeeLocationAssignment_Primary";
DROP INDEX IF EXISTS axionpro."UX_EmployeeWorkArrangement_Current";

CREATE INDEX IF NOT EXISTS "IX_EmployeeLocationAssignment_Employee_Location_EffectiveFrom"
    ON axionpro."EmployeeLocationAssignment" ("TenantId", "EmployeeId", "TenantLocationId", "EffectiveFrom");

CREATE INDEX IF NOT EXISTS "IX_EmployeeLocationAssignment_Primary_EffectiveFrom"
    ON axionpro."EmployeeLocationAssignment" ("TenantId", "EmployeeId", "IsPrimary", "EffectiveFrom");

CREATE INDEX IF NOT EXISTS "IX_EmployeeWorkArrangement_Employee_EffectiveFrom"
    ON axionpro."EmployeeWorkArrangement" ("TenantId", "EmployeeId", "EffectiveFrom");

DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'EX_EmployeeLocationAssignment_Employee_Location_Window') THEN
        ALTER TABLE axionpro."EmployeeLocationAssignment"
            ADD CONSTRAINT "EX_EmployeeLocationAssignment_Employee_Location_Window"
            EXCLUDE USING gist
            ("TenantId" WITH =, "EmployeeId" WITH =, "TenantLocationId" WITH =,
             daterange("EffectiveFrom", COALESCE("EffectiveTo", 'infinity'::date), '[]') WITH &&)
            WHERE ("IsActive" = true AND "IsSoftDeleted" = false);
    END IF;

    IF NOT EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'EX_EmployeeLocationAssignment_Primary_Window') THEN
        ALTER TABLE axionpro."EmployeeLocationAssignment"
            ADD CONSTRAINT "EX_EmployeeLocationAssignment_Primary_Window"
            EXCLUDE USING gist
            ("TenantId" WITH =, "EmployeeId" WITH =,
             daterange("EffectiveFrom", COALESCE("EffectiveTo", 'infinity'::date), '[]') WITH &&)
            WHERE ("IsPrimary" = true AND "IsActive" = true AND "IsSoftDeleted" = false);
    END IF;

    IF NOT EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'EX_EmployeeWorkArrangement_Employee_Window') THEN
        ALTER TABLE axionpro."EmployeeWorkArrangement"
            ADD CONSTRAINT "EX_EmployeeWorkArrangement_Employee_Window"
            EXCLUDE USING gist
            ("TenantId" WITH =, "EmployeeId" WITH =,
             daterange("EffectiveFrom", COALESCE("EffectiveTo", 'infinity'::date), '[]') WITH &&)
            WHERE ("IsActive" = true AND "IsSoftDeleted" = false);
    END IF;
END $$;

COMMIT;
