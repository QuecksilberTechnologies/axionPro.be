-- Bulk import module/operation catalogue seed.
-- Idempotent: safe to run more than once. Does not grant a tenant role access.
-- Run after the canonical module/operation seed and before entitlement sync.
BEGIN;

LOCK TABLE axionpro."Module", axionpro."Operation",
    axionpro."ModuleOperationMapping", axionpro."PlanModuleMapping",
    axionpro."HostRoleModuleAndPermission", axionpro."RoleModuleAndPermission",
    axionpro."TenantEnabledOperation", axionpro."TenantEnabledModule"
    IN SHARE ROW EXCLUSIVE MODE;

-- Preserve existing IDs and their dependent rows when upgrading legacy leaf
-- ModuleCode values. A pre-existing second canonical row is treated as a data
-- conflict instead of being merged or duplicated silently.
DO $canonical_tenant_master_leaf_codes$
DECLARE
    identity record;
    legacy_id integer;
    canonical_id integer;
BEGIN
    FOR identity IN
        SELECT * FROM (VALUES
            ('TENANT_DEPARTMENTS','DEPARTMENT'),
            ('TENANT_DESIGNATIONS','DESIGNATION'),
            ('TENANT_ROLES_PERMISSIONS','ROLE'),
            ('TENANT_EMPLOYEE_TYPES','EMPLOYEE_TYPE')
        ) AS codes(legacy_code,canonical_code)
    LOOP
        SELECT "Id" INTO legacy_id FROM axionpro."Module"
        WHERE "ModuleCode"=identity.legacy_code ORDER BY "Id" LIMIT 1;
        SELECT "Id" INTO canonical_id FROM axionpro."Module"
        WHERE "ModuleCode"=identity.canonical_code ORDER BY "Id" LIMIT 1;

        IF legacy_id IS NOT NULL AND canonical_id IS NOT NULL
           AND legacy_id <> canonical_id THEN
            RAISE EXCEPTION
                'Conflicting module rows exist for legacy code % and canonical code %. Resolve before seeding.',
                identity.legacy_code, identity.canonical_code;
        END IF;

        IF legacy_id IS NOT NULL AND canonical_id IS NULL THEN
            UPDATE axionpro."Module"
            SET "ModuleCode"=identity.canonical_code,
                "UpdatedById"=1,
                "UpdatedDateTime"=CURRENT_TIMESTAMP
            WHERE "Id"=legacy_id;
        END IF;
    END LOOP;
END $canonical_tenant_master_leaf_codes$;

DO $bulk_seed$
DECLARE
    tenant_parent_id integer;
    employee_parent_id integer;
    department_parent_id integer;
    designation_parent_id integer;
    role_parent_id integer;
    import_operation_id integer;
    export_operation_id integer;
BEGIN
    SELECT "Id" INTO tenant_parent_id
    FROM axionpro."Module"
    WHERE "ModuleCode" = 'TENANT_MGMT' AND "ModuleScope" = 1
    ORDER BY "Id" LIMIT 1;

    IF tenant_parent_id IS NULL THEN
        RAISE EXCEPTION 'TENANT_MGMT baseline module is required before bulk module seeding.';
    END IF;

    SELECT "Id" INTO employee_parent_id
    FROM axionpro."Module"
    WHERE "ModuleCode" = 'EMP_MGMT' AND "ModuleScope" = 1
    ORDER BY "Id" LIMIT 1;

    IF employee_parent_id IS NULL THEN
        RAISE EXCEPTION 'EMP_MGMT baseline module is required before bulk module seeding.';
    END IF;

    -- Singular root navigation parents group the existing functional leaf
    -- modules. They are independent roots, not children of TENANT_MGMT.
    -- Non-leaf parents follow the existing DB rule: URLPath is NULL and no
    -- operation is mapped directly to the parent.
    INSERT INTO axionpro."Module"
    ("TenantId","ModuleCode","ModuleName","DisplayName","URLPath","ParentModuleId",
     "IsLeafNode","IsModuleDisplayInUI","IsCommonMenu","IsActive","ImageIconWeb",
     "ImageIconMobile","ItemPriority","Remark","AddedById","AddedDateTime",
     "ModuleScope","PageName")
    SELECT NULL, definition.code, definition.module_name, definition.display_name,
           NULL, NULL, FALSE, TRUE, FALSE, TRUE, definition.web_icon,
           definition.mobile_icon, definition.priority, definition.remark,
           1, CURRENT_TIMESTAMP, 1, definition.page_name
    FROM (VALUES
        ('TENANT_DEPARTMENT','Tenant-Department','Tenant Department',
         'bi bi-building','business',510,
         'Department navigation group for tenant department management.',
         'tenant-department'),
        ('TENANT_DESIGNATION','Tenant-Designation','Tenant Designation',
         'bi bi-person-badge','ribbon',520,
         'Designation navigation group for tenant designation management.',
         'tenant-designation'),
        ('TENANT_ROLE','Tenant-Role','Tenant Role',
         'bi bi-shield-lock','shield-account',530,
         'Role navigation group for tenant role and permission management.',
         'tenant-role')
    ) AS definition(code,module_name,display_name,web_icon,mobile_icon,priority,remark,page_name)
    WHERE NOT EXISTS
    (
        SELECT 1 FROM axionpro."Module" existing
        WHERE existing."ModuleCode"=definition.code
          AND existing."ModuleScope"=1
    );

    -- Re-running the authoritative seed repairs metadata without creating a
    -- second row. ModuleCode is the stable catalogue identity.
    UPDATE axionpro."Module" module
    SET "ModuleName"=definition.module_name,
        "DisplayName"=definition.display_name,
        "URLPath"=NULL,
        "ParentModuleId"=NULL,
        "IsLeafNode"=FALSE,
        "IsModuleDisplayInUI"=TRUE,
        "IsCommonMenu"=FALSE,
        "IsActive"=TRUE,
        "ImageIconWeb"=definition.web_icon,
        "ImageIconMobile"=definition.mobile_icon,
        "ItemPriority"=definition.priority,
        "Remark"=definition.remark,
        "PageName"=definition.page_name,
        "UpdatedById"=1,
        "UpdatedDateTime"=CURRENT_TIMESTAMP
    FROM (VALUES
        ('TENANT_DEPARTMENT','Tenant-Department','Tenant Department','bi bi-building','business',510,
         'Department navigation group for tenant department management.','tenant-department'),
        ('TENANT_DESIGNATION','Tenant-Designation','Tenant Designation','bi bi-person-badge','ribbon',520,
         'Designation navigation group for tenant designation management.','tenant-designation'),
        ('TENANT_ROLE','Tenant-Role','Tenant Role','bi bi-shield-lock','shield-account',530,
         'Role navigation group for tenant role and permission management.','tenant-role')
    ) AS definition(code,module_name,display_name,web_icon,mobile_icon,priority,remark,page_name)
    WHERE module."ModuleCode"=definition.code
      AND module."ModuleScope"=1;

    SELECT "Id" INTO department_parent_id FROM axionpro."Module"
    WHERE "ModuleCode"='TENANT_DEPARTMENT' AND "ModuleScope"=1 ORDER BY "Id" LIMIT 1;
    SELECT "Id" INTO designation_parent_id FROM axionpro."Module"
    WHERE "ModuleCode"='TENANT_DESIGNATION' AND "ModuleScope"=1 ORDER BY "Id" LIMIT 1;
    SELECT "Id" INTO role_parent_id FROM axionpro."Module"
    WHERE "ModuleCode"='TENANT_ROLE' AND "ModuleScope"=1 ORDER BY "Id" LIMIT 1;

    -- Restore functional modules that an older seed placed under BULKUPLOAD.
    UPDATE axionpro."Module"
    SET "ParentModuleId"=CASE "ModuleCode"
            WHEN 'EMP_LIST' THEN employee_parent_id
            WHEN 'DEPARTMENT' THEN department_parent_id
            WHEN 'DESIGNATION' THEN designation_parent_id
            WHEN 'ROLE' THEN role_parent_id
            WHEN 'EMPLOYEE_TYPE' THEN employee_parent_id
            ELSE tenant_parent_id END,
        "UpdatedById"=1,"UpdatedDateTime"=CURRENT_TIMESTAMP
    WHERE "ModuleScope"=1
      AND "ModuleCode" IN ('EMP_LIST','DEPARTMENT','DESIGNATION',
          'ROLE','EMPLOYEE_TYPE');

    UPDATE axionpro."TenantEnabledModule" enabled
    SET "ParentModuleId"=CASE module."ModuleCode"
            WHEN 'EMP_LIST' THEN employee_parent_id
            WHEN 'DEPARTMENT' THEN department_parent_id
            WHEN 'DESIGNATION' THEN designation_parent_id
            WHEN 'ROLE' THEN role_parent_id
            WHEN 'EMPLOYEE_TYPE' THEN employee_parent_id
            ELSE tenant_parent_id END,
        "UpdatedById"=1,"UpdatedDateTime"=CURRENT_TIMESTAMP
    FROM axionpro."Module" module
    WHERE enabled."ModuleId"=module."Id"
      AND module."ModuleScope"=1
      AND module."ModuleCode" IN ('EMP_LIST','DEPARTMENT','DESIGNATION',
          'ROLE','EMPLOYEE_TYPE');

    SELECT "Id" INTO export_operation_id
    FROM axionpro."Operation"
    WHERE "OperationType" = 11
    ORDER BY "IsActive" DESC, "Id" LIMIT 1;

    IF export_operation_id IS NULL THEN
        INSERT INTO axionpro."Operation"
        ("OperationName","Remark","OperationType","IsActive","AddedById","AddedDateTime","IconImage")
        VALUES ('Export','Export authorized module data for spreadsheet use.',11,TRUE,1,CURRENT_TIMESTAMP,'download')
        RETURNING "Id" INTO export_operation_id;
    END IF;

    -- The existing four master modules may already be present in a deployment.
    -- Insert only missing catalogue rows; stable ModuleCode is the identity.
    INSERT INTO axionpro."Module"
    ("TenantId","ModuleCode","ModuleName","DisplayName","URLPath","ParentModuleId",
     "IsLeafNode","IsModuleDisplayInUI","IsCommonMenu","IsActive","ItemPriority",
     "AddedById","AddedDateTime","ModuleScope","PageName")
    SELECT NULL, seed.code, seed.name, seed.display_name, seed.url,
           CASE WHEN seed.code='EMPLOYEE_TYPE' THEN employee_parent_id ELSE tenant_parent_id END,
           TRUE, TRUE, FALSE, TRUE, seed.priority, 1, CURRENT_TIMESTAMP, 1, seed.page_name
    FROM (VALUES
        ('DEPARTMENT','Departments','Departments','/departments',510,'tenant-departments'),
        ('DESIGNATION','Designations','Designations','/designations',520,'tenant-designations'),
        ('ROLE','Roles-Permissions','Roles & Permissions','/roles',530,'tenant-roles-permissions'),
        ('EMPLOYEE_TYPE','Employee-Types','Employee Types','/employee-types',540,'tenant-employee-types'),
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
          ('EMP_LIST','DEPARTMENT','DESIGNATION',
           'ROLE','EMPLOYEE_TYPE')
      AND NOT EXISTS
      (
          SELECT 1 FROM axionpro."ModuleOperationMapping" existing
          WHERE existing."ModuleId" = module."Id"
            AND existing."OperationId" = import_operation_id
      );

    UPDATE axionpro."ModuleOperationMapping" mapping
    SET "IsActive"=TRUE,"IsOperational"=TRUE,"UpdatedById"=1,
        "UpdatedDateTime"=CURRENT_TIMESTAMP
    FROM axionpro."Module" module
    WHERE mapping."ModuleId"=module."Id"
      AND mapping."OperationId"=import_operation_id
      AND module."ModuleScope"=1
      AND module."ModuleCode" IN
          ('EMP_LIST','DEPARTMENT','DESIGNATION',
           'ROLE','EMPLOYEE_TYPE');

    INSERT INTO axionpro."ModuleOperationMapping"
    ("ModuleId","OperationId","PageURL","IconURL","IsCommonItem","IsOperational",
     "Priority","Remark","IsActive","AddedById","AddedDateTime")
    SELECT module."Id", export_operation_id, module."URLPath", 'download', FALSE, TRUE,
           20, 'Bulk export action on the existing functional module.',
           TRUE, 1, CURRENT_TIMESTAMP
    FROM axionpro."Module" module
    WHERE module."ModuleScope" = 1
      AND module."ModuleCode" IN
          ('EMP_LIST','DEPARTMENT','DESIGNATION',
           'ROLE','EMPLOYEE_TYPE')
      AND NOT EXISTS
      (
          SELECT 1 FROM axionpro."ModuleOperationMapping" existing
          WHERE existing."ModuleId" = module."Id"
            AND existing."OperationId" = export_operation_id
      );

    UPDATE axionpro."ModuleOperationMapping" mapping
    SET "IsActive"=TRUE,"IsOperational"=TRUE,"UpdatedById"=1,
        "UpdatedDateTime"=CURRENT_TIMESTAMP
    FROM axionpro."Module" module
    WHERE mapping."ModuleId"=module."Id"
      AND mapping."OperationId"=export_operation_id
      AND module."ModuleScope"=1
      AND module."ModuleCode" IN
          ('EMP_LIST','DEPARTMENT','DESIGNATION',
           'ROLE','EMPLOYEE_TYPE');

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
     AND target."ModuleCode" IN ('DEPARTMENT','DESIGNATION',
                                  'ROLE','EMPLOYEE_TYPE')
    WHERE plan."IsActive" = TRUE
      AND NOT EXISTS
      (
          SELECT 1 FROM axionpro."PlanModuleMapping" existing
          WHERE existing."SubscriptionPlanId" = plan."SubscriptionPlanId"
            AND existing."ModuleId" = target."Id"
      );

    -- Each singular parent inherits the plans of its own functional child.
    -- This makes tenant creation and Host entitlement sync include the branch.
    INSERT INTO axionpro."PlanModuleMapping"
    ("SubscriptionPlanId","ModuleId","IsActive","Remark","AddedById","AddedDateTime")
    SELECT DISTINCT child_plan."SubscriptionPlanId", parent."Id", TRUE,
           'Navigation parent inherited from its functional child plan coverage.',
           1, CURRENT_TIMESTAMP
    FROM (VALUES
        ('DEPARTMENT','TENANT_DEPARTMENT'),
        ('DESIGNATION','TENANT_DESIGNATION'),
        ('ROLE','TENANT_ROLE')
    ) AS hierarchy(child_code,parent_code)
    JOIN axionpro."Module" child
      ON child."ModuleCode"=hierarchy.child_code AND child."ModuleScope"=1
    JOIN axionpro."Module" parent
      ON parent."ModuleCode"=hierarchy.parent_code AND parent."ModuleScope"=1
    JOIN axionpro."PlanModuleMapping" child_plan
      ON child_plan."ModuleId"=child."Id" AND child_plan."IsActive"=TRUE
    WHERE NOT EXISTS
    (
        SELECT 1 FROM axionpro."PlanModuleMapping" existing
        WHERE existing."SubscriptionPlanId"=child_plan."SubscriptionPlanId"
          AND existing."ModuleId"=parent."Id"
    );

    UPDATE axionpro."PlanModuleMapping" parent_plan
    SET "IsActive"=TRUE,
        "Remark"='Navigation parent inherited from its functional child plan coverage.',
        "UpdatedById"=1,
        "UpdatedDateTime"=CURRENT_TIMESTAMP
    FROM (VALUES
        ('DEPARTMENT','TENANT_DEPARTMENT'),
        ('DESIGNATION','TENANT_DESIGNATION'),
        ('ROLE','TENANT_ROLE')
    ) AS hierarchy(child_code,parent_code)
    JOIN axionpro."Module" parent
      ON parent."ModuleCode"=hierarchy.parent_code AND parent."ModuleScope"=1
    WHERE parent_plan."ModuleId"=parent."Id"
      AND EXISTS
      (
          SELECT 1
          FROM axionpro."Module" child
          JOIN axionpro."PlanModuleMapping" child_plan
            ON child_plan."ModuleId"=child."Id"
           AND child_plan."SubscriptionPlanId"=parent_plan."SubscriptionPlanId"
           AND child_plan."IsActive"=TRUE
          WHERE child."ModuleCode"=hierarchy.child_code
            AND child."ModuleScope"=1
      );

    -- Preserve existing tenant role/operation grants on the functional modules.
    INSERT INTO axionpro."RoleModuleAndPermission"
        ("RoleId","ModuleId","OperationId","HasAccess","IsActive","Remark",
         "IsOperational","ImageIcon","AddedById","AddedDateTime","UpdatedById",
         "UpdatedDateTime","IsSoftDeleted")
    SELECT grant_row."RoleId",target."Id",grant_row."OperationId",grant_row."HasAccess",
           grant_row."IsActive",grant_row."Remark",grant_row."IsOperational",
           grant_row."ImageIcon",grant_row."AddedById",grant_row."AddedDateTime",
           grant_row."UpdatedById",grant_row."UpdatedDateTime",grant_row."IsSoftDeleted"
    FROM (VALUES
        ('BULK_EMPLOYEES','EMP_LIST'),
        ('BULK_DEPARTMENTS','DEPARTMENT'),
        ('BULK_DESIGNATIONS','DESIGNATION'),
        ('BULK_ROLES','ROLE'),
        ('BULK_EMPLOYEE_TYPES','EMPLOYEE_TYPE')
    ) AS move(child_code,target_code)
    JOIN axionpro."Module" child ON child."ModuleCode"=move.child_code
    JOIN axionpro."Module" target
      ON target."ModuleCode"=move.target_code AND target."ModuleScope"=1
    JOIN axionpro."RoleModuleAndPermission" grant_row ON grant_row."ModuleId"=child."Id"
    WHERE NOT EXISTS (SELECT 1 FROM axionpro."RoleModuleAndPermission" existing
        WHERE existing."RoleId" IS NOT DISTINCT FROM grant_row."RoleId"
          AND existing."ModuleId"=target."Id"
          AND existing."OperationId" IS NOT DISTINCT FROM grant_row."OperationId"
          AND existing."IsSoftDeleted"=grant_row."IsSoftDeleted");

    INSERT INTO axionpro."TenantEnabledOperation"
        ("TenantId","ModuleId","OperationId","IsOperationUsed","IsEnabled",
         "AddedById","AddedDateTime","UpdatedById","UpdatedDateTime")
    SELECT enabled."TenantId",target."Id",enabled."OperationId",enabled."IsOperationUsed",
           enabled."IsEnabled",enabled."AddedById",enabled."AddedDateTime",
           enabled."UpdatedById",enabled."UpdatedDateTime"
    FROM (VALUES
        ('BULK_EMPLOYEES','EMP_LIST'),
        ('BULK_DEPARTMENTS','DEPARTMENT'),
        ('BULK_DESIGNATIONS','DESIGNATION'),
        ('BULK_ROLES','ROLE'),
        ('BULK_EMPLOYEE_TYPES','EMPLOYEE_TYPE')
    ) AS move(child_code,target_code)
    JOIN axionpro."Module" child ON child."ModuleCode"=move.child_code
    JOIN axionpro."Module" target
      ON target."ModuleCode"=move.target_code AND target."ModuleScope"=1
    JOIN axionpro."TenantEnabledOperation" enabled ON enabled."ModuleId"=child."Id"
    WHERE NOT EXISTS (SELECT 1 FROM axionpro."TenantEnabledOperation" existing
        WHERE existing."TenantId"=enabled."TenantId"
          AND existing."ModuleId"=target."Id"
          AND existing."OperationId"=enabled."OperationId");

    DELETE FROM axionpro."HostRoleModuleAndPermission" WHERE "ModuleId" IN
      (SELECT "Id" FROM axionpro."Module" WHERE "ModuleCode" LIKE 'BULK\_%' ESCAPE '\'
          OR "ModuleCode"='BULKUPLOAD');
    DELETE FROM axionpro."RoleModuleAndPermission" WHERE "ModuleId" IN
      (SELECT "Id" FROM axionpro."Module" WHERE "ModuleCode" LIKE 'BULK\_%' ESCAPE '\'
          OR "ModuleCode"='BULKUPLOAD');
    DELETE FROM axionpro."TenantEnabledOperation" WHERE "ModuleId" IN
      (SELECT "Id" FROM axionpro."Module" WHERE "ModuleCode" LIKE 'BULK\_%' ESCAPE '\'
          OR "ModuleCode"='BULKUPLOAD');
    DELETE FROM axionpro."TenantEnabledModule" WHERE "ModuleId" IN
      (SELECT "Id" FROM axionpro."Module" WHERE "ModuleCode" LIKE 'BULK\_%' ESCAPE '\'
          OR "ModuleCode"='BULKUPLOAD')
       OR "ParentModuleId" IN
      (SELECT "Id" FROM axionpro."Module" WHERE "ModuleCode" LIKE 'BULK\_%' ESCAPE '\'
          OR "ModuleCode"='BULKUPLOAD');
    DELETE FROM axionpro."PlanModuleMapping" WHERE "ModuleId" IN
      (SELECT "Id" FROM axionpro."Module" WHERE "ModuleCode" LIKE 'BULK\_%' ESCAPE '\'
          OR "ModuleCode"='BULKUPLOAD');
    DELETE FROM axionpro."ModuleOperationMapping" WHERE "ModuleId" IN
      (SELECT "Id" FROM axionpro."Module" WHERE "ModuleCode" LIKE 'BULK\_%' ESCAPE '\'
          OR "ModuleCode"='BULKUPLOAD');
    DELETE FROM axionpro."Module"
    WHERE "ModuleCode" LIKE 'BULK\_%' ESCAPE '\' OR "ModuleCode"='BULKUPLOAD';
END $bulk_seed$;

COMMIT;

-- Verification: catalogue only. Role/tenant grants must be added through the
-- existing entitlement synchronization and role-permission API.
SELECT module."ModuleCode", module."Id", operation."OperationName",
       operation."OperationType", mapping."IsActive" AS "MappingIsActive"
FROM axionpro."Module" module
JOIN axionpro."ModuleOperationMapping" mapping ON mapping."ModuleId" = module."Id"
JOIN axionpro."Operation" operation ON operation."Id" = mapping."OperationId"
WHERE module."ModuleCode" IN ('EMP_LIST','DEPARTMENT','DESIGNATION',
    'ROLE','EMPLOYEE_TYPE','TENANT_EMPLOYEE_CODE')
ORDER BY module."ModuleCode", operation."OperationType";
