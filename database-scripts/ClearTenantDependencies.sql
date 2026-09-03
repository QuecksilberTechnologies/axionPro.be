/*
    Tenant clean-up script (PostgreSQL / pgAdmin compatible)

    Purpose
    -------
    Physically remove either one tenant or every tenant with their tenant-scoped
    employee data, so fresh tenant entries can be created. The script was built
    from the live axionpro schema's TenantId / EmployeeId columns and foreign-key
    metadata.

    Safety
    ------
    1. To clean every tenant, set ClearAllTenants = TRUE and leave TenantId NULL.
       To clean one tenant only, set ClearAllTenants = FALSE and provide the
       numeric axionpro."Tenant"."Id" value (never an encrypted ID).
    2. First run with ExecuteCleanup = FALSE. It produces per-table counts and
       commits no deletion.
    3. Verify the output, take a database backup, then change ExecuteCleanup to
       TRUE and run the entire file in one batch.
    4. An error before COMMIT causes the transaction to roll back. Do not run
       individual DELETE statements from this file separately.

    Notes
    -----
    - This removes database rows only. It does not remove physical files from
      object storage/local disk referenced by ticket or employee attachments.
    - Shared/global records such as Country, Operation, HostRole, and
      SubscriptionPlan are deliberately not deleted.
*/

BEGIN;

CREATE TEMP TABLE tenant_cleanup_config
(
    "TenantId" bigint NULL,
    "ClearAllTenants" boolean NOT NULL,
    "ExecuteCleanup" boolean NOT NULL
) ON COMMIT DROP;

-- CHANGE ONLY THIS LINE.
-- All tenants: (NULL, TRUE, FALSE). One tenant: (54, FALSE, FALSE).
INSERT INTO tenant_cleanup_config ("TenantId", "ClearAllTenants", "ExecuteCleanup")
VALUES (NULL, TRUE, FALSE);

DO
$$
DECLARE
    v_tenant_id bigint;
    v_clear_all_tenants boolean;
    v_execute_cleanup boolean;
    v_count bigint;
    v_tenant_count bigint;
    v_dependency record;
    v_sequence record;
BEGIN
    SELECT "TenantId", "ClearAllTenants", "ExecuteCleanup"
    INTO v_tenant_id, v_clear_all_tenants, v_execute_cleanup
    FROM pg_temp.tenant_cleanup_config;

    IF NOT v_clear_all_tenants AND (v_tenant_id IS NULL OR v_tenant_id <= 0) THEN
        RAISE EXCEPTION 'For a single-tenant cleanup, set a valid numeric Tenant.Id in tenant_cleanup_config.';
    END IF;

    CREATE TEMP TABLE tenant_cleanup_scope ON COMMIT DROP AS
    SELECT "Id"
    FROM axionpro."Tenant"
    WHERE v_clear_all_tenants OR "Id" = v_tenant_id;

    IF NOT EXISTS (SELECT 1 FROM pg_temp.tenant_cleanup_scope) THEN
        RAISE EXCEPTION 'No tenant matched the cleanup configuration.';
    END IF;

    SELECT COUNT(*) INTO v_tenant_count FROM pg_temp.tenant_cleanup_scope;

    -- The full employee set is used for preview. It is narrowed to one tenant
    -- for each delete iteration below.
    CREATE TEMP TABLE tenant_cleanup_employee ("Id" bigint PRIMARY KEY) ON COMMIT DROP;
    INSERT INTO pg_temp.tenant_cleanup_employee ("Id")
    SELECT "Id"
    FROM axionpro."Employee"
    WHERE "TenantId" IN (SELECT "Id" FROM pg_temp.tenant_cleanup_scope);

    CREATE TEMP TABLE tenant_cleanup_preflight
    (
        "Scope" text NOT NULL,
        "TableName" text NOT NULL,
        "MatchColumn" text NOT NULL,
        "RecordCount" bigint NOT NULL
    ) ON COMMIT DROP;

    -- Inventory every table that explicitly stores TenantId or EmployeeId,
    -- including legacy tables that do not have an FK constraint.
    FOR v_dependency IN
        SELECT c.relname AS table_name, a.attname AS column_name
        FROM pg_class c
        JOIN pg_namespace n ON n.oid = c.relnamespace
        JOIN pg_attribute a ON a.attrelid = c.oid
        WHERE n.nspname = 'axionpro'
          AND c.relkind IN ('r', 'p')
          AND a.attnum > 0
          AND NOT a.attisdropped
          AND a.attname IN ('TenantId', 'EmployeeId')
        ORDER BY a.attname, c.relname
    LOOP
        IF v_dependency.column_name = 'TenantId' THEN
            EXECUTE format(
                'INSERT INTO pg_temp.tenant_cleanup_preflight ("Scope", "TableName", "MatchColumn", "RecordCount")
                 SELECT ''Tenant'', %L, %L, COUNT(*)
                 FROM axionpro.%I
                 WHERE %I IN (SELECT "Id" FROM pg_temp.tenant_cleanup_scope)',
                v_dependency.table_name,
                v_dependency.column_name,
                v_dependency.table_name,
                v_dependency.column_name);
        ELSE
            EXECUTE format(
                'INSERT INTO pg_temp.tenant_cleanup_preflight ("Scope", "TableName", "MatchColumn", "RecordCount")
                 SELECT ''Employee'', %L, %L, COUNT(*)
                 FROM axionpro.%I
                 WHERE %I IN (SELECT "Id" FROM pg_temp.tenant_cleanup_employee)',
                v_dependency.table_name,
                v_dependency.column_name,
                v_dependency.table_name,
                v_dependency.column_name);
        END IF;
    END LOOP;

    -- These rows are reached through LoginCredential, Ticket, TicketThread,
    -- PayrollEmployee, or UserRole rather than through a direct TenantId / EmployeeId column.
    INSERT INTO pg_temp.tenant_cleanup_preflight ("Scope", "TableName", "MatchColumn", "RecordCount")
    VALUES
    ('Indirect', 'RefreshToken', 'LoginCredentialId',
        (SELECT COUNT(*) FROM axionpro."RefreshToken" rt
         WHERE rt."LoginCredentialId" IN
         (
             SELECT lc."Id" FROM axionpro."LoginCredential" lc
             WHERE lc."TenantId" IN (SELECT "Id" FROM pg_temp.tenant_cleanup_scope)
                OR lc."EmployeeId" IN (SELECT "Id" FROM pg_temp.tenant_cleanup_employee)
         ))),
    ('Indirect', 'TicketAttachment', 'TicketId',
        (SELECT COUNT(*) FROM axionpro."TicketAttachment" ta
         WHERE ta."TicketId" IN
         (
             SELECT t."Id" FROM axionpro."Ticket" t
             WHERE t."TenantId" IN (SELECT "Id" FROM pg_temp.tenant_cleanup_scope)
         ))),
    ('Indirect', 'ThreadMessage', 'ThreadId',
        (SELECT COUNT(*) FROM axionpro."ThreadMessage" tm
         WHERE tm."ThreadId" IN
         (
             SELECT tt."Id" FROM axionpro."TicketThread" tt
             WHERE tt."TenantId" IN (SELECT "Id" FROM pg_temp.tenant_cleanup_scope)
         ))),
    ('Indirect', 'PayrollEmployeeDetail', 'PayrollEmployeeId',
        (SELECT COUNT(*) FROM axionpro."PayrollEmployeeDetail" ped
         WHERE ped."PayrollEmployeeId" IN
         (
             SELECT pe."Id" FROM axionpro."PayrollEmployee" pe
             WHERE pe."EmployeeId" IN (SELECT "Id" FROM pg_temp.tenant_cleanup_employee)
                OR pe."PayrollRunId" IN
                (
                    SELECT pr."Id" FROM axionpro."PayrollRun" pr
                    WHERE pr."TenantId" IN (SELECT "Id" FROM pg_temp.tenant_cleanup_scope)
                )
         ))),
    ('Indirect', 'EmployeeExperienceDocument', 'EmployeeExperienceId',
        (SELECT COUNT(*) FROM axionpro."EmployeeExperienceDocument" eed
         WHERE eed."EmployeeExperienceId" IN
         (
             SELECT ee."Id" FROM axionpro."EmployeeExperience" ee
             WHERE ee."EmployeeId" IN (SELECT "Id" FROM pg_temp.tenant_cleanup_employee)
         ))),
    ('Indirect', 'TenantDeviceConfiguration', 'TenantDeviceId',
        (SELECT COUNT(*) FROM axionpro."TenantDeviceConfiguration" tdc
         WHERE tdc."TenantDeviceId" IN
         (
             SELECT td."Id" FROM axionpro."TenantDevice" td
             WHERE td."TenantId" IN (SELECT "Id" FROM pg_temp.tenant_cleanup_scope)
         )));

    -- This remains empty in preview mode and is populated only after a
    -- successful actual cleanup followed by a safe sequence reset.
    CREATE TEMP TABLE tenant_cleanup_sequence_reset
    (
        "TableName" text NOT NULL,
        "SequenceName" text NOT NULL
    ) ON COMMIT DROP;

    IF NOT v_execute_cleanup THEN
        RAISE NOTICE 'PREVIEW ONLY: no rows were deleted. Change ExecuteCleanup to TRUE only after reviewing tenant_cleanup_preflight.';
        RETURN;
    END IF;

    -- Delete one original tenant at a time. This keeps all employee-id filters
    -- correctly scoped while the original tenant list remains fixed.
    FOR v_tenant_id IN
        SELECT "Id"
        FROM pg_temp.tenant_cleanup_scope
        ORDER BY "Id"
    LOOP
        TRUNCATE TABLE pg_temp.tenant_cleanup_employee;
        INSERT INTO pg_temp.tenant_cleanup_employee ("Id")
        SELECT "Id"
        FROM axionpro."Employee"
        WHERE "TenantId" = v_tenant_id;

        -- Prevent two cleanup sessions from purging the same tenant at the same time.
        PERFORM pg_advisory_xact_lock(hashtextextended('axionpro.tenant.cleanup.' || v_tenant_id::text, 0));

        BEGIN
    ---------------------------------------------------------------------------
    -- 1. Leaf/dependent data. Delete children before their Tenant/Employee parents.
    ---------------------------------------------------------------------------

    DELETE FROM axionpro."ThreadMessage"
    WHERE "ThreadId" IN
          (SELECT "Id" FROM axionpro."TicketThread" WHERE "TenantId" = v_tenant_id)
       OR "AddedById" IN (SELECT "Id" FROM pg_temp.tenant_cleanup_employee);

    DELETE FROM axionpro."TicketAttachment"
    WHERE "TicketId" IN
          (SELECT "Id" FROM axionpro."Ticket" WHERE "TenantId" = v_tenant_id)
       OR "UploadedByUserId" IN (SELECT "Id" FROM pg_temp.tenant_cleanup_employee);

    DELETE FROM axionpro."TicketHistory"
    WHERE "TenantId" = v_tenant_id
       OR "TicketId" IN
          (SELECT "Id" FROM axionpro."Ticket" WHERE "TenantId" = v_tenant_id)
       OR "DoneByUserId" IN (SELECT "Id" FROM pg_temp.tenant_cleanup_employee);

    DELETE FROM axionpro."AssetAssignment"
    WHERE "TenantId" = v_tenant_id
       OR "EmployeeId" IN (SELECT "Id" FROM pg_temp.tenant_cleanup_employee)
       OR "RequestId" IN
          (SELECT "Id" FROM axionpro."AssetRequest" WHERE "TenantId" = v_tenant_id);

    DELETE FROM axionpro."AssetHistory"
    WHERE "TenantId" = v_tenant_id
       OR "EmployeeId" IN (SELECT "Id" FROM pg_temp.tenant_cleanup_employee)
       OR "ScrapApprovedBy" IN (SELECT "Id" FROM pg_temp.tenant_cleanup_employee);

    DELETE FROM axionpro."AssetImage"
    WHERE "TenantId" = v_tenant_id
       OR "AssetId" IN (SELECT "Id" FROM axionpro."Asset" WHERE "TenantId" = v_tenant_id);

    DELETE FROM axionpro."AssetTicketTypeDetail"
    WHERE "AssetTypeId" IN (SELECT "Id" FROM axionpro."AssetType" WHERE "TenantId" = v_tenant_id)
       OR "TicketTypeId" IN (SELECT "Id" FROM axionpro."TicketType" WHERE "TenantId" = v_tenant_id)
       OR "ResponsibleRoleId" IN (SELECT "Id" FROM axionpro."Role" WHERE "TenantId" = v_tenant_id);

    DELETE FROM axionpro."EmployeeExperienceDocument"
    WHERE "EmployeeExperienceId" IN
          (SELECT "Id" FROM axionpro."EmployeeExperience"
           WHERE "EmployeeId" IN (SELECT "Id" FROM pg_temp.tenant_cleanup_employee));

    DELETE FROM axionpro."EmployeePolicyDependentMapping"
    WHERE "TenantId" = v_tenant_id
       OR "DependentId" IN
          (SELECT "Id" FROM axionpro."EmployeeDependent"
           WHERE "EmployeeId" IN (SELECT "Id" FROM pg_temp.tenant_cleanup_employee))
       OR "EmployeePolicyEnrollmentId" IN
          (SELECT "Id" FROM axionpro."EmployeePolicyEnrollment"
           WHERE "EmployeeId" IN (SELECT "Id" FROM pg_temp.tenant_cleanup_employee));

    DELETE FROM axionpro."EmployeeLeaveBalance"
    WHERE "TenantId" = v_tenant_id
       OR "EmployeeLeavePolicyMappingId" IN
          (SELECT "Id" FROM axionpro."EmployeeLeavePolicyMapping"
           WHERE "TenantId" = v_tenant_id
              OR "EmployeeId" IN (SELECT "Id" FROM pg_temp.tenant_cleanup_employee));

    DELETE FROM axionpro."EmployeeWorkModeOverrideRequest"
    WHERE "TenantId" = v_tenant_id
       OR "EmployeeId" IN (SELECT "Id" FROM pg_temp.tenant_cleanup_employee)
       OR "EmployeeWorkArrangementId" IN
          (SELECT "Id" FROM axionpro."EmployeeWorkArrangement"
           WHERE "TenantId" = v_tenant_id
              OR "EmployeeId" IN (SELECT "Id" FROM pg_temp.tenant_cleanup_employee))
       OR "TenantLocationId" IN
          (SELECT "Id" FROM axionpro."TenantLocation" WHERE "TenantId" = v_tenant_id);

    DELETE FROM axionpro."EmployeeWorkPattern"
    WHERE "TenantId" = v_tenant_id
       OR "EmployeeWorkArrangementId" IN
          (SELECT "Id" FROM axionpro."EmployeeWorkArrangement"
           WHERE "TenantId" = v_tenant_id
              OR "EmployeeId" IN (SELECT "Id" FROM pg_temp.tenant_cleanup_employee))
       OR "TenantLocationId" IN
          (SELECT "Id" FROM axionpro."TenantLocation" WHERE "TenantId" = v_tenant_id);

    DELETE FROM axionpro."TenantDeviceConfiguration"
    WHERE "TenantDeviceId" IN
          (SELECT "Id" FROM axionpro."TenantDevice" WHERE "TenantId" = v_tenant_id);

    DELETE FROM axionpro."PayrollEmployeeDetail"
    WHERE "PayrollEmployeeId" IN
          (SELECT "Id" FROM axionpro."PayrollEmployee"
           WHERE "EmployeeId" IN (SELECT "Id" FROM pg_temp.tenant_cleanup_employee)
              OR "PayrollRunId" IN
                 (SELECT "Id" FROM axionpro."PayrollRun" WHERE "TenantId" = v_tenant_id));

    DELETE FROM axionpro."SalaryStructureDetail"
    WHERE "SalaryStructureId" IN
          (SELECT "Id" FROM axionpro."SalaryStructure" WHERE "TenantId" = v_tenant_id)
       OR "ComponentId" IN
          (SELECT "Id" FROM axionpro."SalaryComponentMaster" WHERE "TenantId" = v_tenant_id)
       OR "DependsOnComponentId" IN
          (SELECT "Id" FROM axionpro."SalaryComponentMaster" WHERE "TenantId" = v_tenant_id);

    DELETE FROM axionpro."InsurancePolicyDocument"
    WHERE "TenantId" = v_tenant_id
       OR "InsurancePolicyId" IN
          (SELECT "Id" FROM axionpro."InsurancePolicy" WHERE "TenantId" = v_tenant_id);

    DELETE FROM axionpro."PolicyTypeInsuranceMapping"
    WHERE "TenantId" = v_tenant_id
       OR "PolicyTypeId" IN (SELECT "Id" FROM axionpro."PolicyType" WHERE "TenantId" = v_tenant_id)
       OR "InsurancePolicyId" IN
          (SELECT "Id" FROM axionpro."InsurancePolicy" WHERE "TenantId" = v_tenant_id);

    DELETE FROM axionpro."PolicyTypeDocument"
    WHERE "TenantId" = v_tenant_id
       OR "PolicyTypeId" IN (SELECT "Id" FROM axionpro."PolicyType" WHERE "TenantId" = v_tenant_id);

    DELETE FROM axionpro."UnStructuredPolicyTypeMappingWithEmployeeType"
    WHERE "TenantId" = v_tenant_id
       OR "PolicyTypeId" IN (SELECT "Id" FROM axionpro."PolicyType" WHERE "TenantId" = v_tenant_id);

    DELETE FROM axionpro."AccoumndationAllowancePolicyByDesignation"
    WHERE "DesignationId" IN (SELECT "Id" FROM axionpro."Designation" WHERE "TenantId" = v_tenant_id)
       OR "PolicyTypeId" IN (SELECT "Id" FROM axionpro."PolicyType" WHERE "TenantId" = v_tenant_id);

    DELETE FROM axionpro."MealAllowancePolicyByDesignation"
    WHERE "DesignationId" IN (SELECT "Id" FROM axionpro."Designation" WHERE "TenantId" = v_tenant_id)
       OR "PolicyTypeId" IN (SELECT "Id" FROM axionpro."PolicyType" WHERE "TenantId" = v_tenant_id);

    DELETE FROM axionpro."TravelAllowancePolicyByDesignation"
    WHERE "DesignationId" IN (SELECT "Id" FROM axionpro."Designation" WHERE "TenantId" = v_tenant_id)
       OR "PolicyTypeId" IN (SELECT "Id" FROM axionpro."PolicyType" WHERE "TenantId" = v_tenant_id);

    DELETE FROM axionpro."LeaveSandwichRuleMapping"
    WHERE "TenantId" = v_tenant_id
       OR "LeaveSandwichRuleId" IN
          (SELECT "Id" FROM axionpro."LeaveSandwichRule" WHERE "TenantId" = v_tenant_id)
       OR "DayCombinationId" IN
          (SELECT "Id" FROM axionpro."DayCombination" WHERE "TenantId" = v_tenant_id)
       OR "LeaveRuleId" IN
          (SELECT "Id" FROM axionpro."LeaveRule" WHERE "TenantId" = v_tenant_id);

    DELETE FROM axionpro."EmployeePolicyEnrollment"
    WHERE "TenantId" = v_tenant_id
       OR "EmployeeId" IN (SELECT "Id" FROM pg_temp.tenant_cleanup_employee)
       OR "PolicyTypeId" IN (SELECT "Id" FROM axionpro."PolicyType" WHERE "TenantId" = v_tenant_id)
       OR "InsurancePolicyId" IN
          (SELECT "Id" FROM axionpro."InsurancePolicy" WHERE "TenantId" = v_tenant_id);

    DELETE FROM axionpro."EmployeeLeavePolicyMapping"
    WHERE "TenantId" = v_tenant_id
       OR "EmployeeId" IN (SELECT "Id" FROM pg_temp.tenant_cleanup_employee)
       OR "PolicyLeaveTypeMappingId" IN
          (SELECT "Id" FROM axionpro."PolicyLeaveTypeMapping" WHERE "TenantId" = v_tenant_id);

    DELETE FROM axionpro."EmployeeManagerMapping"
    WHERE "TenantId" = v_tenant_id
       OR "EmployeeId" IN (SELECT "Id" FROM pg_temp.tenant_cleanup_employee)
       OR "ManagerId" IN (SELECT "Id" FROM pg_temp.tenant_cleanup_employee)
       OR "DepartmentId" IN (SELECT "Id" FROM axionpro."Department" WHERE "TenantId" = v_tenant_id)
       OR "DesignationId" IN (SELECT "Id" FROM axionpro."Designation" WHERE "TenantId" = v_tenant_id)
       OR "ReportingTypeId" IN (SELECT "Id" FROM axionpro."ReportingType" WHERE "TenantId" = v_tenant_id);

    DELETE FROM axionpro."EmployeeLocationAssignment"
    WHERE "TenantId" = v_tenant_id
       OR "EmployeeId" IN (SELECT "Id" FROM pg_temp.tenant_cleanup_employee)
       OR "TenantLocationId" IN
          (SELECT "Id" FROM axionpro."TenantLocation" WHERE "TenantId" = v_tenant_id);

    DELETE FROM axionpro."EmployeeDeviceEnrollment"
    WHERE "TenantId" = v_tenant_id
       OR "EmployeeId" IN (SELECT "Id" FROM pg_temp.tenant_cleanup_employee);

    DELETE FROM axionpro."EmployeeWorkArrangement"
    WHERE "TenantId" = v_tenant_id
       OR "EmployeeId" IN (SELECT "Id" FROM pg_temp.tenant_cleanup_employee)
       OR "AttendancePolicyId" IN
          (SELECT "Id" FROM axionpro."AttendancePolicy" WHERE "TenantId" = v_tenant_id)
       OR "PrimaryTenantLocationId" IN
          (SELECT "Id" FROM axionpro."TenantLocation" WHERE "TenantId" = v_tenant_id);

    DELETE FROM axionpro."EmployeeSalary"
    WHERE "EmployeeId" IN (SELECT "Id" FROM pg_temp.tenant_cleanup_employee)
       OR "SalaryStructureId" IN
          (SELECT "Id" FROM axionpro."SalaryStructure" WHERE "TenantId" = v_tenant_id);

    DELETE FROM axionpro."PayrollEmployee"
    WHERE "EmployeeId" IN (SELECT "Id" FROM pg_temp.tenant_cleanup_employee)
       OR "PayrollRunId" IN
          (SELECT "Id" FROM axionpro."PayrollRun" WHERE "TenantId" = v_tenant_id);

    DELETE FROM axionpro."EmployeeCategorySkill"
    WHERE "EmployeeId" IN (SELECT "Id" FROM pg_temp.tenant_cleanup_employee);

    DELETE FROM axionpro."EmployeeContact"
    WHERE "EmployeeId" IN (SELECT "Id" FROM pg_temp.tenant_cleanup_employee);

    DELETE FROM axionpro."EmployeeImage"
    WHERE "TenantId" = v_tenant_id
       OR "EmployeeId" IN (SELECT "Id" FROM pg_temp.tenant_cleanup_employee);

    DELETE FROM axionpro."UserAttendanceSetting"
    WHERE "EmployeeId" IN (SELECT "Id" FROM pg_temp.tenant_cleanup_employee);

    DELETE FROM axionpro."Attendance"
    WHERE "EmployeeId" IN (SELECT "Id" FROM pg_temp.tenant_cleanup_employee);

    DELETE FROM axionpro."AttendanceHistory"
    WHERE "EmployeeId" IN (SELECT "Id" FROM pg_temp.tenant_cleanup_employee);

    DELETE FROM axionpro."AttendanceRequest"
    WHERE "EmployeeId" IN (SELECT "Id" FROM pg_temp.tenant_cleanup_employee);

    DELETE FROM axionpro."EmployeeBankDetail"
    WHERE "EmployeeId" IN (SELECT "Id" FROM pg_temp.tenant_cleanup_employee);

    DELETE FROM axionpro."EmployeeDailyAttendance"
    WHERE "EmployeeId" IN (SELECT "Id" FROM pg_temp.tenant_cleanup_employee);

    DELETE FROM axionpro."EmployeeDependent"
    WHERE "EmployeeId" IN (SELECT "Id" FROM pg_temp.tenant_cleanup_employee);

    DELETE FROM axionpro."EmployeeEducation"
    WHERE "EmployeeId" IN (SELECT "Id" FROM pg_temp.tenant_cleanup_employee);

    DELETE FROM axionpro."EmployeeExperience"
    WHERE "EmployeeId" IN (SELECT "Id" FROM pg_temp.tenant_cleanup_employee);

    DELETE FROM axionpro."EmployeeIdentity"
    WHERE "EmployeeId" IN (SELECT "Id" FROM pg_temp.tenant_cleanup_employee);

    DELETE FROM axionpro."EmployeePersonalDetail"
    WHERE "EmployeeId" IN (SELECT "Id" FROM pg_temp.tenant_cleanup_employee);

    DELETE FROM axionpro."EmployeeStatutoryAccount"
    WHERE "EmployeeId" IN (SELECT "Id" FROM pg_temp.tenant_cleanup_employee);

    DELETE FROM axionpro."EmployeeTaxProfile"
    WHERE "EmployeeId" IN (SELECT "Id" FROM pg_temp.tenant_cleanup_employee);

    DELETE FROM axionpro."EmployeeWorkProfile"
    WHERE "EmployeeId" IN (SELECT "Id" FROM pg_temp.tenant_cleanup_employee);

    DELETE FROM axionpro."EmployeesChangedTypeHistory"
    WHERE "EmployeeId" IN (SELECT "Id" FROM pg_temp.tenant_cleanup_employee);

    DELETE FROM axionpro."ForgotPasswordOTPDetail"
    WHERE "TenantId" = v_tenant_id
       OR "EmployeeId" IN (SELECT "Id" FROM pg_temp.tenant_cleanup_employee);

    DELETE FROM axionpro."LeaveRequest"
    WHERE "TenantId" = v_tenant_id
       OR "EmployeeId" IN (SELECT "Id" FROM pg_temp.tenant_cleanup_employee)
       OR "LeaveTypeId" IN (SELECT "Id" FROM axionpro."LeaveType" WHERE "TenantId" = v_tenant_id)
       OR "LeavePolicyId" IN
          (SELECT "Id" FROM axionpro."PolicyLeaveTypeMapping" WHERE "TenantId" = v_tenant_id);

    DELETE FROM axionpro."RefreshToken"
    WHERE "LoginCredentialId" IN
          (SELECT "Id" FROM axionpro."LoginCredential"
           WHERE "TenantId" = v_tenant_id
              OR "EmployeeId" IN (SELECT "Id" FROM pg_temp.tenant_cleanup_employee));

    DELETE FROM axionpro."InterviewPanelMember"
    WHERE "UserRoleId" IN
          (SELECT "Id" FROM axionpro."UserRole"
           WHERE "EmployeeId" IN (SELECT "Id" FROM pg_temp.tenant_cleanup_employee));

    DELETE FROM axionpro."TenderProject"
    WHERE "UserRoleId" IN
          (SELECT "Id" FROM axionpro."UserRole"
           WHERE "EmployeeId" IN (SELECT "Id" FROM pg_temp.tenant_cleanup_employee));

    DELETE FROM axionpro."UserRole"
    WHERE "EmployeeId" IN (SELECT "Id" FROM pg_temp.tenant_cleanup_employee)
       OR "RoleId" IN (SELECT "Id" FROM axionpro."Role" WHERE "TenantId" = v_tenant_id);

    DELETE FROM axionpro."LoginCredential"
    WHERE "TenantId" = v_tenant_id
       OR "EmployeeId" IN (SELECT "Id" FROM pg_temp.tenant_cleanup_employee);

    -- Ticket has several NO ACTION Employee foreign keys, so it must be gone
    -- before deleting the employees it can reference.
    DELETE FROM axionpro."Ticket" WHERE "TenantId" = v_tenant_id;

    DELETE FROM axionpro."Employee" WHERE "TenantId" = v_tenant_id;

    ---------------------------------------------------------------------------
    -- 2. Tenant-owned organisation, leave, payroll, asset, ticket, role and module data.
    ---------------------------------------------------------------------------

    DELETE FROM axionpro."TicketThread" WHERE "TenantId" = v_tenant_id;
    DELETE FROM axionpro."TicketType" WHERE "TenantId" = v_tenant_id;
    DELETE FROM axionpro."TicketHeader" WHERE "TenantId" = v_tenant_id;
    DELETE FROM axionpro."TicketClassification" WHERE "TenantId" = v_tenant_id;

    DELETE FROM axionpro."AssetRequest"
    WHERE "TenantId" = v_tenant_id
       OR "EmployeeId" IN (SELECT "Id" FROM pg_temp.tenant_cleanup_employee);
    DELETE FROM axionpro."Asset" WHERE "TenantId" = v_tenant_id;
    DELETE FROM axionpro."AssetType" WHERE "TenantId" = v_tenant_id;
    DELETE FROM axionpro."AssetStatus" WHERE "TenantId" = v_tenant_id;
    DELETE FROM axionpro."AssetCategory" WHERE "TenantId" = v_tenant_id;

    DELETE FROM axionpro."LeaveSandwichRule" WHERE "TenantId" = v_tenant_id;
    DELETE FROM axionpro."LeaveRule" WHERE "TenantId" = v_tenant_id;
    DELETE FROM axionpro."PolicyLeaveTypeMapping" WHERE "TenantId" = v_tenant_id;
    DELETE FROM axionpro."LeaveType" WHERE "TenantId" = v_tenant_id;
    DELETE FROM axionpro."DayCombination" WHERE "TenantId" = v_tenant_id;
    DELETE FROM axionpro."OrganizationHolidayCalendar" WHERE "TenantId" = v_tenant_id;
    DELETE FROM axionpro."HolidayMaster" WHERE "TenantId" = v_tenant_id;

    DELETE FROM axionpro."PayrollRun" WHERE "TenantId" = v_tenant_id;
    DELETE FROM axionpro."SalaryStructure" WHERE "TenantId" = v_tenant_id;
    DELETE FROM axionpro."SalaryComponentMaster" WHERE "TenantId" = v_tenant_id;

    DELETE FROM axionpro."InsurancePolicy" WHERE "TenantId" = v_tenant_id;
    DELETE FROM axionpro."AttendancePolicy" WHERE "TenantId" = v_tenant_id;
    DELETE FROM axionpro."PolicyType" WHERE "TenantId" = v_tenant_id;

    DELETE FROM axionpro."TenantDevice" WHERE "TenantId" = v_tenant_id;
    DELETE FROM axionpro."TenantLocation" WHERE "TenantId" = v_tenant_id;
    DELETE FROM axionpro."Designation" WHERE "TenantId" = v_tenant_id;
    DELETE FROM axionpro."Department" WHERE "TenantId" = v_tenant_id;
    DELETE FROM axionpro."ReportingType" WHERE "TenantId" = v_tenant_id;

    DELETE FROM axionpro."RoleModuleAndPermission"
    WHERE "RoleId" IN (SELECT "Id" FROM axionpro."Role" WHERE "TenantId" = v_tenant_id)
       OR "ModuleId" IN (SELECT "Id" FROM axionpro."Module" WHERE "TenantId" = v_tenant_id);

    DELETE FROM axionpro."HostRoleModuleAndPermission"
    WHERE "ModuleId" IN (SELECT "Id" FROM axionpro."Module" WHERE "TenantId" = v_tenant_id);

    DELETE FROM axionpro."ModuleOperationMapping"
    WHERE "ModuleId" IN (SELECT "Id" FROM axionpro."Module" WHERE "TenantId" = v_tenant_id);

    DELETE FROM axionpro."PlanModuleMapping"
    WHERE "ModuleId" IN (SELECT "Id" FROM axionpro."Module" WHERE "TenantId" = v_tenant_id);

    DELETE FROM axionpro."TenantEnabledOperation" WHERE "TenantId" = v_tenant_id;
    DELETE FROM axionpro."TenantEnabledModule" WHERE "TenantId" = v_tenant_id;
    DELETE FROM axionpro."Role" WHERE "TenantId" = v_tenant_id;
    DELETE FROM axionpro."Module" WHERE "TenantId" = v_tenant_id;

    ---------------------------------------------------------------------------
    -- 3. Remaining direct TenantId rows, then the tenant itself.
    ---------------------------------------------------------------------------

    DELETE FROM axionpro."ApprovalWorkflow" WHERE "TenantId" = v_tenant_id;
    DELETE FROM axionpro."ComplianceRule" WHERE "TenantId" = v_tenant_id;
    DELETE FROM axionpro."DeviceCommandQueue" WHERE "TenantId" = v_tenant_id;
    DELETE FROM axionpro."DeviceLogRaw" WHERE "TenantId" = v_tenant_id;
    DELETE FROM axionpro."EmailsLog" WHERE "TenantId" = v_tenant_id;
    DELETE FROM axionpro."EmployeeCodePattern" WHERE "TenantId" = v_tenant_id;
    DELETE FROM axionpro."RequestType" WHERE "TenantId" = v_tenant_id;
    DELETE FROM axionpro."TaxRule" WHERE "TenantId" = v_tenant_id;
    DELETE FROM axionpro."TaxSlab" WHERE "TenantId" = v_tenant_id;
    DELETE FROM axionpro."TenantEncryptionKeys" WHERE "TenantId" = v_tenant_id;
    DELETE FROM axionpro."TenantEmailConfig" WHERE "TenantId" = v_tenant_id;
    DELETE FROM axionpro."TenantProfile" WHERE "TenantId" = v_tenant_id;
    DELETE FROM axionpro."TenantSubscription" WHERE "TenantId" = v_tenant_id;
    DELETE FROM axionpro."WorkflowStage" WHERE "TenantId" = v_tenant_id;

    DELETE FROM axionpro."Tenant" WHERE "Id" = v_tenant_id;

    IF EXISTS (SELECT 1 FROM axionpro."Tenant" WHERE "Id" = v_tenant_id) THEN
        RAISE EXCEPTION 'Tenant % could not be deleted. The transaction will roll back.', v_tenant_id;
    END IF;

    RAISE NOTICE 'Tenant % and its tenant-scoped data have been deleted. Commit will finalize the cleanup.', v_tenant_id;
        EXCEPTION
            WHEN OTHERS THEN
                RAISE EXCEPTION
                    'Tenant cleanup failed for Tenant Id % (SQLSTATE %): %',
                    v_tenant_id,
                    SQLSTATE,
                    SQLERRM;
        END;
    END LOOP;

    ---------------------------------------------------------------------------
    -- 4. Reset auto-generated IDs for empty tenant-dependent tables.
    --    Shared/host tables that still contain records are skipped so their
    --    next IDs cannot collide with existing primary keys.
    ---------------------------------------------------------------------------
    FOR v_sequence IN
        WITH RECURSIVE tenant_roots(table_oid) AS
        (
            SELECT 'axionpro."Tenant"'::regclass::oid
            UNION
            SELECT c.oid
            FROM pg_class c
            JOIN pg_namespace n ON n.oid = c.relnamespace
            JOIN pg_attribute a ON a.attrelid = c.oid
            WHERE n.nspname = 'axionpro'
              AND c.relkind IN ('r', 'p')
              AND a.attnum > 0
              AND NOT a.attisdropped
              AND a.attname IN ('TenantId', 'EmployeeId')
        ),
        tenant_dependencies(table_oid) AS
        (
            SELECT table_oid FROM tenant_roots
            UNION
            SELECT fk.conrelid
            FROM pg_constraint fk
            JOIN tenant_dependencies parent ON parent.table_oid = fk.confrelid
            JOIN pg_class child ON child.oid = fk.conrelid
            JOIN pg_namespace child_schema ON child_schema.oid = child.relnamespace
            WHERE fk.contype = 'f'
              AND child_schema.nspname = 'axionpro'
        )
        SELECT DISTINCT
            tbl.relname AS table_name,
            seq_ns.nspname AS sequence_schema,
            seq.relname AS sequence_name
        FROM pg_class seq
        JOIN pg_namespace seq_ns ON seq_ns.oid = seq.relnamespace
        JOIN pg_depend dep ON dep.objid = seq.oid AND dep.deptype IN ('a', 'i')
        JOIN pg_class tbl ON tbl.oid = dep.refobjid
        JOIN tenant_dependencies td ON td.table_oid = tbl.oid
        WHERE seq.relkind = 'S'
          AND seq_ns.nspname = 'axionpro'
        ORDER BY tbl.relname, seq.relname
    LOOP
        EXECUTE format('LOCK TABLE axionpro.%I IN ACCESS EXCLUSIVE MODE', v_sequence.table_name);
        EXECUTE format('SELECT COUNT(*) FROM axionpro.%I', v_sequence.table_name)
        INTO v_count;

        IF v_count = 0 THEN
            EXECUTE format(
                'ALTER SEQUENCE %I.%I RESTART WITH 1',
                v_sequence.sequence_schema,
                v_sequence.sequence_name);

            INSERT INTO pg_temp.tenant_cleanup_sequence_reset ("TableName", "SequenceName")
            VALUES (v_sequence.table_name, v_sequence.sequence_name);
        END IF;
    END LOOP;

    RAISE NOTICE '% tenant(s) and their tenant-scoped data have been deleted. Commit will finalize the cleanup.', v_tenant_count;
END;
$$;

-- Always review this result. With ExecuteCleanup = FALSE, it is the only data output.
SELECT "Scope", "TableName", "MatchColumn", "RecordCount"
FROM pg_temp.tenant_cleanup_preflight
WHERE "RecordCount" > 0
ORDER BY "Scope", "TableName", "MatchColumn";

-- Filled only when ExecuteCleanup = TRUE and the cleanup succeeds.
SELECT "TableName", "SequenceName"
FROM pg_temp.tenant_cleanup_sequence_reset
ORDER BY "TableName", "SequenceName";

COMMIT;
