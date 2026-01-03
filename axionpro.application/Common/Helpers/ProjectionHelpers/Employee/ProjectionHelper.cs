using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.PercentageHelper;
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
using axionpro.application.DTOS.StoreProcedures;
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

        public static List<GetDependentResponseDTO> ToGetDependentResponseDTOs(
     List<GetDependentResponseDTO> entities,
     IIdEncoderService encoderService,
     string tenantKey,
     IConfiguration configuration)
        {
            if (entities == null || !entities.Any())
                return new List<GetDependentResponseDTO>();

            string baseUrl = configuration["FileSettings:BaseUrl"] ?? string.Empty;

            foreach (var item in entities)
            {
                if (item == null)
                    continue;

                // 🔐 Encode Dependent Id
                if (long.TryParse(item.Id, out long rawId) && rawId > 0)
                    item.Id = encoderService.EncodeId_long(rawId, tenantKey);

                // 🔐 Encode EmployeeId
                if (long.TryParse(item.EmployeeId, out long empRawId) && empRawId > 0)
                    item.EmployeeId = encoderService.EncodeId_long(empRawId, tenantKey);

                // 📁 Build full file URL
                if (!string.IsNullOrWhiteSpace(item.FilePath))
                    item.FilePath = $"{baseUrl}{item.FilePath}";
                else
                    item.FilePath = string.Empty;

                // 📊 Completion %
                item.CompletionPercentage =
                    CompletionCalculatorHelper.DependentPropCalculate(item);

                // 🧹 Null safety
                item.Id ??= string.Empty;
                item.EmployeeId ??= string.Empty;
                item.DependentName ??= string.Empty;
                item.Relation ??= string.Empty;
                item.Remark ??= string.Empty;
                item.Description ??= string.Empty;
            }

            return entities;
        }

        public static List<GetEmployeeIdentityResponseDTO> ToGetIdentityResponseDTO(
        IEnumerable<GetEmployeeIdentitySp> entities,
        IIdEncoderService encoder,
        string tenantKey)
        {
            if (entities == null)
                return new List<GetEmployeeIdentityResponseDTO>();

            return entities.Select(e => new GetEmployeeIdentityResponseDTO
            {
                // 🔐 ONLY EmployeeId is encoded (ASXER rule)
                EmployeeId = e.EmployeeId.HasValue
                    ? encoder.EncodeId_long(e.EmployeeId.Value, tenantKey)
                    : null,

                // 🌍 Country
                Id = e.Id,
                CountryCode = e.CountryCode,
                CountryName = e.CountryName,

                // 🗂 Identity Category
                IdentityCategoryName = e.IdentityCategoryName,

                // 📄 Document (RAW master IDs)
                IdentityCategoryDocumentId = e.IdentityCategoryDocumentId,
                DocumentCode = e.DocumentCode,
                DocumentName = e.DocumentName,
                Description = e.Description,
                IsMandatory = e.IsMandatory,

                // 👤 EmployeeIdentity
                EmployeeIdentityId = e.EmployeeIdentityId,
                IdentityValue = e.IdentityValue,

                // ✅ Flags
                IsVerified = e.IsVerified,
                IsEditAllowed = e.IsEditAllowed,
                HasIdentityUploaded = e.HasIdentityUploaded,

                // 📆 Validity
                EffectiveFrom = e.EffectiveFrom,
                EffectiveTo = e.EffectiveTo,

                IsActive = e.IsActive
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
                    item.EmployeeId = encoderService.EncodeId_long(rawId, tenantKey);
                }
                item.EmployeeImagePath ??= null;

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
                    item.Id = encoderService.EncodeId_long(rawId, tenantKey);
                }

                // Null-safe defaults
                item.FilePath ??= string.Empty;


                // 🔹 Ensure IsPrimary always returns true/false (avoid null)
                item.IsPrimary = item.IsPrimary ? true : false;
            }

            return entities;
        }


        public static GetBaseEmployeeResponseDTO? ToGetBaseInfoResponseDTO(
                 GetBaseEmployeeResponseDTO? entity,
                 IIdEncoderService encoderService,
                 string tenantKey)
        {
            if (entity == null)
                return null;

            // ✅ Sirf Id encode karo
            if (!string.IsNullOrWhiteSpace(entity.Id) && long.TryParse(entity.Id, out long rawId) && rawId > 0)
            {
                entity.Id = encoderService.EncodeId_long(rawId, tenantKey);
            }



            // ✅ Baaki sab as-it-is (optional null safety)          
            entity.FirstName ??= string.Empty;
            entity.LastName ??= string.Empty;
            entity.MiddleName ??= string.Empty;
            entity.OfficialEmail ??= string.Empty;
            entity.EmployementCode ??= string.Empty;


            return entity;
        }

        public static List<GetBaseEmployeeResponseDTO> ToGetBaseInfoListResponseDTOs(List<GetBaseEmployeeResponseDTO> entities,
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
                    item.Id = encoderService.EncodeId_long(rawId, tenantKey);
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

        public static List<GetContactResponseDTO> ToGetContactResponseDTOs(
                PagedResponseDTO<GetContactResponseDTO> pagedResult,
                  IIdEncoderService encoderService,
                   string tenantKey)
        {
            if (pagedResult?.Items == null || !pagedResult.Items.Any())
                return new List<GetContactResponseDTO>();

            foreach (var item in pagedResult.Items)
            {
                if (item == null)
                    continue;

                // ✅ Encode Contact Id
                if (long.TryParse(item.Id, out long rawId) && rawId > 0)
                    item.Id = encoderService.EncodeId_long(rawId, tenantKey);

                // ✅ Encode EmployeeId
                if (long.TryParse(item.EmployeeId, out long empRawId) && empRawId > 0)
                    item.EmployeeId = encoderService.EncodeId_long(empRawId, tenantKey);

                // ✅ Lightweight null safety
                item.Id ??= string.Empty;
                item.EmployeeId ??= string.Empty;
                item.ContactNumber ??= string.Empty;
                item.Email ??= string.Empty;
                item.ContactName ??= string.Empty;
                item.Relation ??= 0;
                item.ContactType ??= 0;
                item.CountryName ??= string.Empty;
                item.StateName ??= string.Empty;
                item.DistrictName ??= string.Empty;
                item.HouseNo ??= string.Empty;
                item.LandMark ??= string.Empty;
                item.Street ??= string.Empty;
                item.Address ??= string.Empty;
                item.Remark ??= string.Empty;
                item.Description ??= string.Empty;
                item.IsPrimary = item.IsPrimary ?? false;
                item.IsActive = item.IsActive ?? false;
                item.IsEditAllowed = item.IsEditAllowed ?? false;
                item.IsInfoVerified = item.IsInfoVerified ?? false;
                item.CountryId = item.CountryId ?? 0;
                item.StateId = item.StateId ?? 0;
                item.DistrictId = item.DistrictId ?? 0;


            }

            return pagedResult.Items;
        }

        public static List<GetBankResponseDTO> ToGetBankResponseDTOs(PagedResponseDTO<GetBankResponseDTO> entities,
         IIdEncoderService encoderService,
        string tenantKey, IConfiguration configuration
           )
        {
            string baseUrl = configuration["FileSettings:BaseUrl"] ?? string.Empty;
            string defaultImg = configuration["FileSettings:DefaultImage"] ?? string.Empty;

            if (entities == null || entities.Items == null || !entities.Items.Any())
                return new List<GetBankResponseDTO>();

            var result = new List<GetBankResponseDTO>();

            foreach (var item in entities.Items) // now item is GetBankResponseDTO
            {
                if (item == null) continue;




                item.FilePath ??= string.Empty;
                // 📁 Final Image URL build
                if (!string.IsNullOrEmpty(item.FilePath))
                    item.FilePath = $"{baseUrl}{item.FilePath}";


                // ✅ Encode EmployeeId separately

                if (long.TryParse(item.EmployeeId, out long rawId) && rawId > 0)
                {
                    item.EmployeeId = encoderService.EncodeId_long(rawId, tenantKey);

                }


                // ✅ Optional cleanup (avoid nulls)
                item.EmployeeId ??= string.Empty;
                //  item.Id ??= 0;

                result.Add(item);
            }

            return result;
        }

        private static string EncodeId(
    string? rawId,
    IIdEncoderService encoder,
    string tenantKey)
        {
            if (long.TryParse(rawId, out long id) && id > 0)
                return encoder.EncodeId_long(id, tenantKey);

            return string.Empty;
        }

        private static string BuildFilePath(
            string? filePath,
            string baseUrl,
            string defaultImage)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return defaultImage;

            return $"{baseUrl}{filePath}";
        }


        public static List<GetEducationResponseDTO> ToGetEducationResponseDTOs(
        PagedResponseDTO<GetEducationResponseDTO> source,
        IIdEncoderService encoderService,
        string tenantKey, IConfiguration configuration)
        {
            string baseUrl = configuration["FileSettings:BaseUrl"] ?? string.Empty;
            string defaultImg = configuration["FileSettings:DefaultImage"] ?? string.Empty;
            if (source?.Items == null || source.Items.Count == 0)
                return new();

            return source.Items
                .Where(x => x != null)
                .Select(item => new GetEducationResponseDTO
                {
                    Id = EncodeId(item.Id, encoderService, tenantKey),
                    EmployeeId = EncodeId(item.EmployeeId, encoderService, tenantKey),

                    Degree = item.Degree,
                    InstituteName = item.InstituteName,
                    ScoreType = item.ScoreType,
                    StartDate = item.StartDate,
                    EndDate = item.EndDate,
                    FilePath   = BuildFilePath(item.FilePath, baseUrl, defaultImg),
                    FileType = item.FileType,
                    IsActive = item.IsActive,
                    IsEditAllowed = item.IsEditAllowed,
                    IsInfoVerified = item.IsInfoVerified,
                    HasEducationDocUploded = item.HasEducationDocUploded,
                    CompletionPercentage = item.CompletionPercentage
                })
                .ToList();
        }


        //public static List<GetDepartmentResponseDTO> ToGetDepartmentResponseDTOs(List<GetDepartmentResponseDTO> entities, IIdEncoderService idEncoderService, string tenantKey)
        //{
        //    if (entities == null || entities.Count == 0)
        //        return new List<GetDepartmentResponseDTO>();

        //    return entities.Select(e => new GetDepartmentResponseDTO
        //    {

        //        Id =  e.,
        //        //AddedById = idEncoderService.EncodeId(SafeParser.TryParseLong(e.AddedById), tenantKey),
        //        //UpdatedById = idEncoderService.EncodeId(SafeParser.TryParseLong(e.UpdatedById), tenantKey),
        //        DepartmentName = e.DepartmentName ?? string.Empty,
        //        Description = e.Description ?? string.Empty,
        //        IsActive = e.IsActive
        //    }).ToList();
        //}

    




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

                item.Id = item.Id.ToString();

                // 🔐 Encode Employee Id
                if (long.TryParse(item.EmployeeId, out long empRawId) && empRawId > 0)
                    item.EmployeeId = encoderService.EncodeId_long(empRawId, tenantKey);

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
