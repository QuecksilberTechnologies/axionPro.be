# Scenario test reports

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
| Employee / China and USA identity options | [2026-09-13](employee/country-identity/2026-09-13.md) | China created; identity GET failed with 500 before fix. USA creation blocked by missing country option. Local suite 18 passed, 5 skipped; post-fix live acceptance pending. |
| Employee / get-all assigned roles | [2026-09-13](employee/get-all-assigned-roles/2026-09-13.md) | Response contract/build pass locally; isolated DB and deployed response verification pending. |
| Bulk / existing-module operation cleanup | [2026-09-13](bulk/module-operation-cleanup/2026-09-13.md) | Target DB cleanup complete: zero bulk modules/orphan entitlements; 22 functional Import/Export mappings. Isolated seed rerun and 36 focused tests passed. |
| Module / seed metadata and operation cleanup | [2026-09-14](module/module-operation-seed/2026-09-14.md) | Dashboard seeds removed; source contract covers metadata completion and Add/Create cleanup. Isolated PostgreSQL verification remains blocked because the local fixture is unavailable. |
| Tenant entitlements / Tenant Admin permission sync | [2026-09-15](tenant-entitlements/admin-permission-sync/2026-09-15.md) | Local build and command wiring pass; disposable PostgreSQL behavior test skipped because its required environment is unavailable; deployed acceptance pending. |
| Module / singular master parent hierarchy | [2026-09-15](module/singular-master-parent-hierarchy/2026-09-15.md) | Source-contract tests pass; disposable PostgreSQL execution and deployed menu verification remain pending. |

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
