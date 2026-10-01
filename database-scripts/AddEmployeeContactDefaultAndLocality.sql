-- ================================================================
-- Purpose : Supports the default EmployeeContact row and locality selection.
-- Safety  : Additive/idempotent; existing contact values are not rewritten.
-- ================================================================

BEGIN;

ALTER TABLE axionpro."EmployeeContact"
    ALTER COLUMN "ContactNumber" DROP NOT NULL,
    ALTER COLUMN "ContactName" TYPE varchar(302);

ALTER TABLE axionpro."EmployeeContact"
    ADD COLUMN IF NOT EXISTS "LocalityId" integer NULL;

DO $$
BEGIN
    IF NOT EXISTS
    (
        SELECT 1
        FROM pg_constraint
        WHERE conname = 'FK_EmployeeContact_Locality'
          AND conrelid = 'axionpro."EmployeeContact"'::regclass
    ) THEN
        ALTER TABLE axionpro."EmployeeContact"
            ADD CONSTRAINT "FK_EmployeeContact_Locality"
            FOREIGN KEY ("LocalityId")
            REFERENCES axionpro."Locality" ("Id")
            ON DELETE SET NULL;
    END IF;
END
$$;

CREATE INDEX IF NOT EXISTS "IX_EmployeeContact_LocalityId"
    ON axionpro."EmployeeContact" ("LocalityId");

COMMIT;
