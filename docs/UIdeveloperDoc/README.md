# UI developer documents

- [Host Catalogue Bulk Upload](HOST_CATALOGUE_BULK_UI_IMPLEMENTATION.md) — four
  Host Admin catalogue bulk screens, API lifecycle, Excel contracts and UI examples.
- [Host Card and Device Bulk Upload](../bulk-upload/HOST_CARD_DEVICE_IMPORT.md)
- [Tenant and Employee Bulk API Scenarios](../bulk-upload/BULK_API_SCENARIOS.md)
- [Tenant Device Permission Contract](TENANT_DEVICE_PERMISSION_CONTRACT.md) — exact
  menu-operation mapping and screen-load API sequence.
- [Employee Profile Verification API](EMPLOYEE_PROFILE_VERIFICATION_API.md) — supported
  section identifiers, read-only rows and the update-bulk payload contract.
- [Employee list assigned roles](EMPLOYEE_GET_ALL_ASSIGNED_ROLES.md) — existing
  Employee get-all route with the `assignedRoles` response contract.
- [Organization Holiday Calendar](ORGANIZATION_HOLIDAY_CALENDAR.md) — location-based
  holiday persistence, date-only response contract, and rollout status.
- [Locality and Locality Type API](LOCALITY_AND_LOCALITY_TYPE_API.md) — Country/State/
  District/Locality lookup flow, City/Town/Village types, and TenantLocation payload.
Treat Module `PageName` and Operation `OperationName` as immutable identities during
seed and catalogue integration. Resolve permission IDs dynamically from the
authenticated menu; never hard-code the numeric examples in documentation.

## Mandatory document for every new API

Whenever a new API or endpoint is requested, add or update one handoff document in
this folder. Include:

1. Feature purpose, screen behavior and user actions.
2. Authentication plus dynamic ModuleId/OperationId discovery.
3. Every route, HTTP method and permission requirement.
4. Mandatory and optional fields with types and validation rules.
5. Copyable JSON, query-string, FormData and Excel/CSV examples as applicable.
6. Representative preview, success, progress and error response JSON.
7. Status/enum meanings, polling, retry, cancellation and report behavior.
8. Destination tables, persistence timing, isolation and retention behavior.
9. Exact automated test results and an explicit local/deployed status.

This handoff is part of the endpoint's completion checklist so another UI developer,
Claude or Codex can implement the frontend without guessing backend behavior.

- [Tenant Policy endpoint catalogue](TENANT_POLICY_ENDPOINT_CATALOG.md) — all
  policy routes with copyable request/response examples and verification status.

Latest Render API evidence: [2026-09-13 smoke test](../RENDER_SMOKE_TEST_2026-09-13.md).

Scenario test evidence is organized by module, scenario and date under
[docs/testing](../testing/README.md). Every tested scenario must have a report
there using the [scenario template](../testing/SCENARIO_TEMPLATE.md), with exact
local/live status, inputs, expected/actual results and outstanding checks.
Current Employee example: [China/USA identity options](../testing/employee/country-identity/2026-09-13.md).
