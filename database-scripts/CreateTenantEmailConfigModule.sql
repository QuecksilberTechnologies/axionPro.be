-- Run once in pgAdmin 4 against the AxionPro database.
-- This seeds separate Host and Tenant EmailConfig authorization modules and
-- their CRUD operations. It does not read, write, or expose SMTP credentials.
-- Assign the resulting operation permissions to the required Host and Tenant
-- Admin roles through the respective role-permission administration screens.

BEGIN;

INSERT INTO axionpro."Module"
(
    "TenantId", "ModuleCode", "ModuleName", "DisplayName", "URLPath",
    "ParentModuleId", "IsLeafNode", "IsModuleDisplayInUI", "IsCommonMenu",
    "ModuleScope", "IsActive", "ItemPriority", "Remark", "AddedDateTime"
)
SELECT
    NULL,
    'HOST_TENANT_EMAIL_CONFIG',
    'Tenant Email Configuration',
    'Tenant Email Configuration',
    '/app/tenants/tenant-email-config',
    NULL,
    true,
    false,
    false,
    2,
    true,
    0,
    'Host-admin management of tenant-specific SMTP configuration.',
    CURRENT_TIMESTAMP
WHERE NOT EXISTS
(
    SELECT 1
    FROM axionpro."Module"
    WHERE "ModuleCode" = 'HOST_TENANT_EMAIL_CONFIG'
);

INSERT INTO axionpro."Module"
(
    "TenantId", "ModuleCode", "ModuleName", "DisplayName", "URLPath",
    "ParentModuleId", "IsLeafNode", "IsModuleDisplayInUI", "IsCommonMenu",
    "ModuleScope", "IsActive", "ItemPriority", "Remark", "AddedDateTime"
)
SELECT
    NULL,
    'TENANT_EMAIL_CONFIG',
    'Tenant Email Configuration',
    'Tenant Email Configuration',
    '/app/tenant-email-config',
    NULL,
    true,
    false,
    false,
    1,
    true,
    0,
    'Tenant-admin management of tenant-specific SMTP configuration.',
    CURRENT_TIMESTAMP
WHERE NOT EXISTS
(
    SELECT 1
    FROM axionpro."Module"
    WHERE "ModuleCode" = 'TENANT_EMAIL_CONFIG'
);

INSERT INTO axionpro."ModuleOperationMapping"
(
    "ModuleId", "OperationId", "IsCommonItem", "IsOperational", "Priority",
    "Remark", "IsActive", "AddedById", "AddedDateTime"
)
SELECT
    module."Id",
    operation."Id",
    false,
    true,
    0,
    'Tenant email configuration permission mapping.',
    true,
    0,
    CURRENT_TIMESTAMP
FROM axionpro."Module" module
INNER JOIN axionpro."Operation" operation
    ON lower(btrim(operation."OperationName")) IN ('create', 'add', 'view', 'read', 'update', 'edit', 'delete')
WHERE module."ModuleCode" IN ('HOST_TENANT_EMAIL_CONFIG', 'TENANT_EMAIL_CONFIG')
  AND operation."IsActive" = true
  AND NOT EXISTS
  (
      SELECT 1
      FROM axionpro."ModuleOperationMapping" mapping
      WHERE mapping."ModuleId" = module."Id"
        AND mapping."OperationId" = operation."Id"
  );

COMMIT;
