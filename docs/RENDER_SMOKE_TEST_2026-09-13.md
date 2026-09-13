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

## China/USA Employee identity check

- Created China employee `78N5XZW2` through `POST /api/Employee/create`; the Employee list confirms CountryId `2`, CountryCode `CN`, Nationality `China`.
- Live `GET /api/Employee/Sensitive/get` returned HTTP 500 for that employee. Root cause is the read path's dependency on the manually maintained PostgreSQL `GetEmployeeIdentityByCountryRule` function; that function is stored only in `axionpro.application/DTOS/SPFunctions.txt` and is absent from executable migration/seed scripts, so deployed database-function drift produces a generic 500. The repository has been changed locally to an EF tenant-safe country-rule query, eliminating that deployment dependency. Date columns now use `DateOnly`, and the response projection's EffectiveTo copy/paste defect is corrected.
- Live Country options contain only India and China. United States/USA is absent, so a valid USA Employee cannot currently be created. Existing identity seed mappings include USA/SSN only when a matching active Country row already exists; they do not create that Country row.
