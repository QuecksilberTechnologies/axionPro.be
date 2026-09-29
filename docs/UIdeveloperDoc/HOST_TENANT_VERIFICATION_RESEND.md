# Host Tenant Verification Resend

## Behavior

`POST /api/Tenant/{tenantId}/resend-verification-by-host` resends the pending
Tenant onboarding email through the active Host default SMTP configuration.
The authenticated Host permission pipeline remains authoritative. The UI must
resolve the current module and operation IDs from the authenticated menu data;
numeric examples are not stable identifiers.

## Request

- Authentication: Host bearer token.
- Route: `POST /api/Tenant/{tenantId}/resend-verification-by-host`
- Body: none.
- Permission carrier: `ModuleId` and `OperationId` query values injected by the
  existing module-operation interceptor.

## Success

```json
{
  "isSucceeded": true,
  "message": "Tenant verification email sent successfully.",
  "data": true
}
```

## SMTP provider failure

Provider failures use HTTP `502` and a safe actionable message. Do not replace
the backend message with a generic UI string.

```json
{
  "isSucceeded": false,
  "message": "The email provider rejected the API server IP address. Authorize the server IP in the email provider and try again.",
  "errorCode": "SMTP_IP_NOT_AUTHORIZED"
}
```

Other stable provider codes are `SMTP_AUTHENTICATION_FAILED` and
`SMTP_PROVIDER_REJECTED`. The response never contains SMTP credentials or the
raw provider exception.

## Persistence and retry

Sending does not mark the Tenant verified. A pending onboarding credential
remains pending until the recipient completes the verification link. Retrying
is user initiated; the UI must prevent duplicate in-flight requests.

## Validation status

Local focused automation passed on 2026-09-29. The Render deployment and live
Brevo delivery remain unverified until the API server IP is authorized and the
updated backend build is deployed.
