using AutoMapper;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.DTOs.Category;
using axionpro.application.DTOs.Client;
using axionpro.application.DTOs.Department;
using axionpro.application.DTOs.Designation;
using axionpro.application.DTOs.EmailTemplate;
using axionpro.application.DTOs.Employee;
using axionpro.application.DTOs.Employee.AccessControlReadOnlyType;
using axionpro.application.DTOs.Employee.AccessResponse;
using axionpro.application.DTOs.Gender;
using axionpro.application.DTOs.Leave;
using axionpro.application.DTOs.Leave.LeaveRule;
using axionpro.application.DTOs.Module;
using axionpro.application.DTOs.Module.NewFolder;
using axionpro.application.DTOs.Operation;
using axionpro.application.DTOs.OrganizationHolidayCalendar;
using axionpro.application.DTOs.Registration;
using axionpro.application.DTOs.Role;
using axionpro.application.DTOs.SubscriptionModule;
using axionpro.application.DTOs.Tenant;
using axionpro.application.DTOs.TenantIndustry;
using axionpro.application.DTOs.Transport;
using axionpro.application.DTOs.UserLogin;
using axionpro.application.DTOs.UserRole;
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
using axionpro.application.DTOS.Employee.Sensitive;
using axionpro.application.DTOS.Employee.Type;
using axionpro.application.DTOS.Gender;
using axionpro.application.DTOS.Location;
using axionpro.application.DTOS.Module.CommonModule;
using axionpro.application.DTOS.Module.ParentModule;
using axionpro.application.DTOS.Module.SubModule;
//using axionpro.application.DTOS.Module.ParentModule;
using axionpro.application.DTOS.TicketDTO.Classification;
using axionpro.application.DTOS.TicketDTO.Header;
using axionpro.application.DTOS.TicketDTO.TicketType;
using axionpro.application.Features.TicketFeatures.Classification.Commands;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.domain.Entity;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;


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


        public MappingProfile()
        {


            CreateMap<AddAssetRequestDTO, Asset>();

            CreateMap<AssetCategory, GetCategoryResponseDTO>().ReverseMap();
            CreateMap<AssetStatus, GetStatusResponseDTO>().ReverseMap();
            CreateMap<AssetCategory, GetCategoryResponseDTO>().ReverseMap();


            CreateMap<Gender, GetGenderResponseDTO>().ReverseMap();
            CreateMap<Gender, GetGenderOptionResponseDTO>().ReverseMap();

            // CreateMap<Asset, GetAllAssetWithDependentEntityDTO>();

            CreateMap<TicketType, GetTicketTypeResponseDTO>().ReverseMap();
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

            CreateMap<AssetType, AddTypeRequestDTO>().ReverseMap();

            //CreateMap<GetTypeResponseDTO, AssetType>()
            //    .ForMember(dest => dest.AssetCategoryId, opt => opt.MapFrom(src => src.AssetCategoryId))
            //    .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));


            // -------------------------
            // Update Mapping (DTO ↔ Entity)
            // -------------------------
            CreateMap<UpdateTypeRequestDTO, AssetType>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null)); // Id included for update

            CreateMap<AssetType, UpdateTypeRequestDTO>()
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.Id))
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            // -------------------------
            // Delete Mapping (DTO ↔ Entity)
            // -------------------------
            CreateMap<DeleteTypeRequestDTO, AssetType>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null)); // Id required for delete

            CreateMap<AssetType, DeleteTypeRequestDTO>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Asset, GetAssetResponseDTO>();
            //.ForMember(dest => dest.WarrantyExpiryDate,
            //           opt => opt.MapFrom(src => src.WarrantyExpiryDate.HasValue
            //                                       ? DateOnly.FromDateTime(src.WarrantyExpiryDate.Value)
            //                                       : default));

            CreateMap<Asset, GetAssetRequestDTO>().ReverseMap();


            CreateMap<SubscriptionPlanResponseDTO, SubscriptionPlan>().ReverseMap();



            CreateMap<GetAssetResponseDTO, Asset>().ReverseMap();


            CreateMap<UpdateAssetRequestDTO, Asset>().ReverseMap();

            CreateMap<DeleteAssetReqestDTO, Asset>().ReverseMap();

            CreateMap<AssetStatus, GetStatusRequestDTO>().ReverseMap();

            CreateMap<GetStatusResponseDTO, AssetStatus>().ReverseMap();
            CreateMap<GetStatusResponseDTO, UpdateStatusRequestDTO>().ReverseMap();





            //   CreateMap<AssetType, AssetTypeResponseDTO>().ReverseMap(); // 🔥 Yeh zaroori hai




            CreateMap<AssetStatus, CreateStatusRequestDTO>().ReverseMap();
            CreateMap<AssetStatus, UpdateStatusRequestDTO>().ReverseMap();
            CreateMap<UpdateStatusRequestDTO, AssetStatus>().ReverseMap();


            CreateMap<AssetStatus, DeleteStatusReqestDTO>().ReverseMap();







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
            CreateMap<domain.Entity.Module, GetParentModuleRequestDTO>().ReverseMap();






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


            CreateMap<CreateRoleRequestDTO, Role>()
                  .ForMember(dest => dest.AddedById, opt => opt.MapFrom(src => src.UserEmployeeId)); // Example

            CreateMap<UpdateRoleRequestDTO, Role>()
                .ForMember(dest => dest.UpdatedById, opt => opt.MapFrom(src => src.UserEmployeeId));


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
                          .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive == true));

            // CreateRoleDTO to Role Mapping (for creating roles)


            #region 🔹 Bank Mappings

            #endregion

            #region 🔹 Education Mappings
            CreateMap<EmployeeEducation, EmployeeEducationEditableFieldsDTO>().ReverseMap();
            CreateMap<EmployeeEducation, GetEducationResponseDTO>()
       .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
       .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.EmployeeId.ToString()))
       .ForMember(dest => dest.Degree, opt => opt.MapFrom(src => src.Degree))
       .ForMember(dest => dest.InstituteName, opt => opt.MapFrom(src => src.InstituteName))
       .ForMember(dest => dest.Remark, opt => opt.MapFrom(src => src.Remark))
       .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
       .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
       .ForMember(dest => dest.ScoreValue, opt => opt.MapFrom(src => src.ScoreValue))
       .ForMember(dest => dest.GradeDivision, opt => opt.MapFrom(src => src.GradeDivision))
       .ForMember(dest => dest.ScoreType, opt => opt.MapFrom(src => src.ScoreType.ToString()))
       .ForMember(dest => dest.FilecPath, opt => opt.MapFrom(src => src.FilePath))
       .ForMember(dest => dest.FileType, opt => opt.MapFrom(src => src.FileType.ToString()))
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

            CreateMap<CreateBankRequestDTO, EmployeeBankDetail>()
           .ForMember(dest => dest.EmployeeId, opt => opt.Ignore())
            .ForMember(dest => dest.AccountType, opt => opt.MapFrom(src => src.AccountType))
           .ForMember(dest => dest.IsPrimaryAccount, opt => opt.MapFrom(src => src.IsPrimaryAccount))
            .ForMember(dest => dest.UPIId, opt => opt.MapFrom(src => src.UPIId));

            CreateMap<CreateContactRequestDTO, EmployeeContact>()
         .ForMember(dest => dest.EmployeeId, opt => opt.Ignore())

         // 🔹 Contact Info
         .ForMember(dest => dest.ContactNumber, opt => opt.MapFrom(src => src.ContactNumber))
         .ForMember(dest => dest.AlternateNumber, opt => opt.MapFrom(src => src.AlternateNumber))
         .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
         //.ForMember(dest => dest.IsPrimary, opt => opt.MapFrom(src => src.IsPrimary))
         //.ForMember(dest => dest.IsPrimary, opt => opt.PreCondition(src => src.IsPrimary.HasValue))

         // 🔹 Address Info (string → int safely)
         .ForMember(dest => dest.CountryId, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.CountryId) ? 0 : int.Parse(src.CountryId)))
         .ForMember(dest => dest.StateId, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.StateId) ? 0 : int.Parse(src.StateId)))
         .ForMember(dest => dest.DistrictId, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.DistrictId) ? 0 : int.Parse(src.DistrictId)))
         .ForMember(dest => dest.ContactType, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.ContactType) ? 0 : src.ContactType.Equals("Personal", StringComparison.OrdinalIgnoreCase) ? 1 :
              src.ContactType.Equals("Official", StringComparison.OrdinalIgnoreCase) ? 2 : 0))


         // 🔹 Other address fields
         .ForMember(dest => dest.HouseNo, opt => opt.MapFrom(src => src.HouseNo))
         .ForMember(dest => dest.LandMark, opt => opt.MapFrom(src => src.LandMark))
         .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.Street))
         .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))

         // 🔹 Optional/Metadata
         .ForMember(dest => dest.Remark, opt => opt.MapFrom(src => src.Remark))
         .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));



            // 🔁 ReverseMap (DTO → Entity)


            #endregion

            #region 🔹 Base Employee Mappings

            CreateMap<CreateBaseEmployeeRequestDTO, Employee>()
             .ForMember(dest => dest.NationalityCountryId,
                   opt => opt.MapFrom(src => src.NationalityCountryId)).ReverseMap();


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
                .ForMember(dest => dest.IsInfoVerified, opt => opt.MapFrom(src => src.IsInfoVerified))
                .ReverseMap();

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
    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
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
            CreateMap<EmployeePersonalDetail, CreateIdentityRequestDTO>()
       .ForMember(dest => dest.UserEmployeeId, opt => opt.Ignore())
       .ForMember(dest => dest.EmployeeId, opt => opt.Ignore())
       .ForMember(dest => dest.AadhaarDocFile, opt => opt.Ignore())
       .ForMember(dest => dest.PanDocFile, opt => opt.Ignore())
       .ForMember(dest => dest.PassportDocFile, opt => opt.Ignore());

            CreateMap<CreateIdentityRequestDTO, EmployeePersonalDetail>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())  // Primary key
                .ForMember(dest => dest.AddedById, opt => opt.Ignore())
                .ForMember(dest => dest.AddedDateTime, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedById, opt => opt.Ignore())
                 .ForMember(dest => dest.EmployeeId, opt => opt.Ignore())
                 .ForMember(dest => dest.SoftDeletedById, opt => opt.Ignore())


                .ForMember(dest => dest.IsActive, opt => opt.Ignore())


                .ForMember(dest => dest.HasEPFAccount, opt => opt.MapFrom(src => src.HasEPFAccount))
                .ForMember(dest => dest.UANNumber, opt => opt.MapFrom(src => src.UANNumber))
                .ForMember(dest => dest.AadhaarNumber, opt => opt.MapFrom(src => src.AadhaarNumber))
                .ForMember(dest => dest.PanNumber, opt => opt.MapFrom(src => src.PanNumber))
                .ForMember(dest => dest.PassportNumber, opt => opt.MapFrom(src => src.PassportNumber))
                .ForMember(dest => dest.DrivingLicenseNumber, opt => opt.MapFrom(src => src.DrivingLicenseNumber))
                .ForMember(dest => dest.VoterId, opt => opt.MapFrom(src => src.VoterId))
                .ForMember(dest => dest.BloodGroup, opt => opt.MapFrom(src => src.BloodGroup))
                .ForMember(dest => dest.MaritalStatus, opt => opt.MapFrom(src => src.MaritalStatus))
                .ForMember(dest => dest.Nationality, opt => opt.MapFrom(src => src.Nationality))
                .ForMember(dest => dest.EmergencyContactName, opt => opt.MapFrom(src => src.EmergencyContactName))
                .ForMember(dest => dest.EmergencyContactRelation, opt => opt.MapFrom(src => src.EmergencyContactRelation))
                .ForMember(dest => dest.EmergencyContactNumber, opt => opt.MapFrom(src => src.EmergencyContactNumber));

            #endregion

            #region 🔹 Experience Mappings
            CreateMap<EmployeeExperience, EmployeeExperienceEditableFieldsDTO>().ReverseMap();
            CreateMap<EmployeeExperience, GetExperienceResponseDTO>().ReverseMap();
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

            CreateMap<SubscriptionPlan, SubscriptionPlanResponseDTO>()
               .ForMember(dest => dest.Modules, opt => opt.MapFrom(src => src.PlanModuleMappings
               .Where(pmm => pmm.Module != null)
                .Select(pmm => new ModuleResponseDTO
                {
                    ModuleId = pmm.Module.Id,
                    ModuleName = pmm.Module.ModuleName,
                    ParentModuleId = pmm.Module.ParentModuleId
                }).ToList()));

            CreateMap<ModuleOperationMapping, TenantEnabledOperationRequestDTO>();
            CreateMap<TenantEnabledOperationRequestDTO, ModuleOperationMapping>();

            CreateMap<TenantEnabledOperation, TenantEnabledOperationRequestDTO>();
            CreateMap<TenantEnabledOperationRequestDTO, TenantEnabledOperation>();







            CreateMap<OperationResponseDTO, TenantEnabledOperation>()
               .ForMember(dest => dest.IsEnabled, opt => opt.MapFrom(src => true))
              .ForMember(dest => dest.OperationId, opt => opt.MapFrom(src => src.OperationId)).ForAllMembers(opt => opt.Ignore());

            CreateMap<TenantSubscriptionPlanRequestDTO, TenantSubscription>().ReverseMap();
            CreateMap<TenantSubscriptionPlanResponseDTO, TenantSubscription>().ReverseMap();


            CreateMap<UpdateModuleOperationMappingByProductOwnerRequestDTO, ModuleOperationMapping>()
         .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ModuleOperationMappingId))
         .ForMember(dest => dest.PageUrl, opt => opt.MapFrom(src => src.PageURL))
         .ForMember(dest => dest.IconUrl, opt => opt.MapFrom(src => src.IconURL))
         .ForMember(dest => dest.IsCommonItem, opt => opt.MapFrom(src => src.IsCommonItem))
         .ForMember(dest => dest.IsOperational, opt => opt.MapFrom(src => src.IsOperational))
         .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority))
         .ForMember(dest => dest.Remark, opt => opt.MapFrom(src => src.Remark))
         .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
         .ForMember(dest => dest.UpdatedById, opt => opt.MapFrom(src => src.ProductOwnerId));

            CreateMap<ModuleOperationMapping, ModuleOperationMappingByProductOwnerResponseDTO>()
    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
    .ForMember(dest => dest.ModuleId, opt => opt.MapFrom(src => src.ModuleId))
    .ForMember(dest => dest.OperationIds, opt => opt.MapFrom(src => new List<int> { src.OperationId }))

    .ForMember(dest => dest.PageURL, opt => opt.MapFrom(src => src.PageUrl))
    .ForMember(dest => dest.IconURL, opt => opt.MapFrom(src => src.IconUrl))
    .ForMember(dest => dest.IsCommonItem, opt => opt.MapFrom(src => src.IsCommonItem ?? false))
    .ForMember(dest => dest.IsOperational, opt => opt.MapFrom(src => src.IsOperational ?? false))
    .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority ?? 0))
    .ForMember(dest => dest.Remark, opt => opt.MapFrom(src => src.Remark))
    .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
    .ForMember(dest => dest.AddedById, opt => opt.MapFrom(src => src.AddedById))
    .ForMember(dest => dest.AddedDateTime, opt => opt.MapFrom(src => src.AddedDateTime))
    .ForMember(dest => dest.UpdatedById, opt => opt.MapFrom(src => src.UpdatedById))
    .ForMember(dest => dest.UpdatedDateTime, opt => opt.MapFrom(src => src.UpdatedDateTime));





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

