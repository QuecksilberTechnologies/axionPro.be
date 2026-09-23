-- Preserves existing holiday rows while renaming the table and database objects.

BEGIN;

DO $migration$
BEGIN
    IF to_regclass('axionpro."Holiday"') IS NOT NULL
       AND to_regclass('axionpro."OrganizationHolidayCalendar"') IS NOT NULL THEN
        RAISE EXCEPTION 'Both axionpro.Holiday and axionpro.OrganizationHolidayCalendar exist; resolve this ambiguity before migration.';
    END IF;

    IF to_regclass('axionpro."Holiday"') IS NULL THEN
        IF to_regclass('axionpro."OrganizationHolidayCalendar"') IS NULL THEN
            RAISE EXCEPTION 'Neither axionpro.Holiday nor axionpro.OrganizationHolidayCalendar exists.';
        END IF;

        ALTER TABLE axionpro."OrganizationHolidayCalendar" RENAME TO "Holiday";
    END IF;
END
$migration$;

ALTER TABLE axionpro."Holiday"
    ADD COLUMN IF NOT EXISTS "Icon" character varying(100);

DO $constraints$
BEGIN
    IF EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'PK__Organiza__3214EC077FBA239C' AND conrelid = 'axionpro."Holiday"'::regclass) THEN
        ALTER TABLE axionpro."Holiday" RENAME CONSTRAINT "PK__Organiza__3214EC077FBA239C" TO "PK_Holiday";
    END IF;

    IF EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'FK_OrganizationHolidayCalendar_Tenant' AND conrelid = 'axionpro."Holiday"'::regclass) THEN
        ALTER TABLE axionpro."Holiday" RENAME CONSTRAINT "FK_OrganizationHolidayCalendar_Tenant" TO "FK_Holiday_Tenant";
    END IF;

    IF EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'FK_OrganizationHolidayCalendar_TenantLocation' AND conrelid = 'axionpro."Holiday"'::regclass) THEN
        ALTER TABLE axionpro."Holiday" RENAME CONSTRAINT "FK_OrganizationHolidayCalendar_TenantLocation" TO "FK_Holiday_TenantLocation";
    END IF;

    IF EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'OrganizationHolidayCalendar_Id_not_null' AND conrelid = 'axionpro."Holiday"'::regclass) THEN
        ALTER TABLE axionpro."Holiday" RENAME CONSTRAINT "OrganizationHolidayCalendar_Id_not_null" TO "Holiday_Id_not_null";
    END IF;

    IF EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'OrganizationHolidayCalendar_HolidayName_not_null' AND conrelid = 'axionpro."Holiday"'::regclass) THEN
        ALTER TABLE axionpro."Holiday" RENAME CONSTRAINT "OrganizationHolidayCalendar_HolidayName_not_null" TO "Holiday_HolidayName_not_null";
    END IF;

    IF EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'OrganizationHolidayCalendar_HolidayDate_not_null' AND conrelid = 'axionpro."Holiday"'::regclass) THEN
        ALTER TABLE axionpro."Holiday" RENAME CONSTRAINT "OrganizationHolidayCalendar_HolidayDate_not_null" TO "Holiday_HolidayDate_not_null";
    END IF;

    IF EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'OrganizationHolidayCalendar_IsOptional_not_null' AND conrelid = 'axionpro."Holiday"'::regclass) THEN
        ALTER TABLE axionpro."Holiday" RENAME CONSTRAINT "OrganizationHolidayCalendar_IsOptional_not_null" TO "Holiday_IsOptional_not_null";
    END IF;

    IF EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'OrganizationHolidayCalendar_TenantLocationId_not_null' AND conrelid = 'axionpro."Holiday"'::regclass) THEN
        ALTER TABLE axionpro."Holiday" RENAME CONSTRAINT "OrganizationHolidayCalendar_TenantLocationId_not_null" TO "Holiday_TenantLocationId_not_null";
    END IF;
END
$constraints$;

DO $sequence$
BEGIN
    IF to_regclass('axionpro."OrganizationHolidayCalendar_Id_seq"') IS NOT NULL
       AND to_regclass('axionpro."Holiday_Id_seq"') IS NULL THEN
        ALTER SEQUENCE axionpro."OrganizationHolidayCalendar_Id_seq" RENAME TO "Holiday_Id_seq";
    END IF;
END
$sequence$;

DO $indexes$
BEGIN
    IF to_regclass('axionpro."IX_OrganizationHolidayCalendar_Location_Date"') IS NOT NULL
       AND to_regclass('axionpro."IX_Holiday_Location_Date"') IS NULL THEN
        ALTER INDEX axionpro."IX_OrganizationHolidayCalendar_Location_Date" RENAME TO "IX_Holiday_Location_Date";
    END IF;

    IF to_regclass('axionpro."UX_OrganizationHolidayCalendar_Tenant_Location_Date_NotDeleted"') IS NOT NULL
       AND to_regclass('axionpro."UX_Holiday_Tenant_Location_Date_NotDeleted"') IS NULL THEN
        ALTER INDEX axionpro."UX_OrganizationHolidayCalendar_Tenant_Location_Date_NotDeleted"
            RENAME TO "UX_Holiday_Tenant_Location_Date_NotDeleted";
    END IF;
END
$indexes$;

COMMIT;
