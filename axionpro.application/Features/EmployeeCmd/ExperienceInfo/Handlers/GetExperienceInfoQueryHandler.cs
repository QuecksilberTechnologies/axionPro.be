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


namespace axionpro.application.Features.EmployeeCmd.ExperienceInfo.Handlers;   

public class GetExperienceInfoQuery : IRequest<ApiResponse<List<GetEmployeeExperienceResponseDTO>>>
{
    public GetExperienceRequestDTO DTO { get; set; }

    public GetExperienceInfoQuery(GetExperienceRequestDTO dTO)
    {
        DTO = dTO;
    }
}

public class GetExperienceInfoQueryHandler
   : IRequestHandler<GetExperienceInfoQuery, ApiResponse<List<GetEmployeeExperienceResponseDTO>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetExperienceInfoQuery> _logger;
    private readonly IIdEncoderService _idEncoderService;
    private readonly ICommonRequestService _commonRequestService;

    public GetExperienceInfoQueryHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetExperienceInfoQuery> logger,
        IIdEncoderService idEncoderService,
        ICommonRequestService commonRequestService)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _idEncoderService = idEncoderService;
        _commonRequestService = commonRequestService;
    }

    public async Task<ApiResponse<List<GetEmployeeExperienceResponseDTO>>> Handle(
        GetExperienceInfoQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("🚀 GetExperience started");

            // ===============================
            // 1️⃣ VALIDATION
            // ===============================
            var validation = await _commonRequestService.ValidateRequestAsync();

            if (!validation.Success)
                throw new UnauthorizedAccessException(validation.ErrorMessage);

            if (request?.DTO == null)
                throw new ValidationErrorException("Invalid request");

            if (string.IsNullOrWhiteSpace(request.DTO.EmployeeId))
                throw new ValidationErrorException("EmployeeId is required");

            if (request.DTO.Prop == null)
                request.DTO.Prop = new();

            request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
            request.DTO.Prop.TenantId = validation.TenantId;

            // ===============================
            // 2️⃣ DECODE (UNCHANGED)
            // ===============================
            request.DTO.Prop.EmployeeId =
                RequestCommonHelper.DecodeOnlyEmployeeId(
                    request.DTO.EmployeeId,
                    validation.Claims.TenantEncriptionKey,
                    _idEncoderService);

            if (request.DTO.Prop.EmployeeId <= 0)
                throw new ValidationErrorException("Invalid EmployeeId.");

            // ===============================
            // 3️⃣ FETCH LIST 🔥
            // ===============================
            var entities = await _unitOfWork.EmployeeExperienceRepository
                .GetByEmployeeIdWithDocumentsAsync(request.DTO.Prop.EmployeeId);

            if (entities == null || !entities.Any())
            {
                return ApiResponse<List<GetEmployeeExperienceResponseDTO>>
                    .Success(new List<GetEmployeeExperienceResponseDTO>(), "No experience found.");
            }

            // ===============================
            // 4️⃣ MAP LIST 🔥
            // ===============================
            var response = entities.Select(entity => new GetEmployeeExperienceResponseDTO
            {
                Id = entity.Id,

                EmployeeId = _idEncoderService.EncodeId_long(
                    entity.EmployeeId,
                    validation.Claims.TenantEncriptionKey),

                Ctc = entity.Ctc,
                CompanyName = entity.CompanyName,
                Designation = entity.Designation,
                EmployeeIdOfCompany = entity.EmployeeIdOfCompany,

                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                Experience = entity.Experience,
                IsWFH = entity.IsWFH,

                WorkingCountryId = entity.WorkingCountryId,
                WorkingStateId = entity.WorkingStateId,
                WorkingDistrictId = entity.WorkingDistrictId,
                IsForeignExperience = entity.IsForeignExperience,

                ReasonForLeaving = entity.ReasonForLeaving,
                Remark = entity.Remark,

                ColleagueName = entity.ColleagueName,
                ColleagueDesignation = entity.ColleagueDesignation,
                ColleagueContactNumber = entity.ColleagueContactNumber,

                ReportingManagerName = entity.ReportingManagerName,
                ReportingManagerNumber = entity.ReportingManagerNumber,
                VerificationEmail = entity.VerificationEmail,

                IsAnyGap = entity.IsAnyGap,
                ReasonOfGap = entity.ReasonOfGap,
                GapYearFrom = entity.GapYearFrom,
                GapYearTo = entity.GapYearTo,

                IsExperienceVerified = entity.IsExperienceVerified,
                IsExperienceVerifiedByMail = entity.IsExperienceVerifiedByMail,
                IsExperienceVerifiedByCall = entity.IsExperienceVerifiedByCall,

                Documents = entity.EmployeeExperienceDocuments?
                    .Select(doc => new GetEmployeeExperienceDocumentDTO
                    {
                        DocumentType = doc.DocumentType,
                        FileName = doc.FileName,
                        FilePath = doc.FilePath,
                        Remark = doc.Remark
                    }).ToList()

            }).ToList();

            // ===============================
            // 5️⃣ RETURN
            // ===============================
            return ApiResponse<List<GetEmployeeExperienceResponseDTO>>
                .Success(response, "Experience list fetched successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ GetExperience failed");
            throw;
        }
    }
}




