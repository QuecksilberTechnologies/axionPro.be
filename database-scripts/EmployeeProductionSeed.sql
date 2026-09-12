-- Employee production seed/migration bundle.
-- Idempotent: safe to run more than once.
-- Scope: tenant operational edit defaults and country identity catalogue.
-- CountryId is Employee nationality/origin country. Identity mappings are catalogue
-- entries only and intentionally non-mandatory; statutory applicability is not inferred.

BEGIN;
SELECT pg_advisory_xact_lock(7421061201);

CREATE TABLE IF NOT EXISTS axionpro."TenantEmployeeSectionDefault" (
    "TenantId" bigint NOT NULL REFERENCES axionpro."Tenant"("Id"),
    "ModuleCode" varchar(64) NOT NULL,
    "IsEditAllowed" boolean NOT NULL DEFAULT false,
    "UpdatedById" bigint NOT NULL,
    "UpdatedDateTime" timestamp with time zone NOT NULL DEFAULT now(),
    PRIMARY KEY ("TenantId", "ModuleCode"),
    CHECK ("ModuleCode" IN ('EMP_WORK_LOCATIONS', 'EMP_DEVICES',
        'EMP_WORK_ARRANGEMENT', 'EMP_WORK_PATTERN', 'EMP_OVERRIDES'))
);

INSERT INTO axionpro."IdentityCategory"
    ("Code", "Name", "Description", "IsActive", "AddedDateTime")
SELECT 'GOVT', 'Government Issued Identity',
       'Government-issued identity documents', true, now()
WHERE NOT EXISTS (
    SELECT 1 FROM axionpro."IdentityCategory" WHERE upper("Code") = 'GOVT'
);

DO $$
BEGIN
    IF (SELECT count(*) FROM axionpro."IdentityCategory"
        WHERE upper("Code") = 'GOVT') <> 1 THEN
        RAISE EXCEPTION 'Ambiguous GOVT identity category';
    END IF;
END $$;

INSERT INTO axionpro."IdentityCategoryDocument"
    ("IdentityCategoryId", "Code", "DocumentName", "Description",
     "IsUnique", "IsActive", "AddedDateTime")
SELECT category."Id", seed.code, seed.name, seed.description, true, true, now()
FROM axionpro."IdentityCategory" category
CROSS JOIN (VALUES
    ('AADHAAR', 'Aadhaar Card', 'UIDAI identity number; not proof of citizenship'),
    ('PAN', 'PAN Card', 'Indian tax identity number'),
    ('CNIC', 'CNIC', 'Pakistan national identity card'),
    ('SSN', 'Social Security Number', 'United States social security identifier'),
    ('EMIRATES_ID', 'Emirates ID', 'UAE citizen and resident identity card'),
    ('NATIONAL_ID_CN', 'Resident Identity Card', 'China resident identity card')
) AS seed(code, name, description)
WHERE upper(category."Code") = 'GOVT'
  AND NOT EXISTS (
      SELECT 1 FROM axionpro."IdentityCategoryDocument" existing
      WHERE upper(existing."Code") = seed.code
  );

INSERT INTO axionpro."CountryIdentityRule"
    ("CountryId", "IdentityCategoryDocumentId", "IsMandatory", "IsActive", "AddedDateTime")
SELECT country."Id", document."Id", false, true, now()
FROM (VALUES
    ('IN', 'IND', 'India', 'AADHAAR'),
    ('IN', 'IND', 'India', 'PAN'),
    ('PK', 'PAK', 'Pakistan', 'CNIC'),
    ('US', 'USA', 'United States', 'SSN'),
    ('AE', 'ARE', 'United Arab Emirates', 'EMIRATES_ID'),
    ('CN', 'CHN', 'China', 'NATIONAL_ID_CN')
) AS seed(iso2, iso3, country_name, document_code)
JOIN axionpro."Country" country
  ON upper(country."CountryCode") IN (seed.iso2, seed.iso3)
  OR lower(country."CountryName") = lower(seed.country_name)
JOIN axionpro."IdentityCategoryDocument" document
  ON upper(document."Code") = seed.document_code
 AND document."IsActive" = true
WHERE country."IsActive" = true
  AND NOT EXISTS (
      SELECT 1 FROM axionpro."CountryIdentityRule" existing
      WHERE existing."CountryId" = country."Id"
        AND existing."IdentityCategoryDocumentId" = document."Id"
  );

COMMIT;
