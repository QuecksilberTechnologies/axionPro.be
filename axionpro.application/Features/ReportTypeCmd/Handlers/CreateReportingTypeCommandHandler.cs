// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines and handles Create Reporting Type Command Handler requests.
// ================================================================

using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Constants;
using axionpro.application.DTOs.Manager.ReportingType;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.ReportTypeCmd.Handlers
{

    #region Command

    //  Command
    /// <summary>
    /// Represents the CreateReportingTypeCommand application component.
    /// </summary>
    public class CreateReportingTypeCommand : IRequest<ApiResponse<GetReportingTypeResponseDTO>>
    {
        public CreateReportingTypeRequestDTO DTO { get; set; }

        public CreateReportingTypeCommand(CreateReportingTypeRequestDTO dto)
        {
            DTO = dto;
        }
    }

    //  Handler
    /// <summary>
    /// Handles CreateReportingTypeCommand requests.
    /// </summary>
        #endregion

    #region Handler

public class CreateReportingTypeCommandHandler
        : IRequestHandler<CreateReportingTypeCommand, ApiResponse<GetReportingTypeResponseDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateReportingTypeCommandHandler> _logger;
        private readonly ICommonRequestService _commonRequestService;

        public CreateReportingTypeCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<CreateReportingTypeCommandHandler> logger,
            ICommonRequestService commonRequestService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _commonRequestService = commonRequestService;
        }

        public async Task<ApiResponse<GetReportingTypeResponseDTO>> Handle(
            CreateReportingTypeCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("🔹 CreateReportingType started");

                #region Tenant Request Validation

                var validation = await _commonRequestService.ValidateTenantUserRequestAsync();
                if (!validation.Success)
                {
                    throw new UnauthorizedAccessException(
                        validation.ErrorMessage ?? AppConstants.ErrorMessages.Unauthorized);
                }

                #endregion

                #region Trusted Request Context

                long userEmployeeId = validation.LoggedInEmployeeId;
                long tenantId = validation.TenantId;
                int tokenRoleId = validation.RoleId;

                if (userEmployeeId <= 0 || tenantId <= 0 || tokenRoleId <= 0)
                {
                    _logger.LogWarning(
                        "Invalid Tenant authorization context while creating Reporting Type. TenantId: {TenantId}, EmployeeId: {EmployeeId}, TokenRoleId: {TokenRoleId}",
                        tenantId, userEmployeeId, tokenRoleId);
                    throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized);
                }

                #endregion

                #region Runtime Permission Validation

                var permissionResult = await _unitOfWork.StoreProcedureRepository
                    .CheckTenantEmployeePermissionAsync(
                        tenantId,
                        userEmployeeId,
                        tokenRoleId,
                        request.DTO.ModuleId,
                        request.DTO.OperationId,
                        cancellationToken);

                TenantRuntimePermissionValidator.EnsureAllowed(permissionResult);

                #endregion

                // ===============================
                // 2️⃣ NULL CHECK
                // ===============================
                if (request?.DTO == null)
                    throw new ValidationErrorException("Invalid request data.");

                // ===============================
                // 3️⃣ REPOSITORY CALL
                // ===============================
                var entity = _mapper.Map<axionpro.domain.Entity.ReportingType>(request.DTO);
                entity.TenantId = tenantId;
                entity.AddedById = userEmployeeId;
                entity.AddedDateTime = DateTime.UtcNow;
                entity.IsSoftDeleted = false;

                var created = await _unitOfWork.ReportingTypeRepository.AddAsync(entity);

                if (created == null)
                    throw new Exception("ReportingType creation failed.");

                // ===============================
                // 4️⃣ SUCCESS
                // ===============================
                return ApiResponse<GetReportingTypeResponseDTO>
                    .Success(_mapper.Map<GetReportingTypeResponseDTO>(created), "ReportingType created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateReportingType");
                throw; //  middleware handle karega
            }
        }
    }
    #endregion
}
