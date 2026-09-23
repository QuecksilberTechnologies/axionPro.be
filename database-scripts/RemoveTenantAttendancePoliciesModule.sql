BEGIN;

DO $precondition$
BEGIN
    IF EXISTS
    (
        SELECT 1 FROM axionpro."Module"
        WHERE "ParentModuleId" = 67
    ) THEN
        RAISE EXCEPTION 'Module Id 67 has child modules and cannot be removed.';
    END IF;
END
$precondition$;

DELETE FROM axionpro."HostRoleModuleAndPermission" WHERE "ModuleId" = 67;
DELETE FROM axionpro."RoleModuleAndPermission" WHERE "ModuleId" = 67;
DELETE FROM axionpro."TenantEnabledOperation" WHERE "ModuleId" = 67;
DELETE FROM axionpro."TenantEnabledModule" WHERE "ModuleId" = 67;
DELETE FROM axionpro."PlanModuleMapping" WHERE "ModuleId" = 67;
DELETE FROM axionpro."ModuleOperationMapping" WHERE "ModuleId" = 67;
DELETE FROM axionpro."Module"
WHERE "Id" = 67
  AND "ModuleCode" = 'TENANT_ATTENDANCE_POLICIES';

DO $verification$
BEGIN
    IF EXISTS
    (
        SELECT 1 FROM axionpro."Module"
        WHERE "Id" = 67 OR "ModuleCode" = 'TENANT_ATTENDANCE_POLICIES'
    )
    OR EXISTS (SELECT 1 FROM axionpro."ModuleOperationMapping" WHERE "ModuleId" = 67)
    OR EXISTS (SELECT 1 FROM axionpro."TenantEnabledModule" WHERE "ModuleId" = 67)
    OR EXISTS (SELECT 1 FROM axionpro."TenantEnabledOperation" WHERE "ModuleId" = 67)
    OR EXISTS (SELECT 1 FROM axionpro."PlanModuleMapping" WHERE "ModuleId" = 67)
    OR EXISTS (SELECT 1 FROM axionpro."RoleModuleAndPermission" WHERE "ModuleId" = 67)
    OR EXISTS (SELECT 1 FROM axionpro."HostRoleModuleAndPermission" WHERE "ModuleId" = 67) THEN
        RAISE EXCEPTION 'Tenant Attendance Policies Module Id 67 cleanup verification failed.';
    END IF;
END
$verification$;

COMMIT;
