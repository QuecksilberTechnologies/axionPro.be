# Refresh API V2 — opt-in migration record

> **CURRENT STATUS: WIP — extended testing found a reproducible concurrency defect (2026-09-13).**
> Normal persistence/rotation works, but simultaneous requests using one token both succeed for Host and Tenant. Two failing regression tests are retained. Do not treat the earlier 20-case pass as complete acceptance. Legacy API remains untouched.

## User-approved decision — 2026-09-13

**Keep the current refresh API unchanged. Build a NEW lightweight API on a NEW branch.**
Branch: `codex/lightweight-refresh-api`.

The UI developer will try the new API separately. Only after consistent behaviour is accepted will the user decide to remove the old API. No session should remove the legacy API, redirect its route, change its DTO, switch UI consumers, or claim migration COMPLETE based only on local tests.

Original instruction: “new api banao, current wale mat change karna, new wale api ko new branch mai ... jab consist ho jaegi ... old remove kar denge ... koi bhi chat session read kar lee”.

## Routes and input

- Existing: `POST /api/Auth/refresh-token` — existing full login response; unchanged.
- New opt-in: `POST /api/Auth/refresh-token-v2` — token-only response for both Host and Tenant Employee sessions.
- Login routes and the current Angular project are unchanged. No database migration or seed change.

Body (same request DTO as legacy):

```json
{
  "refreshToken": "<current opaque refresh token>",
  "ipAddress": "127.0.0.1"
}
```

`refreshToken` is required and must be nonblank. `ipAddress` is optional and follows the existing storage policy. ModuleId, OperationId and a valid access JWT are not required: refresh-token ownership authenticates this operation. Never put real tokens in documentation/logs.

Successful response, showing relevant envelope fields:

```json
{
  "isSucceeded": true,
  "message": "Token refreshed successfully.",
  "data": {
    "token": "<new access JWT>",
    "refreshToken": "<new opaque refresh token>",
    "tokenExpiry": "<actual issued JWT expiration in UTC>",
    "refreshTokenExpiresAtUtc": "<persisted replacement expiration in UTC>"
  },
  "errors": []
}
```

The existing `ApiResponse` wrapper can serialize its additional standard metadata. `data` has exactly four properties; no accessToken alias, nested success flag, profile, menus, role list, or Host permission list. Preserve the `token` name for current Angular compatibility.

## Validation and reduced work

Both branches validate the hashed token, expiry/revocation and mutually exclusive immutable owner foreign keys. Tenant flow rechecks current credential/login, active employee through the existing stored procedure, employee data needed for claims, subscription, active primary role and tenant encryption key. Host flow rechecks active/nondeleted Host user and role and matching login. Existing token service/JWT claims and signing policy are reused.

V2 removes refresh-time common-menu reads, operational menu/permission assembly, unused tenant-enabled-module results, profile-image lookup, repeated credential lookup, tenant-name lookup and unused role-detail lookup. Tenant repository reads before rotation fall from 15 to 7; Host reads from 4 to 3. These are handler-level calls, not a measured SQL statement count. Existing repository methods may perform multiple queries internally.

Protected APIs retain the established permission pipeline. Removing permission LISTS from refresh is not permission bypass. Current Angular `TokenRefreshService` consumes only `data.token` and `data.refreshToken`, and already coalesces concurrent refreshes inside one service instance.

## UI rollout and rollback

1. Deploy this branch to an agreed test environment; this document does not mean it is deployed.
2. On the UI developer's test branch, change the refresh URL to `/Auth/refresh-token-v2`.
3. Check `isSucceeded` and that both token strings are present before saving. Preserve current profile/menu state. Fetch changes through the existing profile/menu/permission flows.
4. Add the new route to existing interceptor exclusions wherever the old refresh route is excluded (auth retries and module/operation attachment/waiting). Otherwise an expired access token or menu-loading dependency can cause recursive refresh/deadlock.
5. Save BOTH replacement tokens, then retry waiting requests once. Keep the existing single-in-flight refresh mechanism. Do not call old and new routes simultaneously with the same token.
6. Verify Host/Tenant sessions, expired JWT refresh, permissions, logout, cold load and multiple waiting requests. Other clients must be inventoried before retirement.
7. For rollback switch URL to the old route and continue with the CURRENT replacement refresh token; the consumed old token stays revoked. Both APIs share the same token storage and signing policy.

Legacy failure semantics retained: blank input uses centralized validation (400), invalid/expired/revoked/owner failures use 401, missing primary role uses 404, employee/subscription business failures can return HTTP 200 with `isSucceeded=false`. Do not interpret HTTP 200 alone as success. Infra failures remain errors; do not retry indefinitely.

## Limits and acceptance gates

- Existing transaction/repository rotation writes are reused. This change does not claim to resolve simultaneous cross-tab/cross-client refresh races; the legacy read-then-write rotation does not provide an atomic single-winner claim. No shared persistence changes were made because legacy behaviour must remain untouched.
- Existing IP storage limits/policy are unchanged.
- No cache of subscription/role eligibility was introduced.
- Reduced data/query work is expected to help, but no production millisecond guarantee. Measure warm/cold latency and p50/p95 after deployment.
- Old API removal: **PENDING explicit UI acceptance + user approval**.
- UI migration, deployment and production latency measurement: **PENDING**.
- Local implementation/build/basic tests completed; **extended acceptance WIP** due to the concurrency failures below. This is not production or UI acceptance.

## Local validation evidence — 2026-09-13

- API build: PASS, 0 errors. Existing repository warnings remain.
- 14 V2 unit cases: PASS. Blank/invalid/expired/revoked tokens, invalid owner combinations, inactive/deleted/renamed Host users, inactive Host role, successful rotation and rollback on replacement insertion failure. Unexpected Host permission/presentation dependencies fail the test.
- 2 V2 real PostgreSQL cases: PASS, Tenant and Host. Actual DI/handler/token service; JWT expiry equals response expiry, replacement expiry equals DB, token stored hashed, owner FK/type retained, consumed token rejected, exactly four response data properties.
- 2 existing legacy PostgreSQL regression cases: PASS. Existing handler was not changed.
- 2 additional loopback HTTP cases: PASS, Tenant and Host. Actual AuthController, MediatR/application services, centralized exception middleware and PostgreSQL; V2-issued token works on old route, old-issued token works on V2, switching back to old works. Blank input yields 400, invalid token yields 401. These are isolated local HTTP tests, not Render tests.
- **Total: 20 distinct cases passed, 0 failed, 0 skipped**, across the initial 18-case run and the additional 2-case HTTP run. Passed cases were not broadly rerun.
- Test-generated token chains were deleted from the isolated local DB by fixture cleanup. No production DB changes.
- Existing handler, LoginResponseDTO and RefreshTokenRepository verified unchanged using Git diff.
- Logs in local ignored artifacts: `artifacts/refresh-v2-build.log`, `artifacts/refresh-v2-tests.log`, `artifacts/refresh-v2-http-tests.log`.
- Observed local single-request handler samples varied: Tenant 77 ms and 1035 ms; Host 20 ms and 82 ms across differently warmed test runs. These include different initialization conditions, are not a controlled benchmark or p95, and must not be presented as production latency guarantees.

## Continuing-session checklist

Read this file first. Preserve legacy. Check branch/status before editing. Record test results accurately, distinguish local from deployed evidence, and leave UI acceptance/old-route removal pending until confirmed.

## Extended tests requested by user — 2026-09-13

13 NEW cases executed; earlier 20 passing cases were not rerun. Results: **11 PASS, 2 FAIL, 0 skipped**. Across recorded runs: **31 passing / 2 failing distinct cases**.

- 9 Tenant failure cases PASS: missing credential, changed login, inactive login, missing employee, missing/expired subscription, missing subscription expiry, missing primary role, missing encryption key. No rotation or presentation queries occur after rejection; identity/tenant arguments are checked.
- 2 actual PostgreSQL rollback cases PASS (Host/Tenant): a 51-character IP causes `DbUpdateException` against the existing varchar(50) column during rotation. The transaction restores the original token to unrevoked with no replacement hash. This proves rollback, not acceptable oversized-input handling: centralized generic error handling would report 500; V2 input validation should address this in follow-up.
- 2 actual PostgreSQL concurrency cases FAIL (Host/Tenant): independent DbContexts/connections are synchronized after reading the same unrevoked token; both requests issue replacements. Expected one success, observed two. This is a real read-before-write race in the reused rotation policy, not a test infrastructure failure.
- Test names: `Tenant_failure_never_rotates_or_loads_menus`, `Database_write_failure_rolls_back_revocation`, `Concurrent_same_token_must_have_only_one_winner`.
- Log: `artifacts/refresh-v2-extended-tests.log`. Test fixture tokens, including both concurrent replacement branches, cleaned; read-only check found 0 remaining `v2-test-%` markers.
- Application/legacy code was not changed in this testing follow-up. Fix scope should remain V2-only unless the user separately authorizes changes to legacy rotation. Recommended next correction: an atomic DB conditional claim of an unrevoked/unexpired token within the replacement transaction; validate IP storage length before writes. Keep failing tests as regression gates.

### What is stored in DB?

Table: `axionpro."RefreshToken"` (EF property name is `RefreshTokens`). A successful rotation inserts the SHA-256 hash of the new opaque refresh token, owner type + immutable owner FK, LoginId, creation time, 7-day expiry and optional IP. The consumed row is retained, marked revoked, timestamped and linked to the replacement hash. The raw refresh token is returned to the client; it is not stored as plaintext. The access JWT is returned and is not stored by this refresh flow. Response refresh expiry equals the persisted expiry; access expiry equals the issued JWT expiration.

No production DB was touched or verified in these local tests. The request/response examples above use placeholders deliberately.
