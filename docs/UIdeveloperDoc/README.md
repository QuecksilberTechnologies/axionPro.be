# UI developer documents

- [Host Catalogue Bulk Upload](HOST_CATALOGUE_BULK_UI_IMPLEMENTATION.md) — four
  Host Admin catalogue bulk screens, API lifecycle, Excel contracts and UI examples.
- [Host Card and Device Bulk Upload](../bulk-upload/HOST_CARD_DEVICE_IMPORT.md)
- [Tenant and Employee Bulk API Scenarios](../bulk-upload/BULK_API_SCENARIOS.md)
- [Tenant Device Permission Contract](TENANT_DEVICE_PERMISSION_CONTRACT.md) — exact
  menu-operation mapping and screen-load API sequence.
- [Employee Profile Verification API](EMPLOYEE_PROFILE_VERIFICATION_API.md) — supported
  section identifiers, read-only rows and the update-bulk payload contract.

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
