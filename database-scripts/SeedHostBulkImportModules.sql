-- Host bulk menu catalogue. Run after baseline module seed; safe to re-run in pgAdmin.
-- Scope 2 only. Host modules do not belong in subscription PlanModuleMapping.
BEGIN;
LOCK TABLE axionpro."Module", axionpro."Operation", axionpro."ModuleOperationMapping"
    IN SHARE ROW EXCLUSIVE MODE;
DO $host_bulk$
DECLARE
    definition record;
    parent_id integer;
    child_id integer;
    operation_id integer;
    operation_type integer;
BEGIN
    FOR definition IN SELECT * FROM (VALUES
        ('HOST_CARD_BULK','Card-Bulk','Card Bulk','/app/tenant-card-inventory/bulk','host-card-bulk','HOST_TENANT_RFID_MANAGEMENT',535),
        ('HOST_DEVICE_BULK','Device-Bulk','Device Bulk','/app/device-masters/bulk','host-device-bulk','HOST_DEVICE_SETUP',515),
        ('HOST_MODULE_CATALOGUE_BULK','Module-Bulk','Module Bulk','/app/modules/bulk','host-module-bulk','HOST_MODULES',725),
        ('HOST_SUBMODULE_CATALOGUE_BULK','ChildModule-Bulk','Child Module Bulk','/app/modules/submodules/bulk','host-submodule-bulk','HOST_SUBMODULES',735),
        ('HOST_OPERATION_CATALOGUE_BULK','Operation-Bulk','Operation Bulk','/app/modules/operations/bulk','host-operation-bulk','HOST_OPERATIONS',745),
        ('HOST_MODULE_OPERATION_CATALOGUE_BULK','OperationMapping-Bulk','Operation Mapping Bulk','/app/modules/module-operations/bulk','host-module-operation-bulk','HOST_MODULE_OPERATIONS',755)
    ) AS x(code,name,display_name,url,page_name,parent_code,priority)
    LOOP
        SELECT "Id" INTO STRICT parent_id FROM axionpro."Module"
        WHERE "ModuleCode"=definition.parent_code AND "ModuleScope"=2 AND "IsActive"=TRUE;
        INSERT INTO axionpro."Module"
            ("TenantId","ModuleCode","ModuleName","DisplayName","URLPath","ParentModuleId",
             "IsLeafNode","IsModuleDisplayInUI","IsCommonMenu","IsActive","ImageIconWeb",
             "ImageIconMobile","ItemPriority","Remark","AddedById","AddedDateTime","ModuleScope","PageName")
        SELECT NULL,definition.code,definition.name,definition.display_name,definition.url,parent_id,
            TRUE,TRUE,FALSE,TRUE,'bi bi-upload','upload',definition.priority,
            'Host-only upload, preview, confirm and track durable bulk imports.',1,CURRENT_TIMESTAMP,2,definition.page_name
        WHERE NOT EXISTS (SELECT 1 FROM axionpro."Module" WHERE "ModuleCode"=definition.code);
        SELECT "Id" INTO STRICT child_id FROM axionpro."Module"
        WHERE "ModuleCode"=definition.code AND "ModuleScope"=2;
        UPDATE axionpro."Module" SET "ParentModuleId"=parent_id,"IsLeafNode"=TRUE,
            "IsModuleDisplayInUI"=TRUE,"IsActive"=TRUE
        WHERE "Id"=child_id;
        -- Catalogue remains navigable with a child; preserve existing names, URL and page identity.
        UPDATE axionpro."Module" SET "IsLeafNode"=FALSE WHERE "Id"=parent_id;
        FOREACH operation_type IN ARRAY ARRAY[4,11,12]
        LOOP
            SELECT "Id" INTO operation_id FROM axionpro."Operation"
            WHERE "OperationType"=operation_type ORDER BY "IsActive" DESC,"Id" LIMIT 1;
            IF operation_id IS NULL THEN
                INSERT INTO axionpro."Operation"
                    ("OperationName","OperationType","Remark","IsActive","IconImage","AddedById","AddedDateTime")
                VALUES (CASE operation_type WHEN 4 THEN 'View' WHEN 11 THEN 'Export' ELSE 'Import' END,operation_type,
                    'Read or execute bulk imports.',TRUE,'upload',1,CURRENT_TIMESTAMP)
                RETURNING "Id" INTO operation_id;
            END IF;
            UPDATE axionpro."Operation" SET "IsActive"=TRUE WHERE "Id"=operation_id;
            INSERT INTO axionpro."ModuleOperationMapping"
                ("ModuleId","OperationId","PageURL","IconURL","IsCommonItem","IsOperational","Priority",
                 "Remark","IsActive","AddedById","AddedDateTime")
            SELECT child_id,operation_id,definition.url,'upload',FALSE,TRUE,operation_type,
                'Host bulk permission; grants use existing Host role permission flow.',TRUE,1,CURRENT_TIMESTAMP
            WHERE NOT EXISTS (SELECT 1 FROM axionpro."ModuleOperationMapping"
                WHERE "ModuleId"=child_id AND "OperationId"=operation_id);
            UPDATE axionpro."ModuleOperationMapping" SET "IsActive"=TRUE,"IsOperational"=TRUE
            WHERE "ModuleId"=child_id AND "OperationId"=operation_id;
        END LOOP;
    END LOOP;
END $host_bulk$;
COMMIT;
-- Use the existing Host role permission screen to grant View/Import on these two modules.
SELECT m."Id",m."ModuleCode",m."ModuleScope",m."ParentModuleId",m."PageName",o."Id" AS "OperationId",o."OperationName"
FROM axionpro."Module" m
JOIN axionpro."ModuleOperationMapping" mm ON mm."ModuleId"=m."Id" AND mm."IsActive"
JOIN axionpro."Operation" o ON o."Id"=mm."OperationId" AND o."IsActive"
WHERE m."ModuleCode" IN ('HOST_CARD_BULK','HOST_DEVICE_BULK','HOST_MODULE_CATALOGUE_BULK',
    'HOST_SUBMODULE_CATALOGUE_BULK','HOST_OPERATION_CATALOGUE_BULK','HOST_MODULE_OPERATION_CATALOGUE_BULK')
ORDER BY m."ModuleCode",o."OperationType";
