-- Completes database-level conflict protection for employee work-mode overrides.
-- Application validation returns the detailed tenant-facing error; this constraint closes
-- concurrent-write races. Safe to rerun. Existing conflicting rows are reported, not modified.

BEGIN;
SELECT pg_advisory_xact_lock(7421092601);
CREATE EXTENSION IF NOT EXISTS btree_gist;

DO $validation$
BEGIN
    IF EXISTS
    (
        SELECT 1
        FROM axionpro."EmployeeWorkModeOverrideRequest" first_override
        JOIN axionpro."EmployeeWorkModeOverrideRequest" second_override
          ON second_override."TenantId" = first_override."TenantId"
         AND second_override."EmployeeId" = first_override."EmployeeId"
         AND second_override."Id" > first_override."Id"
         AND daterange(second_override."FromDate", second_override."ToDate", '[]')
             && daterange(first_override."FromDate", first_override."ToDate", '[]')
        WHERE first_override."IsActive" = TRUE
          AND first_override."IsSoftDeleted" = FALSE
          AND first_override."ApprovalStatus" IN (1, 2)
          AND second_override."IsActive" = TRUE
          AND second_override."IsSoftDeleted" = FALSE
          AND second_override."ApprovalStatus" IN (1, 2)
    ) THEN
        RAISE EXCEPTION 'Active Pending/Approved employee work-mode overrides overlap. Resolve the listed business records before applying this constraint.';
    END IF;
END
$validation$;

DO $constraint$
BEGIN
    IF NOT EXISTS
    (
        SELECT 1 FROM pg_constraint
        WHERE conname = 'EX_EmployeeWorkModeOverride_Employee_Window'
          AND conrelid = 'axionpro."EmployeeWorkModeOverrideRequest"'::regclass
    ) THEN
        ALTER TABLE axionpro."EmployeeWorkModeOverrideRequest"
        ADD CONSTRAINT "EX_EmployeeWorkModeOverride_Employee_Window"
        EXCLUDE USING gist
        (
            "TenantId" WITH =,
            "EmployeeId" WITH =,
            daterange("FromDate", "ToDate", '[]') WITH &&
        )
        WHERE ("IsActive" = TRUE AND "IsSoftDeleted" = FALSE AND "ApprovalStatus" IN (1, 2));
    END IF;
END
$constraint$;

COMMIT;
