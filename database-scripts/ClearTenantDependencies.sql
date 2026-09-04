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
    - Host records, HostRole, Operation, SubscriptionPlan and other host/shared
      configuration are deliberately preserved.
    - With ReplaceLocationReferenceData = FALSE (the safe default), India and
      China are upserted with one capital-level city per state/region while
      existing global country rules and other country data remain untouched.
    - Replacing the entire Country/State/City catalog requires
      ReplaceLocationReferenceData = TRUE. It is fail-safe and aborts if any
      remaining non-tenant Country/State/City dependent data is detected.
*/

BEGIN;

CREATE TEMP TABLE tenant_cleanup_config
(
    "TenantId" bigint NULL,
    "ClearAllTenants" boolean NOT NULL,
    "ExecuteCleanup" boolean NOT NULL,
    "ReplaceLocationReferenceData" boolean NOT NULL
) ON COMMIT DROP;

-- CHANGE ONLY THIS LINE.
-- All tenants: (NULL, TRUE, FALSE, FALSE). One tenant: (54, FALSE, FALSE, FALSE).
-- Run once with ExecuteCleanup = FALSE. After reviewing the preview and taking
-- a backup, change only ExecuteCleanup to TRUE and run this whole file.
INSERT INTO tenant_cleanup_config
    ("TenantId", "ClearAllTenants", "ExecuteCleanup", "ReplaceLocationReferenceData")
VALUES (NULL, TRUE, FALSE, FALSE);

-- Intentionally a compact reference catalog: every India state/UT and China
-- province/region/municipality receives one capital-level city. It is not a
-- full city directory; add more cities later through the location master API.
CREATE TEMP TABLE location_reference_seed
(
    "SortOrder" integer PRIMARY KEY,
    "CountryCode" character varying(10) NOT NULL,
    "StateName" character varying(100) NOT NULL,
    "CityName" character varying(100) NOT NULL
) ON COMMIT DROP;

INSERT INTO location_reference_seed ("SortOrder", "CountryCode", "StateName", "CityName")
VALUES
    (1,  'IN', 'Andhra Pradesh', 'Amaravati'),
    (2,  'IN', 'Arunachal Pradesh', 'Itanagar'),
    (3,  'IN', 'Assam', 'Dispur'),
    (4,  'IN', 'Bihar', 'Patna'),
    (5,  'IN', 'Chhattisgarh', 'Raipur'),
    (6,  'IN', 'Goa', 'Panaji'),
    (7,  'IN', 'Gujarat', 'Gandhinagar'),
    (8,  'IN', 'Haryana', 'Chandigarh'),
    (9,  'IN', 'Himachal Pradesh', 'Shimla'),
    (10, 'IN', 'Jharkhand', 'Ranchi'),
    (11, 'IN', 'Karnataka', 'Bengaluru'),
    (12, 'IN', 'Kerala', 'Thiruvananthapuram'),
    (13, 'IN', 'Madhya Pradesh', 'Bhopal'),
    (14, 'IN', 'Maharashtra', 'Mumbai'),
    (15, 'IN', 'Manipur', 'Imphal'),
    (16, 'IN', 'Meghalaya', 'Shillong'),
    (17, 'IN', 'Mizoram', 'Aizawl'),
    (18, 'IN', 'Nagaland', 'Kohima'),
    (19, 'IN', 'Odisha', 'Bhubaneswar'),
    (20, 'IN', 'Punjab', 'Chandigarh'),
    (21, 'IN', 'Rajasthan', 'Jaipur'),
    (22, 'IN', 'Sikkim', 'Gangtok'),
    (23, 'IN', 'Tamil Nadu', 'Chennai'),
    (24, 'IN', 'Telangana', 'Hyderabad'),
    (25, 'IN', 'Tripura', 'Agartala'),
    (26, 'IN', 'Uttar Pradesh', 'Lucknow'),
    (27, 'IN', 'Uttarakhand', 'Dehradun'),
    (28, 'IN', 'West Bengal', 'Kolkata'),
    (29, 'IN', 'Andaman and Nicobar Islands', 'Port Blair'),
    (30, 'IN', 'Chandigarh', 'Chandigarh'),
    (31, 'IN', 'Dadra and Nagar Haveli and Daman and Diu', 'Daman'),
    (32, 'IN', 'Delhi', 'New Delhi'),
    (33, 'IN', 'Jammu and Kashmir', 'Srinagar'),
    (34, 'IN', 'Ladakh', 'Leh'),
    (35, 'IN', 'Lakshadweep', 'Kavaratti'),
    (36, 'IN', 'Puducherry', 'Puducherry'),
    (37, 'CN', 'Anhui', 'Hefei'),
    (38, 'CN', 'Beijing', 'Beijing'),
    (39, 'CN', 'Chongqing', 'Chongqing'),
    (40, 'CN', 'Fujian', 'Fuzhou'),
    (41, 'CN', 'Gansu', 'Lanzhou'),
    (42, 'CN', 'Guangdong', 'Guangzhou'),
    (43, 'CN', 'Guangxi', 'Nanning'),
    (44, 'CN', 'Guizhou', 'Guiyang'),
    (45, 'CN', 'Hainan', 'Haikou'),
    (46, 'CN', 'Hebei', 'Shijiazhuang'),
    (47, 'CN', 'Heilongjiang', 'Harbin'),
    (48, 'CN', 'Henan', 'Zhengzhou'),
    (49, 'CN', 'Hubei', 'Wuhan'),
    (50, 'CN', 'Hunan', 'Changsha'),
    (51, 'CN', 'Inner Mongolia', 'Hohhot'),
    (52, 'CN', 'Jiangsu', 'Nanjing'),
    (53, 'CN', 'Jiangxi', 'Nanchang'),
    (54, 'CN', 'Jilin', 'Changchun'),
    (55, 'CN', 'Liaoning', 'Shenyang'),
    (56, 'CN', 'Macao', 'Macao'),
    (57, 'CN', 'Ningxia', 'Yinchuan'),
    (58, 'CN', 'Qinghai', 'Xining'),
    (59, 'CN', 'Shaanxi', 'Xi''an'),
    (60, 'CN', 'Shandong', 'Jinan'),
    (61, 'CN', 'Shanghai', 'Shanghai'),
    (62, 'CN', 'Shanxi', 'Taiyuan'),
    (63, 'CN', 'Sichuan', 'Chengdu'),
    (64, 'CN', 'Tianjin', 'Tianjin'),
    (65, 'CN', 'Tibet', 'Lhasa'),
    (66, 'CN', 'Xinjiang', 'Urumqi'),
    (67, 'CN', 'Yunnan', 'Kunming'),
    (68, 'CN', 'Zhejiang', 'Hangzhou'),
    (69, 'CN', 'Hong Kong', 'Hong Kong'),
    (70, 'CN', 'Taiwan', 'Taipei');

DO
$$
DECLARE
    v_tenant_id bigint;
    v_clear_all_tenants boolean;
    v_execute_cleanup boolean;
    v_replace_location_reference_data boolean;
    v_count bigint;
    v_tenant_count bigint;
    v_dependency record;
    v_sequence record;
    v_reference_table text;
    v_reference_sequence text;
BEGIN
    SELECT "TenantId", "ClearAllTenants", "ExecuteCleanup", "ReplaceLocationReferenceData"
    INTO v_tenant_id, v_clear_all_tenants, v_execute_cleanup, v_replace_location_reference_data
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
         ))),
    ('Reference', 'Country', 'Id', (SELECT COUNT(*) FROM axionpro."Country")),
    ('Reference', 'State', 'Id', (SELECT COUNT(*) FROM axionpro."State")),
    ('Reference', 'City', 'Id', (SELECT COUNT(*) FROM axionpro."City")),
    ('Reference', 'District', 'Id', (SELECT COUNT(*) FROM axionpro."District")),
    ('Reference', 'CountryIdentityRule', 'Id', (SELECT COUNT(*) FROM axionpro."CountryIdentityRule")),
    ('Reference', 'CountryStatutoryRule', 'Id', (SELECT COUNT(*) FROM axionpro."CountryStatutoryRule")),
    ('Reference', 'StatutoryType', 'Id', (SELECT COUNT(*) FROM axionpro."StatutoryType")),
    ('Reference', 'TaxSystemMaster', 'Id', (SELECT COUNT(*) FROM axionpro."TaxSystemMaster")),
    ('Reference', 'TaxRegimeMaster', 'Id', (SELECT COUNT(*) FROM axionpro."TaxRegimeMaster"));

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
    -- 4. Rebuild the location-reference catalog only after every tenant row
    --    has gone. The guard prevents a host/non-tenant row from being removed
    --    merely because it uses CountryId, StateId or CityId.
    ---------------------------------------------------------------------------
    IF v_replace_location_reference_data THEN
        FOR v_dependency IN
            SELECT unnest(ARRAY[
                'Tenant', 'Employee', 'TenantLocation', 'ComplianceRule',
                'EmployeeTaxProfile', 'InsurancePolicy', 'SalaryComponentMaster',
                'TaxRule', 'TaxSlab'
            ]) AS table_name
        LOOP
            EXECUTE format('SELECT COUNT(*) FROM axionpro.%I', v_dependency.table_name)
            INTO v_count;

            IF v_count > 0 THEN
                RAISE EXCEPTION
                    'Location reference reset stopped: axionpro.% still has % row(s). No changes will be committed.',
                    v_dependency.table_name,
                    v_count;
            END IF;
        END LOOP;

        -- Reject new country/state/city FK tables that are not explicitly
        -- handled below. This keeps future schema changes fail-safe.
        FOR v_dependency IN
            SELECT DISTINCT source.relname AS table_name
            FROM pg_constraint fk
            JOIN pg_class target ON target.oid = fk.confrelid
            JOIN pg_namespace target_schema ON target_schema.oid = target.relnamespace
            JOIN pg_class source ON source.oid = fk.conrelid
            JOIN pg_namespace source_schema ON source_schema.oid = source.relnamespace
            WHERE fk.contype = 'f'
              AND target_schema.nspname = 'axionpro'
              AND source_schema.nspname = 'axionpro'
              AND target.relname IN ('Country', 'State', 'City')
              AND source.relname NOT IN
              (
                  'City', 'State', 'District', 'CountryIdentityRule',
                  'CountryStatutoryRule', 'StatutoryType', 'TaxSystemMaster',
                  'TaxRegimeMaster', 'TenantLocation', 'ComplianceRule',
                  'Employee', 'EmployeeTaxProfile', 'InsurancePolicy',
                  'SalaryComponentMaster', 'TaxRule', 'TaxSlab'
              )
        LOOP
            EXECUTE format('SELECT COUNT(*) FROM axionpro.%I', v_dependency.table_name)
            INTO v_count;

            IF v_count > 0 THEN
                RAISE EXCEPTION
                    'Location reference reset stopped: unhandled Country/State/City dependency axionpro.% has % row(s).',
                    v_dependency.table_name,
                    v_count;
            END IF;
        END LOOP;

        DELETE FROM axionpro."TaxRegimeMaster";
        DELETE FROM axionpro."CountryStatutoryRule";
        DELETE FROM axionpro."CountryIdentityRule";
        DELETE FROM axionpro."District";
        DELETE FROM axionpro."City";
        DELETE FROM axionpro."TaxSystemMaster";
        DELETE FROM axionpro."StatutoryType";
        DELETE FROM axionpro."State";
        DELETE FROM axionpro."Country";

        FOREACH v_reference_table IN ARRAY ARRAY[
            'Country', 'State', 'City', 'District', 'CountryIdentityRule',
            'CountryStatutoryRule', 'StatutoryType', 'TaxSystemMaster',
            'TaxRegimeMaster'
        ]
        LOOP
            SELECT pg_get_serial_sequence(format('axionpro.%I', v_reference_table), 'Id')
            INTO v_reference_sequence;

            IF v_reference_sequence IS NOT NULL THEN
                EXECUTE format('ALTER SEQUENCE %s RESTART WITH 1', v_reference_sequence);
            END IF;
        END LOOP;

        INSERT INTO axionpro."Country" ("CountryName", "CountryCode", "STDCode", "IsActive")
        VALUES
            ('India', 'IN', '+91', TRUE),
            ('China', 'CN', '+86', TRUE);

        INSERT INTO axionpro."State" ("CountryId", "StateName", "IsActive")
        SELECT country."Id", seed."StateName", TRUE
        FROM pg_temp.location_reference_seed seed
        JOIN axionpro."Country" country ON country."CountryCode" = seed."CountryCode"
        ORDER BY seed."SortOrder";

        INSERT INTO axionpro."City" ("StateId", "CityName", "IsActive")
        SELECT state."Id", seed."CityName", TRUE
        FROM pg_temp.location_reference_seed seed
        JOIN axionpro."Country" country ON country."CountryCode" = seed."CountryCode"
        JOIN axionpro."State" state
          ON state."CountryId" = country."Id"
         AND state."StateName" = seed."StateName"
        ORDER BY seed."SortOrder";

        IF (SELECT COUNT(*) FROM axionpro."Country") <> 2
           OR (SELECT COUNT(*) FROM axionpro."State") <> (SELECT COUNT(*) FROM pg_temp.location_reference_seed)
           OR (SELECT COUNT(*) FROM axionpro."City") <> (SELECT COUNT(*) FROM pg_temp.location_reference_seed) THEN
            RAISE EXCEPTION 'Location reference seed validation failed. No changes will be committed.';
        END IF;
    ELSE
        -- Safe production path: do not remove reference data that can be used
        -- by host/global compliance configuration. Ensure the India/China
        -- catalog exists and is active, adding only missing rows.
        -- A prior manual reset can leave an identity sequence behind existing
        -- rows. Synchronize it before any seed insert to prevent PK collisions.
        FOREACH v_reference_table IN ARRAY ARRAY['Country', 'State', 'City']
        LOOP
            SELECT pg_get_serial_sequence(format('axionpro.%I', v_reference_table), 'Id')
            INTO v_reference_sequence;

            IF v_reference_sequence IS NOT NULL THEN
                EXECUTE format(
                    'SELECT setval(%L::regclass, COALESCE(MAX("Id"), 1), COUNT(*) > 0) FROM axionpro.%I',
                    v_reference_sequence,
                    v_reference_table);
            END IF;
        END LOOP;

        UPDATE axionpro."Country"
        SET "CountryName" = CASE "CountryCode"
                                WHEN 'IN' THEN 'India'
                                WHEN 'CN' THEN 'China'
                            END,
            "STDCode" = CASE "CountryCode"
                            WHEN 'IN' THEN '+91'
                            WHEN 'CN' THEN '+86'
                         END,
            "IsActive" = TRUE
        WHERE "CountryCode" IN ('IN', 'CN');

        INSERT INTO axionpro."Country" ("CountryName", "CountryCode", "STDCode", "IsActive")
        SELECT source."CountryName", source."CountryCode", source."STDCode", TRUE
        FROM
        (
            VALUES
                ('India'::character varying(100), 'IN'::character varying(10), '+91'::character varying(10)),
                ('China'::character varying(100), 'CN'::character varying(10), '+86'::character varying(10))
        ) AS source("CountryName", "CountryCode", "STDCode")
        WHERE NOT EXISTS
        (
            SELECT 1
            FROM axionpro."Country" country
            WHERE country."CountryCode" = source."CountryCode"
        );

        UPDATE axionpro."State" state
        SET "IsActive" = TRUE
        FROM pg_temp.location_reference_seed seed
        JOIN axionpro."Country" country ON country."CountryCode" = seed."CountryCode"
        WHERE state."CountryId" = country."Id"
          AND state."StateName" = seed."StateName";

        INSERT INTO axionpro."State" ("CountryId", "StateName", "IsActive")
        SELECT country."Id", seed."StateName", TRUE
        FROM pg_temp.location_reference_seed seed
        JOIN axionpro."Country" country ON country."CountryCode" = seed."CountryCode"
        WHERE NOT EXISTS
        (
            SELECT 1
            FROM axionpro."State" state
            WHERE state."CountryId" = country."Id"
              AND state."StateName" = seed."StateName"
        )
        ORDER BY seed."SortOrder";

        UPDATE axionpro."City" city
        SET "IsActive" = TRUE
        FROM pg_temp.location_reference_seed seed
        JOIN axionpro."Country" country ON country."CountryCode" = seed."CountryCode"
        JOIN axionpro."State" state
          ON state."CountryId" = country."Id"
         AND state."StateName" = seed."StateName"
        WHERE city."StateId" = state."Id"
          AND city."CityName" = seed."CityName";

        INSERT INTO axionpro."City" ("StateId", "CityName", "IsActive")
        SELECT state."Id", seed."CityName", TRUE
        FROM pg_temp.location_reference_seed seed
        JOIN axionpro."Country" country ON country."CountryCode" = seed."CountryCode"
        JOIN axionpro."State" state
          ON state."CountryId" = country."Id"
         AND state."StateName" = seed."StateName"
        WHERE NOT EXISTS
        (
            SELECT 1
            FROM axionpro."City" city
            WHERE city."StateId" = state."Id"
              AND city."CityName" = seed."CityName"
        )
        ORDER BY seed."SortOrder";
    END IF;

    ---------------------------------------------------------------------------
    -- 5. Reset auto-generated IDs for empty tenant-dependent tables.
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
