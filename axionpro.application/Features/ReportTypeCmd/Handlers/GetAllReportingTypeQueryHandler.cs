using axionpro.application.DTOs.Manager.ReportingType;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.ReportTypeCmd.Handlers
{
    public class GetAllReportingTypeQuery
        : IRequest<ApiResponse<List<GetReportingTypeResponseDTO>>>
    {
        public GetReportingTypeRequestDTO DTO { get; set; }

        public GetAllReportingTypeQuery(GetReportingTypeRequestDTO dto)
        {
            DTO = dto;
        }
    }

    public class GetAllReportingTypeQueryHandler
        : IRequestHandler<GetAllReportingTypeQuery, ApiResponse<List<GetReportingTypeResponseDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAllReportingTypeQueryHandler> _logger;
        private readonly ICommonRequestService _commonRequestService;

        public GetAllReportingTypeQueryHandler(
            IUnitOfWork unitOfWork,
            ILogger<GetAllReportingTypeQueryHandler> logger,
            ICommonRequestService commonRequestService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _commonRequestService = commonRequestService;
        }

        public async Task<ApiResponse<List<GetReportingTypeResponseDTO>>> Handle(
            GetAllReportingTypeQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("🔹 GetAllReportingType started");

                // ===============================
                // 1️⃣ VALIDATION
                // ===============================
                var validation = await _commonRequestService.ValidateRequestAsync();

                if (!validation.Success)
                    throw new UnauthorizedAccessException(validation.ErrorMessage);

                // ===============================
                // 2️⃣ PERMISSION (MANDATORY)
                // ===============================
                //var hasAccess = await _commonRequestService.HasAccessAsync(
                //    validation.RoleId,
                //    Modules.ReportingType,
                //    Operations.View);

                //if (!hasAccess)
                //    throw new UnauthorizedAccessException("Access denied for view operation.");

                // ===============================
                // 3️⃣ NULL CHECK
                // ===============================
                if (request?.DTO == null)
                    throw new Exception("Invalid request data.");

                request.DTO.Prop ??= new();
                request.DTO.Prop.TenantId = validation.TenantId;

                // ===============================
                // 4️⃣ REPOSITORY CALL (PAGINATION)
                // ===============================
                var result = await _unitOfWork.ReportingTypeRepository
                    .AllAsync(request.DTO);

                if (result == null)
                    throw new Exception("Failed to fetch ReportingTypes.");

                // ===============================
                // 5️⃣ SUCCESS RESPONSE (PAGINATED)
                // ===============================
                return ApiResponse<List<GetReportingTypeResponseDTO>>
                    .SuccessPaginatedOnly(
                        Data: result.Data,
                        PageNumber: result.PageNumber,
                        PageSize: result.PageSize,
                        TotalRecords: result.TotalCount,
                        TotalPages: result.TotalPages,
                        Message: result.Data.Any()
                            ? "ReportingTypes retrieved successfully."
                            : "No ReportingTypes found."
                    );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllReportingType");
                throw; // ✅ middleware handle karega
            }
        }
    }
}