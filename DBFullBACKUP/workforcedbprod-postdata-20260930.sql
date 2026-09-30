--
-- PostgreSQL database dump
--

\restrict K4mxsBl5LHtryxcSR0x8XGhncPP5x1KVYQrZ96gYp3Qg7XSLcKP5lcihOQWpP0u

-- Dumped from database version 18.3
-- Dumped by pg_dump version 18.3

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

ALTER TABLE IF EXISTS ONLY axionpro."TenantEmployeeSectionDefault" DROP CONSTRAINT IF EXISTS "TenantEmployeeSectionDefault_TenantId_fkey";
ALTER TABLE IF EXISTS ONLY axionpro."TenantBillingSubscription" DROP CONSTRAINT IF EXISTS "TenantBillingSubscription_TenantSubscriptionId_fkey";
ALTER TABLE IF EXISTS ONLY axionpro."TenantBillingSubscription" DROP CONSTRAINT IF EXISTS "TenantBillingSubscription_TenantId_fkey";
ALTER TABLE IF EXISTS ONLY axionpro."TenantBillingSubscription" DROP CONSTRAINT IF EXISTS "TenantBillingSubscription_SubscriptionPlanPriceId_fkey";
ALTER TABLE IF EXISTS ONLY axionpro."TenantBillingSubscription" DROP CONSTRAINT IF EXISTS "TenantBillingSubscription_PaymentGatewayId_fkey";
ALTER TABLE IF EXISTS ONLY axionpro."TenantBillingProfile" DROP CONSTRAINT IF EXISTS "TenantBillingProfile_TenantId_fkey";
ALTER TABLE IF EXISTS ONLY axionpro."SubscriptionPlanPrice" DROP CONSTRAINT IF EXISTS "SubscriptionPlanPrice_SubscriptionPlanId_fkey";
ALTER TABLE IF EXISTS ONLY axionpro."PaymentWebhookEvent" DROP CONSTRAINT IF EXISTS "PaymentWebhookEvent_PaymentGatewayId_fkey";
ALTER TABLE IF EXISTS ONLY axionpro."PaymentTransaction" DROP CONSTRAINT IF EXISTS "PaymentTransaction_BillingOrderId_fkey";
ALTER TABLE IF EXISTS ONLY axionpro."PaymentAttempt" DROP CONSTRAINT IF EXISTS "PaymentAttempt_BillingOrderId_fkey";
ALTER TABLE IF EXISTS ONLY axionpro."HostBillingConfiguration" DROP CONSTRAINT IF EXISTS "HostBillingConfiguration_PaymentGatewayId_fkey";
ALTER TABLE IF EXISTS ONLY axionpro."Tender" DROP CONSTRAINT IF EXISTS "FK__Tender__TenderSt__3EDC53F0";
ALTER TABLE IF EXISTS ONLY axionpro."State" DROP CONSTRAINT IF EXISTS "FK__State__CountryId__6B0FDBE9";
ALTER TABLE IF EXISTS ONLY axionpro."LeaveRequest" DROP CONSTRAINT IF EXISTS "FK__LeaveRequ__Tenan__04FA9675";
ALTER TABLE IF EXISTS ONLY axionpro."LeaveRequest" DROP CONSTRAINT IF EXISTS "FK__LeaveRequ__Leave__06E2DEE7";
ALTER TABLE IF EXISTS ONLY axionpro."LeaveRequest" DROP CONSTRAINT IF EXISTS "FK__LeaveRequ__Emplo__05EEBAAE";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeLeavePolicyMapping" DROP CONSTRAINT IF EXISTS "FK__EmployeeL__Tenan__72DBE63A";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeLeavePolicyMapping" DROP CONSTRAINT IF EXISTS "FK__EmployeeL__Emplo__73D00A73";
ALTER TABLE IF EXISTS ONLY axionpro."EmailQueue" DROP CONSTRAINT IF EXISTS "FK__EmailQueu__Templ__44B528D7";
ALTER TABLE IF EXISTS ONLY axionpro."Locality" DROP CONSTRAINT IF EXISTS "FK__City__StateId__6EE06CCD";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeWorkModeOverrideRequest" DROP CONSTRAINT IF EXISTS "FK_WorkModeOverride_WorkArrangement";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeWorkModeOverrideRequest" DROP CONSTRAINT IF EXISTS "FK_WorkModeOverride_TenantLocation";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeWorkModeOverrideRequest" DROP CONSTRAINT IF EXISTS "FK_WorkModeOverride_Tenant";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeWorkModeOverrideRequest" DROP CONSTRAINT IF EXISTS "FK_WorkModeOverride_Employee";
ALTER TABLE IF EXISTS ONLY axionpro."UserRole" DROP CONSTRAINT IF EXISTS "FK_UserRole_Role";
ALTER TABLE IF EXISTS ONLY axionpro."UserAttendanceSetting" DROP CONSTRAINT IF EXISTS "FK_UserAttendanceSetting_Employee";
ALTER TABLE IF EXISTS ONLY axionpro."UserAttendanceSetting" DROP CONSTRAINT IF EXISTS "FK_UserAttendanceSetting_AttendanceDeviceType";
ALTER TABLE IF EXISTS ONLY axionpro."Ticket" DROP CONSTRAINT IF EXISTS "FK_Ticket_TicketType";
ALTER TABLE IF EXISTS ONLY axionpro."Ticket" DROP CONSTRAINT IF EXISTS "FK_Ticket_Tenant";
ALTER TABLE IF EXISTS ONLY axionpro."Ticket" DROP CONSTRAINT IF EXISTS "FK_Ticket_RequestedFor";
ALTER TABLE IF EXISTS ONLY axionpro."Ticket" DROP CONSTRAINT IF EXISTS "FK_Ticket_RequestedBy";
ALTER TABLE IF EXISTS ONLY axionpro."Ticket" DROP CONSTRAINT IF EXISTS "FK_Ticket_RecommendedBy";
ALTER TABLE IF EXISTS ONLY axionpro."Ticket" DROP CONSTRAINT IF EXISTS "FK_Ticket_Header";
ALTER TABLE IF EXISTS ONLY axionpro."Ticket" DROP CONSTRAINT IF EXISTS "FK_Ticket_Classification";
ALTER TABLE IF EXISTS ONLY axionpro."Ticket" DROP CONSTRAINT IF EXISTS "FK_Ticket_AssignedUser";
ALTER TABLE IF EXISTS ONLY axionpro."Ticket" DROP CONSTRAINT IF EXISTS "FK_Ticket_AssignedRole";
ALTER TABLE IF EXISTS ONLY axionpro."Ticket" DROP CONSTRAINT IF EXISTS "FK_Ticket_ApprovedBy";
ALTER TABLE IF EXISTS ONLY axionpro."TicketType" DROP CONSTRAINT IF EXISTS "FK_TicketType_Tenant";
ALTER TABLE IF EXISTS ONLY axionpro."TicketType" DROP CONSTRAINT IF EXISTS "FK_TicketType_ResponsibleRole";
ALTER TABLE IF EXISTS ONLY axionpro."TicketType" DROP CONSTRAINT IF EXISTS "FK_TicketType_Header";
ALTER TABLE IF EXISTS ONLY axionpro."TicketType" DROP CONSTRAINT IF EXISTS "FK_TicketType_ApprovalRole";
ALTER TABLE IF EXISTS ONLY axionpro."TicketHistory" DROP CONSTRAINT IF EXISTS "FK_TicketHistory_User";
ALTER TABLE IF EXISTS ONLY axionpro."TicketHistory" DROP CONSTRAINT IF EXISTS "FK_TicketHistory_Ticket";
ALTER TABLE IF EXISTS ONLY axionpro."TicketHeader" DROP CONSTRAINT IF EXISTS "FK_TicketHeader_TenantId";
ALTER TABLE IF EXISTS ONLY axionpro."TicketHeader" DROP CONSTRAINT IF EXISTS "FK_TicketHeaderType_TicketClassification";
ALTER TABLE IF EXISTS ONLY axionpro."TicketClassification" DROP CONSTRAINT IF EXISTS "FK_TicketClassification_Tenant";
ALTER TABLE IF EXISTS ONLY axionpro."TicketAttachment" DROP CONSTRAINT IF EXISTS "FK_TicketAttachment_User";
ALTER TABLE IF EXISTS ONLY axionpro."ThreadMessage" DROP CONSTRAINT IF EXISTS "FK_ThreadMessage_Thread";
ALTER TABLE IF EXISTS ONLY axionpro."ThreadMessage" DROP CONSTRAINT IF EXISTS "FK_ThreadMessage_Employee";
ALTER TABLE IF EXISTS ONLY axionpro."Tender" DROP CONSTRAINT IF EXISTS "FK_Tender_ClientId";
ALTER TABLE IF EXISTS ONLY axionpro."Tenant" DROP CONSTRAINT IF EXISTS "FK_Tenant_TenantIndustry";
ALTER TABLE IF EXISTS ONLY axionpro."TenantSubscription" DROP CONSTRAINT IF EXISTS "FK_TenantSubscription_Tenant";
ALTER TABLE IF EXISTS ONLY axionpro."TenantSubscription" DROP CONSTRAINT IF EXISTS "FK_TenantSubscription_Plan";
ALTER TABLE IF EXISTS ONLY axionpro."TenantProfile" DROP CONSTRAINT IF EXISTS "FK_TenantProfile_Tenant";
ALTER TABLE IF EXISTS ONLY axionpro."TenantLocation" DROP CONSTRAINT IF EXISTS "FK_TenantLocation_Tenant";
ALTER TABLE IF EXISTS ONLY axionpro."TenantLocation" DROP CONSTRAINT IF EXISTS "FK_TenantLocation_District";
ALTER TABLE IF EXISTS ONLY axionpro."TenantLocation" DROP CONSTRAINT IF EXISTS "FK_TenantLocation_Country";
ALTER TABLE IF EXISTS ONLY axionpro."TenantLocation" DROP CONSTRAINT IF EXISTS "FK_TenantLocation_City";
ALTER TABLE IF EXISTS ONLY axionpro."TenantEnabledOperation" DROP CONSTRAINT IF EXISTS "FK_TenantEnabledOperations_Tenant";
ALTER TABLE IF EXISTS ONLY axionpro."TenantEnabledOperation" DROP CONSTRAINT IF EXISTS "FK_TenantEnabledOperations_Operation";
ALTER TABLE IF EXISTS ONLY axionpro."TenantEnabledOperation" DROP CONSTRAINT IF EXISTS "FK_TenantEnabledOperations_Module";
ALTER TABLE IF EXISTS ONLY axionpro."TenantEnabledModule" DROP CONSTRAINT IF EXISTS "FK_TenantEnabledModules_Tenant";
ALTER TABLE IF EXISTS ONLY axionpro."TenantEnabledModule" DROP CONSTRAINT IF EXISTS "FK_TenantEnabledModules_Module";
ALTER TABLE IF EXISTS ONLY axionpro."TenantEmailTemplate" DROP CONSTRAINT IF EXISTS "FK_TenantEmailTemplate_Tenant";
ALTER TABLE IF EXISTS ONLY axionpro."TenantEmailConfig" DROP CONSTRAINT IF EXISTS "FK_TenantEmailConfig_Tenant";
ALTER TABLE IF EXISTS ONLY axionpro."TenantDevice" DROP CONSTRAINT IF EXISTS "FK_TenantDevice_TenantLocation";
ALTER TABLE IF EXISTS ONLY axionpro."TenantDevice" DROP CONSTRAINT IF EXISTS "FK_TenantDevice_Tenant";
ALTER TABLE IF EXISTS ONLY axionpro."TenantDevice" DROP CONSTRAINT IF EXISTS "FK_TenantDevice_DeviceMaster";
ALTER TABLE IF EXISTS ONLY axionpro."TenantDeviceConfiguration" DROP CONSTRAINT IF EXISTS "FK_TenantDeviceConfiguration_TenantDevice";
ALTER TABLE IF EXISTS ONLY axionpro."TenantCardMaster" DROP CONSTRAINT IF EXISTS "FK_TenantCardMaster_Tenant";
ALTER TABLE IF EXISTS ONLY axionpro."TaxSystemMaster" DROP CONSTRAINT IF EXISTS "FK_TaxSystem_Country";
ALTER TABLE IF EXISTS ONLY axionpro."TaxSlab" DROP CONSTRAINT IF EXISTS "FK_TaxSlab_Tenant";
ALTER TABLE IF EXISTS ONLY axionpro."TaxSlab" DROP CONSTRAINT IF EXISTS "FK_TaxSlab_State";
ALTER TABLE IF EXISTS ONLY axionpro."TaxSlab" DROP CONSTRAINT IF EXISTS "FK_TaxSlab_Regime";
ALTER TABLE IF EXISTS ONLY axionpro."TaxSlab" DROP CONSTRAINT IF EXISTS "FK_TaxSlab_Country";
ALTER TABLE IF EXISTS ONLY axionpro."TaxRule" DROP CONSTRAINT IF EXISTS "FK_TaxRule_Tenant";
ALTER TABLE IF EXISTS ONLY axionpro."TaxRule" DROP CONSTRAINT IF EXISTS "FK_TaxRule_Country";
ALTER TABLE IF EXISTS ONLY axionpro."TaxRegimeMaster" DROP CONSTRAINT IF EXISTS "FK_TaxRegime_TaxSystem";
ALTER TABLE IF EXISTS ONLY axionpro."TaxRegimeMaster" DROP CONSTRAINT IF EXISTS "FK_TaxRegime_Country";
ALTER TABLE IF EXISTS ONLY axionpro."SalaryStructureDetail" DROP CONSTRAINT IF EXISTS "FK_StructureDetail_Structure";
ALTER TABLE IF EXISTS ONLY axionpro."SalaryStructureDetail" DROP CONSTRAINT IF EXISTS "FK_StructureDetail_Dependency";
ALTER TABLE IF EXISTS ONLY axionpro."SalaryStructureDetail" DROP CONSTRAINT IF EXISTS "FK_StructureDetail_Component";
ALTER TABLE IF EXISTS ONLY axionpro."StatutoryType" DROP CONSTRAINT IF EXISTS "FK_StatutoryType_Country";
ALTER TABLE IF EXISTS ONLY axionpro."SalaryStructure" DROP CONSTRAINT IF EXISTS "FK_SalaryStructure_Tenant";
ALTER TABLE IF EXISTS ONLY axionpro."SalaryComponentMaster" DROP CONSTRAINT IF EXISTS "FK_SalaryComponent_Tenant";
ALTER TABLE IF EXISTS ONLY axionpro."SalaryComponentMaster" DROP CONSTRAINT IF EXISTS "FK_SalaryComponent_State";
ALTER TABLE IF EXISTS ONLY axionpro."SalaryComponentMaster" DROP CONSTRAINT IF EXISTS "FK_SalaryComponent_Dependency";
ALTER TABLE IF EXISTS ONLY axionpro."SalaryComponentMaster" DROP CONSTRAINT IF EXISTS "FK_SalaryComponent_Country";
ALTER TABLE IF EXISTS ONLY axionpro."SalaryComponentMaster" DROP CONSTRAINT IF EXISTS "FK_SalaryComponent_Compliance";
ALTER TABLE IF EXISTS ONLY axionpro."UserRole" DROP CONSTRAINT IF EXISTS "FK_Role_Employee";
ALTER TABLE IF EXISTS ONLY axionpro."RoleModuleAndPermission" DROP CONSTRAINT IF EXISTS "FK_RoleModulePermission_Role";
ALTER TABLE IF EXISTS ONLY axionpro."RoleModuleAndPermission" DROP CONSTRAINT IF EXISTS "FK_RoleModulePermission_Operation";
ALTER TABLE IF EXISTS ONLY axionpro."RoleModuleAndPermission" DROP CONSTRAINT IF EXISTS "FK_RoleModulePermission_Module";
ALTER TABLE IF EXISTS ONLY axionpro."RequestType" DROP CONSTRAINT IF EXISTS "FK_RequestType_Tenant";
ALTER TABLE IF EXISTS ONLY axionpro."RefreshToken" DROP CONSTRAINT IF EXISTS "FK_RefreshToken_LoginCredential";
ALTER TABLE IF EXISTS ONLY axionpro."RefreshToken" DROP CONSTRAINT IF EXISTS "FK_RefreshToken_HostUser";
ALTER TABLE IF EXISTS ONLY axionpro."Policy" DROP CONSTRAINT IF EXISTS "FK_Policy_Type";
ALTER TABLE IF EXISTS ONLY axionpro."Policy" DROP CONSTRAINT IF EXISTS "FK_Policy_Tenant";
ALTER TABLE IF EXISTS ONLY axionpro."Policy" DROP CONSTRAINT IF EXISTS "FK_Policy_OwnerDepartment";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyVersion" DROP CONSTRAINT IF EXISTS "FK_PolicyVersion_Tenant";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyVersion" DROP CONSTRAINT IF EXISTS "FK_PolicyVersion_Status";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyVersion" DROP CONSTRAINT IF EXISTS "FK_PolicyVersion_Policy";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyType" DROP CONSTRAINT IF EXISTS "FK_PolicyType_Tenant";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyType" DROP CONSTRAINT IF EXISTS "FK_PolicyType_Category";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyRule" DROP CONSTRAINT IF EXISTS "FK_PolicyRule_Version";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyRule" DROP CONSTRAINT IF EXISTS "FK_PolicyRule_Type";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyRule" DROP CONSTRAINT IF EXISTS "FK_PolicyRule_Tenant";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyException" DROP CONSTRAINT IF EXISTS "FK_PolicyException_Version";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyException" DROP CONSTRAINT IF EXISTS "FK_PolicyException_Tenant";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyException" DROP CONSTRAINT IF EXISTS "FK_PolicyException_Status";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyException" DROP CONSTRAINT IF EXISTS "FK_PolicyException_Employee";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyDocument" DROP CONSTRAINT IF EXISTS "FK_PolicyDocument_Version";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyDocument" DROP CONSTRAINT IF EXISTS "FK_PolicyDocument_Type";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyDocument" DROP CONSTRAINT IF EXISTS "FK_PolicyDocument_Tenant";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyChangeAudit" DROP CONSTRAINT IF EXISTS "FK_PolicyChangeAudit_Version";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyChangeAudit" DROP CONSTRAINT IF EXISTS "FK_PolicyChangeAudit_Tenant";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyChangeAudit" DROP CONSTRAINT IF EXISTS "FK_PolicyChangeAudit_Policy";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyAssignment" DROP CONSTRAINT IF EXISTS "FK_PolicyAssignment_Version";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyAssignment" DROP CONSTRAINT IF EXISTS "FK_PolicyAssignment_Tenant";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyAssignment" DROP CONSTRAINT IF EXISTS "FK_PolicyAssignment_Employee";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyAssignment" DROP CONSTRAINT IF EXISTS "FK_PolicyAssignment_Applicability";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyApprovalStage" DROP CONSTRAINT IF EXISTS "FK_PolicyApprovalStage_Tenant";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyApprovalStage" DROP CONSTRAINT IF EXISTS "FK_PolicyApprovalStage_Role";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyApprovalStage" DROP CONSTRAINT IF EXISTS "FK_PolicyApprovalStage_Category";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyApprovalHistory" DROP CONSTRAINT IF EXISTS "FK_PolicyApprovalHistory_Version";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyApprovalHistory" DROP CONSTRAINT IF EXISTS "FK_PolicyApprovalHistory_Tenant";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyApprovalHistory" DROP CONSTRAINT IF EXISTS "FK_PolicyApprovalHistory_Stage";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyApplicability" DROP CONSTRAINT IF EXISTS "FK_PolicyApplicability_Version";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyApplicability" DROP CONSTRAINT IF EXISTS "FK_PolicyApplicability_TenantLocation";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyApplicability" DROP CONSTRAINT IF EXISTS "FK_PolicyApplicability_Tenant";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyApplicability" DROP CONSTRAINT IF EXISTS "FK_PolicyApplicability_State";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyApplicability" DROP CONSTRAINT IF EXISTS "FK_PolicyApplicability_Locality";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyApplicability" DROP CONSTRAINT IF EXISTS "FK_PolicyApplicability_Gender";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyApplicability" DROP CONSTRAINT IF EXISTS "FK_PolicyApplicability_EmployeeType";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyApplicability" DROP CONSTRAINT IF EXISTS "FK_PolicyApplicability_Employee";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyApplicability" DROP CONSTRAINT IF EXISTS "FK_PolicyApplicability_District";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyApplicability" DROP CONSTRAINT IF EXISTS "FK_PolicyApplicability_Designation";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyApplicability" DROP CONSTRAINT IF EXISTS "FK_PolicyApplicability_Department";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyApplicability" DROP CONSTRAINT IF EXISTS "FK_PolicyApplicability_Country";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyAcknowledgement" DROP CONSTRAINT IF EXISTS "FK_PolicyAcknowledgement_Version";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyAcknowledgement" DROP CONSTRAINT IF EXISTS "FK_PolicyAcknowledgement_Tenant";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyAcknowledgement" DROP CONSTRAINT IF EXISTS "FK_PolicyAcknowledgement_Employee";
ALTER TABLE IF EXISTS ONLY axionpro."PlanModuleMapping" DROP CONSTRAINT IF EXISTS "FK_PlanModuleMapping_Module";
ALTER TABLE IF EXISTS ONLY axionpro."PayrollRun" DROP CONSTRAINT IF EXISTS "FK_PayrollRun_Tenant";
ALTER TABLE IF EXISTS ONLY axionpro."PayrollEmployee" DROP CONSTRAINT IF EXISTS "FK_PayrollEmployee_Run";
ALTER TABLE IF EXISTS ONLY axionpro."PayrollEmployee" DROP CONSTRAINT IF EXISTS "FK_PayrollEmployee_Employee";
ALTER TABLE IF EXISTS ONLY axionpro."PayrollEmployeeDetail" DROP CONSTRAINT IF EXISTS "FK_PayrollDetail_Employee";
ALTER TABLE IF EXISTS ONLY axionpro."PayrollEmployeeDetail" DROP CONSTRAINT IF EXISTS "FK_PayrollDetail_Component";
ALTER TABLE IF EXISTS ONLY axionpro."PlanModuleMapping" DROP CONSTRAINT IF EXISTS "FK_PMM_SubscriptionPlan";
ALTER TABLE IF EXISTS ONLY axionpro."Module" DROP CONSTRAINT IF EXISTS "FK_Module_ParentModule";
ALTER TABLE IF EXISTS ONLY axionpro."ModuleOperationMapping" DROP CONSTRAINT IF EXISTS "FK_ModuleOperation_Operation";
ALTER TABLE IF EXISTS ONLY axionpro."ModuleOperationMapping" DROP CONSTRAINT IF EXISTS "FK_ModuleOperationMapping_PageTypeEnum";
ALTER TABLE IF EXISTS ONLY axionpro."ModuleOperationMapping" DROP CONSTRAINT IF EXISTS "FK_ModuleOperationMapping_Module";
ALTER TABLE IF EXISTS ONLY axionpro."ModuleOperationMapping" DROP CONSTRAINT IF EXISTS "FK_ModuleOperationMapping_DataViewStructure";
ALTER TABLE IF EXISTS ONLY axionpro."LoginCredential" DROP CONSTRAINT IF EXISTS "FK_LoginCredential_Employee";
ALTER TABLE IF EXISTS ONLY axionpro."Locality" DROP CONSTRAINT IF EXISTS "FK_Locality_LocalityType";
ALTER TABLE IF EXISTS ONLY axionpro."Locality" DROP CONSTRAINT IF EXISTS "FK_Locality_District";
ALTER TABLE IF EXISTS ONLY axionpro."LeaveType" DROP CONSTRAINT IF EXISTS "FK_LeaveType_TenantId";
ALTER TABLE IF EXISTS ONLY axionpro."IdentityCategoryDocument" DROP CONSTRAINT IF EXISTS "FK_IdentityDocument_Category";
ALTER TABLE IF EXISTS ONLY axionpro."HostUser" DROP CONSTRAINT IF EXISTS "FK_HostUser_HostRole";
ALTER TABLE IF EXISTS ONLY axionpro."Holiday" DROP CONSTRAINT IF EXISTS "FK_Holiday_TenantLocation";
ALTER TABLE IF EXISTS ONLY axionpro."Holiday" DROP CONSTRAINT IF EXISTS "FK_Holiday_Tenant";
ALTER TABLE IF EXISTS ONLY axionpro."HostRoleModuleAndPermission" DROP CONSTRAINT IF EXISTS "FK_HRMP_Operation";
ALTER TABLE IF EXISTS ONLY axionpro."HostRoleModuleAndPermission" DROP CONSTRAINT IF EXISTS "FK_HRMP_Module";
ALTER TABLE IF EXISTS ONLY axionpro."HostRoleModuleAndPermission" DROP CONSTRAINT IF EXISTS "FK_HRMP_HostRole";
ALTER TABLE IF EXISTS ONLY axionpro."Tenant" DROP CONSTRAINT IF EXISTS "FK_Gender_Tenant";
ALTER TABLE IF EXISTS ONLY axionpro."Employee" DROP CONSTRAINT IF EXISTS "FK_Employee_Tenant";
ALTER TABLE IF EXISTS ONLY axionpro."Employee" DROP CONSTRAINT IF EXISTS "FK_Employee_Gender";
ALTER TABLE IF EXISTS ONLY axionpro."Employee" DROP CONSTRAINT IF EXISTS "FK_Employee_EmployeeType";
ALTER TABLE IF EXISTS ONLY axionpro."Employee" DROP CONSTRAINT IF EXISTS "FK_Employee_Designation";
ALTER TABLE IF EXISTS ONLY axionpro."Employee" DROP CONSTRAINT IF EXISTS "FK_Employee_Country";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeWorkPattern" DROP CONSTRAINT IF EXISTS "FK_EmployeeWorkPattern_WorkArrangement";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeWorkPattern" DROP CONSTRAINT IF EXISTS "FK_EmployeeWorkPattern_TenantLocation";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeWorkPattern" DROP CONSTRAINT IF EXISTS "FK_EmployeeWorkPattern_Tenant";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeWorkHistory" DROP CONSTRAINT IF EXISTS "FK_EmployeeWorkHistory_Profile";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeWorkDocument" DROP CONSTRAINT IF EXISTS "FK_EmployeeWorkDocument_WorkHistory";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeWorkDocument" DROP CONSTRAINT IF EXISTS "FK_EmployeeWorkDocument_DocumentType";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeWorkArrangement" DROP CONSTRAINT IF EXISTS "FK_EmployeeWorkArrangement_Tenant";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeWorkArrangement" DROP CONSTRAINT IF EXISTS "FK_EmployeeWorkArrangement_PrimaryLocation";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeWorkArrangement" DROP CONSTRAINT IF EXISTS "FK_EmployeeWorkArrangement_PolicyVersion";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeWorkArrangement" DROP CONSTRAINT IF EXISTS "FK_EmployeeWorkArrangement_Employee";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeWorkArrangement" DROP CONSTRAINT IF EXISTS "FK_EmployeeWorkArrangement_AttendancePolicy";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeTypeBasicMenu" DROP CONSTRAINT IF EXISTS "FK_EmployeeTypeBasicMenu_EmployeeType";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeTaxProfile" DROP CONSTRAINT IF EXISTS "FK_EmployeeTaxProfile_State";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeTaxProfile" DROP CONSTRAINT IF EXISTS "FK_EmployeeTaxProfile_Regime";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeTaxProfile" DROP CONSTRAINT IF EXISTS "FK_EmployeeTaxProfile_Employee";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeTaxProfile" DROP CONSTRAINT IF EXISTS "FK_EmployeeTaxProfile_Country";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeStatutoryAccount" DROP CONSTRAINT IF EXISTS "FK_EmployeeStatutoryAccount_Statutory";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeesChangedTypeHistory" DROP CONSTRAINT IF EXISTS "FK_EmployeeStatusHistory_OldEmployeeType";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeesChangedTypeHistory" DROP CONSTRAINT IF EXISTS "FK_EmployeeStatusHistory_NewEmployeeType";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeesChangedTypeHistory" DROP CONSTRAINT IF EXISTS "FK_EmployeeStatusHistory_Employee";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeSalary" DROP CONSTRAINT IF EXISTS "FK_EmployeeSalary_Structure";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeSalary" DROP CONSTRAINT IF EXISTS "FK_EmployeeSalary_Employee";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeePolicyEnrollment" DROP CONSTRAINT IF EXISTS "FK_EmployeePolicyEnrollment_Employee";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeePersonalDetail" DROP CONSTRAINT IF EXISTS "FK_EmployeePersonalDetail_Employee";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeManagerMapping" DROP CONSTRAINT IF EXISTS "FK_EmployeeManagerMapping_Tenant";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeManagerMapping" DROP CONSTRAINT IF EXISTS "FK_EmployeeManagerMapping_ReportingType";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeManagerMapping" DROP CONSTRAINT IF EXISTS "FK_EmployeeManagerMapping_Manager";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeManagerMapping" DROP CONSTRAINT IF EXISTS "FK_EmployeeManagerMapping_Employee";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeManagerMapping" DROP CONSTRAINT IF EXISTS "FK_EmployeeManagerMapping_Designation";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeManagerMapping" DROP CONSTRAINT IF EXISTS "FK_EmployeeManagerMapping_Department";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeLocationAssignment" DROP CONSTRAINT IF EXISTS "FK_EmployeeLocationAssignment_TenantLocation";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeLocationAssignment" DROP CONSTRAINT IF EXISTS "FK_EmployeeLocationAssignment_Tenant";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeLocationAssignment" DROP CONSTRAINT IF EXISTS "FK_EmployeeLocationAssignment_Employee";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeLeaveBalance" DROP CONSTRAINT IF EXISTS "FK_EmployeeLeaveBalance_Tenant";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeLeaveBalance" DROP CONSTRAINT IF EXISTS "FK_EmployeeLeaveBalance_EmployeeLeavePolicyMapping";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeImage" DROP CONSTRAINT IF EXISTS "FK_EmployeeImages_Employee";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeIdentity" DROP CONSTRAINT IF EXISTS "FK_EmployeeIdentity_Employee";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeIdentity" DROP CONSTRAINT IF EXISTS "FK_EmployeeIdentity_Document";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeExperience" DROP CONSTRAINT IF EXISTS "FK_EmployeeExperience_Employee";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeExperienceDocument" DROP CONSTRAINT IF EXISTS "FK_EmployeeExperienceDocument_Experience";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeDeviceEnrollment" DROP CONSTRAINT IF EXISTS "FK_EmployeeDeviceEnrollment_Tenant";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeDeviceEnrollment" DROP CONSTRAINT IF EXISTS "FK_EmployeeDeviceEnrollment_Employee";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeDependent" DROP CONSTRAINT IF EXISTS "FK_EmployeeDependents_Employee";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeContact" DROP CONSTRAINT IF EXISTS "FK_EmployeeContact_Employee";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeCodePattern" DROP CONSTRAINT IF EXISTS "FK_EmployeeCodePattern_Tenant";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeCategorySkill" DROP CONSTRAINT IF EXISTS "FK_EmployeeCategorySkill_Employee";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeCategorySkill" DROP CONSTRAINT IF EXISTS "FK_EmployeeCategorySkill_Category";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeBankDetail" DROP CONSTRAINT IF EXISTS "FK_EmployeeBank_Employee";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeAttendancePunch" DROP CONSTRAINT IF EXISTS "FK_EmployeeAttendancePunch_Tenant";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeAttendancePunch" DROP CONSTRAINT IF EXISTS "FK_EmployeeAttendancePunch_PolicyVersion";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeAttendancePunch" DROP CONSTRAINT IF EXISTS "FK_EmployeeAttendancePunch_Location";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeAttendancePunch" DROP CONSTRAINT IF EXISTS "FK_EmployeeAttendancePunch_Employee";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeAttendancePunch" DROP CONSTRAINT IF EXISTS "FK_EmployeeAttendancePunch_AttendanceDeviceType";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeAttendancePunch" DROP CONSTRAINT IF EXISTS "FK_EmployeeAttendancePunch_Arrangement";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeDailyAttendance" DROP CONSTRAINT IF EXISTS "FK_Employee";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeePolicyDependentMapping" DROP CONSTRAINT IF EXISTS "FK_EPD_Enrollment";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeePolicyDependentMapping" DROP CONSTRAINT IF EXISTS "FK_EPD_Dependent";
ALTER TABLE IF EXISTS ONLY axionpro."District" DROP CONSTRAINT IF EXISTS "FK_District_State";
ALTER TABLE IF EXISTS ONLY axionpro."DeviceMessageLog" DROP CONSTRAINT IF EXISTS "FK_DeviceMessageLog_TenantDevice";
ALTER TABLE IF EXISTS ONLY axionpro."DeviceMessageLog" DROP CONSTRAINT IF EXISTS "FK_DeviceMessageLog_Tenant";
ALTER TABLE IF EXISTS ONLY axionpro."DeviceInitialProvisioning" DROP CONSTRAINT IF EXISTS "FK_DeviceInitialProvisioning_DeviceMaster";
ALTER TABLE IF EXISTS ONLY axionpro."DeviceCredential" DROP CONSTRAINT IF EXISTS "FK_DeviceCredential_TenantDevice";
ALTER TABLE IF EXISTS ONLY axionpro."DeviceCommand" DROP CONSTRAINT IF EXISTS "FK_DeviceCommand_TenantLocation";
ALTER TABLE IF EXISTS ONLY axionpro."DeviceCommand" DROP CONSTRAINT IF EXISTS "FK_DeviceCommand_TenantDevice";
ALTER TABLE IF EXISTS ONLY axionpro."DeviceCommand" DROP CONSTRAINT IF EXISTS "FK_DeviceCommand_Tenant";
ALTER TABLE IF EXISTS ONLY axionpro."DeviceCommandResponse" DROP CONSTRAINT IF EXISTS "FK_DeviceCommandResponse_TenantDevice";
ALTER TABLE IF EXISTS ONLY axionpro."DeviceCommandResponse" DROP CONSTRAINT IF EXISTS "FK_DeviceCommandResponse_Tenant";
ALTER TABLE IF EXISTS ONLY axionpro."DeviceCommandResponse" DROP CONSTRAINT IF EXISTS "FK_DeviceCommandResponse_DeviceCommand";
ALTER TABLE IF EXISTS ONLY axionpro."Designation" DROP CONSTRAINT IF EXISTS "FK_Designation_Tenant";
ALTER TABLE IF EXISTS ONLY axionpro."Designation" DROP CONSTRAINT IF EXISTS "FK_Designation_Department";
ALTER TABLE IF EXISTS ONLY axionpro."Department" DROP CONSTRAINT IF EXISTS "FK_Department_Tenant";
ALTER TABLE IF EXISTS ONLY axionpro."DayCombination" DROP CONSTRAINT IF EXISTS "FK_DayCombination_Tenant";
ALTER TABLE IF EXISTS ONLY axionpro."CountryIdentityRule" DROP CONSTRAINT IF EXISTS "FK_Country_Document";
ALTER TABLE IF EXISTS ONLY axionpro."CountryStatutoryRule" DROP CONSTRAINT IF EXISTS "FK_CountryStatutoryRule_Statutory";
ALTER TABLE IF EXISTS ONLY axionpro."CountryStatutoryRule" DROP CONSTRAINT IF EXISTS "FK_CountryStatutoryRule_Country";
ALTER TABLE IF EXISTS ONLY axionpro."CountryIdentityRule" DROP CONSTRAINT IF EXISTS "FK_Country";
ALTER TABLE IF EXISTS ONLY axionpro."ComplianceRule" DROP CONSTRAINT IF EXISTS "FK_ComplianceRule_Tenant";
ALTER TABLE IF EXISTS ONLY axionpro."ComplianceRule" DROP CONSTRAINT IF EXISTS "FK_ComplianceRule_State";
ALTER TABLE IF EXISTS ONLY axionpro."ComplianceRule" DROP CONSTRAINT IF EXISTS "FK_ComplianceRule_Country";
ALTER TABLE IF EXISTS ONLY axionpro."ComplianceRule" DROP CONSTRAINT IF EXISTS "FK_ComplianceRule_ComplianceType";
ALTER TABLE IF EXISTS ONLY axionpro."Client" DROP CONSTRAINT IF EXISTS "FK_Client_ClientType";
ALTER TABLE IF EXISTS ONLY axionpro."Category" DROP CONSTRAINT IF EXISTS "FK_Category_Parent";
ALTER TABLE IF EXISTS ONLY axionpro."AttendancePolicy" DROP CONSTRAINT IF EXISTS "FK_AttendancePolicy_Tenant";
ALTER TABLE IF EXISTS ONLY axionpro."AttendancePolicyVersionConfiguration" DROP CONSTRAINT IF EXISTS "FK_AttendancePolicyVersionConfiguration_Tenant";
ALTER TABLE IF EXISTS ONLY axionpro."AttendancePolicyVersionConfiguration" DROP CONSTRAINT IF EXISTS "FK_AttendancePolicyVersionConfiguration_PolicyVersion";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeDailyAttendance" DROP CONSTRAINT IF EXISTS "FK_AttendanceDeviceType";
ALTER TABLE IF EXISTS ONLY axionpro."Asset" DROP CONSTRAINT IF EXISTS "FK_Asset_AssetType";
ALTER TABLE IF EXISTS ONLY axionpro."Asset" DROP CONSTRAINT IF EXISTS "FK_Asset_AssetStatus";
ALTER TABLE IF EXISTS ONLY axionpro."AssetType" DROP CONSTRAINT IF EXISTS "FK_AssetType_AssetCategory";
ALTER TABLE IF EXISTS ONLY axionpro."AssetImage" DROP CONSTRAINT IF EXISTS "FK_AssetImage_Asset";
ALTER TABLE IF EXISTS ONLY axionpro."AssetCategory" DROP CONSTRAINT IF EXISTS "FK_AssetCategory_Tenant";
ALTER TABLE IF EXISTS ONLY axionpro."AssetAssignment" DROP CONSTRAINT IF EXISTS "FK_AssetAssignment_Request";
ALTER TABLE IF EXISTS ONLY axionpro."AssetAssignment" DROP CONSTRAINT IF EXISTS "FK_AssetAssignment_Asset";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeType" DROP CONSTRAINT IF EXISTS "EmployeeType_TenantId_fkey";
ALTER TABLE IF EXISTS ONLY axionpro."BillingSubscriptionChange" DROP CONSTRAINT IF EXISTS "BillingSubscriptionChange_ToSubscriptionPlanPriceId_fkey";
ALTER TABLE IF EXISTS ONLY axionpro."BillingSubscriptionChange" DROP CONSTRAINT IF EXISTS "BillingSubscriptionChange_TenantBillingSubscriptionId_fkey";
ALTER TABLE IF EXISTS ONLY axionpro."BillingSubscriptionChange" DROP CONSTRAINT IF EXISTS "BillingSubscriptionChange_FromSubscriptionPlanPriceId_fkey";
ALTER TABLE IF EXISTS ONLY axionpro."BillingRefund" DROP CONSTRAINT IF EXISTS "BillingRefund_PaymentTransactionId_fkey";
ALTER TABLE IF EXISTS ONLY axionpro."BillingOrder" DROP CONSTRAINT IF EXISTS "BillingOrder_TenantId_fkey";
ALTER TABLE IF EXISTS ONLY axionpro."BillingOrder" DROP CONSTRAINT IF EXISTS "BillingOrder_TenantBillingSubscriptionId_fkey";
ALTER TABLE IF EXISTS ONLY axionpro."BillingOrder" DROP CONSTRAINT IF EXISTS "BillingOrder_SubscriptionPlanPriceId_fkey";
ALTER TABLE IF EXISTS ONLY axionpro."BillingOrder" DROP CONSTRAINT IF EXISTS "BillingOrder_PaymentGatewayId_fkey";
ALTER TABLE IF EXISTS ONLY axionpro."BillingInvoice" DROP CONSTRAINT IF EXISTS "BillingInvoice_TenantId_fkey";
ALTER TABLE IF EXISTS ONLY axionpro."BillingInvoice" DROP CONSTRAINT IF EXISTS "BillingInvoice_BillingOrderId_fkey";
ALTER TABLE IF EXISTS ONLY axionpro."BillingInvoiceLine" DROP CONSTRAINT IF EXISTS "BillingInvoiceLine_BillingInvoiceId_fkey";
DROP TRIGGER IF EXISTS "TR_Module_PageNameImmutable" ON axionpro."Module";
DROP TRIGGER IF EXISTS "TR_EmployeeType_Tenant" ON axionpro."EmployeesChangedTypeHistory";
DROP TRIGGER IF EXISTS "TR_EmployeeType_Tenant" ON axionpro."Employee";
DROP TRIGGER IF EXISTS "TR_EmployeeType_OwnerImmutable" ON axionpro."EmployeeType";
DROP INDEX IF EXISTS axionpro."UX_TenantLocation_TenantId_LocationCode";
DROP INDEX IF EXISTS axionpro."UX_TenantEmailTemplate_Tenant_TemplateCode";
DROP INDEX IF EXISTS axionpro."UX_TenantDevice_TenantId_DeviceCode";
DROP INDEX IF EXISTS axionpro."UX_TenantDevice_DeviceMasterId_Active";
DROP INDEX IF EXISTS axionpro."UX_TenantDeviceConfiguration_PendingHttpsIngressTokenHash";
DROP INDEX IF EXISTS axionpro."UX_TenantDeviceConfiguration_HttpsIngressTokenHash";
DROP INDEX IF EXISTS axionpro."UX_TenantCardMaster_Tenant_CardHash_Live";
DROP INDEX IF EXISTS axionpro."UX_TenantBillingSubscription_Gateway";
DROP INDEX IF EXISTS axionpro."UX_TenantBillingSubscription_Current";
DROP INDEX IF EXISTS axionpro."UX_TenantBillingProfile_Active";
DROP INDEX IF EXISTS axionpro."UX_SubscriptionPlanPrice_Live";
DROP INDEX IF EXISTS axionpro."UX_State_CountryId_StateCode";
DROP INDEX IF EXISTS axionpro."UX_Role_Tenant_Name_Live";
DROP INDEX IF EXISTS axionpro."UX_PolicyVersion_Current";
DROP INDEX IF EXISTS axionpro."UX_PolicyType_Tenant_Code";
DROP INDEX IF EXISTS axionpro."UX_PaymentTransaction_GatewayPayment";
DROP INDEX IF EXISTS axionpro."UX_Module_PageName_CaseInsensitive";
DROP INDEX IF EXISTS axionpro."UX_LoginCredential_NormalizedLogin";
DROP INDEX IF EXISTS axionpro."UX_Locality_DistrictId_LocalityCode";
DROP INDEX IF EXISTS axionpro."UX_HostBillingConfiguration_Active";
DROP INDEX IF EXISTS axionpro."UX_Holiday_Tenant_Location_Date_NotDeleted";
DROP INDEX IF EXISTS axionpro."UX_Employee_Tenant_Code";
DROP INDEX IF EXISTS axionpro."UX_Employee_TenantId_Null_OnlyOnce";
DROP INDEX IF EXISTS axionpro."UX_Employee_TenantIdNullOnce";
DROP INDEX IF EXISTS axionpro."UX_Employee_SystemUser_OnlyOnce";
DROP INDEX IF EXISTS axionpro."UX_EmployeeWorkPattern_Arrangement_Day";
DROP INDEX IF EXISTS axionpro."UX_EmployeeType_Tenant_Name_Live";
DROP INDEX IF EXISTS axionpro."UX_EmployeeDeviceEnrollment_Device_EnrollId";
DROP INDEX IF EXISTS axionpro."UX_EmployeeAttendancePunch_Idempotency";
DROP INDEX IF EXISTS axionpro."UX_District_StateId_DistrictCode";
DROP INDEX IF EXISTS axionpro."UX_DeviceMaster_DeviceCode";
DROP INDEX IF EXISTS axionpro."UX_DeviceMaster_CompanyName_ModelNo";
DROP INDEX IF EXISTS axionpro."UX_DeviceInitialProvisioning_IngressTokenHash";
DROP INDEX IF EXISTS axionpro."UX_DeviceCredential_ActiveType";
DROP INDEX IF EXISTS axionpro."UX_DeviceCommand_OneOutstandingSerial";
DROP INDEX IF EXISTS axionpro."UX_DeviceCommandResponse_DeviceCommand";
DROP INDEX IF EXISTS axionpro."UX_Designation_Tenant_Department_Name_Live";
DROP INDEX IF EXISTS axionpro."UX_Department_Tenant_Name_Live";
DROP INDEX IF EXISTS axionpro."UX_DefaultEmailConfig_OneDefault";
DROP INDEX IF EXISTS axionpro."UX_BillingTaxRule_Live";
DROP INDEX IF EXISTS axionpro."UX_BillingRefund_GatewayRefund";
DROP INDEX IF EXISTS axionpro."UX_BillingOrder_GatewayOrder";
DROP INDEX IF EXISTS axionpro."UX_AttendancePolicy_TenantId_PolicyName";
DROP INDEX IF EXISTS axionpro."UX_AttendancePolicyVersionConfiguration_TenantVersion";
DROP INDEX IF EXISTS axionpro."UX_AttendancePolicyVersionConfiguration_PolicyVersion";
DROP INDEX IF EXISTS axionpro."UX_AttendanceDeviceType_DeviceTypeCode";
DROP INDEX IF EXISTS axionpro."UQ_Tenant_Module_Operation";
DROP INDEX IF EXISTS axionpro."UQ_Structure_Order";
DROP INDEX IF EXISTS axionpro."IX_WorkModeOverride_TenantId";
DROP INDEX IF EXISTS axionpro."IX_WorkModeOverride_Employee_Date";
DROP INDEX IF EXISTS axionpro."IX_WorkModeOverride_EmployeeId";
DROP INDEX IF EXISTS axionpro."IX_WorkModeOverride_ApprovalStatus";
DROP INDEX IF EXISTS axionpro."IX_TenantLocation_TenantId_IsSoftDeleted";
DROP INDEX IF EXISTS axionpro."IX_TenantLocation_TenantId_IsActive";
DROP INDEX IF EXISTS axionpro."IX_TenantLocation_TenantId";
DROP INDEX IF EXISTS axionpro."IX_TenantLocation_StateId";
DROP INDEX IF EXISTS axionpro."IX_TenantLocation_DistrictId";
DROP INDEX IF EXISTS axionpro."IX_TenantLocation_CountryId";
DROP INDEX IF EXISTS axionpro."IX_TenantLocation_CityId";
DROP INDEX IF EXISTS axionpro."IX_TenantDevice_TenantLocationId_IsActive";
DROP INDEX IF EXISTS axionpro."IX_TenantDevice_TenantLocationId";
DROP INDEX IF EXISTS axionpro."IX_TenantDevice_TenantId_TenantLocationId";
DROP INDEX IF EXISTS axionpro."IX_TenantDevice_TenantId_IsActive_IsSoftDeleted";
DROP INDEX IF EXISTS axionpro."IX_TenantDevice_TenantId";
DROP INDEX IF EXISTS axionpro."IX_TenantDevice_DeviceMasterId_IsActive";
DROP INDEX IF EXISTS axionpro."IX_TenantDevice_DeviceMasterId";
DROP INDEX IF EXISTS axionpro."IX_TenantDeviceConfiguration_TenantDeviceId";
DROP INDEX IF EXISTS axionpro."IX_TenantCardMaster_TenantId";
DROP INDEX IF EXISTS axionpro."IX_SubscriptionPlan_IsSoftDeleted";
DROP INDEX IF EXISTS axionpro."IX_RefreshToken_LoginId_UserType";
DROP INDEX IF EXISTS axionpro."IX_RefreshToken_LoginCredentialId";
DROP INDEX IF EXISTS axionpro."IX_RefreshToken_HostUserId";
DROP INDEX IF EXISTS axionpro."IX_PolicyException_Employee";
DROP INDEX IF EXISTS axionpro."IX_PolicyChangeAudit_Policy";
DROP INDEX IF EXISTS axionpro."IX_PolicyAssignment_Employee";
DROP INDEX IF EXISTS axionpro."IX_PolicyApplicability_Resolution";
DROP INDEX IF EXISTS axionpro."IX_PolicyAcknowledgement_Employee";
DROP INDEX IF EXISTS axionpro."IX_Locality_LocalityTypeId";
DROP INDEX IF EXISTS axionpro."IX_Locality_DistrictId";
DROP INDEX IF EXISTS axionpro."IX_HostUser_LoginId";
DROP INDEX IF EXISTS axionpro."IX_Holiday_Location_Date";
DROP INDEX IF EXISTS axionpro."IX_HRMP_OperationId";
DROP INDEX IF EXISTS axionpro."IX_HRMP_ModuleId";
DROP INDEX IF EXISTS axionpro."IX_HRMP_HostRoleId";
DROP INDEX IF EXISTS axionpro."IX_EmployeeWorkPattern_WorkArrangementId";
DROP INDEX IF EXISTS axionpro."IX_EmployeeWorkPattern_TenantLocationId";
DROP INDEX IF EXISTS axionpro."IX_EmployeeWorkArrangement_TenantId";
DROP INDEX IF EXISTS axionpro."IX_EmployeeWorkArrangement_PrimaryLocationId";
DROP INDEX IF EXISTS axionpro."IX_EmployeeWorkArrangement_PolicyVersionId";
DROP INDEX IF EXISTS axionpro."IX_EmployeeWorkArrangement_Employee_EffectiveFrom";
DROP INDEX IF EXISTS axionpro."IX_EmployeeWorkArrangement_EmployeeId";
DROP INDEX IF EXISTS axionpro."IX_EmployeeWorkArrangement_AttendancePolicyId";
DROP INDEX IF EXISTS axionpro."IX_EmployeeLocationAssignment_TenantLocationId";
DROP INDEX IF EXISTS axionpro."IX_EmployeeLocationAssignment_TenantId";
DROP INDEX IF EXISTS axionpro."IX_EmployeeLocationAssignment_Primary_EffectiveFrom";
DROP INDEX IF EXISTS axionpro."IX_EmployeeLocationAssignment_Employee_Location_EffectiveFrom";
DROP INDEX IF EXISTS axionpro."IX_EmployeeLocationAssignment_EmployeeId";
DROP INDEX IF EXISTS axionpro."IX_EmployeeDeviceEnrollment_TenantId";
DROP INDEX IF EXISTS axionpro."IX_EmployeeDeviceEnrollment_TenantDeviceId";
DROP INDEX IF EXISTS axionpro."IX_EmployeeDeviceEnrollment_EmployeeId";
DROP INDEX IF EXISTS axionpro."IX_EmployeeAttendancePunch_DayTimeline";
DROP INDEX IF EXISTS axionpro."IX_EmailQueue_Pending";
DROP INDEX IF EXISTS axionpro."IX_District_StateId";
DROP INDEX IF EXISTS axionpro."IX_District_DistrictName";
DROP INDEX IF EXISTS axionpro."IX_DeviceMessageLog_Topic_PayloadHash";
DROP INDEX IF EXISTS axionpro."IX_DeviceMessageLog_TenantDevice_Occurred";
DROP INDEX IF EXISTS axionpro."IX_DeviceMaster_ModelNo";
DROP INDEX IF EXISTS axionpro."IX_DeviceMaster_IsIntegrationSupported";
DROP INDEX IF EXISTS axionpro."IX_DeviceMaster_IsAttendanceDevice";
DROP INDEX IF EXISTS axionpro."IX_DeviceMaster_IsActive_IsSoftDeleted";
DROP INDEX IF EXISTS axionpro."IX_DeviceMaster_DeviceType";
DROP INDEX IF EXISTS axionpro."IX_DeviceMaster_CompanyName";
DROP INDEX IF EXISTS axionpro."IX_DeviceInitialProvisioning_DeviceMaster_Active";
DROP INDEX IF EXISTS axionpro."IX_DeviceCommand_TenantLocationId";
DROP INDEX IF EXISTS axionpro."IX_DeviceCommand_TenantId";
DROP INDEX IF EXISTS axionpro."IX_DeviceCommand_Dispatch";
DROP INDEX IF EXISTS axionpro."IX_DeviceCommand_DeviceSerial_Order";
DROP INDEX IF EXISTS axionpro."IX_DeviceCommandResponse_TenantDevice_Received";
DROP INDEX IF EXISTS axionpro."IX_BulkImportJob_Owner";
DROP INDEX IF EXISTS axionpro."IX_BulkImportJob_Due";
DROP INDEX IF EXISTS axionpro."IX_AttendancePolicy_TenantId_IsActive_IsSoftDeleted";
DROP INDEX IF EXISTS axionpro."IX_AttendancePolicy_TenantId";
DROP INDEX IF EXISTS axionpro."IX_AttendancePolicy_PolicyTypeId";
DROP INDEX IF EXISTS axionpro."IDX_RolesPermission_RoleId";
DROP INDEX IF EXISTS axionpro."IDX_RolesPermission_OperationId";
ALTER TABLE IF EXISTS ONLY axionpro."DeviceCommand" DROP CONSTRAINT IF EXISTS "UX_DeviceCommand_InternalTrackingId";
ALTER TABLE IF EXISTS ONLY axionpro."DefaultEmailConfig" DROP CONSTRAINT IF EXISTS "UX_DefaultEmailConfig_ConfigName";
ALTER TABLE IF EXISTS ONLY axionpro."Tenant" DROP CONSTRAINT IF EXISTS "UQ__Tenant__F7C944DD7E3D53D9";
ALTER TABLE IF EXISTS ONLY axionpro."IdentityCategory" DROP CONSTRAINT IF EXISTS "UQ__Identity__A25C5AA7EC142E9A";
ALTER TABLE IF EXISTS ONLY axionpro."Gender" DROP CONSTRAINT IF EXISTS "UQ__Gender__F7C177153AF55502";
ALTER TABLE IF EXISTS ONLY axionpro."AssetType" DROP CONSTRAINT IF EXISTS "UQ__AssetTyp__D4E7DFA8692FD5DF";
ALTER TABLE IF EXISTS ONLY axionpro."AssetStatus" DROP CONSTRAINT IF EXISTS "UQ__AssetSta__05E7698A7AA7A28E";
ALTER TABLE IF EXISTS ONLY axionpro."TenantLocation" DROP CONSTRAINT IF EXISTS "UQ_TenantLocation_Id_TenantId";
ALTER TABLE IF EXISTS ONLY axionpro."TenantEncryptionKeys" DROP CONSTRAINT IF EXISTS "UQ_TenantKey_TenantId";
ALTER TABLE IF EXISTS ONLY axionpro."TenantDeviceConfiguration" DROP CONSTRAINT IF EXISTS "UQ_TenantDeviceConfiguration_TenantDeviceId";
ALTER TABLE IF EXISTS ONLY axionpro."State" DROP CONSTRAINT IF EXISTS "UQ_State_Country_Name";
ALTER TABLE IF EXISTS ONLY axionpro."SalaryComponentMaster" DROP CONSTRAINT IF EXISTS "UQ_SalaryComponent_Tenant_ComponentName_Effective";
ALTER TABLE IF EXISTS ONLY axionpro."RequestType" DROP CONSTRAINT IF EXISTS "UQ_RequestType_Tenant";
ALTER TABLE IF EXISTS ONLY axionpro."Policy" DROP CONSTRAINT IF EXISTS "UQ_Policy_Tenant_Code";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyVersion" DROP CONSTRAINT IF EXISTS "UQ_PolicyVersion_Policy_Version";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyStatus" DROP CONSTRAINT IF EXISTS "UQ_PolicyStatus_Code";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyRule" DROP CONSTRAINT IF EXISTS "UQ_PolicyRule_Version_Order";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyRuleType" DROP CONSTRAINT IF EXISTS "UQ_PolicyRuleType_Code";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyDocument" DROP CONSTRAINT IF EXISTS "UQ_PolicyDocument_Version_Object";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyDocumentType" DROP CONSTRAINT IF EXISTS "UQ_PolicyDocumentType_Code";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyCategory" DROP CONSTRAINT IF EXISTS "UQ_PolicyCategory_Code";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyAssignment" DROP CONSTRAINT IF EXISTS "UQ_PolicyAssignment_Version_Employee_From";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyApprovalStage" DROP CONSTRAINT IF EXISTS "UQ_PolicyApprovalStage_Tenant_Category_Order";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyApprovalHistory" DROP CONSTRAINT IF EXISTS "UQ_PolicyApprovalHistory_Version_Sequence";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyAcknowledgement" DROP CONSTRAINT IF EXISTS "UQ_PolicyAcknowledgement_Version_Employee";
ALTER TABLE IF EXISTS ONLY axionpro."PayrollRun" DROP CONSTRAINT IF EXISTS "UQ_Payroll_Month";
ALTER TABLE IF EXISTS ONLY axionpro."PayrollEmployee" DROP CONSTRAINT IF EXISTS "UQ_PayrollEmployee";
ALTER TABLE IF EXISTS ONLY axionpro."PaymentWebhookEvent" DROP CONSTRAINT IF EXISTS "UQ_PaymentWebhookEvent_GatewayEvent";
ALTER TABLE IF EXISTS ONLY axionpro."PaymentGateway" DROP CONSTRAINT IF EXISTS "UQ_PaymentGateway_Code";
ALTER TABLE IF EXISTS ONLY axionpro."PaymentAttempt" DROP CONSTRAINT IF EXISTS "UQ_PaymentAttempt_Number";
ALTER TABLE IF EXISTS ONLY axionpro."LoginCredential" DROP CONSTRAINT IF EXISTS "UQ_LoginId";
ALTER TABLE IF EXISTS ONLY axionpro."LocalityType" DROP CONSTRAINT IF EXISTS "UQ_LocalityType_TypeName";
ALTER TABLE IF EXISTS ONLY axionpro."HostUser" DROP CONSTRAINT IF EXISTS "UQ_HostUser_LoginId";
ALTER TABLE IF EXISTS ONLY axionpro."HostRole" DROP CONSTRAINT IF EXISTS "UQ_HostRole_Name";
ALTER TABLE IF EXISTS ONLY axionpro."HostRoleModuleAndPermission" DROP CONSTRAINT IF EXISTS "UQ_HostRole_Module_Operation";
ALTER TABLE IF EXISTS ONLY axionpro."ComplianceTypeMaster" DROP CONSTRAINT IF EXISTS "UQ_ComplianceType_Country_Name";
ALTER TABLE IF EXISTS ONLY axionpro."BillingRefund" DROP CONSTRAINT IF EXISTS "UQ_BillingRefund_Idempotency";
ALTER TABLE IF EXISTS ONLY axionpro."BillingOrder" DROP CONSTRAINT IF EXISTS "UQ_BillingOrder_Idempotency";
ALTER TABLE IF EXISTS ONLY axionpro."BillingInvoice" DROP CONSTRAINT IF EXISTS "UQ_BillingInvoice_Order";
ALTER TABLE IF EXISTS ONLY axionpro."BillingInvoice" DROP CONSTRAINT IF EXISTS "UQ_BillingInvoice_Number";
ALTER TABLE IF EXISTS ONLY axionpro."Ticket" DROP CONSTRAINT IF EXISTS "Ticket_pkey";
ALTER TABLE IF EXISTS ONLY axionpro."TicketHistory" DROP CONSTRAINT IF EXISTS "TicketHistory_pkey";
ALTER TABLE IF EXISTS ONLY axionpro."TicketAttachment" DROP CONSTRAINT IF EXISTS "TicketAttachment_pkey";
ALTER TABLE IF EXISTS ONLY axionpro."TicketThread" DROP CONSTRAINT IF EXISTS "Thread_pkey";
ALTER TABLE IF EXISTS ONLY axionpro."ThreadMessage" DROP CONSTRAINT IF EXISTS "ThreadMessage_pkey";
ALTER TABLE IF EXISTS ONLY axionpro."TenantEmployeeSectionDefault" DROP CONSTRAINT IF EXISTS "TenantEmployeeSectionDefault_pkey";
ALTER TABLE IF EXISTS ONLY axionpro."TenantEmailTemplate" DROP CONSTRAINT IF EXISTS "TenantEmailTemplate_pkey";
ALTER TABLE IF EXISTS ONLY axionpro."TenantDeviceConfiguration" DROP CONSTRAINT IF EXISTS "TenantDeviceConfiguration_pkey";
ALTER TABLE IF EXISTS ONLY axionpro."TenantCardMaster" DROP CONSTRAINT IF EXISTS "TenantCardMaster_pkey";
ALTER TABLE IF EXISTS ONLY axionpro."TenantBillingSubscription" DROP CONSTRAINT IF EXISTS "TenantBillingSubscription_pkey";
ALTER TABLE IF EXISTS ONLY axionpro."TenantBillingProfile" DROP CONSTRAINT IF EXISTS "TenantBillingProfile_pkey";
ALTER TABLE IF EXISTS ONLY axionpro."SubscriptionPlanPrice" DROP CONSTRAINT IF EXISTS "SubscriptionPlanPrice_pkey";
ALTER TABLE IF EXISTS ONLY axionpro."Policy" DROP CONSTRAINT IF EXISTS "Policy_pkey";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyVersion" DROP CONSTRAINT IF EXISTS "PolicyVersion_pkey";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyType" DROP CONSTRAINT IF EXISTS "PolicyType_pkey";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyStatus" DROP CONSTRAINT IF EXISTS "PolicyStatus_pkey";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyRule" DROP CONSTRAINT IF EXISTS "PolicyRule_pkey";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyRuleType" DROP CONSTRAINT IF EXISTS "PolicyRuleType_pkey";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyException" DROP CONSTRAINT IF EXISTS "PolicyException_pkey";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyDocument" DROP CONSTRAINT IF EXISTS "PolicyDocument_pkey";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyDocumentType" DROP CONSTRAINT IF EXISTS "PolicyDocumentType_pkey";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyChangeAudit" DROP CONSTRAINT IF EXISTS "PolicyChangeAudit_pkey";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyCategory" DROP CONSTRAINT IF EXISTS "PolicyCategory_pkey";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyAssignment" DROP CONSTRAINT IF EXISTS "PolicyAssignment_pkey";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyApprovalStage" DROP CONSTRAINT IF EXISTS "PolicyApprovalStage_pkey";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyApprovalHistory" DROP CONSTRAINT IF EXISTS "PolicyApprovalHistory_pkey";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyApplicability" DROP CONSTRAINT IF EXISTS "PolicyApplicability_pkey";
ALTER TABLE IF EXISTS ONLY axionpro."PolicyAcknowledgement" DROP CONSTRAINT IF EXISTS "PolicyAcknowledgement_pkey";
ALTER TABLE IF EXISTS ONLY axionpro."PaymentWebhookEvent" DROP CONSTRAINT IF EXISTS "PaymentWebhookEvent_pkey";
ALTER TABLE IF EXISTS ONLY axionpro."PaymentTransaction" DROP CONSTRAINT IF EXISTS "PaymentTransaction_pkey";
ALTER TABLE IF EXISTS ONLY axionpro."PaymentGateway" DROP CONSTRAINT IF EXISTS "PaymentGateway_pkey";
ALTER TABLE IF EXISTS ONLY axionpro."PaymentAttempt" DROP CONSTRAINT IF EXISTS "PaymentAttempt_pkey";
ALTER TABLE IF EXISTS ONLY axionpro."WorkDocumentType" DROP CONSTRAINT IF EXISTS "PK__WorkDocu__3214EC0710449AA0";
ALTER TABLE IF EXISTS ONLY axionpro."UserRole" DROP CONSTRAINT IF EXISTS "PK__UserRole__3214EC07C9E25EB9";
ALTER TABLE IF EXISTS ONLY axionpro."UserAttendanceSetting" DROP CONSTRAINT IF EXISTS "PK__UserAtte__3214EC0750CE7646";
ALTER TABLE IF EXISTS ONLY axionpro."TicketType" DROP CONSTRAINT IF EXISTS "PK__TicketTy__3214EC07554DC568";
ALTER TABLE IF EXISTS ONLY axionpro."TicketHeader" DROP CONSTRAINT IF EXISTS "PK__TicketHe__3214EC07EADE3F1B";
ALTER TABLE IF EXISTS ONLY axionpro."TicketClassification" DROP CONSTRAINT IF EXISTS "PK__TicketCl__3214EC07BA509F8F";
ALTER TABLE IF EXISTS ONLY axionpro."Tender" DROP CONSTRAINT IF EXISTS "PK__Tender__3214EC076E847502";
ALTER TABLE IF EXISTS ONLY axionpro."Tenant" DROP CONSTRAINT IF EXISTS "PK__Tenant__3214EC0728DD7C6E";
ALTER TABLE IF EXISTS ONLY axionpro."TenantSubscription" DROP CONSTRAINT IF EXISTS "PK__TenantSu__3214EC07CF143048";
ALTER TABLE IF EXISTS ONLY axionpro."TenantProfile" DROP CONSTRAINT IF EXISTS "PK__TenantPr__3214EC0796A1B93D";
ALTER TABLE IF EXISTS ONLY axionpro."TenantIndustry" DROP CONSTRAINT IF EXISTS "PK__TenantIn__3214EC0765258C5E";
ALTER TABLE IF EXISTS ONLY axionpro."TenantEnabledModule" DROP CONSTRAINT IF EXISTS "PK__TenantEn__3214EC07AC610092";
ALTER TABLE IF EXISTS ONLY axionpro."TenantEncryptionKeys" DROP CONSTRAINT IF EXISTS "PK__TenantEn__3214EC070E3540EE";
ALTER TABLE IF EXISTS ONLY axionpro."TenantEnabledOperation" DROP CONSTRAINT IF EXISTS "PK__TenantEn__3214EC07047C5B46";
ALTER TABLE IF EXISTS ONLY axionpro."TenantEmailConfig" DROP CONSTRAINT IF EXISTS "PK__TenantEm__3214EC077A90B3A3";
ALTER TABLE IF EXISTS ONLY axionpro."SubscriptionPlan" DROP CONSTRAINT IF EXISTS "PK__Subscrip__3214EC07BED0F0D0";
ALTER TABLE IF EXISTS ONLY axionpro."StatutoryType" DROP CONSTRAINT IF EXISTS "PK__Statutor__3214EC078EED6601";
ALTER TABLE IF EXISTS ONLY axionpro."State" DROP CONSTRAINT IF EXISTS "PK__State__3214EC0705AF0D74";
ALTER TABLE IF EXISTS ONLY axionpro."Role" DROP CONSTRAINT IF EXISTS "PK__Role__3214EC0723371271";
ALTER TABLE IF EXISTS ONLY axionpro."RequestType" DROP CONSTRAINT IF EXISTS "PK__RequestT__3214EC07E53AD996";
ALTER TABLE IF EXISTS ONLY axionpro."ReportingType" DROP CONSTRAINT IF EXISTS "PK__Reportin__3214EC0773736C22";
ALTER TABLE IF EXISTS ONLY axionpro."RefreshToken" DROP CONSTRAINT IF EXISTS "PK__RefreshT__3214EC0731D02168";
ALTER TABLE IF EXISTS ONLY axionpro."PlanModuleMapping" DROP CONSTRAINT IF EXISTS "PK__PlanModu__3214EC0729948732";
ALTER TABLE IF EXISTS ONLY axionpro."PageTypeEnum" DROP CONSTRAINT IF EXISTS "PK__PageType__3214EC07D71C5D81";
ALTER TABLE IF EXISTS ONLY axionpro."Operation" DROP CONSTRAINT IF EXISTS "PK__Operatio__3214EC079C437610";
ALTER TABLE IF EXISTS ONLY axionpro."Module" DROP CONSTRAINT IF EXISTS "PK__Module__3214EC078891AB2D";
ALTER TABLE IF EXISTS ONLY axionpro."ModuleOperationMapping" DROP CONSTRAINT IF EXISTS "PK__ModuleOp__3214EC07BF86A196";
ALTER TABLE IF EXISTS ONLY axionpro."LoginCredential" DROP CONSTRAINT IF EXISTS "PK__LoginCre__3214EC0750429DFA";
ALTER TABLE IF EXISTS ONLY axionpro."License" DROP CONSTRAINT IF EXISTS "PK__License__3214EC07FF575687";
ALTER TABLE IF EXISTS ONLY axionpro."LeaveType" DROP CONSTRAINT IF EXISTS "PK__LeaveTyp__3214EC0788A5EF9C";
ALTER TABLE IF EXISTS ONLY axionpro."LeaveRequest" DROP CONSTRAINT IF EXISTS "PK__LeaveReq__3214EC07D95D9BAD";
ALTER TABLE IF EXISTS ONLY axionpro."IdentityCategoryDocument" DROP CONSTRAINT IF EXISTS "PK__Identity__3214EC07AA61914F";
ALTER TABLE IF EXISTS ONLY axionpro."IdentityCategory" DROP CONSTRAINT IF EXISTS "PK__Identity__3214EC07393AF557";
ALTER TABLE IF EXISTS ONLY axionpro."Gender" DROP CONSTRAINT IF EXISTS "PK__Gender__3214EC07EF3CD03D";
ALTER TABLE IF EXISTS ONLY axionpro."ForgotPasswordOTPDetail" DROP CONSTRAINT IF EXISTS "PK__ForgotPa__3214EC071E853072";
ALTER TABLE IF EXISTS ONLY axionpro."Employee" DROP CONSTRAINT IF EXISTS "PK__Employee__3214EC07E3264254";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeImage" DROP CONSTRAINT IF EXISTS "PK__Employee__3214EC07C583D80F";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeWorkDocument" DROP CONSTRAINT IF EXISTS "PK__Employee__3214EC07BDFE60B6";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeCategorySkill" DROP CONSTRAINT IF EXISTS "PK__Employee__3214EC07BB3C49C9";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeLeaveBalance" DROP CONSTRAINT IF EXISTS "PK__Employee__3214EC07B6CC8062";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeStatutoryAccount" DROP CONSTRAINT IF EXISTS "PK__Employee__3214EC07ACDD4E51";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeTypeBasicMenu" DROP CONSTRAINT IF EXISTS "PK__Employee__3214EC07A773CB3F";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeWorkHistory" DROP CONSTRAINT IF EXISTS "PK__Employee__3214EC07994D46FA";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeDailyAttendance" DROP CONSTRAINT IF EXISTS "PK__Employee__3214EC078C36F2DB";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeesChangedTypeHistory" DROP CONSTRAINT IF EXISTS "PK__Employee__3214EC0779153EAD";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeType" DROP CONSTRAINT IF EXISTS "PK__Employee__3214EC0760C5ED38";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeWorkProfile" DROP CONSTRAINT IF EXISTS "PK__Employee__3214EC07605A7EB3";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeManagerMapping" DROP CONSTRAINT IF EXISTS "PK__Employee__3214EC075BB6D3FC";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeIdentity" DROP CONSTRAINT IF EXISTS "PK__Employee__3214EC074FF9BCC1";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeDependent" DROP CONSTRAINT IF EXISTS "PK__Employee__3214EC074B6A13E2";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeePersonalDetail" DROP CONSTRAINT IF EXISTS "PK__Employee__3214EC074796302D";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeContact" DROP CONSTRAINT IF EXISTS "PK__Employee__3214EC07371CFD87";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeLeavePolicyMapping" DROP CONSTRAINT IF EXISTS "PK__Employee__3214EC07305828A7";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeBankDetail" DROP CONSTRAINT IF EXISTS "PK__Employee__3214EC072E9F930F";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeCodePattern" DROP CONSTRAINT IF EXISTS "PK__Employee__3214EC070ECDA69B";
ALTER TABLE IF EXISTS ONLY axionpro."EmailTemplate" DROP CONSTRAINT IF EXISTS "PK__EmailTem__3214EC072FE49A64";
ALTER TABLE IF EXISTS ONLY axionpro."EmailQueue" DROP CONSTRAINT IF EXISTS "PK__EmailQue__3214EC0705218438";
ALTER TABLE IF EXISTS ONLY axionpro."District" DROP CONSTRAINT IF EXISTS "PK__District__3214EC07CB6A47D0";
ALTER TABLE IF EXISTS ONLY axionpro."Designation" DROP CONSTRAINT IF EXISTS "PK__Designat__3214EC07F9FF4C75";
ALTER TABLE IF EXISTS ONLY axionpro."Department" DROP CONSTRAINT IF EXISTS "PK__Departme__3214EC071E7B0B0B";
ALTER TABLE IF EXISTS ONLY axionpro."DayCombination" DROP CONSTRAINT IF EXISTS "PK__DayCombi__3214EC070D2CF976";
ALTER TABLE IF EXISTS ONLY axionpro."Country" DROP CONSTRAINT IF EXISTS "PK__Country__3214EC070584DCC0";
ALTER TABLE IF EXISTS ONLY axionpro."CountryStatutoryRule" DROP CONSTRAINT IF EXISTS "PK__CountryS__3214EC07068D8E4F";
ALTER TABLE IF EXISTS ONLY axionpro."CountryIdentityRule" DROP CONSTRAINT IF EXISTS "PK__CountryI__3214EC0748E4A649";
ALTER TABLE IF EXISTS ONLY axionpro."ClientType" DROP CONSTRAINT IF EXISTS "PK__ClientTy__3214EC078D984A16";
ALTER TABLE IF EXISTS ONLY axionpro."Locality" DROP CONSTRAINT IF EXISTS "PK__City__3214EC07DC3B7144";
ALTER TABLE IF EXISTS ONLY axionpro."Category" DROP CONSTRAINT IF EXISTS "PK__Category__3214EC070D4D0C80";
ALTER TABLE IF EXISTS ONLY axionpro."AttendanceRequest" DROP CONSTRAINT IF EXISTS "PK__Attendan__3214EC07DA4D2CA6";
ALTER TABLE IF EXISTS ONLY axionpro."Attendance" DROP CONSTRAINT IF EXISTS "PK__Attendan__3214EC079EEE2ABB";
ALTER TABLE IF EXISTS ONLY axionpro."AssignmentStatus" DROP CONSTRAINT IF EXISTS "PK__Assignme__3214EC07BCFD76FA";
ALTER TABLE IF EXISTS ONLY axionpro."Asset" DROP CONSTRAINT IF EXISTS "PK__Asset__3214EC076178ABAE";
ALTER TABLE IF EXISTS ONLY axionpro."AssetType" DROP CONSTRAINT IF EXISTS "PK__AssetTyp__3214EC077375A9AA";
ALTER TABLE IF EXISTS ONLY axionpro."AssetStatus" DROP CONSTRAINT IF EXISTS "PK__AssetSta__3214EC0722A59445";
ALTER TABLE IF EXISTS ONLY axionpro."AssetImage" DROP CONSTRAINT IF EXISTS "PK__AssetIma__3214EC0752335BEC";
ALTER TABLE IF EXISTS ONLY axionpro."AssetCategory" DROP CONSTRAINT IF EXISTS "PK__AssetCat__3214EC07E9FE2792";
ALTER TABLE IF EXISTS ONLY axionpro."TenderStatus" DROP CONSTRAINT IF EXISTS "PK_TenderStatus";
ALTER TABLE IF EXISTS ONLY axionpro."Client" DROP CONSTRAINT IF EXISTS "PK_TenderClient";
ALTER TABLE IF EXISTS ONLY axionpro."TenantLocation" DROP CONSTRAINT IF EXISTS "PK_TenantLocation";
ALTER TABLE IF EXISTS ONLY axionpro."TenantDevice" DROP CONSTRAINT IF EXISTS "PK_TenantDevice";
ALTER TABLE IF EXISTS ONLY axionpro."TaxSystemMaster" DROP CONSTRAINT IF EXISTS "PK_TaxSystemMaster";
ALTER TABLE IF EXISTS ONLY axionpro."TaxSlab" DROP CONSTRAINT IF EXISTS "PK_TaxSlab";
ALTER TABLE IF EXISTS ONLY axionpro."TaxRule" DROP CONSTRAINT IF EXISTS "PK_TaxRule";
ALTER TABLE IF EXISTS ONLY axionpro."TaxRegimeMaster" DROP CONSTRAINT IF EXISTS "PK_TaxRegimeMaster";
ALTER TABLE IF EXISTS ONLY axionpro."SalaryStructureDetail" DROP CONSTRAINT IF EXISTS "PK_SalaryStructureDetail";
ALTER TABLE IF EXISTS ONLY axionpro."SalaryStructure" DROP CONSTRAINT IF EXISTS "PK_SalaryStructure";
ALTER TABLE IF EXISTS ONLY axionpro."SalaryComponentMaster" DROP CONSTRAINT IF EXISTS "PK_SalaryComponentMaster";
ALTER TABLE IF EXISTS ONLY axionpro."RoleModuleAndPermission" DROP CONSTRAINT IF EXISTS "PK_RoleModuleAndPermission_Id";
ALTER TABLE IF EXISTS ONLY axionpro."PayrollRun" DROP CONSTRAINT IF EXISTS "PK_PayrollRun";
ALTER TABLE IF EXISTS ONLY axionpro."PayrollEmployeeDetail" DROP CONSTRAINT IF EXISTS "PK_PayrollEmployeeDetail";
ALTER TABLE IF EXISTS ONLY axionpro."PayrollEmployee" DROP CONSTRAINT IF EXISTS "PK_PayrollEmployee";
ALTER TABLE IF EXISTS ONLY axionpro."Holiday" DROP CONSTRAINT IF EXISTS "PK_Holiday";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeWorkPattern" DROP CONSTRAINT IF EXISTS "PK_EmployeeWorkPattern";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeWorkModeOverrideRequest" DROP CONSTRAINT IF EXISTS "PK_EmployeeWorkModeOverrideRequest";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeWorkArrangement" DROP CONSTRAINT IF EXISTS "PK_EmployeeWorkArrangement";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeTaxProfile" DROP CONSTRAINT IF EXISTS "PK_EmployeeTaxProfile";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeSalary" DROP CONSTRAINT IF EXISTS "PK_EmployeeSalary";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeLocationAssignment" DROP CONSTRAINT IF EXISTS "PK_EmployeeLocationAssignment";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeExperienceDocument" DROP CONSTRAINT IF EXISTS "PK_EmployeeExperienceDocument";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeExperience" DROP CONSTRAINT IF EXISTS "PK_EmployeeExperience";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeEducation" DROP CONSTRAINT IF EXISTS "PK_EmployeeEducation";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeDeviceEnrollment" DROP CONSTRAINT IF EXISTS "PK_EmployeeDeviceEnrollment";
ALTER TABLE IF EXISTS ONLY axionpro."DeviceMaster" DROP CONSTRAINT IF EXISTS "PK_DeviceMaster";
ALTER TABLE IF EXISTS ONLY axionpro."DataViewStructure" DROP CONSTRAINT IF EXISTS "PK_DataViewStructure";
ALTER TABLE IF EXISTS ONLY axionpro."ComplianceRule" DROP CONSTRAINT IF EXISTS "PK_ComplianceRule";
ALTER TABLE IF EXISTS ONLY axionpro."AttendancePolicy" DROP CONSTRAINT IF EXISTS "PK_AttendancePolicy";
ALTER TABLE IF EXISTS ONLY axionpro."AttendanceDeviceType" DROP CONSTRAINT IF EXISTS "PK_AttendanceDeviceType";
ALTER TABLE IF EXISTS ONLY axionpro."LocalityType" DROP CONSTRAINT IF EXISTS "LocalityType_pkey";
ALTER TABLE IF EXISTS ONLY axionpro."HostUser" DROP CONSTRAINT IF EXISTS "HostUser_pkey";
ALTER TABLE IF EXISTS ONLY axionpro."HostRole" DROP CONSTRAINT IF EXISTS "HostRole_pkey";
ALTER TABLE IF EXISTS ONLY axionpro."HostRoleModuleAndPermission" DROP CONSTRAINT IF EXISTS "HostRoleModuleAndPermission_pkey";
ALTER TABLE IF EXISTS ONLY axionpro."HostBillingConfiguration" DROP CONSTRAINT IF EXISTS "HostBillingConfiguration_pkey";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeePolicyEnrollment" DROP CONSTRAINT IF EXISTS "EmployeePolicyEnrollment_pkey";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeePolicyDependentMapping" DROP CONSTRAINT IF EXISTS "EmployeePolicyDependentMapping_pkey";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeAttendancePunch" DROP CONSTRAINT IF EXISTS "EmployeeAttendancePunch_pkey";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeWorkArrangement" DROP CONSTRAINT IF EXISTS "EX_EmployeeWorkArrangement_Employee_Window";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeLocationAssignment" DROP CONSTRAINT IF EXISTS "EX_EmployeeLocationAssignment_Primary_Window";
ALTER TABLE IF EXISTS ONLY axionpro."EmployeeLocationAssignment" DROP CONSTRAINT IF EXISTS "EX_EmployeeLocationAssignment_Employee_Location_Window";
ALTER TABLE IF EXISTS ONLY axionpro."DeviceMessageLog" DROP CONSTRAINT IF EXISTS "DeviceMessageLog_pkey";
ALTER TABLE IF EXISTS ONLY axionpro."DeviceInitialProvisioning" DROP CONSTRAINT IF EXISTS "DeviceInitialProvisioning_pkey";
ALTER TABLE IF EXISTS ONLY axionpro."DeviceCredential" DROP CONSTRAINT IF EXISTS "DeviceCredential_pkey";
ALTER TABLE IF EXISTS ONLY axionpro."DeviceCommand" DROP CONSTRAINT IF EXISTS "DeviceCommand_pkey";
ALTER TABLE IF EXISTS ONLY axionpro."DeviceCommandResponse" DROP CONSTRAINT IF EXISTS "DeviceCommandResponse_pkey";
ALTER TABLE IF EXISTS ONLY axionpro."DefaultEmailConfig" DROP CONSTRAINT IF EXISTS "DefaultEmailConfig_pkey";
ALTER TABLE IF EXISTS ONLY axionpro."ComplianceTypeMaster" DROP CONSTRAINT IF EXISTS "ComplianceTypeMaster_pkey";
ALTER TABLE IF EXISTS ONLY axionpro."BulkImportJob" DROP CONSTRAINT IF EXISTS "BulkImportJob_pkey";
ALTER TABLE IF EXISTS ONLY axionpro."BillingTaxRule" DROP CONSTRAINT IF EXISTS "BillingTaxRule_pkey";
ALTER TABLE IF EXISTS ONLY axionpro."BillingSubscriptionChange" DROP CONSTRAINT IF EXISTS "BillingSubscriptionChange_pkey";
ALTER TABLE IF EXISTS ONLY axionpro."BillingRefund" DROP CONSTRAINT IF EXISTS "BillingRefund_pkey";
ALTER TABLE IF EXISTS ONLY axionpro."BillingOrder" DROP CONSTRAINT IF EXISTS "BillingOrder_pkey";
ALTER TABLE IF EXISTS ONLY axionpro."BillingInvoice" DROP CONSTRAINT IF EXISTS "BillingInvoice_pkey";
ALTER TABLE IF EXISTS ONLY axionpro."BillingInvoiceLine" DROP CONSTRAINT IF EXISTS "BillingInvoiceLine_pkey";
ALTER TABLE IF EXISTS ONLY axionpro."BillingAuditLog" DROP CONSTRAINT IF EXISTS "BillingAuditLog_pkey";
ALTER TABLE IF EXISTS ONLY axionpro."AttendancePolicyVersionConfiguration" DROP CONSTRAINT IF EXISTS "AttendancePolicyVersionConfiguration_pkey";
ALTER TABLE IF EXISTS ONLY axionpro."AssetRequest" DROP CONSTRAINT IF EXISTS "AssetRequest_pkey";
ALTER TABLE IF EXISTS ONLY axionpro."AssetAssignment" DROP CONSTRAINT IF EXISTS "AssetAssignment_pkey";
--
-- Name: AssetAssignment AssetAssignment_pkey; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."AssetAssignment"
    ADD CONSTRAINT "AssetAssignment_pkey" PRIMARY KEY ("Id");


--
-- Name: AssetRequest AssetRequest_pkey; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."AssetRequest"
    ADD CONSTRAINT "AssetRequest_pkey" PRIMARY KEY ("Id");


--
-- Name: AttendancePolicyVersionConfiguration AttendancePolicyVersionConfiguration_pkey; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."AttendancePolicyVersionConfiguration"
    ADD CONSTRAINT "AttendancePolicyVersionConfiguration_pkey" PRIMARY KEY ("Id");


--
-- Name: BillingAuditLog BillingAuditLog_pkey; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."BillingAuditLog"
    ADD CONSTRAINT "BillingAuditLog_pkey" PRIMARY KEY ("Id");


--
-- Name: BillingInvoiceLine BillingInvoiceLine_pkey; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."BillingInvoiceLine"
    ADD CONSTRAINT "BillingInvoiceLine_pkey" PRIMARY KEY ("Id");


--
-- Name: BillingInvoice BillingInvoice_pkey; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."BillingInvoice"
    ADD CONSTRAINT "BillingInvoice_pkey" PRIMARY KEY ("Id");


--
-- Name: BillingOrder BillingOrder_pkey; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."BillingOrder"
    ADD CONSTRAINT "BillingOrder_pkey" PRIMARY KEY ("Id");


--
-- Name: BillingRefund BillingRefund_pkey; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."BillingRefund"
    ADD CONSTRAINT "BillingRefund_pkey" PRIMARY KEY ("Id");


--
-- Name: BillingSubscriptionChange BillingSubscriptionChange_pkey; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."BillingSubscriptionChange"
    ADD CONSTRAINT "BillingSubscriptionChange_pkey" PRIMARY KEY ("Id");


--
-- Name: BillingTaxRule BillingTaxRule_pkey; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."BillingTaxRule"
    ADD CONSTRAINT "BillingTaxRule_pkey" PRIMARY KEY ("Id");


--
-- Name: BulkImportJob BulkImportJob_pkey; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."BulkImportJob"
    ADD CONSTRAINT "BulkImportJob_pkey" PRIMARY KEY ("Id");


--
-- Name: ComplianceTypeMaster ComplianceTypeMaster_pkey; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."ComplianceTypeMaster"
    ADD CONSTRAINT "ComplianceTypeMaster_pkey" PRIMARY KEY ("Id");


--
-- Name: DefaultEmailConfig DefaultEmailConfig_pkey; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."DefaultEmailConfig"
    ADD CONSTRAINT "DefaultEmailConfig_pkey" PRIMARY KEY ("Id");


--
-- Name: DeviceCommandResponse DeviceCommandResponse_pkey; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."DeviceCommandResponse"
    ADD CONSTRAINT "DeviceCommandResponse_pkey" PRIMARY KEY ("Id");


--
-- Name: DeviceCommand DeviceCommand_pkey; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."DeviceCommand"
    ADD CONSTRAINT "DeviceCommand_pkey" PRIMARY KEY ("Id");


--
-- Name: DeviceCredential DeviceCredential_pkey; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."DeviceCredential"
    ADD CONSTRAINT "DeviceCredential_pkey" PRIMARY KEY ("Id");


--
-- Name: DeviceInitialProvisioning DeviceInitialProvisioning_pkey; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."DeviceInitialProvisioning"
    ADD CONSTRAINT "DeviceInitialProvisioning_pkey" PRIMARY KEY ("Id");


--
-- Name: DeviceMessageLog DeviceMessageLog_pkey; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."DeviceMessageLog"
    ADD CONSTRAINT "DeviceMessageLog_pkey" PRIMARY KEY ("Id");


--
-- Name: EmployeeLocationAssignment EX_EmployeeLocationAssignment_Employee_Location_Window; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeLocationAssignment"
    ADD CONSTRAINT "EX_EmployeeLocationAssignment_Employee_Location_Window" EXCLUDE USING gist ("TenantId" WITH =, "EmployeeId" WITH =, "TenantLocationId" WITH =, daterange("EffectiveFrom", COALESCE("EffectiveTo", 'infinity'::date), '[]'::text) WITH &&) WHERE ((("IsActive" = true) AND ("IsSoftDeleted" = false)));


--
-- Name: EmployeeLocationAssignment EX_EmployeeLocationAssignment_Primary_Window; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeLocationAssignment"
    ADD CONSTRAINT "EX_EmployeeLocationAssignment_Primary_Window" EXCLUDE USING gist ("TenantId" WITH =, "EmployeeId" WITH =, daterange("EffectiveFrom", COALESCE("EffectiveTo", 'infinity'::date), '[]'::text) WITH &&) WHERE ((("IsPrimary" = true) AND ("IsActive" = true) AND ("IsSoftDeleted" = false)));


--
-- Name: EmployeeWorkArrangement EX_EmployeeWorkArrangement_Employee_Window; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeWorkArrangement"
    ADD CONSTRAINT "EX_EmployeeWorkArrangement_Employee_Window" EXCLUDE USING gist ("TenantId" WITH =, "EmployeeId" WITH =, daterange("EffectiveFrom", COALESCE("EffectiveTo", 'infinity'::date), '[]'::text) WITH &&) WHERE ((("IsActive" = true) AND ("IsSoftDeleted" = false)));


--
-- Name: EmployeeAttendancePunch EmployeeAttendancePunch_pkey; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeAttendancePunch"
    ADD CONSTRAINT "EmployeeAttendancePunch_pkey" PRIMARY KEY ("Id");


--
-- Name: EmployeePolicyDependentMapping EmployeePolicyDependentMapping_pkey; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeePolicyDependentMapping"
    ADD CONSTRAINT "EmployeePolicyDependentMapping_pkey" PRIMARY KEY ("Id");


--
-- Name: EmployeePolicyEnrollment EmployeePolicyEnrollment_pkey; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeePolicyEnrollment"
    ADD CONSTRAINT "EmployeePolicyEnrollment_pkey" PRIMARY KEY ("Id");


--
-- Name: HostBillingConfiguration HostBillingConfiguration_pkey; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."HostBillingConfiguration"
    ADD CONSTRAINT "HostBillingConfiguration_pkey" PRIMARY KEY ("Id");


--
-- Name: HostRoleModuleAndPermission HostRoleModuleAndPermission_pkey; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."HostRoleModuleAndPermission"
    ADD CONSTRAINT "HostRoleModuleAndPermission_pkey" PRIMARY KEY ("Id");


--
-- Name: HostRole HostRole_pkey; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."HostRole"
    ADD CONSTRAINT "HostRole_pkey" PRIMARY KEY ("Id");


--
-- Name: HostUser HostUser_pkey; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."HostUser"
    ADD CONSTRAINT "HostUser_pkey" PRIMARY KEY ("Id");


--
-- Name: LocalityType LocalityType_pkey; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."LocalityType"
    ADD CONSTRAINT "LocalityType_pkey" PRIMARY KEY ("Id");


--
-- Name: AttendanceDeviceType PK_AttendanceDeviceType; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."AttendanceDeviceType"
    ADD CONSTRAINT "PK_AttendanceDeviceType" PRIMARY KEY ("Id");


--
-- Name: AttendancePolicy PK_AttendancePolicy; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."AttendancePolicy"
    ADD CONSTRAINT "PK_AttendancePolicy" PRIMARY KEY ("Id");


--
-- Name: ComplianceRule PK_ComplianceRule; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."ComplianceRule"
    ADD CONSTRAINT "PK_ComplianceRule" PRIMARY KEY ("Id");


--
-- Name: DataViewStructure PK_DataViewStructure; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."DataViewStructure"
    ADD CONSTRAINT "PK_DataViewStructure" PRIMARY KEY ("Id");


--
-- Name: DeviceMaster PK_DeviceMaster; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."DeviceMaster"
    ADD CONSTRAINT "PK_DeviceMaster" PRIMARY KEY ("Id");


--
-- Name: EmployeeDeviceEnrollment PK_EmployeeDeviceEnrollment; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeDeviceEnrollment"
    ADD CONSTRAINT "PK_EmployeeDeviceEnrollment" PRIMARY KEY ("Id");


--
-- Name: EmployeeEducation PK_EmployeeEducation; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeEducation"
    ADD CONSTRAINT "PK_EmployeeEducation" PRIMARY KEY ("Id");


--
-- Name: EmployeeExperience PK_EmployeeExperience; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeExperience"
    ADD CONSTRAINT "PK_EmployeeExperience" PRIMARY KEY ("Id");


--
-- Name: EmployeeExperienceDocument PK_EmployeeExperienceDocument; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeExperienceDocument"
    ADD CONSTRAINT "PK_EmployeeExperienceDocument" PRIMARY KEY ("Id");


--
-- Name: EmployeeLocationAssignment PK_EmployeeLocationAssignment; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeLocationAssignment"
    ADD CONSTRAINT "PK_EmployeeLocationAssignment" PRIMARY KEY ("Id");


--
-- Name: EmployeeSalary PK_EmployeeSalary; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeSalary"
    ADD CONSTRAINT "PK_EmployeeSalary" PRIMARY KEY ("Id");


--
-- Name: EmployeeTaxProfile PK_EmployeeTaxProfile; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeTaxProfile"
    ADD CONSTRAINT "PK_EmployeeTaxProfile" PRIMARY KEY ("Id");


--
-- Name: EmployeeWorkArrangement PK_EmployeeWorkArrangement; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeWorkArrangement"
    ADD CONSTRAINT "PK_EmployeeWorkArrangement" PRIMARY KEY ("Id");


--
-- Name: EmployeeWorkModeOverrideRequest PK_EmployeeWorkModeOverrideRequest; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeWorkModeOverrideRequest"
    ADD CONSTRAINT "PK_EmployeeWorkModeOverrideRequest" PRIMARY KEY ("Id");


--
-- Name: EmployeeWorkPattern PK_EmployeeWorkPattern; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeWorkPattern"
    ADD CONSTRAINT "PK_EmployeeWorkPattern" PRIMARY KEY ("Id");


--
-- Name: Holiday PK_Holiday; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."Holiday"
    ADD CONSTRAINT "PK_Holiday" PRIMARY KEY ("Id");


--
-- Name: PayrollEmployee PK_PayrollEmployee; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PayrollEmployee"
    ADD CONSTRAINT "PK_PayrollEmployee" PRIMARY KEY ("Id");


--
-- Name: PayrollEmployeeDetail PK_PayrollEmployeeDetail; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PayrollEmployeeDetail"
    ADD CONSTRAINT "PK_PayrollEmployeeDetail" PRIMARY KEY ("Id");


--
-- Name: PayrollRun PK_PayrollRun; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PayrollRun"
    ADD CONSTRAINT "PK_PayrollRun" PRIMARY KEY ("Id");


--
-- Name: RoleModuleAndPermission PK_RoleModuleAndPermission_Id; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."RoleModuleAndPermission"
    ADD CONSTRAINT "PK_RoleModuleAndPermission_Id" PRIMARY KEY ("Id");


--
-- Name: SalaryComponentMaster PK_SalaryComponentMaster; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."SalaryComponentMaster"
    ADD CONSTRAINT "PK_SalaryComponentMaster" PRIMARY KEY ("Id");


--
-- Name: SalaryStructure PK_SalaryStructure; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."SalaryStructure"
    ADD CONSTRAINT "PK_SalaryStructure" PRIMARY KEY ("Id");


--
-- Name: SalaryStructureDetail PK_SalaryStructureDetail; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."SalaryStructureDetail"
    ADD CONSTRAINT "PK_SalaryStructureDetail" PRIMARY KEY ("Id");


--
-- Name: TaxRegimeMaster PK_TaxRegimeMaster; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TaxRegimeMaster"
    ADD CONSTRAINT "PK_TaxRegimeMaster" PRIMARY KEY ("Id");


--
-- Name: TaxRule PK_TaxRule; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TaxRule"
    ADD CONSTRAINT "PK_TaxRule" PRIMARY KEY ("Id");


--
-- Name: TaxSlab PK_TaxSlab; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TaxSlab"
    ADD CONSTRAINT "PK_TaxSlab" PRIMARY KEY ("Id");


--
-- Name: TaxSystemMaster PK_TaxSystemMaster; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TaxSystemMaster"
    ADD CONSTRAINT "PK_TaxSystemMaster" PRIMARY KEY ("Id");


--
-- Name: TenantDevice PK_TenantDevice; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TenantDevice"
    ADD CONSTRAINT "PK_TenantDevice" PRIMARY KEY ("Id");


--
-- Name: TenantLocation PK_TenantLocation; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TenantLocation"
    ADD CONSTRAINT "PK_TenantLocation" PRIMARY KEY ("Id");


--
-- Name: Client PK_TenderClient; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."Client"
    ADD CONSTRAINT "PK_TenderClient" PRIMARY KEY ("Id");


--
-- Name: TenderStatus PK_TenderStatus; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TenderStatus"
    ADD CONSTRAINT "PK_TenderStatus" PRIMARY KEY ("Id");


--
-- Name: AssetCategory PK__AssetCat__3214EC07E9FE2792; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."AssetCategory"
    ADD CONSTRAINT "PK__AssetCat__3214EC07E9FE2792" PRIMARY KEY ("Id");


--
-- Name: AssetImage PK__AssetIma__3214EC0752335BEC; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."AssetImage"
    ADD CONSTRAINT "PK__AssetIma__3214EC0752335BEC" PRIMARY KEY ("Id");


--
-- Name: AssetStatus PK__AssetSta__3214EC0722A59445; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."AssetStatus"
    ADD CONSTRAINT "PK__AssetSta__3214EC0722A59445" PRIMARY KEY ("Id");


--
-- Name: AssetType PK__AssetTyp__3214EC077375A9AA; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."AssetType"
    ADD CONSTRAINT "PK__AssetTyp__3214EC077375A9AA" PRIMARY KEY ("Id");


--
-- Name: Asset PK__Asset__3214EC076178ABAE; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."Asset"
    ADD CONSTRAINT "PK__Asset__3214EC076178ABAE" PRIMARY KEY ("Id");


--
-- Name: AssignmentStatus PK__Assignme__3214EC07BCFD76FA; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."AssignmentStatus"
    ADD CONSTRAINT "PK__Assignme__3214EC07BCFD76FA" PRIMARY KEY ("Id");


--
-- Name: Attendance PK__Attendan__3214EC079EEE2ABB; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."Attendance"
    ADD CONSTRAINT "PK__Attendan__3214EC079EEE2ABB" PRIMARY KEY ("Id");


--
-- Name: AttendanceRequest PK__Attendan__3214EC07DA4D2CA6; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."AttendanceRequest"
    ADD CONSTRAINT "PK__Attendan__3214EC07DA4D2CA6" PRIMARY KEY ("Id");


--
-- Name: Category PK__Category__3214EC070D4D0C80; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."Category"
    ADD CONSTRAINT "PK__Category__3214EC070D4D0C80" PRIMARY KEY ("Id");


--
-- Name: Locality PK__City__3214EC07DC3B7144; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."Locality"
    ADD CONSTRAINT "PK__City__3214EC07DC3B7144" PRIMARY KEY ("Id");


--
-- Name: ClientType PK__ClientTy__3214EC078D984A16; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."ClientType"
    ADD CONSTRAINT "PK__ClientTy__3214EC078D984A16" PRIMARY KEY ("Id");


--
-- Name: CountryIdentityRule PK__CountryI__3214EC0748E4A649; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."CountryIdentityRule"
    ADD CONSTRAINT "PK__CountryI__3214EC0748E4A649" PRIMARY KEY ("Id");


--
-- Name: CountryStatutoryRule PK__CountryS__3214EC07068D8E4F; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."CountryStatutoryRule"
    ADD CONSTRAINT "PK__CountryS__3214EC07068D8E4F" PRIMARY KEY ("Id");


--
-- Name: Country PK__Country__3214EC070584DCC0; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."Country"
    ADD CONSTRAINT "PK__Country__3214EC070584DCC0" PRIMARY KEY ("Id");


--
-- Name: DayCombination PK__DayCombi__3214EC070D2CF976; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."DayCombination"
    ADD CONSTRAINT "PK__DayCombi__3214EC070D2CF976" PRIMARY KEY ("Id");


--
-- Name: Department PK__Departme__3214EC071E7B0B0B; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."Department"
    ADD CONSTRAINT "PK__Departme__3214EC071E7B0B0B" PRIMARY KEY ("Id");


--
-- Name: Designation PK__Designat__3214EC07F9FF4C75; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."Designation"
    ADD CONSTRAINT "PK__Designat__3214EC07F9FF4C75" PRIMARY KEY ("Id");


--
-- Name: District PK__District__3214EC07CB6A47D0; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."District"
    ADD CONSTRAINT "PK__District__3214EC07CB6A47D0" PRIMARY KEY ("Id");


--
-- Name: EmailQueue PK__EmailQue__3214EC0705218438; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmailQueue"
    ADD CONSTRAINT "PK__EmailQue__3214EC0705218438" PRIMARY KEY ("Id");


--
-- Name: EmailTemplate PK__EmailTem__3214EC072FE49A64; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmailTemplate"
    ADD CONSTRAINT "PK__EmailTem__3214EC072FE49A64" PRIMARY KEY ("Id");


--
-- Name: EmployeeCodePattern PK__Employee__3214EC070ECDA69B; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeCodePattern"
    ADD CONSTRAINT "PK__Employee__3214EC070ECDA69B" PRIMARY KEY ("Id");


--
-- Name: EmployeeBankDetail PK__Employee__3214EC072E9F930F; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeBankDetail"
    ADD CONSTRAINT "PK__Employee__3214EC072E9F930F" PRIMARY KEY ("Id");


--
-- Name: EmployeeLeavePolicyMapping PK__Employee__3214EC07305828A7; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeLeavePolicyMapping"
    ADD CONSTRAINT "PK__Employee__3214EC07305828A7" PRIMARY KEY ("Id");


--
-- Name: EmployeeContact PK__Employee__3214EC07371CFD87; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeContact"
    ADD CONSTRAINT "PK__Employee__3214EC07371CFD87" PRIMARY KEY ("Id");


--
-- Name: EmployeePersonalDetail PK__Employee__3214EC074796302D; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeePersonalDetail"
    ADD CONSTRAINT "PK__Employee__3214EC074796302D" PRIMARY KEY ("Id");


--
-- Name: EmployeeDependent PK__Employee__3214EC074B6A13E2; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeDependent"
    ADD CONSTRAINT "PK__Employee__3214EC074B6A13E2" PRIMARY KEY ("Id");


--
-- Name: EmployeeIdentity PK__Employee__3214EC074FF9BCC1; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeIdentity"
    ADD CONSTRAINT "PK__Employee__3214EC074FF9BCC1" PRIMARY KEY ("Id");


--
-- Name: EmployeeManagerMapping PK__Employee__3214EC075BB6D3FC; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeManagerMapping"
    ADD CONSTRAINT "PK__Employee__3214EC075BB6D3FC" PRIMARY KEY ("Id");


--
-- Name: EmployeeWorkProfile PK__Employee__3214EC07605A7EB3; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeWorkProfile"
    ADD CONSTRAINT "PK__Employee__3214EC07605A7EB3" PRIMARY KEY ("Id");


--
-- Name: EmployeeType PK__Employee__3214EC0760C5ED38; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeType"
    ADD CONSTRAINT "PK__Employee__3214EC0760C5ED38" PRIMARY KEY ("Id");


--
-- Name: EmployeesChangedTypeHistory PK__Employee__3214EC0779153EAD; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeesChangedTypeHistory"
    ADD CONSTRAINT "PK__Employee__3214EC0779153EAD" PRIMARY KEY ("Id");


--
-- Name: EmployeeDailyAttendance PK__Employee__3214EC078C36F2DB; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeDailyAttendance"
    ADD CONSTRAINT "PK__Employee__3214EC078C36F2DB" PRIMARY KEY ("Id");


--
-- Name: EmployeeWorkHistory PK__Employee__3214EC07994D46FA; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeWorkHistory"
    ADD CONSTRAINT "PK__Employee__3214EC07994D46FA" PRIMARY KEY ("Id");


--
-- Name: EmployeeTypeBasicMenu PK__Employee__3214EC07A773CB3F; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeTypeBasicMenu"
    ADD CONSTRAINT "PK__Employee__3214EC07A773CB3F" PRIMARY KEY ("Id");


--
-- Name: EmployeeStatutoryAccount PK__Employee__3214EC07ACDD4E51; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeStatutoryAccount"
    ADD CONSTRAINT "PK__Employee__3214EC07ACDD4E51" PRIMARY KEY ("Id");


--
-- Name: EmployeeLeaveBalance PK__Employee__3214EC07B6CC8062; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeLeaveBalance"
    ADD CONSTRAINT "PK__Employee__3214EC07B6CC8062" PRIMARY KEY ("Id");


--
-- Name: EmployeeCategorySkill PK__Employee__3214EC07BB3C49C9; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeCategorySkill"
    ADD CONSTRAINT "PK__Employee__3214EC07BB3C49C9" PRIMARY KEY ("Id");


--
-- Name: EmployeeWorkDocument PK__Employee__3214EC07BDFE60B6; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeWorkDocument"
    ADD CONSTRAINT "PK__Employee__3214EC07BDFE60B6" PRIMARY KEY ("Id");


--
-- Name: EmployeeImage PK__Employee__3214EC07C583D80F; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeImage"
    ADD CONSTRAINT "PK__Employee__3214EC07C583D80F" PRIMARY KEY ("Id");


--
-- Name: Employee PK__Employee__3214EC07E3264254; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."Employee"
    ADD CONSTRAINT "PK__Employee__3214EC07E3264254" PRIMARY KEY ("Id");


--
-- Name: ForgotPasswordOTPDetail PK__ForgotPa__3214EC071E853072; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."ForgotPasswordOTPDetail"
    ADD CONSTRAINT "PK__ForgotPa__3214EC071E853072" PRIMARY KEY ("Id");


--
-- Name: Gender PK__Gender__3214EC07EF3CD03D; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."Gender"
    ADD CONSTRAINT "PK__Gender__3214EC07EF3CD03D" PRIMARY KEY ("Id");


--
-- Name: IdentityCategory PK__Identity__3214EC07393AF557; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."IdentityCategory"
    ADD CONSTRAINT "PK__Identity__3214EC07393AF557" PRIMARY KEY ("Id");


--
-- Name: IdentityCategoryDocument PK__Identity__3214EC07AA61914F; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."IdentityCategoryDocument"
    ADD CONSTRAINT "PK__Identity__3214EC07AA61914F" PRIMARY KEY ("Id");


--
-- Name: LeaveRequest PK__LeaveReq__3214EC07D95D9BAD; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."LeaveRequest"
    ADD CONSTRAINT "PK__LeaveReq__3214EC07D95D9BAD" PRIMARY KEY ("Id");


--
-- Name: LeaveType PK__LeaveTyp__3214EC0788A5EF9C; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."LeaveType"
    ADD CONSTRAINT "PK__LeaveTyp__3214EC0788A5EF9C" PRIMARY KEY ("Id");


--
-- Name: License PK__License__3214EC07FF575687; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."License"
    ADD CONSTRAINT "PK__License__3214EC07FF575687" PRIMARY KEY ("Id");


--
-- Name: LoginCredential PK__LoginCre__3214EC0750429DFA; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."LoginCredential"
    ADD CONSTRAINT "PK__LoginCre__3214EC0750429DFA" PRIMARY KEY ("Id");


--
-- Name: ModuleOperationMapping PK__ModuleOp__3214EC07BF86A196; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."ModuleOperationMapping"
    ADD CONSTRAINT "PK__ModuleOp__3214EC07BF86A196" PRIMARY KEY ("Id");


--
-- Name: Module PK__Module__3214EC078891AB2D; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."Module"
    ADD CONSTRAINT "PK__Module__3214EC078891AB2D" PRIMARY KEY ("Id");


--
-- Name: Operation PK__Operatio__3214EC079C437610; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."Operation"
    ADD CONSTRAINT "PK__Operatio__3214EC079C437610" PRIMARY KEY ("Id");


--
-- Name: PageTypeEnum PK__PageType__3214EC07D71C5D81; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PageTypeEnum"
    ADD CONSTRAINT "PK__PageType__3214EC07D71C5D81" PRIMARY KEY ("Id");


--
-- Name: PlanModuleMapping PK__PlanModu__3214EC0729948732; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PlanModuleMapping"
    ADD CONSTRAINT "PK__PlanModu__3214EC0729948732" PRIMARY KEY ("Id");


--
-- Name: RefreshToken PK__RefreshT__3214EC0731D02168; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."RefreshToken"
    ADD CONSTRAINT "PK__RefreshT__3214EC0731D02168" PRIMARY KEY ("Id");


--
-- Name: ReportingType PK__Reportin__3214EC0773736C22; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."ReportingType"
    ADD CONSTRAINT "PK__Reportin__3214EC0773736C22" PRIMARY KEY ("Id");


--
-- Name: RequestType PK__RequestT__3214EC07E53AD996; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."RequestType"
    ADD CONSTRAINT "PK__RequestT__3214EC07E53AD996" PRIMARY KEY ("Id");


--
-- Name: Role PK__Role__3214EC0723371271; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."Role"
    ADD CONSTRAINT "PK__Role__3214EC0723371271" PRIMARY KEY ("Id");


--
-- Name: State PK__State__3214EC0705AF0D74; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."State"
    ADD CONSTRAINT "PK__State__3214EC0705AF0D74" PRIMARY KEY ("Id");


--
-- Name: StatutoryType PK__Statutor__3214EC078EED6601; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."StatutoryType"
    ADD CONSTRAINT "PK__Statutor__3214EC078EED6601" PRIMARY KEY ("Id");


--
-- Name: SubscriptionPlan PK__Subscrip__3214EC07BED0F0D0; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."SubscriptionPlan"
    ADD CONSTRAINT "PK__Subscrip__3214EC07BED0F0D0" PRIMARY KEY ("Id");


--
-- Name: TenantEmailConfig PK__TenantEm__3214EC077A90B3A3; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TenantEmailConfig"
    ADD CONSTRAINT "PK__TenantEm__3214EC077A90B3A3" PRIMARY KEY ("Id");


--
-- Name: TenantEnabledOperation PK__TenantEn__3214EC07047C5B46; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TenantEnabledOperation"
    ADD CONSTRAINT "PK__TenantEn__3214EC07047C5B46" PRIMARY KEY ("Id");


--
-- Name: TenantEncryptionKeys PK__TenantEn__3214EC070E3540EE; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TenantEncryptionKeys"
    ADD CONSTRAINT "PK__TenantEn__3214EC070E3540EE" PRIMARY KEY ("Id");


--
-- Name: TenantEnabledModule PK__TenantEn__3214EC07AC610092; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TenantEnabledModule"
    ADD CONSTRAINT "PK__TenantEn__3214EC07AC610092" PRIMARY KEY ("Id");


--
-- Name: TenantIndustry PK__TenantIn__3214EC0765258C5E; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TenantIndustry"
    ADD CONSTRAINT "PK__TenantIn__3214EC0765258C5E" PRIMARY KEY ("Id");


--
-- Name: TenantProfile PK__TenantPr__3214EC0796A1B93D; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TenantProfile"
    ADD CONSTRAINT "PK__TenantPr__3214EC0796A1B93D" PRIMARY KEY ("Id");


--
-- Name: TenantSubscription PK__TenantSu__3214EC07CF143048; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TenantSubscription"
    ADD CONSTRAINT "PK__TenantSu__3214EC07CF143048" PRIMARY KEY ("Id");


--
-- Name: Tenant PK__Tenant__3214EC0728DD7C6E; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."Tenant"
    ADD CONSTRAINT "PK__Tenant__3214EC0728DD7C6E" PRIMARY KEY ("Id");


--
-- Name: Tender PK__Tender__3214EC076E847502; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."Tender"
    ADD CONSTRAINT "PK__Tender__3214EC076E847502" PRIMARY KEY ("Id");


--
-- Name: TicketClassification PK__TicketCl__3214EC07BA509F8F; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TicketClassification"
    ADD CONSTRAINT "PK__TicketCl__3214EC07BA509F8F" PRIMARY KEY ("Id");


--
-- Name: TicketHeader PK__TicketHe__3214EC07EADE3F1B; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TicketHeader"
    ADD CONSTRAINT "PK__TicketHe__3214EC07EADE3F1B" PRIMARY KEY ("Id");


--
-- Name: TicketType PK__TicketTy__3214EC07554DC568; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TicketType"
    ADD CONSTRAINT "PK__TicketTy__3214EC07554DC568" PRIMARY KEY ("Id");


--
-- Name: UserAttendanceSetting PK__UserAtte__3214EC0750CE7646; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."UserAttendanceSetting"
    ADD CONSTRAINT "PK__UserAtte__3214EC0750CE7646" PRIMARY KEY ("Id");


--
-- Name: UserRole PK__UserRole__3214EC07C9E25EB9; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."UserRole"
    ADD CONSTRAINT "PK__UserRole__3214EC07C9E25EB9" PRIMARY KEY ("Id");


--
-- Name: WorkDocumentType PK__WorkDocu__3214EC0710449AA0; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."WorkDocumentType"
    ADD CONSTRAINT "PK__WorkDocu__3214EC0710449AA0" PRIMARY KEY ("Id");


--
-- Name: PaymentAttempt PaymentAttempt_pkey; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PaymentAttempt"
    ADD CONSTRAINT "PaymentAttempt_pkey" PRIMARY KEY ("Id");


--
-- Name: PaymentGateway PaymentGateway_pkey; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PaymentGateway"
    ADD CONSTRAINT "PaymentGateway_pkey" PRIMARY KEY ("Id");


--
-- Name: PaymentTransaction PaymentTransaction_pkey; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PaymentTransaction"
    ADD CONSTRAINT "PaymentTransaction_pkey" PRIMARY KEY ("Id");


--
-- Name: PaymentWebhookEvent PaymentWebhookEvent_pkey; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PaymentWebhookEvent"
    ADD CONSTRAINT "PaymentWebhookEvent_pkey" PRIMARY KEY ("Id");


--
-- Name: PolicyAcknowledgement PolicyAcknowledgement_pkey; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyAcknowledgement"
    ADD CONSTRAINT "PolicyAcknowledgement_pkey" PRIMARY KEY ("Id");


--
-- Name: PolicyApplicability PolicyApplicability_pkey; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyApplicability"
    ADD CONSTRAINT "PolicyApplicability_pkey" PRIMARY KEY ("Id");


--
-- Name: PolicyApprovalHistory PolicyApprovalHistory_pkey; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyApprovalHistory"
    ADD CONSTRAINT "PolicyApprovalHistory_pkey" PRIMARY KEY ("Id");


--
-- Name: PolicyApprovalStage PolicyApprovalStage_pkey; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyApprovalStage"
    ADD CONSTRAINT "PolicyApprovalStage_pkey" PRIMARY KEY ("Id");


--
-- Name: PolicyAssignment PolicyAssignment_pkey; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyAssignment"
    ADD CONSTRAINT "PolicyAssignment_pkey" PRIMARY KEY ("Id");


--
-- Name: PolicyCategory PolicyCategory_pkey; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyCategory"
    ADD CONSTRAINT "PolicyCategory_pkey" PRIMARY KEY ("Id");


--
-- Name: PolicyChangeAudit PolicyChangeAudit_pkey; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyChangeAudit"
    ADD CONSTRAINT "PolicyChangeAudit_pkey" PRIMARY KEY ("Id");


--
-- Name: PolicyDocumentType PolicyDocumentType_pkey; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyDocumentType"
    ADD CONSTRAINT "PolicyDocumentType_pkey" PRIMARY KEY ("Id");


--
-- Name: PolicyDocument PolicyDocument_pkey; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyDocument"
    ADD CONSTRAINT "PolicyDocument_pkey" PRIMARY KEY ("Id");


--
-- Name: PolicyException PolicyException_pkey; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyException"
    ADD CONSTRAINT "PolicyException_pkey" PRIMARY KEY ("Id");


--
-- Name: PolicyRuleType PolicyRuleType_pkey; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyRuleType"
    ADD CONSTRAINT "PolicyRuleType_pkey" PRIMARY KEY ("Id");


--
-- Name: PolicyRule PolicyRule_pkey; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyRule"
    ADD CONSTRAINT "PolicyRule_pkey" PRIMARY KEY ("Id");


--
-- Name: PolicyStatus PolicyStatus_pkey; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyStatus"
    ADD CONSTRAINT "PolicyStatus_pkey" PRIMARY KEY ("Id");


--
-- Name: PolicyType PolicyType_pkey; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyType"
    ADD CONSTRAINT "PolicyType_pkey" PRIMARY KEY ("Id");


--
-- Name: PolicyVersion PolicyVersion_pkey; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyVersion"
    ADD CONSTRAINT "PolicyVersion_pkey" PRIMARY KEY ("Id");


--
-- Name: Policy Policy_pkey; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."Policy"
    ADD CONSTRAINT "Policy_pkey" PRIMARY KEY ("Id");


--
-- Name: SubscriptionPlanPrice SubscriptionPlanPrice_pkey; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."SubscriptionPlanPrice"
    ADD CONSTRAINT "SubscriptionPlanPrice_pkey" PRIMARY KEY ("Id");


--
-- Name: TenantBillingProfile TenantBillingProfile_pkey; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TenantBillingProfile"
    ADD CONSTRAINT "TenantBillingProfile_pkey" PRIMARY KEY ("Id");


--
-- Name: TenantBillingSubscription TenantBillingSubscription_pkey; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TenantBillingSubscription"
    ADD CONSTRAINT "TenantBillingSubscription_pkey" PRIMARY KEY ("Id");


--
-- Name: TenantCardMaster TenantCardMaster_pkey; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TenantCardMaster"
    ADD CONSTRAINT "TenantCardMaster_pkey" PRIMARY KEY ("Id");


--
-- Name: TenantDeviceConfiguration TenantDeviceConfiguration_pkey; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TenantDeviceConfiguration"
    ADD CONSTRAINT "TenantDeviceConfiguration_pkey" PRIMARY KEY ("Id");


--
-- Name: TenantEmailTemplate TenantEmailTemplate_pkey; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TenantEmailTemplate"
    ADD CONSTRAINT "TenantEmailTemplate_pkey" PRIMARY KEY ("Id");


--
-- Name: TenantEmployeeSectionDefault TenantEmployeeSectionDefault_pkey; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TenantEmployeeSectionDefault"
    ADD CONSTRAINT "TenantEmployeeSectionDefault_pkey" PRIMARY KEY ("TenantId", "ModuleCode");


--
-- Name: ThreadMessage ThreadMessage_pkey; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."ThreadMessage"
    ADD CONSTRAINT "ThreadMessage_pkey" PRIMARY KEY ("Id");


--
-- Name: TicketThread Thread_pkey; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TicketThread"
    ADD CONSTRAINT "Thread_pkey" PRIMARY KEY ("Id");


--
-- Name: TicketAttachment TicketAttachment_pkey; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TicketAttachment"
    ADD CONSTRAINT "TicketAttachment_pkey" PRIMARY KEY ("Id");


--
-- Name: TicketHistory TicketHistory_pkey; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TicketHistory"
    ADD CONSTRAINT "TicketHistory_pkey" PRIMARY KEY ("Id");


--
-- Name: Ticket Ticket_pkey; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."Ticket"
    ADD CONSTRAINT "Ticket_pkey" PRIMARY KEY ("Id");


--
-- Name: BillingInvoice UQ_BillingInvoice_Number; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."BillingInvoice"
    ADD CONSTRAINT "UQ_BillingInvoice_Number" UNIQUE ("InvoiceNumber");


--
-- Name: BillingInvoice UQ_BillingInvoice_Order; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."BillingInvoice"
    ADD CONSTRAINT "UQ_BillingInvoice_Order" UNIQUE ("BillingOrderId");


--
-- Name: BillingOrder UQ_BillingOrder_Idempotency; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."BillingOrder"
    ADD CONSTRAINT "UQ_BillingOrder_Idempotency" UNIQUE ("IdempotencyKey");


--
-- Name: BillingRefund UQ_BillingRefund_Idempotency; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."BillingRefund"
    ADD CONSTRAINT "UQ_BillingRefund_Idempotency" UNIQUE ("IdempotencyKey");


--
-- Name: ComplianceTypeMaster UQ_ComplianceType_Country_Name; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."ComplianceTypeMaster"
    ADD CONSTRAINT "UQ_ComplianceType_Country_Name" UNIQUE ("CountryId", "Name");


--
-- Name: HostRoleModuleAndPermission UQ_HostRole_Module_Operation; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."HostRoleModuleAndPermission"
    ADD CONSTRAINT "UQ_HostRole_Module_Operation" UNIQUE ("HostRoleId", "ModuleId", "OperationId");


--
-- Name: HostRole UQ_HostRole_Name; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."HostRole"
    ADD CONSTRAINT "UQ_HostRole_Name" UNIQUE ("Name");


--
-- Name: HostUser UQ_HostUser_LoginId; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."HostUser"
    ADD CONSTRAINT "UQ_HostUser_LoginId" UNIQUE ("LoginId");


--
-- Name: LocalityType UQ_LocalityType_TypeName; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."LocalityType"
    ADD CONSTRAINT "UQ_LocalityType_TypeName" UNIQUE ("TypeName");


--
-- Name: LoginCredential UQ_LoginId; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."LoginCredential"
    ADD CONSTRAINT "UQ_LoginId" UNIQUE ("LoginId");


--
-- Name: PaymentAttempt UQ_PaymentAttempt_Number; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PaymentAttempt"
    ADD CONSTRAINT "UQ_PaymentAttempt_Number" UNIQUE ("BillingOrderId", "AttemptNumber");


--
-- Name: PaymentGateway UQ_PaymentGateway_Code; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PaymentGateway"
    ADD CONSTRAINT "UQ_PaymentGateway_Code" UNIQUE ("GatewayCode");


--
-- Name: PaymentWebhookEvent UQ_PaymentWebhookEvent_GatewayEvent; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PaymentWebhookEvent"
    ADD CONSTRAINT "UQ_PaymentWebhookEvent_GatewayEvent" UNIQUE ("PaymentGatewayId", "GatewayEventId");


--
-- Name: PayrollEmployee UQ_PayrollEmployee; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PayrollEmployee"
    ADD CONSTRAINT "UQ_PayrollEmployee" UNIQUE ("PayrollRunId", "EmployeeId");


--
-- Name: PayrollRun UQ_Payroll_Month; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PayrollRun"
    ADD CONSTRAINT "UQ_Payroll_Month" UNIQUE ("TenantId", "PayrollMonth", "PayrollYear");


--
-- Name: PolicyAcknowledgement UQ_PolicyAcknowledgement_Version_Employee; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyAcknowledgement"
    ADD CONSTRAINT "UQ_PolicyAcknowledgement_Version_Employee" UNIQUE ("PolicyVersionId", "EmployeeId");


--
-- Name: PolicyApprovalHistory UQ_PolicyApprovalHistory_Version_Sequence; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyApprovalHistory"
    ADD CONSTRAINT "UQ_PolicyApprovalHistory_Version_Sequence" UNIQUE ("PolicyVersionId", "SequenceNumber");


--
-- Name: PolicyApprovalStage UQ_PolicyApprovalStage_Tenant_Category_Order; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyApprovalStage"
    ADD CONSTRAINT "UQ_PolicyApprovalStage_Tenant_Category_Order" UNIQUE ("TenantId", "PolicyCategoryId", "StageOrder");


--
-- Name: PolicyAssignment UQ_PolicyAssignment_Version_Employee_From; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyAssignment"
    ADD CONSTRAINT "UQ_PolicyAssignment_Version_Employee_From" UNIQUE ("PolicyVersionId", "EmployeeId", "EffectiveFrom");


--
-- Name: PolicyCategory UQ_PolicyCategory_Code; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyCategory"
    ADD CONSTRAINT "UQ_PolicyCategory_Code" UNIQUE ("CategoryCode");


--
-- Name: PolicyDocumentType UQ_PolicyDocumentType_Code; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyDocumentType"
    ADD CONSTRAINT "UQ_PolicyDocumentType_Code" UNIQUE ("DocumentTypeCode");


--
-- Name: PolicyDocument UQ_PolicyDocument_Version_Object; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyDocument"
    ADD CONSTRAINT "UQ_PolicyDocument_Version_Object" UNIQUE ("PolicyVersionId", "ObjectKey");


--
-- Name: PolicyRuleType UQ_PolicyRuleType_Code; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyRuleType"
    ADD CONSTRAINT "UQ_PolicyRuleType_Code" UNIQUE ("RuleTypeCode");


--
-- Name: PolicyRule UQ_PolicyRule_Version_Order; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyRule"
    ADD CONSTRAINT "UQ_PolicyRule_Version_Order" UNIQUE ("PolicyVersionId", "RuleOrder");


--
-- Name: PolicyStatus UQ_PolicyStatus_Code; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyStatus"
    ADD CONSTRAINT "UQ_PolicyStatus_Code" UNIQUE ("StatusCode");


--
-- Name: PolicyVersion UQ_PolicyVersion_Policy_Version; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyVersion"
    ADD CONSTRAINT "UQ_PolicyVersion_Policy_Version" UNIQUE ("PolicyId", "VersionNumber");


--
-- Name: Policy UQ_Policy_Tenant_Code; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."Policy"
    ADD CONSTRAINT "UQ_Policy_Tenant_Code" UNIQUE ("TenantId", "PolicyCode");


--
-- Name: RequestType UQ_RequestType_Tenant; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."RequestType"
    ADD CONSTRAINT "UQ_RequestType_Tenant" UNIQUE ("TenantId", "RequestTypeName");


--
-- Name: SalaryComponentMaster UQ_SalaryComponent_Tenant_ComponentName_Effective; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."SalaryComponentMaster"
    ADD CONSTRAINT "UQ_SalaryComponent_Tenant_ComponentName_Effective" UNIQUE ("TenantId", "ComponentName", "EffectiveFrom");


--
-- Name: State UQ_State_Country_Name; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."State"
    ADD CONSTRAINT "UQ_State_Country_Name" UNIQUE ("CountryId", "StateName");


--
-- Name: TenantDeviceConfiguration UQ_TenantDeviceConfiguration_TenantDeviceId; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TenantDeviceConfiguration"
    ADD CONSTRAINT "UQ_TenantDeviceConfiguration_TenantDeviceId" UNIQUE ("TenantDeviceId");


--
-- Name: TenantEncryptionKeys UQ_TenantKey_TenantId; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TenantEncryptionKeys"
    ADD CONSTRAINT "UQ_TenantKey_TenantId" UNIQUE ("TenantId");


--
-- Name: TenantLocation UQ_TenantLocation_Id_TenantId; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TenantLocation"
    ADD CONSTRAINT "UQ_TenantLocation_Id_TenantId" UNIQUE ("Id", "TenantId");


--
-- Name: AssetStatus UQ__AssetSta__05E7698A7AA7A28E; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."AssetStatus"
    ADD CONSTRAINT "UQ__AssetSta__05E7698A7AA7A28E" UNIQUE ("StatusName");


--
-- Name: AssetType UQ__AssetTyp__D4E7DFA8692FD5DF; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."AssetType"
    ADD CONSTRAINT "UQ__AssetTyp__D4E7DFA8692FD5DF" UNIQUE ("TypeName");


--
-- Name: Gender UQ__Gender__F7C177153AF55502; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."Gender"
    ADD CONSTRAINT "UQ__Gender__F7C177153AF55502" UNIQUE ("GenderName");


--
-- Name: IdentityCategory UQ__Identity__A25C5AA7EC142E9A; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."IdentityCategory"
    ADD CONSTRAINT "UQ__Identity__A25C5AA7EC142E9A" UNIQUE ("Code");


--
-- Name: Tenant UQ__Tenant__F7C944DD7E3D53D9; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."Tenant"
    ADD CONSTRAINT "UQ__Tenant__F7C944DD7E3D53D9" UNIQUE ("TenantEmail");


--
-- Name: DefaultEmailConfig UX_DefaultEmailConfig_ConfigName; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."DefaultEmailConfig"
    ADD CONSTRAINT "UX_DefaultEmailConfig_ConfigName" UNIQUE ("ConfigName");


--
-- Name: DeviceCommand UX_DeviceCommand_InternalTrackingId; Type: CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."DeviceCommand"
    ADD CONSTRAINT "UX_DeviceCommand_InternalTrackingId" UNIQUE ("InternalTrackingId");


--
-- Name: IDX_RolesPermission_OperationId; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IDX_RolesPermission_OperationId" ON axionpro."RoleModuleAndPermission" USING btree ("OperationId");


--
-- Name: IDX_RolesPermission_RoleId; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IDX_RolesPermission_RoleId" ON axionpro."RoleModuleAndPermission" USING btree ("RoleId");


--
-- Name: IX_AttendancePolicy_PolicyTypeId; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_AttendancePolicy_PolicyTypeId" ON axionpro."AttendancePolicy" USING btree ("PolicyTypeId");


--
-- Name: IX_AttendancePolicy_TenantId; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_AttendancePolicy_TenantId" ON axionpro."AttendancePolicy" USING btree ("TenantId");


--
-- Name: IX_AttendancePolicy_TenantId_IsActive_IsSoftDeleted; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_AttendancePolicy_TenantId_IsActive_IsSoftDeleted" ON axionpro."AttendancePolicy" USING btree ("TenantId", "IsActive", "IsSoftDeleted");


--
-- Name: IX_BulkImportJob_Due; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_BulkImportJob_Due" ON axionpro."BulkImportJob" USING btree ("Status", "ScheduledAtUtc", "UpdatedAtUtc");


--
-- Name: IX_BulkImportJob_Owner; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_BulkImportJob_Owner" ON axionpro."BulkImportJob" USING btree ("TenantId", "ActorId", "Master", "CreatedAtUtc");


--
-- Name: IX_DeviceCommandResponse_TenantDevice_Received; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_DeviceCommandResponse_TenantDevice_Received" ON axionpro."DeviceCommandResponse" USING btree ("TenantDeviceId", "ReceivedDateTime");


--
-- Name: IX_DeviceCommand_DeviceSerial_Order; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_DeviceCommand_DeviceSerial_Order" ON axionpro."DeviceCommand" USING btree ("DeviceSerialNumber", "AddedDateTime");


--
-- Name: IX_DeviceCommand_Dispatch; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_DeviceCommand_Dispatch" ON axionpro."DeviceCommand" USING btree ("Status", "NextAttemptDateTime", "AddedDateTime");


--
-- Name: IX_DeviceCommand_TenantId; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_DeviceCommand_TenantId" ON axionpro."DeviceCommand" USING btree ("TenantId");


--
-- Name: IX_DeviceCommand_TenantLocationId; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_DeviceCommand_TenantLocationId" ON axionpro."DeviceCommand" USING btree ("TenantLocationId");


--
-- Name: IX_DeviceInitialProvisioning_DeviceMaster_Active; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_DeviceInitialProvisioning_DeviceMaster_Active" ON axionpro."DeviceInitialProvisioning" USING btree ("DeviceMasterId", "RevokedDateTime", "ExpiresDateTime");


--
-- Name: IX_DeviceMaster_CompanyName; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_DeviceMaster_CompanyName" ON axionpro."DeviceMaster" USING btree ("CompanyName");


--
-- Name: IX_DeviceMaster_DeviceType; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_DeviceMaster_DeviceType" ON axionpro."DeviceMaster" USING btree ("DeviceType");


--
-- Name: IX_DeviceMaster_IsActive_IsSoftDeleted; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_DeviceMaster_IsActive_IsSoftDeleted" ON axionpro."DeviceMaster" USING btree ("IsActive", "IsSoftDeleted");


--
-- Name: IX_DeviceMaster_IsAttendanceDevice; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_DeviceMaster_IsAttendanceDevice" ON axionpro."DeviceMaster" USING btree ("IsAttendanceDevice");


--
-- Name: IX_DeviceMaster_IsIntegrationSupported; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_DeviceMaster_IsIntegrationSupported" ON axionpro."DeviceMaster" USING btree ("IsIntegrationSupported");


--
-- Name: IX_DeviceMaster_ModelNo; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_DeviceMaster_ModelNo" ON axionpro."DeviceMaster" USING btree ("ModelNo");


--
-- Name: IX_DeviceMessageLog_TenantDevice_Occurred; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_DeviceMessageLog_TenantDevice_Occurred" ON axionpro."DeviceMessageLog" USING btree ("TenantDeviceId", "OccurredDateTime");


--
-- Name: IX_DeviceMessageLog_Topic_PayloadHash; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_DeviceMessageLog_Topic_PayloadHash" ON axionpro."DeviceMessageLog" USING btree ("Topic", "PayloadHash");


--
-- Name: IX_District_DistrictName; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_District_DistrictName" ON axionpro."District" USING btree ("DistrictName");


--
-- Name: IX_District_StateId; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_District_StateId" ON axionpro."District" USING btree ("StateId");


--
-- Name: IX_EmailQueue_Pending; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_EmailQueue_Pending" ON axionpro."EmailQueue" USING btree ("IsSent", "IsProcessing", "AddedDateTime", "Id");


--
-- Name: IX_EmployeeAttendancePunch_DayTimeline; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_EmployeeAttendancePunch_DayTimeline" ON axionpro."EmployeeAttendancePunch" USING btree ("TenantId", "EmployeeId", "WorkDate", "OccurredAtUtc", "Id");


--
-- Name: IX_EmployeeDeviceEnrollment_EmployeeId; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_EmployeeDeviceEnrollment_EmployeeId" ON axionpro."EmployeeDeviceEnrollment" USING btree ("EmployeeId");


--
-- Name: IX_EmployeeDeviceEnrollment_TenantDeviceId; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_EmployeeDeviceEnrollment_TenantDeviceId" ON axionpro."EmployeeDeviceEnrollment" USING btree ("TenantDeviceId");


--
-- Name: IX_EmployeeDeviceEnrollment_TenantId; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_EmployeeDeviceEnrollment_TenantId" ON axionpro."EmployeeDeviceEnrollment" USING btree ("TenantId");


--
-- Name: IX_EmployeeLocationAssignment_EmployeeId; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_EmployeeLocationAssignment_EmployeeId" ON axionpro."EmployeeLocationAssignment" USING btree ("EmployeeId");


--
-- Name: IX_EmployeeLocationAssignment_Employee_Location_EffectiveFrom; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_EmployeeLocationAssignment_Employee_Location_EffectiveFrom" ON axionpro."EmployeeLocationAssignment" USING btree ("TenantId", "EmployeeId", "TenantLocationId", "EffectiveFrom");


--
-- Name: IX_EmployeeLocationAssignment_Primary_EffectiveFrom; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_EmployeeLocationAssignment_Primary_EffectiveFrom" ON axionpro."EmployeeLocationAssignment" USING btree ("TenantId", "EmployeeId", "IsPrimary", "EffectiveFrom");


--
-- Name: IX_EmployeeLocationAssignment_TenantId; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_EmployeeLocationAssignment_TenantId" ON axionpro."EmployeeLocationAssignment" USING btree ("TenantId");


--
-- Name: IX_EmployeeLocationAssignment_TenantLocationId; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_EmployeeLocationAssignment_TenantLocationId" ON axionpro."EmployeeLocationAssignment" USING btree ("TenantLocationId");


--
-- Name: IX_EmployeeWorkArrangement_AttendancePolicyId; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_EmployeeWorkArrangement_AttendancePolicyId" ON axionpro."EmployeeWorkArrangement" USING btree ("AttendancePolicyId");


--
-- Name: IX_EmployeeWorkArrangement_EmployeeId; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_EmployeeWorkArrangement_EmployeeId" ON axionpro."EmployeeWorkArrangement" USING btree ("EmployeeId");


--
-- Name: IX_EmployeeWorkArrangement_Employee_EffectiveFrom; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_EmployeeWorkArrangement_Employee_EffectiveFrom" ON axionpro."EmployeeWorkArrangement" USING btree ("TenantId", "EmployeeId", "EffectiveFrom");


--
-- Name: IX_EmployeeWorkArrangement_PolicyVersionId; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_EmployeeWorkArrangement_PolicyVersionId" ON axionpro."EmployeeWorkArrangement" USING btree ("PolicyVersionId");


--
-- Name: IX_EmployeeWorkArrangement_PrimaryLocationId; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_EmployeeWorkArrangement_PrimaryLocationId" ON axionpro."EmployeeWorkArrangement" USING btree ("PrimaryTenantLocationId");


--
-- Name: IX_EmployeeWorkArrangement_TenantId; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_EmployeeWorkArrangement_TenantId" ON axionpro."EmployeeWorkArrangement" USING btree ("TenantId");


--
-- Name: IX_EmployeeWorkPattern_TenantLocationId; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_EmployeeWorkPattern_TenantLocationId" ON axionpro."EmployeeWorkPattern" USING btree ("TenantLocationId");


--
-- Name: IX_EmployeeWorkPattern_WorkArrangementId; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_EmployeeWorkPattern_WorkArrangementId" ON axionpro."EmployeeWorkPattern" USING btree ("EmployeeWorkArrangementId");


--
-- Name: IX_HRMP_HostRoleId; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_HRMP_HostRoleId" ON axionpro."HostRoleModuleAndPermission" USING btree ("HostRoleId");


--
-- Name: IX_HRMP_ModuleId; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_HRMP_ModuleId" ON axionpro."HostRoleModuleAndPermission" USING btree ("ModuleId");


--
-- Name: IX_HRMP_OperationId; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_HRMP_OperationId" ON axionpro."HostRoleModuleAndPermission" USING btree ("OperationId");


--
-- Name: IX_Holiday_Location_Date; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_Holiday_Location_Date" ON axionpro."Holiday" USING btree ("TenantLocationId", "HolidayDate");


--
-- Name: IX_HostUser_LoginId; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_HostUser_LoginId" ON axionpro."HostUser" USING btree ("LoginId");


--
-- Name: IX_Locality_DistrictId; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_Locality_DistrictId" ON axionpro."Locality" USING btree ("DistrictId");


--
-- Name: IX_Locality_LocalityTypeId; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_Locality_LocalityTypeId" ON axionpro."Locality" USING btree ("LocalityTypeId");


--
-- Name: IX_PolicyAcknowledgement_Employee; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_PolicyAcknowledgement_Employee" ON axionpro."PolicyAcknowledgement" USING btree ("TenantId", "EmployeeId", "AcknowledgementStatus");


--
-- Name: IX_PolicyApplicability_Resolution; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_PolicyApplicability_Resolution" ON axionpro."PolicyApplicability" USING btree ("TenantId", "PolicyVersionId", "Priority", "EffectiveFrom", "EffectiveTo") WHERE ("IsActive" = true);


--
-- Name: IX_PolicyAssignment_Employee; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_PolicyAssignment_Employee" ON axionpro."PolicyAssignment" USING btree ("TenantId", "EmployeeId", "EffectiveFrom", "EffectiveTo") WHERE ("IsActive" = true);


--
-- Name: IX_PolicyChangeAudit_Policy; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_PolicyChangeAudit_Policy" ON axionpro."PolicyChangeAudit" USING btree ("TenantId", "PolicyId", "ChangedDateTime" DESC);


--
-- Name: IX_PolicyException_Employee; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_PolicyException_Employee" ON axionpro."PolicyException" USING btree ("TenantId", "EmployeeId", "EffectiveFrom", "EffectiveTo") WHERE ("IsActive" = true);


--
-- Name: IX_RefreshToken_HostUserId; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_RefreshToken_HostUserId" ON axionpro."RefreshToken" USING btree ("HostUserId");


--
-- Name: IX_RefreshToken_LoginCredentialId; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_RefreshToken_LoginCredentialId" ON axionpro."RefreshToken" USING btree ("LoginCredentialId");


--
-- Name: IX_RefreshToken_LoginId_UserType; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_RefreshToken_LoginId_UserType" ON axionpro."RefreshToken" USING btree ("LoginId", "UserType");


--
-- Name: IX_SubscriptionPlan_IsSoftDeleted; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_SubscriptionPlan_IsSoftDeleted" ON axionpro."SubscriptionPlan" USING btree ("IsSoftDeleted");


--
-- Name: IX_TenantCardMaster_TenantId; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_TenantCardMaster_TenantId" ON axionpro."TenantCardMaster" USING btree ("TenantId");


--
-- Name: IX_TenantDeviceConfiguration_TenantDeviceId; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_TenantDeviceConfiguration_TenantDeviceId" ON axionpro."TenantDeviceConfiguration" USING btree ("TenantDeviceId");


--
-- Name: IX_TenantDevice_DeviceMasterId; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_TenantDevice_DeviceMasterId" ON axionpro."TenantDevice" USING btree ("DeviceMasterId");


--
-- Name: IX_TenantDevice_DeviceMasterId_IsActive; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_TenantDevice_DeviceMasterId_IsActive" ON axionpro."TenantDevice" USING btree ("DeviceMasterId", "IsActive");


--
-- Name: IX_TenantDevice_TenantId; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_TenantDevice_TenantId" ON axionpro."TenantDevice" USING btree ("TenantId");


--
-- Name: IX_TenantDevice_TenantId_IsActive_IsSoftDeleted; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_TenantDevice_TenantId_IsActive_IsSoftDeleted" ON axionpro."TenantDevice" USING btree ("TenantId", "IsActive", "IsSoftDeleted");


--
-- Name: IX_TenantDevice_TenantId_TenantLocationId; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_TenantDevice_TenantId_TenantLocationId" ON axionpro."TenantDevice" USING btree ("TenantId", "TenantLocationId");


--
-- Name: IX_TenantDevice_TenantLocationId; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_TenantDevice_TenantLocationId" ON axionpro."TenantDevice" USING btree ("TenantLocationId");


--
-- Name: IX_TenantDevice_TenantLocationId_IsActive; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_TenantDevice_TenantLocationId_IsActive" ON axionpro."TenantDevice" USING btree ("TenantLocationId", "IsActive");


--
-- Name: IX_TenantLocation_CityId; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_TenantLocation_CityId" ON axionpro."TenantLocation" USING btree ("CityId");


--
-- Name: IX_TenantLocation_CountryId; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_TenantLocation_CountryId" ON axionpro."TenantLocation" USING btree ("CountryId");


--
-- Name: IX_TenantLocation_DistrictId; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_TenantLocation_DistrictId" ON axionpro."TenantLocation" USING btree ("DistrictId");


--
-- Name: IX_TenantLocation_StateId; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_TenantLocation_StateId" ON axionpro."TenantLocation" USING btree ("StateId");


--
-- Name: IX_TenantLocation_TenantId; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_TenantLocation_TenantId" ON axionpro."TenantLocation" USING btree ("TenantId");


--
-- Name: IX_TenantLocation_TenantId_IsActive; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_TenantLocation_TenantId_IsActive" ON axionpro."TenantLocation" USING btree ("TenantId", "IsActive");


--
-- Name: IX_TenantLocation_TenantId_IsSoftDeleted; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_TenantLocation_TenantId_IsSoftDeleted" ON axionpro."TenantLocation" USING btree ("TenantId", "IsSoftDeleted");


--
-- Name: IX_WorkModeOverride_ApprovalStatus; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_WorkModeOverride_ApprovalStatus" ON axionpro."EmployeeWorkModeOverrideRequest" USING btree ("ApprovalStatus");


--
-- Name: IX_WorkModeOverride_EmployeeId; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_WorkModeOverride_EmployeeId" ON axionpro."EmployeeWorkModeOverrideRequest" USING btree ("EmployeeId");


--
-- Name: IX_WorkModeOverride_Employee_Date; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_WorkModeOverride_Employee_Date" ON axionpro."EmployeeWorkModeOverrideRequest" USING btree ("EmployeeId", "FromDate", "ToDate");


--
-- Name: IX_WorkModeOverride_TenantId; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE INDEX "IX_WorkModeOverride_TenantId" ON axionpro."EmployeeWorkModeOverrideRequest" USING btree ("TenantId");


--
-- Name: UQ_Structure_Order; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE UNIQUE INDEX "UQ_Structure_Order" ON axionpro."SalaryStructureDetail" USING btree ("SalaryStructureId", "CalculationOrder");


--
-- Name: UQ_Tenant_Module_Operation; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE UNIQUE INDEX "UQ_Tenant_Module_Operation" ON axionpro."TenantEnabledOperation" USING btree ("TenantId", "ModuleId", "OperationId");


--
-- Name: UX_AttendanceDeviceType_DeviceTypeCode; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE UNIQUE INDEX "UX_AttendanceDeviceType_DeviceTypeCode" ON axionpro."AttendanceDeviceType" USING btree (upper(btrim(("DeviceTypeCode")::text)));


--
-- Name: UX_AttendancePolicyVersionConfiguration_PolicyVersion; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE UNIQUE INDEX "UX_AttendancePolicyVersionConfiguration_PolicyVersion" ON axionpro."AttendancePolicyVersionConfiguration" USING btree ("PolicyVersionId");


--
-- Name: UX_AttendancePolicyVersionConfiguration_TenantVersion; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE UNIQUE INDEX "UX_AttendancePolicyVersionConfiguration_TenantVersion" ON axionpro."AttendancePolicyVersionConfiguration" USING btree ("TenantId", "PolicyVersionId");


--
-- Name: UX_AttendancePolicy_TenantId_PolicyName; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE UNIQUE INDEX "UX_AttendancePolicy_TenantId_PolicyName" ON axionpro."AttendancePolicy" USING btree ("TenantId", lower(("PolicyName")::text)) WHERE ("IsSoftDeleted" = false);


--
-- Name: UX_BillingOrder_GatewayOrder; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE UNIQUE INDEX "UX_BillingOrder_GatewayOrder" ON axionpro."BillingOrder" USING btree ("PaymentGatewayId", "GatewayOrderId") WHERE ("GatewayOrderId" IS NOT NULL);


--
-- Name: UX_BillingRefund_GatewayRefund; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE UNIQUE INDEX "UX_BillingRefund_GatewayRefund" ON axionpro."BillingRefund" USING btree ("GatewayRefundId") WHERE ("GatewayRefundId" IS NOT NULL);


--
-- Name: UX_BillingTaxRule_Live; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE UNIQUE INDEX "UX_BillingTaxRule_Live" ON axionpro."BillingTaxRule" USING btree ("CountryCode", "TaxCode", "EffectiveFrom") WHERE ("IsActive" = true);


--
-- Name: UX_DefaultEmailConfig_OneDefault; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE UNIQUE INDEX "UX_DefaultEmailConfig_OneDefault" ON axionpro."DefaultEmailConfig" USING btree ("IsDefault") WHERE ("IsDefault" = true);


--
-- Name: UX_Department_Tenant_Name_Live; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE UNIQUE INDEX "UX_Department_Tenant_Name_Live" ON axionpro."Department" USING btree ("TenantId", lower(btrim(("DepartmentName")::text))) WHERE ("IsSoftDeleted" IS NOT TRUE);


--
-- Name: UX_Designation_Tenant_Department_Name_Live; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE UNIQUE INDEX "UX_Designation_Tenant_Department_Name_Live" ON axionpro."Designation" USING btree ("TenantId", "DepartmentId", lower(btrim(("DesignationName")::text))) WHERE ("IsSoftDeleted" IS NOT TRUE);


--
-- Name: UX_DeviceCommandResponse_DeviceCommand; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE UNIQUE INDEX "UX_DeviceCommandResponse_DeviceCommand" ON axionpro."DeviceCommandResponse" USING btree ("DeviceCommandId") WHERE ("DeviceCommandId" IS NOT NULL);


--
-- Name: UX_DeviceCommand_OneOutstandingSerial; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE UNIQUE INDEX "UX_DeviceCommand_OneOutstandingSerial" ON axionpro."DeviceCommand" USING btree ("DeviceSerialNumber") WHERE ("Status" = ANY (ARRAY[2, 3, 6]));


--
-- Name: UX_DeviceCredential_ActiveType; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE UNIQUE INDEX "UX_DeviceCredential_ActiveType" ON axionpro."DeviceCredential" USING btree ("TenantDeviceId", "CredentialType") WHERE ("IsActive" = true);


--
-- Name: UX_DeviceInitialProvisioning_IngressTokenHash; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE UNIQUE INDEX "UX_DeviceInitialProvisioning_IngressTokenHash" ON axionpro."DeviceInitialProvisioning" USING btree ("IngressTokenHash");


--
-- Name: UX_DeviceMaster_CompanyName_ModelNo; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE UNIQUE INDEX "UX_DeviceMaster_CompanyName_ModelNo" ON axionpro."DeviceMaster" USING btree (lower(("CompanyName")::text), lower(("ModelNo")::text)) WHERE ("IsSoftDeleted" = false);


--
-- Name: UX_DeviceMaster_DeviceCode; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE UNIQUE INDEX "UX_DeviceMaster_DeviceCode" ON axionpro."DeviceMaster" USING btree (lower(("DeviceCode")::text)) WHERE ("IsSoftDeleted" = false);


--
-- Name: UX_District_StateId_DistrictCode; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE UNIQUE INDEX "UX_District_StateId_DistrictCode" ON axionpro."District" USING btree ("StateId", "DistrictCode");


--
-- Name: UX_EmployeeAttendancePunch_Idempotency; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE UNIQUE INDEX "UX_EmployeeAttendancePunch_Idempotency" ON axionpro."EmployeeAttendancePunch" USING btree ("TenantId", "EmployeeId", "IdempotencyKey");


--
-- Name: UX_EmployeeDeviceEnrollment_Device_EnrollId; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE UNIQUE INDEX "UX_EmployeeDeviceEnrollment_Device_EnrollId" ON axionpro."EmployeeDeviceEnrollment" USING btree ("TenantDeviceId", lower(("EnrollId")::text)) WHERE ("IsSoftDeleted" = false);


--
-- Name: UX_EmployeeType_Tenant_Name_Live; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE UNIQUE INDEX "UX_EmployeeType_Tenant_Name_Live" ON axionpro."EmployeeType" USING btree ("TenantId", lower(btrim(("TypeName")::text))) WHERE (("TenantId" IS NOT NULL) AND ("IsSoftDeleted" IS NOT TRUE));


--
-- Name: UX_EmployeeWorkPattern_Arrangement_Day; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE UNIQUE INDEX "UX_EmployeeWorkPattern_Arrangement_Day" ON axionpro."EmployeeWorkPattern" USING btree ("EmployeeWorkArrangementId", "DayOfWeek") WHERE (("IsActive" = true) AND ("IsSoftDeleted" = false));


--
-- Name: UX_Employee_SystemUser_OnlyOnce; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE UNIQUE INDEX "UX_Employee_SystemUser_OnlyOnce" ON axionpro."Employee" USING btree ("OfficialEmail");


--
-- Name: UX_Employee_TenantIdNullOnce; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE UNIQUE INDEX "UX_Employee_TenantIdNullOnce" ON axionpro."Employee" USING btree ("Id");


--
-- Name: UX_Employee_TenantId_Null_OnlyOnce; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE UNIQUE INDEX "UX_Employee_TenantId_Null_OnlyOnce" ON axionpro."Employee" USING btree ("Id");


--
-- Name: UX_Employee_Tenant_Code; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE UNIQUE INDEX "UX_Employee_Tenant_Code" ON axionpro."Employee" USING btree ("TenantId", lower(btrim(("EmployementCode")::text))) WHERE (("EmployementCode" IS NOT NULL) AND (btrim(("EmployementCode")::text) <> ''::text));


--
-- Name: UX_Holiday_Tenant_Location_Date_NotDeleted; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE UNIQUE INDEX "UX_Holiday_Tenant_Location_Date_NotDeleted" ON axionpro."Holiday" USING btree ("TenantId", "TenantLocationId", "HolidayDate") WHERE ("IsSoftDeleted" IS DISTINCT FROM true);


--
-- Name: UX_HostBillingConfiguration_Active; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE UNIQUE INDEX "UX_HostBillingConfiguration_Active" ON axionpro."HostBillingConfiguration" USING btree ((true)) WHERE ("IsActive" = true);


--
-- Name: UX_Locality_DistrictId_LocalityCode; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE UNIQUE INDEX "UX_Locality_DistrictId_LocalityCode" ON axionpro."Locality" USING btree ("DistrictId", "LocalityCode");


--
-- Name: UX_LoginCredential_NormalizedLogin; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE UNIQUE INDEX "UX_LoginCredential_NormalizedLogin" ON axionpro."LoginCredential" USING btree (lower(btrim(("LoginId")::text)));


--
-- Name: UX_Module_PageName_CaseInsensitive; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE UNIQUE INDEX "UX_Module_PageName_CaseInsensitive" ON axionpro."Module" USING btree (lower(("PageName")::text)) WHERE ("PageName" IS NOT NULL);


--
-- Name: UX_PaymentTransaction_GatewayPayment; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE UNIQUE INDEX "UX_PaymentTransaction_GatewayPayment" ON axionpro."PaymentTransaction" USING btree ("GatewayPaymentId") WHERE ("GatewayPaymentId" IS NOT NULL);


--
-- Name: UX_PolicyType_Tenant_Code; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE UNIQUE INDEX "UX_PolicyType_Tenant_Code" ON axionpro."PolicyType" USING btree ("TenantId", "PolicyTypeCode") WHERE (("PolicyTypeCode" IS NOT NULL) AND (COALESCE("IsSoftDelete", false) = false));


--
-- Name: UX_PolicyVersion_Current; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE UNIQUE INDEX "UX_PolicyVersion_Current" ON axionpro."PolicyVersion" USING btree ("PolicyId") WHERE (("IsCurrent" = true) AND ("IsActive" = true));


--
-- Name: UX_Role_Tenant_Name_Live; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE UNIQUE INDEX "UX_Role_Tenant_Name_Live" ON axionpro."Role" USING btree ("TenantId", lower(btrim(("RoleName")::text))) WHERE ("IsSoftDeleted" IS NOT TRUE);


--
-- Name: UX_State_CountryId_StateCode; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE UNIQUE INDEX "UX_State_CountryId_StateCode" ON axionpro."State" USING btree ("CountryId", "StateCode");


--
-- Name: UX_SubscriptionPlanPrice_Live; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE UNIQUE INDEX "UX_SubscriptionPlanPrice_Live" ON axionpro."SubscriptionPlanPrice" USING btree ("SubscriptionPlanId", "CountryCode", "CurrencyCode", "BillingCycle", "EffectiveFrom") WHERE ("IsActive" = true);


--
-- Name: UX_TenantBillingProfile_Active; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE UNIQUE INDEX "UX_TenantBillingProfile_Active" ON axionpro."TenantBillingProfile" USING btree ("TenantId") WHERE ("IsActive" = true);


--
-- Name: UX_TenantBillingSubscription_Current; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE UNIQUE INDEX "UX_TenantBillingSubscription_Current" ON axionpro."TenantBillingSubscription" USING btree ("TenantId") WHERE (("Status")::text = ANY (ARRAY[('PendingAuthorization'::character varying)::text, ('Active'::character varying)::text, ('PastDue'::character varying)::text, ('GracePeriod'::character varying)::text]));


--
-- Name: UX_TenantBillingSubscription_Gateway; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE UNIQUE INDEX "UX_TenantBillingSubscription_Gateway" ON axionpro."TenantBillingSubscription" USING btree ("PaymentGatewayId", "GatewaySubscriptionId") WHERE ("GatewaySubscriptionId" IS NOT NULL);


--
-- Name: UX_TenantCardMaster_Tenant_CardHash_Live; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE UNIQUE INDEX "UX_TenantCardMaster_Tenant_CardHash_Live" ON axionpro."TenantCardMaster" USING btree ("TenantId", "CardNumberLookupHash") WHERE (NOT "IsSoftDeleted");


--
-- Name: UX_TenantDeviceConfiguration_HttpsIngressTokenHash; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE UNIQUE INDEX "UX_TenantDeviceConfiguration_HttpsIngressTokenHash" ON axionpro."TenantDeviceConfiguration" USING btree ("HttpsIngressTokenHash") WHERE ("HttpsIngressTokenHash" IS NOT NULL);


--
-- Name: UX_TenantDeviceConfiguration_PendingHttpsIngressTokenHash; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE UNIQUE INDEX "UX_TenantDeviceConfiguration_PendingHttpsIngressTokenHash" ON axionpro."TenantDeviceConfiguration" USING btree ("PendingHttpsIngressTokenHash") WHERE ("PendingHttpsIngressTokenHash" IS NOT NULL);


--
-- Name: UX_TenantDevice_DeviceMasterId_Active; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE UNIQUE INDEX "UX_TenantDevice_DeviceMasterId_Active" ON axionpro."TenantDevice" USING btree ("DeviceMasterId") WHERE ("IsSoftDeleted" = false);


--
-- Name: UX_TenantDevice_TenantId_DeviceCode; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE UNIQUE INDEX "UX_TenantDevice_TenantId_DeviceCode" ON axionpro."TenantDevice" USING btree ("TenantId", lower(("DeviceCode")::text)) WHERE ("IsSoftDeleted" = false);


--
-- Name: UX_TenantEmailTemplate_Tenant_TemplateCode; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE UNIQUE INDEX "UX_TenantEmailTemplate_Tenant_TemplateCode" ON axionpro."TenantEmailTemplate" USING btree ("TenantId", "TemplateCode");


--
-- Name: UX_TenantLocation_TenantId_LocationCode; Type: INDEX; Schema: axionpro; Owner: -
--

CREATE UNIQUE INDEX "UX_TenantLocation_TenantId_LocationCode" ON axionpro."TenantLocation" USING btree ("TenantId", lower(("LocationCode")::text)) WHERE ("IsSoftDeleted" = false);


--
-- Name: EmployeeType TR_EmployeeType_OwnerImmutable; Type: TRIGGER; Schema: axionpro; Owner: -
--

CREATE TRIGGER "TR_EmployeeType_OwnerImmutable" BEFORE UPDATE OF "TenantId" ON axionpro."EmployeeType" FOR EACH ROW EXECUTE FUNCTION axionpro.prevent_employee_type_owner_change();


--
-- Name: Employee TR_EmployeeType_Tenant; Type: TRIGGER; Schema: axionpro; Owner: -
--

CREATE TRIGGER "TR_EmployeeType_Tenant" BEFORE INSERT OR UPDATE ON axionpro."Employee" FOR EACH ROW EXECUTE FUNCTION axionpro.validate_employee_type_tenant();


--
-- Name: EmployeesChangedTypeHistory TR_EmployeeType_Tenant; Type: TRIGGER; Schema: axionpro; Owner: -
--

CREATE TRIGGER "TR_EmployeeType_Tenant" BEFORE INSERT OR UPDATE ON axionpro."EmployeesChangedTypeHistory" FOR EACH ROW EXECUTE FUNCTION axionpro.validate_employee_type_tenant();


--
-- Name: Module TR_Module_PageNameImmutable; Type: TRIGGER; Schema: axionpro; Owner: -
--

CREATE TRIGGER "TR_Module_PageNameImmutable" BEFORE UPDATE OF "PageName" ON axionpro."Module" FOR EACH ROW EXECUTE FUNCTION axionpro."PreventModulePageNameChange"();


--
-- Name: BillingInvoiceLine BillingInvoiceLine_BillingInvoiceId_fkey; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."BillingInvoiceLine"
    ADD CONSTRAINT "BillingInvoiceLine_BillingInvoiceId_fkey" FOREIGN KEY ("BillingInvoiceId") REFERENCES axionpro."BillingInvoice"("Id") ON DELETE RESTRICT;


--
-- Name: BillingInvoice BillingInvoice_BillingOrderId_fkey; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."BillingInvoice"
    ADD CONSTRAINT "BillingInvoice_BillingOrderId_fkey" FOREIGN KEY ("BillingOrderId") REFERENCES axionpro."BillingOrder"("Id") ON DELETE RESTRICT;


--
-- Name: BillingInvoice BillingInvoice_TenantId_fkey; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."BillingInvoice"
    ADD CONSTRAINT "BillingInvoice_TenantId_fkey" FOREIGN KEY ("TenantId") REFERENCES axionpro."Tenant"("Id") ON DELETE RESTRICT;


--
-- Name: BillingOrder BillingOrder_PaymentGatewayId_fkey; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."BillingOrder"
    ADD CONSTRAINT "BillingOrder_PaymentGatewayId_fkey" FOREIGN KEY ("PaymentGatewayId") REFERENCES axionpro."PaymentGateway"("Id") ON DELETE RESTRICT;


--
-- Name: BillingOrder BillingOrder_SubscriptionPlanPriceId_fkey; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."BillingOrder"
    ADD CONSTRAINT "BillingOrder_SubscriptionPlanPriceId_fkey" FOREIGN KEY ("SubscriptionPlanPriceId") REFERENCES axionpro."SubscriptionPlanPrice"("Id") ON DELETE RESTRICT;


--
-- Name: BillingOrder BillingOrder_TenantBillingSubscriptionId_fkey; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."BillingOrder"
    ADD CONSTRAINT "BillingOrder_TenantBillingSubscriptionId_fkey" FOREIGN KEY ("TenantBillingSubscriptionId") REFERENCES axionpro."TenantBillingSubscription"("Id") ON DELETE RESTRICT;


--
-- Name: BillingOrder BillingOrder_TenantId_fkey; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."BillingOrder"
    ADD CONSTRAINT "BillingOrder_TenantId_fkey" FOREIGN KEY ("TenantId") REFERENCES axionpro."Tenant"("Id") ON DELETE RESTRICT;


--
-- Name: BillingRefund BillingRefund_PaymentTransactionId_fkey; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."BillingRefund"
    ADD CONSTRAINT "BillingRefund_PaymentTransactionId_fkey" FOREIGN KEY ("PaymentTransactionId") REFERENCES axionpro."PaymentTransaction"("Id") ON DELETE RESTRICT;


--
-- Name: BillingSubscriptionChange BillingSubscriptionChange_FromSubscriptionPlanPriceId_fkey; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."BillingSubscriptionChange"
    ADD CONSTRAINT "BillingSubscriptionChange_FromSubscriptionPlanPriceId_fkey" FOREIGN KEY ("FromSubscriptionPlanPriceId") REFERENCES axionpro."SubscriptionPlanPrice"("Id") ON DELETE RESTRICT;


--
-- Name: BillingSubscriptionChange BillingSubscriptionChange_TenantBillingSubscriptionId_fkey; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."BillingSubscriptionChange"
    ADD CONSTRAINT "BillingSubscriptionChange_TenantBillingSubscriptionId_fkey" FOREIGN KEY ("TenantBillingSubscriptionId") REFERENCES axionpro."TenantBillingSubscription"("Id") ON DELETE RESTRICT;


--
-- Name: BillingSubscriptionChange BillingSubscriptionChange_ToSubscriptionPlanPriceId_fkey; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."BillingSubscriptionChange"
    ADD CONSTRAINT "BillingSubscriptionChange_ToSubscriptionPlanPriceId_fkey" FOREIGN KEY ("ToSubscriptionPlanPriceId") REFERENCES axionpro."SubscriptionPlanPrice"("Id") ON DELETE RESTRICT;


--
-- Name: EmployeeType EmployeeType_TenantId_fkey; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeType"
    ADD CONSTRAINT "EmployeeType_TenantId_fkey" FOREIGN KEY ("TenantId") REFERENCES axionpro."Tenant"("Id");


--
-- Name: AssetAssignment FK_AssetAssignment_Asset; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."AssetAssignment"
    ADD CONSTRAINT "FK_AssetAssignment_Asset" FOREIGN KEY ("AssetId") REFERENCES axionpro."Asset"("Id");


--
-- Name: AssetAssignment FK_AssetAssignment_Request; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."AssetAssignment"
    ADD CONSTRAINT "FK_AssetAssignment_Request" FOREIGN KEY ("RequestId") REFERENCES axionpro."AssetRequest"("Id");


--
-- Name: AssetCategory FK_AssetCategory_Tenant; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."AssetCategory"
    ADD CONSTRAINT "FK_AssetCategory_Tenant" FOREIGN KEY ("TenantId") REFERENCES axionpro."Tenant"("Id");


--
-- Name: AssetImage FK_AssetImage_Asset; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."AssetImage"
    ADD CONSTRAINT "FK_AssetImage_Asset" FOREIGN KEY ("AssetId") REFERENCES axionpro."Asset"("Id");


--
-- Name: AssetType FK_AssetType_AssetCategory; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."AssetType"
    ADD CONSTRAINT "FK_AssetType_AssetCategory" FOREIGN KEY ("AssetCategoryId") REFERENCES axionpro."AssetCategory"("Id");


--
-- Name: Asset FK_Asset_AssetStatus; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."Asset"
    ADD CONSTRAINT "FK_Asset_AssetStatus" FOREIGN KEY ("AssetStatusId") REFERENCES axionpro."AssetStatus"("Id");


--
-- Name: Asset FK_Asset_AssetType; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."Asset"
    ADD CONSTRAINT "FK_Asset_AssetType" FOREIGN KEY ("AssetTypeId") REFERENCES axionpro."AssetType"("Id");


--
-- Name: EmployeeDailyAttendance FK_AttendanceDeviceType; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeDailyAttendance"
    ADD CONSTRAINT "FK_AttendanceDeviceType" FOREIGN KEY ("AttendanceDeviceTypeId") REFERENCES axionpro."AttendanceDeviceType"("Id");


--
-- Name: AttendancePolicyVersionConfiguration FK_AttendancePolicyVersionConfiguration_PolicyVersion; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."AttendancePolicyVersionConfiguration"
    ADD CONSTRAINT "FK_AttendancePolicyVersionConfiguration_PolicyVersion" FOREIGN KEY ("PolicyVersionId") REFERENCES axionpro."PolicyVersion"("Id") ON DELETE CASCADE;


--
-- Name: AttendancePolicyVersionConfiguration FK_AttendancePolicyVersionConfiguration_Tenant; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."AttendancePolicyVersionConfiguration"
    ADD CONSTRAINT "FK_AttendancePolicyVersionConfiguration_Tenant" FOREIGN KEY ("TenantId") REFERENCES axionpro."Tenant"("Id") ON DELETE CASCADE;


--
-- Name: AttendancePolicy FK_AttendancePolicy_Tenant; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."AttendancePolicy"
    ADD CONSTRAINT "FK_AttendancePolicy_Tenant" FOREIGN KEY ("TenantId") REFERENCES axionpro."Tenant"("Id") ON DELETE RESTRICT;


--
-- Name: Category FK_Category_Parent; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."Category"
    ADD CONSTRAINT "FK_Category_Parent" FOREIGN KEY ("ParentID") REFERENCES axionpro."Category"("Id");


--
-- Name: Client FK_Client_ClientType; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."Client"
    ADD CONSTRAINT "FK_Client_ClientType" FOREIGN KEY ("ClientTypeId") REFERENCES axionpro."ClientType"("Id");


--
-- Name: ComplianceRule FK_ComplianceRule_ComplianceType; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."ComplianceRule"
    ADD CONSTRAINT "FK_ComplianceRule_ComplianceType" FOREIGN KEY ("ComplianceTypeId") REFERENCES axionpro."ComplianceTypeMaster"("Id");


--
-- Name: ComplianceRule FK_ComplianceRule_Country; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."ComplianceRule"
    ADD CONSTRAINT "FK_ComplianceRule_Country" FOREIGN KEY ("CountryId") REFERENCES axionpro."Country"("Id");


--
-- Name: ComplianceRule FK_ComplianceRule_State; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."ComplianceRule"
    ADD CONSTRAINT "FK_ComplianceRule_State" FOREIGN KEY ("StateId") REFERENCES axionpro."State"("Id");


--
-- Name: ComplianceRule FK_ComplianceRule_Tenant; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."ComplianceRule"
    ADD CONSTRAINT "FK_ComplianceRule_Tenant" FOREIGN KEY ("TenantId") REFERENCES axionpro."Tenant"("Id");


--
-- Name: CountryIdentityRule FK_Country; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."CountryIdentityRule"
    ADD CONSTRAINT "FK_Country" FOREIGN KEY ("CountryId") REFERENCES axionpro."Country"("Id");


--
-- Name: CountryStatutoryRule FK_CountryStatutoryRule_Country; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."CountryStatutoryRule"
    ADD CONSTRAINT "FK_CountryStatutoryRule_Country" FOREIGN KEY ("CountryId") REFERENCES axionpro."Country"("Id");


--
-- Name: CountryStatutoryRule FK_CountryStatutoryRule_Statutory; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."CountryStatutoryRule"
    ADD CONSTRAINT "FK_CountryStatutoryRule_Statutory" FOREIGN KEY ("StatutoryTypeId") REFERENCES axionpro."StatutoryType"("Id");


--
-- Name: CountryIdentityRule FK_Country_Document; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."CountryIdentityRule"
    ADD CONSTRAINT "FK_Country_Document" FOREIGN KEY ("IdentityCategoryDocumentId") REFERENCES axionpro."IdentityCategoryDocument"("Id");


--
-- Name: DayCombination FK_DayCombination_Tenant; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."DayCombination"
    ADD CONSTRAINT "FK_DayCombination_Tenant" FOREIGN KEY ("TenantId") REFERENCES axionpro."Tenant"("Id");


--
-- Name: Department FK_Department_Tenant; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."Department"
    ADD CONSTRAINT "FK_Department_Tenant" FOREIGN KEY ("TenantId") REFERENCES axionpro."Tenant"("Id");


--
-- Name: Designation FK_Designation_Department; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."Designation"
    ADD CONSTRAINT "FK_Designation_Department" FOREIGN KEY ("DepartmentId") REFERENCES axionpro."Department"("Id");


--
-- Name: Designation FK_Designation_Tenant; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."Designation"
    ADD CONSTRAINT "FK_Designation_Tenant" FOREIGN KEY ("TenantId") REFERENCES axionpro."Tenant"("Id");


--
-- Name: DeviceCommandResponse FK_DeviceCommandResponse_DeviceCommand; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."DeviceCommandResponse"
    ADD CONSTRAINT "FK_DeviceCommandResponse_DeviceCommand" FOREIGN KEY ("DeviceCommandId") REFERENCES axionpro."DeviceCommand"("Id") ON DELETE RESTRICT;


--
-- Name: DeviceCommandResponse FK_DeviceCommandResponse_Tenant; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."DeviceCommandResponse"
    ADD CONSTRAINT "FK_DeviceCommandResponse_Tenant" FOREIGN KEY ("TenantId") REFERENCES axionpro."Tenant"("Id") ON DELETE RESTRICT;


--
-- Name: DeviceCommandResponse FK_DeviceCommandResponse_TenantDevice; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."DeviceCommandResponse"
    ADD CONSTRAINT "FK_DeviceCommandResponse_TenantDevice" FOREIGN KEY ("TenantDeviceId") REFERENCES axionpro."TenantDevice"("Id") ON DELETE RESTRICT;


--
-- Name: DeviceCommand FK_DeviceCommand_Tenant; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."DeviceCommand"
    ADD CONSTRAINT "FK_DeviceCommand_Tenant" FOREIGN KEY ("TenantId") REFERENCES axionpro."Tenant"("Id") ON DELETE RESTRICT;


--
-- Name: DeviceCommand FK_DeviceCommand_TenantDevice; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."DeviceCommand"
    ADD CONSTRAINT "FK_DeviceCommand_TenantDevice" FOREIGN KEY ("TenantDeviceId") REFERENCES axionpro."TenantDevice"("Id") ON DELETE RESTRICT;


--
-- Name: DeviceCommand FK_DeviceCommand_TenantLocation; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."DeviceCommand"
    ADD CONSTRAINT "FK_DeviceCommand_TenantLocation" FOREIGN KEY ("TenantLocationId") REFERENCES axionpro."TenantLocation"("Id") ON DELETE RESTRICT;


--
-- Name: DeviceCredential FK_DeviceCredential_TenantDevice; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."DeviceCredential"
    ADD CONSTRAINT "FK_DeviceCredential_TenantDevice" FOREIGN KEY ("TenantDeviceId") REFERENCES axionpro."TenantDevice"("Id") ON DELETE RESTRICT;


--
-- Name: DeviceInitialProvisioning FK_DeviceInitialProvisioning_DeviceMaster; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."DeviceInitialProvisioning"
    ADD CONSTRAINT "FK_DeviceInitialProvisioning_DeviceMaster" FOREIGN KEY ("DeviceMasterId") REFERENCES axionpro."DeviceMaster"("Id") ON DELETE RESTRICT;


--
-- Name: DeviceMessageLog FK_DeviceMessageLog_Tenant; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."DeviceMessageLog"
    ADD CONSTRAINT "FK_DeviceMessageLog_Tenant" FOREIGN KEY ("TenantId") REFERENCES axionpro."Tenant"("Id") ON DELETE RESTRICT;


--
-- Name: DeviceMessageLog FK_DeviceMessageLog_TenantDevice; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."DeviceMessageLog"
    ADD CONSTRAINT "FK_DeviceMessageLog_TenantDevice" FOREIGN KEY ("TenantDeviceId") REFERENCES axionpro."TenantDevice"("Id") ON DELETE RESTRICT;


--
-- Name: District FK_District_State; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."District"
    ADD CONSTRAINT "FK_District_State" FOREIGN KEY ("StateId") REFERENCES axionpro."State"("Id") ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: EmployeePolicyDependentMapping FK_EPD_Dependent; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeePolicyDependentMapping"
    ADD CONSTRAINT "FK_EPD_Dependent" FOREIGN KEY ("DependentId") REFERENCES axionpro."EmployeeDependent"("Id");


--
-- Name: EmployeePolicyDependentMapping FK_EPD_Enrollment; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeePolicyDependentMapping"
    ADD CONSTRAINT "FK_EPD_Enrollment" FOREIGN KEY ("EmployeePolicyEnrollmentId") REFERENCES axionpro."EmployeePolicyEnrollment"("Id");


--
-- Name: EmployeeDailyAttendance FK_Employee; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeDailyAttendance"
    ADD CONSTRAINT "FK_Employee" FOREIGN KEY ("EmployeeId") REFERENCES axionpro."Employee"("Id");


--
-- Name: EmployeeAttendancePunch FK_EmployeeAttendancePunch_Arrangement; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeAttendancePunch"
    ADD CONSTRAINT "FK_EmployeeAttendancePunch_Arrangement" FOREIGN KEY ("EmployeeWorkArrangementId") REFERENCES axionpro."EmployeeWorkArrangement"("Id");


--
-- Name: EmployeeAttendancePunch FK_EmployeeAttendancePunch_AttendanceDeviceType; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeAttendancePunch"
    ADD CONSTRAINT "FK_EmployeeAttendancePunch_AttendanceDeviceType" FOREIGN KEY ("AttendanceDeviceTypeId") REFERENCES axionpro."AttendanceDeviceType"("Id") ON DELETE RESTRICT;


--
-- Name: EmployeeAttendancePunch FK_EmployeeAttendancePunch_Employee; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeAttendancePunch"
    ADD CONSTRAINT "FK_EmployeeAttendancePunch_Employee" FOREIGN KEY ("EmployeeId") REFERENCES axionpro."Employee"("Id");


--
-- Name: EmployeeAttendancePunch FK_EmployeeAttendancePunch_Location; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeAttendancePunch"
    ADD CONSTRAINT "FK_EmployeeAttendancePunch_Location" FOREIGN KEY ("TenantLocationId") REFERENCES axionpro."TenantLocation"("Id");


--
-- Name: EmployeeAttendancePunch FK_EmployeeAttendancePunch_PolicyVersion; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeAttendancePunch"
    ADD CONSTRAINT "FK_EmployeeAttendancePunch_PolicyVersion" FOREIGN KEY ("PolicyVersionId") REFERENCES axionpro."PolicyVersion"("Id");


--
-- Name: EmployeeAttendancePunch FK_EmployeeAttendancePunch_Tenant; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeAttendancePunch"
    ADD CONSTRAINT "FK_EmployeeAttendancePunch_Tenant" FOREIGN KEY ("TenantId") REFERENCES axionpro."Tenant"("Id");


--
-- Name: EmployeeBankDetail FK_EmployeeBank_Employee; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeBankDetail"
    ADD CONSTRAINT "FK_EmployeeBank_Employee" FOREIGN KEY ("EmployeeId") REFERENCES axionpro."Employee"("Id");


--
-- Name: EmployeeCategorySkill FK_EmployeeCategorySkill_Category; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeCategorySkill"
    ADD CONSTRAINT "FK_EmployeeCategorySkill_Category" FOREIGN KEY ("CategoryId") REFERENCES axionpro."Category"("Id") ON DELETE CASCADE;


--
-- Name: EmployeeCategorySkill FK_EmployeeCategorySkill_Employee; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeCategorySkill"
    ADD CONSTRAINT "FK_EmployeeCategorySkill_Employee" FOREIGN KEY ("EmployeeId") REFERENCES axionpro."Employee"("Id") ON DELETE CASCADE;


--
-- Name: EmployeeCodePattern FK_EmployeeCodePattern_Tenant; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeCodePattern"
    ADD CONSTRAINT "FK_EmployeeCodePattern_Tenant" FOREIGN KEY ("TenantId") REFERENCES axionpro."Tenant"("Id");


--
-- Name: EmployeeContact FK_EmployeeContact_Employee; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeContact"
    ADD CONSTRAINT "FK_EmployeeContact_Employee" FOREIGN KEY ("EmployeeId") REFERENCES axionpro."Employee"("Id") ON DELETE CASCADE;


--
-- Name: EmployeeDependent FK_EmployeeDependents_Employee; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeDependent"
    ADD CONSTRAINT "FK_EmployeeDependents_Employee" FOREIGN KEY ("EmployeeId") REFERENCES axionpro."Employee"("Id");


--
-- Name: EmployeeDeviceEnrollment FK_EmployeeDeviceEnrollment_Employee; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeDeviceEnrollment"
    ADD CONSTRAINT "FK_EmployeeDeviceEnrollment_Employee" FOREIGN KEY ("EmployeeId") REFERENCES axionpro."Employee"("Id") ON DELETE RESTRICT;


--
-- Name: EmployeeDeviceEnrollment FK_EmployeeDeviceEnrollment_Tenant; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeDeviceEnrollment"
    ADD CONSTRAINT "FK_EmployeeDeviceEnrollment_Tenant" FOREIGN KEY ("TenantId") REFERENCES axionpro."Tenant"("Id") ON DELETE RESTRICT;


--
-- Name: EmployeeExperienceDocument FK_EmployeeExperienceDocument_Experience; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeExperienceDocument"
    ADD CONSTRAINT "FK_EmployeeExperienceDocument_Experience" FOREIGN KEY ("EmployeeExperienceId") REFERENCES axionpro."EmployeeExperience"("Id") ON DELETE CASCADE;


--
-- Name: EmployeeExperience FK_EmployeeExperience_Employee; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeExperience"
    ADD CONSTRAINT "FK_EmployeeExperience_Employee" FOREIGN KEY ("EmployeeId") REFERENCES axionpro."Employee"("Id") ON DELETE RESTRICT;


--
-- Name: EmployeeIdentity FK_EmployeeIdentity_Document; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeIdentity"
    ADD CONSTRAINT "FK_EmployeeIdentity_Document" FOREIGN KEY ("IdentityCategoryDocumentId") REFERENCES axionpro."IdentityCategoryDocument"("Id");


--
-- Name: EmployeeIdentity FK_EmployeeIdentity_Employee; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeIdentity"
    ADD CONSTRAINT "FK_EmployeeIdentity_Employee" FOREIGN KEY ("EmployeeId") REFERENCES axionpro."Employee"("Id");


--
-- Name: EmployeeImage FK_EmployeeImages_Employee; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeImage"
    ADD CONSTRAINT "FK_EmployeeImages_Employee" FOREIGN KEY ("EmployeeId") REFERENCES axionpro."Employee"("Id") ON DELETE CASCADE;


--
-- Name: EmployeeLeaveBalance FK_EmployeeLeaveBalance_EmployeeLeavePolicyMapping; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeLeaveBalance"
    ADD CONSTRAINT "FK_EmployeeLeaveBalance_EmployeeLeavePolicyMapping" FOREIGN KEY ("EmployeeLeavePolicyMappingId") REFERENCES axionpro."EmployeeLeavePolicyMapping"("Id");


--
-- Name: EmployeeLeaveBalance FK_EmployeeLeaveBalance_Tenant; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeLeaveBalance"
    ADD CONSTRAINT "FK_EmployeeLeaveBalance_Tenant" FOREIGN KEY ("TenantId") REFERENCES axionpro."Tenant"("Id") ON DELETE CASCADE;


--
-- Name: EmployeeLocationAssignment FK_EmployeeLocationAssignment_Employee; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeLocationAssignment"
    ADD CONSTRAINT "FK_EmployeeLocationAssignment_Employee" FOREIGN KEY ("EmployeeId") REFERENCES axionpro."Employee"("Id") ON DELETE RESTRICT;


--
-- Name: EmployeeLocationAssignment FK_EmployeeLocationAssignment_Tenant; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeLocationAssignment"
    ADD CONSTRAINT "FK_EmployeeLocationAssignment_Tenant" FOREIGN KEY ("TenantId") REFERENCES axionpro."Tenant"("Id") ON DELETE RESTRICT;


--
-- Name: EmployeeLocationAssignment FK_EmployeeLocationAssignment_TenantLocation; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeLocationAssignment"
    ADD CONSTRAINT "FK_EmployeeLocationAssignment_TenantLocation" FOREIGN KEY ("TenantLocationId") REFERENCES axionpro."TenantLocation"("Id") ON DELETE RESTRICT;


--
-- Name: EmployeeManagerMapping FK_EmployeeManagerMapping_Department; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeManagerMapping"
    ADD CONSTRAINT "FK_EmployeeManagerMapping_Department" FOREIGN KEY ("DepartmentId") REFERENCES axionpro."Department"("Id");


--
-- Name: EmployeeManagerMapping FK_EmployeeManagerMapping_Designation; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeManagerMapping"
    ADD CONSTRAINT "FK_EmployeeManagerMapping_Designation" FOREIGN KEY ("DesignationId") REFERENCES axionpro."Designation"("Id");


--
-- Name: EmployeeManagerMapping FK_EmployeeManagerMapping_Employee; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeManagerMapping"
    ADD CONSTRAINT "FK_EmployeeManagerMapping_Employee" FOREIGN KEY ("EmployeeId") REFERENCES axionpro."Employee"("Id");


--
-- Name: EmployeeManagerMapping FK_EmployeeManagerMapping_Manager; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeManagerMapping"
    ADD CONSTRAINT "FK_EmployeeManagerMapping_Manager" FOREIGN KEY ("ManagerId") REFERENCES axionpro."Employee"("Id");


--
-- Name: EmployeeManagerMapping FK_EmployeeManagerMapping_ReportingType; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeManagerMapping"
    ADD CONSTRAINT "FK_EmployeeManagerMapping_ReportingType" FOREIGN KEY ("ReportingTypeId") REFERENCES axionpro."ReportingType"("Id");


--
-- Name: EmployeeManagerMapping FK_EmployeeManagerMapping_Tenant; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeManagerMapping"
    ADD CONSTRAINT "FK_EmployeeManagerMapping_Tenant" FOREIGN KEY ("TenantId") REFERENCES axionpro."Tenant"("Id");


--
-- Name: EmployeePersonalDetail FK_EmployeePersonalDetail_Employee; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeePersonalDetail"
    ADD CONSTRAINT "FK_EmployeePersonalDetail_Employee" FOREIGN KEY ("EmployeeId") REFERENCES axionpro."Employee"("Id");


--
-- Name: EmployeePolicyEnrollment FK_EmployeePolicyEnrollment_Employee; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeePolicyEnrollment"
    ADD CONSTRAINT "FK_EmployeePolicyEnrollment_Employee" FOREIGN KEY ("EmployeeId") REFERENCES axionpro."Employee"("Id");


--
-- Name: EmployeeSalary FK_EmployeeSalary_Employee; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeSalary"
    ADD CONSTRAINT "FK_EmployeeSalary_Employee" FOREIGN KEY ("EmployeeId") REFERENCES axionpro."Employee"("Id");


--
-- Name: EmployeeSalary FK_EmployeeSalary_Structure; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeSalary"
    ADD CONSTRAINT "FK_EmployeeSalary_Structure" FOREIGN KEY ("SalaryStructureId") REFERENCES axionpro."SalaryStructure"("Id");


--
-- Name: EmployeesChangedTypeHistory FK_EmployeeStatusHistory_Employee; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeesChangedTypeHistory"
    ADD CONSTRAINT "FK_EmployeeStatusHistory_Employee" FOREIGN KEY ("EmployeeId") REFERENCES axionpro."Employee"("Id");


--
-- Name: EmployeesChangedTypeHistory FK_EmployeeStatusHistory_NewEmployeeType; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeesChangedTypeHistory"
    ADD CONSTRAINT "FK_EmployeeStatusHistory_NewEmployeeType" FOREIGN KEY ("NewEmployeeTypeId") REFERENCES axionpro."EmployeeType"("Id");


--
-- Name: EmployeesChangedTypeHistory FK_EmployeeStatusHistory_OldEmployeeType; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeesChangedTypeHistory"
    ADD CONSTRAINT "FK_EmployeeStatusHistory_OldEmployeeType" FOREIGN KEY ("OldEmployeeTypeId") REFERENCES axionpro."EmployeeType"("Id");


--
-- Name: EmployeeStatutoryAccount FK_EmployeeStatutoryAccount_Statutory; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeStatutoryAccount"
    ADD CONSTRAINT "FK_EmployeeStatutoryAccount_Statutory" FOREIGN KEY ("StatutoryTypeId") REFERENCES axionpro."StatutoryType"("Id");


--
-- Name: EmployeeTaxProfile FK_EmployeeTaxProfile_Country; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeTaxProfile"
    ADD CONSTRAINT "FK_EmployeeTaxProfile_Country" FOREIGN KEY ("CountryId") REFERENCES axionpro."Country"("Id");


--
-- Name: EmployeeTaxProfile FK_EmployeeTaxProfile_Employee; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeTaxProfile"
    ADD CONSTRAINT "FK_EmployeeTaxProfile_Employee" FOREIGN KEY ("EmployeeId") REFERENCES axionpro."Employee"("Id");


--
-- Name: EmployeeTaxProfile FK_EmployeeTaxProfile_Regime; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeTaxProfile"
    ADD CONSTRAINT "FK_EmployeeTaxProfile_Regime" FOREIGN KEY ("RegimeId") REFERENCES axionpro."TaxRegimeMaster"("Id");


--
-- Name: EmployeeTaxProfile FK_EmployeeTaxProfile_State; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeTaxProfile"
    ADD CONSTRAINT "FK_EmployeeTaxProfile_State" FOREIGN KEY ("StateId") REFERENCES axionpro."State"("Id");


--
-- Name: EmployeeTypeBasicMenu FK_EmployeeTypeBasicMenu_EmployeeType; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeTypeBasicMenu"
    ADD CONSTRAINT "FK_EmployeeTypeBasicMenu_EmployeeType" FOREIGN KEY ("EmployeeTypeId") REFERENCES axionpro."EmployeeType"("Id") ON DELETE CASCADE;


--
-- Name: EmployeeWorkArrangement FK_EmployeeWorkArrangement_AttendancePolicy; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeWorkArrangement"
    ADD CONSTRAINT "FK_EmployeeWorkArrangement_AttendancePolicy" FOREIGN KEY ("AttendancePolicyId") REFERENCES axionpro."AttendancePolicy"("Id") ON DELETE RESTRICT;


--
-- Name: EmployeeWorkArrangement FK_EmployeeWorkArrangement_Employee; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeWorkArrangement"
    ADD CONSTRAINT "FK_EmployeeWorkArrangement_Employee" FOREIGN KEY ("EmployeeId") REFERENCES axionpro."Employee"("Id") ON DELETE RESTRICT;


--
-- Name: EmployeeWorkArrangement FK_EmployeeWorkArrangement_PolicyVersion; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeWorkArrangement"
    ADD CONSTRAINT "FK_EmployeeWorkArrangement_PolicyVersion" FOREIGN KEY ("PolicyVersionId") REFERENCES axionpro."PolicyVersion"("Id") ON DELETE RESTRICT;


--
-- Name: EmployeeWorkArrangement FK_EmployeeWorkArrangement_PrimaryLocation; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeWorkArrangement"
    ADD CONSTRAINT "FK_EmployeeWorkArrangement_PrimaryLocation" FOREIGN KEY ("PrimaryTenantLocationId") REFERENCES axionpro."TenantLocation"("Id") ON DELETE RESTRICT;


--
-- Name: EmployeeWorkArrangement FK_EmployeeWorkArrangement_Tenant; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeWorkArrangement"
    ADD CONSTRAINT "FK_EmployeeWorkArrangement_Tenant" FOREIGN KEY ("TenantId") REFERENCES axionpro."Tenant"("Id") ON DELETE RESTRICT;


--
-- Name: EmployeeWorkDocument FK_EmployeeWorkDocument_DocumentType; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeWorkDocument"
    ADD CONSTRAINT "FK_EmployeeWorkDocument_DocumentType" FOREIGN KEY ("WorkDocumentTypeId") REFERENCES axionpro."WorkDocumentType"("Id");


--
-- Name: EmployeeWorkDocument FK_EmployeeWorkDocument_WorkHistory; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeWorkDocument"
    ADD CONSTRAINT "FK_EmployeeWorkDocument_WorkHistory" FOREIGN KEY ("EmployeeWorkHistoryId") REFERENCES axionpro."EmployeeWorkHistory"("Id");


--
-- Name: EmployeeWorkHistory FK_EmployeeWorkHistory_Profile; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeWorkHistory"
    ADD CONSTRAINT "FK_EmployeeWorkHistory_Profile" FOREIGN KEY ("EmployeeWorkProfileId") REFERENCES axionpro."EmployeeWorkProfile"("Id");


--
-- Name: EmployeeWorkPattern FK_EmployeeWorkPattern_Tenant; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeWorkPattern"
    ADD CONSTRAINT "FK_EmployeeWorkPattern_Tenant" FOREIGN KEY ("TenantId") REFERENCES axionpro."Tenant"("Id") ON DELETE RESTRICT;


--
-- Name: EmployeeWorkPattern FK_EmployeeWorkPattern_TenantLocation; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeWorkPattern"
    ADD CONSTRAINT "FK_EmployeeWorkPattern_TenantLocation" FOREIGN KEY ("TenantLocationId") REFERENCES axionpro."TenantLocation"("Id") ON DELETE RESTRICT;


--
-- Name: EmployeeWorkPattern FK_EmployeeWorkPattern_WorkArrangement; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeWorkPattern"
    ADD CONSTRAINT "FK_EmployeeWorkPattern_WorkArrangement" FOREIGN KEY ("EmployeeWorkArrangementId") REFERENCES axionpro."EmployeeWorkArrangement"("Id") ON DELETE RESTRICT;


--
-- Name: Employee FK_Employee_Country; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."Employee"
    ADD CONSTRAINT "FK_Employee_Country" FOREIGN KEY ("CountryId") REFERENCES axionpro."Country"("Id");


--
-- Name: Employee FK_Employee_Designation; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."Employee"
    ADD CONSTRAINT "FK_Employee_Designation" FOREIGN KEY ("DesignationId") REFERENCES axionpro."Designation"("Id") ON DELETE SET NULL;


--
-- Name: Employee FK_Employee_EmployeeType; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."Employee"
    ADD CONSTRAINT "FK_Employee_EmployeeType" FOREIGN KEY ("EmployeeTypeId") REFERENCES axionpro."EmployeeType"("Id");


--
-- Name: Employee FK_Employee_Gender; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."Employee"
    ADD CONSTRAINT "FK_Employee_Gender" FOREIGN KEY ("GenderId") REFERENCES axionpro."Gender"("Id");


--
-- Name: Employee FK_Employee_Tenant; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."Employee"
    ADD CONSTRAINT "FK_Employee_Tenant" FOREIGN KEY ("TenantId") REFERENCES axionpro."Tenant"("Id") ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: Tenant FK_Gender_Tenant; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."Tenant"
    ADD CONSTRAINT "FK_Gender_Tenant" FOREIGN KEY ("GenderId") REFERENCES axionpro."Gender"("Id");


--
-- Name: HostRoleModuleAndPermission FK_HRMP_HostRole; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."HostRoleModuleAndPermission"
    ADD CONSTRAINT "FK_HRMP_HostRole" FOREIGN KEY ("HostRoleId") REFERENCES axionpro."HostRole"("Id");


--
-- Name: HostRoleModuleAndPermission FK_HRMP_Module; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."HostRoleModuleAndPermission"
    ADD CONSTRAINT "FK_HRMP_Module" FOREIGN KEY ("ModuleId") REFERENCES axionpro."Module"("Id");


--
-- Name: HostRoleModuleAndPermission FK_HRMP_Operation; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."HostRoleModuleAndPermission"
    ADD CONSTRAINT "FK_HRMP_Operation" FOREIGN KEY ("OperationId") REFERENCES axionpro."Operation"("Id");


--
-- Name: Holiday FK_Holiday_Tenant; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."Holiday"
    ADD CONSTRAINT "FK_Holiday_Tenant" FOREIGN KEY ("TenantId") REFERENCES axionpro."Tenant"("Id");


--
-- Name: Holiday FK_Holiday_TenantLocation; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."Holiday"
    ADD CONSTRAINT "FK_Holiday_TenantLocation" FOREIGN KEY ("TenantLocationId") REFERENCES axionpro."TenantLocation"("Id") ON DELETE RESTRICT;


--
-- Name: HostUser FK_HostUser_HostRole; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."HostUser"
    ADD CONSTRAINT "FK_HostUser_HostRole" FOREIGN KEY ("HostRoleId") REFERENCES axionpro."HostRole"("Id");


--
-- Name: IdentityCategoryDocument FK_IdentityDocument_Category; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."IdentityCategoryDocument"
    ADD CONSTRAINT "FK_IdentityDocument_Category" FOREIGN KEY ("IdentityCategoryId") REFERENCES axionpro."IdentityCategory"("Id");


--
-- Name: LeaveType FK_LeaveType_TenantId; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."LeaveType"
    ADD CONSTRAINT "FK_LeaveType_TenantId" FOREIGN KEY ("TenantId") REFERENCES axionpro."Tenant"("Id");


--
-- Name: Locality FK_Locality_District; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."Locality"
    ADD CONSTRAINT "FK_Locality_District" FOREIGN KEY ("DistrictId") REFERENCES axionpro."District"("Id") ON DELETE RESTRICT;


--
-- Name: Locality FK_Locality_LocalityType; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."Locality"
    ADD CONSTRAINT "FK_Locality_LocalityType" FOREIGN KEY ("LocalityTypeId") REFERENCES axionpro."LocalityType"("Id") ON DELETE RESTRICT;


--
-- Name: LoginCredential FK_LoginCredential_Employee; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."LoginCredential"
    ADD CONSTRAINT "FK_LoginCredential_Employee" FOREIGN KEY ("EmployeeId") REFERENCES axionpro."Employee"("Id");


--
-- Name: ModuleOperationMapping FK_ModuleOperationMapping_DataViewStructure; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."ModuleOperationMapping"
    ADD CONSTRAINT "FK_ModuleOperationMapping_DataViewStructure" FOREIGN KEY ("DataViewStructureId") REFERENCES axionpro."DataViewStructure"("Id");


--
-- Name: ModuleOperationMapping FK_ModuleOperationMapping_Module; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."ModuleOperationMapping"
    ADD CONSTRAINT "FK_ModuleOperationMapping_Module" FOREIGN KEY ("ModuleId") REFERENCES axionpro."Module"("Id");


--
-- Name: ModuleOperationMapping FK_ModuleOperationMapping_PageTypeEnum; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."ModuleOperationMapping"
    ADD CONSTRAINT "FK_ModuleOperationMapping_PageTypeEnum" FOREIGN KEY ("PageTypeId") REFERENCES axionpro."PageTypeEnum"("Id");


--
-- Name: ModuleOperationMapping FK_ModuleOperation_Operation; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."ModuleOperationMapping"
    ADD CONSTRAINT "FK_ModuleOperation_Operation" FOREIGN KEY ("OperationId") REFERENCES axionpro."Operation"("Id");


--
-- Name: Module FK_Module_ParentModule; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."Module"
    ADD CONSTRAINT "FK_Module_ParentModule" FOREIGN KEY ("ParentModuleId") REFERENCES axionpro."Module"("Id");


--
-- Name: PlanModuleMapping FK_PMM_SubscriptionPlan; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PlanModuleMapping"
    ADD CONSTRAINT "FK_PMM_SubscriptionPlan" FOREIGN KEY ("SubscriptionPlanId") REFERENCES axionpro."SubscriptionPlan"("Id");


--
-- Name: PayrollEmployeeDetail FK_PayrollDetail_Component; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PayrollEmployeeDetail"
    ADD CONSTRAINT "FK_PayrollDetail_Component" FOREIGN KEY ("ComponentId") REFERENCES axionpro."SalaryComponentMaster"("Id");


--
-- Name: PayrollEmployeeDetail FK_PayrollDetail_Employee; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PayrollEmployeeDetail"
    ADD CONSTRAINT "FK_PayrollDetail_Employee" FOREIGN KEY ("PayrollEmployeeId") REFERENCES axionpro."PayrollEmployee"("Id");


--
-- Name: PayrollEmployee FK_PayrollEmployee_Employee; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PayrollEmployee"
    ADD CONSTRAINT "FK_PayrollEmployee_Employee" FOREIGN KEY ("EmployeeId") REFERENCES axionpro."Employee"("Id");


--
-- Name: PayrollEmployee FK_PayrollEmployee_Run; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PayrollEmployee"
    ADD CONSTRAINT "FK_PayrollEmployee_Run" FOREIGN KEY ("PayrollRunId") REFERENCES axionpro."PayrollRun"("Id");


--
-- Name: PayrollRun FK_PayrollRun_Tenant; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PayrollRun"
    ADD CONSTRAINT "FK_PayrollRun_Tenant" FOREIGN KEY ("TenantId") REFERENCES axionpro."Tenant"("Id");


--
-- Name: PlanModuleMapping FK_PlanModuleMapping_Module; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PlanModuleMapping"
    ADD CONSTRAINT "FK_PlanModuleMapping_Module" FOREIGN KEY ("ModuleId") REFERENCES axionpro."Module"("Id");


--
-- Name: PolicyAcknowledgement FK_PolicyAcknowledgement_Employee; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyAcknowledgement"
    ADD CONSTRAINT "FK_PolicyAcknowledgement_Employee" FOREIGN KEY ("EmployeeId") REFERENCES axionpro."Employee"("Id") ON DELETE RESTRICT;


--
-- Name: PolicyAcknowledgement FK_PolicyAcknowledgement_Tenant; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyAcknowledgement"
    ADD CONSTRAINT "FK_PolicyAcknowledgement_Tenant" FOREIGN KEY ("TenantId") REFERENCES axionpro."Tenant"("Id") ON DELETE RESTRICT;


--
-- Name: PolicyAcknowledgement FK_PolicyAcknowledgement_Version; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyAcknowledgement"
    ADD CONSTRAINT "FK_PolicyAcknowledgement_Version" FOREIGN KEY ("PolicyVersionId") REFERENCES axionpro."PolicyVersion"("Id") ON DELETE RESTRICT;


--
-- Name: PolicyApplicability FK_PolicyApplicability_Country; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyApplicability"
    ADD CONSTRAINT "FK_PolicyApplicability_Country" FOREIGN KEY ("CountryId") REFERENCES axionpro."Country"("Id") ON DELETE RESTRICT;


--
-- Name: PolicyApplicability FK_PolicyApplicability_Department; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyApplicability"
    ADD CONSTRAINT "FK_PolicyApplicability_Department" FOREIGN KEY ("DepartmentId") REFERENCES axionpro."Department"("Id") ON DELETE RESTRICT;


--
-- Name: PolicyApplicability FK_PolicyApplicability_Designation; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyApplicability"
    ADD CONSTRAINT "FK_PolicyApplicability_Designation" FOREIGN KEY ("DesignationId") REFERENCES axionpro."Designation"("Id") ON DELETE RESTRICT;


--
-- Name: PolicyApplicability FK_PolicyApplicability_District; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyApplicability"
    ADD CONSTRAINT "FK_PolicyApplicability_District" FOREIGN KEY ("DistrictId") REFERENCES axionpro."District"("Id") ON DELETE RESTRICT;


--
-- Name: PolicyApplicability FK_PolicyApplicability_Employee; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyApplicability"
    ADD CONSTRAINT "FK_PolicyApplicability_Employee" FOREIGN KEY ("EmployeeId") REFERENCES axionpro."Employee"("Id") ON DELETE RESTRICT;


--
-- Name: PolicyApplicability FK_PolicyApplicability_EmployeeType; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyApplicability"
    ADD CONSTRAINT "FK_PolicyApplicability_EmployeeType" FOREIGN KEY ("EmployeeTypeId") REFERENCES axionpro."EmployeeType"("Id") ON DELETE RESTRICT;


--
-- Name: PolicyApplicability FK_PolicyApplicability_Gender; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyApplicability"
    ADD CONSTRAINT "FK_PolicyApplicability_Gender" FOREIGN KEY ("GenderId") REFERENCES axionpro."Gender"("Id") ON DELETE RESTRICT;


--
-- Name: PolicyApplicability FK_PolicyApplicability_Locality; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyApplicability"
    ADD CONSTRAINT "FK_PolicyApplicability_Locality" FOREIGN KEY ("LocalityId") REFERENCES axionpro."Locality"("Id") ON DELETE RESTRICT;


--
-- Name: PolicyApplicability FK_PolicyApplicability_State; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyApplicability"
    ADD CONSTRAINT "FK_PolicyApplicability_State" FOREIGN KEY ("StateId") REFERENCES axionpro."State"("Id") ON DELETE RESTRICT;


--
-- Name: PolicyApplicability FK_PolicyApplicability_Tenant; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyApplicability"
    ADD CONSTRAINT "FK_PolicyApplicability_Tenant" FOREIGN KEY ("TenantId") REFERENCES axionpro."Tenant"("Id") ON DELETE RESTRICT;


--
-- Name: PolicyApplicability FK_PolicyApplicability_TenantLocation; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyApplicability"
    ADD CONSTRAINT "FK_PolicyApplicability_TenantLocation" FOREIGN KEY ("TenantLocationId") REFERENCES axionpro."TenantLocation"("Id") ON DELETE RESTRICT;


--
-- Name: PolicyApplicability FK_PolicyApplicability_Version; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyApplicability"
    ADD CONSTRAINT "FK_PolicyApplicability_Version" FOREIGN KEY ("PolicyVersionId") REFERENCES axionpro."PolicyVersion"("Id") ON DELETE CASCADE;


--
-- Name: PolicyApprovalHistory FK_PolicyApprovalHistory_Stage; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyApprovalHistory"
    ADD CONSTRAINT "FK_PolicyApprovalHistory_Stage" FOREIGN KEY ("PolicyApprovalStageId") REFERENCES axionpro."PolicyApprovalStage"("Id") ON DELETE RESTRICT;


--
-- Name: PolicyApprovalHistory FK_PolicyApprovalHistory_Tenant; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyApprovalHistory"
    ADD CONSTRAINT "FK_PolicyApprovalHistory_Tenant" FOREIGN KEY ("TenantId") REFERENCES axionpro."Tenant"("Id") ON DELETE RESTRICT;


--
-- Name: PolicyApprovalHistory FK_PolicyApprovalHistory_Version; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyApprovalHistory"
    ADD CONSTRAINT "FK_PolicyApprovalHistory_Version" FOREIGN KEY ("PolicyVersionId") REFERENCES axionpro."PolicyVersion"("Id") ON DELETE RESTRICT;


--
-- Name: PolicyApprovalStage FK_PolicyApprovalStage_Category; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyApprovalStage"
    ADD CONSTRAINT "FK_PolicyApprovalStage_Category" FOREIGN KEY ("PolicyCategoryId") REFERENCES axionpro."PolicyCategory"("Id") ON DELETE RESTRICT;


--
-- Name: PolicyApprovalStage FK_PolicyApprovalStage_Role; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyApprovalStage"
    ADD CONSTRAINT "FK_PolicyApprovalStage_Role" FOREIGN KEY ("ApproverRoleId") REFERENCES axionpro."Role"("Id") ON DELETE RESTRICT;


--
-- Name: PolicyApprovalStage FK_PolicyApprovalStage_Tenant; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyApprovalStage"
    ADD CONSTRAINT "FK_PolicyApprovalStage_Tenant" FOREIGN KEY ("TenantId") REFERENCES axionpro."Tenant"("Id") ON DELETE RESTRICT;


--
-- Name: PolicyAssignment FK_PolicyAssignment_Applicability; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyAssignment"
    ADD CONSTRAINT "FK_PolicyAssignment_Applicability" FOREIGN KEY ("SourceApplicabilityId") REFERENCES axionpro."PolicyApplicability"("Id") ON DELETE SET NULL;


--
-- Name: PolicyAssignment FK_PolicyAssignment_Employee; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyAssignment"
    ADD CONSTRAINT "FK_PolicyAssignment_Employee" FOREIGN KEY ("EmployeeId") REFERENCES axionpro."Employee"("Id") ON DELETE RESTRICT;


--
-- Name: PolicyAssignment FK_PolicyAssignment_Tenant; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyAssignment"
    ADD CONSTRAINT "FK_PolicyAssignment_Tenant" FOREIGN KEY ("TenantId") REFERENCES axionpro."Tenant"("Id") ON DELETE RESTRICT;


--
-- Name: PolicyAssignment FK_PolicyAssignment_Version; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyAssignment"
    ADD CONSTRAINT "FK_PolicyAssignment_Version" FOREIGN KEY ("PolicyVersionId") REFERENCES axionpro."PolicyVersion"("Id") ON DELETE RESTRICT;


--
-- Name: PolicyChangeAudit FK_PolicyChangeAudit_Policy; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyChangeAudit"
    ADD CONSTRAINT "FK_PolicyChangeAudit_Policy" FOREIGN KEY ("PolicyId") REFERENCES axionpro."Policy"("Id") ON DELETE RESTRICT;


--
-- Name: PolicyChangeAudit FK_PolicyChangeAudit_Tenant; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyChangeAudit"
    ADD CONSTRAINT "FK_PolicyChangeAudit_Tenant" FOREIGN KEY ("TenantId") REFERENCES axionpro."Tenant"("Id") ON DELETE RESTRICT;


--
-- Name: PolicyChangeAudit FK_PolicyChangeAudit_Version; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyChangeAudit"
    ADD CONSTRAINT "FK_PolicyChangeAudit_Version" FOREIGN KEY ("PolicyVersionId") REFERENCES axionpro."PolicyVersion"("Id") ON DELETE RESTRICT;


--
-- Name: PolicyDocument FK_PolicyDocument_Tenant; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyDocument"
    ADD CONSTRAINT "FK_PolicyDocument_Tenant" FOREIGN KEY ("TenantId") REFERENCES axionpro."Tenant"("Id") ON DELETE RESTRICT;


--
-- Name: PolicyDocument FK_PolicyDocument_Type; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyDocument"
    ADD CONSTRAINT "FK_PolicyDocument_Type" FOREIGN KEY ("PolicyDocumentTypeId") REFERENCES axionpro."PolicyDocumentType"("Id") ON DELETE RESTRICT;


--
-- Name: PolicyDocument FK_PolicyDocument_Version; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyDocument"
    ADD CONSTRAINT "FK_PolicyDocument_Version" FOREIGN KEY ("PolicyVersionId") REFERENCES axionpro."PolicyVersion"("Id") ON DELETE CASCADE;


--
-- Name: PolicyException FK_PolicyException_Employee; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyException"
    ADD CONSTRAINT "FK_PolicyException_Employee" FOREIGN KEY ("EmployeeId") REFERENCES axionpro."Employee"("Id") ON DELETE RESTRICT;


--
-- Name: PolicyException FK_PolicyException_Status; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyException"
    ADD CONSTRAINT "FK_PolicyException_Status" FOREIGN KEY ("ApprovalStatusId") REFERENCES axionpro."PolicyStatus"("Id") ON DELETE RESTRICT;


--
-- Name: PolicyException FK_PolicyException_Tenant; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyException"
    ADD CONSTRAINT "FK_PolicyException_Tenant" FOREIGN KEY ("TenantId") REFERENCES axionpro."Tenant"("Id") ON DELETE RESTRICT;


--
-- Name: PolicyException FK_PolicyException_Version; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyException"
    ADD CONSTRAINT "FK_PolicyException_Version" FOREIGN KEY ("PolicyVersionId") REFERENCES axionpro."PolicyVersion"("Id") ON DELETE RESTRICT;


--
-- Name: PolicyRule FK_PolicyRule_Tenant; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyRule"
    ADD CONSTRAINT "FK_PolicyRule_Tenant" FOREIGN KEY ("TenantId") REFERENCES axionpro."Tenant"("Id") ON DELETE RESTRICT;


--
-- Name: PolicyRule FK_PolicyRule_Type; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyRule"
    ADD CONSTRAINT "FK_PolicyRule_Type" FOREIGN KEY ("PolicyRuleTypeId") REFERENCES axionpro."PolicyRuleType"("Id") ON DELETE RESTRICT;


--
-- Name: PolicyRule FK_PolicyRule_Version; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyRule"
    ADD CONSTRAINT "FK_PolicyRule_Version" FOREIGN KEY ("PolicyVersionId") REFERENCES axionpro."PolicyVersion"("Id") ON DELETE CASCADE;


--
-- Name: PolicyType FK_PolicyType_Category; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyType"
    ADD CONSTRAINT "FK_PolicyType_Category" FOREIGN KEY ("PolicyCategoryId") REFERENCES axionpro."PolicyCategory"("Id") ON DELETE RESTRICT;


--
-- Name: PolicyType FK_PolicyType_Tenant; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyType"
    ADD CONSTRAINT "FK_PolicyType_Tenant" FOREIGN KEY ("TenantId") REFERENCES axionpro."Tenant"("Id") ON DELETE RESTRICT;


--
-- Name: PolicyVersion FK_PolicyVersion_Policy; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyVersion"
    ADD CONSTRAINT "FK_PolicyVersion_Policy" FOREIGN KEY ("PolicyId") REFERENCES axionpro."Policy"("Id") ON DELETE CASCADE;


--
-- Name: PolicyVersion FK_PolicyVersion_Status; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyVersion"
    ADD CONSTRAINT "FK_PolicyVersion_Status" FOREIGN KEY ("PolicyStatusId") REFERENCES axionpro."PolicyStatus"("Id") ON DELETE RESTRICT;


--
-- Name: PolicyVersion FK_PolicyVersion_Tenant; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PolicyVersion"
    ADD CONSTRAINT "FK_PolicyVersion_Tenant" FOREIGN KEY ("TenantId") REFERENCES axionpro."Tenant"("Id") ON DELETE RESTRICT;


--
-- Name: Policy FK_Policy_OwnerDepartment; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."Policy"
    ADD CONSTRAINT "FK_Policy_OwnerDepartment" FOREIGN KEY ("OwnerDepartmentId") REFERENCES axionpro."Department"("Id") ON DELETE RESTRICT;


--
-- Name: Policy FK_Policy_Tenant; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."Policy"
    ADD CONSTRAINT "FK_Policy_Tenant" FOREIGN KEY ("TenantId") REFERENCES axionpro."Tenant"("Id") ON DELETE RESTRICT;


--
-- Name: Policy FK_Policy_Type; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."Policy"
    ADD CONSTRAINT "FK_Policy_Type" FOREIGN KEY ("PolicyTypeId") REFERENCES axionpro."PolicyType"("Id") ON DELETE RESTRICT;


--
-- Name: RefreshToken FK_RefreshToken_HostUser; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."RefreshToken"
    ADD CONSTRAINT "FK_RefreshToken_HostUser" FOREIGN KEY ("HostUserId") REFERENCES axionpro."HostUser"("Id") ON DELETE RESTRICT;


--
-- Name: RefreshToken FK_RefreshToken_LoginCredential; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."RefreshToken"
    ADD CONSTRAINT "FK_RefreshToken_LoginCredential" FOREIGN KEY ("LoginCredentialId") REFERENCES axionpro."LoginCredential"("Id") ON DELETE RESTRICT;


--
-- Name: RequestType FK_RequestType_Tenant; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."RequestType"
    ADD CONSTRAINT "FK_RequestType_Tenant" FOREIGN KEY ("TenantId") REFERENCES axionpro."Tenant"("Id");


--
-- Name: RoleModuleAndPermission FK_RoleModulePermission_Module; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."RoleModuleAndPermission"
    ADD CONSTRAINT "FK_RoleModulePermission_Module" FOREIGN KEY ("ModuleId") REFERENCES axionpro."Module"("Id");


--
-- Name: RoleModuleAndPermission FK_RoleModulePermission_Operation; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."RoleModuleAndPermission"
    ADD CONSTRAINT "FK_RoleModulePermission_Operation" FOREIGN KEY ("OperationId") REFERENCES axionpro."Operation"("Id");


--
-- Name: RoleModuleAndPermission FK_RoleModulePermission_Role; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."RoleModuleAndPermission"
    ADD CONSTRAINT "FK_RoleModulePermission_Role" FOREIGN KEY ("RoleId") REFERENCES axionpro."Role"("Id");


--
-- Name: UserRole FK_Role_Employee; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."UserRole"
    ADD CONSTRAINT "FK_Role_Employee" FOREIGN KEY ("EmployeeId") REFERENCES axionpro."Employee"("Id");


--
-- Name: SalaryComponentMaster FK_SalaryComponent_Compliance; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."SalaryComponentMaster"
    ADD CONSTRAINT "FK_SalaryComponent_Compliance" FOREIGN KEY ("ComplianceTypeId") REFERENCES axionpro."ComplianceTypeMaster"("Id");


--
-- Name: SalaryComponentMaster FK_SalaryComponent_Country; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."SalaryComponentMaster"
    ADD CONSTRAINT "FK_SalaryComponent_Country" FOREIGN KEY ("CountryId") REFERENCES axionpro."Country"("Id");


--
-- Name: SalaryComponentMaster FK_SalaryComponent_Dependency; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."SalaryComponentMaster"
    ADD CONSTRAINT "FK_SalaryComponent_Dependency" FOREIGN KEY ("DependsOnComponentId") REFERENCES axionpro."SalaryComponentMaster"("Id");


--
-- Name: SalaryComponentMaster FK_SalaryComponent_State; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."SalaryComponentMaster"
    ADD CONSTRAINT "FK_SalaryComponent_State" FOREIGN KEY ("StateId") REFERENCES axionpro."State"("Id");


--
-- Name: SalaryComponentMaster FK_SalaryComponent_Tenant; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."SalaryComponentMaster"
    ADD CONSTRAINT "FK_SalaryComponent_Tenant" FOREIGN KEY ("TenantId") REFERENCES axionpro."Tenant"("Id");


--
-- Name: SalaryStructure FK_SalaryStructure_Tenant; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."SalaryStructure"
    ADD CONSTRAINT "FK_SalaryStructure_Tenant" FOREIGN KEY ("TenantId") REFERENCES axionpro."Tenant"("Id");


--
-- Name: StatutoryType FK_StatutoryType_Country; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."StatutoryType"
    ADD CONSTRAINT "FK_StatutoryType_Country" FOREIGN KEY ("CountryId") REFERENCES axionpro."Country"("Id");


--
-- Name: SalaryStructureDetail FK_StructureDetail_Component; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."SalaryStructureDetail"
    ADD CONSTRAINT "FK_StructureDetail_Component" FOREIGN KEY ("ComponentId") REFERENCES axionpro."SalaryComponentMaster"("Id");


--
-- Name: SalaryStructureDetail FK_StructureDetail_Dependency; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."SalaryStructureDetail"
    ADD CONSTRAINT "FK_StructureDetail_Dependency" FOREIGN KEY ("DependsOnComponentId") REFERENCES axionpro."SalaryComponentMaster"("Id");


--
-- Name: SalaryStructureDetail FK_StructureDetail_Structure; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."SalaryStructureDetail"
    ADD CONSTRAINT "FK_StructureDetail_Structure" FOREIGN KEY ("SalaryStructureId") REFERENCES axionpro."SalaryStructure"("Id");


--
-- Name: TaxRegimeMaster FK_TaxRegime_Country; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TaxRegimeMaster"
    ADD CONSTRAINT "FK_TaxRegime_Country" FOREIGN KEY ("CountryId") REFERENCES axionpro."Country"("Id");


--
-- Name: TaxRegimeMaster FK_TaxRegime_TaxSystem; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TaxRegimeMaster"
    ADD CONSTRAINT "FK_TaxRegime_TaxSystem" FOREIGN KEY ("TaxSystemId") REFERENCES axionpro."TaxSystemMaster"("Id");


--
-- Name: TaxRule FK_TaxRule_Country; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TaxRule"
    ADD CONSTRAINT "FK_TaxRule_Country" FOREIGN KEY ("CountryId") REFERENCES axionpro."Country"("Id");


--
-- Name: TaxRule FK_TaxRule_Tenant; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TaxRule"
    ADD CONSTRAINT "FK_TaxRule_Tenant" FOREIGN KEY ("TenantId") REFERENCES axionpro."Tenant"("Id");


--
-- Name: TaxSlab FK_TaxSlab_Country; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TaxSlab"
    ADD CONSTRAINT "FK_TaxSlab_Country" FOREIGN KEY ("CountryId") REFERENCES axionpro."Country"("Id");


--
-- Name: TaxSlab FK_TaxSlab_Regime; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TaxSlab"
    ADD CONSTRAINT "FK_TaxSlab_Regime" FOREIGN KEY ("RegimeId") REFERENCES axionpro."TaxRegimeMaster"("Id");


--
-- Name: TaxSlab FK_TaxSlab_State; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TaxSlab"
    ADD CONSTRAINT "FK_TaxSlab_State" FOREIGN KEY ("StateId") REFERENCES axionpro."State"("Id");


--
-- Name: TaxSlab FK_TaxSlab_Tenant; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TaxSlab"
    ADD CONSTRAINT "FK_TaxSlab_Tenant" FOREIGN KEY ("TenantId") REFERENCES axionpro."Tenant"("Id");


--
-- Name: TaxSystemMaster FK_TaxSystem_Country; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TaxSystemMaster"
    ADD CONSTRAINT "FK_TaxSystem_Country" FOREIGN KEY ("CountryId") REFERENCES axionpro."Country"("Id");


--
-- Name: TenantCardMaster FK_TenantCardMaster_Tenant; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TenantCardMaster"
    ADD CONSTRAINT "FK_TenantCardMaster_Tenant" FOREIGN KEY ("TenantId") REFERENCES axionpro."Tenant"("Id") ON DELETE RESTRICT;


--
-- Name: TenantDeviceConfiguration FK_TenantDeviceConfiguration_TenantDevice; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TenantDeviceConfiguration"
    ADD CONSTRAINT "FK_TenantDeviceConfiguration_TenantDevice" FOREIGN KEY ("TenantDeviceId") REFERENCES axionpro."TenantDevice"("Id") ON DELETE CASCADE;


--
-- Name: TenantDevice FK_TenantDevice_DeviceMaster; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TenantDevice"
    ADD CONSTRAINT "FK_TenantDevice_DeviceMaster" FOREIGN KEY ("DeviceMasterId") REFERENCES axionpro."DeviceMaster"("Id") ON DELETE RESTRICT;


--
-- Name: TenantDevice FK_TenantDevice_Tenant; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TenantDevice"
    ADD CONSTRAINT "FK_TenantDevice_Tenant" FOREIGN KEY ("TenantId") REFERENCES axionpro."Tenant"("Id") ON DELETE RESTRICT;


--
-- Name: TenantDevice FK_TenantDevice_TenantLocation; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TenantDevice"
    ADD CONSTRAINT "FK_TenantDevice_TenantLocation" FOREIGN KEY ("TenantLocationId", "TenantId") REFERENCES axionpro."TenantLocation"("Id", "TenantId") ON DELETE RESTRICT;


--
-- Name: TenantEmailConfig FK_TenantEmailConfig_Tenant; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TenantEmailConfig"
    ADD CONSTRAINT "FK_TenantEmailConfig_Tenant" FOREIGN KEY ("TenantId") REFERENCES axionpro."Tenant"("Id") ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: TenantEmailTemplate FK_TenantEmailTemplate_Tenant; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TenantEmailTemplate"
    ADD CONSTRAINT "FK_TenantEmailTemplate_Tenant" FOREIGN KEY ("TenantId") REFERENCES axionpro."Tenant"("Id") ON DELETE CASCADE;


--
-- Name: TenantEnabledModule FK_TenantEnabledModules_Module; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TenantEnabledModule"
    ADD CONSTRAINT "FK_TenantEnabledModules_Module" FOREIGN KEY ("ModuleId") REFERENCES axionpro."Module"("Id");


--
-- Name: TenantEnabledModule FK_TenantEnabledModules_Tenant; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TenantEnabledModule"
    ADD CONSTRAINT "FK_TenantEnabledModules_Tenant" FOREIGN KEY ("TenantId") REFERENCES axionpro."Tenant"("Id");


--
-- Name: TenantEnabledOperation FK_TenantEnabledOperations_Module; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TenantEnabledOperation"
    ADD CONSTRAINT "FK_TenantEnabledOperations_Module" FOREIGN KEY ("ModuleId") REFERENCES axionpro."Module"("Id");


--
-- Name: TenantEnabledOperation FK_TenantEnabledOperations_Operation; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TenantEnabledOperation"
    ADD CONSTRAINT "FK_TenantEnabledOperations_Operation" FOREIGN KEY ("OperationId") REFERENCES axionpro."Operation"("Id");


--
-- Name: TenantEnabledOperation FK_TenantEnabledOperations_Tenant; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TenantEnabledOperation"
    ADD CONSTRAINT "FK_TenantEnabledOperations_Tenant" FOREIGN KEY ("TenantId") REFERENCES axionpro."Tenant"("Id");


--
-- Name: TenantLocation FK_TenantLocation_City; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TenantLocation"
    ADD CONSTRAINT "FK_TenantLocation_City" FOREIGN KEY ("CityId") REFERENCES axionpro."Locality"("Id") ON DELETE RESTRICT;


--
-- Name: TenantLocation FK_TenantLocation_Country; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TenantLocation"
    ADD CONSTRAINT "FK_TenantLocation_Country" FOREIGN KEY ("CountryId") REFERENCES axionpro."Country"("Id") ON DELETE RESTRICT;


--
-- Name: TenantLocation FK_TenantLocation_District; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TenantLocation"
    ADD CONSTRAINT "FK_TenantLocation_District" FOREIGN KEY ("DistrictId") REFERENCES axionpro."District"("Id") ON DELETE RESTRICT;


--
-- Name: TenantLocation FK_TenantLocation_Tenant; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TenantLocation"
    ADD CONSTRAINT "FK_TenantLocation_Tenant" FOREIGN KEY ("TenantId") REFERENCES axionpro."Tenant"("Id") ON DELETE RESTRICT;


--
-- Name: TenantProfile FK_TenantProfile_Tenant; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TenantProfile"
    ADD CONSTRAINT "FK_TenantProfile_Tenant" FOREIGN KEY ("TenantId") REFERENCES axionpro."Tenant"("Id") ON DELETE CASCADE;


--
-- Name: TenantSubscription FK_TenantSubscription_Plan; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TenantSubscription"
    ADD CONSTRAINT "FK_TenantSubscription_Plan" FOREIGN KEY ("SubscriptionPlanId") REFERENCES axionpro."SubscriptionPlan"("Id");


--
-- Name: TenantSubscription FK_TenantSubscription_Tenant; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TenantSubscription"
    ADD CONSTRAINT "FK_TenantSubscription_Tenant" FOREIGN KEY ("TenantId") REFERENCES axionpro."Tenant"("Id");


--
-- Name: Tenant FK_Tenant_TenantIndustry; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."Tenant"
    ADD CONSTRAINT "FK_Tenant_TenantIndustry" FOREIGN KEY ("TenantIndustryId") REFERENCES axionpro."TenantIndustry"("Id");


--
-- Name: Tender FK_Tender_ClientId; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."Tender"
    ADD CONSTRAINT "FK_Tender_ClientId" FOREIGN KEY ("ClientId") REFERENCES axionpro."ClientType"("Id");


--
-- Name: ThreadMessage FK_ThreadMessage_Employee; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."ThreadMessage"
    ADD CONSTRAINT "FK_ThreadMessage_Employee" FOREIGN KEY ("AddedById") REFERENCES axionpro."Employee"("Id") ON DELETE SET NULL;


--
-- Name: ThreadMessage FK_ThreadMessage_Thread; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."ThreadMessage"
    ADD CONSTRAINT "FK_ThreadMessage_Thread" FOREIGN KEY ("ThreadId") REFERENCES axionpro."TicketThread"("Id") ON DELETE CASCADE;


--
-- Name: TicketAttachment FK_TicketAttachment_User; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TicketAttachment"
    ADD CONSTRAINT "FK_TicketAttachment_User" FOREIGN KEY ("UploadedByUserId") REFERENCES axionpro."Employee"("Id") ON DELETE SET NULL;


--
-- Name: TicketClassification FK_TicketClassification_Tenant; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TicketClassification"
    ADD CONSTRAINT "FK_TicketClassification_Tenant" FOREIGN KEY ("TenantId") REFERENCES axionpro."Tenant"("Id");


--
-- Name: TicketHeader FK_TicketHeaderType_TicketClassification; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TicketHeader"
    ADD CONSTRAINT "FK_TicketHeaderType_TicketClassification" FOREIGN KEY ("TicketClassificationId") REFERENCES axionpro."TicketClassification"("Id");


--
-- Name: TicketHeader FK_TicketHeader_TenantId; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TicketHeader"
    ADD CONSTRAINT "FK_TicketHeader_TenantId" FOREIGN KEY ("TenantId") REFERENCES axionpro."Tenant"("Id") ON UPDATE CASCADE;


--
-- Name: TicketHistory FK_TicketHistory_Ticket; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TicketHistory"
    ADD CONSTRAINT "FK_TicketHistory_Ticket" FOREIGN KEY ("TicketId") REFERENCES axionpro."Ticket"("Id");


--
-- Name: TicketHistory FK_TicketHistory_User; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TicketHistory"
    ADD CONSTRAINT "FK_TicketHistory_User" FOREIGN KEY ("DoneByUserId") REFERENCES axionpro."Employee"("Id");


--
-- Name: TicketType FK_TicketType_ApprovalRole; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TicketType"
    ADD CONSTRAINT "FK_TicketType_ApprovalRole" FOREIGN KEY ("ApprovalRoleId") REFERENCES axionpro."Role"("Id") ON DELETE SET NULL;


--
-- Name: TicketType FK_TicketType_Header; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TicketType"
    ADD CONSTRAINT "FK_TicketType_Header" FOREIGN KEY ("TicketHeaderId") REFERENCES axionpro."TicketHeader"("Id");


--
-- Name: TicketType FK_TicketType_ResponsibleRole; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TicketType"
    ADD CONSTRAINT "FK_TicketType_ResponsibleRole" FOREIGN KEY ("ResponsibleRoleId") REFERENCES axionpro."Role"("Id");


--
-- Name: TicketType FK_TicketType_Tenant; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TicketType"
    ADD CONSTRAINT "FK_TicketType_Tenant" FOREIGN KEY ("TenantId") REFERENCES axionpro."Tenant"("Id");


--
-- Name: Ticket FK_Ticket_ApprovedBy; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."Ticket"
    ADD CONSTRAINT "FK_Ticket_ApprovedBy" FOREIGN KEY ("ApprovedByUserId") REFERENCES axionpro."Employee"("Id");


--
-- Name: Ticket FK_Ticket_AssignedRole; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."Ticket"
    ADD CONSTRAINT "FK_Ticket_AssignedRole" FOREIGN KEY ("AssignedToRoleId") REFERENCES axionpro."Role"("Id");


--
-- Name: Ticket FK_Ticket_AssignedUser; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."Ticket"
    ADD CONSTRAINT "FK_Ticket_AssignedUser" FOREIGN KEY ("AssignedToUserId") REFERENCES axionpro."Employee"("Id");


--
-- Name: Ticket FK_Ticket_Classification; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."Ticket"
    ADD CONSTRAINT "FK_Ticket_Classification" FOREIGN KEY ("TicketClassificationId") REFERENCES axionpro."TicketClassification"("Id");


--
-- Name: Ticket FK_Ticket_Header; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."Ticket"
    ADD CONSTRAINT "FK_Ticket_Header" FOREIGN KEY ("TicketHeaderId") REFERENCES axionpro."TicketHeader"("Id");


--
-- Name: Ticket FK_Ticket_RecommendedBy; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."Ticket"
    ADD CONSTRAINT "FK_Ticket_RecommendedBy" FOREIGN KEY ("RecommendedByUserId") REFERENCES axionpro."Employee"("Id");


--
-- Name: Ticket FK_Ticket_RequestedBy; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."Ticket"
    ADD CONSTRAINT "FK_Ticket_RequestedBy" FOREIGN KEY ("RequestedByUserId") REFERENCES axionpro."Employee"("Id");


--
-- Name: Ticket FK_Ticket_RequestedFor; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."Ticket"
    ADD CONSTRAINT "FK_Ticket_RequestedFor" FOREIGN KEY ("RequestedForUserId") REFERENCES axionpro."Employee"("Id");


--
-- Name: Ticket FK_Ticket_Tenant; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."Ticket"
    ADD CONSTRAINT "FK_Ticket_Tenant" FOREIGN KEY ("TenantId") REFERENCES axionpro."Tenant"("Id");


--
-- Name: Ticket FK_Ticket_TicketType; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."Ticket"
    ADD CONSTRAINT "FK_Ticket_TicketType" FOREIGN KEY ("TicketTypeId") REFERENCES axionpro."TicketType"("Id");


--
-- Name: UserAttendanceSetting FK_UserAttendanceSetting_AttendanceDeviceType; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."UserAttendanceSetting"
    ADD CONSTRAINT "FK_UserAttendanceSetting_AttendanceDeviceType" FOREIGN KEY ("AttendanceDeviceTypeId") REFERENCES axionpro."AttendanceDeviceType"("Id");


--
-- Name: UserAttendanceSetting FK_UserAttendanceSetting_Employee; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."UserAttendanceSetting"
    ADD CONSTRAINT "FK_UserAttendanceSetting_Employee" FOREIGN KEY ("EmployeeId") REFERENCES axionpro."Employee"("Id") ON DELETE CASCADE;


--
-- Name: UserRole FK_UserRole_Role; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."UserRole"
    ADD CONSTRAINT "FK_UserRole_Role" FOREIGN KEY ("RoleId") REFERENCES axionpro."Role"("Id");


--
-- Name: EmployeeWorkModeOverrideRequest FK_WorkModeOverride_Employee; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeWorkModeOverrideRequest"
    ADD CONSTRAINT "FK_WorkModeOverride_Employee" FOREIGN KEY ("EmployeeId") REFERENCES axionpro."Employee"("Id") ON DELETE RESTRICT;


--
-- Name: EmployeeWorkModeOverrideRequest FK_WorkModeOverride_Tenant; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeWorkModeOverrideRequest"
    ADD CONSTRAINT "FK_WorkModeOverride_Tenant" FOREIGN KEY ("TenantId") REFERENCES axionpro."Tenant"("Id") ON DELETE RESTRICT;


--
-- Name: EmployeeWorkModeOverrideRequest FK_WorkModeOverride_TenantLocation; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeWorkModeOverrideRequest"
    ADD CONSTRAINT "FK_WorkModeOverride_TenantLocation" FOREIGN KEY ("TenantLocationId") REFERENCES axionpro."TenantLocation"("Id") ON DELETE RESTRICT;


--
-- Name: EmployeeWorkModeOverrideRequest FK_WorkModeOverride_WorkArrangement; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeWorkModeOverrideRequest"
    ADD CONSTRAINT "FK_WorkModeOverride_WorkArrangement" FOREIGN KEY ("EmployeeWorkArrangementId") REFERENCES axionpro."EmployeeWorkArrangement"("Id") ON DELETE RESTRICT;


--
-- Name: Locality FK__City__StateId__6EE06CCD; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."Locality"
    ADD CONSTRAINT "FK__City__StateId__6EE06CCD" FOREIGN KEY ("StateId") REFERENCES axionpro."State"("Id");


--
-- Name: EmailQueue FK__EmailQueu__Templ__44B528D7; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmailQueue"
    ADD CONSTRAINT "FK__EmailQueu__Templ__44B528D7" FOREIGN KEY ("TemplateId") REFERENCES axionpro."EmailTemplate"("Id");


--
-- Name: EmployeeLeavePolicyMapping FK__EmployeeL__Emplo__73D00A73; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeLeavePolicyMapping"
    ADD CONSTRAINT "FK__EmployeeL__Emplo__73D00A73" FOREIGN KEY ("EmployeeId") REFERENCES axionpro."Employee"("Id");


--
-- Name: EmployeeLeavePolicyMapping FK__EmployeeL__Tenan__72DBE63A; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."EmployeeLeavePolicyMapping"
    ADD CONSTRAINT "FK__EmployeeL__Tenan__72DBE63A" FOREIGN KEY ("TenantId") REFERENCES axionpro."Tenant"("Id");


--
-- Name: LeaveRequest FK__LeaveRequ__Emplo__05EEBAAE; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."LeaveRequest"
    ADD CONSTRAINT "FK__LeaveRequ__Emplo__05EEBAAE" FOREIGN KEY ("EmployeeId") REFERENCES axionpro."Employee"("Id");


--
-- Name: LeaveRequest FK__LeaveRequ__Leave__06E2DEE7; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."LeaveRequest"
    ADD CONSTRAINT "FK__LeaveRequ__Leave__06E2DEE7" FOREIGN KEY ("LeaveTypeId") REFERENCES axionpro."LeaveType"("Id");


--
-- Name: LeaveRequest FK__LeaveRequ__Tenan__04FA9675; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."LeaveRequest"
    ADD CONSTRAINT "FK__LeaveRequ__Tenan__04FA9675" FOREIGN KEY ("TenantId") REFERENCES axionpro."Tenant"("Id");


--
-- Name: State FK__State__CountryId__6B0FDBE9; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."State"
    ADD CONSTRAINT "FK__State__CountryId__6B0FDBE9" FOREIGN KEY ("CountryId") REFERENCES axionpro."Country"("Id");


--
-- Name: Tender FK__Tender__TenderSt__3EDC53F0; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."Tender"
    ADD CONSTRAINT "FK__Tender__TenderSt__3EDC53F0" FOREIGN KEY ("TenderStatusId") REFERENCES axionpro."TenderStatus"("Id");


--
-- Name: HostBillingConfiguration HostBillingConfiguration_PaymentGatewayId_fkey; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."HostBillingConfiguration"
    ADD CONSTRAINT "HostBillingConfiguration_PaymentGatewayId_fkey" FOREIGN KEY ("PaymentGatewayId") REFERENCES axionpro."PaymentGateway"("Id") ON DELETE RESTRICT;


--
-- Name: PaymentAttempt PaymentAttempt_BillingOrderId_fkey; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PaymentAttempt"
    ADD CONSTRAINT "PaymentAttempt_BillingOrderId_fkey" FOREIGN KEY ("BillingOrderId") REFERENCES axionpro."BillingOrder"("Id") ON DELETE RESTRICT;


--
-- Name: PaymentTransaction PaymentTransaction_BillingOrderId_fkey; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PaymentTransaction"
    ADD CONSTRAINT "PaymentTransaction_BillingOrderId_fkey" FOREIGN KEY ("BillingOrderId") REFERENCES axionpro."BillingOrder"("Id") ON DELETE RESTRICT;


--
-- Name: PaymentWebhookEvent PaymentWebhookEvent_PaymentGatewayId_fkey; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."PaymentWebhookEvent"
    ADD CONSTRAINT "PaymentWebhookEvent_PaymentGatewayId_fkey" FOREIGN KEY ("PaymentGatewayId") REFERENCES axionpro."PaymentGateway"("Id") ON DELETE RESTRICT;


--
-- Name: SubscriptionPlanPrice SubscriptionPlanPrice_SubscriptionPlanId_fkey; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."SubscriptionPlanPrice"
    ADD CONSTRAINT "SubscriptionPlanPrice_SubscriptionPlanId_fkey" FOREIGN KEY ("SubscriptionPlanId") REFERENCES axionpro."SubscriptionPlan"("Id") ON DELETE RESTRICT;


--
-- Name: TenantBillingProfile TenantBillingProfile_TenantId_fkey; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TenantBillingProfile"
    ADD CONSTRAINT "TenantBillingProfile_TenantId_fkey" FOREIGN KEY ("TenantId") REFERENCES axionpro."Tenant"("Id") ON DELETE RESTRICT;


--
-- Name: TenantBillingSubscription TenantBillingSubscription_PaymentGatewayId_fkey; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TenantBillingSubscription"
    ADD CONSTRAINT "TenantBillingSubscription_PaymentGatewayId_fkey" FOREIGN KEY ("PaymentGatewayId") REFERENCES axionpro."PaymentGateway"("Id") ON DELETE RESTRICT;


--
-- Name: TenantBillingSubscription TenantBillingSubscription_SubscriptionPlanPriceId_fkey; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TenantBillingSubscription"
    ADD CONSTRAINT "TenantBillingSubscription_SubscriptionPlanPriceId_fkey" FOREIGN KEY ("SubscriptionPlanPriceId") REFERENCES axionpro."SubscriptionPlanPrice"("Id") ON DELETE RESTRICT;


--
-- Name: TenantBillingSubscription TenantBillingSubscription_TenantId_fkey; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TenantBillingSubscription"
    ADD CONSTRAINT "TenantBillingSubscription_TenantId_fkey" FOREIGN KEY ("TenantId") REFERENCES axionpro."Tenant"("Id") ON DELETE RESTRICT;


--
-- Name: TenantBillingSubscription TenantBillingSubscription_TenantSubscriptionId_fkey; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TenantBillingSubscription"
    ADD CONSTRAINT "TenantBillingSubscription_TenantSubscriptionId_fkey" FOREIGN KEY ("TenantSubscriptionId") REFERENCES axionpro."TenantSubscription"("Id") ON DELETE RESTRICT;


--
-- Name: TenantEmployeeSectionDefault TenantEmployeeSectionDefault_TenantId_fkey; Type: FK CONSTRAINT; Schema: axionpro; Owner: -
--

ALTER TABLE ONLY axionpro."TenantEmployeeSectionDefault"
    ADD CONSTRAINT "TenantEmployeeSectionDefault_TenantId_fkey" FOREIGN KEY ("TenantId") REFERENCES axionpro."Tenant"("Id");


--
-- PostgreSQL database dump complete
--

\unrestrict K4mxsBl5LHtryxcSR0x8XGhncPP5x1KVYQrZ96gYp3Qg7XSLcKP5lcihOQWpP0u

