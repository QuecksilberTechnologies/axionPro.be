-- Run only after the corrected API build is deployed and verified.

BEGIN;

DROP VIEW IF EXISTS axionpro."AccoumndationAllowancePolicyByDesignation";

COMMIT;

SELECT to_regclass('axionpro."AccommodationAllowancePolicyByDesignation"') AS corrected_table,
       to_regclass('axionpro."AccoumndationAllowancePolicyByDesignation"') AS legacy_compatibility_view;
