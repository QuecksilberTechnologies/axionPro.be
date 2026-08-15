using AutoMapper;
using axionpro.application.DTOs.Department;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IPermission;
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
    public class CreateDepartmentCommand : IRequest<ApiResponse<List<GetDepartmentResponseDTO>>>
    {
        /// <summary>
        /// Gets the client-supplied department data.
        /// </summary>
        public CreateDepartmentRequestDTO DTO { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateDepartmentCommand"/> class.
        /// </summary>
        /// <param name="dto">The client-supplied department data.</param>
        public CreateDepartmentCommand(CreateDepartmentRequestDTO dto)
        {
            DTO = dto;
        }
    }

    #endregion

    #region Handler

    /// <summary>
    /// Handles department creation requests.
    /// </summary>
    public class CreateDepartmentCommandHandler : IRequestHandler<CreateDepartmentCommand, ApiResponse<List<GetDepartmentResponseDTO>>>
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateDepartmentCommandHandler> _logger;
        private readonly IPermissionService _permissionService;
        private readonly ICommonRequestService _commonRequestService;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateDepartmentCommandHandler"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work used for persistence.</param>
        /// <param name="mapper">The object mapper.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="permissionService">The permission service.</param>
        /// <param name="commonRequestService">The shared authenticated-request validation service.</param>
        public CreateDepartmentCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<CreateDepartmentCommandHandler> logger,
            IPermissionService permissionService,
            ICommonRequestService commonRequestService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _permissionService = permissionService;
            _commonRequestService = commonRequestService;
        }

        #endregion

        #region Handle

        /// <summary>
        /// Creates a department from the supplied command.
        /// </summary>
        /// <param name="request">The create-department command.</param>
        /// <param name="cancellationToken">A token used to cancel the operation.</param>
        /// <returns>The API response containing the created department.</returns>
        public async Task<ApiResponse<List<GetDepartmentResponseDTO>>> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Creating Department ");

                // Validate the authenticated tenant-user context and confirm the request identity.
                var validation = await _commonRequestService.ValidateRequestAsync(request.DTO.UserEmployeeId);

                if (!validation.Success)
                    return ApiResponse<List<GetDepartmentResponseDTO>>.Fail(validation.ErrorMessage ?? "Unauthorized request.");

                // Resolve trusted tenant and employee identifiers from the validated context.
                long userEmployeeId = validation.LoggedInEmployeeId;
                long tenantId = validation.TenantId;

                // Preserve the existing permission lookup behavior.
                var permissions = await _permissionService.GetPermissionsAsync(validation.RoleId);
                if (!permissions.Contains("AddBankInfo"))
                {
                    //  await _unitOfWork.RollbackTransactionAsync();
                    //return ApiResponse<List<GetBankResponseDTO>>.Fail("You do not have permission to add bank info.");
                }

                // Validate the client-supplied department name.
                string? departmentName = request.DTO.DepartmentName?.Trim();

                if (string.IsNullOrWhiteSpace(departmentName))
                {
                    return ApiResponse<List<GetDepartmentResponseDTO>>.Fail("Department name should not be empty or whitespace.");
                }

                // Map client-supplied department data to the domain entity.
                var department = _mapper.Map<Department>(request.DTO);

                // Apply server-controlled tenant, audit, and default values before persistence.
                department.TenantId = tenantId;
                department.AddedById = userEmployeeId;
                department.AddedDateTime = DateTime.UtcNow;
                department.IsActive = true;
                department.IsSoftDeleted = false;
                department.IsExecutiveOffice = false;

                // Persist the entity through the department repository.
                var createdDepartment = await _unitOfWork.DepartmentRepository.CreateAsync(department, cancellationToken);

                if (createdDepartment == null)
                {
                    _logger.LogWarning("❌ Department creation failed or empty result. TenantId: {TenantId}", tenantId);
                    return ApiResponse<List<GetDepartmentResponseDTO>>.Fail("No department was created. Please try again.");
                }

                // Preserve the existing transaction completion behavior.
                await _unitOfWork.CommitTransactionAsync();

                // Convert the persisted entity to the API response shape.
                var responseDTO = _mapper.Map<GetDepartmentResponseDTO>(createdDepartment);

                return new ApiResponse<List<GetDepartmentResponseDTO>>
                {
                    IsSucceeded = true,
                    Message = "1 department(s) created successfully.",
                    PageNumber = 1,
                    PageSize = 1,
                    TotalRecords = 1,
                    TotalPages = 1,
                    Data = new List<GetDepartmentResponseDTO> { responseDTO }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while creating department(s).");
                return ApiResponse<List<GetDepartmentResponseDTO>>.Fail("Failed to create department(s) due to an internal error.");
            }
        }

        #endregion
    }

    #endregion
}
