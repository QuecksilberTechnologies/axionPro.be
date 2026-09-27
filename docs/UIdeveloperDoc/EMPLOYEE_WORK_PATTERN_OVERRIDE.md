# Employee Work Pattern and Work Mode Override UI handoff

## Behavior

`EmployeeWorkPattern` defines one weekday row inside an existing work arrangement. Its effective
dates are the parent arrangement dates. A working day may use Office, Work From Home, Field or
Client Site. Hybrid is represented by different concrete modes across weekday rows. The API rejects
duplicate weekdays in one arrangement, invalid location types, uncovered employee location
assignments, inactive dependencies and attendance-policy mismatches.

The employee is selected through the parent `EmployeeWorkArrangementId`; create/update requests do
not repeat an employee ID. Every work-pattern read or mutation response includes `employeeId` as the
existing tenant-salted string and `employeeName` for display. Never expose or derive the database
employee `bigint` in the UI. A representative row is:

```json
{
  "id": 17,
  "employeeId": "XGQRD0Z8",
  "employeeName": "Example Employee",
  "employeeWorkArrangementId": 9,
  "dayOfWeek": 0,
  "dayOfWeekName": "Sunday",
  "workMode": 4,
  "workModeName": "ClientSite",
  "tenantLocationId": 12,
  "tenantLocationName": "Client Location",
  "isWorkingDay": true,
  "isActive": true
}
```

The weekday is recurring and has no independent date fields. Its applicable period is the parent
work arrangement's `effectiveFrom` / `effectiveTo`. A working day also carries its concrete work
mode and any required location; a weekly off sets `isWorkingDay` to `false` and sends no location.

`EmployeeWorkModeOverride` is a temporary exception to the base arrangement/pattern. It may overlap
the base arrangement by design. Two active Pending or Approved overrides for the same employee may
not overlap, including a shared boundary date. Rejected, Cancelled, inactive and soft-deleted rows
do not participate. Approval repeats all dependency and overlap validation.

## Permissions

Resolve `ModuleId` and `OperationId` dynamically from the authenticated menu/permission flow for
module code `EMP_OVERRIDES`. Never hard-code the illustrative operation IDs.

- `POST /api/EmployeeWorkModeOverride/approve` — operation `Approve`
- `POST /api/EmployeeWorkModeOverride/reject` — operation `Reject`

## Decision request examples

```json
{
  "id": 42,
  "remark": "Client visit approved",
  "moduleId": 0,
  "operationId": 0
}
```

Rejection requires a non-empty `remark` of at most 500 characters. Approval remark is optional.
The response uses the standard mutation envelope and returns the updated override. Employee IDs in
responses and filters are the existing salt-encoded strings; database IDs remain `bigint`.

## Persistence and deployment

Rows persist in `axionpro."EmployeeWorkPattern"` and
`axionpro."EmployeeWorkModeOverrideRequest"`. Run
`database-scripts/CompleteEmployeeWorkPatternAndOverrideValidation.sql` after resolving any
conflicting active Pending/Approved rows. The script adds concurrent-write protection and does not
alter conflicting business data.

Local API and Angular builds and focused automated tests are recorded in the linked scenario
reports. Deployment and authenticated deployed-environment acceptance are separate and pending.
