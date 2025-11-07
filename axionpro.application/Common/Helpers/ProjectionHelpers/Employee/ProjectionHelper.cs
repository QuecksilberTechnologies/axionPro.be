using axionpro.application.Common.Helpers.Converters;
using axionpro.application.DTOs.Department;
using axionpro.application.DTOs.Designation;
using axionpro.application.DTOs.Employee.AccessResponse;
using axionpro.application.DTOs.Role;
using axionpro.application.DTOS.Employee.Bank;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.DTOS.Employee.Contact;
using axionpro.application.DTOS.Employee.Dependent;
using axionpro.application.DTOS.Employee.Education;
using axionpro.application.DTOS.Employee.Experience;
using axionpro.application.DTOS.Employee.Sensitive;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.domain.Entity;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Common.Helpers.ProjectionHelpers.Employee
{
    public static class ProjectionHelper
    {
        // Manual projection for bank entity -> DTO with encrypted Id

        private static string SafeEncrypt(string? value, IEncryptionService encryptionService, string tenantKey)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            return encryptionService.Encrypt(value, tenantKey);
        }
        private static string EnsureEncrypted(string? value, IEncryptionService encryptionService, string tenantKey)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            // If already encrypted (heuristic check)
            if (value.Length > 10 && (value.Contains("=") || value.Contains("+") || value.Contains("/")))
                return value; // Return as-is

            return encryptionService.Encrypt(value, tenantKey);
        }


        public static List<GetBankResponseDTO> ToGetBankResponseDTOs(
                      List<GetBankResponseDTO> entities,
                      IEncryptionService encryptionService,
                       string tenantKey,
                       string empId)
        {
            if (entities == null || entities.Count == 0)
                return new List<GetBankResponseDTO>();

            return entities.Select(e => new GetBankResponseDTO
            {
                Id = SafeEncrypt(e.Id?.ToString(), encryptionService, tenantKey),
                EmployeeId = empId,
                BankName = e.BankName ?? string.Empty,
                AccountNumber = e.AccountNumber ?? string.Empty,
                IFSCCode = e.IFSCCode ?? string.Empty,
                BranchName = e.BranchName ?? string.Empty,
                IsPrimary = e.IsPrimaryAccount
            }).ToList();
        }

        public static List<GetContactResponseDTO> ToGetContactResponseDTOs(
     List<GetContactResponseDTO> entities,
     IEncryptionService encryptionService,
     string tenantKey)
        {
            if (entities == null || entities.Count == 0)
                return new List<GetContactResponseDTO>();

            return entities.Select(e => new GetContactResponseDTO
            {
                Id = SafeEncrypt(e.Id, encryptionService, tenantKey),
                UserEmployeeId = SafeEncrypt(e.UserEmployeeId, encryptionService, tenantKey),
                ContactLocation = e.ContactLocation ?? string.Empty,
                LocalAddress = e.LocalAddress ?? string.Empty,
                LandMark = e.LandMark ?? string.Empty,
                CountryId = e.CountryId ?? 0,
                Country = e.Country ?? string.Empty,
                StateId = e.StateId ?? 0,
                State = e.State ?? string.Empty,
                DistrictId = e.DistrictId ?? 0,
                District = e.District ?? string.Empty,
                IsPrimary = e.IsPrimary,
                IsActive = e.IsActive,
                IsInfoVerified = e.IsInfoVerified,
                IsEditAllowed = e.IsEditAllowed,
                ContactType = e.ContactType,
                PermanentAddress = e.PermanentAddress ?? string.Empty,
                ContactNumber = e.ContactNumber ?? string.Empty,
                AlternateNumber = e.AlternateNumber ?? string.Empty,
                Email = e.Email ?? string.Empty,
                Remark = e.Remark ?? string.Empty,
                Description = e.Description ?? string.Empty,
                AddedById = SafeEncrypt(e.AddedById, encryptionService, tenantKey),
                AddedDateTime = e.AddedDateTime,
                UpdatedById = SafeEncrypt(e.UpdatedById, encryptionService, tenantKey),
                UpdatedDateTime = e.UpdatedDateTime,
                InfoVerifiedById = SafeEncrypt(e.InfoVerifiedById, encryptionService, tenantKey),
                InfoVerifiedDateTime = e.InfoVerifiedDateTime
            }).ToList();
        }


        public static List<GetDependentResponseDTO> ToGetDependentResponseDTOs(
       List<GetDependentResponseDTO> entities,
       IEncryptionService encryptionService,
       string tenantKey)
        {
            if (entities == null || entities.Count == 0)
                return new List<GetDependentResponseDTO>();

            return entities.Select(e => new GetDependentResponseDTO
            {
                Id = SafeEncrypt(e.Id, encryptionService, tenantKey),
                RoleId = e.RoleId,
                DependentName = e.DependentName ?? string.Empty,
                Relation = e.Relation ?? string.Empty,
                DateOfBirth = e.DateOfBirth,
                IsCoveredInPolicy = e.IsCoveredInPolicy,
                IsMarried = e.IsMarried,
                Remark = e.Remark ?? string.Empty,
                Description = e.Description ?? string.Empty,

                AddedById = SafeEncrypt(e.AddedById, encryptionService, tenantKey),
                UpdatedById = SafeEncrypt(e.UpdatedById, encryptionService, tenantKey),
                AddedDateTime = e.AddedDateTime,
                UpdatedDateTime = e.UpdatedDateTime,

                IsActive = e.IsActive,
                InfoVerifiedById = SafeEncrypt(e.InfoVerifiedById, encryptionService, tenantKey),
                IsInfoVerified = e.IsInfoVerified,
                IsEditAllowed = e.IsEditAllowed,
                InfoVerifiedDateTime = e.InfoVerifiedDateTime,

                PageNumber = e.PageNumber,
                PageSize = e.PageSize,
                SortBy = e.SortBy,
                SortOrder = e.SortOrder
            }).ToList();
        }


        public static List<GetEducationResponseDTO> ToGetEducationResponseDTOs(
     List<GetEducationResponseDTO> entities,
     IEncryptionService encryptionService,
     string tenantKey,
     string encryptedEmployeeId)
        {
            if (entities == null || !entities.Any())
                return new List<GetEducationResponseDTO>();

            return entities.Select(e => new GetEducationResponseDTO
            {
                // 🆔 Encrypted IDs (no double encryption)
                Id = EnsureEncrypted(e.Id, encryptionService, tenantKey),
                EmployeeId = encryptedEmployeeId, // already encrypted from request

                // 🎓 Education Info
                Degree = e.Degree,
                InstituteName = e.InstituteName,
                Remark = e.Remark,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                GradeOrPercentage = e.GradeOrPercentage,
                GPAOrPercentage = e.GPAOrPercentage,
                EducationDocPath = e.EducationDocPath,
                DocType = e.DocType,
                DocName = e.DocName,
                EducationGap = e.EducationGap,
                ReasonOfEducationGap = e.ReasonOfEducationGap,

                // 🕒 Audit Info
                AddedById = EnsureEncrypted(e.AddedById, encryptionService, tenantKey),
                AddedDateTime = e.AddedDateTime,
                UpdatedById = EnsureEncrypted(e.UpdatedById, encryptionService, tenantKey),
                UpdatedDateTime = e.UpdatedDateTime,

                // 🧾 Verification Info
                InfoVerifiedById = EnsureEncrypted(e.InfoVerifiedById, encryptionService, tenantKey),
                IsInfoVerified = e.IsInfoVerified,
                InfoVerifiedDateTime = e.InfoVerifiedDateTime,
                IsEditAllowed = e.IsEditAllowed,

                // ⚙️ Status
                IsActive = e.IsActive
            }).ToList();
        }

        /// <summary>
        /// Prevents double encryption by detecting pre-encrypted strings.
        /// </summary>

        public static List<GetExperienceResponseDTO> ToGetExperienceResponseDTOs(List<GetExperienceResponseDTO> entities, IEncryptionService encryptionService, string tenantKey, string encryptedEmployeeId)
        {
            if (entities == null || entities.Count == 0)
                return new List<GetExperienceResponseDTO>();

            return entities.Select(e => new GetExperienceResponseDTO
            {
                Id = SafeEncrypt(e.UserEmployeeId, encryptionService, tenantKey),
                EmployeeId = encryptedEmployeeId,
                CompanyName = e.CompanyName ?? string.Empty,
                JobTitle = e.JobTitle ?? string.Empty,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                ReasonForLeaving = e.ReasonForLeaving ?? string.Empty,
                Remark = e.Remark ?? string.Empty,
                ExperienceTypeId = e.ExperienceTypeId,
                Location = e.Location ?? string.Empty,
                CTC = e.CTC,
                ReportingManagerName = e.ReportingManagerName ?? string.Empty,
                ReportingManagerNumber = e.ReportingManagerNumber ?? string.Empty,
                ReportingManagerEmail = e.ReportingManagerEmail ?? string.Empty,
                WorkedWithName = e.WorkedWithName ?? string.Empty,
                WorkedWithContactNumber = e.WorkedWithContactNumber ?? string.Empty,
                WorkedWithDesignation = e.WorkedWithDesignation ?? string.Empty,
                ExperienceLetterPath = e.ExperienceLetterPath ?? string.Empty,
                Comment = e.Comment ?? string.Empty,
                AddedById = SafeEncrypt(e.AddedById, encryptionService, tenantKey),
                AddedDateTime = e.AddedDateTime,
                UpdatedById = SafeEncrypt(e.UpdatedById, encryptionService, tenantKey),
                UpdatedDateTime = e.UpdatedDateTime,
                DeletedDateTime = e.DeletedDateTime,
                IsExperienceVerified = e.IsExperienceVerified,
                ExperienceVerificationBy = SafeEncrypt(e.ExperienceVerificationBy, encryptionService, tenantKey),
                ExperienceVerificationDateTime = e.ExperienceVerificationDateTime,
                IsExperienceVerifiedByMail = e.IsExperienceVerifiedByMail,
                IsExperienceVerifiedByCall = e.IsExperienceVerifiedByCall,
                InfoVerifiedById = SafeEncrypt(e.InfoVerifiedById, encryptionService, tenantKey),
                IsInfoVerified = e.IsInfoVerified,
                InfoVerifiedDateTime = e.InfoVerifiedDateTime,
                IsActive = e.IsActive,
                IsEditAllowed = e.IsEditAllowed,
                IsSoftDeleted = e.IsSoftDeleted
            }).ToList();
        }

        public static List<GetIdentityResponseDTO> ToGetIdentityResponseDTOs(List<GetIdentityResponseDTO> entities, IEncryptionService encryptionService, string tenantKey)
        {
            if (entities == null || entities.Count == 0)
                return new List<GetIdentityResponseDTO>();

            return entities.Select(e => new GetIdentityResponseDTO
            {
                Id = SafeEncrypt(e.UserEmployeeId?.ToString(), encryptionService, tenantKey),
                AadhaarNumber = e.AadhaarNumber ?? string.Empty,
                PanNumber = e.PanNumber ?? string.Empty,
                PassportNumber = e.PassportNumber ?? string.Empty,
                DrivingLicenseNumber = e.DrivingLicenseNumber ?? string.Empty,
                VoterId = e.VoterId ?? string.Empty,
                BloodGroup = e.BloodGroup ?? string.Empty,
                MaritalStatus = e.MaritalStatus ?? string.Empty,
                Nationality = e.Nationality ?? string.Empty,
                EmergencyContactName = e.EmergencyContactName ?? string.Empty,
                EmergencyContactNumber = e.EmergencyContactNumber ?? string.Empty,
                IsActive = e.IsActive,
                IsEditAllowed = e.IsEditAllowed,
                IsSoftDeleted = e.IsSoftDeleted,
                AddedById = SafeEncrypt(e.AddedById, encryptionService, tenantKey),
                AddedDateTime = e.AddedDateTime,
                UpdatedById = SafeEncrypt(e.UpdatedById, encryptionService, tenantKey),
                UpdatedDateTime = e.UpdatedDateTime,
                DeletedDateTime = e.DeletedDateTime,
                InfoVerifiedById = SafeEncrypt(e.InfoVerifiedById, encryptionService, tenantKey),
                InfoVerifiedDateTime = e.InfoVerifiedDateTime,
                IsInfoVerified = e.IsInfoVerified
            }).ToList();
        }

        public static List<GetBaseEmployeeResponseDTO> ToGetBaseInfoResponseDTOs(List<GetBaseEmployeeResponseDTO> entities, IEncryptionService encryptionService, string tenantKey)
        {
            if (entities == null || entities.Count == 0)
                return new List<GetBaseEmployeeResponseDTO>();

            return entities.Select(e => new GetBaseEmployeeResponseDTO
            {
                Id = SafeEncrypt(e.Id, encryptionService, tenantKey),
                EmployementCode = e.EmployementCode ?? string.Empty,
                LastName = e.LastName ?? string.Empty,
                MiddleName = e.MiddleName ?? string.Empty,
                FirstName = e.FirstName ?? string.Empty,
                DesignationId = e.DesignationId,
                DepartmentId = e.DepartmentId,
                OfficialEmail = e.OfficialEmail ?? string.Empty,
                IsActive = e.IsActive
            }).ToList();
        }

        public static List<GetDepartmentResponseDTO> ToGetDepartmentResponseDTOs(List<GetDepartmentResponseDTO> entities, IIdEncoderService idEncoderService, string tenantKey)
        {
            if (entities == null || entities.Count == 0)
                return new List<GetDepartmentResponseDTO>();

            return entities.Select(e => new GetDepartmentResponseDTO
            {

                Id = idEncoderService.EncodeString(e.Id, tenantKey),
                AddedById = idEncoderService.EncodeId(SafeParser.TryParseLong(e.AddedById), tenantKey),
                UpdatedById = idEncoderService.EncodeId(SafeParser.TryParseLong(e.UpdatedById), tenantKey),
                DepartmentName = e.DepartmentName ?? string.Empty,
                Description = e.Description ?? string.Empty,
                IsActive = e.IsActive
            }).ToList();
        }

        public static List<GetRoleResponseDTO> ToGetRoleResponseDTOs(List<GetRoleResponseDTO> entities, IEncryptionService encryptionService, string tenantKey)
        {
            if (entities == null || entities.Count == 0)
                return new List<GetRoleResponseDTO>();

            return entities.Select(e => new GetRoleResponseDTO
            {
                AddedById = SafeEncrypt(e.AddedById?.ToString(), encryptionService, tenantKey),
                UpdatedById = SafeEncrypt(e.UpdatedById?.ToString(), encryptionService, tenantKey),
                RoleName = e.RoleName ?? string.Empty,
                Remark = e.Remark ?? string.Empty,
                IsActive = e.IsActive
            }).ToList();
        }

        public static List<GetDesignationResponseDTO> ToGetDesignationResponseDTOs(List<GetDesignationResponseDTO> entities, IEncryptionService encryptionService, string tenantKey)
        {
            if (entities == null || entities.Count == 0)
                return new List<GetDesignationResponseDTO>();

            return entities.Select(e => new GetDesignationResponseDTO
            {
                Id = SafeEncrypt(e.Id?.ToString(), encryptionService, tenantKey),
                AddedById = SafeEncrypt(e.AddedById?.ToString(), encryptionService, tenantKey),
                UpdatedById = SafeEncrypt(e.UpdatedById?.ToString(), encryptionService, tenantKey),
                DepartmentName = e.DepartmentName ?? string.Empty,
                Description = e.Description ?? string.Empty,
                IsActive = e.IsActive
            }).ToList();
        }

        public static List<GetEmployeeImageReponseDTO> ToGetEmployeeImageResponseDTOs(List<GetEmployeeImageReponseDTO> entities, IEncryptionService encryptionService, string tenantKey)
        {
            if (entities == null || entities.Count == 0)
                return new List<GetEmployeeImageReponseDTO>();

            return entities.Select(e => new GetEmployeeImageReponseDTO
            {
                Id = SafeEncrypt(e.Id, encryptionService, tenantKey),
                EmployeeImagePath = e.EmployeeImagePath ?? string.Empty,
                IsActive = e.IsActive,
                IsPrimary = e.IsPrimary
            }).ToList();
        }

    }

}
