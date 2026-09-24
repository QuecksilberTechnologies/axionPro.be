# Employee Work Location and Work Arrangement validation

## Purpose

Employee Work Location records define where an employee is allowed to mark attendance. Employee Work Arrangement records define the employee's effective work mode, Published Attendance policy version, and authoritative primary location. Create, update, status, and delete flows now protect the relationship between these records.

## Typed Attendance policy configuration

Generic Attendance policy create/update requests now include `attendanceConfiguration`. The same object is returned in policy detail and is copied when a version is cloned. It is mandatory only when the selected Policy Type belongs to the stable `ATTENDANCE` category.

```json
{
  "attendanceConfiguration": {
    "attendanceLocationScope": 2,
    "allowBiometric": false,
    "allowMobile": true,
    "allowWeb": true,
    "allowManualAttendance": false,
    "allowWorkFromHome": true,
    "requireGeoFenceForOffice": true,
    "requireGpsForRemote": true,
    "allowOutsideLocationWithApproval": false
  }
}
```

Location scope values are `1 PrimaryLocationOnly`, `2 AssignedLocations`, `3 AnyTenantLocation`, and `4 RemoteAnywhere`. At least one attendance channel must be enabled. Physical arrangements cannot use `RemoteAnywhere`; Work From Home requires `allowWorkFromHome=true` and no physical primary location.

`EmployeeDeviceEnrollment` remains a biometric-device registration record. Mobile or web attendance does not require a biometric device allotment. The production self-service flow is documented in `ATTENDANCE_PUNCH_API.md`; a successful punch response confirms persistence in the immutable attendance ledger.

## Authentication and permission

- Bearer token is required.
- Existing module/operation permission pipeline remains unchanged.
- Resolve current `ModuleId` and `OperationId` from the authenticated menu/permission flow. Do not hard-code numeric IDs from examples.
- Tenant and employee access remain server-resolved and tenant-isolated.

## Existing routes affected

- `POST /api/EmployeeLocationAssignment/create`
- `POST /api/EmployeeLocationAssignment/update`
- `POST /api/EmployeeLocationAssignment/update-status`
- `DELETE /api/EmployeeLocationAssignment/delete/{id}`
- `POST /api/EmployeeWorkArrangement/create`
- `POST /api/EmployeeWorkArrangement/update`
- `POST /api/EmployeeWorkArrangement/update-status`

No new endpoint, polling, retry, cancellation, file, Excel, CSV, or FormData contract was introduced.

## Validation behavior

### Effective windows

Dates are inclusive. Two active records conflict when their windows share even one date. `effectiveTo: null` means open-ended. Non-overlapping historical and future records are allowed.

Example: `2026-01-01..2026-01-31` and `2026-01-31..2026-02-28` overlap. The second period must start on `2026-02-01` to avoid a conflict.

### Primary-location relationship

When a Work Arrangement supplies `primaryTenantLocationId`, the employee must already have an Employee Location Assignment for exactly that location which:

- is active and not soft deleted;
- is marked `isPrimary: true`;
- has `isAttendanceAllowed: true`;
- starts on or before the arrangement;
- covers the arrangement's complete end date, including open-ended arrangements.

The API blocks shortening, moving, deactivating, or deleting that assignment while a live arrangement depends on it. Update the arrangement first.

### Work mode and location type

| Work mode | Primary-location rule |
| --- | --- |
| Office | Required; Head Office, Branch, Office, Plant, Warehouse, Campus, or Remote Office |
| Hybrid | Required; same physical office types as Office |
| Client Site | Required; Client Site only |
| Field | Optional; when supplied, Client Site or Project Site |
| Work From Home | Physical primary location is not allowed |

The location dropdown should show both name and type, for example `PANIPAT-REFIN (Client Site)`.

## UI submit order

1. Create or update the employee's primary Location Assignment first.
2. Ensure its effective range covers the intended arrangement.
3. Create or update the Work Arrangement with the same location ID.
4. When changing primary location, change the Work Arrangement away from the old location before deactivating/deleting the old assignment.

Illustrative location assignment:

```json
{
  "moduleId": 0,
  "operationId": 0,
  "employeeId": "<encoded-employee-id>",
  "tenantLocationId": 21,
  "isPrimary": true,
  "isAttendanceAllowed": true,
  "effectiveFrom": "2026-10-01",
  "effectiveTo": null,
  "isActive": true
}
```

Illustrative Client Site arrangement:

```json
{
  "moduleId": 0,
  "operationId": 0,
  "employeeId": "<encoded-employee-id>",
  "policyVersionId": 318,
  "primaryTenantLocationId": 21,
  "workMode": 5,
  "hybridType": null,
  "minimumOfficeDaysPerWeek": null,
  "minimumOfficeDaysPerMonth": null,
  "maximumWFHDaysPerMonth": null,
  "effectiveFrom": "2026-10-01",
  "effectiveTo": null,
  "isActive": true
}
```

## Representative validation errors

```json
{
  "isSucceeded": false,
  "message": "The selected primary location must have an active, primary, attendance-allowed employee location assignment covering the full arrangement period.",
  "data": null,
  "errors": [],
  "errorCode": "VALIDATION_ERROR"
}
```

```json
{
  "isSucceeded": false,
  "message": "The selected primary location type is not valid for the selected work mode.",
  "data": null,
  "errors": [],
  "errorCode": "VALIDATION_ERROR"
}
```

```json
{
  "isSucceeded": false,
  "message": "This location assignment is the primary location of an active work arrangement. Update the work arrangement first.",
  "data": null,
  "errors": [],
  "errorCode": "CONFLICT"
}
```

## Persistence and deployment

Tables are `EmployeeLocationAssignment`, `EmployeeWorkArrangement`, `TenantLocation`, `PolicyVersion`, and the one-to-one `AttendancePolicyVersionConfiguration`. Apply `database-scripts/AlignEmployeeWorkLocationAndArrangementValidation.sql` and `database-scripts/AddAttendancePolicyVersionConfiguration.sql`. The second script backfills existing Attendance versions once with compatibility settings and does not overwrite an existing typed configuration.

## Attendance-policy scope status

The selected generic `PolicyVersion` currently has no typed `AttendanceLocationScope`. The older separate `AttendancePolicy` table contains that field, while generic Policy rules only have free-form JSON such as `locationValidationRequired`. No JSON key was invented in this change. A stable generic-policy scope contract must be selected before this final cross-check can be implemented safely.

## Verification status — 2026-09-24

- Solution build: PASS with existing warnings, 0 errors.
- Focused automated tests: PASS, 27 passed, 0 failed, 0 skipped.
- Migration execution against a database: not run.
- Authenticated API and deployed verification: not run.
