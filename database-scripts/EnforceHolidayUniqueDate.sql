BEGIN;

DO $$
BEGIN
    IF EXISTS (
        SELECT 1
        FROM axionpro."Holiday"
        WHERE "IsSoftDeleted" IS DISTINCT FROM TRUE
        GROUP BY "TenantId", "TenantLocationId", "HolidayDate"
        HAVING count(*) > 1
    ) THEN
        RAISE EXCEPTION 'Resolve duplicate non-deleted tenant/location/date holidays before adding the unique index';
    END IF;
END $$;

CREATE UNIQUE INDEX IF NOT EXISTS
    "UX_Holiday_Tenant_Location_Date_NotDeleted"
ON axionpro."Holiday"
    ("TenantId", "TenantLocationId", "HolidayDate")
WHERE "IsSoftDeleted" IS DISTINCT FROM TRUE;

COMMIT;
