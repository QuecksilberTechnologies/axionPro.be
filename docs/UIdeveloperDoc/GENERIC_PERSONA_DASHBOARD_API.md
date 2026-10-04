# Generic Persona Dashboard API

## Behavior

The dashboard is resolved from the authenticated user's trusted RoleType. The UI must not send or override a `roleTypeCode`.

```text
Bearer identity -> permission pipeline -> trusted RoleTypeId -> stable RoleTypeCode -> allowed widgets
```

Supported codes are `TENANT_ADMIN`, `PEOPLE_MANAGER`, `WORKFORCE_USER`, and `EXTERNAL_USER`.

## Authentication and permission discovery

Both endpoints require the bearer token and the Dashboard View `moduleId` and `operationId`. Resolve these numeric values from the existing authenticated menu/permission flow; never hardcode the sample values below.

## Endpoints

| Method | Route | Purpose |
| --- | --- | --- |
| GET | `/api/Dashboard/Configuration` | Returns the caller's trusted persona and ordered widget catalogue |
| GET | `/api/Dashboard/Widget/{widgetCode}` | Returns one widget allowed for that persona |

Example:

```http
GET /api/Dashboard/Configuration?moduleId=<dashboard-module>&operationId=<view-operation>
Authorization: Bearer <token>
```

Configuration response:

```json
{
  "isSucceeded": true,
  "data": {
    "roleTypeCode": "PEOPLE_MANAGER",
    "roleTypeName": "People Manager",
    "widgets": [
      {
        "code": "TEAM_SUMMARY",
        "title": "Team Summary",
        "displayOrder": 1,
        "isPlaceholder": true
      }
    ]
  }
}
```

The UI should request visible widgets independently and may run those HTTP requests concurrently:

```http
GET /api/Dashboard/Widget/TEAM_SUMMARY?moduleId=<dashboard-module>&operationId=<view-operation>
Authorization: Bearer <token>
```

Widget response:

```json
{
  "isSucceeded": true,
  "data": {
    "code": "TEAM_SUMMARY",
    "title": "Team Summary",
    "isPlaceholder": true,
    "source": "TEMPORARY_STATIC",
    "metrics": [
      { "code": "TEAM_MEMBERS", "label": "Team Members", "value": 8, "unit": null }
    ],
    "items": []
  }
}
```

## Widget catalogue

| Persona | Widgets |
| --- | --- |
| `TENANT_ADMIN` | `SUMMARY`, `EMPLOYEE_OVERVIEW`, `CURRENTLY_ON_LEAVE`, `LOCATIONS`, `BIRTHDAYS`, `STORAGE_STATUS`, `HIRING_PIPELINE`, `DEPARTMENT_HEADCOUNT`, `RECENT_ONBOARDING` |
| `PEOPLE_MANAGER` | `TEAM_SUMMARY`, `TEAM_ATTENDANCE`, `TEAM_LEAVE`, `TEAM_BIRTHDAYS`, `TEAM_ONBOARDING` |
| `WORKFORCE_USER` | `MY_ATTENDANCE`, `MY_LEAVE`, `MY_TASKS`, `MY_DOCUMENTS`, `ANNOUNCEMENTS` |
| `EXTERNAL_USER` | `CLIENT_SUMMARY`, `OPEN_TICKETS`, `RECENT_ACTIVITY`, `CLIENT_SITE_EMPLOYEES`, `CLIENT_DOCUMENTS` |

Tenant Administrator employee, department, birthday, onboarding, leave and location widgets read current tenant-scoped database data. Its storage and hiring widgets remain temporary. All three other persona catalogues currently return realistic temporary values as explicitly requested.

`isPlaceholder=true` and `source=TEMPORARY_STATIC` are mandatory UI signals. The UI may display a “Sample data” indicator and must not present these values as audited/live business data. Dynamic responses use `isPlaceholder=false` and `source=DATABASE`.

## Errors and isolation

- The server ignores any client attempt to choose a RoleType because no such request field exists.
- An unsupported RoleType is denied through the standard forbidden path.
- A widget code outside the authenticated persona catalogue returns the standard not-found error.
- All dynamic queries are tenant scoped. Existing opaque employee identifiers remain opaque.
- The endpoints use cancellable async calls and contain no `.Result` or `.Wait()` blocking calls.

There is no body, FormData, Excel/CSV import, polling, write, retry command or persistence table for these GET endpoints. Cancellation occurs when the HTTP request cancellation token is signalled.

## Compatibility and status

The nine `/api/Dashboard/SuperAdmin/*` endpoints remain available. UI migration to the generic routes can be gradual. Local API build and focused contract results are recorded in [the scenario report](../testing/dashboard/generic-persona-widgets/2026-10-04.md). Deployment and authenticated HTTP verification remain pending.
