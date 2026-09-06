-- Tenant address now has one authoritative home: TenantLocation.Address.
-- This intentionally removes the duplicate, legacy TenantProfile.Address data.
-- Run once before deploying the matching API build.

BEGIN;

ALTER TABLE IF EXISTS axionpro."TenantProfile"
    DROP COLUMN IF EXISTS "Address";

COMMIT;
