# Auto-seed execution record

This folder records database seed and migration execution. A script is marked **EXECUTED** only after a target database connection returns success and its verification query is captured.

## Current run

Status: **SAFE BULK SEED EXECUTION COMPLETE** (development connection from `appsettings.Development.json`).

Executed successfully on 2026-09-11: `AddDurableMasterBulkImport.sql`, `AddEmployeeBulkImport.sql`, `SeedTenantEmployeeTypeModule.sql`, and `SeedBulkImportModules.sql`. Each returned successfully through Npgsql. Existing module names were preserved; scripts are conditional/idempotent.

Scope verification: bulk catalogue rows were inserted/verified with `ModuleScope = 1` (tenant scope). No Host Admin module, operation grant, tenant role grant, or tenant entitlement was inserted by the bulk module seed.

The full consolidated production seed was not replayed because it is a restore/reference script and could overwrite or conflict with existing production data.

The configured development connection is in `axionpro.api/appsettings.Development.json`. Before execution, take a target backup and confirm the target database. Then run scripts sequentially and append timestamp, script name, transaction result, and verification output here.

## Approved execution order

1. `AddDurableMasterBulkImport.sql`
2. `AddEmployeeBulkImport.sql`
3. `SeedTenantEmployeeTypeModule.sql`
4. `SeedBulkImportModules.sql`
5. `AxionPro_New_Production_Module_Operation_Seed.sql` only where its verification confirms an existing-safe/idempotent operation

The complete seed-data file is a reference/restore script, not an automatic replacement for the canonical production seed. Destructive scripts (`ClearTenantDependencies.sql`, reset/replace scripts, and full backup restore) are excluded from automatic execution.

## Scope recorded for the bulk scripts

- Existing module names are preserved; inserts are conditional on module code.
- EmployeeType operations remain CRUD plus the required `Import` operation.
- Bulk mappings are added for Employee, Department, Designation, Role/Permission, and EmployeeType.
- Role grants and tenant enablement remain on the existing permission pipeline.
