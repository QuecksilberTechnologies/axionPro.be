# DB and API cleanup reference

Last updated: 2026-09-16 (Asia/Calcutta)

## Safety baseline

- API source backup is retained by the user.
- Full Render PostgreSQL backup: `C:\AxionProCodeBase\DBFullBACKUP\workforcedb_full_20260916_035225.dump`.
- Restore catalogue: `C:\AxionProCodeBase\DBFullBACKUP\workforcedb_full_20260916_035225.list.txt`.
- SHA-256 record: `C:\AxionProCodeBase\DBFullBACKUP\workforcedb_full_20260916_035225.sha256.txt`.
- `pg_restore --list` verification passed before database cleanup.

## Completed cleanup

The guarded transaction in `database-scripts/CleanupUnusedLegacyTables.sql`
was executed against the Render database. These zero-row, unreferenced tables
were removed:

| Removed table | Reason |
| --- | --- |
| `DistrictMaster` | Duplicate legacy structure; active location API uses `District`. |
| `HolidayMaster` | Unused legacy structure; active holiday API uses `Holiday`. |
| `NoImagePath` | Empty and absent from runtime behavior. |
| `demo` | Empty database-only test table. |
| `dummy1` | Empty database-only test table. |

Obsolete EF entities, mappings, schema-snapshot definitions, and the tenant
cleanup reference were also removed. The migration aborts if a candidate has
data or a dependency.

### Phase 2 closed legacy graph

`database-scripts/CleanupUnusedApiPersistencePhase2.sql` removed another 22
tables after checking API/application/persistence source, DbSet usage, live
foreign keys, views, triggers, and PostgreSQL functions/procedures:

- Workflow: `ApprovalWorkflow`, `WorkflowStep`.
- Asset legacy history: `AssetHistory`, `AssetTicketTypeDetail`.
- Attendance legacy history: `AttendanceHistory`, `AttendanceLogs`.
- Candidate/demo/email logs: `CandidateHistory`, `DemoRequest`,
  `DemoRequestBiometricDetail`, `EmailsLog`.
- Legacy interview graph: `InterviewFeedback`, `InterviewPanel`,
  `InterviewPanelMember`, `InterviewSchedule`, `InterviewSdule`.
- Other unused logs: `LeaveTransactionLog`.
- Legacy tender graph: `TenderProject`, `TenderService`,
  `TenderServiceHistory`, `TenderServiceProvider`,
  `TenderServiceSpecification`, `TenderServiceType`.

Twenty tables were empty. The two `DemoRequest` rows and sixteen
`TenderServiceType` rows are retained in the full backup and in dedicated CSV
exports under `C:\AxionProCodeBase\DBFullBACKUP`.

## Active structures retained

- `Country`, `State`, `District`, `City`.
- `TenantLocation`, `EmployeeLocationAssignment`.
- `AttendancePolicy`, `EmployeeWorkArrangement`, `EmployeeWorkPattern`.
- `PolicyType`, `PolicyTypeDocument`, policy mappings, sandwich rules,
  compliance rules, and `Holiday`.
- `InterviewSchedule` and `InterviewSdule`, because they have different shapes
  and domain relationships and are not proven duplicates.

`District` remains the source for `/api/Location/District/option`. `City` is
referenced by tenant and employee addresses. They require a compatibility
migration before consolidation.

## Naming correction prepared

Code and schema references now use
`AccommodationAllowancePolicyByDesignation` instead of the misspelled
`AccoumndationAllowancePolicyByDesignation`. The data-preserving migration is
`database-scripts/RenameAccommodationAllowancePolicyTable.sql`.

Status: **production table rename and code/tests complete; corrected API deploy
pending**. A temporary, automatically updatable view with the legacy name keeps
the currently deployed API compatible. After the corrected build is deployed
and verified, run `database-scripts/FinalizeAccommodationAllowancePolicyRename.sql`
to remove that compatibility view.

## Policy-engine preparation

The detailed prerequisite audit is in
`docs/POLICY_ENGINE_PREREQUISITE_AUDIT.md`. Existing active structures cannot
be destructively reshaped while preserving API behavior. Safe order:

1. Add versioned policy and jurisdiction structures alongside current tables.
2. Backfill and reconcile existing records.
3. switch repositories with compatibility reads.
4. Remove superseded structures only after deployed verification.

No endpoint was removed based only on an empty table. API source inspection
cannot prove that an external client never calls an endpoint; telemetry or an
explicit retirement list is required.

## Verification

- Solution build passed with 0 errors; existing warnings remain.
- Focused automation: 6 passed, 0 failed, 0 skipped.
- Phase-2 cleanup automation: 2 passed, 0 failed, 0 skipped.
- Render DB post-check: all five removed tables resolve to `NULL`.
- Phase-2 Render DB post-check: all 22 retired tables resolve to `NULL`.
- Accommodation persistence: corrected object is a table, legacy name is a
  zero-row compatibility view; both return the same row count.
- Deployed lookup smoke test after cleanup: Country HTTP 200/2 rows, India
  State 36 rows, selected State District 4 rows.
