# Angular-to-API Controller Documentation Sync

Generated: 2026-08-28 (current controller and Angular source scan)

## Scope

- Angular source analysed: `C:\latestAxionProUI\axionpro-app\src`
- Backend source updated: `axionpro.api\Controllers`
- Matching requires both HTTP method and normalized route to match. Query strings,
  case, the `api/` prefix, and route-parameter names/types are normalized.
- Only active TypeScript HTTP calls and active C# HTTP actions were considered.

## Result

| Item | Count |
|---|---:|
| Current backend HTTP endpoints | 284 |
| Exact Angular-to-backend matches | 205 |
| Backend endpoints without an exact Angular call | 79 |
| Controllers changed | 68 |
| Routed Angular components resolved | 132 |
| Matched endpoints without a statically resolvable page route | 8 |

## Changes made

For every one of the 205 matched controller actions, the XML documentation now
contains:

1. the Angular UI purpose inferred from the service method;
2. the Angular page route or, when static route resolution is not possible, the
   consuming Angular component/source file; and
3. the Angular API service method and source location.

For each of the 79 backend endpoints without an exact Angular match, only the
endpoint-level XML documentation was removed. The controller action, route,
authorization, request/response types, and all implementation code were left
unchanged.

## Example

`GET /api/asset/get` (`AssetController.GetAllAssets`) now documents that it
retrieves assets for `/app/assets/list` through
`AssetsApi.getAssets (app/core/services/assets-api.ts:35)`.

## Interpretation notes

- This is static source analysis; a dynamically composed URL or dynamically
  loaded component may not resolve to a page route. Those 8 cases are stated as
  such in the endpoint XML documentation instead of assigning an inaccurate page.
- “Unmatched” means no exact method-and-route call was found in the scanned
  Angular source. It does not mean the backend API was deleted or is invalid.
- The endpoint-level XML comments in the controllers are the detailed,
  per-endpoint documentation and are the authoritative output of this sync.
