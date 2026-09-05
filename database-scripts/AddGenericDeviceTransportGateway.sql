-- Adds generic command-transport support and secure HTTPS device ingress.
-- Run this script once BEFORE deploying the matching API build.
-- It is additive and backfills legacy MQTT/MQTTS configurations safely.

BEGIN;

ALTER TABLE axionpro."TenantDeviceConfiguration"
    ADD COLUMN IF NOT EXISTS "CommandTransport" smallint NULL;

ALTER TABLE axionpro."TenantDeviceConfiguration"
    ADD COLUMN IF NOT EXISTS "HttpsIngressTokenHash" character varying(64) NULL;

-- Existing rows retain their transport semantics without any manual conversion.
UPDATE axionpro."TenantDeviceConfiguration"
SET "CommandTransport" = "MqttTransport"
WHERE "CommandTransport" IS NULL
  AND "MqttTransport" IN (1, 2);

ALTER TABLE axionpro."TenantDeviceConfiguration"
    DROP CONSTRAINT IF EXISTS "CK_TenantDeviceConfiguration_CommandTransport";

ALTER TABLE axionpro."TenantDeviceConfiguration"
    ADD CONSTRAINT "CK_TenantDeviceConfiguration_CommandTransport"
    CHECK ("CommandTransport" IS NULL OR "CommandTransport" IN (1, 2, 3, 4, 5, 6));

ALTER TABLE axionpro."TenantDeviceConfiguration"
    DROP CONSTRAINT IF EXISTS "CK_TenantDeviceConfiguration_HttpsIngressTokenHash";

ALTER TABLE axionpro."TenantDeviceConfiguration"
    ADD CONSTRAINT "CK_TenantDeviceConfiguration_HttpsIngressTokenHash"
    CHECK ("HttpsIngressTokenHash" IS NULL OR "HttpsIngressTokenHash" ~ '^[0-9a-f]{64}$');

CREATE UNIQUE INDEX IF NOT EXISTS "UX_TenantDeviceConfiguration_HttpsIngressTokenHash"
    ON axionpro."TenantDeviceConfiguration" ("HttpsIngressTokenHash")
    WHERE "HttpsIngressTokenHash" IS NOT NULL;

COMMIT;
