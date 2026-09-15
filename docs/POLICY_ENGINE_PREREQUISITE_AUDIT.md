# Generic multi-country policy engine prerequisite audit

Audit date: 2026-09-16  
Status: preparation only; no schema or runtime behavior changed.

## Confirmed reusable foundation

- `Country`, `State`, `District` and `City` reference catalogues exist.
- `TenantLocation` already stores Tenant, country, optional state/city, address,
  coordinates, geofence, timezone and attendance/device flags.
- `EmployeeLocationAssignment` provides effective-dated employee/location mapping.
- `AttendancePolicy`, `EmployeeWorkArrangement`, `EmployeeWorkPattern` and
  `EmployeeWorkModeOverrideRequest` already support office, remote, hybrid,
  field and client-site attendance scenarios.
- `PolicyType`, `PolicyTypeDocument` and
  `UnStructuredPolicyTypeMappingWithEmployeeType` provide a partial tenant policy,
  document and EmployeeType mapping foundation.
- `LeaveSandwichRule` and `LeaveSandwichRuleMapping` provide the current leave-rule
  integration point.
- `ComplianceRule.RuleJson` is PostgreSQL `jsonb` and already supports country,
  optional state, tenant, priority and effective dates.
- `IFileStorageService` is implemented with private AWS S3 objects and temporary
  pre-signed read URLs. Policy documents should reuse it.

## Live database facts

- Countries: 2 (`IN`, `CN`); USA and other countries are not loaded.
- States: 70; districts: 8,333; cities: 8,333.
- District and City currently contain parallel/synchronized catalogues. A single
  authoritative hierarchy must be selected before policy targeting is added.
- Tenant locations: 2; employee location assignments: 4.
- Policy types: 4; EmployeeType-policy mappings: 5.
- Attendance policies, work arrangements, compliance rules, sandwich rules,
  holiday calendars and policy documents currently have zero live rows.

## Blocking schema gaps

1. `Country.CountryCode` is nullable and has no confirmed unique constraint.
2. `State` has no stable state/province code; its uniqueness is name-based within
   a country.
3. `District` has code/pin fields but `City` has only name/state.
4. `TenantLocation.StateId` exists but no State foreign key was found. It has no
   DistrictId and no LegalEntityId.
5. There is no Legal Entity/employing-entity master for one Tenant operating in
   multiple countries.
6. There is no effective-dated employee employment-jurisdiction record separating
   legal jurisdiction from current/temporary physical work location.
7. Existing `PolicyType` represents a tenant policy instance and type together;
   it has no stable policy code, version, priority, default, publish status or
   effective-to date.
8. Current EmployeeType mapping has StartDate only; it has no end date,
   include/exclude mode, priority or overlap constraint.
9. Current policy document belongs to PolicyType, not an immutable policy version;
   it lacks content type, size, checksum, primary-document flag and document version.
10. Current holiday rows use country/state strings and individual dates. There is
    no calendar header, version/status or work-location mapping.
11. Sandwich rules are tenant-level and leave-rule mapped; EmployeeType, location,
    jurisdiction and effective-date targeting are absent.
12. Policy evaluation/assignment history and decision audit do not exist.

## Required preparation sequence

### Phase 1 — normalize geography

- Decide whether District or City is the authoritative third-level area.
- Add stable country/state/area codes and uniqueness rules.
- Expand the country catalogue through controlled seeds; do not invent legal data.
- Add missing TenantLocation jurisdiction foreign keys and validate
  Country → State → District/City consistency.

### Phase 2 — employment structure

- Add Tenant LegalEntity and LegalEntityWorkLocation mapping.
- Add effective-dated EmployeeEmploymentContext with LegalEntity,
  employment jurisdiction, EmployeeType, primary work location and work mode.
- Preserve the distinction between employment jurisdiction, scheduled work
  location and travel destination.

### Phase 3 — policy lifecycle

- Separate stable PolicyType definitions from tenant Policy instances.
- Add PolicyVersion with validated `jsonb` configuration and immutable publishing.
- Add typed target mappings for legal entity, jurisdiction, work location,
  EmployeeType, department, work arrangement and employee.
- Add effective-dated employee assignments and temporary exceptions.

### Phase 4 — calendars and legal rules

- Replace individual ungrouped holidays with HolidayCalendar,
  HolidayCalendarDate and HolidayCalendarWorkLocation.
- Extend the existing ComplianceRule foundation into reviewed regulatory rule
  packs with source documents and effective versions.

### Phase 5 — documents and audit

- Reuse S3 storage. Store only the private object key and metadata in PostgreSQL.
- Suggested key:
  `tenants/{tenantId}/policies/{policyId}/versions/{versionId}/documents/{documentId}/{safeFileName}`.
- Add PolicyEvaluationLog and persist PolicyId/PolicyVersionId on resulting
  attendance, leave, travel, accommodation or insurance transactions.

## PostgreSQL datatype decision

- Use `jsonb` for validated module-specific configuration, conditions/actions and
  evaluation snapshots.
- Use relational foreign keys for Tenant, LegalEntity, country/state/area,
  location, EmployeeType, department and employee targets.
- Use `date` for business effective periods and `timestamptz` for audit events.
- Do not store target IDs or uploaded document binaries in policy JSON.

## Decisions required before migrations

1. Choose District or City as the authoritative location level, or define both
   with an explicit District → City relationship.
2. Confirm whether LegalEntity is required in the first release; it is strongly
   recommended for the stated India/USA scenario.
3. Confirm that published policies are immutable and changes create new versions.
4. Confirm targeting match semantics: all selected dimensions must match, with
   explicit exclusions applied first.
5. Confirm conflict behavior: block publishing when equal-priority policies overlap.
6. Confirm that regulatory packs are reviewed/admin-maintained data and are never
   automatically inferred as legal advice.

