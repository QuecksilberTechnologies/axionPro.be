# Secure Device Initial Configuration — Implementation Report

## Scope completed

This change implements a secure, outbound-HTTPS device provisioning flow for the
AiFace-style physical device. The server never opens a connection to a private
device LAN address. The physical device initiates every connection.

The requested serial-only URL was deliberately strengthened. A URL such as
`/api/initial/{SNo}` alone is not safe: anyone who knows or guesses a serial
could impersonate the device. The implemented initial route is:

```text
POST /api/initial/{serial-number}/{opaque-64-hex-token}
```

The raw token is generated once, shown once to the Host operator, and only its
SHA-256 hash is stored in the database. Tenant, location, and device IDs are
never placed in the route.

## Endpoints

| Route | Caller | Purpose |
|---|---|---|
| `POST /api/initial-device-configure/issue-bootstrap-url` | Host provisioning role | Creates a temporary initial device URL for an unassigned HTTPS-capable physical device. Default heartbeat is 20 seconds. |
| `POST /api/initial/{serial}/{token}` | Physical device only | Receives device-initiated bootstrap polling. Invalid serial/token/payload gets a non-descriptive 404. |
| `POST /api/initial-device-configure/apply-runtime-configuration` | Tenant Admin role | Queues typed `setdevinfo` settings through the device's outbound HTTPS connection. |
| `POST /api/initial-device-configure/reboot` | Tenant Admin role | Queues reboot through the outbound HTTPS connection. |
| `POST /device-gateway/{token}` | Physical device only | Existing normal per-Tenant HTTPS gateway. |

The generic `POST /api/device-commands/submit` endpoint now rejects all
device-global administration, configuration, destructive, upgrade, and
file-write commands. Callers therefore cannot bypass the typed Tenant-admin
flow with a raw vendor JSON payload. At present the typed runtime operations
cover heartbeat, volume, local-WebServer disable, optional local password
rotation, and reboot. Further firmware settings must first receive a typed,
field-validated Tenant-admin operation before they are exposed.

## Operational flow

```text
Host Admin
  -> issues temporary bootstrap URL (20-second heartbeat)
  -> operator enters it in Device > Server > Use domain name = Yes
  -> device POSTs outbound HTTPS to /api/initial/{serial}/{token}

Host Admin
  -> creates/assigns TenantDevice

Tenant Admin
  -> creates HTTPS TenantDeviceConfiguration
     ServerUrl = https://axionpro-api.onrender.com
     ServerPath = /device-gateway
     ServerPort = 443
  -> applies typed runtime settings from Angular

Server
  -> queues encrypted-at-rest setdevinfo command on bootstrap route
  -> device receives normal gateway URL in the command response
  -> device switches to /device-gateway/{new opaque token}
  -> first normal poll revokes every active bootstrap URL for that device
```

For later updates, the device is already on its normal gateway. The same typed
endpoint queues heartbeat, volume, local-WebServer disable, optional new
WebServer password, then optional reboot in strict device order.

## Tenant-admin request contract

`apply-runtime-configuration` accepts only typed settings, not arbitrary vendor
JSON:

```json
{
  "tenantDeviceId": 0,
  "moduleId": 0,
  "operationId": 0,
  "currentWebServerPassword": "enter-current-device-password-in-ui",
  "heartbeatIntervalSeconds": 20,
  "volume": 8,
  "disableLocalWebServer": true,
  "newWebServerPassword": "optional-new-device-password",
  "rebootAfterApply": true
}
```

Validation rules:

- heartbeat: 10 to 3,600 seconds;
- volume: 0 to 15 when supplied;
- local WebServer disable is mandatory for this secure flow;
- current device WebServer password: 4 to 128 characters;
- optional replacement password: 8 to 128 characters;
- target must be an active HTTPS-capable Tenant device with HTTPS / port 443 /
  `/device-gateway` configuration.

The request body is never logged. The command payload is encrypted before it is
stored in `DeviceCommand`, decrypted only immediately before device delivery,
and sensitive JSON fields are redacted from `DeviceMessageLog`.

The optional replacement WebServer password is intentionally **not retained**
after queueing. A later configuration change requires the current password to
be entered again through the authorized Angular UI. This avoids making the
application database a long-term store of a local-device management password.

## Authorization model

Two module seeds are supplied in `database-scripts/CreateDeviceConfigurationModules.sql`:

- `HOST_INITIAL_DEVICE_CONFIGURATION`: issue initial bootstrap URLs only.
- `TENANT_DEVICE_CONFIGURATION`: read/create/update/delete Tenant device
  configuration, rotate its normal gateway URL, apply runtime settings, and
  reboot.

The code verifies the **module code**, not merely a caller-provided module ID.
This prevents a user with permission to some unrelated module from borrowing
that module's operation ID to configure a device.

Do not grant `TENANT_DEVICE_CONFIGURATION` to ordinary Tenant employees. Grant
it only to the Tenant Admin role through existing role-permission management.
Likewise grant the Host module only to approved provisioning roles.

## Database work

Run these scripts once, in order, after taking a database backup and before
deploying the matching API build:

1. `database-scripts/AddSecureInitialDeviceConfiguration.sql`
2. `database-scripts/CreateDeviceConfigurationModules.sql`

The first script adds:

- `DeviceCommand.IsSensitivePayload`;
- `DeviceInitialProvisioning`, containing token hashes, expiry, heartbeat,
  audit identity, and bootstrap connection timestamps;
- credential enum support for the existing `DeviceCredential` table.

Neither script was run by this implementation. They are intentionally
idempotent where safe; role assignment remains an explicit administrator
decision.

## Local Postman / LAN security

A public cloud API cannot block a laptop already on the same LAN from calling a
device IP address. The server-side controls implemented here stop **AxionPro
API** bypasses, not arbitrary LAN traffic.

The queued `setdevinfo` command sets `use_webserver` to `0`, requests the
device to disable its local WebServer, and is delivered through HTTPS. Confirm
that behavior on the exact firmware before relying on it. Defense in depth
still requires:

1. put devices on a separate VLAN/management network;
2. firewall the device management port from employee/user subnets;
3. use a strong device-local WebServer password while provisioning;
4. do not expose device IPs with router port forwarding;
5. revoke/reissue an initial URL if it was copied to an unintended party.

`webserver_pwd` is based on the observed device `getdevinfo` field. Verify the
exact firmware's `setdevinfo` acceptance on a non-production device before
using a password rotation in production.

## Code change record

| Area | Change |
|---|---|
| Domain/EF | Added `DeviceInitialProvisioning` entity, DbSet, indexes, and FK mapping. |
| Persistence | Added secure bootstrap service; added protected command payload encryption and audit redaction; added bootstrap polling support. |
| API | Added Host/Tenant configuration controller and hidden anonymous initial device gateway. |
| Authorization | Tenant configuration CRUD is Tenant-only and module-code constrained. Host retains only initial bootstrap issuance. |
| Commands | Generic raw device-global administration/configuration commands are blocked. Typed runtime endpoints queue only approved vendor fields. |
| Seed | Added Host and Tenant modules with active CRUD operation mappings. |
| Tests | Added opaque-token validation tests. |

## Verification evidence

- `dotnet build axionpro.api/axionpro.api.csproj --no-restore -clp:ErrorsOnly`
  completed successfully: **0 errors**.
- `dotnet test axionpro.automationtests/axionpro.automationtests.csproj --no-restore --filter "FullyQualifiedName~DeviceHttpsGatewaySecurityTests"`
  completed successfully: **5 passed, 0 failed**.

Existing solution warnings (package vulnerability advisories and pre-existing
nullable/XML documentation warnings) remain and are unrelated to this feature.

## Remaining rollout tasks

1. Run the two database scripts and assign the seeded permissions to the
   intended Host provisioning and Tenant Admin roles.
2. Deploy this API build with `DeviceGateway:PublicBaseUrl` set to the public
   HTTPS base URL. The current default is the requested Render URL.
3. Build Angular screens around the three authenticated controller endpoints;
   do not place direct device-IP or raw vendor JSON controls in the UI.
4. Use a test device to verify `setdevinfo` fields, especially
   `use_webserver` and `webserver_pwd`, against firmware `ai806_f06v_v5.16`.
5. Perform an end-to-end test: bootstrap -> assign TenantDevice -> create HTTPS
   configuration -> apply runtime settings -> observe normal gateway heartbeat
   -> confirm direct local WebServer is disabled.
