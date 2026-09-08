# Tenant device configuration — Angular UI handoff

## Scope and boundaries

This screen is for an **assigned Tenant device** only. It gives a Tenant
administrator control of the device's runtime configuration, location,
outbound HTTPS gateway, local Web UI/API access, and the physical
System/Local-Manager PIN.

Do not use this screen for Employee/user enrollment, user add/update, or
attendance administration. Those remain in their dedicated controllers.

Never display or send a numeric Tenant-device/configuration ID. Use the
opaque `id` / `tenantDeviceId` token returned by the Tenant device APIs. Do
not render `deviceMasterName`, `deviceMasterModelNo`, `deviceMasterSNo`, or a
device serial number in the Tenant UI.

Every Tenant request below requires the normal authorization header and the
existing `tenantId`, `moduleId`, and `operationId` fields for the
`TENANT_DEVICE_CONFIGURATION` permission. Every settings request also needs
the opaque `tenantDeviceId` and `currentWebServerPassword` fields. The latter
is an input-only field: keep it masked, do not persist it in browser storage,
and clear it from the form after the request completes.

### Permission enforcement (backend)

The API does not rely on Angular button visibility. Every MediatR request in
this feature is registered in
`TenantDeviceConfigurationPermissionBehavior&lt;TRequest,TResponse&gt;` and is
checked in this order:

1. validate the authenticated principal from the access token;
2. read the supplied module ID and require its server-side code to equal
   `TENANT_DEVICE_CONFIGURATION`;
3. require an authenticated Tenant employee and resolve the Tenant, employee,
   and current role from the trusted request context;
4. call `CheckTenantEmployeePermissionAsync(tenant, employee, role, module,
   operation)`; and
5. proceed only when the database function returns its allow result. A denied
   operation stops before the command handler or command queue runs.

Host principals are denied these assigned-device runtime endpoints. Their
scope remains inventory assignment and initial provisioning only.

## Command-delivery model

All setting changes are durable server-side queue items; Angular never sends
vendor JSON or calls a device LAN address.

| Device transport | Delivery |
| --- | --- |
| HTTPS | The device fetches the next queued command during its own outbound heartbeat poll. The server does not open a connection to the device. |
| MQTTS | The queue can be published immediately with the explicit manual endpoint below. It still uses the already queued command, never a raw Angular payload. |

The existing central MQTT dispatcher remains enabled for the application's
pre-existing MQTT/MQTTS command flows. It never handles HTTPS settings; those
are delivered only by the device heartbeat poll. The manual MQTTS endpoint is
an additional controlled dispatch path and can race the normal broker worker,
so the UI should use it only when an immediate publish is required.

A successful settings API result means **queued**, not necessarily applied.
Show the returned command tracking values, then refresh the device status and
command history until it is completed or failed. Diagnostic message logs and
stored command responses redact password, PIN, token, and other credential
fields.

### Queue retention and status lifecycle

Queued command rows are **not auto-deleted**. They remain as the audit and
retry record. The lifecycle is:

`Queued` → `Publishing` → `AwaitingResponse` → `Completed`

or, when a broker/publish/response deadline fails:

`Publishing` → `RetryScheduled` → `Publishing` (up to the attempt limit) →
`Failed`.

Publish-only commands go from `Publishing` directly to `Completed`. The
outbound `DeviceMessageLog` and the redacted device response are retained as
well; there is no queue worker that removes the command row after delivery.

### Transport resolution

Angular does not submit `transport`, `mqtt`, `mqtts`, or `https` for a setting.
The backend decodes the opaque `tenantDeviceId`, loads that Tenant device's
stored `TenantDeviceConfiguration`, and resolves its configured
`CommandTransport` (falling back to the legacy `MqttTransport` only when the
new field is absent). The queue dispatcher then filters by that stored
transport. This prevents a caller from routing a command to a different
transport by changing a request field.

The older connection-record `create`/`update` routes still exist for the
transport/gateway record itself (and therefore expose transport fields for
that setup contract). They are not the runtime-settings form. Once the record
exists, every `settings/*`, location, URL-replacement, and manual-dispatch
request is transport-neutral from Angular's point of view.

## Dropdown data APIs

Do not hard-code firmware numbers in Angular. Call the matching endpoint when
the corresponding card first opens (cache the result for that browser session).
Each response is `data[]`, with `key`, `label`, and `options[]` (`value`,
`label`).

| Angular card / section | Call when opened | Dropdown endpoint |
| --- | --- | --- |
| Time and scheduled reboot | Time card opens | `GET /api/device-ddl-options/time` |
| Bell settings | Bell card opens | `GET /api/device-ddl-options/bell` |
| Screen, voice, and recognition | Device setup card opens | `GET /api/device-ddl-options/device-setup` |
| Verification, QR, and camera | Advanced card opens | `GET /api/device-ddl-options/advanced` |
| Door, Wiegand, and access settings | Lock card opens | `GET /api/device-ddl-options/lock` |
| Serial port | Serial card opens | `GET /api/device-ddl-options/serial` |
| Ethernet | Ethernet card opens | `GET /api/device-ddl-options/ethernet` |
| Wi-Fi | Wi-Fi card opens | `GET /api/device-ddl-options/wifi` |
| App notification | App notification card opens | `GET /api/device-ddl-options/app-notification` |

The dropdown labels are deliberately human-readable. The selected `value` is
submitted in the typed request below; the backend maps it to the vendor field.

## Settings APIs

All endpoints below are `POST /api/TenantDeviceConfiguration/...` and return a
queued-command response. Send only the fields of the requested typed DTO plus
the common permission/device fields.

| UI card | Endpoint | User-facing fields |
| --- | --- | --- |
| Time | `settings/time` | Clock/date format, DST and start/end dates, network time, timezone, three scheduled reboot times. |
| Set clock now | `settings/time/sync` | Optional `utcDateTime`; leave absent to use server UTC now. |
| Bell | `settings/bell` | Bell count, ring pattern, bell output. |
| Device setup | `settings/device-setup` | Language, voice volume/name announcement, multi-face detection, result/screen-saver/sleep timing, wake-up method, recognition distance, result style, liveness, avatar. |
| Advanced | `settings/advanced` | Maximum admins, verification/QR/privacy options, face/liveness/fingerprint/palm thresholds, mask settings, fill light, exposure, disable-face and online-debug switches. |
| Lock / access hardware | `settings/lock` | Door delay/sensor/access controls, anti-passback, Wiegand/card formats and reversals, interlock/alarm, failure limits, access logging options. |
| Serial | `settings/serial` | Device address, network port, baud rate, serial function. |
| Ethernet | `settings/ethernet` | DHCP, IP/subnet/gateway/DNS when static, hide-IP. Do not send static values while DHCP is selected. |
| Wi-Fi | `settings/wifi` | DHCP, IP/subnet/gateway when static. Do not send static values while DHCP is selected. |
| App notification | `settings/app-notification` | Enabled switch, app token, notification frequency. Treat app token as a password. |
| Local Web UI/API | `settings/web-access` | `localWebServerEnabled`; optional `newWebServerPassword` to rotate the device Web UI/API password. The old password is supplied in `currentWebServerPassword` to authorize this change. |
| Physical System-menu lock | `settings/screen-menu-pin` | `screenMenuPin` (numeric input, 4–12 digits). This changes the device's Local-Manager/System-menu PIN; it does not hide the System menu. |

For all cards except location, include `currentWebServerPassword`. It is sent
only inside an encrypted-at-rest queue item and is not included in the API
response or stored diagnostics.

### Location

Use `POST /api/TenantDevice/update-location` with the opaque
`tenantDeviceId`, a Tenant-owned `tenantLocationId`, and permission fields.
This changes only the assigned Tenant location. It cannot change the Host
inventory DeviceMaster assignment.

## HTTPS gateway address and replacement

`GET /api/TenantDeviceConfiguration/gateway-address/{tenantDeviceId}` returns
only safe metadata: server base URL/path/port and whether an active or pending
gateway exists. It intentionally does **not** return the complete current
gateway URL because its trailing bearer token is stored only as a hash.

To remotely change an existing HTTPS device URL without changing its
heartbeat, use:

```text
POST /api/TenantDeviceConfiguration/replace-https-gateway-url
```

Send the common settings fields plus `replacementLifetimeMinutes` (5–1440).
The backend creates a pending one-time URL and queues the corresponding device
configuration through the **still active** gateway. It does not show the raw
replacement URL to the browser. The old address remains valid until the
device's first successful poll on the pending address; only then is the new
address promoted and the old one revoked. Show `isReplacementPending` and
`replacementExpiresDateTime` from `gateway-address` while waiting.

Do not use the old rotate endpoint for a remote replacement: it invalidates
the old address immediately and is for a technician who will enter a fresh URL
on the physical device.

## Explicit MQTTS delivery

For a device whose configured command transport is MQTTS, the UI may show a
**Dispatch next queued command now** button:

```text
POST /api/TenantDeviceConfiguration/dispatch-mqtts-now
```

Its request contains the opaque `tenantDeviceId` and permission fields only.
It accepts no command name, payload, password, or broker credential. The
response has `wasDispatched` and a message. HTTPS devices must not show this
button: their queue is delivered through the next heartbeat poll.

## Suggested UI states

1. Disable every submit button while its request is in flight.
2. On `queued`, show “Waiting for device heartbeat” for HTTPS, or “Queued for
   secure MQTT” for MQTTS; do not show “Saved on device” yet.
3. On a command failure, show the server's non-sensitive failure message and
   retain non-secret form values for correction. Never repopulate passwords,
   PINs, or app tokens.
4. For network, Web UI/API, and physical menu PIN changes, show a confirmation
   dialog because the change may alter future technician access.
5. Angular must never call `http://<device-ip>/...`; all device action goes
   through AxionPro APIs and the durable command queue.

## Database deployment

Before deploying the remote HTTPS replacement feature, apply
`database-scripts/AddPendingHttpsGatewayReplacement.sql`. It adds the pending
gateway hash/expiry columns and the uniqueness constraint used to safely
promote a replacement after the device confirms it.

## Automated tests

`axionpro.automationtests/Unit/TenantDeviceConfigurationPermissionBehaviorTests.cs`
verifies the permission behavior for the new typed Web UI/API settings,
manual MQTTS dispatch, and remote gateway replacement requests. It covers the
allowed path plus wrong-module and denied-operation rejection before a handler
can run.

Run only these tests with:

```powershell
dotnet test .\axionpro.automationtests\axionpro.automationtests.csproj --filter "FullyQualifiedName~TenantDeviceConfigurationPermissionBehaviorTests"
```
