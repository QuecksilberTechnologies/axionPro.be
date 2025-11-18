using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.axionpro.application.Configuration;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.DTOs.Department;
using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.DTOS.Employee.Education;
using axionpro.application.DTOS.Employee.Experience;
using axionpro.application.DTOS.Employee.Sensitive;
using axionpro.application.DTOS.Pagination;
using axionpro.application.Features.EmployeeCmd.EducationInfo.Handlers;
using axionpro.application.Features.EmployeeCmd.SensitiveInfo.Handlers;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IFileStorage;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace axionpro.application.Features.EmployeeCmd.ExperienceInfo.Handlers
{
    public class CreateExperienceInfoCommand : IRequest<ApiResponse<List<GetExperienceResponseDTO>>>
    {
        public CreateExperienceRequestDTO DTO { get; set; }

        public CreateExperienceInfoCommand(CreateExperienceRequestDTO dTO)
        {
            DTO = dTO;
        }
    }

    public class CreateExperienceInfoCommandHandler
     : IRequestHandler<CreateExperienceInfoCommand, ApiResponse<List<GetExperienceResponseDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CreateExperienceInfoCommand> _logger;
        private readonly ITokenService _tokenService;
        private readonly IPermissionService _permissionService;
        private readonly IConfiguration _config;
        private readonly IEncryptionService _encryptionService;
        private readonly IIdEncoderService _idEncoderService;
        private readonly IFileStorageService _fileStorageService;

        public CreateExperienceInfoCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<CreateExperienceInfoCommand> logger,
            ITokenService tokenService,
            IPermissionService permissionService,
            IConfiguration config,
            IEncryptionService encryptionService,
            IIdEncoderService idEncoderService,
            IFileStorageService fileStorageService
        )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _tokenService = tokenService;
            _permissionService = permissionService;
            _config = config;
            _encryptionService = encryptionService;
            _idEncoderService = idEncoderService;
            _fileStorageService = fileStorageService;
        }
        public async Task<ApiResponse<List<GetExperienceResponseDTO>>> Handle(
            CreateExperienceInfoCommand request,
            CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                //--------------------------------------------------------
                // 1️⃣ TOKEN + CLAIMS VALIDATION
                //--------------------------------------------------------
                // 🧩 STEP 1: Validate JWT Token
                // 🧩 STEP 1: Token Validation
                var bearerToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
                    .ToString()?.Replace("Bearer ", "");

                if (string.IsNullOrEmpty(bearerToken))
                    return ApiResponse<List<GetExperienceResponseDTO>>.Fail("Unauthorized: Token not found.");

                var secretKey = TokenKeyHelper.GetJwtSecret(_config);
                var tokenClaims = TokenClaimHelper.ExtractClaims(bearerToken, secretKey);

                if (tokenClaims == null || tokenClaims.IsExpired)
                    return ApiResponse<List<GetExperienceResponseDTO>>.Fail("Invalid or expired token.");

                long loggedInEmpId = await _unitOfWork.CommonRepository.ValidateActiveUserLoginOnlyAsync(tokenClaims.UserId);
                if (loggedInEmpId < 1)
                    return ApiResponse<List<GetExperienceResponseDTO>>.Fail("Unauthorized or inactive user.");

                string tenantKey = tokenClaims.TenantEncriptionKey ?? string.Empty;
                if (string.IsNullOrEmpty(tenantKey) || string.IsNullOrEmpty(request.DTO.UserEmployeeId))
                    return ApiResponse<List<GetExperienceResponseDTO>>.Fail("User invalid.");

                string finalKey = EncryptionSanitizer.SuperSanitize(tenantKey);
                //UserEmployeeId
                string UserEmpId = EncryptionSanitizer.CleanEncodedInput(request.DTO.UserEmployeeId);
                request.DTO._UserEmployeeId = _idEncoderService.DecodeId(UserEmpId, finalKey);
                //Token TenantId
                string tokenTenant = EncryptionSanitizer.CleanEncodedInput(tokenClaims.TenantId);
                long decryptedTenantId = _idEncoderService.DecodeId(tokenTenant, finalKey);
                //Id              
                // Actual EmployeeId
                string actualEmpId = EncryptionSanitizer.CleanEncodedInput(request.DTO.EmployeeId);
                request.DTO._EmployeeId = _idEncoderService.DecodeId(UserEmpId, finalKey);

                // 🧩 STEP 4: Validate all employee references
                if (decryptedTenantId <= 0)
                {
                    _logger.LogWarning("❌ Tenant or employee information missing in token/request.");
                    return ApiResponse<List<GetExperienceResponseDTO>>.Fail("Tenant or employee information missing.");
                }


                if (decryptedTenantId <= 0 || request.DTO._UserEmployeeId <= 0)
                {
                    _logger.LogWarning("❌ Tenant or employee information missing in token/request.");
                    return ApiResponse<List<GetExperienceResponseDTO>>.Fail("Tenant or employee information missing.");
                }

                if (!(request.DTO._UserEmployeeId == loggedInEmpId))
                {
                    _logger.LogWarning(
                        "❌ EmployeeId mismatch. RequestEmpId: {ReqEmp}, LoggedEmpId: {LoggedEmp}",
                         request.DTO._UserEmployeeId, loggedInEmpId
                    );

                    return ApiResponse<List<GetExperienceResponseDTO>>.Fail("Unauthorized: Employee mismatch.");
                }

                var permissions = await _permissionService.GetPermissionsAsync(SafeParser.TryParseInt(tokenClaims.RoleId));
                if (!permissions.Contains("AddBankInfo"))
                {
                    //  await _unitOfWork.RollbackTransactionAsync();
                    //return ApiResponse<List<GetBankResponseDTO>>.Fail("You do not have permission to add bank info.");
                }
                // 🧩 STEP 4: Call Repository to get data     


                //--------------------------------------------------------
                // 2️⃣ INSERT PARENT — EmployeeExperience
                //--------------------------------------------------------
                var experience = new EmployeeExperience
                {
                    EmployeeId = request.DTO._EmployeeId,
                    Ctc = request.DTO._CTC,
                    Comment = request.DTO.Comment,
                    AddedById = request.DTO._UserEmployeeId,
                    AddedDateTime = DateTime.UtcNow,
                    IsActive = true,
                    IsEditAllowed = true,
                    IsFresher = request.DTO.IsFresher,
                    HasEPFAccount = request.DTO.HasEPFAccount
                };

                var parentResult =
                    await _unitOfWork.EmployeeExpereinceRepository.AddExperienceAsync(experience);

                long experienceId = parentResult.Items.First().Id;
               

                //--------------------------------------------------------
                // 3️⃣ BUILD CHILD LISTS
                //--------------------------------------------------------
                var detailList = new List<EmployeeExperienceDetail>();
                var payslipList = new List<EmployeeExperiencePayslipUpload>();

              


                // *****************************************************************
                // ⭐ SCENARIO-WISE CHILD BUILDING ⭐
                // *****************************************************************

                // =============================
                // CASE 1: Fresher + No Gap
                // =============================
                if (request.DTO.IsFresher && !request.DTO.IsGap)
                {
                    // NO DETAILS, NO PAYSLIP
                }

                // =============================
                // CASE 2: Fresher + Gap
                // =============================
                else if (request.DTO.IsFresher && request.DTO.IsGap)
                {
                    foreach (var d in request.DTO.ExperienceDetails)
                    {
                        detailList.Add(await BuildGapDetail(d, request, decryptedTenantId, experienceId));
                    }
                }

                // =============================
                // CASE 3: Experience + (Gap or No-Gap)
                // =============================


                else if (!request.DTO.IsFresher)
                {
                    

                    foreach (var d in request.DTO.ExperienceDetails)
                    {
                        d.IsAnyGap= request.DTO.IsGap;
                        // GAP detail
                        if (d.IsAnyGap)
                        {
                              detailList.Add(
                                await BuildGapDetail(d, request, decryptedTenantId, experienceId)
                            );
                            continue;
                        }

                        // EXPERIENCE detail
                        var det = await BuildExperienceDetail(d, request, decryptedTenantId, experienceId);

                        detailList.Add(det);

                        //--------------------------------------------------------
                        // Payslips linked WITH ExperienceDetailID (NOT parent)
                        //--------------------------------------------------------
                        foreach (var slip in d.Payslips)
                        {
                            // upload
                            var up = await UploadDocAsync(new UploadDocRequestDTO
                            {
                                TenantId = decryptedTenantId,
                                EmployeeId = request.DTO._EmployeeId,
                                File = slip.PayslipDocument,
                                SubFolderName = "payslip-docs",
                                FilePrefix = $"{slip.Month}-{slip.Year}",
                                FileType = 2
                            });

                            payslipList.Add(new EmployeeExperiencePayslipUpload
                            {
                                EmployeeId = request.DTO._EmployeeId,
                                 
                                ExperienceDetailId = det.Id,        // **IMPORTANT: detail ref**
                                Month = SafeParser.TryParseInt(slip.Month),
                                Year = SafeParser.TryParseInt(slip.Year),
                                PayslipDocName = up.FileName,
                                PayslipDocPath = up.FilePath,
                                AddedById = request.DTO._UserEmployeeId,
                                AddedDateTime = DateTime.UtcNow,
                                IsActive = true,
                                HasUploadedPayslip = true
                            });
                        }
                    }
                }

                //--------------------------------------------------------
                // 4️⃣ BULK INSERTS
                //--------------------------------------------------------

                if (detailList.Any())
                    await _unitOfWork.EmployeeExpereinceRepository.AddDetailAsync(detailList);

                if (payslipList.Any())
                    await _unitOfWork.EmployeeExpereinceRepository.AddPayslipAsync(payslipList);



                await _unitOfWork.CommitTransactionAsync();


                //--------------------------------------------------------
                // 5️⃣ FINAL RESPONSE
                //--------------------------------------------------------
                var all = await _unitOfWork.EmployeeExpereinceRepository
                    .GetAllAsync(request.DTO._EmployeeId);

                //var encryptedList = ProjectionHelper.ToGetExperienceResponseDTOs(
                //    all.Items,
                //    _encryptionService,
                //    tenantKey,
                //    request.DTO.EmployeeId);

                //return ApiResponse<List<GetExperienceResponseDTO>>
                //    .Success(encryptedList, "Experience saved successfully");
                return ApiResponse<List<GetExperienceResponseDTO>>
                 .Fail("Error", new List<string> { });
            }
            catch (Exception ex)
            {
              //  await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error while saving experience");

                return ApiResponse<List<GetExperienceResponseDTO>>
                    .Fail("Error", new List<string> { ex.Message });
            }
        }

        // =====================================================================
        // BUILD GAP DETAIL
        ////ExperienceDetails[0].ReasonOfGap:"Money problem"
        //ExperienceDetails[0].GapYearFrom: 2021-09-26
        //ExperienceDetails[0].GapYearTo: 2025-09-26
       // ExperienceDetails[0].IsInfoLatestYear:true
        // =====================================================================
        private async Task<EmployeeExperienceDetail> BuildGapDetail(
            EmployeeExperienceDetailDTO d,
            CreateExperienceInfoCommand request,
            long tenantId,
            long experienceId)
        {
            var detail = new EmployeeExperienceDetail
            {
                EmployeeExperienceId = experienceId,
                EmployeeId = request.DTO._EmployeeId,
                IsAnyGap = true,
                IsInfoLatestYear = d.IsInfoLatestYear ? d.IsInfoLatestYear : false,
                ReasonOfGap = d.ReasonOfGap,
                GapYearFrom = d.GapYearFrom,
                GapYearTo = d.GapYearTo,
                AddedById = request.DTO._UserEmployeeId,
                AddedDateTime = DateTime.UtcNow,
                IsActive = true
            };

            var gapDoc = await UploadDocAsync(new UploadDocRequestDTO
            {
                TenantId = tenantId,
                EmployeeId = request.DTO._EmployeeId,
                File = d.GapCertificateDocument,
                SubFolderName = "gap-docs",
                FilePrefix = "gap",
                FileType = 2
            });

            if (gapDoc.IsUploaded)
            {
                detail.GapCertificateDocName = gapDoc.FileName;
                detail.GapCertificateDocPath = gapDoc.FilePath;
                detail.HasUploadedGapCertificate = true;
            }

            return detail;
        }
        // =====================================================================
        // BUILD EXPERIENCE DETAIL
        // =====================================================================
        private async Task<EmployeeExperienceDetail> BuildExperienceDetail(
      EmployeeExperienceDetailDTO d,
      CreateExperienceInfoCommand request,
      long tenantId,
      long experienceId)
        {
            bool isForeign = d.IsForeignExperience;

            var detail = new EmployeeExperienceDetail
            {
                EmployeeExperienceId = experienceId,
                EmployeeId = request.DTO._EmployeeId,
                CompanyName = d.CompanyName,
                Experience = SafeParser.TryParseInt(d.Experience),
                IsAnyGap = false,
                IsWFH = d.IsWFH,
                WorkingCountryId = SafeParser.TryParseInt(d.WorkingCountryId),
                WorkingStateId = SafeParser.TryParseInt(d.WorkingStateId),
                WorkingDistrictId = SafeParser.TryParseInt(d.WorkingDistrictId),
                HasForeignContractUploaded = false,
                HasImmigrationStampUploaded = false,
                HasWorkPermitUploaded = false,
                HasTaxationDoc = false,
                HasUploadedExperienceLetter = false,
                HasUploadedJoiningLetter = false,
                HasBankStatementUploaded = false,          
                HasVisaUploaded = false,
                EmployeeIdOfCompany = d.EmployeeIdOfCompany,
                ColleagueName = d.ColleagueName,
                ColleagueDesignation = d.ColleagueDesignation,
                ColleagueContactNumber = d.ColleagueContactNumber,
                ReportingManagerName = d.ReportingManagerName,
                ReportingManagerNumber = d.ReportingManagerNumber,
                VerificationEmail = d.VerificationEmail,
                IsInfoLatestYear = d.IsInfoLatestYear?d.IsInfoLatestYear:false ,
                ReasonForLeaving = d.ReasonForLeaving,
                Remark = d.Remark,
                Designation = d.Designation,
                StartDate = d.StartDate,
                EndDate = d.EndDate,
                AddedById = request.DTO._UserEmployeeId,
                AddedDateTime = DateTime.UtcNow,
                IsActive = true,

                IsForeignExperience = isForeign,
                VisaType = isForeign ? d.VisaType : null,
                WorkPermitNumber = isForeign ? d.WorkPermitNumber : null
            };


            // --------------------------------------------------------------------------------
            // STANDARD DOCUMENT UPLOADS (NO REPEAT)
            // --------------------------------------------------------------------------------

            var tax = await UploadFile(d._EmployeeId, tenantId, "tax", d.TaxationDocument);
            detail.TaxationDocFileName = tax.FileName;
            detail.TaxationDocFilePath = tax.FilePath;
            detail.HasTaxationDoc = tax.IsUploaded;


            var exp = await UploadFile(d._EmployeeId, tenantId, "exp-docs", d.ExperienceLetterDocument);
            detail.ExperienceLetterDocName = tax.FileName;
            detail.ExperienceLetterDocPath = tax.FilePath;
            detail.HasUploadedExperienceLetter = tax.IsUploaded;

            var join = await UploadFile(d._EmployeeId, tenantId, "join", d.JoiningLetterDocument);
            detail.JoiningLetterDocName = tax.FileName;
            detail.JoiningLetterDocPath = tax.FilePath;
            detail.HasUploadedJoiningLetter = tax.IsUploaded;

            // --------------------------------------------------------------------------------
            // FOREIGN DOCUMENTS ONLY
            // --------------------------------------------------------------------------------
            if (isForeign)
            {
                var visa = await UploadFile(d._EmployeeId, tenantId, "visa", d.VisaDocument);
                detail.VisaDocName = visa.FileName;
                detail.VisaDocPath = visa.FilePath;
                detail.HasVisaUploaded = visa.IsUploaded;

                var permit = await UploadFile(d._EmployeeId, tenantId, "work-permit", d.WorkPermitDocument);
                detail.WorkPermitDocName = permit.FileName;
                detail.WorkPermitDocPath = permit.FilePath;
                detail.HasWorkPermitUploaded = permit.IsUploaded;


                var immigration = await UploadFile(d._EmployeeId, tenantId, "immigration-stamp", d.ImmigrationStampDocument);
                detail.ImmigrationStampDocName = immigration.FileName;
                detail.ImmigrationStampDocPath = immigration.FilePath;
                detail.HasImmigrationStampUploaded = immigration.IsUploaded;


                var contract = await UploadFile(d._EmployeeId, tenantId, "foreign-contract", d.ForeignContractDocument);
                detail.ForeignContractDocName = contract.FileName;
                detail.ForeignContractDocPath = contract.FilePath;
                detail.HasForeignContractUploaded = contract.IsUploaded;

            }
            else
            {
                // CLEAR FOREIGN FIELDS
                detail.VisaDocName = null;
                detail.VisaDocPath = null;
                detail.WorkPermitDocName = null;
                detail.WorkPermitDocPath = null;
                detail.ImmigrationStampDocName = null;
                detail.ImmigrationStampDocPath = null;
                detail.ForeignContractDocName = null;
                detail.ForeignContractDocPath = null;
 
          }

            return detail;
        }



        // =====================================================================
        // UPLOAD + MAP DOCS
        // =====================================================================
        private async Task<UploadDocResponseDTO> UploadFile( long employeeId, long tenantId, string prefix,  IFormFile file)
        {
            var result = await UploadDocAsync(new UploadDocRequestDTO
            {
                EmployeeId = employeeId,
                FilePrefix = prefix,
                File = file,
                TenantId = tenantId
            });

            return new UploadDocResponseDTO
            {
                FileName = result.FileName,
                FilePath = result.FilePath,
                IsUploaded = result.IsUploaded
            };
        }



        // =====================================================================
        // GENERIC UPLOAD HELPER
        // =====================================================================
        private async Task<UploadDocResponseDTO> UploadDocAsync(UploadDocRequestDTO req)
        {
            var result = new UploadDocResponseDTO();

            if (req.File == null || req.File.Length == 0)
                return result;

            string ext = req.FileType == 1 ? ".pdf" : ".png";

            string fileName = $"{req.FilePrefix}-{req.EmployeeId}-{DateTime.UtcNow:yyyyMMddHHmmssfff}{ext}";

            using var ms = new MemoryStream();
            await req.File.CopyToAsync(ms);
            var bytes = ms.ToArray();

            var folderPath = _fileStorageService.GetEmployeeFolderPath(req.TenantId, req.EmployeeId, req.SubFolderName);
            var fullPath = await _fileStorageService.SaveFileAsync(bytes, fileName, folderPath);

            if (string.IsNullOrEmpty(fullPath))
                return result;

            result.IsUploaded = true;
            result.FileName = fileName;
            result.FilePath = _fileStorageService.GetRelativePath(fullPath);

            return result;
        }
    }




}
