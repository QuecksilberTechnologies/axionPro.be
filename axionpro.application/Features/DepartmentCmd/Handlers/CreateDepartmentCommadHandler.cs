// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Handles CreateDepartmentCommand requests using current
//           tenant identity, role, and module-operation authorization.
// ================================================================

using AutoMapper;
using axionpro.application.Constants;
using axionpro.application.DTOs.Department;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.DepartmentCmd.Handlers
{
    #region Command

    /// <summary>
    /// Represents a request to create a department.
    /// </summary>
    public class CreateDepartmentCommand
        : IRequest<ApiResponse<List<GetDepartmentResponseDTO>>>
    {
        /// <summary>
        /// Gets the client-supplied department data.
        /// </summary>
        public CreateDepartmentRequestDTO DTO { get; set; }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="CreateDepartmentCommand"/> class.
        /// </summary>
        /// <param name="dto">
        /// The client-supplied department data.
        /// </param>
        public CreateDepartmentCommand(
            CreateDepartmentRequestDTO dto)
        {
            DTO = dto;
        }
    }

    #endregion

    #region Handler

    /// <summary>
    /// Handles authenticated and authorized department creation requests.
    /// </summary>
    public class CreateDepartmentCommandHandler
        : IRequestHandler<
            CreateDepartmentCommand,
            ApiResponse<List<GetDepartmentResponseDTO>>>
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateDepartmentCommandHandler> _logger;
        private readonly ICommonRequestService _commonRequestService;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="CreateDepartmentCommandHandler"/> class.
        /// </summary>
        /// <param name="unitOfWork">
        /// Provides repositories and stored-function access.
        /// </param>
        /// <param name="mapper">
        /// Maps request and response objects.
        /// </param>
        /// <param name="logger">
        /// Provides structured application logging.
        /// </param>
        /// <param name="commonRequestService">
        /// Validates the authenticated Tenant request context.
        /// </param>
        public CreateDepartmentCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<CreateDepartmentCommandHandler> logger,
            ICommonRequestService commonRequestService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _commonRequestService = commonRequestService;
        }

        #endregion

        #region Handle

        /// <summary>
        /// Creates a department after validating the current Tenant user
        /// and checking the current Department/Add permission.
        /// </summary>
        /// <param name="request">
        /// The create-department command.
        /// </param>
        /// <param name="cancellationToken">
        /// Token used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// An API response containing the newly created department.
        /// </returns>
        /// <exception cref="UnauthorizedAccessException">
        /// Thrown when the authenticated user context is invalid or stale.
        /// </exception>
        /// <exception cref="ValidationErrorException">
        /// Thrown when the supplied department data is invalid.
        /// </exception>
        /// <exception cref="ConflictException">
        /// Thrown when the department conflicts with an existing record.
        /// </exception>
        public async Task<ApiResponse<List<GetDepartmentResponseDTO>>> Handle(
            CreateDepartmentCommand request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Creating Department.");

            #region Tenant Request Validation

            // Validate the authenticated Tenant identity and resolve trusted
            // Employee, Tenant and token role context.
            var validation =
                await _commonRequestService
                    .ValidateTenantUserRequestAsync();

            if (!validation.Success)
            {
                throw new UnauthorizedAccessException(
                    validation.ErrorMessage ??
                    AppConstants.ErrorMessages.Unauthorized);
            }

            #endregion

            #region Trusted Request Context

            long userEmployeeId =
                validation.LoggedInEmployeeId;

            long tenantId =
                validation.TenantId;

            int tokenRoleId =
                validation.RoleId;

            // A missing Tenant, Employee or token Primary Role means that
            // the authorization context cannot be trusted.
            if (userEmployeeId <= 0 ||
                tenantId <= 0 ||
                tokenRoleId <= 0)
            {
                _logger.LogWarning(
                    "Invalid Tenant authorization context while creating Department. " +
                    "TenantId: {TenantId}, EmployeeId: {EmployeeId}, TokenRoleId: {TokenRoleId}",
                    tenantId,
                    userEmployeeId,
                    tokenRoleId);

                throw new UnauthorizedAccessException(
                    AppConstants.ErrorMessages.Unauthorized);
            }

            #endregion

            #region Runtime Permission Validation

            /*
             * Runtime authorization is resolved from the database.
             *
             * The PostgreSQL function performs:
             *
             * 1. Current Primary Role validation.
             * 2. JWT Primary Role vs current Primary Role comparison.
             * 3. Current active Primary + Secondary UserRole resolution.
             * 4. RoleModuleAndPermission lookup.
             * 5. Department + Add permission decision.
             *
             * Therefore the stale RoleId stored in an old JWT is never used
             * as the final authorization source.
             */
            var permissionResult =
                await _unitOfWork
                    .StoreProcedureRepository
                    .CheckTenantEmployeePermissionAsync(
                        tenantId,
                        userEmployeeId,
                        tokenRoleId,
                        request.DTO.ModuleId,
                        request.DTO.OperationId,
                        cancellationToken);

            switch (permissionResult.ResultCode)
            {
                #region Permission Allowed

                case 1:
                    {
                        _logger.LogInformation(
                            "Department create permission granted. " +
                            "TenantId: {TenantId}, EmployeeId: {EmployeeId}, " +
                            "CurrentPrimaryRoleId: {CurrentPrimaryRoleId}, GrantedRoleId: {GrantedRoleId}",
                            tenantId,
                            userEmployeeId,
                            permissionResult.CurrentPrimaryRoleId,
                            permissionResult.GrantedRoleId);

                        break;
                    }

                #endregion

                #region Authorization Context Changed

                case -1:
                    {
                        /*
                         * Example:
                         *
                         * JWT Primary Role:
                         * Engineering Manager = 52
                         *
                         * Current DB Primary Role:
                         * Employee = 36
                         *
                         * The JWT authorization context is stale.
                         *
                         * This should result in 401 so the client can clear
                         * the old session and perform a fresh login.
                         */
                        _logger.LogWarning(
                            "Tenant authorization context changed. " +
                            "TenantId: {TenantId}, EmployeeId: {EmployeeId}, " +
                            "TokenRoleId: {TokenRoleId}, CurrentPrimaryRoleId: {CurrentPrimaryRoleId}",
                            tenantId,
                            userEmployeeId,
                            tokenRoleId,
                            permissionResult.CurrentPrimaryRoleId);

                        throw new UnauthorizedAccessException(
                            AppConstants.ErrorMessages.Unauthorized);
                    }

                #endregion

                #region Invalid Role Context

                case -2:
                    {
                        /*
                         * No valid current Primary Role exists, multiple invalid
                         * Primary Role assignments exist, or the supplied
                         * authorization context is otherwise unusable.
                         */
                        _logger.LogWarning(
                            "Invalid Tenant role context while creating Department. " +
                            "TenantId: {TenantId}, EmployeeId: {EmployeeId}, TokenRoleId: {TokenRoleId}",
                            tenantId,
                            userEmployeeId,
                            tokenRoleId);

                        throw new UnauthorizedAccessException(
                            AppConstants.ErrorMessages.Unauthorized);
                    }

                #endregion

                #region Permission Denied

                case 0:
                default:
                    {
                        /*
                         * The identity/session remains valid, but none of the
                         * employee's CURRENT Primary or Secondary roles grants
                         * Department + Add permission.
                         *
                         * IMPORTANT:
                         * This is an authorization denial, not a stale-session
                         * condition.
                         *
                         * If the project's centralized exception middleware
                         * already has a dedicated 403 Forbidden exception,
                         * use that exception here instead of
                         * UnauthorizedAccessException.
                         */
                        _logger.LogWarning(
                            "Department create permission denied. " +
                            "TenantId: {TenantId}, EmployeeId: {EmployeeId}, " +
                            "ModuleId: {ModuleId}, OperationId: {OperationId}",
                            tenantId,
                            userEmployeeId,
                            request.DTO.ModuleId,
                            request.DTO.OperationId);

                        throw new UnauthorizedAccessException(
                            AppConstants.ErrorMessages.Unauthorized);
                    }

                #endregion
            }

            #endregion

            #region Department Request Validation

            string? departmentName =
                request.DTO.DepartmentName?.Trim();

            if (string.IsNullOrWhiteSpace(departmentName))
            {
                throw new ValidationErrorException(
                    AppConstants.ErrorMessages.InvalidRequest);
            }

            // Ensure the normalized value is persisted.
            request.DTO.DepartmentName =
                departmentName;

            #endregion

            #region Entity Mapping

            var department =
                _mapper.Map<Department>(
                    request.DTO);

            #endregion

            #region Server Controlled Values

            department.TenantId =
                tenantId;

            department.AddedById =
                userEmployeeId;

            department.AddedDateTime =
                DateTime.UtcNow;

            department.IsActive =true;

            department.IsSoftDeleted =
                false;

            department.IsExecutiveOffice =
                false;

            #endregion

            #region Persistence

            var createdDepartment =
                await _unitOfWork
                    .DepartmentRepository
                    .CreateAsync(
                        department,
                        cancellationToken);

            if (createdDepartment == null)
            {
                _logger.LogWarning(
                    "Department creation conflicted with an existing record. TenantId: {TenantId}",
                    tenantId);

                throw new ConflictException(
                    AppConstants.ErrorMessages.ResourceConflict);
            }

            // Preserve the existing transaction completion behavior.
            await _unitOfWork
                .CommitTransactionAsync();

            #endregion

            #region Response Mapping

            var responseDTO =
                _mapper.Map<GetDepartmentResponseDTO>(
                    createdDepartment);

            #endregion

            #region Success Response

            _logger.LogInformation(
                "Department created successfully. TenantId: {TenantId}, EmployeeId: {EmployeeId}",
                tenantId,
                userEmployeeId);

            return new ApiResponse<List<GetDepartmentResponseDTO>>
            {
                IsSucceeded = true,
                Message = "1 department(s) created successfully.",
                PageNumber = 1,
                PageSize = 1,
                TotalRecords = 1,
                TotalPages = 1,
                Data = new List<GetDepartmentResponseDTO>
                {
                    responseDTO
                }
            };

            #endregion
        }

        #endregion
    }

    #endregion
}