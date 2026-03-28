using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.DTOS.Employee.Experience;
using axionpro.application.DTOS.Pagination;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.EmployeeCmd.ExperienceInfo.Handlers;

public class GetExperienceInfoQuery
    : IRequest<ApiResponse<List<GetEmployeeExperienceResponseDTO>>>
{
    public GetExperienceRequestDTO DTO { get; set; }

    public GetExperienceInfoQuery(GetExperienceRequestDTO dto)
    {
        DTO = dto;
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

            // ===============================
            // 4️⃣ FETCH
            // ===============================
            var expEntities =
                await _unitOfWork.EmployeeExperienceRepository
                    .GetByEmployeeIdWithDocumentsAsync(request.DTO);

            // ===============================
            // 5️⃣ EMPTY LIST = SUCCESS
            // ===============================
            if (expEntities == null || expEntities.Items == null || !expEntities.Items.Any())
            {
                return ApiResponse<List<GetEmployeeExperienceResponseDTO>>
                    .SuccessPaginatedPercentage(
                        Data: new List<GetEmployeeExperienceResponseDTO>(),
                        Message: "No experience info found.",
                        PageNumber: expEntities?.PageNumber ?? 1,
                        PageSize: expEntities?.PageSize ?? 0,
                        TotalRecords: expEntities?.TotalCount ?? 0,
                        TotalPages: expEntities?.TotalPages ?? 0,
                        HasUploadedAll: expEntities?.HasUploadedAll ?? false,
                        CompletionPercentage: expEntities?.CompletionPercentage ?? 0
                    );
            }

            if (expEntities?.Items != null && expEntities.Items.Any())
            {
                foreach (var item in expEntities.Items)
                {
                    // 🔥 Encode EmployeeId
                    item.EmployeeId = _idEncoderService.EncodeId_long(
                        request.DTO.Prop.EmployeeId,
                        validation.Claims.TenantEncriptionKey);
                }
            }

            // ===============================
            // 7️⃣ SUCCESS RESPONSE
            // ===============================
            _logger.LogInformation("GetExperienceInfo success");

            return ApiResponse<List<GetEmployeeExperienceResponseDTO>>
                .SuccessPaginatedPercentage(
                    Data: expEntities.Items,
                    Message: "Experience info retrieved successfully.",
                    PageNumber: expEntities.PageNumber,
                    PageSize: expEntities.PageSize,
                    TotalRecords: expEntities.TotalCount,
                    TotalPages: expEntities.TotalPages,
                    HasUploadedAll: expEntities.HasUploadedAll,
                    CompletionPercentage: expEntities.CompletionPercentage
                );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ GetExperience failed");
            throw;
        }
    }
}
 




