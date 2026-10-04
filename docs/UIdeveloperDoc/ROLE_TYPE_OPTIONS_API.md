# Role Type Options API

## Purpose and screen behavior

`GET /api/Role/type-options` is the single source for the Role Type dropdown on
`/app/roles`. The Role list filter and the Add/Edit Role dialog use the same
Angular `RolesApi.roleTypes` signal. The response publishes four professional
access-persona labels while keeping persisted numeric values stable. Role names
such as Sales Manager or Developer and employee Designations remain separate
from this broad dashboard/access category.

The old Angular `ROLE_TYPES` array was removed. Do not add a second hardcoded role
type list in the UI.

## Authentication and permissions

- Bearer authentication is required.
- This constants lookup does not accept or validate `ModuleId` or `OperationId`.
- Angular's module-operation interceptor explicitly leaves this route unchanged.
- Existing Role create, update, list, delete, permission and bulk-import routes
  keep their established permission behavior.

The normal authorization header is supplied by the existing authentication
interceptor:

```http
GET /api/Role/type-options HTTP/1.1
Authorization: Bearer <access-token>
```

No dynamic permission-ID discovery is needed for this route. Numeric permission
IDs must not be appended.

## Request contract

| Method | Route | Query | Body |
| --- | --- | --- | --- |
| GET | `/api/Role/type-options` | None | None |

There are no mandatory or optional request fields, FormData fields, CSV columns,
polling parameters or cancellation commands.

## Success response

```json
{
  "isSucceeded": true,
  "message": "Role options fetched successfully.",
  "data": [
    {
      "id": 1,
      "name": "Tenant Administrator",
      "description": "Full company workspace administration, including users, roles, settings, and tenant-level configuration."
    },
    {
      "id": 2,
      "name": "Workforce User",
      "description": "Employee self-service access to personal information, attendance, leave, documents, and assigned work features."
    },
    {
      "id": 3,
      "name": "People Manager",
      "description": "Team management access for supervisors and managers, subject to assigned role permissions."
    },
    {
      "id": 4,
      "name": "External User",
      "description": "Limited portal access for clients, consultants, vendors, and other external users, subject to assigned role permissions."
    }
  ],
  "errors": []
}
```

Persisted values are stable: Tenant Administrator `1`, Workforce User `2`,
People Manager `3`, External User `4`. Existing database Role names such as
`Super-Admin`, `Employee`, `Manager`, and `Client` are not renamed by this lookup
change.
Unsupported persisted values still display as `Unknown` in Role/login response
mapping; they are not published as selectable options.

## Error handling

An unauthenticated request is rejected by the API authentication middleware. A
representative envelope is:

```json
{
  "isSucceeded": false,
  "message": "Unauthorized.",
  "data": null,
  "errors": [],
  "errorCode": "UNAUTHORIZED"
}
```

The Role page keeps the dropdown empty when the lookup fails and displays the
central API/interceptor error message. The user may reopen the dialog or reload
the page to retry. There is no background polling or automatic write retry.

## Persistence and isolation

The handler reads application constants only. It does not read or write a database,
does not create tenant-scoped records and has no retention or cleanup behavior.
All authenticated tenants receive the same supported role-type catalogue.

## Angular integration

`RolesApi.getRoleTypeOptions()` loads the endpoint. `RolesList` loads it for the
filter and normal dialog flow. `RoleDialog` also loads it when the shared signal is
empty, covering direct dialog use. Add and Edit use the same API-backed options.

## Verification status — 2026-10-04

- Backend professional persona mapping: 9 passed, 0 failed, 0 skipped.
- Protected tenant-registration regression: 14 passed, 0 failed, 0 skipped.
- Angular workspace was not changed or tested in this backend-only update.
- Authenticated local HTTP and deployed verification: pending.

Detailed evidence: [Professional role-type personas](../testing/role/professional-access-personas/2026-10-04.md).
