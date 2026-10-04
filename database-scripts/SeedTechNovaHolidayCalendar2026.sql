\set ON_ERROR_STOP on

-- Idempotent TechNova 2026 calendar seed.
-- Tenant and location ownership are resolved by stable TenantCode rather than environment-specific IDs.
BEGIN;

SELECT pg_advisory_xact_lock(20261004, 2026);

CREATE TEMP TABLE holiday_source (
    "LegacyLocationId" bigint NOT NULL,
    "HolidayName" varchar(100) NOT NULL,
    "HolidayDate" date NOT NULL,
    "IsOptional" boolean NOT NULL,
    "Description" varchar(255),
    "Icon" varchar(100)
) ON COMMIT DROP;

\copy holiday_source ("LegacyLocationId", "HolidayName", "HolidayDate", "IsOptional", "Description", "Icon") FROM 'docs/testing/holiday-calendar/live-2026-jabalpur/holidays-2026.csv' WITH (FORMAT csv, HEADER true, ENCODING 'UTF8')

DO $$
BEGIN
    IF (SELECT count(*) FROM axionpro."Tenant" WHERE "TenantCode" = 'TS') <> 1 THEN
        RAISE EXCEPTION 'Expected exactly one Tenant with TenantCode TS';
    END IF;

    IF (SELECT count(*) FROM holiday_source) <> 31
       OR (SELECT count(DISTINCT extract(month FROM "HolidayDate")) FROM holiday_source) <> 12
       OR (SELECT count(*) FROM holiday_source WHERE "IsOptional") <> 2
       OR EXISTS (SELECT 1 FROM holiday_source WHERE extract(year FROM "HolidayDate") <> 2026)
    THEN
        RAISE EXCEPTION 'Holiday source failed count, month, optional or year validation';
    END IF;

    IF EXISTS (
        SELECT 1
        FROM holiday_source
        GROUP BY "HolidayDate"
        HAVING count(*) > 1
    ) THEN
        RAISE EXCEPTION 'Holiday source contains duplicate dates';
    END IF;
END $$;

INSERT INTO axionpro."TenantLocation"
    ("TenantId", "LocationCode", "LocationName", "LocationType", "CountryId",
     "TimeZoneId", "IsHeadOffice", "IsGeoFenceEnabled", "IsAttendanceAllowed",
     "IsBiometricEnabled", "IsActive", "IsSoftDeleted", "AddedById", "AddedDateTime")
SELECT tenant."Id", 'PRIMARY', tenant."CompanyName", 1, tenant."CountryId",
       'UTC', true, false, false, false, true, false, tenant."Id", now()
FROM axionpro."Tenant" AS tenant
WHERE tenant."TenantCode" = 'TS'
  AND NOT EXISTS (
      SELECT 1
      FROM axionpro."TenantLocation" AS existing
      WHERE existing."TenantId" = tenant."Id"
        AND existing."IsSoftDeleted" = false
  );

CREATE TEMP TABLE target_holiday_scope ON COMMIT DROP AS
SELECT tenant."Id" AS "TenantId", location."Id" AS "TenantLocationId"
FROM axionpro."Tenant" AS tenant
JOIN LATERAL (
    SELECT candidate."Id"
    FROM axionpro."TenantLocation" AS candidate
    WHERE candidate."TenantId" = tenant."Id"
      AND candidate."IsActive" = true
      AND candidate."IsSoftDeleted" = false
    ORDER BY candidate."IsHeadOffice" DESC, candidate."AddedDateTime", candidate."Id"
    LIMIT 1
) AS location ON true
WHERE tenant."TenantCode" = 'TS';

DO $$
BEGIN
    IF (SELECT count(*) FROM target_holiday_scope) <> 1 THEN
        RAISE EXCEPTION 'TechNova does not have exactly one resolved active target location';
    END IF;
END $$;

INSERT INTO axionpro."Holiday"
    ("TenantId", "TenantLocationId", "HolidayName", "HolidayDate", "IsOptional",
     "Description", "Icon", "IsActive", "IsSoftDeleted", "AddedDateTime")
SELECT scope."TenantId", scope."TenantLocationId", source."HolidayName", source."HolidayDate",
       source."IsOptional", source."Description", source."Icon", true, false, now()
FROM holiday_source AS source
CROSS JOIN target_holiday_scope AS scope
WHERE NOT EXISTS (
    SELECT 1
    FROM axionpro."Holiday" AS existing
    WHERE existing."TenantId" = scope."TenantId"
      AND existing."TenantLocationId" = scope."TenantLocationId"
      AND existing."HolidayDate" = source."HolidayDate"
      AND existing."IsSoftDeleted" IS DISTINCT FROM true
);

DO $$
BEGIN
    IF (
        SELECT count(*)
        FROM axionpro."Holiday" AS holiday
        CROSS JOIN target_holiday_scope AS scope
        WHERE holiday."TenantId" = scope."TenantId"
          AND holiday."TenantLocationId" = scope."TenantLocationId"
          AND extract(year FROM holiday."HolidayDate") = 2026
          AND holiday."IsSoftDeleted" = false
    ) <> 31 THEN
        RAISE EXCEPTION 'Final TechNova 2026 holiday count is not 31';
    END IF;
END $$;

COMMIT;
