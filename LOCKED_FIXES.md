# Locked Fix Regression Protocol

This file is the canonical registry for user-approved fixes whose behavior must
remain stable while later work is performed. It applies to every contributor,
account, automated agent and session working on this project.

## Mandatory first-read rule

1. Read this entire file before inspecting, changing or testing application
   code, database state, configuration or API behavior.
2. Read the repository `AGENTS.md` and any feature-specific reference named by
   it before starting work.
3. Before fixing another feature, identify every locked item that the proposed
   work can affect through shared handlers, repositories, entities, database
   tables, middleware, permissions, response contracts or UI consumers.
4. Run the affected locked items' **pre-change regression gate** before making
   the new change. If a gate fails, stop the new feature work, record the actual
   failure and tell the user. Do not hide the failure or relabel it as unrelated.
5. After the authorized change, rerun the same locked gates and the new
   feature's tests. A new fix is not locked until both sets pass and its evidence
   is recorded.

These regression runs protect established behavior and therefore are not
documentation-only reruns.

## Angular authorization boundary

- Do not edit, format, generate, build, test, install dependencies in, or
  otherwise change the Angular workspace unless the user explicitly authorizes
  Angular work in the current request.
- Backend work, an API error, a related feature name or an existing UI consumer
  does not imply permission to touch Angular.
- If an authorized backend change requires an Angular contract change, prepare
  the backend result and explain the exact required UI change. Wait for explicit
  Angular authorization before applying it.
- When Angular work is authorized, read the Angular repository's `AGENTS.md`
  before touching that workspace and run every applicable UI lock gate.

## Meaning of a locked fix

A `LOCKED` entry establishes its listed observable behavior as a regression
baseline. Later work must not silently change that behavior, API contract,
transaction outcome, permission path, persistence rule or user-facing error.

- Only an explicit user instruction can change, unlock or retire locked
  behavior.
- A refactor is not permission to alter a locked result.
- An expanded test suite may add coverage, but it must preserve the original
  locked assertions unless the user approves a behavior change.
- Skipped, blocked and not-run checks are never passes.
- Local verification is not deployed acceptance.
- Never store credentials, tokens, real identity numbers or unredacted personal
  documents in this registry or its evidence reports.

## Required workflow for every small fix

### 1. Define the requested scope

- Restate the exact defect and intended result.
- List the files, APIs, tables and consumers likely to be affected.
- Ask the user if intended behavior is unclear; do not invent a flow.

### 2. Check existing locks before changing code

- Select applicable entries from the registry below.
- Run each selected entry's pre-change gate exactly as documented.
- Record passed, failed, skipped and blocked counts honestly.
- If no lock applies, state why in the new scenario report.

### 3. Implement only the authorized fix

- Preserve existing project patterns, permission flow, regions, comments,
  constants, enums and mappings.
- Do not combine an unrelated cleanup, refactor or UI change with the fix.
- Do not weaken an existing assertion just to make a regression pass.

### 4. Verify and lock the result

- Rerun all applicable pre-change gates as post-change gates.
- Add meaningful cases to an existing relevant test when that is the established
  test location. Create a new suite only when no suitable suite exists.
- Create the required scenario report under
  `docs/testing/<module>/<scenario>/<YYYY-MM-DD>.md` and link it from the test
  indexes required by `AGENTS.md`.
- Add or update one registry entry below. Mark it `LOCKED` only after required
  local checks pass. Record deployed verification separately.

## Lock entry requirements

Every entry must include:

- stable lock ID, date and status;
- user-approved behavior that must remain unchanged;
- protected APIs, code areas and persistence objects;
- exact pre-change and post-change regression commands;
- expected assertions or minimum result;
- evidence report;
- local and deployed verification status;
- any approved exception or remaining acceptance item.

## Lock registry

| Lock ID | Area | Status | Local baseline | Deployed status | Evidence |
| --- | --- | --- | --- | --- | --- |
| `LOCK-TENANT-REG-001` | Tenant registration transaction and actionable errors | LOCKED | Backend 14/14; PostgreSQL rollback 2/2; Angular 44/44 and production build | PENDING | [2026-10-01](docs/testing/tenant/registration-actionable-errors/2026-10-01.md) |
| `LOCK-ROLE-TYPE-002` | Tenant role-type response mapping and API-backed DDL | LOCKED | Backend 9/9; protected tenant registration 14/14; Angular 69/69 and production build | PENDING | [2026-10-01](docs/testing/role/client-role-type-display/2026-10-01.md) |

## LOCK-TENANT-REG-001: Tenant registration transaction and actionable errors

### Locked behavior

- Public and Host tenant registration follow the established transaction flow.
- A failed creation does not leave a partially committed tenant graph.
- Duplicate email is rejected before a transaction begins.
- Known database conflicts and unexpected failures return a safe, actionable
  stage-specific message and stable error code. Raw exception, SQL and database
  details remain server-log-only.
- Request cancellation rolls back and returns a clear cancellation result.
- Verification-email failure after commit does not roll back the created tenant;
  the result reports that the email was not sent.
- Tenant registration does not implicitly create obsolete Insurance or Leave
  policy types.
- The Angular registration screen, when explicitly authorized for UI work,
  displays the API message plus distinct validation details and uses clear
  fallbacks when no detail is available.

### Protected areas

- `axionpro.application/Features/RegistrationCmd/Handlers/CreateTenantCommandHandler.cs`
- `axionpro.application/Constants/AppConstants.cs`
- `axionpro.api/Controllers/Tenant/TenantController.cs`
- `axionpro.automationtests/Unit/HostApiRegressionTests.cs`
- Tenant, tenant subscription, security key, location, employee, role,
  permission and tenant-profile persistence used by registration.
- Angular registration response contract, component and existing component
  specification. The Angular authorization boundary above still applies.

### Required backend gate

Run before and after any change that can affect this lock:

```powershell
dotnet test .\axionpro.automationtests\axionpro.automationtests.csproj -c Release --no-restore --filter "FullyQualifiedName~HostApiRegressionTests.Tenant_creation_awaits_dependencies_and_preserves_transaction_outcome" --logger "console;verbosity=minimal"
```

Expected locked baseline: all named public and Host cases pass. At lock time the
result was 14 passed, 0 failed and 0 skipped.

For database, entity, repository, transaction or registration-persistence
changes, also run the opt-in rollback probe against the explicitly approved
Development settings:

```powershell
$env:AXIONPRO_HOST_DB_SETTINGS=(Resolve-Path '.\axionpro.api\appsettings.Development.json').Path
dotnet test .\axionpro.automationtests\axionpro.automationtests.csproj -c Release --no-build --filter "FullyQualifiedName~HostApiRegressionTests.Tenant_creation_real_database_rollback_probe" --logger "console;verbosity=minimal"
```

Expected locked baseline: 2 passed, 0 failed and 0 skipped, with both transactions
rolled back and no test tenant persisted. If the environment variable or
database is unavailable, record the probe as BLOCKED or SKIPPED and do not claim
database regression acceptance.

### Required Angular gate

Run only after the user explicitly authorizes Angular work:

```powershell
bun run test -- --include src/app/features/authentication/registration/registration.spec.ts
bun run build -- --configuration production
```

Expected locked baseline: the focused registration specification passes (44
tests at lock time) and the production bundle builds successfully.

### Known data-integrity prerequisite

Tenant cleanup must not leave orphan tenant-encryption-key rows. The lock-time
failure was caused by four orphan `TenantEncryptionKeys` rows colliding with
reused tenant identities; the local orphans were removed and the final orphan
count was zero. Do not delete or alter tenant data for a regression run without
the user's explicit authorization.

### Acceptance state

- Local backend mocked gate: PASS.
- Local PostgreSQL rollback gate: PASS.
- Local Angular focused tests and production build: PASS under the authorization
  that created this lock.
- Deployed end-to-end registration: PENDING and must not be reported as passed
  until independently executed and reconciled.

## LOCK-ROLE-TYPE-002: Tenant role-type response mapping and API-backed DDL

### Locked behavior

- Persisted Tenant role type `1` displays as `Super Admin`.
- Persisted Tenant role type `2` displays as `Employee`.
- Persisted Tenant role type `3` displays as `Manager`.
- Persisted Tenant role type `4` displays as `Client`, never `Unknown`.
- An unsupported numeric role type uses the safe `Unknown` fallback.
- Role GET and login role responses resolve names through the same centralized
  constants mapping.
- `GET /api/Role/type-options` is bearer-authenticated and returns the supported
  1–4 option catalogue from the same constants. It accepts no ModuleId or
  OperationId and deliberately bypasses the Role module-operation pipeline.
- Existing Role create/update/list/delete, permission and bulk-import requests
  retain their established permission behavior.
- Angular Role Add, Edit and filter controls use the API-backed shared signal;
  no hardcoded `ROLE_TYPES` list may replace it. The Angular permission
  interceptor must not attach permission IDs to this lookup.

### Protected areas

- `axionpro.application/Constants/AppConstants.cs`, Tenant Role Types region.
- `axionpro.application/Mappings/MappingProfile.cs`, mappings from `Role` to
  `GetRoleResponseDTO` and `NewLoginRoleDTO`.
- `axionpro.application/Features/RoleCmd/Handlers/GetRoleTypeOptionsQueryHandler.cs`.
- `axionpro.application/Features/RoleCmd/RolePermissionBehavior.cs`, the narrow
  `GetRoleTypeOptionsQuery` exception only.
- `axionpro.api/Controllers/Role/RoleController.cs`, `GET type-options`.
- `axionpro.automationtests/Unit/RoleTypeMappingTests.cs`.
- `GET /api/Role/get` role-type fields and login role-type display fields.
- Angular `roles-api.ts`, `role-dialog`, `roles-list`, role interfaces and the
  `/Role/type-options` interceptor exclusion.

### Required backend gate

Run before and after any change that can affect this lock:

```powershell
dotnet test .\axionpro.automationtests\axionpro.automationtests.csproj -c Release --no-restore --filter "TestCategory=RoleTypeMapping" --logger "console;verbosity=minimal"
```

Expected locked baseline: 9 passed, 0 failed and 0 skipped. The assertions cover
types 1, 2, 3, 4 and an unsupported value across the central resolver and response
mappings, the four-option query output, the no-permission pipeline exception and
the authenticated controller route. A regular Role query still rejects missing
permission IDs, proving the exception does not weaken existing Role operations.

Because `AppConstants.cs` is also protected by `LOCK-TENANT-REG-001`, changes to
that file must additionally run that lock's backend gate.

### Required Angular gate

Run before and after changes to the protected Role option consumers or interceptor:

```powershell
C:\latestAxionProUI\axionpro-app\node_modules\.bin\ng.exe test --project axionpro --include C:\latestAxionProUI\axionpro-app\src\app\features\roles\role-dialog\role-dialog.spec.ts --include C:\latestAxionProUI\axionpro-app\src\app\core\services\roles-api.spec.ts --include C:\latestAxionProUI\axionpro-app\src\app\core\interceptors\module-operation-routes.spec.ts --watch=false
```

Expected locked baseline: 3 files and 69 tests pass.

### Acceptance state

- Local backend gate: PASS, 9/9.
- Shared constants tenant-registration gate: PASS, 14/14.
- Angular Role option gate: PASS, 69/69.
- Angular production build: PASS.
- Authenticated local Role GET JSON: PENDING.
- Authenticated local `GET /api/Role/type-options`: PENDING.
- Deployed Role GET: PENDING.
- Deployed Role type options and UI acceptance: PENDING.
- Angular Role create/edit dropdown: known hardcoded 1–3 list, unchanged and
  outside this backend lock.

## Adding the next lock

Append the next entry; do not rewrite or erase historical locked evidence. If a
new authorized requirement intentionally conflicts with a lock, mark the old
entry `SUPERSEDED` with a link to the user's decision and the replacement lock.
Keep the earlier report available.
