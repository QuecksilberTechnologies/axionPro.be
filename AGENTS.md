# User instructions for this repository

These instructions apply throughout this repository and should be followed in every session working on this project.

- Do not create or change code, folders, or business logic on your own initiative. Work only within the scope explicitly requested by the user. Do not add unsolicited implementations, refactors, or alternative flows.
- If a requirement, existing flow, or intended change is unclear, ask the user before implementing it. Do not guess or invent behavior.
- Maintain the existing Employee handler and repository patterns. Preserve and maintain the project's `#region` structure, comments, and endpoint documentation when making authorized changes.
- Use the existing permission pipeline. Do not introduce separate permission logic or bypass the established flow.
- Keep code readable and properly formatted across multiple lines. Do not compress handlers, methods, or logic into single-line implementations.
- Reuse the existing constants files, enums, and mappings. Inspect the existing definitions before making changes; do not invent duplicate constants, enums, or mapping logic.
- Refresh API coexistence decision (2026-09-13): read `docs/AUTH_REFRESH_TOKEN_V2.md` before changing refresh authentication. The user requested the lightweight API on a separate branch. Preserve existing `/api/Auth/refresh-token` and its response/handler; `/api/Auth/refresh-token-v2` is opt-in. Do not switch UI consumers or remove the old API until the UI developer confirms consistency and the user explicitly approves retirement. Do not treat local tests as deployment acceptance.
- Follow the established project conventions and the user's requested scope. Ask before introducing any new logic or structure that the user has not authorized.
- Until the bulk-import work is complete, read `docs/AI_ASSISTED_BULK_IMPORT_REFERENCE.md` before working on bulk onboarding/import tasks. Use it as the continuing design and progress reference. Record user-approved decisions, completed work, validation results, and pending work there. Do not treat unresolved decisions or proposed features as implemented behavior or authorization to invent business rules.
- For each added bulk module/feature, create meaningful automated test cases in the existing `axionpro.automationtests` project and run the relevant tests. Maintain the reference file's implementation sequence and COMPLETE / WIP / PENDING statuses. Mark a feature COMPLETE only after its required tests pass; record skipped or blocked tests explicitly, never as passes.
- For every user-requested new API or endpoint, create or update its UI implementation handoff under `docs/UIdeveloperDoc`. Document feature behavior, authentication and dynamic permission-ID discovery, routes/methods, mandatory and optional inputs, copyable JSON/FormData/query and Excel/CSV examples where applicable, representative response/error JSON, enums/statuses, polling/retry/cancellation, persistence tables/timing/isolation, and exact tested versus deployed status. Numeric ModuleId/OperationId examples are illustrative only; UI consumers must resolve current IDs through the existing authenticated menu/permission flow.

Original user instruction (Hinglish):

> kahi bhi apni marzi se code/folder/ new logic bilkul nahi likna hai, kuch nahi samaj mei aaye to puchna hai, Employee Handler/repo/#regin/comments/endpoint doc ko maitain karna hai, permissions ke liye pipeline bani hai, code ko ek hi line mei likh kar khidi nahi banai hai, constans file/enums/mapping alreay hai usski ka use karna hai , apni marzi se kuch bhi nahi likhna hai
