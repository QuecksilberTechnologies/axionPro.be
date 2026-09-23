-- Removes the standalone EMP_PASSWORD_MANAGEMENT module and moves its
-- Reset Password permission to the existing EMP_LIST module.

BEGIN;

DO $preconditions$
DECLARE
    employee_list_id integer;
    reset_password_id integer;
    missing_tenant_count integer;
    missing_plan_count integer;
BEGIN
    SELECT "Id" INTO employee_list_id
    FROM axionpro."Module"
    WHERE "ModuleCode" = 'EMP_LIST';

    SELECT "Id" INTO reset_password_id
    FROM axionpro."Operation"
    WHERE "Id" = 21
      AND lower(btrim("OperationName")) = 'reset password';

    IF employee_list_id IS NULL THEN
        RAISE EXCEPTION 'EMP_LIST module was not found.';
    END IF;

    IF reset_password_id IS NULL THEN
        RAISE EXCEPTION 'Operation Id 21 must be Reset Password.';
    END IF;

    SELECT count(*) INTO missing_tenant_count
    FROM axionpro."TenantEnabledModule" legacy
    WHERE legacy."ModuleId" = 38
      AND NOT EXISTS
      (
          SELECT 1 FROM axionpro."TenantEnabledModule" employee_list
          WHERE employee_list."TenantId" = legacy."TenantId"
            AND employee_list."ModuleId" = employee_list_id
      );

    IF missing_tenant_count <> 0 THEN
        RAISE EXCEPTION '% tenant(s) have Module 38 without EMP_LIST entitlement.', missing_tenant_count;
    END IF;

    SELECT count(*) INTO missing_plan_count
    FROM axionpro."PlanModuleMapping" legacy
    WHERE legacy."ModuleId" = 38
      AND NOT EXISTS
      (
          SELECT 1 FROM axionpro."PlanModuleMapping" employee_list
          WHERE employee_list."SubscriptionPlanId" = legacy."SubscriptionPlanId"
            AND employee_list."ModuleId" = employee_list_id
      );

    IF missing_plan_count <> 0 THEN
        RAISE EXCEPTION '% plan(s) have Module 38 without EMP_LIST entitlement.', missing_plan_count;
    END IF;
END
$preconditions$;

UPDATE axionpro."Operation"
SET "Remark" = 'Reset the login password of a selected tenant employee',
    "OperationType" = 2,
    "IsActive" = true,
    "IconImage" = 'key-round',
    "UpdatedById" = 1,
    "UpdatedDateTime" = CURRENT_TIMESTAMP
WHERE "Id" = 21
  AND lower(btrim("OperationName")) = 'reset password';

INSERT INTO axionpro."ModuleOperationMapping"
(
    "ModuleId", "OperationId", "PageURL", "IconURL", "IsCommonItem",
    "IsOperational", "Priority", "Remark", "IsActive", "AddedById", "AddedDateTime"
)
SELECT module."Id", 21, module."URLPath", 'key-round', false, true, 10,
       'Reset a selected employee password from the Employees module.',
       true, 1, CURRENT_TIMESTAMP
FROM axionpro."Module" module
WHERE module."ModuleCode" = 'EMP_LIST'
  AND NOT EXISTS
  (
      SELECT 1 FROM axionpro."ModuleOperationMapping" existing
      WHERE existing."ModuleId" = module."Id" AND existing."OperationId" = 21
  );

UPDATE axionpro."ModuleOperationMapping" mapping
SET "PageURL" = module."URLPath",
    "IconURL" = 'key-round',
    "IsCommonItem" = false,
    "IsOperational" = true,
    "Priority" = 10,
    "Remark" = 'Reset a selected employee password from the Employees module.',
    "IsActive" = true,
    "UpdatedById" = 1,
    "UpdatedDateTime" = CURRENT_TIMESTAMP
FROM axionpro."Module" module
WHERE module."ModuleCode" = 'EMP_LIST'
  AND mapping."ModuleId" = module."Id"
  AND mapping."OperationId" = 21;

INSERT INTO axionpro."TenantEnabledOperation"
(
    "TenantId", "ModuleId", "OperationId", "IsOperationUsed", "IsEnabled",
    "AddedById", "AddedDateTime", "UpdatedById", "UpdatedDateTime"
)
SELECT legacy."TenantId", employee_list."Id", 21,
       legacy."IsOperationUsed", legacy."IsEnabled",
       legacy."AddedById", legacy."AddedDateTime", 1, CURRENT_TIMESTAMP
FROM axionpro."TenantEnabledOperation" legacy
CROSS JOIN axionpro."Module" employee_list
WHERE legacy."ModuleId" = 38
  AND legacy."OperationId" = 21
  AND employee_list."ModuleCode" = 'EMP_LIST'
  AND NOT EXISTS
  (
      SELECT 1 FROM axionpro."TenantEnabledOperation" existing
      WHERE existing."TenantId" = legacy."TenantId"
        AND existing."ModuleId" = employee_list."Id"
        AND existing."OperationId" = 21
  );

INSERT INTO axionpro."RoleModuleAndPermission"
(
    "RoleId", "ModuleId", "OperationId", "HasAccess", "IsActive", "Remark",
    "IsOperational", "ImageIcon", "AddedById", "AddedDateTime",
    "UpdatedById", "UpdateDateTime", "SoftDeletedById", "DeletedDateTime",
    "IsSoftDeleted", "UpdatedDateTime"
)
SELECT legacy."RoleId", employee_list."Id", 21, legacy."HasAccess", legacy."IsActive",
       'Reset Password permission moved from retired Module 38 to EMP_LIST.',
       legacy."IsOperational", legacy."ImageIcon", legacy."AddedById", legacy."AddedDateTime",
       1, CURRENT_TIMESTAMP, legacy."SoftDeletedById", legacy."DeletedDateTime",
       legacy."IsSoftDeleted", CURRENT_TIMESTAMP
FROM axionpro."RoleModuleAndPermission" legacy
CROSS JOIN axionpro."Module" employee_list
WHERE legacy."ModuleId" = 38
  AND legacy."OperationId" = 21
  AND employee_list."ModuleCode" = 'EMP_LIST'
  AND NOT EXISTS
  (
      SELECT 1 FROM axionpro."RoleModuleAndPermission" existing
      WHERE existing."RoleId" = legacy."RoleId"
        AND existing."ModuleId" = employee_list."Id"
        AND existing."OperationId" = 21
        AND existing."IsSoftDeleted" IS DISTINCT FROM true
  );

DELETE FROM axionpro."HostRoleModuleAndPermission" WHERE "ModuleId" = 38;
DELETE FROM axionpro."RoleModuleAndPermission" WHERE "ModuleId" = 38;
DELETE FROM axionpro."TenantEnabledOperation" WHERE "ModuleId" = 38;
DELETE FROM axionpro."TenantEnabledModule" WHERE "ModuleId" = 38;
DELETE FROM axionpro."PlanModuleMapping" WHERE "ModuleId" = 38;
DELETE FROM axionpro."ModuleOperationMapping" WHERE "ModuleId" = 38;

DO $children$
BEGIN
    IF EXISTS (SELECT 1 FROM axionpro."Module" WHERE "ParentModuleId" = 38) THEN
        RAISE EXCEPTION 'Module 38 still has child modules and cannot be deleted.';
    END IF;
END
$children$;

DELETE FROM axionpro."Module" WHERE "Id" = 38 OR "ModuleCode" = 'EMP_PASSWORD_MANAGEMENT';

DO $verification$
DECLARE
    employee_list_id integer;
BEGIN
    SELECT "Id" INTO employee_list_id FROM axionpro."Module" WHERE "ModuleCode" = 'EMP_LIST';

    IF EXISTS (SELECT 1 FROM axionpro."Module" WHERE "Id" = 38 OR "ModuleCode" = 'EMP_PASSWORD_MANAGEMENT')
       OR EXISTS (SELECT 1 FROM axionpro."ModuleOperationMapping" WHERE "ModuleId" = 38)
       OR EXISTS (SELECT 1 FROM axionpro."TenantEnabledModule" WHERE "ModuleId" = 38)
       OR EXISTS (SELECT 1 FROM axionpro."TenantEnabledOperation" WHERE "ModuleId" = 38)
       OR EXISTS (SELECT 1 FROM axionpro."RoleModuleAndPermission" WHERE "ModuleId" = 38)
       OR EXISTS (SELECT 1 FROM axionpro."PlanModuleMapping" WHERE "ModuleId" = 38) THEN
        RAISE EXCEPTION 'Module 38 cleanup verification failed.';
    END IF;

    IF NOT EXISTS
    (
        SELECT 1 FROM axionpro."ModuleOperationMapping"
        WHERE "ModuleId" = employee_list_id AND "OperationId" = 21 AND "IsActive" = true
    ) THEN
        RAISE EXCEPTION 'EMP_LIST Reset Password mapping was not created.';
    END IF;
END
$verification$;

COMMIT;
