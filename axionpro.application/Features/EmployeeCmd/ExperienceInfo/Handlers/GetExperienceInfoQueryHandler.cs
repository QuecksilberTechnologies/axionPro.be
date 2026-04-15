using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.DTOS.Employee.Experience;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Wrappers;
using MediatR;
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
            _logger.LogInformation("🚀 START GetExperience Handler");

            // ===============================
            // 1️⃣ VALIDATION
            // ===============================
            var validation = await _commonRequestService.ValidateRequestAsync();

            if (!validation.Success)
            {
                _logger.LogWarning("❌ Validation failed");
                throw new UnauthorizedAccessException(validation.ErrorMessage);
            }

            if (request?.DTO == null)
            {
                _logger.LogWarning("❌ Request DTO is null");
                throw new ValidationErrorException("Invalid request");
            }

            if (string.IsNullOrWhiteSpace(request.DTO.EmployeeId))
            {
                _logger.LogWarning("❌ EmployeeId is missing");
                throw new ValidationErrorException("EmployeeId is required");
            }

            request.DTO.Prop ??= new();

            request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
            request.DTO.Prop.TenantId = validation.TenantId;

            _logger.LogInformation("✅ Validation Passed | UserId: {UserId}", validation.UserEmployeeId);

            // ===============================
            // 2️⃣ DECODE
            // ===============================
            _logger.LogInformation("🔓 Decoding EmployeeId: {EncodedId}", request.DTO.EmployeeId);

            request.DTO.Prop.EmployeeId =
                RequestCommonHelper.DecodeOnlyEmployeeId(
                    request.DTO.EmployeeId,
                    validation.Claims.TenantEncriptionKey,
                    _idEncoderService);

            _logger.LogInformation("🔑 Decoded EmployeeId: {DecodedId}", request.DTO.Prop.EmployeeId);

            if (request.DTO.Prop.EmployeeId <= 0)
            {
                _logger.LogError("❌ Invalid decoded EmployeeId");
                throw new ValidationErrorException("Invalid EmployeeId.");
            }

            // ===============================
            // 3️⃣ FETCH
            // ===============================
            _logger.LogInformation("📥 Fetching experience data...");

            var expEntities =
                await _unitOfWork.EmployeeExperienceRepository
                    .GetByEmployeeIdWithDocumentsAsync(request.DTO);

            _logger.LogInformation("📦 Data fetched | Count: {Count}",
                expEntities?.Data?.Count ?? 0);

            // ===============================
            // 4️⃣ EMPTY LIST = SUCCESS
            // ===============================
            if (expEntities == null || expEntities.Data == null || !expEntities.Data.Any())
            {
                _logger.LogWarning("⚠️ No experience records found");

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

            // ===============================
            // 5️⃣ ENCODE IDS (FIXED 🔥)
            // ===============================
            foreach (var item in expEntities.Data)
            {
                if (!string.IsNullOrWhiteSpace(item.EmployeeId) &&
                    long.TryParse(item.EmployeeId, out var empId))
                {
                    item.EmployeeId = _idEncoderService.EncodeId_long(
                        empId,
                        validation.Claims.TenantEncriptionKey);
                }
                else
                {
                    _logger.LogWarning("⚠️ Skipping encoding | Invalid EmployeeId in item");
                }
            }

            _logger.LogInformation("🔐 EmployeeId encoding completed");

            // ===============================
            // 6️⃣ SUCCESS RESPONSE
            // ===============================
            _logger.LogInformation("✅ END GetExperience SUCCESS");

            return ApiResponse<List<GetEmployeeExperienceResponseDTO>>
                .SuccessPaginatedPercentage(
                    Data: expEntities.Data,
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
            _logger.LogError(ex, "❌ ERROR GetExperience Handler");
            throw;
        }
    }
}
 




