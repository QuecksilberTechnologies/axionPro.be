-- Final verification for the canonical production seed.
-- This script is read-only and fails when the expected clean baseline is not present.
BEGIN;

DO $$
DECLARE
    host_count bigint;
    tenant_count bigint;
    employee_count bigint;
    unexpected_host_count bigint;
    duplicate_login_count bigint;
BEGIN
    SELECT count(*) INTO host_count FROM axionpro."HostUser"
    WHERE "IsActive" AND NOT "IsSoftDeleted";

    SELECT count(*) INTO unexpected_host_count FROM axionpro."HostUser"
    WHERE lower(btrim("LoginId")) NOT IN
          ('mca.deepesh@gmail.com', 'sujeet@axionpro.com');

    SELECT count(*) INTO duplicate_login_count
    FROM
    (
        SELECT lower(btrim("LoginId"))
        FROM axionpro."HostUser"
        GROUP BY lower(btrim("LoginId"))
        HAVING count(*) > 1
    ) duplicate_login;

    SELECT count(*) INTO tenant_count FROM axionpro."Tenant";
    SELECT count(*) INTO employee_count FROM axionpro."Employee";

    IF host_count <> 2 OR unexpected_host_count <> 0 OR duplicate_login_count <> 0 THEN
        RAISE EXCEPTION
            'Host verification failed: active %, unexpected %, duplicate logins %.',
            host_count,
            unexpected_host_count,
            duplicate_login_count;
    END IF;

    IF tenant_count <> 0 OR employee_count <> 0 THEN
        RAISE EXCEPTION
            'Business-data cleanup failed: tenants %, employees %.',
            tenant_count,
            employee_count;
    END IF;

    IF NOT EXISTS
    (
        SELECT 1
        FROM axionpro."HostRole" role
        WHERE role."Id" = 1
          AND role."Name" = 'Host-Super-Admin'
          AND role."IsActive"
          AND NOT role."IsSoftDeleted"
    ) THEN
        RAISE EXCEPTION 'Canonical Host-Super-Admin role is missing.';
    END IF;

    IF NOT EXISTS (SELECT 1 FROM axionpro."HostRoleModuleAndPermission") THEN
        RAISE EXCEPTION 'Host permission baseline is empty.';
    END IF;

    IF NOT EXISTS (SELECT 1 FROM axionpro."Module")
       OR NOT EXISTS (SELECT 1 FROM axionpro."Operation")
       OR NOT EXISTS (SELECT 1 FROM axionpro."ModuleOperationMapping") THEN
        RAISE EXCEPTION 'Module/operation master seed is incomplete.';
    END IF;

    IF NOT EXISTS
    (
        SELECT 1
        FROM axionpro."EmployeeType"
        WHERE "Id" = 1
          AND "TenantId" IS NULL
          AND "TypeName" = 'Permanent'
          AND "IsActive"
          AND NOT "IsSoftDeleted"
    ) THEN
        RAISE EXCEPTION 'Global Permanent employee onboarding template is missing.';
    END IF;

    IF (SELECT count(*) FROM axionpro."Country"
        WHERE "CountryCode" IN ('IN', 'CN', 'DE', 'US')) <> 4 THEN
        RAISE EXCEPTION 'Four-country geography seed is incomplete.';
    END IF;
END $$;

SELECT "Id", "Name", "LoginId", "HostRoleId", "IsActive", "IsSoftDeleted"
FROM axionpro."HostUser"
ORDER BY "Id";

SELECT
    (SELECT count(*) FROM axionpro."Module") AS "ModuleCount",
    (SELECT count(*) FROM axionpro."Operation") AS "OperationCount",
    (SELECT count(*) FROM axionpro."ModuleOperationMapping") AS "ModuleOperationCount",
    (SELECT count(*) FROM axionpro."Country") AS "CountryCount",
    (SELECT count(*) FROM axionpro."State") AS "StateCount",
    (SELECT count(*) FROM axionpro."District") AS "DistrictCount",
    (SELECT count(*) FROM axionpro."Locality") AS "LocalityCount";

ROLLBACK;
