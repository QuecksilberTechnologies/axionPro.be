-- Seeds the least-privilege modules required by secure device provisioning.
-- Run once after AddSecureInitialDeviceConfiguration.sql and before assigning roles.
-- Assign HOST_INITIAL_DEVICE_CONFIGURATION only to approved Host provisioning roles.
-- Assign TENANT_DEVICE_CONFIGURATION only to Tenant Admin roles.

BEGIN;

INSERT INTO axionpro."Module"
(
    "TenantId", "ModuleCode", "ModuleName", "DisplayName", "URLPath",
    "ParentModuleId", "IsLeafNode", "IsModuleDisplayInUI", "IsCommonMenu",
    "ModuleScope", "IsActive", "ItemPriority", "Remark", "AddedDateTime"
)
SELECT
    NULL,
    'HOST_INITIAL_DEVICE_CONFIGURATION',
    'Initial Device Provisioning',
    'Initial Device Provisioning',
    '/app/host/devices/initial-provisioning',
    NULL,
    true,
    false,
    false,
    2,
    true,
    0,
    'Host-only generation of short-lived initial physical-device gateway URLs.',
    CURRENT_TIMESTAMP
WHERE NOT EXISTS
(
    SELECT 1 FROM axionpro."Module"
    WHERE "ModuleCode" = 'HOST_INITIAL_DEVICE_CONFIGURATION'
);

INSERT INTO axionpro."Module"
(
    "TenantId", "ModuleCode", "ModuleName", "DisplayName", "URLPath",
    "ParentModuleId", "IsLeafNode", "IsModuleDisplayInUI", "IsCommonMenu",
    "ModuleScope", "IsActive", "ItemPriority", "Remark", "AddedDateTime"
)
SELECT
    NULL,
    'TENANT_DEVICE_CONFIGURATION',
    'Tenant Device Configuration',
    'Tenant Device Configuration',
    '/app/tenant/devices/configuration',
    NULL,
    true,
    false,
    false,
    1,
    true,
    0,
    'Tenant-admin-only runtime device configuration, gateway rotation, and reboot control.',
    CURRENT_TIMESTAMP
WHERE NOT EXISTS
(
    SELECT 1 FROM axionpro."Module"
    WHERE "ModuleCode" = 'TENANT_DEVICE_CONFIGURATION'
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
    'Secure device provisioning permission mapping.',
    true,
    0,
    CURRENT_TIMESTAMP
FROM axionpro."Module" module
INNER JOIN axionpro."Operation" operation
    ON lower(btrim(operation."OperationName")) IN ('create', 'add', 'view', 'read', 'update', 'edit', 'delete')
WHERE module."ModuleCode" IN ('HOST_INITIAL_DEVICE_CONFIGURATION', 'TENANT_DEVICE_CONFIGURATION')
  AND operation."IsActive" = true
  AND NOT EXISTS
  (
      SELECT 1
      FROM axionpro."ModuleOperationMapping" mapping
      WHERE mapping."ModuleId" = module."Id"
        AND mapping."OperationId" = operation."Id"
  );

COMMIT;
