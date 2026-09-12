-- Applies to existing and future employees through tenant-level lookup.
BEGIN;
CREATE TABLE IF NOT EXISTS axionpro."TenantEmployeeSectionDefault" (
    "TenantId" bigint NOT NULL REFERENCES axionpro."Tenant"("Id"),
    "ModuleCode" varchar(64) NOT NULL,
    "IsEditAllowed" boolean NOT NULL DEFAULT false,
    "UpdatedById" bigint NOT NULL,
    "UpdatedDateTime" timestamp with time zone NOT NULL DEFAULT now(),
    PRIMARY KEY ("TenantId", "ModuleCode"),
    CHECK ("ModuleCode" IN ('EMP_WORK_LOCATIONS', 'EMP_DEVICES',
        'EMP_WORK_ARRANGEMENT', 'EMP_WORK_PATTERN', 'EMP_OVERRIDES'))
);
COMMIT;
