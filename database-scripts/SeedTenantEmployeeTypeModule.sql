-- Adds only EmployeeType module metadata and plan coverage. No role grants or Host resets.
BEGIN;
LOCK TABLE axionpro."Module", axionpro."ModuleOperationMapping", axionpro."PlanModuleMapping" IN SHARE ROW EXCLUSIVE MODE;
DO $seed$
DECLARE table_name text;
BEGIN
    FOREACH table_name IN ARRAY ARRAY['Module','ModuleOperationMapping','PlanModuleMapping'] LOOP
        EXECUTE format('SELECT setval(pg_get_serial_sequence(%L,''Id''), GREATEST(COALESCE((SELECT max("Id") FROM axionpro.%I),0),
            COALESCE(pg_sequence_last_value(pg_get_serial_sequence(%L,''Id'')::regclass),0),1),true)',
            'axionpro."' || table_name || '"',table_name,'axionpro."' || table_name || '"');
    END LOOP;
    IF (SELECT count(*) FROM axionpro."Module" WHERE "ModuleCode"='TENANT_DEPARTMENTS' AND "ModuleScope"=1) <> 1 THEN
        RAISE EXCEPTION 'Exactly one TENANT_DEPARTMENTS baseline module is required';
    END IF;
END $seed$;
INSERT INTO axionpro."Module" ("TenantId","ModuleCode","ModuleName","DisplayName","URLPath","ParentModuleId",
    "IsLeafNode","IsModuleDisplayInUI","IsCommonMenu","IsActive","ImageIconWeb","ImageIconMobile",
    "ItemPriority","Remark","AddedById","AddedDateTime","UpdatedById","UpdatedDateTime","ModuleScope","PageName")
SELECT NULL,'TENANT_EMPLOYEE_TYPES','Employee-Types','Employee Types','/employee-types',"ParentModuleId",
    TRUE,TRUE,FALSE,TRUE,"ImageIconWeb","ImageIconMobile","ItemPriority"+1,
    'Tenant-owned EmployeeType management and bulk import',1,CURRENT_TIMESTAMP,1,CURRENT_TIMESTAMP,1,'tenant-employee-types'
FROM axionpro."Module" source WHERE source."ModuleCode"='TENANT_DEPARTMENTS'
AND NOT EXISTS (SELECT 1 FROM axionpro."Module" WHERE "ModuleCode"='TENANT_EMPLOYEE_TYPES');

INSERT INTO axionpro."ModuleOperationMapping" ("ModuleId","OperationId","DataViewStructureId","PageTypeId",
    "PageURL","IconURL","IsCommonItem","IsOperational","Priority","Remark","IsActive","AddedById","AddedDateTime")
SELECT target."Id",op."Id",NULL,NULL,'/employee-types','',FALSE,TRUE,op."OperationType",
    'EmployeeType operation; authorization uses existing permission pipeline',TRUE,1,CURRENT_TIMESTAMP
FROM axionpro."Module" target CROSS JOIN axionpro."Operation" op
WHERE target."ModuleCode"='TENANT_EMPLOYEE_TYPES' AND op."IsActive"=TRUE AND op."OperationType" IN (1,4,12)
AND NOT EXISTS (SELECT 1 FROM axionpro."ModuleOperationMapping" existing
    WHERE existing."ModuleId"=target."Id" AND existing."OperationId"=op."Id");

INSERT INTO axionpro."PlanModuleMapping" ("SubscriptionPlanId","ModuleId","IsActive","Remark","AddedById","AddedDateTime")
SELECT DISTINCT source."SubscriptionPlanId",target."Id",TRUE,'EmployeeType baseline inherited from Department plan coverage',1,CURRENT_TIMESTAMP
FROM axionpro."PlanModuleMapping" source
JOIN axionpro."Module" baseline ON baseline."Id"=source."ModuleId" AND baseline."ModuleCode"='TENANT_DEPARTMENTS'
CROSS JOIN axionpro."Module" target
WHERE source."IsActive"=TRUE AND target."ModuleCode"='TENANT_EMPLOYEE_TYPES'
AND NOT EXISTS (SELECT 1 FROM axionpro."PlanModuleMapping" existing
    WHERE existing."SubscriptionPlanId"=source."SubscriptionPlanId" AND existing."ModuleId"=target."Id");
COMMIT;
-- Existing tenants: use the established plan-entitlement synchronization and role-grant UI/API.
