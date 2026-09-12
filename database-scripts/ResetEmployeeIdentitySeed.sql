-- Resets only the Employee identity catalogue seed data.
-- Existing EmployeeIdentity records are intentionally preserved.
-- Run after taking the normal database backup.
BEGIN;
SELECT pg_advisory_xact_lock(7421061201);

-- Remove rules for the seeded identity documents first.
DELETE FROM axionpro."CountryIdentityRule" rule
WHERE rule."IdentityCategoryDocumentId" IN (
    SELECT document."Id"
    FROM axionpro."IdentityCategoryDocument" document
    WHERE upper(document."Code") IN (
        'AADHAAR', 'PAN', 'CNIC', 'SSN', 'EMIRATES_ID', 'NATIONAL_ID_CN'
    )
);

-- Do not delete a document that is referenced by an employee identity row.
DELETE FROM axionpro."IdentityCategoryDocument" document
WHERE upper(document."Code") IN (
    'AADHAAR', 'PAN', 'CNIC', 'SSN', 'EMIRATES_ID', 'NATIONAL_ID_CN'
)
AND NOT EXISTS (
    SELECT 1
    FROM axionpro."EmployeeIdentity" identity_record
    WHERE identity_record."IdentityCategoryDocumentId" = document."Id"
);

-- Remove the seed category only when it is no longer referenced.
DELETE FROM axionpro."IdentityCategory" category
WHERE upper(category."Code") = 'GOVT'
AND NOT EXISTS (
    SELECT 1
    FROM axionpro."IdentityCategoryDocument" document
    WHERE document."IdentityCategoryId" = category."Id"
);

-- Reset sequences only when the corresponding table is empty.
DO $$
DECLARE
    sequence_name text;
BEGIN
    IF NOT EXISTS (SELECT 1 FROM axionpro."IdentityCategory") THEN
        sequence_name := pg_get_serial_sequence('axionpro."IdentityCategory"', 'Id');
        IF sequence_name IS NOT NULL THEN PERFORM setval(sequence_name, 1, false); END IF;
    END IF;
    IF NOT EXISTS (SELECT 1 FROM axionpro."IdentityCategoryDocument") THEN
        sequence_name := pg_get_serial_sequence('axionpro."IdentityCategoryDocument"', 'Id');
        IF sequence_name IS NOT NULL THEN PERFORM setval(sequence_name, 1, false); END IF;
    END IF;
END $$;

COMMIT;
