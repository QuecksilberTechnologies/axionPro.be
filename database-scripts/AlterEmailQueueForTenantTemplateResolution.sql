BEGIN;

ALTER TABLE axionpro."EmailQueue"
    ADD COLUMN IF NOT EXISTS "TenantId" bigint NULL,
    ADD COLUMN IF NOT EXISTS "TemplateCode" character varying(100) NULL,
    ADD COLUMN IF NOT EXISTS "PlaceholdersJson" text NULL,
    ADD COLUMN IF NOT EXISTS "IsProcessing" boolean NOT NULL DEFAULT FALSE,
    ADD COLUMN IF NOT EXISTS "ProcessingStartedDateTime" timestamp with time zone NULL;

UPDATE axionpro."EmailQueue" queue
SET "TemplateCode" = template."TemplateCode"
FROM axionpro."EmailTemplate" template
WHERE queue."TemplateId" = template."Id"
  AND queue."TemplateCode" IS NULL;

CREATE INDEX IF NOT EXISTS "IX_EmailQueue_Pending"
    ON axionpro."EmailQueue" ("IsSent", "IsProcessing", "AddedDateTime", "Id");

COMMIT;
