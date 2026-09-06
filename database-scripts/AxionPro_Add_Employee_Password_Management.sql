-- ============================================================================
-- Author      : Deepesh Gupta
-- Company     : Quecksilber Technologies
-- Role        : CEO
-- Purpose     : Add one Employee Password Management child Module and its
--               required Operation, ModuleOperationMapping and Plan mappings.
-- Reference   : AxionPro_Production_Module_Operation_Seed.sql
-- Safety      : ADDITIVE ONLY. Existing records are not deleted or reseeded.
-- ============================================================================

BEGIN;

-- ============================================================================
-- REGION 1: PRECONDITION
-- ============================================================================

DO $$
DECLARE
    employee_parent_count integer;
BEGIN
    SELECT COUNT(*)
    INTO employee_parent_count
    FROM axionpro."Module"
    WHERE "ModuleCode" = 'EMP_MGMT'
      AND "ParentModuleId" IS NULL
      AND "IsLeafNode" = false
      AND "ModuleScope" = 1;

    IF employee_parent_count <> 1 THEN
        RAISE EXCEPTION
            'Expected exactly one Tenant-scope EMP_MGMT parent Module, found %.',
            employee_parent_count;
    END IF;
END $$;

-- ============================================================================
-- REGION 2: ADD ONE CHILD MODULE
-- This page allows an authorized tenant administrator to reset the password
-- of an employee selected from the Employee List.
-- ============================================================================

INSERT INTO axionpro."Module"
(
    "ModuleCode",
    "ModuleName",
    "DisplayName",
    "URLPath",
    "ParentModuleId",
    "IsLeafNode",
    "IsModuleDisplayInUI",
    "ModuleScope"
)
SELECT
    'EMP_PASSWORD_MANAGEMENT',
    'Employee-Password-Management',
    'Employee Password Management',
    '/employees/password-management',
    parent."Id",
    true,
    true,
    1
FROM axionpro."Module" parent
WHERE parent."ModuleCode" = 'EMP_MGMT'
  AND parent."ParentModuleId" IS NULL
  AND parent."IsLeafNode" = false
  AND parent."ModuleScope" = 1
  AND NOT EXISTS
  (
      SELECT 1
      FROM axionpro."Module" existing
      WHERE existing."ModuleCode" = 'EMP_PASSWORD_MANAGEMENT'
  );

-- Keep the single target record synchronized if the script is run again.
UPDATE axionpro."Module" module
SET
    "ModuleName" = 'Employee-Password-Management',
    "DisplayName" = 'Employee Password Management',
    "URLPath" = '/employees/password-management',
    "ParentModuleId" =
    (
        SELECT parent."Id"
        FROM axionpro."Module" parent
        WHERE parent."ModuleCode" = 'EMP_MGMT'
        LIMIT 1
    ),
    "IsLeafNode" = true,
    "IsModuleDisplayInUI" = true,
    "ModuleScope" = 1
WHERE module."ModuleCode" = 'EMP_PASSWORD_MANAGEMENT';

-- ============================================================================
-- REGION 3: ADD DEDICATED OPERATION
-- Generic Update is intentionally not reused because password reset is a
-- privileged security action that must be granted independently.
-- ============================================================================

INSERT INTO axionpro."Operation"
(
    "OperationName",
    "Remark",
    "OperationType",
    "IsActive",
    "AddedById",
    "AddedDateTime",
    "IconImage"
)
SELECT
    'Reset Password',
    'Reset the login password of a selected tenant employee',
    4,
    true,
    1,
    CURRENT_TIMESTAMP,
    'key-round'
WHERE NOT EXISTS
(
    SELECT 1
    FROM axionpro."Operation"
    WHERE LOWER(TRIM("OperationName")) = LOWER('Reset Password')
);

-- Normalize the dedicated Operation if the script is run again.
UPDATE axionpro."Operation"
SET
    "Remark" = 'Reset the login password of a selected tenant employee',
    "OperationType" = 4,
    "IsActive" = true,
    "IconImage" = 'key-round'
WHERE LOWER(TRIM("OperationName")) = LOWER('Reset Password');

-- ============================================================================
-- REGION 4: MODULE-OPERATION MAPPING
-- Exactly one permission is mapped to this Module.
-- ============================================================================

INSERT INTO axionpro."ModuleOperationMapping"
(
    "ModuleId",
    "OperationId",
    "PageURL",
    "IconURL",
    "IsCommonItem",
    "IsOperational",
    "Priority",
    "Remark",
    "IsActive",
    "AddedById",
    "AddedDateTime"
)
SELECT
    module."Id",
    operation."Id",
    '/employees/password-management',
    'key-round',
    false,
    true,
    1,
    'Allow an authorized tenant administrator to reset an employee password',
    true,
    1,
    CURRENT_TIMESTAMP
FROM axionpro."Module" module
CROSS JOIN axionpro."Operation" operation
WHERE module."ModuleCode" = 'EMP_PASSWORD_MANAGEMENT'
  AND module."IsLeafNode" = true
  AND module."ModuleScope" = 1
  AND LOWER(TRIM(operation."OperationName")) = LOWER('Reset Password')
  AND NOT EXISTS
  (
      SELECT 1
      FROM axionpro."ModuleOperationMapping" existing
      WHERE existing."ModuleId" = module."Id"
        AND existing."OperationId" = operation."Id"
  );

-- Synchronize non-key mapping metadata on repeated execution.
UPDATE axionpro."ModuleOperationMapping" mapping
SET
    "PageURL" = '/employees/password-management',
    "IconURL" = 'key-round',
    "IsCommonItem" = false,
    "IsOperational" = true,
    "Priority" = 1,
    "Remark" = 'Allow an authorized tenant administrator to reset an employee password',
    "IsActive" = true
FROM axionpro."Module" module
INNER JOIN axionpro."Operation" operation
    ON LOWER(TRIM(operation."OperationName")) = LOWER('Reset Password')
WHERE mapping."ModuleId" = module."Id"
  AND mapping."OperationId" = operation."Id"
  AND module."ModuleCode" = 'EMP_PASSWORD_MANAGEMENT';

-- ============================================================================
-- REGION 5: PLAN-MODULE MAPPING
-- The new Module is added only to plans that already contain EMP_LIST.
-- FK names are discovered from PostgreSQL metadata to support PlanId or
-- SubscriptionPlanId without hard-coding the production column name.
-- ============================================================================

DO $$
DECLARE
    plan_fk_column text;
    insert_columns text;
    select_columns text;
    sql_statement text;
    employee_list_plan_count integer;
BEGIN
    SELECT child_attribute.attname
    INTO plan_fk_column
    FROM pg_constraint constraint_info
    INNER JOIN pg_class child_table
        ON child_table.oid = constraint_info.conrelid
    INNER JOIN pg_namespace child_namespace
        ON child_namespace.oid = child_table.relnamespace
    INNER JOIN pg_class parent_table
        ON parent_table.oid = constraint_info.confrelid
    INNER JOIN LATERAL unnest(constraint_info.conkey)
        WITH ORDINALITY AS child_key(attribute_number, position)
        ON true
    INNER JOIN pg_attribute child_attribute
        ON child_attribute.attrelid = child_table.oid
       AND child_attribute.attnum = child_key.attribute_number
    WHERE constraint_info.contype = 'f'
      AND child_namespace.nspname = 'axionpro'
      AND child_table.relname = 'PlanModuleMapping'
      AND parent_table.relname <> 'Module'
    ORDER BY constraint_info.oid
    LIMIT 1;

    IF plan_fk_column IS NULL THEN
        RAISE EXCEPTION
            'Unable to discover PlanModuleMapping plan FK column.';
    END IF;

    EXECUTE format(
        'SELECT COUNT(DISTINCT source.%I)
         FROM axionpro."PlanModuleMapping" source
         INNER JOIN axionpro."Module" source_module
             ON source_module."Id" = source."ModuleId"
         WHERE source_module."ModuleCode" = %L',
        plan_fk_column,
        'EMP_LIST'
    )
    INTO employee_list_plan_count;

    IF employee_list_plan_count = 0 THEN
        RAISE EXCEPTION
            'No EMP_LIST PlanModuleMapping exists. Password Module was not added to any plan.';
    END IF;

    insert_columns := format('%I, %I', plan_fk_column, 'ModuleId');
    select_columns := format('source.%I, target_module."Id"', plan_fk_column);

    IF EXISTS
    (
        SELECT 1 FROM information_schema.columns
        WHERE table_schema = 'axionpro'
          AND table_name = 'PlanModuleMapping'
          AND column_name = 'IsActive'
    ) THEN
        insert_columns := insert_columns || ', "IsActive"';
        select_columns := select_columns || ', true';
    END IF;

    IF EXISTS
    (
        SELECT 1 FROM information_schema.columns
        WHERE table_schema = 'axionpro'
          AND table_name = 'PlanModuleMapping'
          AND column_name = 'AddedById'
    ) THEN
        insert_columns := insert_columns || ', "AddedById"';
        select_columns := select_columns || ', 1';
    END IF;

    IF EXISTS
    (
        SELECT 1 FROM information_schema.columns
        WHERE table_schema = 'axionpro'
          AND table_name = 'PlanModuleMapping'
          AND column_name = 'AddedDateTime'
    ) THEN
        insert_columns := insert_columns || ', "AddedDateTime"';
        select_columns := select_columns || ', CURRENT_TIMESTAMP';
    END IF;

    sql_statement := format(
        'INSERT INTO axionpro."PlanModuleMapping" (%s)
         SELECT DISTINCT %s
         FROM axionpro."PlanModuleMapping" source
         INNER JOIN axionpro."Module" source_module
             ON source_module."Id" = source."ModuleId"
            AND source_module."ModuleCode" = %L
         CROSS JOIN axionpro."Module" target_module
         WHERE target_module."ModuleCode" = %L
           AND NOT EXISTS
           (
               SELECT 1
               FROM axionpro."PlanModuleMapping" existing
               WHERE existing.%I = source.%I
                 AND existing."ModuleId" = target_module."Id"
           )',
        insert_columns,
        select_columns,
        'EMP_LIST',
        'EMP_PASSWORD_MANAGEMENT',
        plan_fk_column,
        plan_fk_column
    );

    EXECUTE sql_statement;
END $$;

-- ============================================================================
-- REGION 6: STRICT FINAL VALIDATION
-- ============================================================================

DO $$
DECLARE
    module_count integer;
    operation_count integer;
    module_operation_count integer;
BEGIN
    SELECT COUNT(*)
    INTO module_count
    FROM axionpro."Module"
    WHERE "ModuleCode" = 'EMP_PASSWORD_MANAGEMENT'
      AND "IsLeafNode" = true
      AND "ModuleScope" = 1;

    SELECT COUNT(*)
    INTO operation_count
    FROM axionpro."Operation"
    WHERE LOWER(TRIM("OperationName")) = LOWER('Reset Password')
      AND "IsActive" = true;

    SELECT COUNT(*)
    INTO module_operation_count
    FROM axionpro."ModuleOperationMapping" mapping
    INNER JOIN axionpro."Module" module
        ON module."Id" = mapping."ModuleId"
    INNER JOIN axionpro."Operation" operation
        ON operation."Id" = mapping."OperationId"
    WHERE module."ModuleCode" = 'EMP_PASSWORD_MANAGEMENT'
      AND LOWER(TRIM(operation."OperationName")) = LOWER('Reset Password')
      AND mapping."IsActive" = true;

    IF module_count <> 1 THEN
        RAISE EXCEPTION
            'Module validation failed. Expected 1, found %.', module_count;
    END IF;

    IF operation_count <> 1 THEN
        RAISE EXCEPTION
            'Operation validation failed. Expected 1, found %.', operation_count;
    END IF;

    IF module_operation_count <> 1 THEN
        RAISE EXCEPTION
            'ModuleOperationMapping validation failed. Expected 1, found %.',
            module_operation_count;
    END IF;
END $$;

COMMIT;

-- ============================================================================
-- READ-ONLY VERIFICATION
-- ============================================================================

SELECT
    module."Id" AS "ModuleId",
    module."ModuleCode",
    module."DisplayName",
    module."URLPath",
    module."ParentModuleId",
    module."ModuleScope",
    operation."Id" AS "OperationId",
    operation."OperationName",
    mapping."Id" AS "ModuleOperationMappingId",
    mapping."PageURL",
    mapping."IconURL",
    mapping."Priority",
    mapping."Remark",
    mapping."IsActive"
FROM axionpro."Module" module
INNER JOIN axionpro."ModuleOperationMapping" mapping
    ON mapping."ModuleId" = module."Id"
INNER JOIN axionpro."Operation" operation
    ON operation."Id" = mapping."OperationId"
WHERE module."ModuleCode" = 'EMP_PASSWORD_MANAGEMENT';

SELECT
    plan_mapping.*,
    module."ModuleCode",
    module."DisplayName"
FROM axionpro."PlanModuleMapping" plan_mapping
INNER JOIN axionpro."Module" module
    ON module."Id" = plan_mapping."ModuleId"
WHERE module."ModuleCode" = 'EMP_PASSWORD_MANAGEMENT'
ORDER BY plan_mapping."Id";

-- IMPORTANT:
-- RoleModuleAndPermission is intentionally not auto-seeded. Reset Password is
-- security-sensitive and must be granted only to the intended Tenant Admin role.
-- Existing tenants must run the normal plan-to-tenant module/operation sync after
-- this migration so TenantEnabledModule and TenantEnabledOperation remain governed
-- by the application's established onboarding/synchronization flow.
