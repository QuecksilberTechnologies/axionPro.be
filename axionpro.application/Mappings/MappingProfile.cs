// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Configures application DTO-to-domain mapping rules.
// ================================================================

using AutoMapper;
using axionpro.application.Constants;
using axionpro.application.DTOs.Client;
using axionpro.application.DTOs.Department;
using axionpro.application.DTOs.Designation;
using axionpro.application.DTOs.EmailTemplate;
using axionpro.application.DTOs.Employee;
using axionpro.application.DTOs.Employee.AccessControlReadOnlyType;
using axionpro.application.DTOs.Gender;
using axionpro.application.DTOs.Leave;
using axionpro.application.DTOs.Leave.LeaveRule;
using axionpro.application.DTOs.Manager.ReportingType;
using axionpro.application.DTOs.Module;
using axionpro.application.DTOs.Module.NewFolder;
using axionpro.application.DTOs.ModuleOperation;
using axionpro.application.DTOs.Operation;
using axionpro.application.DTOs.OrganizationHolidayCalendar;
using axionpro.application.DTOs.PolicyType;
using axionpro.application.DTOs.Registration;
using axionpro.application.DTOs.Role;
using axionpro.application.DTOs.SubscriptionModule;
using axionpro.application.DTOs.Tenant;
using axionpro.application.DTOs.TenantIndustry;
using axionpro.application.DTOs.Transport;
using axionpro.application.DTOs.UserLogin;
using axionpro.application.DTOs.WorkflowStage;
using axionpro.application.DTOS.AssetDTO.asset;
using axionpro.application.DTOS.AssetDTO.category;
using axionpro.application.DTOS.AssetDTO.status;
using axionpro.application.DTOS.AssetDTO.type;
using axionpro.application.DTOS.Department;
using axionpro.application.DTOS.Employee.Bank;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.DTOS.Employee.Contact;
using axionpro.application.DTOS.Employee.Dependent;
using axionpro.application.DTOS.Employee.Education;
using axionpro.application.DTOS.Employee.Experience;
using axionpro.application.DTOS.Gender;
using axionpro.application.DTOS.Host;
using axionpro.application.DTOS.InsurancePoliciesMapping;
using axionpro.application.DTOS.InsurancePolicy;
using axionpro.application.DTOS.Location;
using axionpro.application.DTOS.Module.CommonModule;
using axionpro.application.DTOS.Module.ParentModule;
using axionpro.application.DTOS.Module.SubModule;
using axionpro.application.DTOS.PolicyTypeDocument;
using axionpro.application.DTOS.Role;
using axionpro.application.DTOS.SubscriptionModule;


//using axionpro.application.DTOS.Module.ParentModule;
using axionpro.application.DTOS.TicketDTO.Classification;
using axionpro.application.DTOS.TicketDTO.Header;
using axionpro.application.DTOS.TicketDTO.TicketType;
using axionpro.application.DTOS.UserRoles;
using axionpro.application.DTOS.TenantConfiguration;
using axionpro.application.Features.TickeAllCmd.Classification;
using axionpro.domain.Entity;
using FluentValidation;


namespace axionpro.application.Mappings
{
    public class MappingProfile : Profile
    {
        private static string CleanSkillSet(string skillSet)
        {
            if (string.IsNullOrEmpty(skillSet))
                return string.Empty;

            return string.Join(",", skillSet.Split(',')
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrEmpty(s))
            );
        }

        private static string EmployeeName(Employee? employee) =>
            string.Join(' ', new[] { employee?.FirstName, employee?.MiddleName, employee?.LastName }.Where(value => !string.IsNullOrWhiteSpace(value)));


        public MappingProfile()
        {

            #region Host Device Management Mappings

            CreateMap<CreateDeviceMasterRequestDTO, DeviceMaster>()
                .ForMember(
                    d => d.DeviceType,
                    o => o.MapFrom(s => (short)s.DeviceType))
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.AddedById, o => o.Ignore())
                .ForMember(d => d.AddedDateTime, o => o.Ignore())
                .ForMember(d => d.UpdatedById, o => o.Ignore())
                .ForMember(d => d.UpdatedDateTime, o => o.Ignore())
                .ForMember(d => d.SoftDeletedById, o => o.Ignore())
                .ForMember(d => d.SoftDeletedDateTime, o => o.Ignore())
                .ForMember(d => d.IsSoftDeleted, o => o.Ignore())
                .ForMember(d => d.TenantDevice, o => o.Ignore());

            CreateMap<UpdateDeviceMasterRequestDTO, DeviceMaster>()
                .ForMember(
                    d => d.DeviceType,
                    o => o.MapFrom(s => (short)s.DeviceType))
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.AddedById, o => o.Ignore())
                .ForMember(d => d.AddedDateTime, o => o.Ignore())
                .ForMember(d => d.UpdatedById, o => o.Ignore())
                .ForMember(d => d.UpdatedDateTime, o => o.Ignore())
                .ForMember(d => d.SoftDeletedById, o => o.Ignore())
                .ForMember(d => d.SoftDeletedDateTime, o => o.Ignore())
                .ForMember(d => d.IsSoftDeleted, o => o.Ignore())
                .ForMember(d => d.TenantDevice, o => o.Ignore());

            CreateMap<DeviceMaster, DeviceMasterResponseDTO>()
                .ForMember(
                    d => d.DeviceType,
                    o => o.MapFrom(s => (DeviceType)s.DeviceType))
                .ForMember(
                    d => d.DeviceTypeName,
                    o => o.MapFrom(s => ((DeviceType)s.DeviceType).ToString()));

            CreateMap<CreateTenantDeviceRequestDTO, TenantDevice>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.TenantId, o => o.Ignore())
                .ForMember(d => d.AddedById, o => o.Ignore())
                .ForMember(d => d.AddedDateTime, o => o.Ignore())
                .ForMember(d => d.UpdatedById, o => o.Ignore())
                .ForMember(d => d.UpdatedDateTime, o => o.Ignore())
                .ForMember(d => d.SoftDeletedById, o => o.Ignore())
                .ForMember(d => d.SoftDeletedDateTime, o => o.Ignore())
                .ForMember(d => d.IsSoftDeleted, o => o.Ignore())
                .ForMember(d => d.Tenant, o => o.Ignore())
                .ForMember(d => d.TenantLocation, o => o.Ignore())
                .ForMember(d => d.DeviceMaster, o => o.Ignore())
                .ForMember(d => d.TenantDeviceConfiguration, o => o.Ignore());

            CreateMap<UpdateTenantDeviceRequestDTO, TenantDevice>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.TenantId, o => o.Ignore())
                .ForMember(d => d.AddedById, o => o.Ignore())
                .ForMember(d => d.AddedDateTime, o => o.Ignore())
                .ForMember(d => d.UpdatedById, o => o.Ignore())
                .ForMember(d => d.UpdatedDateTime, o => o.Ignore())
                .ForMember(d => d.SoftDeletedById, o => o.Ignore())
                .ForMember(d => d.SoftDeletedDateTime, o => o.Ignore())
                .ForMember(d => d.IsSoftDeleted, o => o.Ignore())
                .ForMember(d => d.Tenant, o => o.Ignore())
                .ForMember(d => d.TenantLocation, o => o.Ignore())
                .ForMember(d => d.DeviceMaster, o => o.Ignore())
                .ForMember(d => d.TenantDeviceConfiguration, o => o.Ignore());

            CreateMap<TenantDevice, TenantDeviceResponseDTO>()
                .ForMember(d => d.TenantId, o => o.Ignore())
                .ForMember(
                    d => d.TenantName,
                    o => o.MapFrom(s =>
                        s.Tenant != null
                            ? s.Tenant.CompanyName
                            : null))
                .ForMember(
                    d => d.TenantLocationName,
                    o => o.MapFrom(s =>
                        s.TenantLocation != null
                            ? s.TenantLocation.LocationName
                            : null))
                .ForMember(
                    d => d.LocationCode,
                    o => o.MapFrom(s =>
                        s.TenantLocation != null
                            ? s.TenantLocation.LocationCode
                            : null))
                .ForMember(
                    d => d.DeviceMasterName,
                    o => o.MapFrom(s =>
                        s.DeviceMaster != null
                            ? s.DeviceMaster.DeviceName
                            : null))
                .ForMember(
                    d => d.DeviceMasterModelNo,
                    o => o.MapFrom(s =>
                        s.DeviceMaster != null
                            ? s.DeviceMaster.ModelNo
                            : null))
                .ForMember(d => d.HasConfiguration, o => o.MapFrom(s => s.TenantDeviceConfiguration != null));

            CreateMap<CreateTenantDeviceConfigurationRequestDTO, TenantDeviceConfiguration>()
                .ForMember(d => d.CommunicationType, o => o.MapFrom(s => s.CommunicationType.HasValue ? (short?)s.CommunicationType.Value : null))
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.AddedById, o => o.Ignore())
                .ForMember(d => d.AddedDateTime, o => o.Ignore())
                .ForMember(d => d.UpdatedById, o => o.Ignore())
                .ForMember(d => d.UpdatedDateTime, o => o.Ignore())
                .ForMember(d => d.LastHeartbeatDateTime, o => o.Ignore())
                .ForMember(d => d.LastSyncDateTime, o => o.Ignore())
                .ForMember(d => d.LastAttendanceReceivedDateTime, o => o.Ignore())
                .ForMember(d => d.LastSuccessfulConnectionDateTime, o => o.Ignore())
                .ForMember(d => d.LastFailedConnectionDateTime, o => o.Ignore())
                .ForMember(d => d.LastConnectionError, o => o.Ignore())
                .ForMember(d => d.TenantDevice, o => o.Ignore());

            CreateMap<UpdateTenantDeviceConfigurationRequestDTO, TenantDeviceConfiguration>()
                .ForMember(d => d.CommunicationType, o => o.MapFrom(s => s.CommunicationType.HasValue ? (short?)s.CommunicationType.Value : null))
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.AddedById, o => o.Ignore())
                .ForMember(d => d.AddedDateTime, o => o.Ignore())
                .ForMember(d => d.UpdatedById, o => o.Ignore())
                .ForMember(d => d.UpdatedDateTime, o => o.Ignore())
                .ForMember(d => d.LastHeartbeatDateTime, o => o.Ignore())
                .ForMember(d => d.LastSyncDateTime, o => o.Ignore())
                .ForMember(d => d.LastAttendanceReceivedDateTime, o => o.Ignore())
                .ForMember(d => d.LastSuccessfulConnectionDateTime, o => o.Ignore())
                .ForMember(d => d.LastFailedConnectionDateTime, o => o.Ignore())
                .ForMember(d => d.LastConnectionError, o => o.Ignore())
                .ForMember(d => d.TenantDevice, o => o.Ignore());

            CreateMap<TenantDeviceConfiguration, TenantDeviceConfigurationResponseDTO>()
                .ForMember(d => d.TenantId, o => o.Ignore())
                .ForMember(d => d.CommunicationType, o => o.MapFrom(s => s.CommunicationType.HasValue ? (DeviceCommunicationType?)s.CommunicationType.Value : null))
                .ForMember(d => d.DeviceCode, o => o.MapFrom(s => s.TenantDevice != null ? s.TenantDevice.DeviceCode : null))
                .ForMember(d => d.DeviceName, o => o.MapFrom(s => s.TenantDevice != null ? s.TenantDevice.DeviceName : null))
                .ForMember(d => d.DeviceMasterName, o => o.MapFrom(s => s.TenantDevice != null && s.TenantDevice.DeviceMaster != null ? s.TenantDevice.DeviceMaster.DeviceName : null))
                .ForMember(d => d.DeviceMasterSNo, o => o.MapFrom(s => s.TenantDevice != null && s.TenantDevice.DeviceMaster != null ? s.TenantDevice.DeviceMaster.SNo : null));

            #endregion

            #region Tenant Configuration Mappings

            CreateMap<CreateTenantLocationRequestDTO, TenantLocation>()
                .ForMember(d => d.LocationType, o => o.MapFrom(s => checked((short)s.LocationType)))
                .ForMember(d => d.Id, o => o.Ignore()).ForMember(d => d.TenantId, o => o.Ignore())
                .ForMember(d => d.AddedById, o => o.Ignore()).ForMember(d => d.AddedDateTime, o => o.Ignore())
                .ForMember(d => d.UpdatedById, o => o.Ignore()).ForMember(d => d.UpdatedDateTime, o => o.Ignore())
                .ForMember(d => d.SoftDeletedById, o => o.Ignore()).ForMember(d => d.SoftDeletedDateTime, o => o.Ignore())
                .ForMember(d => d.IsSoftDeleted, o => o.Ignore()).ForMember(d => d.Tenant, o => o.Ignore())
                .ForMember(d => d.Country, o => o.Ignore()).ForMember(d => d.City, o => o.Ignore())
                .ForMember(d => d.TenantDevice, o => o.Ignore()).ForMember(d => d.EmployeeLocationAssignment, o => o.Ignore())
                .ForMember(d => d.EmployeeWorkArrangement, o => o.Ignore()).ForMember(d => d.EmployeeWorkPattern, o => o.Ignore())
                .ForMember(d => d.EmployeeWorkModeOverrideRequest, o => o.Ignore());
            CreateMap<UpdateTenantLocationRequestDTO, TenantLocation>()
                .IncludeBase<CreateTenantLocationRequestDTO, TenantLocation>()
                .ForMember(d => d.Id, o => o.Ignore());

            CreateMap<CreateAttendancePolicyRequestDTO, AttendancePolicy>()
                .ForMember(d => d.AttendanceLocationScope, o => o.MapFrom(s => checked((short)s.AttendanceLocationScope)))
                .ForMember(d => d.Id, o => o.Ignore()).ForMember(d => d.TenantId, o => o.Ignore())
                .ForMember(d => d.AddedById, o => o.Ignore()).ForMember(d => d.AddedDateTime, o => o.Ignore())
                .ForMember(d => d.UpdatedById, o => o.Ignore()).ForMember(d => d.UpdatedDateTime, o => o.Ignore())
                .ForMember(d => d.SoftDeletedById, o => o.Ignore()).ForMember(d => d.SoftDeletedDateTime, o => o.Ignore())
                .ForMember(d => d.IsSoftDeleted, o => o.Ignore()).ForMember(d => d.Tenant, o => o.Ignore())
                .ForMember(d => d.PolicyType, o => o.Ignore()).ForMember(d => d.EmployeeWorkArrangement, o => o.Ignore());
            CreateMap<UpdateAttendancePolicyRequestDTO, AttendancePolicy>()
                .IncludeBase<CreateAttendancePolicyRequestDTO, AttendancePolicy>()
                .ForMember(d => d.Id, o => o.Ignore());

            CreateMap<CreateEmployeeLocationAssignmentRequestDTO, EmployeeLocationAssignment>()
                .ForMember(d => d.EmployeeId, o => o.Ignore())
                .ForMember(d => d.Id, o => o.Ignore()).ForMember(d => d.TenantId, o => o.Ignore())
                .ForMember(d => d.AddedById, o => o.Ignore()).ForMember(d => d.AddedDateTime, o => o.Ignore())
                .ForMember(d => d.UpdatedById, o => o.Ignore()).ForMember(d => d.UpdatedDateTime, o => o.Ignore())
                .ForMember(d => d.SoftDeletedById, o => o.Ignore()).ForMember(d => d.SoftDeletedDateTime, o => o.Ignore())
                .ForMember(d => d.IsSoftDeleted, o => o.Ignore()).ForMember(d => d.Tenant, o => o.Ignore())
                .ForMember(d => d.Employee, o => o.Ignore()).ForMember(d => d.TenantLocation, o => o.Ignore());
            CreateMap<UpdateEmployeeLocationAssignmentRequestDTO, EmployeeLocationAssignment>()
                .IncludeBase<CreateEmployeeLocationAssignmentRequestDTO, EmployeeLocationAssignment>()
                .ForMember(d => d.Id, o => o.Ignore());

            CreateMap<CreateEmployeeDeviceEnrollmentRequestDTO, EmployeeDeviceEnrollment>()
                .ForMember(d => d.EmployeeId, o => o.Ignore())
                .ForMember(d => d.Id, o => o.Ignore()).ForMember(d => d.TenantId, o => o.Ignore())
                .ForMember(d => d.LastSyncedDateTime, o => o.Ignore()).ForMember(d => d.AddedById, o => o.Ignore()).ForMember(d => d.AddedDateTime, o => o.Ignore())
                .ForMember(d => d.UpdatedById, o => o.Ignore()).ForMember(d => d.UpdatedDateTime, o => o.Ignore())
                .ForMember(d => d.SoftDeletedById, o => o.Ignore()).ForMember(d => d.SoftDeletedDateTime, o => o.Ignore())
                .ForMember(d => d.IsSoftDeleted, o => o.Ignore()).ForMember(d => d.Tenant, o => o.Ignore())
                .ForMember(d => d.Employee, o => o.Ignore());
            CreateMap<UpdateEmployeeDeviceEnrollmentRequestDTO, EmployeeDeviceEnrollment>()
                .IncludeBase<CreateEmployeeDeviceEnrollmentRequestDTO, EmployeeDeviceEnrollment>()
                .ForMember(d => d.Id, o => o.Ignore());

            CreateMap<CreateEmployeeWorkArrangementRequestDTO, EmployeeWorkArrangement>()
                .ForMember(d => d.WorkMode, o => o.MapFrom(s => checked((short)s.WorkMode)))
                .ForMember(d => d.HybridType, o => o.MapFrom(s => s.HybridType.HasValue ? checked((short)s.HybridType.Value) : (short?)null))
                .ForMember(d => d.EmployeeId, o => o.Ignore())
                .ForMember(d => d.Id, o => o.Ignore()).ForMember(d => d.TenantId, o => o.Ignore())
                .ForMember(d => d.AddedById, o => o.Ignore()).ForMember(d => d.AddedDateTime, o => o.Ignore())
                .ForMember(d => d.UpdatedById, o => o.Ignore()).ForMember(d => d.UpdatedDateTime, o => o.Ignore())
                .ForMember(d => d.SoftDeletedById, o => o.Ignore()).ForMember(d => d.SoftDeletedDateTime, o => o.Ignore())
                .ForMember(d => d.IsSoftDeleted, o => o.Ignore()).ForMember(d => d.Tenant, o => o.Ignore())
                .ForMember(d => d.Employee, o => o.Ignore()).ForMember(d => d.AttendancePolicy, o => o.Ignore())
                .ForMember(d => d.PrimaryTenantLocation, o => o.Ignore()).ForMember(d => d.EmployeeWorkPattern, o => o.Ignore())
                .ForMember(d => d.EmployeeWorkModeOverrideRequest, o => o.Ignore());
            CreateMap<UpdateEmployeeWorkArrangementRequestDTO, EmployeeWorkArrangement>()
                .IncludeBase<CreateEmployeeWorkArrangementRequestDTO, EmployeeWorkArrangement>()
                .ForMember(d => d.Id, o => o.Ignore());

            CreateMap<CreateEmployeeWorkPatternRequestDTO, EmployeeWorkPattern>()
                .ForMember(d => d.DayOfWeek, o => o.MapFrom(s => checked((short)s.DayOfWeek)))
                .ForMember(d => d.WorkMode, o => o.MapFrom(s => checked((short)s.WorkMode)))
                .ForMember(d => d.Id, o => o.Ignore()).ForMember(d => d.TenantId, o => o.Ignore())
                .ForMember(d => d.AddedById, o => o.Ignore()).ForMember(d => d.AddedDateTime, o => o.Ignore())
                .ForMember(d => d.UpdatedById, o => o.Ignore()).ForMember(d => d.UpdatedDateTime, o => o.Ignore())
                .ForMember(d => d.SoftDeletedById, o => o.Ignore()).ForMember(d => d.SoftDeletedDateTime, o => o.Ignore())
                .ForMember(d => d.IsSoftDeleted, o => o.Ignore()).ForMember(d => d.Tenant, o => o.Ignore())
                .ForMember(d => d.EmployeeWorkArrangement, o => o.Ignore()).ForMember(d => d.TenantLocation, o => o.Ignore());
            CreateMap<UpdateEmployeeWorkPatternRequestDTO, EmployeeWorkPattern>()
                .IncludeBase<CreateEmployeeWorkPatternRequestDTO, EmployeeWorkPattern>()
                .ForMember(d => d.Id, o => o.Ignore());

            CreateMap<CreateEmployeeWorkModeOverrideRequestDTO, EmployeeWorkModeOverrideRequest>()
                .ForMember(d => d.RequestedWorkMode, o => o.MapFrom(s => checked((short)s.RequestedWorkMode)))
                .ForMember(d => d.EmployeeId, o => o.Ignore())
                .ForMember(d => d.Id, o => o.Ignore()).ForMember(d => d.TenantId, o => o.Ignore())
                .ForMember(d => d.ApprovalStatus, o => o.Ignore()).ForMember(d => d.ApprovedById, o => o.Ignore()).ForMember(d => d.ApprovedDateTime, o => o.Ignore())
                .ForMember(d => d.ApprovalRemark, o => o.Ignore()).ForMember(d => d.RejectedById, o => o.Ignore()).ForMember(d => d.RejectedDateTime, o => o.Ignore()).ForMember(d => d.RejectionRemark, o => o.Ignore())
                .ForMember(d => d.AddedById, o => o.Ignore()).ForMember(d => d.AddedDateTime, o => o.Ignore())
                .ForMember(d => d.UpdatedById, o => o.Ignore()).ForMember(d => d.UpdatedDateTime, o => o.Ignore())
                .ForMember(d => d.SoftDeletedById, o => o.Ignore()).ForMember(d => d.SoftDeletedDateTime, o => o.Ignore())
                .ForMember(d => d.IsSoftDeleted, o => o.Ignore()).ForMember(d => d.Tenant, o => o.Ignore())
                .ForMember(d => d.Employee, o => o.Ignore()).ForMember(d => d.EmployeeWorkArrangement, o => o.Ignore())
                .ForMember(d => d.TenantLocation, o => o.Ignore());
            CreateMap<UpdateEmployeeWorkModeOverrideRequestDTO, EmployeeWorkModeOverrideRequest>()
                .IncludeBase<CreateEmployeeWorkModeOverrideRequestDTO, EmployeeWorkModeOverrideRequest>()
                .ForMember(d => d.Id, o => o.Ignore());

            CreateMap<TenantLocation, TenantLocationResponseDTO>()
                .ForMember(d => d.LocationType, o => o.MapFrom(s => (TenantLocationType)s.LocationType))
                .ForMember(d => d.LocationTypeName, o => o.MapFrom(s => ((TenantLocationType)s.LocationType).ToString()))
                .ForMember(d => d.CountryName, o => o.MapFrom(s => s.Country != null ? s.Country.CountryName : string.Empty))
                .ForMember(d => d.StateName, o => o.MapFrom(_ => (string?)null))
                .ForMember(d => d.CityName, o => o.MapFrom(s => s.City != null ? s.City.CityName : null));
            CreateMap<AttendancePolicy, AttendancePolicyResponseDTO>()
                .ForMember(d => d.AttendanceLocationScope, o => o.MapFrom(s => (AttendanceLocationScope)s.AttendanceLocationScope))
                .ForMember(d => d.PolicyTypeName, o => o.MapFrom(s => s.PolicyType != null ? s.PolicyType.PolicyName : string.Empty))
                .ForMember(d => d.AttendanceLocationScopeName, o => o.MapFrom(s => ((AttendanceLocationScope)s.AttendanceLocationScope).ToString()));
            CreateMap<EmployeeLocationAssignment, EmployeeLocationAssignmentResponseDTO>()
                .ForMember(d => d.EmployeeName, o => o.MapFrom(s => EmployeeName(s.Employee)))
                .ForMember(d => d.EmployeeCode, o => o.MapFrom(s => s.Employee != null ? s.Employee.EmployementCode : null))
                .ForMember(d => d.TenantLocationName, o => o.MapFrom(s => s.TenantLocation != null ? s.TenantLocation.LocationName : string.Empty))
                .ForMember(d => d.LocationCode, o => o.MapFrom(s => s.TenantLocation != null ? s.TenantLocation.LocationCode : string.Empty));
            CreateMap<EmployeeDeviceEnrollment, EmployeeDeviceEnrollmentResponseDTO>()
                .ForMember(d => d.EmployeeName, o => o.MapFrom(s => EmployeeName(s.Employee)))
                .ForMember(d => d.EmployeeCode, o => o.MapFrom(s => s.Employee != null ? s.Employee.EmployementCode : null))
                .ForMember(d => d.DeviceCode, o => o.MapFrom(_ => string.Empty))
                .ForMember(d => d.DeviceName, o => o.MapFrom(_ => (string?)null))
                .ForMember(d => d.SerialNumber, o => o.MapFrom(_ => string.Empty))
                .ForMember(d => d.TenantLocationId, o => o.MapFrom(_ => 0L))
                .ForMember(d => d.TenantLocationName, o => o.MapFrom(_ => string.Empty));
            CreateMap<EmployeeWorkArrangement, EmployeeWorkArrangementResponseDTO>()
                .ForMember(d => d.WorkMode, o => o.MapFrom(s => (WorkMode)s.WorkMode))
                .ForMember(d => d.HybridType, o => o.MapFrom(s => s.HybridType.HasValue ? (HybridType?)s.HybridType.Value : null))
                .ForMember(d => d.EmployeeName, o => o.MapFrom(s => EmployeeName(s.Employee)))
                .ForMember(d => d.AttendancePolicyName, o => o.MapFrom(s => s.AttendancePolicy != null ? s.AttendancePolicy.PolicyName : string.Empty))
                .ForMember(d => d.PrimaryTenantLocationName, o => o.MapFrom(s => s.PrimaryTenantLocation != null ? s.PrimaryTenantLocation.LocationName : null))
                .ForMember(d => d.WorkModeName, o => o.MapFrom(s => ((WorkMode)s.WorkMode).ToString()))
                .ForMember(d => d.HybridTypeName, o => o.MapFrom(s => s.HybridType.HasValue ? ((HybridType)s.HybridType.Value).ToString() : null));
            CreateMap<EmployeeWorkPattern, EmployeeWorkPatternResponseDTO>()
                .ForMember(d => d.DayOfWeek, o => o.MapFrom(s => (WorkPatternDay)s.DayOfWeek))
                .ForMember(d => d.WorkMode, o => o.MapFrom(s => (WorkMode)s.WorkMode))
                .ForMember(d => d.DayOfWeekName, o => o.MapFrom(s => ((WorkPatternDay)s.DayOfWeek).ToString()))
                .ForMember(d => d.WorkModeName, o => o.MapFrom(s => ((WorkMode)s.WorkMode).ToString()))
                .ForMember(d => d.TenantLocationName, o => o.MapFrom(s => s.TenantLocation != null ? s.TenantLocation.LocationName : null));
            CreateMap<EmployeeWorkModeOverrideRequest, EmployeeWorkModeOverrideResponseDTO>()
                .ForMember(d => d.RequestedWorkMode, o => o.MapFrom(s => (WorkMode)s.RequestedWorkMode))
                .ForMember(d => d.ApprovalStatus, o => o.MapFrom(s => (WorkModeOverrideApprovalStatus)s.ApprovalStatus))
                .ForMember(d => d.EmployeeName, o => o.MapFrom(s => EmployeeName(s.Employee)))
                .ForMember(d => d.RequestedWorkModeName, o => o.MapFrom(s => ((WorkMode)s.RequestedWorkMode).ToString()))
                .ForMember(d => d.TenantLocationName, o => o.MapFrom(s => s.TenantLocation != null ? s.TenantLocation.LocationName : null))
                .ForMember(d => d.ApprovalStatusName, o => o.MapFrom(s => ((WorkModeOverrideApprovalStatus)s.ApprovalStatus).ToString()));

            #endregion

            #region New Login Mappings

            CreateMap<NewLoginBootstrapReadModel, NewLoginUserContextDTO>()
                .ForMember(destination => destination.EmployeeId, options => options.Ignore())
                .ForMember(destination => destination.FullName, options => options.Ignore())
                .ForMember(destination => destination.ProfileImageUrl, options => options.Ignore())
                .ForMember(destination => destination.TenantId, options => options.Ignore())
                .ForMember(destination => destination.PrimaryRole, options => options.Ignore())
                .ForMember(destination => destination.SecondaryRoles, options => options.Ignore());

            CreateMap<Role, NewLoginRoleDTO>()
                .ForMember(destination => destination.RoleId, options => options.MapFrom(source => source.Id))
                .ForMember(destination => destination.RoleName, options => options.MapFrom(source => source.RoleName ?? string.Empty))
                .ForMember(destination => destination.RoleTypeId, options => options.MapFrom(source => source.RoleType))
                .ForMember(destination => destination.RoleTypeName, options => options.MapFrom(source =>
                    source.RoleType == ConstantValues.RoleTypeAdmin ? "Super Admin" :
                    source.RoleType == ConstantValues.RoleTypeEmployee ? "Employee" :
                    source.RoleType == ConstantValues.RoleTypeManager ? "Manager" :
                    "Unknown"));

            #endregion


            CreateMap<AddAssetRequestDTO, Asset>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TenantId, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.IsSoftDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.SoftDeletedById, opt => opt.Ignore())
                .ForMember(dest => dest.AddedById, opt => opt.Ignore())
                .ForMember(dest => dest.AddedDateTime, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedById, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedDateTime, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedDateTime, opt => opt.Ignore())
                .ForMember(dest => dest.Qrcode, opt => opt.Ignore())
                .ForMember(dest => dest.AssetAssignment, opt => opt.Ignore())
                .ForMember(dest => dest.AssetImage, opt => opt.Ignore())
                .ForMember(dest => dest.AssetStatus, opt => opt.Ignore())
                .ForMember(dest => dest.AssetType, opt => opt.Ignore());

            CreateMap<AssetCategory, GetCategoryResponseDTO>().ReverseMap();
            CreateMap<AssetStatus, GetStatusResponseDTO>().ReverseMap();
            CreateMap<AssetCategory, GetCategoryResponseDTO>().ReverseMap();

            CreateMap<AddCategoryReqestDTO, AssetCategory>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TenantId, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.IsSoftDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.AddedById, opt => opt.Ignore())
                .ForMember(dest => dest.AddedDateTime, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedById, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedDateTime, opt => opt.Ignore())
                .ForMember(dest => dest.SoftDeletedById, opt => opt.Ignore())
                .ForMember(dest => dest.SoftDeletedDateTime, opt => opt.Ignore())
                .ForMember(dest => dest.AssetType, opt => opt.Ignore())
                .ForMember(dest => dest.Tenant, opt => opt.Ignore());

            CreateMap<UpdateCategoryReqestDTO, AssetCategory>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TenantId, opt => opt.Ignore())
                .ForMember(dest => dest.AddedById, opt => opt.Ignore())
                .ForMember(dest => dest.AddedDateTime, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedById, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedDateTime, opt => opt.Ignore())
                .ForMember(dest => dest.SoftDeletedById, opt => opt.Ignore())
                .ForMember(dest => dest.SoftDeletedDateTime, opt => opt.Ignore())
                .ForMember(dest => dest.IsSoftDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.AssetType, opt => opt.Ignore())
                .ForMember(dest => dest.Tenant, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));


            CreateMap<Gender, GetGenderResponseDTO>().ReverseMap();
            CreateMap<Gender, GetGenderOptionResponseDTO>().ReverseMap();

            // CreateMap<Asset, GetAllAssetWithDependentEntityDTO>();

            CreateMap<AddTicketTypeRequestDTO, TicketType>().ReverseMap();

            CreateMap<TicketType, GetTicketTypeResponseDTO>()

                // 🔹 Responsible Role
                .ForMember(dest => dest.ResponsibleRoleName,
                    opt => opt.MapFrom(src =>
                        src.ResponsibleRole != null
                            ? src.ResponsibleRole.RoleName
                            : null))

                // 🔹 Approval Role
                .ForMember(dest => dest.ApprovalRoleName,
                    opt => opt.MapFrom(src =>
                        src.ApprovalRole != null
                            ? src.ApprovalRole.RoleName
                            : null))

                // 🔹 Header
                .ForMember(dest => dest.TicketHeaderName,
                    opt => opt.MapFrom(src =>
                        src.TicketHeader != null
                            ? src.TicketHeader.HeaderName
                            : null));


            CreateMap<GetClassificationRequestDTO, TicketClassification>().ReverseMap();
            CreateMap<GetClassificationResponseDTO, TicketClassification>().ReverseMap();
            CreateMap<AddClassificationRequestDTO, TicketClassification>().ReverseMap();
            CreateMap<UpdateClassificationRequestDTO, TicketClassification>().ReverseMap();
            CreateMap<GetClassificationByIdQuery, TicketClassification>().ReverseMap();


            CreateMap<TicketHeader, GetHeaderResponseDTO>().ReverseMap();
            CreateMap<AddHeaderRequestDTO, TicketHeader>().ReverseMap();
            CreateMap<UpdateHeaderRequestDTO, TicketHeader>().ReverseMap();
            CreateMap<GetHeaderRequestDTO, TicketHeader>().ReverseMap();


            CreateMap<axionpro.domain.Entity.WorkflowStage, GetWorkflowStageResponseDTO>().ReverseMap();




            // -------------------------
            // GetAll Mapping (DTO ↔ Entity)
            // -------------------------

            // -------------------------
            // Create Mapping (Insert) - Ignore Identity column
            // -------------------------

            // -------------------------
            // Response Mapping (DTO ↔ Entity)
            CreateMap<AssetType, GetTypeResponseDTO>().ReverseMap();
            // .ForMember(dest => dest.AssetCategoryId, opt => opt.MapFrom(src => src.AssetCategoryId))
            //.ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            //CreateMap<AddTypeRequestDTO, AssetType>()
            //    .ForMember(dest => dest.Id, opt => opt.Ignore())
            //    .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<AddTypeRequestDTO, AssetType>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TenantId, opt => opt.Ignore())
                .ForMember(dest => dest.AddedById, opt => opt.Ignore())
                .ForMember(dest => dest.AddedDateTime, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedById, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedDateTime, opt => opt.Ignore())
                .ForMember(dest => dest.SoftDeletedById, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedDateTime, opt => opt.Ignore())
                .ForMember(dest => dest.IsSoftDeleted, opt => opt.Ignore());

            //CreateMap<GetTypeResponseDTO, AssetType>()
            //    .ForMember(dest => dest.AssetCategoryId, opt => opt.MapFrom(src => src.AssetCategoryId))
            //    .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));


            // -------------------------
            // Update Mapping (DTO ↔ Entity)
            // -------------------------
            CreateMap<UpdateTypeRequestDTO, AssetType>()
                .ForMember(dest => dest.AssetCategoryId, opt => opt.MapFrom(src => src.CategoryId))
                .ForMember(dest => dest.TenantId, opt => opt.Ignore())
                .ForMember(dest => dest.AddedById, opt => opt.Ignore())
                .ForMember(dest => dest.AddedDateTime, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedById, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedDateTime, opt => opt.Ignore())
                .ForMember(dest => dest.SoftDeletedById, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedDateTime, opt => opt.Ignore())
                .ForMember(dest => dest.IsSoftDeleted, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            // -------------------------
            // Delete Mapping (DTO ↔ Entity)
            // -------------------------

            CreateMap<Asset, GetAssetResponseDTO>();
            //.ForMember(dest => dest.WarrantyExpiryDate,
            //           opt => opt.MapFrom(src => src.WarrantyExpiryDate.HasValue
            //                                       ? DateOnly.FromDateTime(src.WarrantyExpiryDate.Value)
            //                                       : default));



            CreateMap<CreateHostUserRequestDTO, HostUser>()
                .ForMember(destination => destination.AddedById, options => options.Ignore())
                .ForMember(destination => destination.AddedDateTime, options => options.Ignore())
                .ForMember(destination => destination.UpdatedById, options => options.Ignore())
                .ForMember(destination => destination.UpdatedDateTime, options => options.Ignore())
                .ForMember(destination => destination.DeletedById, options => options.Ignore())
                .ForMember(destination => destination.DeletedDateTime, options => options.Ignore())
                .ForMember(destination => destination.IsSoftDeleted, options => options.Ignore())
                .ReverseMap();
            CreateMap<CreateHostUserResponseDTO, HostUser>().ReverseMap();
            CreateMap<CreateHostRoleRequestDTO, HostRole>()
                .ForMember(destination => destination.AddedById, options => options.Ignore())
                .ForMember(destination => destination.AddedDateTime, options => options.Ignore())
                .ForMember(destination => destination.UpdatedById, options => options.Ignore())
                .ForMember(destination => destination.UpdatedDateTime, options => options.Ignore())
                .ForMember(destination => destination.DeletedById, options => options.Ignore())
                .ForMember(destination => destination.DeletedDateTime, options => options.Ignore())
                .ForMember(destination => destination.IsSoftDeleted, options => options.Ignore())
                .ReverseMap();
            CreateMap<CreateHostRoleResponseDTO, HostRole>().ReverseMap();
   

            CreateMap<CreateSubscriptionRequestDTO, SubscriptionPlan>()
                .ForMember(destination => destination.AddedById, options => options.Ignore())
                .ForMember(destination => destination.AddedDateTime, options => options.Ignore())
                .ForMember(destination => destination.UpdatedById, options => options.Ignore())
                .ForMember(destination => destination.UpdatedDateTime, options => options.Ignore())
                .ForMember(destination => destination.IsSoftDeleted, options => options.Ignore())
                .ForMember(destination => destination.DeletedById, options => options.Ignore())
                .ForMember(destination => destination.DeletedDateTime, options => options.Ignore())
                .ReverseMap();
            CreateMap<SubscriptionActivePlanDTO, SubscriptionPlan>().ReverseMap(); 
            CreateMap<UpdateSubscriptionRequestDTO, SubscriptionPlan>()
                .ForMember(destination => destination.Id, options => options.Ignore())
                .ForMember(destination => destination.AddedById, options => options.Ignore())
                .ForMember(destination => destination.AddedDateTime, options => options.Ignore())
                .ForMember(destination => destination.UpdatedById, options => options.Ignore())
                .ForMember(destination => destination.UpdatedDateTime, options => options.Ignore())
                .ForMember(destination => destination.IsSoftDeleted, options => options.Ignore())
                .ForMember(destination => destination.DeletedById, options => options.Ignore())
                .ForMember(destination => destination.DeletedDateTime, options => options.Ignore())
                .ReverseMap();


            CreateMap<GetAssetResponseDTO, Asset>().ReverseMap();








            //   CreateMap<AssetType, AssetTypeResponseDTO>().ReverseMap(); // 🔥 Yeh zaroori hai




            CreateMap<CreateStatusRequestDTO, AssetStatus>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TenantId, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.IsSoftDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.AddedById, opt => opt.Ignore())
                .ForMember(dest => dest.AddedDateTime, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedById, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedDateTime, opt => opt.Ignore())
                .ForMember(dest => dest.SoftDeletedById, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedDateTime, opt => opt.Ignore());

            CreateMap<UpdateStatusRequestDTO, AssetStatus>()
                .ForMember(dest => dest.TenantId, opt => opt.Ignore())
                .ForMember(dest => dest.AddedById, opt => opt.Ignore())
                .ForMember(dest => dest.AddedDateTime, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedById, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedDateTime, opt => opt.Ignore())
                .ForMember(dest => dest.SoftDeletedById, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedDateTime, opt => opt.Ignore())
                .ForMember(dest => dest.IsSoftDeleted, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));







            CreateMap<CreateDesignationRequestDTO, Designation>().ReverseMap();
            CreateMap<Designation, GetDesignationRequestDTO>().ReverseMap();
            CreateMap<UpdateDesignationRequestDTO, Designation>().ReverseMap();
            CreateMap<GetDesignationResponseDTO, Designation>().ReverseMap();


            CreateMap<CreateDepartmentRequestDTO, Department>().ReverseMap();
            CreateMap<Department, GetSingleDepartmentResponseDTO>().ReverseMap();
            CreateMap<Department, GetDepartmentRequestDTO>().ReverseMap();
            CreateMap<UpdateDepartmentRequestDTO, Department>().ReverseMap();



            CreateMap<GetParentModuleResponseDTO, domain.Entity.Module>().ReverseMap();

            CreateMap<CreateParentModuleRequestDTO, domain.Entity.Module>().ReverseMap();
            CreateMap<GetCommonModuleResponseDTO, domain.Entity.Module>().ReverseMap();

            CreateMap<CreateCommonModuleRequestDTO, domain.Entity.Module>().ReverseMap();



            CreateMap<domain.Entity.Module, CommonItemDTO>().ReverseMap();


            CreateMap<MainModuleResponseDTO, domain.Entity.Module>();
            CreateMap<domain.Entity.Module, MainModuleResponseDTO>();

            CreateMap<CreateSubModuleRequestDTO, domain.Entity.Module>();
            CreateMap<domain.Entity.Module, CreateSubModuleRequestDTO>();





            CreateMap<domain.Entity.Operation, CreateOperationRequestDTO>()
                .ForMember(dest => dest.ProductOwnerId, opt => opt.MapFrom(src => (long)src.AddedById));



            CreateMap<CreateOperationRequestDTO, domain.Entity.Operation>()
             .ForMember(dest => dest.AddedById, opt => opt.MapFrom(src => (long)src.ProductOwnerId));


            CreateMap<domain.Entity.Operation, CreateOperationRequestDTO>();


            CreateMap<domain.Entity.Operation, GetOperationResponseDTO>();
            CreateMap<GetOperationResponseDTO, domain.Entity.Operation>();


            CreateMap<UpdateOperationRequestDTO, domain.Entity.Operation>();


            CreateMap<domain.Entity.Operation, UpdateOperationRequestDTO>();
            // Create
            CreateMap<CreateInsurancePolicyRequestDTO, InsurancePolicy>();
            CreateMap<PolicyTypeInsuranceMapping, CreatePolicyTypeInsuranceMappingRequetDTO>().ReverseMap();
            CreateMap<GetPolicyTypeInsuranceMappingResponseDTO, PolicyTypeInsuranceMapping>().ReverseMap();
            CreateMap<GetPolicyTypeResponseDTO, PolicyType>().ReverseMap();
            CreateMap<GetPolicyTypeDocumentResponseDTO, PolicyTypeDocument>().ReverseMap();
           

            // Get
            CreateMap<InsurancePolicy, GetInsurancePolicyResponseDTO>()
                .ForMember(d => d.PolicyTypeName,
                    opt => opt.MapFrom(s => s.PolicyType.PolicyName))
                .ForMember(d => d.CountryName,
                    opt => opt.MapFrom(s => s.Country.CountryName));



            CreateMap<CreateClientTypeDTO, ClientType>();
            CreateMap<ClientType, GetClientTypeDTO>();
            CreateMap<UpdateClientTypeDTO, ClientType>();  // ✅ Yeh likhna hoga!
            CreateMap<CreateTravelModeDTO, TravelMode>();
            CreateMap<TravelMode, GetAllTravelModeDTO>();
            CreateMap<UpdateTravelModeDTO, TravelMode>();
            CreateMap<CreateLeaveTypeRequestDTO, LeaveType>();

            CreateMap<GetPolicyLeaveTypeMappingRequestDTO, PolicyLeaveTypeMapping>().ReverseMap();
            CreateMap<GetLeaveTypeWithPolicyMappingResponseDTO, PolicyLeaveTypeMapping>().ReverseMap();


            // CreateMap<UpdatePolicyLeaveTypeMappingRequestDTO, PolicyLeaveTypeMapping>().ReverseMap();               
            CreateMap<LeaveType, GetLeaveTypResponseDTO>();
            CreateMap<UpdateLeaveTypeRequestDTO, LeaveType>();  // ✅ Yeh likhna hoga!


            CreateMap<GetLeaveRuleResponseDTO, LeaveRule>().ReverseMap();
            CreateMap<CreateLeaveRuleDTORequest, LeaveRule>().ReverseMap();
            CreateMap<UpdateLeaveRuleRequestDTO, LeaveRule>().ReverseMap();
            CreateMap<Role, GetSingleRoleResponseDTO>().ReverseMap();
            CreateMap<Role, GetTicketTypeResponseDTO>().ReverseMap();


            // Audit fields are set from trusted request context in the handlers.
            CreateMap<CreateRoleRequestDTO, Role>();
            CreateMap<UpdateRoleRequestDTO, Role>();


            // Role Entity to GetAllRoleDTO Mapping
            // Direct entity to DTO mapping
            //   CreateMap<GetActiveRoleRequestDTO,Role >()

            //    .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));// Example

            CreateMap<Role, GetRoleResponseDTO>()
      .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
      .ForMember(dest => dest.RoleType, opt => opt.MapFrom(src => src.RoleType.ToString()))
      .ForMember(dest => dest.RoleTypeName, opt => opt.MapFrom(src =>
          src.RoleType == 1 ? "Super Admin" :
          src.RoleType == 2 ? "Employee" :
          src.RoleType == 3 ? "Manager" :
          "Unknown"
      ))
      .ForMember(dest => dest.Remark, opt => opt.MapFrom(src => src.Remark)) // ✅ Added
      .ReverseMap()
       .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
        .ForMember(dest => dest.RoleType, opt => opt.MapFrom(src => src.RoleType))
       .ForMember(dest => dest.Remark, opt => opt.MapFrom(src => src.Remark)); // ✅ Added



            CreateMap<Department, GetDepartmentResponseDTO>();
            CreateMap<GetDepartmentResponseDTO, Department>();


            // CreateMap<RoleResponseDTO, Role>().ReverseMap();
            // CreateMap<GetRoleSummaryResponseDTO, Role>();
            //  CreateMap<Role, GetRoleSummaryResponseDTO>().ReverseMap();

            //   CreateMap<TenantCreateResponseDTO, TenantIndustry>();
            CreateMap<TenantSubscriptionPlanResponseDTO, TenantSubscription>().ReverseMap();
            CreateMap<TenantIndustry, TenantIndustryResponseDTO>();



            CreateMap<UserRole, UserRoleDTO>()
                          .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.RoleName)) // Example
                          .ForMember(dest => dest.RoleType, opt => opt.MapFrom(src => src.Role.RoleType)) // Example
                          .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.Role.Id)) // Example
                          .ForMember(dest => dest.UserRoleId, opt => opt.MapFrom(src => src.Id))  
                          .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive == true));

            // CreateRoleDTO to Role Mapping (for creating roles)


            #region 🔹 Bank Mappings

            #endregion

            #region 🔹 Education Mappings
            CreateMap<EmployeeEducation, EmployeeEducationEditableFieldsDTO>().ReverseMap();
            CreateMap<EmployeeEducation, GetEducationResponseDTO>()
       .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
       .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.EmployeeId.ToString()))
       .ForMember(dest => dest.Degree, opt => opt.MapFrom(src => src.Degree))
       .ForMember(dest => dest.InstituteName, opt => opt.MapFrom(src => src.InstituteName))
       .ForMember(dest => dest.Remark, opt => opt.MapFrom(src => src.Remark))
       .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
       .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
       .ForMember(dest => dest.ScoreValue, opt => opt.MapFrom(src => src.ScoreValue))
       .ForMember(dest => dest.GradeDivision, opt => opt.MapFrom(src => src.GradeDivision))
       .ForMember(dest => dest.ScoreType, opt => opt.MapFrom(src => src.ScoreType.ToString()))
       .ForMember(dest => dest.FilePath, opt => opt.MapFrom(src => src.FilePath))
       .ForMember(dest => dest.FileType, opt => opt.MapFrom(src => src.FileType))
       .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.FileName))
       .ForMember(dest => dest.EducationGap, opt => opt.MapFrom(src => src.EducationGap))
       .ForMember(dest => dest.IsEditAllowed, opt => opt.MapFrom(src => src.IsEditAllowed))
       .ForMember(dest => dest.GapYears, opt => opt.MapFrom(src => src.GapYears))
       .ForMember(dest => dest.IsInfoVerified, opt => opt.MapFrom(src => src.IsInfoVerified))
       .ForMember(dest => dest.ReasonOfEducationGap, opt => opt.MapFrom(src => src.ReasonOfEducationGap))
       .ForMember(dest => dest.InfoVerifiedById, opt => opt.MapFrom(src => src.InfoVerifiedById.ToString()))
       .ForMember(dest => dest.HasEducationDocUploded, opt => opt.MapFrom(src => src.HasEducationDocUploded))
       .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));



            CreateMap<EmployeeEducation, GetEducationResponseDTO>().ReverseMap();

            CreateMap<CreateEducationRequestDTO, EmployeeEducation>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.EmployeeId, opt => opt.Ignore())
                .ForMember(dest => dest.AddedById, opt => opt.Ignore())
                .ForMember(dest => dest.AddedDateTime, opt => opt.Ignore())


                // BOOL → BOOL
                .ForMember(dest => dest.EducationGap,
                           opt => opt.MapFrom(src => src.IsEducationGapBeforeDegree))
                // BOOL → BOOL
                .ForMember(dest => dest.GapYears,
                           opt => opt.MapFrom(src => src.GapYears))

                // ScoreValue → string
                .ForMember(dest => dest.ScoreValue,
                           opt => opt.MapFrom(src => src.ScoreValue != null
                                ? src.ScoreValue.ToString()
                                : null))

                // GradeDivision → string
                .ForMember(dest => dest.GradeDivision,
                           opt => opt.MapFrom(src => src.GradeDivision != null
                                ? src.GradeDivision.ToString()
                                : null))

                // ScoreType → string
                .ForMember(dest => dest.ScoreType,
                           opt => opt.MapFrom(src => src.ScoreType != null
                                ? src.ScoreType.ToString()
                                : null));



            #region 🔹 Education Mappings
            CreateMap<CreateEmployeeImageRequestDTO, EmployeeImage>()
     .ForMember(dest => dest.EmployeeId, opt => opt.Ignore())   // ❗ ignore
     .ForMember(dest => dest.FilePath, opt => opt.Ignore())     // ❗ file path later set hoga
     .ForMember(dest => dest.FileName, opt => opt.Ignore())     // ❗ file name later set hoga
     .ForMember(dest => dest.HasImageUploaded, opt => opt.Ignore())
     .ForMember(dest => dest.IsPrimary, opt => opt.Ignore())
     .ForMember(dest => dest.TenantId, opt => opt.Ignore()).ReverseMap();

            #endregion
            CreateMap<UpdateBankReqestDTO, GetBankResponseDTO>().ReverseMap();
            CreateMap<UpdateBankReqestDTO, EmployeeBankDetail>().ReverseMap();

            CreateMap<CreateBankRequestDTO, EmployeeBankDetail>()
           .ForMember(dest => dest.EmployeeId, opt => opt.Ignore())
            .ForMember(dest => dest.AccountType, opt => opt.MapFrom(src => src.AccountType))
           .ForMember(dest => dest.IsPrimaryAccount, opt => opt.MapFrom(src => src.IsPrimaryAccount))
            .ForMember(dest => dest.Upiid, opt => opt.MapFrom(src => src.UPIId));

            CreateMap<CreateContactRequestDTO, EmployeeContact>()
    // ❌ EmployeeId handler me set hoga
    .ForMember(dest => dest.EmployeeId, opt => opt.Ignore())

    // 🔹 Contact Info
    .ForMember(dest => dest.ContactNumber, opt => opt.MapFrom(src => src.ContactNumber))
    .ForMember(dest => dest.AlternateNumber, opt => opt.MapFrom(src => src.AlternateNumber))
    .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
    .ForMember(dest => dest.ContactName,
        opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.ContactName) ? null : src.ContactName))

     // 🔹 Address (string → int safe)
     .ForMember(dest => dest.Relation, opt => opt.MapFrom(src => src.Relation))
     .ForMember(dest => dest.CountryId, opt => opt.MapFrom(src => src.CountryId))
     .ForMember(dest => dest.DistrictId, opt => opt.MapFrom(src => src.DistrictId))
     .ForMember(dest => dest.StateId, opt => opt.MapFrom(src => src.StateId))  

    // 🔹 ContactType (ENUM → INT) ✅
    .ForMember(dest => dest.ContactType,
        opt => opt.MapFrom(src => (int)src.ContactType))

    // 🔹 IsPrimary (nullable safe)
    .ForMember(dest => dest.IsPrimary,
        opt => opt.MapFrom(src => src.IsPrimary))

    // 🔹 Address fields
    .ForMember(dest => dest.HouseNo, opt => opt.MapFrom(src => src.HouseNo))
    .ForMember(dest => dest.LandMark, opt => opt.MapFrom(src => src.LandMark))
    .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.Street))
    .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))

    // 🔹 Meta
    .ForMember(dest => dest.Remark, opt => opt.MapFrom(src => src.Remark))
    .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));



            // 🔁 ReverseMap (DTO → Entity)


            #endregion

            #region 🔹 Base Employee Mappings

            CreateMap<CreateBaseEmployeeRequestDTO, Employee>();
            //.ForMember(dest => dest.CountryId,
            //      opt => opt.MapFrom(src => src.CountryId)).ReverseMap();


            CreateMap<Employee, GetBaseEmployeeResponseDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id)) // already int -> no need tostring
                .ForMember(dest => dest.EmployementCode, opt => opt.MapFrom(src => src.EmployementCode))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.MiddleName, opt => opt.MapFrom(src => src.MiddleName))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.GenderId, opt => opt.MapFrom(src => src.GenderId)) // int
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth))
                .ForMember(dest => dest.DateOfOnBoarding, opt => opt.MapFrom(src => src.DateOfOnBoarding))
                .ForMember(dest => dest.DateOfExit, opt => opt.MapFrom(src => src.DateOfExit))
                .ForMember(dest => dest.DesignationId, opt => opt.MapFrom(src => src.DesignationId))  // int now
                .ForMember(dest => dest.EmployeeTypeId, opt => opt.MapFrom(src => src.EmployeeTypeId)) // int now
                .ForMember(dest => dest.DepartmentId, opt => opt.MapFrom(src => src.DepartmentId)) // int now
                .ForMember(dest => dest.OfficialEmail, opt => opt.MapFrom(src => src.OfficialEmail))
                .ForMember(dest => dest.HasPermanent, opt => opt.MapFrom(src => src.HasPermanent))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(dest => dest.IsEditAllowed, opt => opt.MapFrom(src => src.IsEditAllowed))
                .ForMember(dest => dest.IsInfoVerified, opt => opt.MapFrom(src => src.IsInfoVerified));
              //  .ForMember(dest => dest.CountryId, opt => opt.MapFrom(src => src.CountryId));
            

            #endregion




            //.ForMember(dest => dest.DesignationId, opt => opt.MapFrom(src => src.DesignationId != null ? src.DesignationId.ToString() : null))
            //.ForMember(dest => dest.EmployeeTypeId, opt => opt.MapFrom(src => src.EmployeeTypeId != null ? src.EmployeeTypeId.ToString() : null))
            //.ForMember(dest => dest.DepartmentId, opt => opt.MapFrom(src => src.DepartmentId != null ? src.DepartmentId.ToString() : null))


            CreateMap<GetMinimalEmployeeResponseDTO, CreateBaseEmployeeRequestDTO>().ReverseMap();
            CreateMap<GetMinimalEmployeeResponseDTO, GetBaseEmployeeResponseDTO>().ReverseMap();
            CreateMap<GetMinimalEmployeeResponseDTO, EmployeeInfoEditableFieldsDTO>().ReverseMap();


            #region Dependent
            CreateMap<CreateDependentRequestDTO, EmployeeDependent>()
.ForMember(dest => dest.EmployeeId, opt => opt.Ignore()) // manually assign later
.ForMember(dest => dest.DependentName, opt => opt.MapFrom(src => src.DependentName))
.ForMember(dest => dest.Relation, opt => opt.MapFrom(src => src.Relation))
.ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth))
.ForMember(dest => dest.IsCoveredInPolicy, opt => opt.MapFrom(src => src.IsCoveredInPolicy))
.ForMember(dest => dest.IsMarried, opt => opt.MapFrom(src => src.IsMarried))
.ForMember(dest => dest.Remark, opt => opt.MapFrom(src => src.Remark))
.ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));

            CreateMap<EmployeeDependent, GetDependentResponseDTO>()
    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
    .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.EmployeeId.ToString()))
    .ForMember(dest => dest.DependentName, opt => opt.MapFrom(src => src.DependentName))
    .ForMember(dest => dest.Relation, opt => opt.MapFrom(src => src.Relation))
    .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth))
    .ForMember(dest => dest.IsCoveredInPolicy, opt => opt.MapFrom(src => src.IsCoveredInPolicy))
    .ForMember(dest => dest.IsMarried, opt => opt.MapFrom(src => src.IsMarried))
    .ForMember(dest => dest.Remark, opt => opt.MapFrom(src => src.Remark))
    .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
     .ForMember(dest => dest.InfoVerifiedById, opt => opt.MapFrom(src => src.InfoVerifiedById.HasValue ? src.InfoVerifiedById.Value.ToString() : null))
    .ForMember(dest => dest.IsInfoVerified, opt => opt.MapFrom(src => src.IsInfoVerified))
    .ForMember(dest => dest.InfoVerifiedDateTime, opt => opt.MapFrom(src => src.InfoVerifiedDateTime))
    .ForMember(dest => dest.FilePath, opt => opt.MapFrom(src => src.FilePath))
    .ForMember(dest => dest.HasProofUploaded, opt => opt.MapFrom(src => src.HasProofUploaded));


            #endregion

            #region 🔹 Contact Mappings
            CreateMap<EmployeeContact, EmployeeContactEditableFieldDTO>().ReverseMap();
            CreateMap<GetContactResponseDTO, EmployeeContact>().ReverseMap();
            #endregion

            #region 🔹 Personal Detail Mappings
            //     CreateMap<EmployeePersonalDetail, CreateIdentityRequestDTO>()
            //.ForMember(dest => dest.UserEmployeeId, opt => opt.Ignore())
            //.ForMember(dest => dest.EmployeeId, opt => opt.Ignore())
            //.ForMember(dest => dest.AadhaarDocFile, opt => opt.Ignore())
            //.ForMember(dest => dest.PanDocFile, opt => opt.Ignore())
            //.ForMember(dest => dest.PassportDocFile, opt => opt.Ignore());

            //CreateMap<CreateIdentityRequestDTO, IdentityCategory>()
            //    .ForMember(dest => dest.Id, opt => opt.Ignore())  // Primary key
            //    .ForMember(dest => dest.AddedById, opt => opt.Ignore())
            //    .ForMember(dest => dest.AddedDateTime, opt => opt.Ignore())
            //    .ForMember(dest => dest.UpdatedById, opt => opt.Ignore())
            //     .ForMember(dest => dest.EmployeeId, opt => opt.Ignore())
            //     .ForMember(dest => dest.SoftDeletedById, opt => opt.Ignore())


            //    .ForMember(dest => dest.IsActive, opt => opt.Ignore())


            //    .ForMember(dest => dest.HasEPFAccount, opt => opt.MapFrom(src => src.HasEPFAccount))
            //    .ForMember(dest => dest.UANNumber, opt => opt.MapFrom(src => src.UANNumber))
            //    .ForMember(dest => dest.AadhaarNumber, opt => opt.MapFrom(src => src.AadhaarNumber))
            //    .ForMember(dest => dest.PanNumber, opt => opt.MapFrom(src => src.PanNumber))
            //    .ForMember(dest => dest.PassportNumber, opt => opt.MapFrom(src => src.PassportNumber))
            //    .ForMember(dest => dest.DrivingLicenseNumber, opt => opt.MapFrom(src => src.DrivingLicenseNumber))
            //    .ForMember(dest => dest.VoterId, opt => opt.MapFrom(src => src.VoterId))
            //    .ForMember(dest => dest.BloodGroup, opt => opt.MapFrom(src => src.BloodGroup))
            //    .ForMember(dest => dest.MaritalStatus, opt => opt.MapFrom(src => src.MaritalStatus))
            //    .ForMember(dest => dest.Nationality, opt => opt.MapFrom(src => src.Nationality))
            //    .ForMember(dest => dest.EmergencyContactName, opt => opt.MapFrom(src => src.EmergencyContactName))
            //    .ForMember(dest => dest.EmergencyContactRelation, opt => opt.MapFrom(src => src.EmergencyContactRelation))
            //    .ForMember(dest => dest.EmergencyContactNumber, opt => opt.MapFrom(src => src.EmergencyContactNumber));

            #endregion

            #region 🔹 Experience Mappings

            CreateMap<CreateExperienceRequestDTO, EmployeeExperience>()
                 .ForMember(dest => dest.Id, opt => opt.Ignore())
                 .ForMember(dest => dest.EmployeeId, opt => opt.Ignore()) // 🔥 set manually (decoded)
                 .ForMember(dest => dest.AddedById, opt => opt.Ignore())
                 .ForMember(dest => dest.AddedDateTime, opt => opt.Ignore())
                 .ForMember(dest => dest.UpdatedById, opt => opt.Ignore())
                 .ForMember(dest => dest.UpdatedDateTime, opt => opt.Ignore())
                 .ForMember(dest => dest.Employee, opt => opt.Ignore()) // navigation
                 .ForMember(dest => dest.EmployeeExperienceDocument, opt => opt.Ignore()); // handle manually


            CreateMap<CreateExperienceDocumentDTO, EmployeeExperienceDocument>()
                  .ForMember(dest => dest.Id, opt => opt.Ignore())
                  .ForMember(dest => dest.EmployeeExperienceId, opt => opt.Ignore()) // 🔥 set by EF relation
                  .ForMember(dest => dest.AddedById, opt => opt.Ignore())
                  .ForMember(dest => dest.AddedDateTime, opt => opt.Ignore())
                  .ForMember(dest => dest.UpdatedById, opt => opt.Ignore())
                  .ForMember(dest => dest.UpdatedDateTime, opt => opt.Ignore())
                  .ForMember(dest => dest.EmployeeExperience, opt => opt.Ignore()); // navigation
 
            #endregion

            #region 🔹 Login / Info Mappings

            CreateMap<GetEmployeeLoginInfoResponseDTO, LoginResponseDTO>()
                .ForMember(dest => dest.EmployeeInfo, opt => opt.MapFrom(src => src));
            CreateMap<GetMinimalEmployeeResponseDTO, GetEmployeeLoginInfoResponseDTO>()
                .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.Id))  // now int
                .ForMember(dest => dest.EmployeeFullName, opt => opt.MapFrom(src =>
                    $"{src.FirstName} {(string.IsNullOrWhiteSpace(src.MiddleName) ? "" : src.MiddleName + " ")}{src.LastName}".Trim()))
                .ForMember(dest => dest.EmployeeTypeId, opt => opt.MapFrom(src => src.EmployeeTypeId)) // now int?
                .ForMember(dest => dest.DepartmentId, opt => opt.MapFrom(src => src.DepartmentId))
                .ForMember(dest => dest.DesignationId, opt => opt.MapFrom(src => src.DesignationId))

                .ForMember(dest => dest.OfficialEmail, opt => opt.MapFrom(src => src.OfficialEmail));

            #endregion

            // Agar reverse mapping chahiye toh, isse bhi add kar sakte hain
            CreateMap<GetReportingTypeResponseDTO, ReportingType>().ReverseMap();


            CreateMap<OrganizationHolidayCalendar, OrganizationHolidayCalendarDTO>();
            CreateMap<OrganizationHolidayCalendarDTO, OrganizationHolidayCalendar>();

            // `Employee.OfficialEmail` ➝ `EmployeeInfoDTO.OfficialEmail`


            // Add mapping for List<Role> to List<GetAllRoleDTO>
            // CreateMap<List<Role>, List<GetAllRoleDTO>>();  // Add this line

            // CreateMap<List<Role>, List<GetAllRoleDTO>>()
            //.ForMember(dest => dest, opt => opt.MapFrom(src => src.Select(x => new GetAllRoleDTO { /* Manually map properties here */ }).ToList()));


            // Mapping Employee entity to LoginEmployeeInfoDTO
            //CreateMap<Employee, EmployeeInfoDTO>()
            //    .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.Id)) // Map EmployeeId
            //    .ForMember(dest => dest.LoginId, opt => opt.MapFrom((src, dest, _, context) => context.Itaxionpro["LoginId"] as string)) // Map LoginId from context
            //                                                                                                                        //  .ForMember(dest => dest.EmployeeFirstName, opt => opt.MapFrom(src => src.FirstName)) // Map UserName
            //                                                                                                                        //.ForMember(dest => dest.EmployeeMiddleName, opt => opt.MapFrom(src => src.MiddleName)) // Map UserName
            //                                                                                                                        // .ForMember(dest => dest.EmployeeLastName, opt => opt.MapFrom(src => src.LastName)) // Map UserName
            //    .ForMember(dest => dest.EmployeeFullName, opt => opt.MapFrom(src => ((src.FirstName) + src.MiddleName) + src.LastName));
            //    // Map UserName
            //.ForMember(dest => dest.EmployeeTypeId, opt => opt.MapFrom(src => src.EmployeeTypeId.ToString())) // Map EmployeeTypeId
            // .ForMember(dest => dest.EmployeeType, opt => opt.MapFrom(src => src.EmployementType.ToString())) // Map EmployeeTypeId
            // .ForMember(dest => dest.EmployeeType, opt => opt.MapFrom(src => src.EmployeeType.TypeName)) // Map EmployeeType Name
            //.ForMember(dest => dest.EmployeeAssignedRoles, opt => opt.MapFrom(src => src.UserRoles.Select(ur => new RoleInfoDTO
            //{
            //    Id = ur.RoleId.GetValueOrDefault(),  // ✅ Fix: Converts nullable int? to int (Defaults to 0 if null)
            //    RoleName = ur.Role.RoleName,
            //    Description = ur.Remark
            //})));

            //CreateMap<EmailTemplate, EmailTemplateDTO>();
            CreateMap<EmailTemplate, EmailTemplateDTO>().ReverseMap();

            CreateMap<Tenant, DTOs.Registration.TenantCreateRequestDTO>().ReverseMap();
            CreateMap<LoginCredential, GetMinimalEmployeeResponseDTO>().ReverseMap();

            CreateMap<Country, GetCountryOptionResponseDTO>().ReverseMap();
            CreateMap<State, GetStateOptionResponseDTO>().ReverseMap();
            CreateMap<District, GetDistrictOptionResponseDTO>().ReverseMap();


            CreateMap<Tenant, DTOs.Tenant.TenantRequestDTO>().ReverseMap();
            CreateMap<TenantResponseDTO, Tenant>().ReverseMap();
            CreateMap<Tenant, HostTenantResponseDTO>()
                .ForMember(destination => destination.Id, options => options.Ignore())
                .ForMember(destination => destination.EmployeeId, options => options.Ignore());

            CreateMap<SubscriptionPlan, SubscriptionPlanResponseDTO>()
               .ForMember(dest => dest.Modules, opt => opt.MapFrom(src => src.PlanModuleMapping
               .Where(pmm => pmm.Module != null)
                .Select(pmm => new ModuleResponseDTO
                {
                    ModuleId = pmm.Module.Id,
                    ModuleName = pmm.Module.ModuleName,
                    ParentModuleId = pmm.Module.ParentModuleId
                }).ToList()));

            CreateMap<ModuleOperationMapping, TenantEnabledOperation>()
               .ForMember(dest => dest.Id, opt => opt.Ignore()) // IMPORTANT
.ForMember(dest => dest.ModuleId,
          opt => opt.MapFrom(src => src.ModuleId))

.ForMember(dest => dest.OperationId,
          opt => opt.MapFrom(src => src.OperationId))

.ForMember(dest => dest.IsEnabled,
          opt => opt.MapFrom(src => src.IsActive ?? true))

.ForMember(dest => dest.IsOperationUsed,
          opt => opt.MapFrom(src => true))

.ForMember(dest => dest.AddedById,
          opt => opt.MapFrom(src => src.AddedById))

.ForMember(dest => dest.AddedDateTime,
          opt => opt.MapFrom(src => src.AddedDateTime))

// ignore navigation properties
.ForMember(dest => dest.Module, opt => opt.Ignore())
.ForMember(dest => dest.Operation, opt => opt.Ignore())
.ForMember(dest => dest.Tenant, opt => opt.Ignore());

            CreateMap<TenantEnabledOperationRequestDTO, ModuleOperationMapping>();

            CreateMap<TenantEnabledOperation, TenantEnabledOperationRequestDTO>();
            CreateMap<TenantEnabledOperationRequestDTO, TenantEnabledOperation>();







            CreateMap<OperationResponseDTO, TenantEnabledOperation>()
               .ForMember(dest => dest.IsEnabled, opt => opt.MapFrom(src => true))
              .ForMember(dest => dest.OperationId, opt => opt.MapFrom(src => src.OperationId)).ForAllMembers(opt => opt.Ignore());

            CreateMap<TenantSubscriptionPlanRequestDTO, TenantSubscription>().ReverseMap();
            CreateMap<TenantSubscriptionPlanResponseDTO, TenantSubscription>().ReverseMap();


            CreateMap<CreateModuleOperationRequestDTO, ModuleOperationMapping>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PageUrl, opt => opt.MapFrom(src => src.PageURL))
                .ForMember(dest => dest.IconUrl, opt => opt.MapFrom(src => src.IconURL))
                .ForMember(dest => dest.AddedById, opt => opt.Ignore())
                .ForMember(dest => dest.AddedDateTime, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedById, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedDateTime, opt => opt.Ignore())
                .ForMember(dest => dest.Module, opt => opt.Ignore())
                .ForMember(dest => dest.Operation, opt => opt.Ignore())
                .ForMember(dest => dest.DataViewStructure, opt => opt.Ignore())
                .ForMember(dest => dest.PageType, opt => opt.Ignore());

            CreateMap<UpdateModuleOperationMappingByProductOwnerRequestDTO, ModuleOperationMapping>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PageUrl, opt => opt.MapFrom(src => src.PageURL))
                .ForMember(dest => dest.IconUrl, opt => opt.MapFrom(src => src.IconURL))
                .ForMember(dest => dest.AddedById, opt => opt.Ignore())
                .ForMember(dest => dest.AddedDateTime, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedById, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedDateTime, opt => opt.Ignore())
                .ForMember(dest => dest.Module, opt => opt.Ignore())
                .ForMember(dest => dest.Operation, opt => opt.Ignore())
                .ForMember(dest => dest.DataViewStructure, opt => opt.Ignore())
                .ForMember(dest => dest.PageType, opt => opt.Ignore());

            CreateMap<ModuleOperationMapping, ModuleOperationMappingByProductOwnerResponseDTO>()
                .ForMember(dest => dest.PageURL, opt => opt.MapFrom(src => src.PageUrl))
                .ForMember(dest => dest.IconURL, opt => opt.MapFrom(src => src.IconUrl))
                .ForMember(dest => dest.ModuleName, opt => opt.MapFrom(src => src.Module != null ? src.Module.ModuleName : null))
                .ForMember(dest => dest.OperationName, opt => opt.MapFrom(src => src.Operation.OperationName))
                .ForMember(dest => dest.DataViewStructureDisplayOn, opt => opt.MapFrom(src => src.DataViewStructure != null ? src.DataViewStructure.DisplayOn : null))
                .ForMember(dest => dest.PageTypeName, opt => opt.MapFrom(src => src.PageType != null ? src.PageType.PageTypeName : null));





            CreateMap<CandidateRequestDTO, Candidate>()
         .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
         .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
         .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
         .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
         .ForMember(dest => dest.Pan, opt => opt.MapFrom(src => src.Pan))
         .ForMember(dest => dest.Aadhaar, opt => opt.MapFrom(src => src.Aadhaar))
         .ForMember(dest => dest.CandidateReferenceCode, opt => opt.MapFrom(src => src.CandidateReferenceCode))
         .ForMember(dest => dest.ResumeUrl, opt => opt.MapFrom(src => src.ResumeUrl))
         .ForMember(dest => dest.ExperienceYears, opt => opt.MapFrom(src => src.ExperienceYears))
         .ForMember(dest => dest.CurrentLocation, opt => opt.MapFrom(src => src.CurrentLocation))
         .ForMember(dest => dest.ExpectedSalary, opt => opt.MapFrom(src => src.ExpectedSalary))
         .ForMember(dest => dest.CurrentCompany, opt => opt.MapFrom(src => src.CurrentCompany))
         .ForMember(dest => dest.NoticePeriod, opt => opt.MapFrom(src => src.NoticePeriod))
         .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth))
         .ForMember(dest => dest.AppliedDate, opt => opt.MapFrom(src => src.AppliedDate))
         .ForMember(dest => dest.SkillSet, static opt => opt.MapFrom(static src => CleanSkillSet(src.SkillSet)))
         .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
         .ForMember(dest => dest.ActionStatus, opt => opt.MapFrom(src => src.ActionStatus))
         .ForMember(dest => dest.Education, opt => opt.MapFrom(src => src.Education))
         .ForMember(dest => dest.IsFresher, opt => opt.MapFrom(src => src.IsFresher))
         //.ForMember(dest => dest.Resume, opt =>opt.MapFrom(src => src.ResumeUpload != null ? Convert.ToBase64String(src.ResumeUpload) : null))
         .ForMember(dest => dest.ResumePath, opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.ResumeUpload) ? Convert.FromBase64String(src.ResumeUpload) : Array.Empty<byte>()))

         .ForMember(dest => dest.IsBlacklisted, opt => opt.MapFrom(src => false)) // Default value
         .ForMember(dest => dest.LastUpdatedDateTime, opt => opt.MapFrom(src => DateTime.UtcNow));

        }


    }
}

