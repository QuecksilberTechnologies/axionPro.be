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
using axionpro.application.DTOS.Pagination;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.domain.Entity;
using Microsoft.Extensions.Configuration;
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


      

        public static List<GetDependentResponseDTO> ToGetDependentResponseDTOs(List<GetDependentResponseDTO> entities,
      IIdEncoderService encoderService,
      string tenantKey)
        {
            if (entities == null || !entities.Any())
                return new List<GetDependentResponseDTO>();

            foreach (var item in entities)
            {
                // ✅ Sirf Id encode karo (agar valid hai)
                if (long.TryParse(item.Id, out long rawId) && rawId > 0)
                {
                    item.Id = encoderService.EncodeId(rawId, tenantKey);
                }
                // ✅ Encode EmployeeId separately
                if (long.TryParse(item.EmployeeId, out long empRawId) && empRawId > 0)
                {
                    item.EmployeeId = encoderService.EncodeId(empRawId, tenantKey);
                }

                // ✅ Optional cleanup (avoid nulls)
                item.EmployeeId ??= string.Empty;
                item.Id ??= string.Empty;

            }

            return entities;
        }



        /// <summary>
        /// Prevents double encryption by detecting pre-encrypted strings.
        /// </summary>

        //public static List<GetExperienceResponseDTO> ToGetExperienceResponseDTOs(List<GetExperienceResponseDTO> entities, IEncryptionService encryptionService, string tenantKey, string encryptedEmployeeId)
        //{
        //    if (entities == null || entities.Count == 0)
        //        return new List<GetExperienceResponseDTO>();

        //    return entities.Select(e => new GetExperienceResponseDTO
        //    {
        //        Id = SafeEncrypt(e.UserEmployeeId, encryptionService, tenantKey),
        //        EmployeeId = encryptedEmployeeId,
        //        CompanyName = e.CompanyName ?? string.Empty,
        //        JobTitle = e.JobTitle ?? string.Empty,
        //        StartDate = e.StartDate,
        //        EndDate = e.EndDate,
        //        ReasonForLeaving = e.ReasonForLeaving ?? string.Empty,
        //        Remark = e.Remark ?? string.Empty,
        //        ExperienceTypeId = e.ExperienceTypeId,
        //        Location = e.Location ?? string.Empty,
        //        CTC = e.CTC,
        //        ReportingManagerName = e.ReportingManagerName ?? string.Empty,
        //        ReportingManagerNumber = e.ReportingManagerNumber ?? string.Empty,
        //        ReportingManagerEmail = e.ReportingManagerEmail ?? string.Empty,
        //        WorkedWithName = e.WorkedWithName ?? string.Empty,
        //        WorkedWithContactNumber = e.WorkedWithContactNumber ?? string.Empty,
        //        WorkedWithDesignation = e.WorkedWithDesignation ?? string.Empty,
        //        ExperienceLetterPath = e.ExperienceLetterPath ?? string.Empty,
        //        Comment = e.Comment ?? string.Empty,
        //        AddedById = SafeEncrypt(e.AddedById, encryptionService, tenantKey),
          
        //    }).ToList();
        //}

        public static List<GetIdentityResponseDTO> ToGetIdentityResponseDTOs(
    List<GetIdentityResponseDTO> entities,
    IIdEncoderService encryptionService,
    string tenantKey)
        {
            if (entities == null || entities.Count == 0)
                return new List<GetIdentityResponseDTO>();

            return entities.Select(e => new GetIdentityResponseDTO
            {
                EmployeeId = encryptionService.EncodeString(e.EmployeeId, tenantKey),
                AadhaarNumber = e.AadhaarNumber,
                PanNumber = e.PanNumber,
                PassportNumber = e.PassportNumber,
                DrivingLicenseNumber = e.DrivingLicenseNumber,
                VoterId = e.VoterId,
                BloodGroup = e.BloodGroup,
                MaritalStatus = e.MaritalStatus,
                Nationality = e.Nationality,
                EmergencyContactName = e.EmergencyContactName,
                EmergencyContactNumber = e.EmergencyContactNumber,
                EmergencyContactRelation = e.EmergencyContactRelation,
                IsInfoVerified = e.IsInfoVerified,
                IsEditAllowed = e.IsEditAllowed,                
                hasPanIdUploaded = e.hasPanIdUploaded,
                hasAadharIdUploaded = e.hasAadharIdUploaded,
                hasPassportIdUploaded = e.hasPassportIdUploaded,
                panFilePath = e.panFilePath,
                passportFilePath = e.passportFilePath,
                aadharFilePath = e.aadharFilePath,
                CompletionPercentage = e.CompletionPercentage,
                HasEPFAccount = e.HasEPFAccount,
                UANNumber = e.UANNumber
            }).ToList();
        }

        public static List<GetAllEmployeeInfoResponseDTO> ToGetAllEmployeeInfoResponseDTOs(
     PagedResponseDTO<GetAllEmployeeInfoResponseDTO> entities,
      IIdEncoderService encoderService,
      string tenantKey, IConfiguration configuration)
        {
            string baseUrl = configuration["FileSettings:BaseUrl"] ?? string.Empty;
            string defaultImg = configuration["FileSettings:DefaultImage"] ?? string.Empty;
            if (entities == null || !entities.Items.Any())
                return new List<GetAllEmployeeInfoResponseDTO>();

            foreach (var item in entities.Items)
            {
                if (long.TryParse(item.EmployeeId, out long rawId) && rawId > 0)
                {
                    item.EmployeeId = encoderService.EncodeId(rawId, tenantKey);
                }
                item.EmployeeImagePath ??= string.Empty;

                // 📁 Final Image URL build
                if (!string.IsNullOrEmpty(item.EmployeeImagePath))
                    item.EmployeeImagePath = $"{baseUrl}{item.EmployeeImagePath}";
              
                item.EmployementCode ??= string.Empty;
                item.FirstName ??= string.Empty;
                item.LastName ??= string.Empty;
                item.MiddleName ??= string.Empty;
                item.OfficialEmail ??= string.Empty;
            } 

            return entities.Items;
        }


        public static List<GetEmployeeImageReponseDTO> ToGetProfileImageInfoResponseDTOs(
       List<GetEmployeeImageReponseDTO> entities,
       IIdEncoderService encoderService,
       string tenantKey)
        {
            if (entities == null || entities.Count == 0)
                return new List<GetEmployeeImageReponseDTO>();

            foreach (var item in entities)
            {
                // Encode numeric Id only
                if (!string.IsNullOrWhiteSpace(item.Id) &&
                    long.TryParse(item.Id, out long rawId) &&
                    rawId > 0)
                {
                    item.Id = encoderService.EncodeId(rawId, tenantKey);
                }

                // Null-safe defaults
                item.FilePath ??= string.Empty;
               

                // 🔹 Ensure IsPrimary always returns true/false (avoid null)
                item.IsPrimary = item.IsPrimary ?? false;
            }

            return entities;
        }


        public static List<GetBaseEmployeeResponseDTO> ToGetBaseInfoResponseDTOs(List<GetBaseEmployeeResponseDTO> entities,
      IIdEncoderService encoderService,
      string tenantKey)
        {
            if (entities == null || !entities.Any())
                return new List<GetBaseEmployeeResponseDTO>();

            foreach (var item in entities)
            {
                // ✅ Sirf Id encode karo (agar valid hai)
                if (long.TryParse(item.Id, out long rawId) && rawId > 0)
                {
                    item.Id = encoderService.EncodeId(rawId, tenantKey);
                }

                // ✅ Optional null-safe cleanup (lightweight safety)
                item.EmployementCode ??= string.Empty;
                item.FirstName ??= string.Empty;
                item.LastName ??= string.Empty;
                item.MiddleName ??= string.Empty;
                item.OfficialEmail ??= string.Empty;
            }

            return entities;
        }
        public static List<GetBankResponseDTO> ToGetBankResponseDTOs(  PagedResponseDTO<GetBankResponseDTO> entities,
    IIdEncoderService encoderService,
    string tenantKey)
        {
            if (entities == null || entities.Items == null || !entities.Items.Any())
                return new List<GetBankResponseDTO>();

            var result = new List<GetBankResponseDTO>();

            foreach (var item in entities.Items) // now item is GetBankResponseDTO
            {
                if (item == null) continue;

                // ✅ Encode Bank Record Id
                if (long.TryParse(item.Id, out long rawId) && rawId > 0)
                {
                    item.Id = encoderService.EncodeId(rawId, tenantKey);
                }

                // ✅ Encode EmployeeId separately
                if (long.TryParse(item.EmployeeId, out long empRawId) && empRawId > 0)
                {
                    item.EmployeeId = encoderService.EncodeId(empRawId, tenantKey);
                }

                // ✅ Optional cleanup (avoid nulls)
                item.EmployeeId ??= string.Empty;
                item.Id ??= string.Empty;

                result.Add(item);
            }

            return result;
        }


        public static List<GetEducationResponseDTO> ToGetEducationResponseDTOs(  PagedResponseDTO<GetEducationResponseDTO> entities, IIdEncoderService encryptionService,
            string tenantKey, IConfiguration configuration
           )
        {
            string baseUrl = configuration["FileSettings:BaseUrl"] ?? string.Empty;
            string defaultImg = configuration["FileSettings:DefaultImage"] ?? string.Empty;

            // 🔹 Null / Empty check
            if (entities == null || entities.Items == null || !entities.Items.Any())
                return new List<GetEducationResponseDTO>();

            var result = new List<GetEducationResponseDTO>();

            foreach (var item in entities.Items)
            {
                if (item == null) continue;

                item.FilecPath ??= string.Empty;
                // ✅ ID encrypt करो (अगर valid long है)
                if (long.TryParse(item.Id, out long rawId) && rawId > 0)
                {
                    item.Id = encryptionService.EncodeId(rawId, tenantKey);
                }

                // 📁 Final Image URL build
                if (!string.IsNullOrEmpty(item.FilecPath))
                    item.FilecPath = $"{baseUrl}{item.FilecPath}";

                // ✅ EmployeeId encrypt करो
                if (long.TryParse(item.EmployeeId, out long empRawId) && empRawId > 0)
                {
                    item.EmployeeId = encryptionService.EncodeId(empRawId, tenantKey);
                }

                // ✅ Optional cleanup (null safety)
                item.Id ??= string.Empty;
                item.EmployeeId ??= string.Empty;

                result.Add(item);
            }

            return result;
        }

        public static List<GetDepartmentResponseDTO> ToGetDepartmentResponseDTOs(List<GetDepartmentResponseDTO> entities, IIdEncoderService idEncoderService, string tenantKey)
        {
            if (entities == null || entities.Count == 0)
                return new List<GetDepartmentResponseDTO>();

            return entities.Select(e => new GetDepartmentResponseDTO
            {

                Id = idEncoderService.EncodeString(e.Id, tenantKey),
                //AddedById = idEncoderService.EncodeId(SafeParser.TryParseLong(e.AddedById), tenantKey),
                //UpdatedById = idEncoderService.EncodeId(SafeParser.TryParseLong(e.UpdatedById), tenantKey),
                DepartmentName = e.DepartmentName ?? string.Empty,
                Description = e.Description ?? string.Empty,
                IsActive = e.IsActive
            }).ToList();
        }

        public static List<GetContactResponseDTO> ToGetContactResponseDTOs(PagedResponseDTO<GetContactResponseDTO> entities,
            IIdEncoderService encoderService,
                    string tenantKey)
        {
            if (entities == null || entities.Items == null || !entities.Items.Any())
                return new List<GetContactResponseDTO>();

            var result = new List<GetContactResponseDTO>();

            foreach (var item in entities.Items) // now item is GetBankResponseDTO
            {
                if (item == null) continue;

                // ✅ Encode Bank Record Id
                if (long.TryParse(item.Id, out long rawId) && rawId > 0)
                {
                    item.Id = encoderService.EncodeId(rawId, tenantKey);
                }

                // ✅ Encode EmployeeId separately
                if (long.TryParse(item.EmployeeId, out long empRawId) && empRawId > 0)
                {
                    item.EmployeeId = encoderService.EncodeId(empRawId, tenantKey);
                }

                // ✅ Optional cleanup (avoid nulls)
                item.EmployeeId ??= string.Empty;
                item.Id ??= string.Empty;

                result.Add(item);
            }

            return result;
        }





        //public static List<GetRoleResponseDTO> ToGetRoleResponseDTOs(List<GetRoleResponseDTO> entities, IEncryptionService encryptionService, string tenantKey)
        //{
        //    if (entities == null || entities.Count == 0)
        //        return new List<GetRoleResponseDTO>();

        //    return entities.Select(e => new GetRoleResponseDTO
        //    {
        //        AddedById = SafeEncrypt(e.AddedById?.ToString(), encryptionService, tenantKey),
        //        UpdatedById = SafeEncrypt(e.UpdatedById?.ToString(), encryptionService, tenantKey),
        //        RoleName = e.RoleName ?? string.Empty,
        //        Remark = e.Remark ?? string.Empty,
        //        IsActive = e.IsActive
        //    }).ToList();
        //}

        //public static List<GetDesignationResponseDTO> ToGetDesignationResponseDTOs(List<GetDesignationResponseDTO> entities, IEncryptionService encryptionService, string tenantKey)
        //{
        //    if (entities == null || entities.Count == 0)
        //        return new List<GetDesignationResponseDTO>();

        //    return entities.Select(e => new GetDesignationResponseDTO
        //    {
        //        Id = SafeEncrypt(e.Id?.ToString(), encryptionService, tenantKey),
        //        AddedById = SafeEncrypt(e.AddedById?.ToString(), encryptionService, tenantKey),
        //        UpdatedById = SafeEncrypt(e.UpdatedById?.ToString(), encryptionService, tenantKey),
        //        DepartmentName = e.DepartmentName ?? string.Empty,
        //        Description = e.Description ?? string.Empty,
        //        IsActive = e.IsActive
        //    }).ToList();
        //}

        public static List<GetEmployeeImageReponseDTO> ToGetEmployeeImageResponseDTOs(
      PagedResponseDTO<GetEmployeeImageReponseDTO> entities,
      IIdEncoderService encoderService,
      string tenantKey,
      IConfiguration configuration)
        {
            if (entities == null || entities.Items == null || !entities.Items.Any())
                return new List<GetEmployeeImageReponseDTO>();

            // 🔥 Base URL & Default Image — Correct Way
            string baseUrl = configuration["FileSettings:BaseUrl"] ?? string.Empty;
            string defaultImg = configuration["FileSettings:DefaultImage"] ?? string.Empty;

            var result = new List<GetEmployeeImageReponseDTO>();

            foreach (var item in entities.Items)
            {
                if (item == null) continue;

                // 🔐 Encode Image Id
                if (long.TryParse(item.Id, out long rawId) && rawId > 0)
                    item.Id = encoderService.EncodeId(rawId, tenantKey);

                // 🔐 Encode Employee Id
                if (long.TryParse(item.EmployeeId, out long empRawId) && empRawId > 0)
                    item.EmployeeId = encoderService.EncodeId(empRawId, tenantKey);

                // 🧹 Null sanitization
                item.Id ??= string.Empty;
                item.EmployeeId ??= string.Empty;
                item.FilePath ??= string.Empty;

                // 📁 Final Image URL build
                if (!string.IsNullOrEmpty(item.FilePath))
                    item.FilePath = $"{baseUrl}{item.FilePath}";
               

                // These are already same — keeping for clarity
                item.IsPrimary = item.IsPrimary;
                item.IsActive = item.IsActive;
                item.CompletionPercentage = item.CompletionPercentage;

                result.Add(item);
            }

            return result;
        }





    }

}
