-- Tenant card inventory and server-owned employee-device credentials.
-- Apply before deploying the matching API version. The script preserves all
-- legacy enrollment data and deliberately does not copy card numbers forward.

ALTER TABLE "EmployeeDeviceEnrollment" ADD COLUMN IF NOT EXISTS "TenantLocationId" bigint;
UPDATE "EmployeeDeviceEnrollment" e
SET "TenantLocationId" = d."TenantLocationId"
FROM "TenantDevice" d
WHERE e."TenantDeviceId" = d."Id" AND e."TenantLocationId" IS NULL;
DO $$ BEGIN
    IF EXISTS (SELECT 1 FROM "EmployeeDeviceEnrollment" WHERE "TenantLocationId" IS NULL) THEN
        RAISE EXCEPTION 'EmployeeDeviceEnrollment contains a device with no TenantLocationId; correct it before migration.';
    END IF;
END $$;
ALTER TABLE "EmployeeDeviceEnrollment" ALTER COLUMN "TenantLocationId" SET NOT NULL;
ALTER TABLE "EmployeeDeviceEnrollment" ADD COLUMN IF NOT EXISTS "TenantCardMasterId" bigint NULL;
ALTER TABLE "EmployeeDeviceEnrollment" ADD COLUMN IF NOT EXISTS "FaceImageHash" varchar(128) NULL;
ALTER TABLE "EmployeeDeviceEnrollment" ADD COLUMN IF NOT EXISTS "FaceDeploymentStatus" smallint NOT NULL DEFAULT 0;
ALTER TABLE "EmployeeDeviceEnrollment" ADD COLUMN IF NOT EXISTS "CardDeploymentStatus" smallint NOT NULL DEFAULT 0;
ALTER TABLE "EmployeeDeviceEnrollment" ADD COLUMN IF NOT EXISTS "PinDeploymentStatus" smallint NOT NULL DEFAULT 0;
ALTER TABLE "EmployeeDeviceEnrollment" ADD COLUMN IF NOT EXISTS "FaceDeviceCommandId" bigint NULL;
ALTER TABLE "EmployeeDeviceEnrollment" ADD COLUMN IF NOT EXISTS "CardDeviceCommandId" bigint NULL;
ALTER TABLE "EmployeeDeviceEnrollment" ADD COLUMN IF NOT EXISTS "PinDeviceCommandId" bigint NULL;
ALTER TABLE "EmployeeDeviceEnrollment" ADD COLUMN IF NOT EXISTS "UserActivationDeviceCommandId" bigint NULL;
ALTER TABLE "EmployeeDeviceEnrollment" ADD COLUMN IF NOT EXISTS "AccessEffectiveFromDateTime" timestamp NULL;
ALTER TABLE "EmployeeDeviceEnrollment" ADD COLUMN IF NOT EXISTS "AccessEffectiveToDateTime" timestamp NULL;

CREATE TABLE IF NOT EXISTS "TenantCardMaster" (
  "Id" bigserial PRIMARY KEY, "TenantId" bigint NOT NULL,
  "CardNumberEncrypted" varchar(1024) NOT NULL, "CardNumberLookupHash" varchar(128) NOT NULL,
  "CardReference" varchar(100), "CardStatus" smallint NOT NULL DEFAULT 1,
  "PurchaseCurrencyCode" varchar(3) NOT NULL DEFAULT 'INR', "UnitPurchasePriceExcludingTax" numeric(18,2) NOT NULL,
  "SupplierCountryId" integer, "SupplierStateId" integer, "PlaceOfSupplyCountryId" integer, "PlaceOfSupplyStateId" integer,
  "SupplierName" varchar(200), "SupplierTaxRegistrationNumber" varchar(100), "PurchaseInvoiceNumber" varchar(100), "PurchaseInvoiceDate" date,
  "TaxTreatment" smallint NOT NULL, "CgstRate" numeric(9,4) NOT NULL DEFAULT 0, "CgstAmount" numeric(18,2) NOT NULL DEFAULT 0,
  "SgstRate" numeric(9,4) NOT NULL DEFAULT 0, "SgstAmount" numeric(18,2) NOT NULL DEFAULT 0,
  "IgstRate" numeric(9,4) NOT NULL DEFAULT 0, "IgstAmount" numeric(18,2) NOT NULL DEFAULT 0,
  "ForeignTaxLabel" varchar(100), "ForeignTaxRate" numeric(9,4) NOT NULL DEFAULT 0, "ForeignTaxAmount" numeric(18,2) NOT NULL DEFAULT 0,
  "CustomsDutyAmount" numeric(18,2) NOT NULL DEFAULT 0, "FreightAmount" numeric(18,2) NOT NULL DEFAULT 0, "LandedCost" numeric(18,2) NOT NULL,
  "IsActive" boolean NOT NULL DEFAULT true, "IsSoftDeleted" boolean NOT NULL DEFAULT false,
  "AddedById" bigint NOT NULL, "AddedDateTime" timestamp NOT NULL, "UpdatedById" bigint, "UpdatedDateTime" timestamp, "SoftDeletedById" bigint, "SoftDeletedDateTime" timestamp,
  CONSTRAINT "FK_TenantCardMaster_Tenant" FOREIGN KEY ("TenantId") REFERENCES "Tenant"("Id") ON DELETE RESTRICT
);
CREATE UNIQUE INDEX IF NOT EXISTS "UX_TenantCardMaster_Tenant_CardHash_Live" ON "TenantCardMaster" ("TenantId", "CardNumberLookupHash") WHERE NOT "IsSoftDeleted";
CREATE INDEX IF NOT EXISTS "IX_EmployeeDeviceEnrollment_TenantLocationId" ON "EmployeeDeviceEnrollment" ("TenantLocationId");
CREATE INDEX IF NOT EXISTS "IX_EmployeeDeviceEnrollment_TenantCardMasterId" ON "EmployeeDeviceEnrollment" ("TenantCardMasterId");
CREATE INDEX IF NOT EXISTS "IX_EmployeeDeviceEnrollment_FaceDeviceCommandId" ON "EmployeeDeviceEnrollment" ("FaceDeviceCommandId");
CREATE INDEX IF NOT EXISTS "IX_EmployeeDeviceEnrollment_CardDeviceCommandId" ON "EmployeeDeviceEnrollment" ("CardDeviceCommandId");
CREATE INDEX IF NOT EXISTS "IX_EmployeeDeviceEnrollment_PinDeviceCommandId" ON "EmployeeDeviceEnrollment" ("PinDeviceCommandId");
CREATE INDEX IF NOT EXISTS "IX_EmployeeDeviceEnrollment_UserActivationDeviceCommandId" ON "EmployeeDeviceEnrollment" ("UserActivationDeviceCommandId");
ALTER TABLE "EmployeeDeviceEnrollment" ADD CONSTRAINT "FK_EmployeeDeviceEnrollment_TenantLocation" FOREIGN KEY ("TenantLocationId") REFERENCES "TenantLocation"("Id") ON DELETE RESTRICT;
ALTER TABLE "EmployeeDeviceEnrollment" ADD CONSTRAINT "FK_EmployeeDeviceEnrollment_TenantCardMaster" FOREIGN KEY ("TenantCardMasterId") REFERENCES "TenantCardMaster"("Id") ON DELETE RESTRICT;

CREATE TABLE IF NOT EXISTS "EmployeeDeviceAccessWindow" (
  "Id" bigserial PRIMARY KEY, "EmployeeDeviceEnrollmentId" bigint NOT NULL, "DayOfWeek" smallint NOT NULL,
  "StartLocalTime" time NOT NULL, "EndLocalTime" time NOT NULL, "IsActive" boolean NOT NULL DEFAULT true, "IsSoftDeleted" boolean NOT NULL DEFAULT false,
  "AddedById" bigint NOT NULL, "AddedDateTime" timestamp NOT NULL, "UpdatedById" bigint, "UpdatedDateTime" timestamp, "SoftDeletedById" bigint, "SoftDeletedDateTime" timestamp,
  CONSTRAINT "FK_EmployeeDeviceAccessWindow_Enrollment" FOREIGN KEY ("EmployeeDeviceEnrollmentId") REFERENCES "EmployeeDeviceEnrollment"("Id") ON DELETE CASCADE,
  CONSTRAINT "CK_EmployeeDeviceAccessWindow_TimeRange" CHECK ("StartLocalTime" < "EndLocalTime")
);
CREATE INDEX IF NOT EXISTS "IX_EmployeeDeviceAccessWindow_Enrollment" ON "EmployeeDeviceAccessWindow" ("EmployeeDeviceEnrollmentId");

-- Host Tenant RFID Management parent and its Card Inventory child module.
-- The parent is navigation-only; every CRUD mapping remains attached to the
-- Card Inventory child. The platform Super Admin role is id 1; additional Host
-- roles must be granted these mappings through the normal Host-role permission screen.
INSERT INTO axionpro."Module" ("TenantId","ModuleCode","ModuleName","DisplayName","URLPath","ParentModuleId","IsLeafNode","IsModuleDisplayInUI","IsCommonMenu","IsActive","ImageIconWeb","ImageIconMobile","ItemPriority","Remark","AddedById","AddedDateTime","ModuleScope")
SELECT NULL,'HOST_TENANT_RFID_MANAGEMENT','Host-Tenant-RFID-Management','Tenant RFID Management',NULL,NULL,FALSE,TRUE,FALSE,TRUE,'bi bi-broadcast-pin','contactless',525,'Host management of tenant RFID cards and card inventory.',1,CURRENT_TIMESTAMP,2
WHERE NOT EXISTS (SELECT 1 FROM axionpro."Module" WHERE "ModuleCode"='HOST_TENANT_RFID_MANAGEMENT');

UPDATE axionpro."Module" parent
SET "ModuleName"='Host-Tenant-RFID-Management', "DisplayName"='Tenant RFID Management', "URLPath"=NULL,
    "ParentModuleId"=NULL, "IsLeafNode"=FALSE, "IsModuleDisplayInUI"=TRUE, "IsCommonMenu"=FALSE,
    "IsActive"=TRUE, "ImageIconWeb"='bi bi-broadcast-pin', "ImageIconMobile"='contactless',
    "ItemPriority"=525, "Remark"='Host management of tenant RFID cards and card inventory.',
    "UpdatedById"=1, "UpdatedDateTime"=CURRENT_TIMESTAMP, "ModuleScope"=2
WHERE parent."ModuleCode"='HOST_TENANT_RFID_MANAGEMENT';

INSERT INTO axionpro."Module" ("TenantId","ModuleCode","ModuleName","DisplayName","URLPath","ParentModuleId","IsLeafNode","IsModuleDisplayInUI","IsCommonMenu","IsActive","ImageIconWeb","ImageIconMobile","ItemPriority","Remark","AddedById","AddedDateTime","ModuleScope")
SELECT NULL,'HOST_TENANT_CARD_INVENTORY','Host-Tenant-Card-Inventory','Card Inventory','/app/tenant-card-inventory',parent."Id",TRUE,TRUE,FALSE,TRUE,'bi bi-credit-card','credit-card',530,'Host-only procurement and lifecycle inventory for Tenant-issued physical cards.',1,CURRENT_TIMESTAMP,2
FROM axionpro."Module" parent
WHERE parent."ModuleCode"='HOST_TENANT_RFID_MANAGEMENT'
  AND NOT EXISTS (SELECT 1 FROM axionpro."Module" WHERE "ModuleCode"='HOST_TENANT_CARD_INVENTORY');

UPDATE axionpro."Module" child
SET "ParentModuleId"=parent."Id", "IsLeafNode"=TRUE, "IsModuleDisplayInUI"=TRUE, "IsActive"=TRUE,
    "UpdatedById"=1, "UpdatedDateTime"=CURRENT_TIMESTAMP, "ModuleScope"=2
FROM axionpro."Module" parent
WHERE child."ModuleCode"='HOST_TENANT_CARD_INVENTORY'
  AND parent."ModuleCode"='HOST_TENANT_RFID_MANAGEMENT';

INSERT INTO axionpro."ModuleOperationMapping" ("ModuleId","OperationId","PageURL","IconURL","IsCommonItem","IsOperational","Priority","Remark","IsActive","AddedById","AddedDateTime")
SELECT m."Id",o."Id",m."URLPath",m."ImageIconWeb",FALSE,TRUE,CASE lower(o."OperationName") WHEN 'view' THEN 10 WHEN 'create' THEN 20 WHEN 'update' THEN 30 WHEN 'delete' THEN 40 ELSE 99 END,'Card inventory permission.',TRUE,1,CURRENT_TIMESTAMP
FROM axionpro."Module" m CROSS JOIN axionpro."Operation" o
WHERE m."ModuleCode"='HOST_TENANT_CARD_INVENTORY' AND lower(btrim(o."OperationName")) IN ('view','create','update','delete') AND o."IsActive"=TRUE
AND NOT EXISTS (SELECT 1 FROM axionpro."ModuleOperationMapping" x WHERE x."ModuleId"=m."Id" AND x."OperationId"=o."Id");

INSERT INTO axionpro."HostRoleModuleAndPermission" ("HostRoleId","ModuleId","OperationId","IsActive","IsSoftDeleted","AddedById","AddedDateTime")
SELECT 1,m."Id",o."Id",TRUE,FALSE,1,CURRENT_TIMESTAMP
FROM axionpro."Module" m CROSS JOIN axionpro."Operation" o
WHERE m."ModuleCode"='HOST_TENANT_CARD_INVENTORY' AND lower(btrim(o."OperationName")) IN ('view','create','update','delete') AND o."IsActive"=TRUE
AND NOT EXISTS (SELECT 1 FROM axionpro."HostRoleModuleAndPermission" x WHERE x."HostRoleId"=1 AND x."ModuleId"=m."Id" AND x."OperationId"=o."Id" AND NOT x."IsSoftDeleted");
