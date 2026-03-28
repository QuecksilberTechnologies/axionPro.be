using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.DTOS.Employee.Experience;
using axionpro.application.DTOS.Pagination;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.EmployeeCmd.ExperienceInfo.Handlers;

public class GetExperienceInfoQuery
    : IRequest<ApiResponse<PagedResponseDTO<GetEmployeeExperienceResponseDTO>>>
{
    public GetExperienceRequestDTO DTO { get; set; }

    public GetExperienceInfoQuery(GetExperienceRequestDTO dto)
    {
        DTO = dto;
    }
}

public class GetExperienceInfoQueryHandler
    : IRequestHandler<GetExperienceInfoQuery, ApiResponse<PagedResponseDTO<GetEmployeeExperienceResponseDTO>>>
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

    public async Task<ApiResponse<PagedResponseDTO<GetEmployeeExperienceResponseDTO>>> Handle(
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

            request.DTO.Prop ??= new();

            request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
            request.DTO.Prop.TenantId = validation.TenantId;

            // ===============================
            // 2️⃣ DECODE
            // ===============================
            request.DTO.Prop.EmployeeId =
                RequestCommonHelper.DecodeOnlyEmployeeId(
                    request.DTO.EmployeeId,
                    validation.Claims.TenantEncriptionKey,
                    _idEncoderService);

            if (request.DTO.Prop.EmployeeId <= 0)
                throw new ValidationErrorException("Invalid EmployeeId.");

            // ===============================
            // 3️⃣ FETCH (REPO CALL)
            // ===============================
            var pagedResult = await _unitOfWork.EmployeeExperienceRepository
                .GetByEmployeeIdWithDocumentsAsync(request.DTO);

            // ===============================
            // 4️⃣ ENCODE EMPLOYEE ID + VALIDATION
            // ===============================
            if (pagedResult?.Items != null && pagedResult.Items.Any())
            {
                foreach (var item in pagedResult.Items)
                {
                    // 🔥 Encode EmployeeId
                    item.EmployeeId = _idEncoderService.EncodeId_long(
                        request.DTO.Prop.EmployeeId,
                        validation.Claims.TenantEncriptionKey);
                }
            }

            // ===============================
            // 4️⃣ RETURN
            // ===============================
            return ApiResponse<PagedResponseDTO<GetEmployeeExperienceResponseDTO>>
                .Success(pagedResult, "Experience list fetched successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ GetExperience failed");
            throw;
        }
    }
}
 




