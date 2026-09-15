-- Corrects the legacy Accoumndation spelling without changing table data or behavior.

BEGIN;

DO $rename$
BEGIN
    IF to_regclass('axionpro."AccommodationAllowancePolicyByDesignation"') IS NOT NULL
       AND to_regclass('axionpro."AccoumndationAllowancePolicyByDesignation"') IS NOT NULL THEN
        RAISE EXCEPTION
            'Both old and corrected accommodation policy tables exist; manual reconciliation is required.';
    END IF;

    IF to_regclass('axionpro."AccommodationAllowancePolicyByDesignation"') IS NULL
       AND to_regclass('axionpro."AccoumndationAllowancePolicyByDesignation"') IS NOT NULL THEN
        ALTER TABLE axionpro."AccoumndationAllowancePolicyByDesignation"
            RENAME TO "AccommodationAllowancePolicyByDesignation";

        -- Keep the currently deployed API compatible until the corrected build
        -- is deployed. This simple view remains automatically updatable.
        CREATE VIEW axionpro."AccoumndationAllowancePolicyByDesignation" AS
        SELECT *
        FROM axionpro."AccommodationAllowancePolicyByDesignation";
    END IF;
END
$rename$;

COMMIT;

SELECT to_regclass('axionpro."AccommodationAllowancePolicyByDesignation"') AS corrected_table,
       to_regclass('axionpro."AccoumndationAllowancePolicyByDesignation"') AS legacy_table;
