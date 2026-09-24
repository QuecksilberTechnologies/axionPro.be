# Scenario test reports

- [Attendance punch flow — 2026-09-24](attendance/punch-flow/2026-09-24.md)
- [Subscription billing foundation — 2026-09-24](billing/foundation/2026-09-24.md)
- [Employee bulk invitation email smoke — 2026-09-24](bulk/employee-invitation-email/2026-09-24.md)
- [All durable bulk targets — live smoke test — 2026-09-19](bulk/all-live-smoke/2026-09-19.md)
- [Tenant Policy interactive HTML simulator — 2026-09-18](policy/interactive-html-simulator/2026-09-18.md)

Every tested scenario has a report grouped by module, scenario and test date.
Start with [the report template](SCENARIO_TEMPLATE.md). This is the continuing
test-evidence index; API integration instructions remain in
[UIdeveloperDoc](../UIdeveloperDoc/README.md).

```text
docs/
  testing/
    README.md
    SCENARIO_TEMPLATE.md
    employee/
      country-identity/
        2026-09-13.md
```

Add another module/scenario directory when that scenario is actually worked on.
Append a new run section for another run on the same date; use a new dated file
for a later run. Keep earlier failures and link the run that supersedes them.

## Reports

| Module / scenario | Report | Current recorded result |
| --- | --- | --- |
| Database / guarded legacy cleanup | [2026-09-16](database/legacy-cleanup/2026-09-16.md) | Verified backup; 27 legacy tables removed across two guarded phases; location smoke checks pass; accommodation table safely renamed with a temporary compatibility view. |
| Database / manual table removal reconciliation | [2026-09-16](database/manual-table-removal-reconciliation/2026-09-16.md) | Live inventory 128; 22 absent EF mappings quarantined; model mismatch zero, build and API startup pass. |
| Holiday calendar / tenant-location refactor | [2026-09-16](holiday-calendar/tenant-location-refactor/2026-09-16.md) | Local contract/build suite 3/3 passed; coordinated target migration and deployed API acceptance pending. |
| Holiday calendar / target DB migration | [2026-09-22](holiday-calendar/tenant-location-refactor/2026-09-22.md) | Development-configured target table backed up and migrated; 15-column schema/FK verified, focused suite 3/3 passed; deployed API acceptance pending. |
| Holiday calendar / Tenant Policies child module | [2026-09-22](holiday-calendar/policy-child-module/2026-09-22.md) | Child Id 118 under parent Id 108, View/Add/Update/Delete mappings and 10 plan entitlements verified; holiday write APIs remain pending. |
| Holiday calendar / CRUD implementation | [2026-09-22](holiday-calendar/crud/2026-09-22.md) | Local API build, 11 focused tests and unauthenticated HTTP 401 checks on all 5 routes passed; authenticated CRUD and deployed verification pending. |
| Holiday calendar / import-export | [2026-09-22](holiday-calendar/import-export/2026-09-22.md) | Local build and 14 focused tests passed; target DB seed rerun yielded one Import and one Export mapping. Authenticated HTTP, isolated full-seed fixture and deployed checks pending. |
| Holiday calendar / 2026 Jabalpur data | [2026-09-22](holiday-calendar/live-2026-jabalpur/2026-09-22.md) | Tenant 8 location 5: 31 sourced rows across all 12 months, repeat insert 0, local unauthenticated API smoke and 14 focused tests passed; company approval and authenticated HTTP pending. |
| Holiday calendar / unique date | [2026-09-22](holiday-calendar/unique-date/2026-09-22.md) | Non-soft-deleted same-date rows, including inactive, now block create/import; partial unique index and rollback checks passed. Authenticated HTTP pending. |
| Holiday calendar / display constants | [2026-09-24](holiday-calendar/calendar-display-constants/2026-09-24.md) | Token-only color/status/priority API added; 4 Holiday contract tests and unauthenticated HTTP 401 passed; authenticated and deployed verification pending. |
| Location / City-to-Locality refactor | [2026-09-16](location/locality-refactor/2026-09-16.md) | Four-country postal seed applied (222,683 postal localities); 10/10 focused tests passed; updated deployed API acceptance pending. |
| Policy / generic framework schema | [2026-09-16](policy/framework-schema/2026-09-16.md) | Generic persistence and scope-1 policy module hierarchy applied to target DB; endpoint implementation remains a separate phase. |
| Policy / API foundation | [2026-09-16](policy/api-foundation/2026-09-16.md) | Local EF/API/permission/lifecycle contract implemented; focused result and deployed acceptance are recorded in the report. |
| Policy / target DB migration | [2026-09-16](policy/target-db-migration/2026-09-16.md) | Target prerequisites, 11 policy tables, module hierarchy, 47 mappings and bulk master 1–14 verified; 29 focused tests passed. |
| Policy / durable bulk import | [2026-09-16](policy/bulk-import/2026-09-16.md) | Authenticated Policy Type preview/confirm/worker and DB reconciliation pass; definitions, assignments, retry and cancellation remain pending. |
| Policy / deployed real-data business flow | [2026-09-16](policy/live-business-flow/2026-09-16.md) | Type CRUD, draft/version lifecycle, ordered approval, publish, assignment, resolution, exception, acknowledgement, audit, clone and assignment bulk reconciled; document upload blocked by invalid Render S3 credentials. |
| Policy / deployed route smoke | [2026-09-16](policy/deployed-route-smoke/2026-09-16.md) | Live commit and 37 deployed operations verified; authenticated business-flow acceptance needs tenant credentials. |
| Employee / China and USA identity options | [2026-09-13](employee/country-identity/2026-09-13.md) | China created; identity GET failed with 500 before fix. USA creation blocked by missing country option. Local suite 18 passed, 5 skipped; post-fix live acceptance pending. |
| Employee / get-all assigned roles | [2026-09-13](employee/get-all-assigned-roles/2026-09-13.md) | Response contract/build pass locally; isolated DB and deployed response verification pending. |
| Employee / Reset Password module consolidation | [2026-09-23](employee/reset-password-module-consolidation/2026-09-23.md) | Module 38 and all checked dependencies removed from the Development DB; Operation 21, tenant entitlement and role grant moved to `EMP_LIST`; focused suite 3/3 passed. |
| Employee Work Arrangement / Attendance policy options | [2026-09-23](employee-work-arrangement/attendance-policy-options/2026-09-23.md) | Token-only effective-date dropdown endpoint added; build, 4 new tests, 23 relevant regressions and local unauthenticated HTTP 401 passed; authenticated data and deployed verification pending. |
| Employee Work Arrangement / Attendance policy permission regression | [2026-09-24](employee-work-arrangement/attendance-policy-options/2026-09-24.md) | Diagnosed deployed `FORBIDDEN` as a global Employee permission-pipeline interception; exact token-only query exception added and 6 focused tests passed locally; corrected deployment verification pending. |
| Employee Work Arrangement / PolicyVersion persistence | [2026-09-24](employee-work-arrangement/policy-version-persistence/2026-09-24.md) | Dropdown-to-save contract migrated to exact generic `PolicyVersionId`; backend 9/9, form schema 9/9, Angular stores 18/18 and production build passed; configured target DB migration verified; deployed authenticated acceptance pending. |
| Employee Work Arrangement / location consistency | [2026-09-24](employee-work-arrangement/location-consistency/2026-09-24.md) | Primary-assignment coverage, work-mode/location-type and date-window overlap validation implemented; focused tests and migration/API acceptance recorded in the report. |
| Bulk / existing-module operation cleanup | [2026-09-13](bulk/module-operation-cleanup/2026-09-13.md) | Target DB cleanup complete: zero bulk modules/orphan entitlements; 22 functional Import/Export mappings. Isolated seed rerun and 36 focused tests passed. |
| Module / seed metadata and operation cleanup | [2026-09-14](module/module-operation-seed/2026-09-14.md) | Dashboard seeds removed; source contract covers metadata completion and Add/Create cleanup. Isolated PostgreSQL verification remains blocked because the local fixture is unavailable. |
| Module / duplicate Export operation cleanup | [2026-09-23](module/duplicate-export-operation-cleanup/2026-09-23.md) | Duplicate Export Id 14 and all checked dependencies removed; canonical Id 23 retained and permissions migrated; focused suite 2/2 passed. |
| Module / Tenant Attendance Policies removal | [2026-09-23](module/tenant-attendance-policies-removal/2026-09-23.md) | Module Id 67, four mappings, tenant entitlements, plan mapping and role grants removed from Development DB; production seed definition removed; focused suite 2/2 passed. |
| Tenant entitlements / Tenant Admin permission sync | [2026-09-15](tenant-entitlements/admin-permission-sync/2026-09-15.md) | Local build and command wiring pass; disposable PostgreSQL behavior test skipped because its required environment is unavailable; deployed acceptance pending. |
| Tenant Email Template / CRUD and seed | [2026-09-24](tenant-email-template/crud-and-seed/2026-09-24.md) | Tenant table/data, module child, CRUD permission flow, four mappings, plan and enabled-tenant entries implemented; configured target seed verified; authenticated HTTP pending. |
| Module / singular master parent hierarchy | [2026-09-15](module/singular-master-parent-hierarchy/2026-09-15.md) | Final focused canonical leaf, EmployeeType, permission, and source-contract suite: 51 passed, 4 DB-fixture skips; isolated PostgreSQL execution and deployed menu verification remain pending. |

## Existing evidence kept at its original location

- [Render smoke checks, 2026-09-13](../RENDER_SMOKE_TEST_2026-09-13.md)
- [Automation test commands and historical results](../../axionpro.automationtests/README.md)

These links preserve previous evidence; they do not claim a new test run.

## Status rules

- **PASS:** the stated assertion was executed and matched the expected result.
- **FAIL:** the assertion was executed and did not match.
- **BLOCKED:** a required prerequisite prevents the scenario from being executed.
- **SKIPPED:** the test runner explicitly skipped a test; include its reason.
- **NOT RUN / PENDING:** planned verification has not been executed.

Label each result with its environment. A local PASS does not establish deployed
acceptance. Do not reconstruct a missing payload or raw response as if it was
captured. Mark illustrative examples and code/seed expectations explicitly.

Record exact test names, counts, command, commit/build when known, and links to
sanitized logs or reports. Redact secrets and sensitive personal data before
saving evidence. Documentation-only work does not require repeating passed tests.
## Latest holiday report

- [Holiday rename and Icon field — 2026-09-23](holiday-calendar/rename-to-holiday/2026-09-23.md)
 - [Tenant email queue runtime resolution — 2026-09-24](tenant-email-template/queue-runtime-resolution/2026-09-24.md)
