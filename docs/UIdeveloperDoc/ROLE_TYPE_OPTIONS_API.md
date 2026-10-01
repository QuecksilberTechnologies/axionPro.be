# Role Type Options API

## Purpose and screen behavior

`GET /api/Role/type-options` is the single source for the Role Type dropdown on
`/app/roles`. The Role list filter and the Add/Edit Role dialog use the same
Angular `RolesApi.roleTypes` signal. The response includes Client (`id: 4`), so a
Client role created during tenant registration is selectable and remains selected
when edited.

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
      "name": "Admin",
      "description": "Full access to manage users, roles, settings, and system-level configurations."
    },
    {
      "id": 2,
      "name": "Employee",
      "description": "Can manage team members, assign tasks, and oversee day-to-day operations."
    },
    {
      "id": 3,
      "name": "Manager",
      "description": "Limited access to perform assigned tasks and view only relevant information."
    },
    {
      "id": 4,
      "name": "Client",
      "description": ""
    }
  ],
  "errors": []
}
```

Persisted values are stable: Admin `1`, Employee `2`, Manager `3`, Client `4`.
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

## Verification status — 2026-10-01

- Backend Role type contract: 9 passed, 0 failed, 0 skipped.
- Protected tenant-registration regression: 14 passed, 0 failed, 0 skipped.
- Angular Role API/dialog/route-resolution specs: 69 passed, 0 failed.
- Angular production build: passed.
- ESLint and formatter checks for the nine changed Angular files: passed.
- Authenticated local HTTP and deployed verification: pending.

Detailed evidence: [Role Client type display scenario](../testing/role/client-role-type-display/2026-10-01.md).
