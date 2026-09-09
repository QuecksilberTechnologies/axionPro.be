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
