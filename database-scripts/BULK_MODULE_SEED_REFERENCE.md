# Bulk module seed reference

Updated: 2026-09-11. This note records which module/operation seed is authoritative
for bulk onboarding and what each seed changes. It does not grant permissions.

## Immutable PageName rule — user-confirmed 2026-09-13

Never change an existing Module `PageName` while editing Module, child-module,
Operation or ModuleOperationMapping seed sections. `PageName` is the stable UI
component-registration identity, independent of URLPath and display metadata.
Assign the approved value when a module is first inserted; every later seed rerun
must preserve it. This applies to Tenant and Host module seeds.

## Immutable OperationName rule — user-confirmed 2026-09-13

Never rename or overwrite an existing `Operation.OperationName` in Module,
child-module, Operation or ModuleOperationMapping seed work. Reuse the existing
canonical operation/type. If an operation type is genuinely missing, insert its
approved initial name once; subsequent reruns must preserve the stored name and
must not introduce an alias or duplicate operation.

## Which seed to use?

`database-scripts/AxionPro_New_Production_Module_Operation_Seed.sql` is the
canonical production module/operation seed. Use it for normal release execution.

`database-scripts/complete seed data/AxionPro_New_Production_Module_Operation_Seed.sql`
contains the same consolidated sections plus 38 backup-recovered `Module` rows:
common menu/profile leaves, employee profile leaves, tenant/host hierarchy and
device inventory/location leaves. It is a complete fresh/restore reference, not
a drop-in production replacement; its extra rows can be environment-specific and
IDs are generated. The later operation/mapping logic is otherwise the same, with
shifted line numbers.

## Bulk-related module entries

| ModuleCode | Scope | Used by |
|---|---:|---|
| `EMP_LIST` | Tenant | Employee bulk preview/confirm/jobs/report/template/invitations |
| `TENANT_DEPARTMENTS` | Tenant | Department bulk |
| `TENANT_DESIGNATIONS` | Tenant | Designation bulk |
| `TENANT_ROLES_PERMISSIONS` | Tenant | Role bulk |
| `TENANT_EMPLOYEE_TYPES` | Tenant | EmployeeType bulk |
| `TENANT_EMPLOYEE_CODE` | Tenant | Employee-code pattern add/update |

The complete seed's approved navigation hierarchy is now functional-parent based:
`BULK_EMPLOYEES` under `EMP_MGMT`; Department, Designation, Role and EmployeeType
bulk children under their corresponding existing tenant modules. There is no
standalone tenant `BULKUPLOAD` parent. Each bulk child has View (4), Export (11)
and Import (12) catalogue mappings. Export is seed permission metadata only; its
UI implementation belongs to the UI developer.

Local integration verification (2026-09-13): the entire complete seed ran twice
successfully on an isolated clone after adding its required test-only email-config
fixture. Both runs produced five bulk children, no `BULKUPLOAD` root, exactly 15
active tenant bulk mappings, and five plan mappings per child. Both canonical Host
Admin accounts were present. No production DB was used. Existing duplicate Host
Card/Device Import mappings in the baseline are recorded separately and were not
altered by this tenant-only hierarchy decision.

`TENANT_EMPLOYEE_TYPES` is also maintained by the idempotent
`database-scripts/SeedTenantEmployeeTypeModule.sql`; the current complete consolidated
seed contains that module as well. That script creates CRUD catalogue/plan coverage;
tenant grants still use entitlement synchronization and the existing permission API.

For the complete bulk catalogue, run the new idempotent
`database-scripts/SeedBulkImportModules.sql`. It ensures the five bulk master
modules, the Import operation (type 12), their `ModuleOperationMapping` rows and
plan inheritance from `EMP_LIST`. It intentionally does not create
`TenantEnabledModule`, `TenantEnabledOperation` or `RoleModuleAndPermission`
grants; run the existing entitlement synchronization and role-permission command
afterward to make Bulk Import appear in a tenant's menu.

Bulk master import uses the existing Add/Import grant. Canonical CRUD operation
types are View=4, Create/Add=1, Update=2, Delete=3; Import is enum value 12 when
the environment has an Import operation. Do not hardcode IDs or grant roles in UI.

## Tables changed

| Seed section | Tables | Effect |
|---|---|---|
| Email setup | `DefaultEmailConfig`, `TenantEmailConfig` | Creates/upgrades email config and active defaults |
| Module definitions | `Module` | Idempotently inserts missing metadata; parents resolve by ModuleCode |
| Operation normalization | `Operation` | Ensures active CRUD metadata/icons |
| Catalogue mappings | `ModuleOperationMapping` | Adds/updates module-operation catalogue rows |
| Plan inheritance | `PlanModuleMapping` | Copies selected baseline modules to subscription plans |
| EmployeeType add-on | `Module`, `ModuleOperationMapping`, `PlanModuleMapping`, `TenantEnabledOperation`, stale `RoleModuleAndPermission` | Adds CRUD catalogue, cleans stale EmployeeType rows; no role grant |
| Host baseline | Host role/permission tables and related module tables | Normalizes existing Host hierarchy/baseline |

Bulk job storage is separate: run `AddDurableMasterBulkImport.sql` and
`AddEmployeeBulkImport.sql` for `axionpro.BulkImportJob` and Employee uniqueness
constraints.

## Recommended reference and order

Use the canonical seed for deployment, then `SeedBulkImportModules.sql` (which
covers EmployeeType catalogue too; the older EmployeeType-only script remains
safe/idempotent),
existing Host entitlement synchronization, existing tenant role-permission grants,
and finally durable bulk migrations. Verify by stable ModuleCode and authenticated
`my-menu`; never assume numeric IDs. Before merging the 38 backup-only modules into
the canonical seed, review their product scope—they are not all bulk modules.
## Host catalogue bulk seed — 2026-09-13

`SeedHostBulkImportModules.sql` and the embedded consolidated complete-seed block
add these scope-2 children without `PlanModuleMapping` rows:

| Child code | Parent code | PageName |
| --- | --- | --- |
| HOST_MODULE_CATALOGUE_BULK | HOST_MODULES | host-module-bulk |
| HOST_SUBMODULE_CATALOGUE_BULK | HOST_SUBMODULES | host-submodule-bulk |
| HOST_OPERATION_CATALOGUE_BULK | HOST_OPERATIONS | host-operation-bulk |
| HOST_MODULE_OPERATION_CATALOGUE_BULK | HOST_MODULE_OPERATIONS | host-module-operation-bulk |

Each child maps one canonical View (4), Export (11), and Import (12). Grant them
through the existing Host role permission flow. The seed ran twice on the isolated
`axionpro_bulk_test`; identities and counts remained stable.

The consolidated seed reparents known legacy Host bulk children before deleting
the obsolete `BULKUPLOAD` root. This prevents `FK_Module_ParentModule` failures on
databases seeded by an earlier hierarchy. If an unknown legacy child remains, the
root is hidden/deactivated and retained for review instead of aborting the seed.
