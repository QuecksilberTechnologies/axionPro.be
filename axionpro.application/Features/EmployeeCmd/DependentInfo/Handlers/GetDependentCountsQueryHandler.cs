using axionpro.application.DTOS.Employee.Dependent;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IFileStorage;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.EmployeeCmd.DependentInfo.Handlers
{
    // ======================================================
    // 🔹 COMMAND
    // ======================================================
    public class GetDependentCountsQuery
        : IRequest<ApiResponse<GetDependentsDetailResponseDTO>>
    {
        public GetDependentRequestDTO DTO { get; set; }

        public GetDependentCountsQuery(GetDependentRequestDTO dto)
        {
            DTO = dto;
        }
    }

    // ======================================================
    // 🔹 HANDLER
    // ======================================================
    public class GetDependentCountsQueryHandler
        : IRequestHandler<GetDependentCountsQuery, ApiResponse<GetDependentsDetailResponseDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetDependentCountsQueryHandler> _logger;
        private readonly IPermissionService _permissionService;
        private readonly IIdEncoderService _idEncoderService;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IFileStorageService _fileStorageService;

        public GetDependentCountsQueryHandler(
            IUnitOfWork unitOfWork,
            ILogger<GetDependentCountsQueryHandler> logger,
            IPermissionService permissionService,
            IIdEncoderService idEncoderService,
            ICommonRequestService commonRequestService,
            IFileStorageService fileStorageService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _permissionService = permissionService;
            _idEncoderService = idEncoderService;
            _commonRequestService = commonRequestService;
            _fileStorageService = fileStorageService;
        }

        public async Task<ApiResponse<GetDependentsDetailResponseDTO>> Handle(
            GetDependentCountsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("🔹 GetDependentCounts started");

                // ===============================
                // 🔐 STEP 1: COMMON VALIDATION
                // ===============================
                var validation = await _commonRequestService.ValidateRequestAsync();

                if (!validation.Success)
                    throw new UnauthorizedAccessException(validation.ErrorMessage);

                // ===============================
                // 🔓 STEP 2: DECODE EMPLOYEE ID
                // ===============================
                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;
                request.DTO.Prop.EmployeeId = validation.LoggedInEmployeeId;



                // ===============================
                // 🔑 STEP 3: PERMISSION CHECK (OPTIONAL)
                // ===============================
                // bool hasAccess = await _permissionService.HasAccessAsync(
                //     validation.RoleId,
                //     Modules.Employee,
                //     Operations.View);

                // if (!hasAccess)
                //     throw new UnauthorizedAccessException("Access denied.");

                // ===============================
                // 📦 STEP 4: REPOSITORY CALL
                // ===============================
                var result = await _unitOfWork
                    .EmployeeDependentRepository
                    .GetDetailInfo(request.DTO);

                if (result == null)
                    result = new GetDependentsDetailResponseDTO();

                // ===============================
                // ✅ STEP 5: COMMIT
                // ===============================
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("✅ GetDependentCounts success");

                // ===============================
                // 📤 STEP 6: RESPONSE
                // ===============================
                return ApiResponse<GetDependentsDetailResponseDTO>
                    .Success(result, "Dependent details fetched successfully.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();

                _logger.LogError(
                    ex,
                    "❌ Error while fetching dependent details. EmployeeId: {EmployeeId}",
                    request.DTO?.EmployeeId);

                throw; // 🔥 As per your architecture rules
            }
        }
    }
}