// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines shared application constants and standardized error values.
// ================================================================
using System;

namespace axionpro.application.Constants
{
    public static class AppConstants
    {
        #region Error Codes

        /// <summary>
        /// Defines stable application error codes emitted by the exception middleware.
        /// </summary>
        public static class ErrorCodes
        {
            public const string Validation = "VALIDATION_ERROR";
            public const string Unauthorized = "UNAUTHORIZED";
            public const string Forbidden = "FORBIDDEN";
            public const string NotFound = "NOT_FOUND";
            public const string Conflict = "CONFLICT";
            public const string InternalServerError = "INTERNAL_SERVER_ERROR";
        }

        #endregion

        #region Error Messages

        /// <summary>
        /// Defines reusable public messages for standardized application errors.
        /// </summary>
        public static class ErrorMessages
        {
            public const string InvalidRequest = "The request is invalid.";
            public const string InvalidIdentifier = "A valid identifier is required.";
            public const string Unauthorized = "The request is not authenticated.";
            public const string PermissionDenied = "You do not have permission to perform this action.";
            public const string ResourceNotFound = "The requested resource was not found.";
            public const string ResourceConflict = "The request conflicts with the current resource state.";
            public const string InternalServerError = "Something went wrong. Please try again.";
            public const string RequiredDataMissing = "Required data is missing.";
            public const string ValidationFailed = "The request failed validation.";

            /// <summary>
            /// Indicates that the requested subscription plan is unavailable or already soft deleted.
            /// </summary>
            public const string SubscriptionPlanNotFound = "The requested subscription plan was not found.";

            /// <summary>
            /// Indicates that a subscription plan cannot be deleted while an active tenant uses it.
            /// </summary>
            public const string SubscriptionPlanInUse = "The subscription plan cannot be deleted because it is currently assigned to one or more tenants.";

            /// <summary>
            /// Indicates that one or more submitted Modules cannot be assigned to a subscription plan.
            /// </summary>
            public const string InvalidSubscriptionPlanModule = "One or more selected modules are not eligible for the subscription plan.";

            /// <summary>
            /// Indicates that the requested Parent Module is unavailable in the requested scope.
            /// </summary>
            public const string ParentModuleNotFound = "The requested Parent Module was not found.";

            /// <summary>
            /// Indicates that an operation remains linked to one or more module-operation mappings.
            /// </summary>
            public const string OperationLinkedToModule = "The operation cannot be deactivated or deleted because it is linked to one or more modules. Unlink the operation from all module-operation mappings first.";

            /// <summary>
            /// Indicates that an operation remains assigned to one or more current Host-role permissions.
            /// </summary>
            public const string OperationAssignedToHostRolePermission = "The operation cannot be updated or deleted because it is assigned to Host-role module permissions.";

            /// <summary>
            /// Indicates that verification cannot be resent for an already verified Tenant.
            /// </summary>
            public const string TenantAlreadyVerified = "Tenant is already verified.";

            public const string TenantLocationNotFound = "The requested tenant location was not found.";
            public const string TenantLocationInUse = "Tenant location is currently in use. Remove or deactivate the dependent configuration before changing this location.";
            public const string AttendancePolicyNotFound = "The requested attendance policy was not found.";
            public const string AttendancePolicyInUse = "Attendance policy is currently assigned to an employee work arrangement. Unassign or deactivate the work arrangement first.";
            public const string EmployeeLocationAssignmentNotFound = "The requested employee location assignment was not found.";
            public const string EmployeeDeviceEnrollmentNotFound = "The requested employee device enrollment was not found.";
            public const string EmployeeWorkArrangementNotFound = "The requested employee work arrangement was not found.";
            public const string EmployeeWorkArrangementInUse = "Employee work arrangement has dependent configuration that must be removed or deactivated first.";
            public const string EmployeeWorkPatternNotFound = "The requested employee work pattern was not found.";
            public const string EmployeeWorkModeOverrideNotFound = "The requested work mode override request was not found.";
            public const string InvalidTenantConfigurationReference = "One or more selected configuration references are invalid for this tenant.";
            public const string DuplicateTenantLocationCode = "A live tenant location already uses this location code.";
            public const string DuplicateAttendancePolicyName = "A live attendance policy already uses this policy name.";
            public const string DuplicateEmployeeLocationAssignment = "The employee already has this active location assignment.";
            public const string EmployeeAlreadyHasPrimaryLocation = "The employee already has an active primary location assignment.";
            public const string DuplicateDeviceEnrollId = "The selected device already has this live enrollment identifier.";
            public const string EmployeeAlreadyHasCurrentWorkArrangement = "The employee already has a current active work arrangement.";
            public const string DuplicateEmployeeWorkPatternDay = "The work arrangement already has an active pattern for this day.";
            public const string InvalidEffectiveDateRange = "The end date cannot be earlier than the start date.";
            public const string InvalidHybridConfiguration = "Hybrid type is required only when work mode is Hybrid.";
            public const string InvalidOverrideWorkMode = "Hybrid work mode is not allowed for a temporary override request.";
            public const string DeviceMasterNotFound = "The requested device master was not found.";
            public const string DuplicateDeviceMaster = "A live device master already uses this device code or company and model combination.";
            public const string DeviceMasterInUse = "Device master is currently assigned to one or more Tenant devices. Remove those device assignments before changing the device master lifecycle.";
            public const string DeviceMasterAlreadyRegisteredWithTenant = "This device is already registered with a tenant and cannot be updated, have its status changed, or be deleted.";
            public const string TenantDeviceNotFound = "The requested tenant device was not found.";
            public const string TenantDeviceConfigurationNotFound = "The requested tenant device configuration was not found.";
            public const string TenantDeviceConfigurationAlreadyExists = "This tenant device already has a configuration.";
            public const string TenantDeviceConfigurationInUse = "Delete the tenant device configuration before deleting the tenant device.";
            public const string DuplicateTenantDeviceSerialNumber = "A live tenant device already uses this serial number.";
            public const string DuplicateTenantDeviceCode = "A live tenant device already uses this device code for the selected tenant.";
            public const string DuplicateTenantDeviceAssetTag = "A live tenant device already uses this asset tag for the selected tenant.";
            public const string InvalidDeviceManagementTenant = "The selected tenant is unavailable or inactive.";
            public const string InvalidDeviceManagementTenantLocation = "The selected tenant location is unavailable or inactive.";
            public const string TenantLocationDoesNotBelongToTenant = "The selected tenant location does not belong to the selected tenant.";
            public const string InvalidDeviceMaster = "The selected device master is unavailable or inactive.";
            public const string TenantDeviceEnrollmentInUse = "Tenant device has active employee enrollments that must be removed or deactivated first.";
        }

        #endregion

        #region Success Messages

        /// <summary>
        /// Defines success messages for response-boundary handlers.
        /// </summary>
        public static class SuccessMessages
        {
            /// <summary>
            /// Confirms that a Tenant Employee session was created successfully.
            /// </summary>
            public const string LoginSuccessful = "Login successful.";

            /// <summary>
            /// Confirms that the shared Common navigation hierarchy was retrieved successfully.
            /// </summary>
            public const string CommonMenuRetrieved = "Common menu retrieved successfully.";

            /// <summary>
            /// Confirms that the current Tenant employee authorization bootstrap was retrieved successfully.
            /// </summary>
            public const string TenantUserAccessRetrieved = "Tenant user access retrieved successfully.";

            /// <summary>
            /// Confirms that the current Host authorization bootstrap was retrieved successfully.
            /// </summary>
            public const string HostAccessRetrieved = "Host access retrieved successfully.";

            public const string GenderOptionsRetrieved = "Gender options fetched successfully.";
            public const string RoleOptionsRetrieved = "Role options fetched successfully.";
            public const string RolesRetrieved = "Roles retrieved successfully.";
            public const string RolePermissionsRetrieved = "Role permissions retrieved successfully.";
            public const string RolePermissionsUpdated = "Role permissions updated successfully.";
            public const string CountriesRetrieved = "Countries fetched successfully.";
            public const string StatesRetrieved = "States fetched successfully.";
            public const string DistrictsRetrieved = "Districts fetched successfully.";
            public const string ConsumedInsurancePoliciesRetrieved = "Insurance policies fetched successfully.";
            public const string WorkflowStageDeleted = "Workflow stage deleted successfully.";
            public const string EmployeeTypesRetrieved = "Employee types fetched successfully.";
            public const string MenuDisplayStructureRetrieved = "Menu display structure fetched successfully.";

            /// <summary>
            /// Confirms that a subscription plan was soft deleted successfully.
            /// </summary>
            public const string SubscriptionPlanDeletedSuccessfully = "Subscription plan deleted successfully.";

            /// <summary>
            /// Confirms that subscription plans were retrieved successfully.
            /// </summary>
            public const string SubscriptionPlansRetrievedSuccessfully = "Subscription plans retrieved successfully.";

            /// <summary>
            /// Confirms that a subscription plan was created successfully.
            /// </summary>
            public const string SubscriptionPlanCreatedSuccessfully = "Subscription plan created successfully.";

            /// <summary>
            /// Confirms that a subscription plan was updated successfully.
            /// </summary>
            public const string SubscriptionPlanUpdatedSuccessfully = "Subscription plan updated successfully.";

            /// <summary>
            /// Confirms that tenant subscription information was retrieved successfully.
            /// </summary>
            public const string TenantSubscriptionPlanRetrievedSuccessfully = "Tenant subscription plan retrieved successfully.";

            /// <summary>
            /// Confirms that modules available to a subscription plan were retrieved successfully.
            /// </summary>
            public const string SubscriptionPlanModulesRetrievedSuccessfully = "Subscription plan modules retrieved successfully.";

            /// <summary>
            /// Confirms that selectable Module options for a subscription plan were retrieved successfully.
            /// </summary>
            public const string SubscriptionPlanModuleOptionsRetrievedSuccessfully = "Subscription plan module options retrieved successfully.";

            /// <summary>
            /// Confirms that a subscription plan's Module mapping was synchronized successfully.
            /// </summary>
            public const string SubscriptionPlanModuleMappingSavedSuccessfully = "Subscription plan module mapping saved successfully.";

            /// <summary>
            /// Confirms that a Parent Module status cascade completed successfully.
            /// </summary>
            public const string ParentModuleStatusUpdatedSuccessfully = "Parent Module status updated successfully.";

            /// <summary>
            /// Confirms that Host-managed Tenant details were updated successfully.
            /// </summary>
            public const string TenantUpdatedSuccessfully = "Tenant updated successfully.";

            /// <summary>
            /// Confirms that missing active-plan entitlement snapshot rows were synchronized for a Tenant.
            /// </summary>
            public const string TenantPlanEntitlementsSynchronizedSuccessfully = "Tenant plan entitlements synchronized successfully.";

            /// <summary>
            /// Confirms that a Tenant was soft deleted successfully.
            /// </summary>
            public const string TenantDeletedSuccessfully = "Tenant deleted successfully.";

            /// <summary>
            /// Confirms that a Tenant was activated successfully.
            /// </summary>
            public const string TenantActivatedSuccessfully = "Tenant activated successfully.";

            /// <summary>
            /// Confirms that a Tenant was deactivated successfully.
            /// </summary>
            public const string TenantDeactivatedSuccessfully = "Tenant deactivated successfully.";

            /// <summary>
            /// Confirms that a Tenant verification welcome email was sent successfully.
            /// </summary>
            public const string TenantVerificationResentSuccessfully = "Tenant verification email sent successfully.";

            public const string TenantLocationCreated = "Tenant location created successfully.";
            public const string TenantLocationUpdated = "Tenant location updated successfully.";
            public const string TenantLocationStatusUpdated = "Tenant location status updated successfully.";
            public const string TenantLocationDeleted = "Tenant location deleted successfully.";
            public const string TenantLocationRetrieved = "Tenant locations retrieved successfully.";
            public const string AttendancePolicyCreated = "Attendance policy created successfully.";
            public const string AttendancePolicyUpdated = "Attendance policy updated successfully.";
            public const string AttendancePolicyStatusUpdated = "Attendance policy status updated successfully.";
            public const string AttendancePolicyDeleted = "Attendance policy deleted successfully.";
            public const string AttendancePolicyRetrieved = "Attendance policies retrieved successfully.";
            public const string EmployeeLocationAssignmentCreated = "Employee location assignment created successfully.";
            public const string EmployeeLocationAssignmentUpdated = "Employee location assignment updated successfully.";
            public const string EmployeeLocationAssignmentStatusUpdated = "Employee location assignment status updated successfully.";
            public const string EmployeeLocationAssignmentDeleted = "Employee location assignment deleted successfully.";
            public const string EmployeeDeviceEnrollmentCreated = "Employee device enrollment created successfully.";
            public const string EmployeeDeviceEnrollmentUpdated = "Employee device enrollment updated successfully.";
            public const string EmployeeDeviceEnrollmentStatusUpdated = "Employee device enrollment status updated successfully.";
            public const string EmployeeDeviceEnrollmentDeleted = "Employee device enrollment deleted successfully.";
            public const string EmployeeWorkArrangementCreated = "Employee work arrangement created successfully.";
            public const string EmployeeWorkArrangementUpdated = "Employee work arrangement updated successfully.";
            public const string EmployeeWorkArrangementStatusUpdated = "Employee work arrangement status updated successfully.";
            public const string EmployeeWorkArrangementDeleted = "Employee work arrangement deleted successfully.";
            public const string EmployeeWorkPatternCreated = "Employee work pattern created successfully.";
            public const string EmployeeWorkPatternUpdated = "Employee work pattern updated successfully.";
            public const string EmployeeWorkPatternStatusUpdated = "Employee work pattern status updated successfully.";
            public const string EmployeeWorkPatternDeleted = "Employee work pattern deleted successfully.";
            public const string EmployeeWorkModeOverrideCreated = "Work mode override request created successfully.";
            public const string EmployeeWorkModeOverrideUpdated = "Work mode override request updated successfully.";
            public const string EmployeeWorkModeOverrideStatusUpdated = "Work mode override request status updated successfully.";
            public const string EmployeeWorkModeOverrideDeleted = "Work mode override request deleted successfully.";
            public const string DeviceMasterCreated = "Device master created successfully.";
            public const string DeviceMasterUpdated = "Device master updated successfully.";
            public const string DeviceMasterStatusUpdated = "Device master status updated successfully.";
            public const string DeviceMasterDeleted = "Device master deleted successfully.";
            public const string DeviceMasterRetrieved = "Device masters retrieved successfully.";
            public const string TenantDeviceCreated = "Tenant device created successfully.";
            public const string TenantDeviceUpdated = "Tenant device updated successfully.";
            public const string TenantDeviceStatusUpdated = "Tenant device status updated successfully.";
            public const string TenantDeviceDeleted = "Tenant device deleted successfully.";
            public const string TenantDeviceRetrieved = "Tenant devices retrieved successfully.";
            public const string TenantDeviceConfigurationCreated = "Tenant device configuration created successfully.";
            public const string TenantDeviceConfigurationUpdated = "Tenant device configuration updated successfully.";
            public const string TenantDeviceConfigurationDeleted = "Tenant device configuration deleted successfully.";
            public const string TenantDeviceConfigurationRetrieved = "Tenant device configurations retrieved successfully.";
        }

        #endregion

        #region Application Limits

        /// <summary>
        /// Defines the maximum number of roles that may be assigned to one Employee.
        /// </summary>
        public const int MaxEmployeeRoleAssigned = 2;

        #endregion

        #region Module Scopes

        /// <summary>
        /// Identifies modules that are available in a tenant application scope.
        /// </summary>
        public const int TenantModuleScope = 1;

        /// <summary>
        /// Identifies modules that are available in the Host application scope.
        /// </summary>
        public const int HostModuleScope = 2;

        #endregion

        #region Host Token Claims

        /// <summary>
        /// Identifies the Host-user identifier claim in a Host access token.
        /// </summary>
        public const string HostUserIdClaim = "HostUserId";

        /// <summary>
        /// Identifies the Host-role identifier claim in a Host access token.
        /// </summary>
        public const string HostRoleIdClaim = "HostRoleId";

        /// <summary>
        /// Identifies the Host login identifier claim in a Host access token.
        /// </summary>
        public const string LoginIdClaim = "LoginId";

        /// <summary>
        /// Identifies the application principal-type claim.
        /// </summary>
        public const string UserTypeClaim = "UserType";

        /// <summary>
        /// Identifies a Host principal in the application principal-type claim.
        /// </summary>
        public const string HostUserType = "Host";

        /// <summary>
        /// Identifies the verified current Host role authorized to administer Parent Modules.
        /// </summary>
        public const long SuperAdminHostRoleId = 1;

        /// <summary>
        /// Identifies an access token in the token-purpose claim.
        /// </summary>
        public const string AccessTokenPurpose = "Access";

        #endregion

        public static readonly int DeviceTypeWeb = 1;
        public static readonly int DeviceTypeMobile = 2;
        public static readonly int DeviceTypeForAll = 3;
        public static int EmployeeRoll = 14;

        // Add other constants as needed
        public static readonly string DefaultDateFormat = "yyyy-MM-dd";
        // etc.
    }


    public static class ConstantValues
    {
        public enum ContactTypeEnum
        {
            None = 0,
            Personal = 1,
            Official = 2
        }
        // ==============================
        // Default SMTP (Brevo Fallback)
        // ==============================


        public const string DefaultSmtpHost = "smtp-relay.brevo.com";
        public const int DefaultSmtpPort = 2525;

        public const string DefaultSmtpUserName = "a4e423001@smtp-brevo.com";

        public const bool DefaultSmtpEnableSsl = true;

        // ==============================
        // Email Defaults
        // ==============================

        public const string DefaultFromEmail = "hr@quecksilber.in";
        public const string DefaultFromName = "AxionPro HRMS";

        public const string TenantFolder = "tenants";
        public const string EmployeeFolder = "employees";
        public const string ProfileFolder = "profile";
        public const string GapDocFolder = "gap-doc";
        public const string AssetsFolder = "assets";
        public const string IdentityFolder = "identity";
        public const string BankFolder = "bank";
        public const string PoliciesFolder = "policies";
        public const string TenantPoliciesFolder = "tenant-policies";
        public const string DependentFolder = "dependent";
        public const string EducationFolder = "education";
        public const string ExperienceFolder = "experience";

        public const string DefaultInsurancePolicy = "Insurance Policy";
        public const string DefaultLeavePolicy = "Leave Policy";

        public static readonly string invalidCredential = "Invalid credentials";
        public static readonly int ParmanentEmployeeType = 1;
        public static readonly string Duplicate = "Name you inserted is already exist";
        public static readonly string userMissingAttendanceProfile = "Attendance settings not configured or not matched! for this employee.";
        public static readonly string attendanceNotAllowed = "Attendance is not allowed for the employee based on current settings";
        public static readonly string outOfGeoFence = "You are outside the geofence area and cannot mark attendance.";
        public static readonly string invalidId = "Invalid Id";
        public static readonly string invalidPassword = "Invalid credentials";
        public static readonly string successMessage = "Request processed successfully";
        public static readonly string attendanceSucessful = "Attendance successfully marked";
        public static readonly string attendancefail = "Attendance not marked please try again";
        public static readonly bool isSucceeded = true;
        public static readonly bool fail = false;
        public static readonly DateTime ExpireTokenDate = DateTime.UtcNow.AddDays(5);
        public static readonly string IP = "100.100.100.100";

        public static readonly string SuperAdminRoleName = "Host Admin";
        public static readonly string SuperAdminRoleType = "SYSTEM";
        public static readonly string SuperAdminRoleCode = "Auth_0";
        public static readonly int Web = 1;

        public static readonly string TenantAdminRoleName = "Super-Admin";
        //public static readonly string TenantAdminRoleType = "Employee";

        public static readonly int RoleTypeAdmin = 1;
        public static readonly int RoleTypeEmployee = 2;
        public static readonly int RoleTypeManager = 3;

         

        public static readonly int SetPassword = 1;
        public static readonly int Auth = 2;
        public static readonly int ForgotPassword = 3;


        public static readonly string TenantHRRoleCode = "TENANT_HR";
        public static readonly string TenantHRRoleType = "TENANT_OPERATIONAL";
        public static readonly string TenantManagerRoleName = "Manager";


        public static readonly string TenantEmployeeRoleCode = "TENANT_EMPLOYEE";
        public static readonly string TenantEmployeeRoleType = "EMPLOYEE";
        public static readonly string TenantEmployeeRoleName = "Employee";




        public static readonly string TenantAllRoleRemark = "This is an auto-generated Admin account by AI for the initial setup of the tenant.";
        public static readonly bool IsByDefaultTrue = true;
        public static readonly bool IsByDefaultFalse = false;
        public static readonly long SystemUserIdByDefaultZero = 0; // For system-generated entries
        public static readonly string DefaultPassword = "Guest@123"; // For system-generated entries



        #region Email Templates
        public static readonly string WelcomeEmail = "WELCOME_EMAIL";
        #endregion

        //   public static readonly DateOnly SystemOnlyTodaysDate= DateOnly.MaxValue;

        //int adminRoleId = await _unitOfWork.RoleRepository.GetRoleIdByRoleInfoAsync(role);
        //public static readonly string RoleCode = "Super-Admin";
        //public static readonly string AdminRoleName = "Admin";
        //public static readonly string AdminRoleRemark = "This is an auto-generated Admin account by AI for the initial setup of the tenant.";

    }

}
