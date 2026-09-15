# SQL Rule: Tenant Module and Role Permission Creation

## Purpose

This document records the current application rules used when a tenant is created and its subscribed modules, enabled operations, and Tenant Admin permissions are generated.

## Direct answer: parent versus child module

- `TenantEnabledModule` receives both eligible parent modules and eligible child modules returned for the selected subscription plan.
- `TenantEnabledOperation` is prepared only from modules where `Module.IsLeafNode = true`.
- `RoleModuleAndPermission` is consequently created for the leaf/child module and its operations.
- A parent/container module does not normally receive a `RoleModuleAndPermission` row because it is not included in the leaf-module operation lookup.
- The parent remains available for navigation and hierarchy through `Module.ParentModuleId` and `TenantEnabledModule.ParentModuleId`.

Example:

```text
Employee Management                         Parent/container
├── Employees                               Leaf/child → role permissions
├── Departments                             Leaf/child → role permissions
└── Designations                            Leaf/child → role permissions
```

The stored permission combination is:

```text
RoleId + child/leaf ModuleId + OperationId
```

## Tenant-creation sequence

### 1. Read modules mapped to the selected plan

Source tables:

- `PlanModuleMapping`
- `Module`

`PlanModuleMapping` filters:

```text
SubscriptionPlanId = selected plan ID
IsActive = true
```

Duplicate module IDs are removed. A missing or invalid subscription plan returns no modules and tenant creation stops with `No modules found for selected subscription plan.`

Eligible `Module` filters:

```text
ModuleScope = 1 (Tenant) OR ModuleScope = 3 (Common)
IsModuleDisplayInUI = true
IsActive = true
```

The repository composes the eligible module branches so the required hierarchy is available.

### 2. Create tenant-enabled modules

For every returned subscription module, the application prepares a `TenantEnabledModule` row:

```text
TenantId       = newly created tenant ID
ModuleId       = Module.Id
ParentModuleId = Module.ParentModuleId
IsLeafNode     = Module.IsLeafNode
IsEnabled      = true
```

This step contains both eligible parents and children. It preserves the menu hierarchy for the tenant.

### 3. Select leaf modules for operation mapping

The application applies this filter:

```csharp
subscriptionModules.Where(module => module.IsLeafNode == true)
```

Therefore, only child/leaf modules proceed to operation-level permission preparation.

### 4. Read operations for leaf modules

Source tables:

- `ModuleOperationMapping`
- `Operation`

`ModuleOperationMapping` filters:

```text
ModuleId IN selected leaf-module IDs
IsActive = true
```

The current repository includes the related `Operation` record. This particular query does not add a separate `Operation.IsActive` predicate.

### 5. Create tenant-enabled operations

Each selected module-operation mapping is converted into `TenantEnabledOperation` and assigned the newly created `TenantId`.

Before Tenant Admin permissions are prepared, enabled operations are read using:

```text
TenantEnabledOperation.TenantId = newly created tenant ID
TenantEnabledOperation.IsEnabled = true
```

The result is grouped by `ModuleId`. The current read does not add separate `Module.IsActive` or `Operation.IsActive` filters; its effective filter is the tenant-specific enabled-operation row.

### 6. Find the Tenant Admin role

Source table:

- `Role`

Tenant Admin filters:

```text
TenantId = newly created tenant ID
RoleType = Admin
IsActive = true
IsSoftDeleted = false
IsSystemDefault = false
```

The default Manager and Employee roles may also be created during tenant registration, but this automatic full-permission step targets only the Tenant Admin role.

### 7. Create RoleModuleAndPermission rows

For every enabled child-module operation, the application prepares:

```text
RoleId      = Tenant Admin Role.Id
ModuleId    = enabled child/leaf Module.Id
OperationId = enabled Operation.Id
HasAccess   = true
IsActive    = true
Remark      = Auto-assigned during tenant creation for admin role
```

Before insertion, the in-memory list is deduplicated by:

```text
RoleId + ModuleId + OperationId
```

`RoleModuleAndPermission` has no direct `TenantId` column. Tenant ownership and isolation are derived through `Role.TenantId`.

## Tables involved and responsibility

| Table | Responsibility | Parent row included? |
|---|---|---:|
| `SubscriptionPlan` | Selected tenant plan | Not applicable |
| `PlanModuleMapping` | Maps plan to modules | Can reference a branch used to compose hierarchy |
| `Module` | Module metadata and parent-child hierarchy | Yes |
| `ModuleOperationMapping` | Declares operations available for a module | Leaf/child lookup during tenant creation |
| `Operation` | Operation metadata such as Add, Update, Delete, View, Import, and Export | Not applicable |
| `TenantEnabledModule` | Enables subscribed module hierarchy for one tenant | Yes, parent and child |
| `TenantEnabledOperation` | Enables a module-operation combination for one tenant | Generated from leaf/child modules |
| `Role` | Holds the tenant-owned role | Not applicable |
| `RoleModuleAndPermission` | Grants a role access to a module operation | Child/leaf module in the current creation flow |

## Why Add permission can be missing

For Department, Designation, Role, EmployeeType, or another tenant module, the Add permission will not be generated when any required link is absent:

1. The module is not actively mapped to the selected subscription plan.
2. The module is outside Tenant/Common scope.
3. The module is inactive or hidden from UI plan eligibility.
4. The module is not marked as a leaf node.
5. Its active `ModuleOperationMapping` for the Add operation is missing.
6. Its `TenantEnabledOperation` row is missing or disabled.
7. The Tenant Admin role is inactive, soft deleted, a system-default role, or belongs to another tenant.
8. The final `RoleModuleAndPermission` row is missing, inactive, or does not grant access.

## Diagnostic SQL pattern

Replace the example values before running this read-only diagnostic query:

```sql
SELECT
    role."TenantId",
    role."Id" AS "RoleId",
    role."RoleName",
    module."Id" AS "ModuleId",
    module."ModuleCode",
    module."ParentModuleId",
    module."IsLeafNode",
    operation."Id" AS "OperationId",
    operation."OperationName",
    permission."HasAccess",
    permission."IsActive" AS "PermissionIsActive",
    permission."IsSoftDeleted"
FROM axionpro."RoleModuleAndPermission" AS permission
JOIN axionpro."Role" AS role
  ON role."Id" = permission."RoleId"
JOIN axionpro."Module" AS module
  ON module."Id" = permission."ModuleId"
JOIN axionpro."Operation" AS operation
  ON operation."Id" = permission."OperationId"
WHERE role."TenantId" = :tenant_id
  AND role."IsActive" = TRUE
  AND role."IsSoftDeleted" = FALSE
  AND permission."HasAccess" = TRUE
  AND permission."IsActive" = TRUE
  AND permission."IsSoftDeleted" = FALSE
ORDER BY module."ParentModuleId", module."Id", operation."Id";
```

This query is diagnostic only. It does not change permissions.

## Implementation references

- `axionpro.application/Features/RegistrationCmd/Handlers/CreateTenantCommandHandler.cs`
- `axionpro.persistance/Repositories/PlanModuleMappingRepository.cs`
- `axionpro.persistance/Repositories/UserRolesPermissionOnModuleRepository.cs`
- `axionpro.persistance/Repositories/RoleRepository.cs`
- `axionpro.domain/Entity/RoleModuleAndPermission.cs`

## Maintenance rule

When module seed data changes, keep these relationships consistent:

```text
Module
→ ModuleOperationMapping
→ PlanModuleMapping
→ TenantEnabledModule
→ TenantEnabledOperation
→ RoleModuleAndPermission
```

Adding a module row alone does not grant permission. Its plan mapping, leaf classification, active operation mappings, tenant enablement, and role permission grant must all be present in the relevant flow.

## Host entitlement synchronization rule

The Host Admin entitlement synchronization API now completes this sequence inside the existing transaction:

```text
Active TenantSubscription and SubscriptionPlan
→ missing TenantEnabledModule rows
→ missing TenantEnabledOperation rows
→ save the entitlement snapshot
→ missing Tenant Admin RoleModuleAndPermission rows
→ commit
```

The final permission synchronization uses these source and target tables:

| Purpose | Table | Filter |
|---|---|---|
| Identify Tenant Admin | `Role` | Matching `TenantId`, Admin role type, active, not soft deleted, not system default |
| Identify entitled operations | `TenantEnabledOperation` | Matching `TenantId` and `IsEnabled = true` |
| Detect existing grants | `RoleModuleAndPermission` | Matching Tenant Admin `RoleId`, `ModuleId`, and `OperationId` |
| Insert missing grants | `RoleModuleAndPermission` | Only combinations for which no row currently exists |

Inserted permissions use:

```text
RoleId        = active Tenant Admin role ID
ModuleId      = enabled TenantEnabledOperation.ModuleId
OperationId   = enabled TenantEnabledOperation.OperationId
HasAccess     = true
IsActive      = true
IsSoftDeleted = false
AddedById     = authenticated Host user ID
```

The existence check deliberately considers every existing row for the exact `RoleId + ModuleId + OperationId` combination, including an inactive or soft-deleted row. Such a row is left unchanged and a second row is not inserted. Consequently, the synchronization is additive and duplicate-safe; it does not reactivate, overwrite, or delete an existing permission decision.

Only the active Tenant Admin role receives missing permissions. Other Tenant roles remain unchanged.
