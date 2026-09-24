-- =============================================================================
-- EmployeeWorkArrangement: use the generic Published Attendance PolicyVersion
-- =============================================================================
-- Existing legacy AttendancePolicy references are retained for historical rows.
-- New application writes use PolicyVersionId. No guessed legacy-data backfill is
-- performed because the two policy models do not have a guaranteed 1:1 mapping.

BEGIN;

ALTER TABLE axionpro."EmployeeWorkArrangement"
    ALTER COLUMN "AttendancePolicyId" DROP NOT NULL;

ALTER TABLE axionpro."EmployeeWorkArrangement"
    ADD COLUMN IF NOT EXISTS "PolicyVersionId" bigint;

CREATE INDEX IF NOT EXISTS "IX_EmployeeWorkArrangement_PolicyVersionId"
    ON axionpro."EmployeeWorkArrangement" ("PolicyVersionId");

DO $$
BEGIN
    IF NOT EXISTS
    (
        SELECT 1
        FROM pg_constraint
        WHERE conname = 'FK_EmployeeWorkArrangement_PolicyVersion'
          AND conrelid = 'axionpro."EmployeeWorkArrangement"'::regclass
    ) THEN
        ALTER TABLE axionpro."EmployeeWorkArrangement"
            ADD CONSTRAINT "FK_EmployeeWorkArrangement_PolicyVersion"
            FOREIGN KEY ("PolicyVersionId")
            REFERENCES axionpro."PolicyVersion" ("Id")
            ON DELETE RESTRICT;
    END IF;
END $$;

COMMIT;
