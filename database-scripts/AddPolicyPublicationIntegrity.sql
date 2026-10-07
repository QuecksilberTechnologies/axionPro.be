BEGIN;

ALTER TABLE axionpro."PolicyVersion"
    ADD COLUMN IF NOT EXISTS "ApprovedContentChecksumSha256" varchar(64);

ALTER TABLE axionpro."PolicyApprovalHistory"
    ADD COLUMN IF NOT EXISTS "ContentChecksumSha256" varchar(64);

UPDATE axionpro."PolicyApprovalHistory"
SET "ContentChecksumSha256" = repeat('0', 64)
WHERE "ContentChecksumSha256" IS NULL;

ALTER TABLE axionpro."PolicyApprovalHistory"
    ALTER COLUMN "ContentChecksumSha256" SET NOT NULL;

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM pg_constraint
        WHERE conname = 'CK_PolicyVersion_ApprovedContentChecksum'
          AND connamespace = 'axionpro'::regnamespace
    ) THEN
        ALTER TABLE axionpro."PolicyVersion"
            ADD CONSTRAINT "CK_PolicyVersion_ApprovedContentChecksum"
            CHECK ("ApprovedContentChecksumSha256" IS NULL
                OR "ApprovedContentChecksumSha256" ~ '^[0-9A-F]{64}$');
    END IF;

    IF NOT EXISTS (
        SELECT 1
        FROM pg_constraint
        WHERE conname = 'CK_PolicyApprovalHistory_ContentChecksum'
          AND connamespace = 'axionpro'::regnamespace
    ) THEN
        ALTER TABLE axionpro."PolicyApprovalHistory"
            ADD CONSTRAINT "CK_PolicyApprovalHistory_ContentChecksum"
            CHECK ("ContentChecksumSha256" ~ '^[0-9A-F]{64}$');
    END IF;
END $$;

COMMIT;
