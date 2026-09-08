-- Supports seamless remote replacement of a device's opaque HTTPS gateway URL.
-- The old hash remains valid while a protected setdevinfo command gives the
-- physical device its new address. The pending hash is promoted on first poll.

ALTER TABLE axionpro."TenantDeviceConfiguration"
    ADD COLUMN IF NOT EXISTS "PendingHttpsIngressTokenHash" character varying(64) NULL;

ALTER TABLE axionpro."TenantDeviceConfiguration"
    ADD COLUMN IF NOT EXISTS "PendingHttpsIngressTokenExpiresDateTime" timestamp without time zone NULL;

ALTER TABLE axionpro."TenantDeviceConfiguration"
    DROP CONSTRAINT IF EXISTS "CK_TenantDeviceConfiguration_PendingHttpsIngressTokenHash";

ALTER TABLE axionpro."TenantDeviceConfiguration"
    ADD CONSTRAINT "CK_TenantDeviceConfiguration_PendingHttpsIngressTokenHash"
    CHECK ("PendingHttpsIngressTokenHash" IS NULL OR "PendingHttpsIngressTokenHash" ~ '^[0-9a-f]{64}$');

CREATE UNIQUE INDEX IF NOT EXISTS "UX_TenantDeviceConfiguration_PendingHttpsIngressTokenHash"
    ON axionpro."TenantDeviceConfiguration" ("PendingHttpsIngressTokenHash")
    WHERE "PendingHttpsIngressTokenHash" IS NOT NULL;
