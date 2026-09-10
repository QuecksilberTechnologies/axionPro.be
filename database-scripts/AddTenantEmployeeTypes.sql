-- Tenant-owned EmployeeTypes. Apply after AddDurableMasterBulkImport.sql.
-- Legacy null-tenant rows remain as historical/bootstrap templates, never tenant API options.
BEGIN;
LOCK TABLE axionpro."EmployeeType", axionpro."Employee", axionpro."EmployeesChangedTypeHistory",
    axionpro."EmployeeTypeBasicMenu", axionpro."UnStructuredPolicyTypeMappingWithEmployeeType",
    axionpro."PolicyLeaveTypeMapping", axionpro."AccoumndationAllowancePolicyByDesignation",
    axionpro."MealAllowancePolicyByDesignation", axionpro."TravelAllowancePolicyByDesignation"
    IN SHARE ROW EXCLUSIVE MODE;
-- Restored backups may have explicit IDs ahead of identity sequences.
SELECT setval(pg_get_serial_sequence('axionpro."EmployeeType"','Id'),
    GREATEST(COALESCE((SELECT max("Id") FROM axionpro."EmployeeType"),0),
        COALESCE(pg_sequence_last_value(pg_get_serial_sequence('axionpro."EmployeeType"','Id')::regclass),0),1),true);
SELECT setval(pg_get_serial_sequence('axionpro."EmployeeTypeBasicMenu"','Id'),
    GREATEST(COALESCE((SELECT max("Id") FROM axionpro."EmployeeTypeBasicMenu"),0),
        COALESCE(pg_sequence_last_value(pg_get_serial_sequence('axionpro."EmployeeTypeBasicMenu"','Id')::regclass),0),1),true);
ALTER TABLE axionpro."EmployeeType" ADD COLUMN IF NOT EXISTS "TenantId" bigint
    REFERENCES axionpro."Tenant"("Id");
CREATE UNIQUE INDEX IF NOT EXISTS "UX_EmployeeType_Tenant_Name_Live"
    ON axionpro."EmployeeType" ("TenantId", lower(btrim("TypeName")))
    WHERE "TenantId" IS NOT NULL AND "IsSoftDeleted" IS NOT TRUE;

CREATE TEMP TABLE bulk_type_migration("TenantId" bigint, "OldId" integer, "NewId" integer,
    PRIMARY KEY("TenantId", "OldId")) ON COMMIT DROP;
INSERT INTO bulk_type_migration("TenantId", "OldId")
SELECT DISTINCT usage."TenantId", usage."TypeId" FROM (
    SELECT "TenantId", "EmployeeTypeId" AS "TypeId" FROM axionpro."Employee"
    UNION SELECT e."TenantId", h."OldEmployeeTypeId" FROM axionpro."EmployeesChangedTypeHistory" h
        JOIN axionpro."Employee" e ON e."Id"=h."EmployeeId"
    UNION SELECT e."TenantId", h."NewEmployeeTypeId" FROM axionpro."EmployeesChangedTypeHistory" h
        JOIN axionpro."Employee" e ON e."Id"=h."EmployeeId"
    UNION SELECT "TenantId", "EmployeeTypeId" FROM axionpro."UnStructuredPolicyTypeMappingWithEmployeeType"
    UNION SELECT "TenantId", "EmployeeTypeId" FROM axionpro."PolicyLeaveTypeMapping"
    UNION SELECT d."TenantId", a."EmployeeTypeId" FROM axionpro."AccoumndationAllowancePolicyByDesignation" a
        JOIN axionpro."Designation" d ON d."Id"=a."DesignationId"
    UNION SELECT d."TenantId", a."EmployeeTypeId" FROM axionpro."MealAllowancePolicyByDesignation" a
        JOIN axionpro."Designation" d ON d."Id"=a."DesignationId"
    UNION SELECT d."TenantId", a."EmployeeTypeId" FROM axionpro."TravelAllowancePolicyByDesignation" a
        JOIN axionpro."Designation" d ON d."Id"=a."DesignationId"
) usage JOIN axionpro."EmployeeType" source ON source."Id"=usage."TypeId" AND source."TenantId" IS NULL
WHERE usage."TenantId" IS NOT NULL;

DO $migration$
DECLARE mapping record; new_id integer;
BEGIN
    FOR mapping IN SELECT * FROM bulk_type_migration ORDER BY "TenantId","OldId" LOOP
        INSERT INTO axionpro."EmployeeType" ("TenantId","TypeName","Description","Remark","IsActive",
            "AddedById","AddedDateTime","UpdatedById","UpdatedDateTime","IsSoftDeleted","SoftDeletedById","SoftDeletedDateTime")
        SELECT mapping."TenantId","TypeName","Description","Remark","IsActive",
            "AddedById","AddedDateTime","UpdatedById","UpdatedDateTime","IsSoftDeleted","SoftDeletedById","SoftDeletedDateTime"
        FROM axionpro."EmployeeType" WHERE "Id"=mapping."OldId" RETURNING "Id" INTO new_id;
        UPDATE bulk_type_migration SET "NewId"=new_id WHERE "TenantId"=mapping."TenantId" AND "OldId"=mapping."OldId";
        INSERT INTO axionpro."EmployeeTypeBasicMenu" ("BasicMenuId","EmployeeTypeId","ForPlatform",
            "IsMenuDisplayInUI","IsDisplayable","IsActive","HasAccess","AddedById","AddedDateTime","UpdatedById","UpdatedDateTime")
        SELECT "BasicMenuId",new_id,"ForPlatform","IsMenuDisplayInUI","IsDisplayable","IsActive","HasAccess",
            "AddedById","AddedDateTime","UpdatedById","UpdatedDateTime"
        FROM axionpro."EmployeeTypeBasicMenu" WHERE "EmployeeTypeId"=mapping."OldId";
    END LOOP;
END $migration$;

UPDATE axionpro."Employee" e SET "EmployeeTypeId"=m."NewId" FROM bulk_type_migration m
WHERE e."TenantId"=m."TenantId" AND e."EmployeeTypeId"=m."OldId";
UPDATE axionpro."EmployeesChangedTypeHistory" h SET "OldEmployeeTypeId"=m."NewId"
FROM bulk_type_migration m, axionpro."Employee" e
WHERE e."Id"=h."EmployeeId" AND e."TenantId"=m."TenantId" AND h."OldEmployeeTypeId"=m."OldId";
UPDATE axionpro."EmployeesChangedTypeHistory" h SET "NewEmployeeTypeId"=m."NewId"
FROM bulk_type_migration m, axionpro."Employee" e
WHERE e."Id"=h."EmployeeId" AND e."TenantId"=m."TenantId" AND h."NewEmployeeTypeId"=m."OldId";
UPDATE axionpro."UnStructuredPolicyTypeMappingWithEmployeeType" p SET "EmployeeTypeId"=m."NewId"
FROM bulk_type_migration m WHERE p."TenantId"=m."TenantId" AND p."EmployeeTypeId"=m."OldId";
UPDATE axionpro."PolicyLeaveTypeMapping" p SET "EmployeeTypeId"=m."NewId"
FROM bulk_type_migration m WHERE p."TenantId"=m."TenantId" AND p."EmployeeTypeId"=m."OldId";
DO $allowances$
DECLARE table_name text;
BEGIN
    FOREACH table_name IN ARRAY ARRAY['AccoumndationAllowancePolicyByDesignation','MealAllowancePolicyByDesignation','TravelAllowancePolicyByDesignation'] LOOP
        EXECUTE format('UPDATE axionpro.%I a SET "EmployeeTypeId"=m."NewId" FROM bulk_type_migration m, axionpro."Designation" d
            WHERE d."Id"=a."DesignationId" AND d."TenantId"=m."TenantId" AND a."EmployeeTypeId"=m."OldId"', table_name);
    END LOOP;
END $allowances$;

-- Validate even references without an existing FK; never commit a partial remap.
DO $verify$
BEGIN
    IF EXISTS (
        SELECT 1 FROM (
            SELECT "TenantId", "EmployeeTypeId" AS "TypeId" FROM axionpro."Employee"
            UNION ALL SELECT e."TenantId", h."OldEmployeeTypeId" FROM axionpro."EmployeesChangedTypeHistory" h
                JOIN axionpro."Employee" e ON e."Id"=h."EmployeeId"
            UNION ALL SELECT e."TenantId", h."NewEmployeeTypeId" FROM axionpro."EmployeesChangedTypeHistory" h
                JOIN axionpro."Employee" e ON e."Id"=h."EmployeeId"
            UNION ALL SELECT "TenantId", "EmployeeTypeId" FROM axionpro."PolicyLeaveTypeMapping"
            UNION ALL SELECT "TenantId", "EmployeeTypeId" FROM axionpro."UnStructuredPolicyTypeMappingWithEmployeeType"
            UNION ALL SELECT d."TenantId", p."EmployeeTypeId" FROM axionpro."AccoumndationAllowancePolicyByDesignation" p
                JOIN axionpro."Designation" d ON d."Id"=p."DesignationId"
            UNION ALL SELECT d."TenantId", p."EmployeeTypeId" FROM axionpro."MealAllowancePolicyByDesignation" p
                JOIN axionpro."Designation" d ON d."Id"=p."DesignationId"
            UNION ALL SELECT d."TenantId", p."EmployeeTypeId" FROM axionpro."TravelAllowancePolicyByDesignation" p
                JOIN axionpro."Designation" d ON d."Id"=p."DesignationId"
        ) usage LEFT JOIN axionpro."EmployeeType" type ON type."Id"=usage."TypeId"
        WHERE usage."TypeId" IS NOT NULL AND (type."Id" IS NULL OR type."TenantId" IS DISTINCT FROM usage."TenantId")
    ) THEN
        RAISE EXCEPTION 'EmployeeType migration found unresolved or cross-tenant references; review source data';
    END IF;
END $verify$;

CREATE OR REPLACE FUNCTION axionpro.prevent_employee_type_owner_change() RETURNS trigger
LANGUAGE plpgsql AS $body$
BEGIN
    IF OLD."TenantId" IS DISTINCT FROM NEW."TenantId" THEN
        RAISE EXCEPTION 'EmployeeType ownership cannot be moved between tenants' USING ERRCODE='23514';
    END IF;
    RETURN NEW;
END $body$;
DROP TRIGGER IF EXISTS "TR_EmployeeType_OwnerImmutable" ON axionpro."EmployeeType";
CREATE TRIGGER "TR_EmployeeType_OwnerImmutable" BEFORE UPDATE OF "TenantId" ON axionpro."EmployeeType"
    FOR EACH ROW EXECUTE FUNCTION axionpro.prevent_employee_type_owner_change();

-- Protect existing employee/policy writers as well as imports from cross-tenant type assignments.
CREATE OR REPLACE FUNCTION axionpro.validate_employee_type_tenant() RETURNS trigger
LANGUAGE plpgsql AS $body$
DECLARE tenant_id bigint; type_ids integer[]; type_id integer; type_tenant bigint;
BEGIN
    IF TG_TABLE_NAME='EmployeesChangedTypeHistory' THEN
        SELECT "TenantId" INTO tenant_id FROM axionpro."Employee" WHERE "Id"=NEW."EmployeeId";
        type_ids := ARRAY[NEW."OldEmployeeTypeId",NEW."NewEmployeeTypeId"];
    ELSIF TG_TABLE_NAME IN ('AccoumndationAllowancePolicyByDesignation','MealAllowancePolicyByDesignation','TravelAllowancePolicyByDesignation') THEN
        SELECT "TenantId" INTO tenant_id FROM axionpro."Designation" WHERE "Id"=NEW."DesignationId";
        type_ids := ARRAY[NEW."EmployeeTypeId"];
    ELSE
        tenant_id := NEW."TenantId";
        type_ids := ARRAY[NEW."EmployeeTypeId"];
    END IF;
    FOREACH type_id IN ARRAY type_ids LOOP
        IF type_id IS NOT NULL THEN
            SELECT "TenantId" INTO type_tenant FROM axionpro."EmployeeType" WHERE "Id"=type_id;
            IF NOT FOUND OR type_tenant IS DISTINCT FROM tenant_id THEN
                RAISE EXCEPTION 'EmployeeType must belong to the same tenant' USING ERRCODE='23514';
            END IF;
        END IF;
    END LOOP;
    RETURN NEW;
END $body$;
DO $triggers$
DECLARE table_name text;
BEGIN
    FOREACH table_name IN ARRAY ARRAY['Employee','EmployeesChangedTypeHistory','UnStructuredPolicyTypeMappingWithEmployeeType',
        'PolicyLeaveTypeMapping','AccoumndationAllowancePolicyByDesignation','MealAllowancePolicyByDesignation','TravelAllowancePolicyByDesignation'] LOOP
        EXECUTE format('DROP TRIGGER IF EXISTS "TR_EmployeeType_Tenant" ON axionpro.%I',table_name);
        EXECUTE format('CREATE TRIGGER "TR_EmployeeType_Tenant" BEFORE INSERT OR UPDATE ON axionpro.%I
            FOR EACH ROW EXECUTE FUNCTION axionpro.validate_employee_type_tenant()',table_name);
    END LOOP;
END $triggers$;

ALTER TABLE axionpro."BulkImportJob" DROP CONSTRAINT IF EXISTS "BulkImportJob_Master_check";
ALTER TABLE axionpro."BulkImportJob" ADD CONSTRAINT "BulkImportJob_Master_check" CHECK ("Master" BETWEEN 1 AND 4);
COMMIT;
