-- Refactors holidays to use a TenantLocation FK and a local business date.

BEGIN;

DO $migration$
DECLARE
    existing_rows bigint;
BEGIN
    IF to_regclass('axionpro."OrganizationHolidayCalendar"') IS NULL THEN
        RAISE EXCEPTION 'axionpro.OrganizationHolidayCalendar does not exist.';
    END IF;

    IF NOT EXISTS
    (
        SELECT 1
        FROM information_schema.columns
        WHERE table_schema = 'axionpro'
          AND table_name = 'OrganizationHolidayCalendar'
          AND column_name = 'TenantLocationId'
    ) THEN
        SELECT count(*)
        INTO existing_rows
        FROM axionpro."OrganizationHolidayCalendar";

        IF existing_rows > 0 THEN
            RAISE EXCEPTION
                'Migration stopped: % holiday rows require an explicit TenantLocation mapping.',
                existing_rows;
        END IF;

        ALTER TABLE axionpro."OrganizationHolidayCalendar"
            ADD COLUMN "TenantLocationId" bigint NOT NULL;
    END IF;

    SELECT count(*)
    INTO existing_rows
    FROM axionpro."OrganizationHolidayCalendar"
    WHERE "TenantLocationId" IS NULL;

    IF existing_rows > 0 THEN
        RAISE EXCEPTION
            'Migration stopped: % holiday rows require an explicit TenantLocation mapping.',
            existing_rows;
    END IF;

    ALTER TABLE axionpro."OrganizationHolidayCalendar"
        ALTER COLUMN "TenantLocationId" SET NOT NULL;
END
$migration$;

DO $holiday_date$
BEGIN
    IF EXISTS
    (
        SELECT 1
        FROM information_schema.columns
        WHERE table_schema = 'axionpro'
          AND table_name = 'OrganizationHolidayCalendar'
          AND column_name = 'HolidayDate'
          AND data_type <> 'date'
    ) THEN
        ALTER TABLE axionpro."OrganizationHolidayCalendar"
            ALTER COLUMN "HolidayDate" TYPE date
            USING ("HolidayDate" AT TIME ZONE 'UTC')::date;
    END IF;
END
$holiday_date$;

ALTER TABLE axionpro."OrganizationHolidayCalendar"
    DROP COLUMN IF EXISTS "CountryCode",
    DROP COLUMN IF EXISTS "StateCode",
    DROP COLUMN IF EXISTS "HolidayYear",
    DROP COLUMN IF EXISTS "Remark";

DO $constraint$
BEGIN
    IF NOT EXISTS
    (
        SELECT 1
        FROM pg_constraint
        WHERE conname = 'FK_OrganizationHolidayCalendar_TenantLocation'
          AND conrelid = 'axionpro."OrganizationHolidayCalendar"'::regclass
    ) THEN
        ALTER TABLE axionpro."OrganizationHolidayCalendar"
            ADD CONSTRAINT "FK_OrganizationHolidayCalendar_TenantLocation"
            FOREIGN KEY ("TenantLocationId")
            REFERENCES axionpro."TenantLocation" ("Id")
            ON DELETE RESTRICT;
    END IF;
END
$constraint$;

CREATE INDEX IF NOT EXISTS "IX_OrganizationHolidayCalendar_Location_Date"
    ON axionpro."OrganizationHolidayCalendar" ("TenantLocationId", "HolidayDate");

COMMIT;
