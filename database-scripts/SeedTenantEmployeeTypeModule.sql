-- Adds EmployeeType CRUD and bulk operation metadata plus plan coverage. No direct role grants or Host resets.
BEGIN;
LOCK TABLE axionpro."Module", axionpro."ModuleOperationMapping", axionpro."PlanModuleMapping",
    axionpro."TenantEnabledOperation", axionpro."RoleModuleAndPermission" IN SHARE ROW EXCLUSIVE MODE;

DO $employee_type_legacy_code$
DECLARE legacy_id integer; canonical_id integer;
BEGIN
    SELECT "Id" INTO legacy_id FROM axionpro."Module"
    WHERE "ModuleCode"='TENANT_EMPLOYEE_TYPES' ORDER BY "Id" LIMIT 1;
    SELECT "Id" INTO canonical_id FROM axionpro."Module"
    WHERE "ModuleCode"='EMPLOYEE_TYPE' ORDER BY "Id" LIMIT 1;

    IF legacy_id IS NOT NULL AND canonical_id IS NOT NULL AND legacy_id <> canonical_id THEN
        RAISE EXCEPTION 'Conflicting TENANT_EMPLOYEE_TYPES and EMPLOYEE_TYPE rows exist.';
    END IF;

    IF legacy_id IS NOT NULL AND canonical_id IS NULL THEN
        UPDATE axionpro."Module"
        SET "ModuleCode"='EMPLOYEE_TYPE',"UpdatedById"=1,"UpdatedDateTime"=CURRENT_TIMESTAMP
        WHERE "Id"=legacy_id;
    END IF;
END $employee_type_legacy_code$;
DO $seed$
DECLARE table_name text;
BEGIN
    FOREACH table_name IN ARRAY ARRAY['Module','ModuleOperationMapping','PlanModuleMapping'] LOOP
        EXECUTE format('SELECT setval(pg_get_serial_sequence(%L,''Id''), GREATEST(COALESCE((SELECT max("Id") FROM axionpro.%I),0),
            COALESCE(pg_sequence_last_value(pg_get_serial_sequence(%L,''Id'')::regclass),0),1),true)',
            'axionpro."' || table_name || '"',table_name,'axionpro."' || table_name || '"');
    END LOOP;
    IF (SELECT count(*) FROM axionpro."Module" WHERE "ModuleCode"='EMP_MGMT' AND "ModuleScope"=1) <> 1 THEN
        RAISE EXCEPTION 'Exactly one EMP_MGMT parent module is required';
    END IF;
END $seed$;
INSERT INTO axionpro."Module" ("TenantId","ModuleCode","ModuleName","DisplayName","URLPath","ParentModuleId",
    "IsLeafNode","IsModuleDisplayInUI","IsCommonMenu","IsActive","ImageIconWeb","ImageIconMobile",
    "ItemPriority","Remark","AddedById","AddedDateTime","UpdatedById","UpdatedDateTime","ModuleScope","PageName")
SELECT NULL,'EMPLOYEE_TYPE','Employee-Types','Employee Types','/employee-types',"Id",
    TRUE,TRUE,FALSE,TRUE,"ImageIconWeb","ImageIconMobile","ItemPriority"+1,
    'Tenant-owned EmployeeType management',1,CURRENT_TIMESTAMP,1,CURRENT_TIMESTAMP,1,'tenant-employee-types'
FROM axionpro."Module" source WHERE source."ModuleCode"='EMP_MGMT'
AND NOT EXISTS (SELECT 1 FROM axionpro."Module" WHERE "ModuleCode"='EMPLOYEE_TYPE');

INSERT INTO axionpro."ModuleOperationMapping" ("ModuleId","OperationId","DataViewStructureId","PageTypeId",
    "PageURL","IconURL","IsCommonItem","IsOperational","Priority","Remark","IsActive","AddedById","AddedDateTime")
SELECT target."Id",op."Id",NULL,NULL,'/employee-types','',FALSE,TRUE,op."OperationType",
    'EmployeeType operation; authorization uses existing permission pipeline',TRUE,1,CURRENT_TIMESTAMP
FROM axionpro."Module" target CROSS JOIN axionpro."Operation" op
WHERE target."ModuleCode"='EMPLOYEE_TYPE'
  AND op."IsActive"=TRUE
  AND (lower(btrim(op."OperationName")) IN ('add','update','delete','view')
    OR (lower(btrim(op."OperationName"))='import' AND op."OperationType"=12)
    OR (lower(btrim(op."OperationName"))='export' AND op."OperationType"=11))
AND NOT EXISTS (SELECT 1 FROM axionpro."ModuleOperationMapping" existing
    WHERE existing."ModuleId"=target."Id" AND existing."OperationId"=op."Id");

INSERT INTO axionpro."PlanModuleMapping" ("SubscriptionPlanId","ModuleId","IsActive","Remark","AddedById","AddedDateTime")
SELECT DISTINCT source."SubscriptionPlanId",target."Id",TRUE,'EmployeeType inherited from EMP_LIST plan coverage',1,CURRENT_TIMESTAMP
FROM axionpro."PlanModuleMapping" source
JOIN axionpro."Module" baseline ON baseline."Id"=source."ModuleId" AND baseline."ModuleCode"='EMP_LIST'
CROSS JOIN axionpro."Module" target
WHERE source."IsActive"=TRUE AND target."ModuleCode"='EMPLOYEE_TYPE'
AND NOT EXISTS (SELECT 1 FROM axionpro."PlanModuleMapping" existing
    WHERE existing."SubscriptionPlanId"=source."SubscriptionPlanId" AND existing."ModuleId"=target."Id");

-- Existing enabled tenants receive only the active CRUD module-operation catalogue.
INSERT INTO axionpro."TenantEnabledOperation" ("TenantId","ModuleId","OperationId","IsOperationUsed","IsEnabled","AddedById","AddedDateTime")
SELECT enabled_module."TenantId", mapping."ModuleId", mapping."OperationId",
    COALESCE(mapping."IsOperational", TRUE), TRUE, 1, CURRENT_TIMESTAMP
FROM axionpro."TenantEnabledModule" AS enabled_module
INNER JOIN axionpro."Module" AS target
    ON target."Id" = enabled_module."ModuleId"
INNER JOIN axionpro."ModuleOperationMapping" AS mapping
    ON mapping."ModuleId" = target."Id"
WHERE target."ModuleCode" = 'EMPLOYEE_TYPE'
  AND enabled_module."IsEnabled" = TRUE
  AND mapping."IsActive" = TRUE
  AND EXISTS (
      SELECT 1
      FROM axionpro."Operation" AS operation
      WHERE operation."Id" = mapping."OperationId"
        AND operation."IsActive" = TRUE
        AND (lower(btrim(operation."OperationName")) IN ('add','update','delete','view')
          OR (lower(btrim(operation."OperationName"))='import' AND operation."OperationType"=12)
          OR (lower(btrim(operation."OperationName"))='export' AND operation."OperationType"=11)))
  AND NOT EXISTS (
      SELECT 1
      FROM axionpro."TenantEnabledOperation" AS existing
      WHERE existing."TenantId" = enabled_module."TenantId"
        AND existing."ModuleId" = mapping."ModuleId"
        AND existing."OperationId" = mapping."OperationId");
COMMIT;
-- Existing tenants: use the established plan-entitlement synchronization and role-grant UI/API.
