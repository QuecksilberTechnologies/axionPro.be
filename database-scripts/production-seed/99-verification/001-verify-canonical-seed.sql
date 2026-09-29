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
        FROM axionpro."ModuleOperationMapping" mapping
        INNER JOIN axionpro."Module" module
            ON module."Id" = mapping."ModuleId"
        INNER JOIN axionpro."Operation" operation
            ON operation."Id" = mapping."OperationId"
        WHERE module."ModuleCode" = 'HOST_TENANT_CREATE'
          AND operation."OperationType" = 1
          AND LOWER(BTRIM(operation."OperationName")) = 'add'
          AND mapping."IsActive"
    ) THEN
        RAISE EXCEPTION 'HOST_TENANT_CREATE/Add mapping is missing.';
    END IF;

    IF EXISTS
    (
        SELECT 1
        FROM axionpro."ModuleOperationMapping" mapping
        INNER JOIN axionpro."Module" module
            ON module."Id" = mapping."ModuleId"
        INNER JOIN axionpro."Operation" operation
            ON operation."Id" = mapping."OperationId"
        WHERE module."ModuleCode" = 'HOST_TENANT_LIST'
          AND operation."OperationType" = 1
          AND LOWER(BTRIM(operation."OperationName")) = 'add'
    ) THEN
        RAISE EXCEPTION 'Obsolete HOST_TENANT_LIST/Add mapping still exists.';
    END IF;

    IF
    (
        SELECT COUNT(DISTINCT LOWER(BTRIM(operation."OperationName")))
        FROM axionpro."ModuleOperationMapping" mapping
        INNER JOIN axionpro."Module" module
            ON module."Id" = mapping."ModuleId"
        INNER JOIN axionpro."Operation" operation
            ON operation."Id" = mapping."OperationId"
        WHERE module."ModuleCode" = 'HOST_TENANT_LIST'
          AND LOWER(BTRIM(operation."OperationName")) IN
              ('view', 'update', 'delete', 'active', 'inactive', 'restore')
          AND mapping."IsActive"
    ) <> 6 THEN
        RAISE EXCEPTION 'HOST_TENANT_LIST lifecycle mappings are incomplete.';
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

    IF (SELECT count(*)
        FROM axionpro."EmployeeType"
        WHERE "TenantId" IS NULL
          AND "TypeName" IN
              ('Permanent', 'Contract', 'Intern', 'Part-Time', 'Freelancer', 'Probationer')
          AND "IsActive"
          AND NOT "IsSoftDeleted") <> 6 THEN
        RAISE EXCEPTION 'Canonical global employee type catalogue is incomplete.';
    END IF;

    IF (SELECT count(*)
        FROM axionpro."SubscriptionPlan"
        WHERE "IsActive"
          AND NOT "IsSoftDeleted") <> 3 THEN
        RAISE EXCEPTION 'Exactly three active subscription plans are required.';
    END IF;

    IF (SELECT count(*) FROM axionpro."AttendanceDeviceType"
        WHERE "DeviceTypeCode" IN ('BIOMETRIC', 'WEB', 'MOBILE', 'MANUAL')
          AND "IsActive") <> 4 THEN
        RAISE EXCEPTION 'Attendance device type seed is incomplete.';
    END IF;

    IF (SELECT count(*) FROM axionpro."DefaultEmailConfig"
        WHERE "IsActive" AND "IsDefault") <> 1 THEN
        RAISE EXCEPTION 'Exactly one active default email configuration is required.';
    END IF;

    IF NOT EXISTS
    (
        SELECT 1
        FROM axionpro."DefaultEmailConfig"
        WHERE "ConfigName" = 'DEFAULT_REGISTRATION_SMTP'
          AND "IsActive"
          AND "IsDefault"
          AND NULLIF(BTRIM("SmtpHost"), '') IS NOT NULL
          AND "SmtpPort" BETWEEN 1 AND 65535
          AND NULLIF(BTRIM("SmtpUsername"), '') IS NOT NULL
          AND NULLIF(BTRIM("SmtpPasswordEncrypted"), '') IS NOT NULL
          AND NULLIF(BTRIM("FromEmail"), '') IS NOT NULL
          AND NULLIF(BTRIM("FromName"), '') IS NOT NULL
          AND NULLIF(BTRIM("SecrateKey"), '') IS NOT NULL
    ) THEN
        RAISE EXCEPTION 'Canonical default email configuration is incomplete.';
    END IF;

    IF (SELECT count(*) FROM axionpro."ComplianceTypeMaster") <> 1018
       OR (SELECT count(*) FROM axionpro."StatutoryType") <> 22 THEN
        RAISE EXCEPTION 'Compliance or statutory master seed is incomplete.';
    END IF;

    IF EXISTS
    (
        SELECT 1
        FROM axionpro."Country" country
        WHERE country."IsActive"
          AND
          (
              (SELECT count(*)
               FROM axionpro."ComplianceTypeMaster" compliance
               WHERE compliance."CountryId" = country."Id"
                 AND compliance."IsActive") < 4
              OR
              (country."CountryCode" IN ('IN', 'CN', 'DE', 'US') AND NOT EXISTS
              (
                  SELECT 1
                  FROM axionpro."StatutoryType" statutory
                  WHERE statutory."CountryId" = country."Id"
                    AND statutory."IsActive"
              ))
          )
    ) THEN
        RAISE EXCEPTION 'A supported country is missing compliance or statutory data.';
    END IF;

    IF NOT EXISTS (SELECT 1 FROM axionpro."IdentityCategory")
       OR NOT EXISTS (SELECT 1 FROM axionpro."IdentityCategoryDocument")
       OR NOT EXISTS (SELECT 1 FROM axionpro."CountryIdentityRule") THEN
        RAISE EXCEPTION 'Employee identity master catalogue is incomplete.';
    END IF;

    IF (SELECT count(*) FROM axionpro."Country" WHERE "IsActive") <> 249 THEN
        RAISE EXCEPTION 'ISO country catalogue is incomplete.';
    END IF;

    IF (SELECT count(*) FROM axionpro."Country"
        WHERE "CountryCode" IN ('IN', 'CN', 'DE', 'US')) <> 4 THEN
        RAISE EXCEPTION 'Detailed four-country geography seed is incomplete.';
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
    (SELECT count(*) FROM axionpro."ComplianceTypeMaster") AS "ComplianceTypeCount",
    (SELECT count(*) FROM axionpro."StatutoryType") AS "StatutoryTypeCount",
    (SELECT count(*) FROM axionpro."SubscriptionPlan") AS "SubscriptionPlanCount",
    (SELECT count(*) FROM axionpro."EmployeeType" WHERE "TenantId" IS NULL) AS "GlobalEmployeeTypeCount",
    (SELECT count(*) FROM axionpro."State") AS "StateCount",
    (SELECT count(*) FROM axionpro."District") AS "DistrictCount",
    (SELECT count(*) FROM axionpro."Locality") AS "LocalityCount";

ROLLBACK;
