BEGIN;

-- Retire the legacy employee-insurance menu and every permission/plan mapping
-- that points to it. Generic policy modules are selected by different stable
-- codes and are intentionally outside this cleanup.
CREATE TEMP TABLE retired_policy_module_ids ON COMMIT DROP AS
SELECT "Id"
FROM axionpro."Module"
WHERE "ModuleCode" = 'EMP_INSURANCE';

DELETE FROM axionpro."TenantEnabledOperation"
WHERE "ModuleId" IN (SELECT "Id" FROM retired_policy_module_ids);

DELETE FROM axionpro."TenantEnabledModule"
WHERE "ModuleId" IN (SELECT "Id" FROM retired_policy_module_ids);

DELETE FROM axionpro."RoleModuleAndPermission"
WHERE "ModuleId" IN (SELECT "Id" FROM retired_policy_module_ids);

DELETE FROM axionpro."HostRoleModuleAndPermission"
WHERE "ModuleId" IN (SELECT "Id" FROM retired_policy_module_ids);

DELETE FROM axionpro."PlanModuleMapping"
WHERE "ModuleId" IN (SELECT "Id" FROM retired_policy_module_ids);

DELETE FROM axionpro."ModuleOperationMapping"
WHERE "ModuleId" IN (SELECT "Id" FROM retired_policy_module_ids);

DELETE FROM axionpro."Module"
WHERE "Id" IN (SELECT "Id" FROM retired_policy_module_ids);

-- These tables belong to the replaced policy implementations. Their live
-- equivalents are Policy, PolicyVersion, PolicyRule, PolicyApplicability,
-- PolicyAssignment, PolicyDocument and the related generic policy tables.
DROP TABLE IF EXISTS axionpro."EmployeePolicyDependentMapping" CASCADE;
DROP TABLE IF EXISTS axionpro."EmployeePolicyEnrollment" CASCADE;
DROP TABLE IF EXISTS axionpro."InsurancePolicyDocument" CASCADE;
DROP TABLE IF EXISTS axionpro."PolicyTypeInsuranceMapping" CASCADE;
DROP TABLE IF EXISTS axionpro."InsurancePolicy" CASCADE;
DROP TABLE IF EXISTS axionpro."EmployeeLeaveBalance" CASCADE;
DROP TABLE IF EXISTS axionpro."EmployeeLeavePolicyMapping" CASCADE;
DROP TABLE IF EXISTS axionpro."LeaveSandwichRuleMapping" CASCADE;
DROP TABLE IF EXISTS axionpro."LeaveSandwichRule" CASCADE;
DROP TABLE IF EXISTS axionpro."DayCombination" CASCADE;
DROP TABLE IF EXISTS axionpro."LeaveRule" CASCADE;
DROP TABLE IF EXISTS axionpro."PolicyLeaveTypeMapping" CASCADE;
DROP TABLE IF EXISTS axionpro."AccommodationAllowancePolicyByDesignation" CASCADE;
DROP TABLE IF EXISTS axionpro."MealAllowancePolicyByDesignation" CASCADE;
DROP TABLE IF EXISTS axionpro."TravelAllowancePolicyByDesignation" CASCADE;
DROP TABLE IF EXISTS axionpro."PolicyTypeDocument" CASCADE;
DROP TABLE IF EXISTS axionpro."UnStructuredPolicyTypeMappingWithEmployeeType" CASCADE;

ALTER TABLE IF EXISTS axionpro."LeaveRequest"
    DROP COLUMN IF EXISTS "LeavePolicyId";

COMMIT;
