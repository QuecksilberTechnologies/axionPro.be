-- Tenant-owned EmployeeTypes. Apply after AddDurableMasterBulkImport.sql.
-- Legacy null-tenant rows remain as historical/bootstrap templates, never tenant API options.
BEGIN;
LOCK TABLE axionpro."EmployeeType", axionpro."Employee",
    axionpro."EmployeesChangedTypeHistory", axionpro."EmployeeTypeBasicMenu"
    IN SHARE ROW EXCLUSIVE MODE;

SELECT setval(
    pg_get_serial_sequence('axionpro."EmployeeType"','Id'),
    GREATEST(
        COALESCE((SELECT max("Id") FROM axionpro."EmployeeType"), 0),
        COALESCE(pg_sequence_last_value(pg_get_serial_sequence('axionpro."EmployeeType"','Id')::regclass), 0),
        1),
    true);
SELECT setval(
    pg_get_serial_sequence('axionpro."EmployeeTypeBasicMenu"','Id'),
    GREATEST(
        COALESCE((SELECT max("Id") FROM axionpro."EmployeeTypeBasicMenu"), 0),
        COALESCE(pg_sequence_last_value(pg_get_serial_sequence('axionpro."EmployeeTypeBasicMenu"','Id')::regclass), 0),
        1),
    true);

ALTER TABLE axionpro."EmployeeType"
    ADD COLUMN IF NOT EXISTS "TenantId" bigint REFERENCES axionpro."Tenant"("Id");
CREATE UNIQUE INDEX IF NOT EXISTS "UX_EmployeeType_Tenant_Name_Live"
    ON axionpro."EmployeeType" ("TenantId", lower(btrim("TypeName")))
    WHERE "TenantId" IS NOT NULL AND "IsSoftDeleted" IS NOT TRUE;

CREATE TEMP TABLE bulk_type_migration
(
    "TenantId" bigint,
    "OldId" integer,
    "NewId" integer,
    PRIMARY KEY ("TenantId", "OldId")
) ON COMMIT DROP;

INSERT INTO bulk_type_migration ("TenantId", "OldId")
SELECT DISTINCT usage."TenantId", usage."TypeId"
FROM
(
    SELECT "TenantId", "EmployeeTypeId" AS "TypeId"
    FROM axionpro."Employee"
    UNION
    SELECT employee."TenantId", history."OldEmployeeTypeId"
    FROM axionpro."EmployeesChangedTypeHistory" history
    JOIN axionpro."Employee" employee ON employee."Id" = history."EmployeeId"
    UNION
    SELECT employee."TenantId", history."NewEmployeeTypeId"
    FROM axionpro."EmployeesChangedTypeHistory" history
    JOIN axionpro."Employee" employee ON employee."Id" = history."EmployeeId"
) usage
JOIN axionpro."EmployeeType" source
    ON source."Id" = usage."TypeId" AND source."TenantId" IS NULL
WHERE usage."TenantId" IS NOT NULL;

DO $migration$
DECLARE
    mapping record;
    new_id integer;
BEGIN
    FOR mapping IN
        SELECT * FROM bulk_type_migration ORDER BY "TenantId", "OldId"
    LOOP
        INSERT INTO axionpro."EmployeeType"
        (
            "TenantId", "TypeName", "Description", "Remark", "IsActive",
            "AddedById", "AddedDateTime", "UpdatedById", "UpdatedDateTime",
            "IsSoftDeleted", "SoftDeletedById", "SoftDeletedDateTime"
        )
        SELECT
            mapping."TenantId", "TypeName", "Description", "Remark", "IsActive",
            "AddedById", "AddedDateTime", "UpdatedById", "UpdatedDateTime",
            "IsSoftDeleted", "SoftDeletedById", "SoftDeletedDateTime"
        FROM axionpro."EmployeeType"
        WHERE "Id" = mapping."OldId"
        RETURNING "Id" INTO new_id;

        UPDATE bulk_type_migration
        SET "NewId" = new_id
        WHERE "TenantId" = mapping."TenantId" AND "OldId" = mapping."OldId";

        INSERT INTO axionpro."EmployeeTypeBasicMenu"
        (
            "BasicMenuId", "EmployeeTypeId", "ForPlatform", "IsMenuDisplayInUI",
            "IsDisplayable", "IsActive", "HasAccess", "AddedById", "AddedDateTime",
            "UpdatedById", "UpdatedDateTime"
        )
        SELECT
            "BasicMenuId", new_id, "ForPlatform", "IsMenuDisplayInUI",
            "IsDisplayable", "IsActive", "HasAccess", "AddedById", "AddedDateTime",
            "UpdatedById", "UpdatedDateTime"
        FROM axionpro."EmployeeTypeBasicMenu"
        WHERE "EmployeeTypeId" = mapping."OldId";
    END LOOP;
END $migration$;

UPDATE axionpro."Employee" employee
SET "EmployeeTypeId" = mapping."NewId"
FROM bulk_type_migration mapping
WHERE employee."TenantId" = mapping."TenantId"
  AND employee."EmployeeTypeId" = mapping."OldId";

UPDATE axionpro."EmployeesChangedTypeHistory" history
SET "OldEmployeeTypeId" = mapping."NewId"
FROM bulk_type_migration mapping, axionpro."Employee" employee
WHERE employee."Id" = history."EmployeeId"
  AND employee."TenantId" = mapping."TenantId"
  AND history."OldEmployeeTypeId" = mapping."OldId";

UPDATE axionpro."EmployeesChangedTypeHistory" history
SET "NewEmployeeTypeId" = mapping."NewId"
FROM bulk_type_migration mapping, axionpro."Employee" employee
WHERE employee."Id" = history."EmployeeId"
  AND employee."TenantId" = mapping."TenantId"
  AND history."NewEmployeeTypeId" = mapping."OldId";

CREATE OR REPLACE FUNCTION axionpro.prevent_employee_type_owner_change()
RETURNS trigger LANGUAGE plpgsql AS $body$
BEGIN
    IF OLD."TenantId" IS DISTINCT FROM NEW."TenantId" THEN
        RAISE EXCEPTION 'EmployeeType ownership cannot be moved between tenants' USING ERRCODE = '23514';
    END IF;
    RETURN NEW;
END $body$;

DROP TRIGGER IF EXISTS "TR_EmployeeType_OwnerImmutable" ON axionpro."EmployeeType";
CREATE TRIGGER "TR_EmployeeType_OwnerImmutable"
BEFORE UPDATE OF "TenantId" ON axionpro."EmployeeType"
FOR EACH ROW EXECUTE FUNCTION axionpro.prevent_employee_type_owner_change();

CREATE OR REPLACE FUNCTION axionpro.validate_employee_type_tenant()
RETURNS trigger LANGUAGE plpgsql AS $body$
DECLARE
    tenant_id bigint;
    type_ids integer[];
    type_id integer;
    type_tenant bigint;
BEGIN
    IF TG_TABLE_NAME = 'EmployeesChangedTypeHistory' THEN
        SELECT "TenantId" INTO tenant_id
        FROM axionpro."Employee"
        WHERE "Id" = NEW."EmployeeId";
        type_ids := ARRAY[NEW."OldEmployeeTypeId", NEW."NewEmployeeTypeId"];
    ELSE
        tenant_id := NEW."TenantId";
        type_ids := ARRAY[NEW."EmployeeTypeId"];
    END IF;

    FOREACH type_id IN ARRAY type_ids
    LOOP
        IF type_id IS NOT NULL THEN
            SELECT "TenantId" INTO type_tenant
            FROM axionpro."EmployeeType"
            WHERE "Id" = type_id;
            IF NOT FOUND OR type_tenant IS DISTINCT FROM tenant_id THEN
                RAISE EXCEPTION 'EmployeeType must belong to the same tenant' USING ERRCODE = '23514';
            END IF;
        END IF;
    END LOOP;
    RETURN NEW;
END $body$;

DO $triggers$
DECLARE
    table_name text;
BEGIN
    FOREACH table_name IN ARRAY ARRAY['Employee', 'EmployeesChangedTypeHistory']
    LOOP
        EXECUTE format('DROP TRIGGER IF EXISTS "TR_EmployeeType_Tenant" ON axionpro.%I', table_name);
        EXECUTE format(
            'CREATE TRIGGER "TR_EmployeeType_Tenant" BEFORE INSERT OR UPDATE ON axionpro.%I '
            'FOR EACH ROW EXECUTE FUNCTION axionpro.validate_employee_type_tenant()',
            table_name);
    END LOOP;
END $triggers$;

ALTER TABLE axionpro."BulkImportJob" DROP CONSTRAINT IF EXISTS "BulkImportJob_Master_check";
ALTER TABLE axionpro."BulkImportJob"
    ADD CONSTRAINT "BulkImportJob_Master_check" CHECK ("Master" BETWEEN 1 AND 5);
COMMIT;
