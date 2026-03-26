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

public class GetExperienceInfoQuery : IRequest<ApiResponse<GetEmployeeExperienceResponseDTO>>
{
    public GetExperienceRequestDTO DTO { get; set; }

    public GetExperienceInfoQuery(GetExperienceRequestDTO dTO)
    {
        DTO = dTO;
    }
}

public class GetExperienceInfoQueryHandler
 : IRequestHandler<GetExperienceInfoQuery, ApiResponse<GetEmployeeExperienceResponseDTO>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<GetExperienceInfoQuery> _logger;
    private readonly ITokenService _tokenService;
    private readonly IPermissionService _permissionService;
    private readonly IConfiguration _config;
    private readonly IEncryptionService _encryptionService;
    private readonly IIdEncoderService _idEncoderService;
    private readonly IFileStorageService _fileStorageService;
    private readonly ICommonRequestService _commonRequestService;
    public GetExperienceInfoQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor,
        ILogger<GetExperienceInfoQuery> logger,
        ITokenService tokenService,
        IPermissionService permissionService,
        IConfiguration config,
        IEncryptionService encryptionService,
        IIdEncoderService idEncoderService,
        IFileStorageService fileStorageService, ICommonRequestService commonRequestService

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
        _commonRequestService = commonRequestService;

    }
    public async Task<ApiResponse<GetEmployeeExperienceResponseDTO>> Handle(
GetExperienceInfoQuery request,
CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("🚀 GetExperience started");

            // ===============================
            // 1️⃣ COMMON VALIDATION
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
            // 2️⃣ DECODE EMPLOYEE ID
            // ===============================
            request.DTO.Prop.EmployeeId =
               RequestCommonHelper.DecodeOnlyEmployeeId(
                   request.DTO.EmployeeId,
                   validation.Claims.TenantEncriptionKey,
                   _idEncoderService);

            if (request.DTO.Prop.EmployeeId <= 0)
                throw new ValidationErrorException("Invalid EmployeeId.");

            // ===============================
            // 3️⃣ FETCH FULL DATA (PARENT + CHILD + DOCS)
            // ===============================
            var entity = await _unitOfWork.EmployeeExperienceRepository.GetByEmployeeIdWithDetailsAsync(request.DTO.Prop.EmployeeId);

            if (entity == null)
            {
                return ApiResponse<GetEmployeeExperienceResponseDTO>
                    .Success(null, "No experience record found.");
            }

            // ===============================
            // 4️⃣ MAP TO DTO
            // ===============================
            var response = new GetEmployeeExperienceResponseDTO
            {
                Id = entity.Id,
                EmployeeId = _idEncoderService.EncodeId_long(entity.Id, validation.Claims.TenantEncriptionKey),

                Ctc = entity.Ctc,
                Comment = entity.Comment,
                HasEPFAccount = entity.HasEPFAccount,
                IsFresher = entity.IsFresher,

                ExperienceDetails = entity.EmployeeExperienceDetails?
                    .Select(d => new GetEmployeeExperienceDetailDTO
                    {
                        CompanyName = d.CompanyName,
                        Designation = d.Designation,
                        EmployeeIdOfCompany = d.EmployeeIdOfCompany,

                        StartDate = d.StartDate,
                        EndDate = d.EndDate,
                        Experience = d.Experience,
                        IsWFH = d.IsWFH,

                        WorkingCountryId = d.WorkingCountryId,
                        WorkingStateId = d.WorkingStateId,
                        WorkingDistrictId = d.WorkingDistrictId,
                        IsForeignExperience = d.IsForeignExperience,

                        ReasonForLeaving = d.ReasonForLeaving,
                        Remark = d.Remark,

                        ColleagueName = d.ColleagueName,
                        ColleagueDesignation = d.ColleagueDesignation,
                        ColleagueContactNumber = d.ColleagueContactNumber,

                        ReportingManagerName = d.ReportingManagerName,
                        ReportingManagerNumber = d.ReportingManagerNumber,
                        VerificationEmail = d.VerificationEmail,

                        IsAnyGap = d.IsAnyGap,
                        ReasonOfGap = d.ReasonOfGap,
                        GapYearFrom = d.GapYearFrom,
                        GapYearTo = d.GapYearTo,

                        Documents = d.EmployeeExperienceDocuments?
                            .Select(doc => new GetEmployeeExperienceDocumentDTO
                            {
                                DocumentType = doc.DocumentType,
                                FileName = doc.FileName,
                                FilePath = doc.FilePath,
                                Remark = doc.Remark
                            }).ToList()
                    }).ToList()
            };

            // ===============================
            // 5️⃣ RETURN
            // ===============================
            return ApiResponse<GetEmployeeExperienceResponseDTO>
                .Success(response, "Experience fetched successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ GetExperience failed");

            throw;
        }
    }


}





