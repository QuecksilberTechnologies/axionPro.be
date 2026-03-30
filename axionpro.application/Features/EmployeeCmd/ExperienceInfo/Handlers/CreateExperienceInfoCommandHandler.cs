using AutoMapper;
using axionpro.application.Common.Helpers.PercentageHelper;
using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.Constants;
using axionpro.application.DTOS.Employee.Experience;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
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


namespace axionpro.application.Features.EmployeeCmd.ExperienceInfo.Handlers
{
    public class CreateExperienceInfoCommand : IRequest<ApiResponse<GetEmployeeExperienceResponseDTO>>
    {
        public CreateExperienceRequestDTO DTO { get; set; }

        public CreateExperienceInfoCommand(CreateExperienceRequestDTO dTO)
        {
            DTO = dTO;
        }
    }

    public class CreateExperienceInfoCommandHandler
     : IRequestHandler<CreateExperienceInfoCommand, ApiResponse<GetEmployeeExperienceResponseDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateExperienceInfoCommand> _logger;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IFileStorageService _fileStorageService;
        private readonly IIdEncoderService _idEncoderService;
        public CreateExperienceInfoCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<CreateExperienceInfoCommand> logger,
            ICommonRequestService commonRequestService,
            IFileStorageService fileStorageService, IIdEncoderService idEncoderService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _commonRequestService = commonRequestService;
            _fileStorageService = fileStorageService;
            _idEncoderService =idEncoderService;
        }

        public async Task<ApiResponse<GetEmployeeExperienceResponseDTO>> Handle(
            CreateExperienceInfoCommand request,
            CancellationToken cancellationToken)
        {
            List<string> uploadedFiles = new();

            try
            {
                _logger.LogInformation("🚀 CreateExperience started");

                // ===============================
                // 1️⃣ VALIDATION
                // ===============================
                var validation = await _commonRequestService.ValidateRequestAsync();

                if (!validation.Success)
                    throw new UnauthorizedAccessException(validation.ErrorMessage);

                if (request?.DTO == null)
                    throw new ValidationErrorException("Invalid request");

                request.DTO.Prop.EmployeeId =
                  RequestCommonHelper.DecodeOnlyEmployeeId(request.DTO.EmployeeId, validation.Claims.TenantEncriptionKey, _idEncoderService);

                if (request.DTO.Prop.EmployeeId <= 0)
                    throw new ValidationErrorException("Invalid EmployeeId.");
                // ===============================
                // 2️⃣ TRANSACTION START
                // ===============================
                await _unitOfWork.BeginTransactionAsync();
                // ===============================
                // 5️⃣ FILE UPLOAD
                // ===============================
              

                // ===============================
                // 3️⃣ CREATE EXPERIENCE
                // ===============================
                var exp = new EmployeeExperience
                {
                    EmployeeId = request.DTO.Prop.EmployeeId,
                    CompanyName = request.DTO.CompanyName,
                    Designation = request.DTO.Designation,
                    EmployeeIdOfCompany = request.DTO.EmployeeIdOfCompany,
                    Ctc = request.DTO.Ctc,
                    StartDate = request.DTO.StartDate,
                    EndDate = request.DTO.EndDate,
                    Experience = request.DTO.Experience,
                    IsWFH = request.DTO.IsWFH,
                    WorkingCountryId = request.DTO.WorkingCountryId,
                    WorkingStateId = request.DTO.WorkingStateId,
                    WorkingDistrictId = request.DTO.WorkingDistrictId,
                    IsForeignExperience = request.DTO.IsForeignExperience,
                    ReasonForLeaving = request.DTO.ReasonForLeaving,
                    Remark = request.DTO.Remark,
                    ColleagueName = request.DTO.ColleagueName,
                    ColleagueDesignation = request.DTO.ColleagueDesignation,
                    ColleagueContactNumber = request.DTO.ColleagueContactNumber,

                    ReportingManagerName = request.DTO.ReportingManagerName,
                    ReportingManagerNumber = request.DTO.ReportingManagerNumber,

                    VerificationEmail = request.DTO.VerificationEmail,

                    IsAnyGap = request.DTO.IsAnyGap,
                    ReasonOfGap = request.DTO.ReasonOfGap,
                    GapYearFrom = request.DTO.GapYearFrom,
                    GapYearTo = request.DTO.GapYearTo,

                    IsExperienceVerified = false,
                    IsExperienceVerifiedByMail = false,
                    IsExperienceVerifiedByCall = false,

                    InfoVerifiedById = null,
                    InfoVerifiedDateTime = null,
                    IsInfoVerified = false,
                    IsEditAllowed = true,
                    AddedById = validation.UserEmployeeId,
                    AddedDateTime = DateTime.UtcNow,
                    IsActive = true,
                    IsSoftDeleted = false
                };

                
                     

                // ===============================
                // 4️⃣ DOCUMENTS (UPLOAD LIKE EDUCATION 🔥)
                // ===============================
                if (request.DTO.Documents != null && request.DTO.Documents.Any())
                {
                    exp.EmployeeExperienceDocuments = new List<EmployeeExperienceDocument>();

                    foreach (var docDto in request.DTO.Documents)
                    {
                        string? docPath = null;
                        string? fileName = null;
                        bool hasUploaded = false;

                        if (docDto.File != null && docDto.File.Length > 0)
                        {
                            try
                            {
                                fileName =
                                    $"experience-{request.DTO.Prop.EmployeeId}-{DateTime.UtcNow:yyyyMMddHHmmss}";

                                string folderPath = $"{ConstantValues.TenantFolder}-{request.DTO.Prop.TenantId}/" +
                                    $"{ConstantValues.EmployeeFolder}/{request.DTO.Prop.EmployeeId}/" +
                                       $"{ConstantValues.ExperienceFolder}";
                      

                                docPath = await _fileStorageService.UploadFileAsync(
                                    docDto.File,   
                                    folderPath,
                                    fileName);

                                if (!string.IsNullOrWhiteSpace(docPath))
                                {
                                    hasUploaded = true;
                                    uploadedFiles.Add(docPath); // rollback support
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Experience document upload failed");
                                throw new ApiException("File upload failed.", 500);
                            }
                        }

                        var doc = new EmployeeExperienceDocument
                        {
                            DocumentType = docDto.DocumentType,

                            FileName = fileName,
                            FilePath = docPath,

                            HasExperienceDocUploaded = hasUploaded,

                            AddedById = validation.UserEmployeeId,
                            AddedDateTime = DateTime.UtcNow,
                            IsActive = true,
                            IsSoftDeleted = false
                        };

                        exp.EmployeeExperienceDocuments.Add(doc);
                    }
                }

                // ===============================
                // 4️⃣ ADD
                // ===============================

                await _unitOfWork.EmployeeExperienceRepository.AddAsync(exp);

                // ===============================
                // SAVE (ONLY HERE 🔥)
                // ===============================
                var rows = await   _unitOfWork.SaveChangesAsync(cancellationToken);

                if (rows <= 0 || exp.Id <= 0)
                {
                    _logger.LogError("❌ CreateExperience failed | SaveChanges failed");
                    throw new ApiException("Failed to save experience.", 500);
                }

                // ===============================
                // COMMIT
                // ===============================
                await _unitOfWork.CommitTransactionAsync();

                // ===============================
                // MAP (NOW SAFE)
                // ===============================
                var response = new GetEmployeeExperienceResponseDTO
                {
                    Id = exp.Id,
                    EmployeeId = exp.EmployeeId.ToString(),
                    CompanyName = exp.CompanyName,
                    Designation = exp.Designation,
                    EmployeeIdOfCompany = exp.EmployeeIdOfCompany,
                    Ctc = exp.Ctc,
                    StartDate = exp.StartDate,
                    EndDate = exp.EndDate,
                    Experience = exp.Experience,
                    IsWFH = exp.IsWFH,
                    WorkingCountryId = exp.WorkingCountryId,
                    WorkingStateId = exp.WorkingStateId,
                    WorkingDistrictId = exp.WorkingDistrictId,
                    IsForeignExperience = exp.IsForeignExperience,
                    ReasonForLeaving = exp.ReasonForLeaving,
                    Remark = exp.Remark,
                    ColleagueName = exp.ColleagueName,
                    ColleagueDesignation = exp.ColleagueDesignation,
                    ColleagueContactNumber = exp.ColleagueContactNumber,
                    ReportingManagerName = exp.ReportingManagerName,
                    ReportingManagerNumber = exp.ReportingManagerNumber,
                    VerificationEmail = exp.VerificationEmail,
                    IsAnyGap = exp.IsAnyGap,
                    ReasonOfGap = exp.ReasonOfGap,
                    GapYearFrom = exp.GapYearFrom,
                    GapYearTo = exp.GapYearTo,
                    IsEditAllowed = exp.IsEditAllowed,
                    IsInfoVerified = exp.IsInfoVerified,
                    Documents = new List<GetEmployeeExperienceDocumentDTO>()
                };

                // Completion
                response.CompletionPercentage =
                    CompletionCalculatorHelper.ExperiencePropCalculate(response);

                _logger.LogInformation("✅ Experience created successfully | Id: {Id}", exp.Id);

                return ApiResponse<GetEmployeeExperienceResponseDTO>
                    .Success(response, "Experience saved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ CreateExperience failed");

                await _unitOfWork.RollbackTransactionAsync();

                // 🔥 FILE ROLLBACK

                foreach (var file in uploadedFiles)
                {
                    try { await _fileStorageService.DeleteFileAsync(file); }
                    catch { }
                }

                throw;
            }
        }
    }


}
