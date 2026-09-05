-- Run once in pgAdmin 4 against the AxionPro database after deploying the EmailTemplate CRUD API.
-- The EmailTemplate table already exists; this script only creates the Host permission module and maps existing CRUD operations.
-- Assign the resulting operation permissions to non-Super-Admin Host roles through Host Role Permissions.

BEGIN;

INSERT INTO axionpro."Module"
(
    "TenantId", "ModuleCode", "ModuleName", "DisplayName", "URLPath",
    "ParentModuleId", "IsLeafNode", "IsModuleDisplayInUI", "IsCommonMenu",
    "ModuleScope", "IsActive", "ItemPriority", "Remark", "AddedDateTime"
)
SELECT
    NULL,
    'HOST_EMAIL_TEMPLATE',
    'Email Templates',
    'Email Templates',
    '/app/email-templates',
    NULL,
    true,
    false,
    false,
    2,
    true,
    0,
    'Host-managed reusable email templates.',
    CURRENT_TIMESTAMP
WHERE NOT EXISTS
(
    SELECT 1
    FROM axionpro."Module"
    WHERE "ModuleCode" = 'HOST_EMAIL_TEMPLATE'
);

-- The API accepts the current active create/add, view/read, update/edit, and delete operation IDs.
-- If this deployment has renamed operations, map the appropriate active operation through the Host UI instead.
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
    'Email template permission mapping.',
    true,
    0,
    CURRENT_TIMESTAMP
FROM axionpro."Module" module
INNER JOIN axionpro."Operation" operation
    ON lower(btrim(operation."OperationName")) IN ('create', 'add', 'view', 'read', 'update', 'edit', 'delete')
WHERE module."ModuleCode" = 'HOST_EMAIL_TEMPLATE'
  AND operation."IsActive" = true
  AND NOT EXISTS
  (
      SELECT 1
      FROM axionpro."ModuleOperationMapping" mapping
      WHERE mapping."ModuleId" = module."Id"
        AND mapping."OperationId" = operation."Id"
  );

COMMIT;
