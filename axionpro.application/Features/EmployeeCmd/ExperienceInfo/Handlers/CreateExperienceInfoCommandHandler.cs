using AutoMapper;
using axionpro.application.Common.Helpers.RequestHelper;
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
    public class CreateExperienceInfoCommand : IRequest<ApiResponse<long>>
    {
        public CreateExperienceRequestDTO DTO { get; set; }

        public CreateExperienceInfoCommand(CreateExperienceRequestDTO dTO)
        {
            DTO = dTO;
        }
    }

    public class CreateExperienceInfoCommandHandler
     : IRequestHandler<CreateExperienceInfoCommand, ApiResponse<long>>
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

        public async Task<ApiResponse<long>> Handle(
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
                  RequestCommonHelper.DecodeOnlyEmployeeId(   request.DTO.EmployeeId,  validation.Claims.TenantEncriptionKey,    _idEncoderService);

                if (request.DTO.Prop.EmployeeId <= 0)
                    throw new ValidationErrorException("Invalid EmployeeId.");
                // ===============================
                // 2️⃣ TRANSACTION START
                // ===============================
                await _unitOfWork.BeginTransactionAsync();

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
                // 4️⃣ DOCUMENTS (DIRECT)
                // ===============================
                if (request.DTO.Documents != null && request.DTO.Documents.Any())
                {
                    exp.EmployeeExperienceDocuments = new List<EmployeeExperienceDocument>();

                    foreach (var docDto in request.DTO.Documents)
                    {
                        var doc = new EmployeeExperienceDocument
                        {
                            DocumentType = docDto.DocumentType,
                            FileName = docDto.FileName,
                            FilePath = docDto.FilePath,

                            IsUploaded = true,

                            AddedById = validation.UserEmployeeId,
                            AddedDateTime = DateTime.UtcNow,
                            IsActive = true,
                            IsSoftDeleted = false
                        };

                        if (!string.IsNullOrWhiteSpace(doc.FilePath))
                            uploadedFiles.Add(doc.FilePath);

                        exp.EmployeeExperienceDocuments.Add(doc);
                    }
                }

                // ===============================
                // 5️⃣ SAVE (SINGLE)
                // ===============================
             

                // ADD
                await _unitOfWork.EmployeeExperienceRepository.AddAsync(exp);

                // SAVE (single point 🔥)
                await _unitOfWork.SaveChangesAsync();

                // GET ID
                var id = exp.Id;

                // COMMIT
 
                _logger.LogInformation("✅ Experience created successfully");

                return ApiResponse<long>.Success(id, "Experience saved successfully.");
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
