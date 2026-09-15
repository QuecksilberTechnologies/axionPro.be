# Organization Holiday Calendar

## Purpose

`OrganizationHolidayCalendar` now assigns each holiday to one physical tenant
location. Country and state are resolved through `TenantLocation`; they are not
copied into the holiday row. The year is derived from `HolidayDate`.

## Current API

`GET /api/HolidayCalandar/get`

The endpoint uses the existing authentication and authorization behavior. It does
not currently accept `ModuleId`, `OperationId`, `TenantLocationId`, or year filters.
No create/update endpoint was added by this refactor.

Representative response item:

```json
{
  "tenantId": 9,
  "tenantLocationId": 5,
  "holidayName": "Republic Day",
  "holidayDate": "2027-01-26",
  "isOptional": false,
  "description": "India office holiday"
}
```

`holidayDate` is an ISO date (`YYYY-MM-DD`) without a time or timezone. The UI
must resolve the location from the tenant's location options and store/send its
numeric `tenantLocationId` when a write endpoint is introduced.

## Persistence

Data is stored in `axionpro."OrganizationHolidayCalendar"`. `TenantLocationId`
references `axionpro."TenantLocation"("Id")`. Country, state, district and city
come from that location relationship. The retained fields are:

- `TenantId` for tenant isolation
- `TenantLocationId` for the applicable office/location
- `HolidayName`, `HolidayDate`, `IsOptional`, and `Description`
- active, soft-delete, and audit fields

`CountryCode`, `StateCode`, `HolidayYear`, and duplicate `Remark` are removed by
the guarded migration. Existing rows require an explicit location mapping; the
migration intentionally stops instead of guessing when unmapped rows exist.

## Validation status

- Local build: PASS on 2026-09-16.
- Focused automated tests: 3 passed, 0 failed, 0 skipped.
- Target DB migration: NOT RUN. It must be coordinated with deployment because
  the old deployed build still maps the columns removed by the migration.
- Deployed API verification: NOT RUN.

See the [scenario report](../testing/holiday-calendar/tenant-location-refactor/2026-09-16.md).
