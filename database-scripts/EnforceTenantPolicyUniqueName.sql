-- Prevent two active policy identities in one tenant from using the same
-- trimmed, case-insensitive business name. Versions remain unique per PolicyId.
DO $$
BEGIN
    IF EXISTS (
        SELECT 1
        FROM axionpro."Policy"
        WHERE NOT "IsSoftDeleted"
        GROUP BY "TenantId", lower(btrim("PolicyName"))
        HAVING count(*) > 1
    ) THEN
        RAISE EXCEPTION
            'Duplicate tenant policy names exist. Reconcile the duplicate policy identities before applying UX_Policy_Tenant_NormalizedName.';
    END IF;
END $$;

CREATE UNIQUE INDEX IF NOT EXISTS "UX_Policy_Tenant_NormalizedName"
    ON axionpro."Policy" ("TenantId", lower(btrim("PolicyName")))
    WHERE NOT "IsSoftDeleted";
