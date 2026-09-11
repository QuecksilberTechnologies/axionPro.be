# AxionPro automated API + UI tests

This project runs API tests directly and opens Playwright-managed Chromium for UI tests. It uses NUnit, so every test appears in Visual Studio **Test Explorer**.

## What is already covered

- API: Swagger document is available.
- API: public `ClientInfo/detect-device` response contains the automation browser identity.
- API: authenticated navigation rejects a request without a token.
- API: a permission-aware Tenant Email Configuration CRUD flow (create, read, list, update, delete, and cleanup).
- Unit: Tenant device configuration permission behavior rejects wrong-module and denied-operation requests before a handler/queue can run, and confirms transport is resolved from stored configuration rather than a Tenant request field.
- Unit: the recent Device and Employee API surface is locked down end-to-end at the controller contract level: route and HTTP verb, authentication, Device DDL sections and values, employee permission-module bindings, and credential-response redaction. This covers tenant-device configuration, gateway/runtime commands, location and attendance setup, employee enrollment/work setup, and Host card inventory.
- Unit: Host administration regression tests cover all Host controllers plus the Host-facing Tenant, SMTP, and device endpoints. They confirm every non-onboarding action is authenticated, an already verified Tenant cannot trigger SMTP resend, that conflict is returned as HTTP 409, and Host users are intentionally denied Tenant runtime-configuration screens after device assignment while initial device bootstrap remains available.
- UI: the Angular login route loads in Chromium.

The UI test is deliberately skipped until a frontend address is supplied. That keeps `Run All` safe when the frontend is not running or is located outside this repository.

## One-time setup

1. Open `AxionPro.sln` in Visual Studio and allow NuGet restore to finish.
2. Build the `axionpro.automationtests` project once.
3. Install the browser revision matched to the project once:

```powershell
pwsh .\axionpro.automationtests\bin\Debug\net10.0\playwright.ps1 install chromium
```

## First run in Visual Studio

1. Start `axionpro.api` using the **http** launch profile. Swagger should open at `http://localhost:5170/swagger`.
2. In **Test Explorer**, run the tests in the `API` category. All three API tests should pass.
3. Start the Angular frontend from its own repository/application.
4. Change `WebBaseUrl` in `automationsettings.json` to the frontend address, for example `http://localhost:4200`.
5. Run the `UI` category in Test Explorer. A headless Chromium browser performs the login-page smoke test.

To watch the browser, set `Headless` to `false` in `automationsettings.json`, then rerun the UI test.

## Commands

### Durable bulk master import and designation department scope

`Category=BulkImport` covers Excel/CSV/paste parsing, column mapping, row errors,
department-specific designation names, existing roles, endpoint authentication and
the existing Department/Designation/Role permission behaviors.

`Category=BulkImportDatabase` executes actual designation repository and database
constraint checks, real persisted permission checks, confirmation idempotency,
scheduled execution, parallel workers, interrupted-batch recovery, partial-failure
retry, cancellation and the hosted background worker. Set `AXIONPRO_BULK_TEST_CONNECTION` to an isolated database named
`axionpro_bulk_test`, restored from the workforce backup, then apply
`database-scripts/EnforceDesignationDepartmentScope.sql` followed by
`database-scripts/AddDurableMasterBulkImport.sql` to that isolated database.
Never point these tests at production. Missing configuration is skipped explicitly;
designation tests roll back their changes; durable tests commit to verify recovery
and then remove only their own uniquely named records/jobs. The restored fixture
needs an active tenant employee/role with existing grants for all three masters.

```powershell
dotnet test .\axionpro.automationtests\axionpro.automationtests.csproj --filter "Category=BulkImport|Category=BulkImportDatabase" --logger "trx;LogFileName=bulk-import.trx" --results-directory artifacts/bulk-import
```

Department, Designation, Role and EmployeeType support saved previews, confirmation, durable
background execution, history, retry, cancellation, templates and result reports.
AI is disabled by user decision. Angular integration remains a separate frontend
task. The single API/UI handoff and progress reference is
`docs/AI_ASSISTED_BULK_IMPORT_REFERENCE.md`.

EmployeeType is tenant-owned. Also apply `AddTenantEmployeeTypes.sql` and
`SeedTenantEmployeeTypeModule.sql`, or run `database-scripts/ApplyBulkImportMigrations.ps1`
with the intended API environment. EmployeeTypeDatabaseTests cover ownership,
duplicate names, hidden legacy/foreign rows, onboarding and database assignment
constraints. Durable tests include EmployeeType import and repeatable migration
execution for both Development and Production selections, always overriding the
connection to the isolated test database. These runner checks require PowerShell 7
and psql; set AXIONPRO_TEST_PSQL_PATH if PostgreSQL is installed elsewhere.

```powershell
# API tests only
dotnet test .\axionpro.automationtests\axionpro.automationtests.csproj --filter "Category=API"

# UI tests only
dotnet test .\axionpro.automationtests\axionpro.automationtests.csproj --filter "Category=UI"

# Entire suite
dotnet test .\axionpro.automationtests\axionpro.automationtests.csproj

# Tenant Email Configuration CRUD only
dotnet test .\axionpro.automationtests\axionpro.automationtests.csproj --filter "Category=TenantEmailConfig"

# Tenant device configuration permission/transport behavior unit tests
dotnet test .\axionpro.automationtests\axionpro.automationtests.csproj --filter "FullyQualifiedName~TenantDeviceConfigurationPermissionBehaviorTests"

# Recent device, employee enrollment, work setup, card inventory and DDL API contract tests
dotnet test .\axionpro.automationtests\axionpro.automationtests.csproj --filter "Category=RecentDeviceEmployee"

# Host administration API scenarios: auth boundary, resend-verification state,
# HTTP 409 conflict envelope, and the Host/Tenant device ownership rule
dotnet test .\axionpro.automationtests\axionpro.automationtests.csproj --filter "Category=HostApi"
```

## Host API regression scenarios

The Host regression tests also compile the actual repository queries with the PostgreSQL EF provider.
Connection/command interceptors prevent database access and supply empty results: these checks validate
SQL translation, not production schema compatibility or populated response data. Coverage includes
Tenant lists (verification unset/true/false, search, active filter and paging), Tenant detail/update loading,
Host users/roles, device catalogue, installed devices and device connectivity lists.

The Tenant list regression reproduces `The LINQ expression 'employee' could not be translated` before
the fix. The filtered credential Include now compares TenantId through `credential.Employee.TenantId`,
preserving the tenant match without referencing the outer Include lambda. Deploy/restart the updated
API to apply this repository fix; no seed reset or schema migration is required.

| Scenario | Expected result |
| --- | --- |
| Host opens a Host administration page or calls its endpoint | The action has a concrete HTTP route and requires an authenticated session. |
| Anonymous registration / token verification | Only `POST /api/Tenant/create-tenant` and `POST /api/Tenant/verify` remain anonymous. |
| Host selects **Resend verification** for a Tenant already shown as verified | No token or SMTP call is made; the API returns the standard `409 Conflict` response. |
| Host issues the initial bootstrap URL for unassigned inventory | The permission behavior permits the request to reach the handler. |
| Host reads Tenant Device Configuration | List/detail reads require the scope-2 `TENANT_DEVICE_CONFIG` module and a persisted Host grant. A missing grant or a different module returns `403`. Runtime mutation ownership remains Tenant-only. |

### Authenticated Host device checks

`Category=HostLive` logs in and calls the real device/configuration list endpoints, checking HTTP 200,
the success envelope, array data and pagination. Supply `AXIONPRO_HOST_LOGIN_ID`,
`AXIONPRO_HOST_LOGIN_PASSWORD`, `AXIONPRO_HOST_DEVICE_MODULE_ID`,
`AXIONPRO_HOST_CONFIGURATION_MODULE_ID`, and `AXIONPRO_HOST_VIEW_OPERATION_ID` through the
environment. Obtain IDs from my-menu; configure the target with `AXIONPRO_TEST_API_BASE_URL`.
Missing settings cause an explicit skip, not a pass. No records are created or deleted by these list checks.

`Category=HostDatabaseRead` additionally reads actual rows and maps them through the application mapping
profile. Opt in with `AXIONPRO_HOST_DB_SETTINGS` pointing to the intended API settings JSON containing
`ConnectionStrings:DefaultConnection`. This detects missing columns that translation-only tests cannot find.

On 2026-09-09 the populated read reproduced PostgreSQL `42703` for
`PendingHttpsIngressTokenExpiresDateTime`. Apply `database-scripts/AddPendingHttpsGatewayReplacement.sql`
transactionally before the corresponding API release. This adds the pending token columns, constraint
and index without resetting seed data. The production device list returned HTTP 200 with one row after
the migration; the configuration permission fix additionally requires the updated API deployment.
Both authenticated HostLive tests passed against the updated local API using the configured database;
the populated database check mapped one device and one configuration row successfully.

## Configuration and secrets

`automationsettings.json` contains only local addresses and browser mode. Environment variables override it when needed:

```powershell
$env:AXIONPRO_TEST_API_BASE_URL = "http://localhost:5170"
$env:AXIONPRO_TEST_WEB_BASE_URL = "http://localhost:4200"
$env:AXIONPRO_TEST_HEADLESS = "false"
```

The Tenant Email Configuration CRUD test intentionally needs a dedicated Tenant Admin test account. Set these only in the terminal session that runs the test; do not save them in a file:

```powershell
$env:AXIONPRO_TEST_LOGIN_ID = "tenant-admin@example.test"
$env:AXIONPRO_TEST_LOGIN_PASSWORD = "your-test-password"
dotnet test .\axionpro.automationtests\axionpro.automationtests.csproj --filter "Category=TenantEmailConfig"
```

Before this test can run, apply `database-scripts/AxionPro_New_Production_Module_Operation_Seed.sql` to the intended **non-production** database, enable `TENANT_EMAIL_CONFIG` plus its CRUD operations for that tenant through the normal plan-entitlement synchronization, and assign Create, View, Update, and Delete to the test account's role. The test creates an **inactive** SMTP configuration and always deletes it, so it cannot replace the tenant's active mail configuration.

Do not place user passwords, JWTs, or production credentials in `automationsettings.json`.

## Artifacts when a UI test fails

On a failed UI test, Playwright saves a screenshot and trace ZIP under the test project's build output `artifacts` directory. The trace can be opened with:

```powershell
pwsh .\axionpro.automationtests\bin\Debug\net10.0\playwright.ps1 show-trace <trace-file.zip>
```

## Bulk validation checklist: read before every bulk-import session

Continuing contract: [AI_ASSISTED_BULK_IMPORT_REFERENCE.md](../docs/AI_ASSISTED_BULK_IMPORT_REFERENCE.md).
Read this checklist plus that reference before changes or deployment. Preserve
existing permission pipelines, constants, enums, mappings and handler/repository
patterns. Do not claim a target deployment or live HTTP acceptance pass from an
isolated database or controller-contract test.

### Source headers and filenames

| Source header for designation names | Current behavior |
| --- | --- |
| DesignationName / designation name / Designation_Name | Auto-matches after case/space/punctuation normalization |
| DesName / design name / DesignationType | Requires explicit manual mapping to DesignationName |
| Several possible designation-name columns | User selects the correct source; do not guess or merge columns |

Example ColumnMappingJson for source headers DesName and Dept:

```json
{"DesignationName":"DesName","DepartmentName":"Dept"}
```

For another alias, replace DesName with the exact source header. This is column
mapping, not database schema renaming. If DesignationType actually means a category
instead of a designation name, the user must select the correct column based on
its values. Semantic meaning is not automatically validated by AI; AI is disabled.
Unmapped required fields produce preview errors and CanCommit=false. Arbitrary
unused source columns are ignored and should be visibly marked as unmapped in UI.

The uploaded filename can be any name ending in .xlsx or .csv; it does not decide
the master type. The selected /api/Department, /Designation, /Role or /EmployeeType
route does. .cvs is not a supported extension. Raw files are not retained; normalized
preview rows/mapping/results are persisted in BulkImportJob. Multiple-sheet XLSX
requires the exact SheetName; no automatic semantic sheet selection exists.

### Validation and acceptance matrix

| Check | Required outcome |
| --- | --- |
| File versus pasted data | Exactly one source; empty input rejected |
| CSV | Valid UTF-8, quoted commas/newlines preserved, consistent row widths |
| XLSX | Valid bounded workbook, chosen sheet; reject formulas, error cells, merged cells and external worksheet links |
| Limits | 5 MiB file, 25 MiB expanded workbook, 5,000 rows, 64 columns, 4,000 characters/cell, 1,000 ZIP entries |
| Headers/mapping | Nonempty unique headers; supported targets only; one source column cannot populate several targets; required fields mapped |
| Required values | DepartmentName; DesignationName + DepartmentName; RoleName + existing numeric RoleType; EmployeeType TypeName |
| Text lengths | Department/Designation names 255; RoleName 100; Description 500 and Remark 200; EmployeeType fields each 255 |
| IsActive | true/false only; missing/blank defaults true |
| Source duplicates | Trim/case-insensitive identity duplicates invalidate all involved rows |
| Existing active match | Skip existing record; preserve values; no overwrite/reactivation |
| Inactive/ambiguous match | Preview error; no silent merge/reactivation |
| Designation ownership | Same name allowed in different departments; parent must be active and belong to this tenant |
| Role type | Existing Admin=1, Employee=2, Manager=3 only; conflicting existing type rejected; no role grants/employee assignment |
| EmployeeType ownership | Tenant-only options/reads/import; hide shared legacy/foreign rows; existing references migrated; cross-tenant assignment rejected |
| Authentication/permissions | Existing module/action grant required; client cannot choose TenantId/actor; View cannot confirm/create |
| Job ownership | Other tenant or another actor cannot read, confirm, cancel, retry or download this job |
| Confirmation | Only valid saved draft; no replacement rows; repeated Confirm does not create another job |
| RequestId | Same source/sheet/mapping replay returns saved draft; changed input with same ID returns conflict |
| Schedule | Null means eligible now; future UTC means not-before time; past time rejected |
| Concurrent changes | Worker rechecks DB and permissions per batch; unique constraints protect races; changed parent/error rows reported |
| Crash/restart | Master inserts and progress commit together; uncommitted batch rolls back; committed rows are not recreated |
| Retry | Only Failed/CompletedWithErrors; created rows retained; failed/pending rows revalidated |
| Cancel | Stops pending work at batch boundary; committed rows remain; no undo |
| Reports | Quoted CSV, formula protection, row numbers/IDs/errors; downloads are files, not JSON success envelopes |
| Infrastructure failure | Rollback/log/retry next poll; no attempt ceiling or automatic low-traffic detection implemented |

These are the implemented validation contracts, not a guarantee that every possible
conflict can be predicted at preview time. Another user can change data before the
worker runs; revalidation, database constraints and row error reporting handle that.
Structural validity also cannot prove a manually mapped column has the user's intended
business meaning. UI must show mapped sample values and require confirmation.

### Repeatable tests and target-deployment acceptance

- BulkImportPreviewTests: parser and mapping cases, duplicates, required values,
  master matching and the explicit designation alias examples across XLSX/CSV/paste.
- BulkImportPermissionTests: routes, authentication, permission pipeline and action restrictions.
- DesignationDepartmentDatabaseTests / EmployeeTypeDatabaseTests: real isolated-DB
  ownership, duplicate and migration/reference constraints.
- DurableBulkImportDatabaseTests: saved jobs, worker, replay, scheduling, concurrency,
  recovery, retry/cancel/history and migration runner in both environment selections.

```powershell
# Focused aliases/canonical headers: no DB required.
dotnet test axionpro.automationtests/axionpro.automationtests.csproj --no-restore --filter 'FullyQualifiedName~Custom_designation_headers|FullyQualifiedName~Canonical_designation_headers' --logger 'trx;LogFileName=bulk-header-mapping.trx' --results-directory artifacts/bulk-import
# Full backend suite: set AXIONPRO_BULK_TEST_CONNECTION to the isolated fixture first.
dotnet test axionpro.automationtests/axionpro.automationtests.csproj --no-restore --filter 'Category=BulkImport|Category=BulkImportDatabase' --logger 'trx;LogFileName=bulk-import.trx' --results-directory artifacts/bulk-import
```

Before a requested target deployment: identify the selected environment/connection
without logging credentials, verify backup and existing data conflicts, drain old
API/worker, apply the four scripts via ApplyBulkImportMigrations.ps1, start the new
API, verify schema/worker logs and existing entitlements/grants. Use the intended
authorized test tenant for authenticated XLSX/CSV/paste → preview → confirm → poll →
report checks. Include alias mapping, duplicates, cross-tenant denial and retry.
Verify actual persisted rows/counts against the job report and clean only explicitly
identified test records. Record environment, test cases, results, failure/skip details
and cleanup in the continuing reference. Never run the destructive isolated-DB
automation suite against production.

Current target migration/restart/live HTTP acceptance: PENDING. The user requested
confirmation of this behavior and document names before giving the next instruction.
The subsequent 2026-09-10 instruction grants migration/restart and completion approval
and forbids rerunning already tested cases. Target acceptance is now WIP: the intended
API URL/service host and authorized test tenant/account must be identified. Development
and Production configuration-only migration checks both succeeded and resolve to the
same remote database. No migration, restart or live acceptance has been performed;
existing passed test suites were not rerun. See the reference's target acceptance follow-up.

Latest focused run (2026-09-10): 18 alias/canonical-header tests passed, zero failed
or skipped. Result: artifacts/bulk-import/bulk-header-mapping.trx. Previous complete
backend run: 88 bulk + 119 regression tests passed; no actual-target deployment or
live HTTP validation was performed during this documentation/header review.

Production execution follow-up (2026-09-10): target identified as
https://axionpro-api.onrender.com. Backup archive created and fully read successfully;
Render suspension verified; all four Production migrations committed; post-migration
schema/ownership checks passed. Deployment `dep-dahcpf7qj5pc73a6mu1g` for commit
`d2754cd984e8edf2570d310e556fda68a8eb7716` succeeded and is Live. Actual Swagger
exposes all 32 bulk routes and runtime logs show queue polling. Authenticated bulk acceptance and
DB-result comparison remain PENDING on an authorized tenant login/session. Earlier
passed suites were not rerun. Backup location/hash and execution evidence are in
the main reference's Production execution section; this supersedes the earlier
statement that no target migration was performed.

Authenticated development acceptance (2026-09-10): tenant 8 passed nine live
Department/Designation/Role XLSX, CSV and pasted-CSV flows plus seven HTTP validation
checks. Nine completed jobs and six sample records matched downloaded reports in
read-only database reconciliation. EmployeeType remains BLOCKED on missing tenant
entitlement/module 79 and Add/View grants; its successful import is not a pass.
The existing passed test suites were not rerun. See `../docs/bulk-upload/README.md`
for sample XLSX/CSV files, actual response/report evidence and the remaining
EmployeeType-only command. No passwords or tokens are stored in those artifacts.

### Employee phase: 2026-09-11

77 distinct focused Employee-pattern/import/invitation tests passed. Zero remaining
failures/skips. The inventory and exact source TRX files are listed in
`../docs/bulk-upload/results/employee-automated-summary.json`. Changed locking and
capacity paths were retested; unrelated previously passed master suites were not.

New test categories: `EmployeeBulkCode`, `EmployeeBulkCodeDatabase`, `EmployeeImport`,
`EmployeeImportDatabase`, `EmployeeImportInvitations`. Database cases require
`AXIONPRO_BULK_TEST_CONNECTION` pointing to the disposable database named exactly
`axionpro_bulk_test`. They must never run against the target/production database.
The fixture requires existing tenant/admin/master data, a valid subscription and
all five bulk SQL migrations. Fixtures restore their test changes.

Focused command example (select only new/affected cases when continuing):

```powershell
dotnet test axionpro.automationtests/axionpro.automationtests.csproj `
  --artifacts-path artifacts/employee-code-tests `
  --filter 'TestCategory=EmployeeImportInvitations' `
  --logger 'trx;LogFileName=employee-invitations.trx' `
  --results-directory artifacts/bulk-import
```

Separate artifact output avoids locking a developer's running API executable.
For later runs, do not rerun all categories by default. Use the saved case inventory
to select unresolved cases or tests directly affected by new code changes.

Verified: original joining dates and Excel date systems; preserved suffixes and
counter reservations; preview/confirm staleness; pattern and account transactions;
tenant isolation; active references; Admin capacity count; role/image/contact
creation; duplicate rejection; invitation claim/retry/idempotency and public
response privacy. No real SMTP email was sent by these tests.

Authenticated HTTP/deployed acceptance remains PENDING. Automatic approval review
rejected a separate isolated API process launch with `blocked by policy`; no API
was started by that command. Do not report repository/route tests as live HTTP
passes. UI handoff: `../docs/bulk-upload/EMPLOYEE_IMPORT_UI.md`.

### Release correction, 11 September 2026

Release publish reproduced the EmployeeType handler/repository response namespace
mismatch in commit `3387971`. The handler now aliases the existing repository DTO.
Release publish passes. Two new `List_preserves_repository_response_and_page_metadata`
cases pass (0 failed/skipped); evidence is
`artifacts/bulk-import/employee-type-response-regression.trx`.
Only these new cases ran; the existing 77 Employee cases were not repeated.
EmployeeType live XLSX/CSV/paste bulk and DB/report verification also passed;
manual CRUD update/delete and live Employee acceptance remain pending deployment.
