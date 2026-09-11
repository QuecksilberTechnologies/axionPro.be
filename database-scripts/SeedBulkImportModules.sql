-- Bulk import module/operation catalogue seed.
-- Idempotent: safe to run more than once. Does not grant a tenant role access.
-- Run after the canonical module/operation seed and before entitlement sync.
BEGIN;

LOCK TABLE axionpro."Module", axionpro."Operation",
    axionpro."ModuleOperationMapping", axionpro."PlanModuleMapping"
    IN SHARE ROW EXCLUSIVE MODE;

DO $bulk_seed$
DECLARE
    tenant_parent_id integer;
    import_operation_id integer;
BEGIN
    SELECT "Id" INTO tenant_parent_id
    FROM axionpro."Module"
    WHERE "ModuleCode" = 'TENANT_MGMT' AND "ModuleScope" = 1
    ORDER BY "Id" LIMIT 1;

    IF tenant_parent_id IS NULL THEN
        RAISE EXCEPTION 'TENANT_MGMT baseline module is required before bulk module seeding.';
    END IF;

    -- The existing four master modules may already be present in a deployment.
    -- Insert only missing catalogue rows; stable ModuleCode is the identity.
    INSERT INTO axionpro."Module"
    ("TenantId","ModuleCode","ModuleName","DisplayName","URLPath","ParentModuleId",
     "IsLeafNode","IsModuleDisplayInUI","IsCommonMenu","IsActive","ItemPriority",
     "AddedById","AddedDateTime","ModuleScope","PageName")
    SELECT NULL, seed.code, seed.name, seed.display_name, seed.url, tenant_parent_id,
           TRUE, TRUE, FALSE, TRUE, seed.priority, 1, CURRENT_TIMESTAMP, 1, seed.page_name
    FROM (VALUES
        ('TENANT_DEPARTMENTS','Departments','Departments','/departments',510,'tenant-departments'),
        ('TENANT_DESIGNATIONS','Designations','Designations','/designations',520,'tenant-designations'),
        ('TENANT_ROLES_PERMISSIONS','Roles-Permissions','Roles & Permissions','/roles',530,'tenant-roles-permissions'),
        ('TENANT_EMPLOYEE_TYPES','Employee-Types','Employee Types','/employee-types',540,'tenant-employee-types'),
        ('TENANT_EMPLOYEE_CODE','Employee-Code-Pattern','Employee Code Pattern','/tenant/employee-code-pattern',550,'tenant-employee-code')
    ) AS seed(code,name,display_name,url,priority,page_name)
    WHERE NOT EXISTS
    (
        SELECT 1 FROM axionpro."Module" existing
        WHERE existing."ModuleCode" = seed.code
          AND existing."ModuleScope" = 1
    );

    -- Import is a first-class operation type (enum value 12). Reuse it when
    -- already seeded; never create a duplicate operation.
    SELECT "Id" INTO import_operation_id
    FROM axionpro."Operation"
    WHERE LOWER(BTRIM("OperationName")) = 'import'
    ORDER BY "Id" LIMIT 1;

    IF import_operation_id IS NULL THEN
        INSERT INTO axionpro."Operation"
        ("OperationName","Remark","OperationType","IsActive","AddedById","AddedDateTime","IconImage")
        VALUES ('Import','Upload, preview and queue a validated bulk import.',12,TRUE,1,CURRENT_TIMESTAMP,'upload')
        RETURNING "Id" INTO import_operation_id;
    ELSE
        UPDATE axionpro."Operation"
        SET "OperationType" = 12,
            "Remark" = 'Upload, preview and queue a validated bulk import.',
            "IsActive" = TRUE,
            "IconImage" = COALESCE(NULLIF(BTRIM("IconImage"), ''), 'upload'),
            "UpdatedById" = 1,
            "UpdatedDateTime" = CURRENT_TIMESTAMP
        WHERE "Id" = import_operation_id;
    END IF;

    INSERT INTO axionpro."ModuleOperationMapping"
    ("ModuleId","OperationId","PageURL","IconURL","IsCommonItem","IsOperational",
     "Priority","Remark","IsActive","AddedById","AddedDateTime")
    SELECT module."Id", import_operation_id, module."URLPath", 'upload', FALSE, TRUE,
           15, 'Bulk import catalogue operation; role grants use the existing permission flow.',
           TRUE, 1, CURRENT_TIMESTAMP
    FROM axionpro."Module" module
    WHERE module."ModuleScope" = 1
      AND module."ModuleCode" IN
          ('EMP_LIST','TENANT_DEPARTMENTS','TENANT_DESIGNATIONS',
           'TENANT_ROLES_PERMISSIONS','TENANT_EMPLOYEE_TYPES')
      AND NOT EXISTS
      (
          SELECT 1 FROM axionpro."ModuleOperationMapping" existing
          WHERE existing."ModuleId" = module."Id"
            AND existing."OperationId" = import_operation_id
      );

    -- Make bulk modules available to the same subscription plans that already
    -- include EMP_LIST. TenantEnabledModule/role grants are deliberately not
    -- inserted here; the existing Host entitlement and permission commands own them.
    INSERT INTO axionpro."PlanModuleMapping"
    ("SubscriptionPlanId","ModuleId","IsActive","Remark","AddedById","AddedDateTime")
    SELECT DISTINCT plan."SubscriptionPlanId", target."Id", TRUE,
           'Bulk import module inherited from EMP_LIST plan coverage.', 1, CURRENT_TIMESTAMP
    FROM axionpro."PlanModuleMapping" plan
    JOIN axionpro."Module" employee_list
      ON employee_list."Id" = plan."ModuleId" AND employee_list."ModuleCode" = 'EMP_LIST'
    JOIN axionpro."Module" target
      ON target."ModuleScope" = 1
     AND target."ModuleCode" IN ('TENANT_DEPARTMENTS','TENANT_DESIGNATIONS',
                                  'TENANT_ROLES_PERMISSIONS','TENANT_EMPLOYEE_TYPES')
    WHERE plan."IsActive" = TRUE
      AND NOT EXISTS
      (
          SELECT 1 FROM axionpro."PlanModuleMapping" existing
          WHERE existing."SubscriptionPlanId" = plan."SubscriptionPlanId"
            AND existing."ModuleId" = target."Id"
      );
END $bulk_seed$;

COMMIT;

-- Verification: catalogue only. Role/tenant grants must be added through the
-- existing entitlement synchronization and role-permission API.
SELECT module."ModuleCode", module."Id", operation."OperationName",
       operation."OperationType", mapping."IsActive" AS "MappingIsActive"
FROM axionpro."Module" module
JOIN axionpro."ModuleOperationMapping" mapping ON mapping."ModuleId" = module."Id"
JOIN axionpro."Operation" operation ON operation."Id" = mapping."OperationId"
WHERE module."ModuleCode" IN ('EMP_LIST','TENANT_DEPARTMENTS','TENANT_DESIGNATIONS',
    'TENANT_ROLES_PERMISSIONS','TENANT_EMPLOYEE_TYPES','TENANT_EMPLOYEE_CODE')
ORDER BY module."ModuleCode", operation."OperationType";
