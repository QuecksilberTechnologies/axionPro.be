// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Creates tenant roles using trusted request context.
// ================================================================

using AutoMapper;
using axionpro.application.DTOs.Role;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.RoleCmd.Handlers
{
    #region Command

    /// <summary>
    /// Represents a request to create a tenant role.
    /// </summary>
    public class CreateRoleCommand : IRequest<ApiResponse<List<GetRoleResponseDTO>>>
    {
        public CreateRoleRequestDTO DTO { get; }

        /// <summary>
        /// Initializes the create request.
        /// </summary>
        public CreateRoleCommand(CreateRoleRequestDTO dto)
        {
            DTO = dto;
        }
    }

    #endregion

    #region Handler

    /// <summary>
    /// Handles tenant-role creation for the authenticated tenant.
    /// </summary>
    public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, ApiResponse<List<GetRoleResponseDTO>>>
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICommonRequestService _commonRequestService;
        private readonly ILogger<CreateRoleCommandHandler> _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes the handler dependencies.
        /// </summary>
        public CreateRoleCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ICommonRequestService commonRequestService,
            ILogger<CreateRoleCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _commonRequestService = commonRequestService;
            _logger = logger;
        }

        #endregion

        #region Handle

        /// <summary>
        /// Creates a role from client-editable values and trusted tenant audit context.
        /// </summary>
        public async Task<ApiResponse<List<GetRoleResponseDTO>>> Handle(
            CreateRoleCommand request,
            CancellationToken cancellationToken)
        {
            var validation = await _commonRequestService.ValidateTenantUserRequestAsync();
            if (!validation.Success)
                throw new UnauthorizedAccessException(validation.ErrorMessage);

            var roleName = request.DTO.RoleName?.Trim();
            if (string.IsNullOrWhiteSpace(roleName))
                throw new ValidationErrorException("Role name cannot be empty.");

            // Map client-editable fields to the domain entity.
            var entity = _mapper.Map<Role>(request.DTO);
            entity.RoleName = roleName;
            entity.TenantId = validation.TenantId;
            entity.AddedById = validation.LoggedInEmployeeId;
            entity.AddedDateTime = DateTime.UtcNow;
            entity.IsSystemDefault = false;
            entity.IsSoftDeleted = false;

            var created = await _unitOfWork.RoleRepository.CreateAsync(entity, cancellationToken);
            if (created == null)
                throw new ApiException("This role already exists for the tenant.", 409);

            _logger.LogInformation("Role created. RoleId: {RoleId}", created.Id);
            return ApiResponse<List<GetRoleResponseDTO>>.Success(
                new List<GetRoleResponseDTO> { _mapper.Map<GetRoleResponseDTO>(created) },
                "Role created successfully.");
        }

        #endregion
    }

    #endregion
}
