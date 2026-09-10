-- Company: Quecksilber Technologies
-- Purpose: Every designation belongs to a department. Names are unique only
-- within that tenant/department; IT/Manager and HR/Manager are both valid.
-- Does not delete, merge or invent departments for existing data.
BEGIN;

LOCK TABLE axionpro."Designation" IN SHARE ROW EXCLUSIVE MODE;

DO $$
BEGIN
    IF EXISTS (
        SELECT 1
        FROM axionpro."Designation"
        WHERE "DepartmentId" IS NULL OR "DepartmentId" <= 0
    ) THEN
        RAISE EXCEPTION 'Designation has missing/invalid DepartmentId. Resolve those records before applying this migration.';
    END IF;

    IF EXISTS (
        SELECT 1
        FROM axionpro."Designation" designation
        LEFT JOIN axionpro."Department" department
            ON department."Id" = designation."DepartmentId"
           AND department."TenantId" = designation."TenantId"
        WHERE department."Id" IS NULL
    ) THEN
        RAISE EXCEPTION 'Designation has a missing or other-tenant department. Resolve those records first.';
    END IF;

    IF EXISTS (
        SELECT 1
        FROM axionpro."Designation"
        WHERE "IsSoftDeleted" IS NOT TRUE
        GROUP BY "TenantId", "DepartmentId", lower(btrim("DesignationName"))
        HAVING count(*) > 1
    ) THEN
        RAISE EXCEPTION 'Duplicate designation names exist within a tenant department. Resolve duplicates before applying this migration.';
    END IF;
END;
$$;

ALTER TABLE axionpro."Designation"
    ALTER COLUMN "DepartmentId" SET NOT NULL;

CREATE UNIQUE INDEX IF NOT EXISTS "UX_Designation_Tenant_Department_Name_Live"
    ON axionpro."Designation" (
        "TenantId",
        "DepartmentId",
        lower(btrim("DesignationName"))
    )
    WHERE "IsSoftDeleted" IS NOT TRUE;

COMMIT;
