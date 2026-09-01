// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Creates tenant-scoped designations using trusted request context.
// ================================================================

using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Constants;
using axionpro.application.DTOs.Designation;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.DesignationCmd.Handlers
{
    #region Command

    /// <summary>
    /// Represents a request to create a designation.
    /// </summary>
    public class CreateDesignationCommand : IRequest<ApiResponse<List<GetDesignationResponseDTO>>>
    {
        public CreateDesignationRequestDTO DTO { get; }

        /// <summary>
        /// Initializes the create request with client-editable values.
        /// </summary>
        public CreateDesignationCommand(CreateDesignationRequestDTO dto)
        {
            DTO = dto;
        }
    }

    #endregion

    #region Handler

    /// <summary>
    /// Handles creation of designations for the authenticated tenant.
    /// </summary>
    public class CreateDesignationCommandHandler : IRequestHandler<CreateDesignationCommand, ApiResponse<List<GetDesignationResponseDTO>>>
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICommonRequestService _commonRequestService;
        private readonly ILogger<CreateDesignationCommandHandler> _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes the handler dependencies.
        /// </summary>
        public CreateDesignationCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ICommonRequestService commonRequestService,
            ILogger<CreateDesignationCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _commonRequestService = commonRequestService;
            _logger = logger;
        }

        #endregion

        #region Handle

        /// <summary>
        /// Creates a designation from client-editable values and trusted tenant audit context.
        /// </summary>
        public async Task<ApiResponse<List<GetDesignationResponseDTO>>> Handle(
            CreateDesignationCommand request,
            CancellationToken cancellationToken)
        {
            #region Tenant Request Validation

            var validation = await _commonRequestService.ValidateTenantUserRequestAsync();
            if (!validation.Success)
            {
                throw new UnauthorizedAccessException(
                    validation.ErrorMessage ??
                    AppConstants.ErrorMessages.Unauthorized);
            }

            #endregion

            #region Trusted Request Context

            long userEmployeeId = validation.LoggedInEmployeeId;
            long tenantId = validation.TenantId;
            int tokenRoleId = validation.RoleId;

            if (userEmployeeId <= 0 || tenantId <= 0 || tokenRoleId <= 0)
            {
                _logger.LogWarning(
                    "Invalid Tenant authorization context while creating Designation. TenantId: {TenantId}, EmployeeId: {EmployeeId}, TokenRoleId: {TokenRoleId}",
                    tenantId,
                    userEmployeeId,
                    tokenRoleId);

                throw new UnauthorizedAccessException(
                    AppConstants.ErrorMessages.Unauthorized);
            }

            #endregion

            #region Runtime Permission Validation

            // Current database role assignments are authoritative so a stale
            // JWT role cannot authorize a designation create operation.
            var permissionResult =
                await _unitOfWork.StoreProcedureRepository
                    .CheckTenantEmployeePermissionAsync(
                        tenantId,
                        userEmployeeId,
                        tokenRoleId,
                        request.DTO.ModuleId,
                        request.DTO.OperationId,
                        cancellationToken);

            TenantRuntimePermissionValidator.EnsureAllowed(permissionResult);

            #endregion

            if (request.DTO.DepartmentId <= 0)
                throw new ValidationErrorException(
                    AppConstants.ErrorMessages.InvalidIdentifier);

            var designationName = request.DTO.DesignationName?.Trim();
            if (string.IsNullOrWhiteSpace(designationName))
                throw new ValidationErrorException(
                    AppConstants.ErrorMessages.InvalidRequest);

            if (await _unitOfWork.DesignationRepository.CheckDuplicateValueAsync(tenantId, designationName))
                throw new ConflictException(
                    AppConstants.ErrorMessages.ResourceConflict);

            // Map client-editable values to the domain entity.
            var entity = _mapper.Map<Designation>(request.DTO);
            entity.DesignationName = designationName;
            entity.TenantId = tenantId;
            entity.AddedById = userEmployeeId;
            entity.AddedDateTime = DateTime.UtcNow;
            entity.IsSoftDeleted = false;

            // Persist the prepared domain entity.
            var created = await _unitOfWork.DesignationRepository.CreateAsync(entity, cancellationToken);
            if (created == null)
                throw new ConflictException(
                    AppConstants.ErrorMessages.ResourceConflict);

            _logger.LogInformation("Designation created. DesignationId: {DesignationId}", created.Id);
            return ApiResponse<List<GetDesignationResponseDTO>>.Success(
                new List<GetDesignationResponseDTO> { _mapper.Map<GetDesignationResponseDTO>(created) },
                "Designation created successfully.");
        }

        #endregion
    }

    #endregion
}
