# Render development API smoke test — 2026-09-13

Target: `https://axionpro-api.onrender.com`

## Passed live checks

- Swagger UI: HTTP 200.
- Tenant Admin login: PASS.
- Authenticated `GET /api/Navigation/my-menu`: PASS.
- `POST /api/Auth/refresh-token-v2`: HTTP 200; response data contained exactly `token`, `refreshToken`, `tokenExpiry`, and `refreshTokenExpiresAtUtc`.
- Anonymous `POST /api/Employee/update-bulk`: HTTP 401.
- Authenticated `POST /api/Employee/update-bulk` with invalid permission IDs: HTTP 403. This confirms the deployed authentication-versus-permission distinction.

Real access and refresh tokens are deliberately excluded from this document.

## Deployment-version blocker

The live OpenAPI document was downloaded twice after the user reported deployment. It contains `/api/Auth/refresh-token-v2`, but does not contain either `canUpdateVerificationStatus` or `reactivatedCount`. These properties are present in remote `main_branch` commits `81f4fcf5` and `8dfcaea2` respectively.

Therefore the live service is not yet serving the latest `8dfcaea2` artifact. Employee section metadata and inactive-master reactivation cannot be honestly marked live-tested until Render deploys that exact commit. No destructive Role/Department/Designation/EmployeeType mutation was attempted against the older live contract.

## Required final checks after exact commit deployment

1. Confirm OpenAPI contains `canUpdateVerificationStatus` and `reactivatedCount`.
2. Read an Employee profile and verify editable sections have `tabInfoType`; assignment summary sections have null IDs and `canUpdateVerificationStatus=false`.
3. Preview one disposable inactive Role with `IsActive=true`; verify `willReactivate=true`.
4. Confirm and poll the job; verify `reactivatedCount=1`, no new Role row, and existing Remark/RoleType remain unchanged.
5. Restore/clean the disposable test Role through the normal API flow.

