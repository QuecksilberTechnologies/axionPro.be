# Tenant Device permission contract

## Why the permission error repeats

`TENANT_DEVICES` models assignment of a catalogue device to a Tenant. Its action
names are therefore `Assign` and `Remove`, rather than the generic CRUD names
`Add` and `Delete`. A UI lookup for `Add` or `Delete` returns no OperationId. The
subsequent request is rejected or cannot be constructed, even though the Host role
has the correct Tenant Device grants.

Do not cache numeric IDs or infer an action from a CRUD verb. Resolve the module
and its operations from `GET /api/Navigation/my-menu` after login and after a role
or seed change.

## Exact dynamic mapping

Find the module whose `moduleCode` is `TENANT_DEVICES`, then resolve the operation
by the following name:

| UI action | API | Operation name |
|---|---|---|
| Open/list/detail | `GET /api/TenantDevice/get-all`, `get-by-id` | `View` |
| Assign catalogue device | `POST /api/TenantDevice/create` | `Assign` |
| Edit or move assignment | `POST /api/TenantDevice/update`, `update-location` | `Update` |
| Activate | `POST /api/TenantDevice/update-status` | `Active` |
| Deactivate | `POST /api/TenantDevice/update-status` | `Inactive` |
| Remove assignment | `DELETE /api/TenantDevice/delete/{id}` | `Remove` |

Numeric values currently observed on the development server are module `35`,
View `4`, Assign `11`, Update `2`, Active `8`, Inactive `9`, and Remove `12`.
These are diagnostic examples only. The UI must use the IDs returned by `my-menu`.

## Screen-load sequence

Each request uses its own module and `View` operation from `my-menu`:

1. Tenant assignments: module code `TENANT_DEVICES` →
   `GET /api/TenantDevice/get-all?pageNumber=1&pageSize=10&moduleId=<id>&operationId=<viewId>`.
2. Tenant filter: `HOST_TENANT_LIST` → `GET /api/Tenant/get-all-tenants`.
3. Location filter after Tenant selection: `HOST_TENANT_LOCATION_LIST` →
   `GET /api/TenantLocation/get-all?tenantId=<opaqueTenantId>`.
4. Device model filter: `HOST_DEVICE_SETUP` → `GET /api/DeviceMaster/get-all`.

Do not make the page list fail because an optional filter lookup fails. Display the
specific lookup error and keep the successfully loaded assignment list visible.

## Production verification (2026-09-13)

Authenticated development-server calls returned HTTP 200 for all four screen
dependencies: TenantDevice list, Tenant list, Tenant locations, and Device
catalogue. TenantDevice returned zero records, so the empty-state table was the
correct data result. The existing Host live automation suite passed 3/3 tests; its
menu-driven coverage now also includes the Tenant Location lookup.
