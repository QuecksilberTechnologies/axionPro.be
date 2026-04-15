using AutoMapper;
using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.DTOS.UserRoles;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.UserRolesCmd.Handlers
{
    // ===============================
    // COMMAND
    // ===============================
    public class AssignUserRolesCommand  : IRequest<ApiResponse<List<UserRoleDTO>>>
    {
        public UserRoleListDTO DTO { get; set; }

        public AssignUserRolesCommand(UserRoleListDTO dto)
        {
            DTO = dto;
        }
    }

    // ===============================
    // HANDLER
    // ===============================
    public class AssignUserRolesCommandHandler
    : IRequestHandler<AssignUserRolesCommand, ApiResponse<List<UserRoleDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AssignUserRolesCommandHandler> _logger;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IIdEncoderService _idEncoderService;

        public AssignUserRolesCommandHandler(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ILogger<AssignUserRolesCommandHandler> logger,
            ICommonRequestService commonRequestService,
            IIdEncoderService idEncoderService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _commonRequestService = commonRequestService;
            _idEncoderService = idEncoderService;
        }

        public async Task<ApiResponse<List<UserRoleDTO>>> Handle(
    AssignUserRolesCommand request,
    CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("🚀 AssignUserRoles started");

                // ===============================
                // 1️⃣ VALIDATION
                // ===============================
                var validation = await _commonRequestService.ValidateRequestAsync();

                if (!validation.Success)
                    throw new UnauthorizedAccessException(validation.ErrorMessage);

                if (request?.DTO == null)
                    throw new ValidationErrorException("Invalid request.");

                request.DTO.Prop ??= new();
                request.DTO.Prop.TenantId = validation.TenantId;

                // ===============================
                // 2️⃣ EMPLOYEE ID CHECK
                // ===============================
                if (string.IsNullOrWhiteSpace(request.DTO.EmployeeId))
                    throw new ValidationErrorException("Invalid EmployeeId.");

                var employeeId = request.DTO.Prop.EmployeeId =
                    RequestCommonHelper.DecodeOnlyEmployeeId(
                        request.DTO.EmployeeId,
                        validation.Claims.TenantEncriptionKey,
                        _idEncoderService);

                // ===============================
                // 3️⃣ FETCH EXISTING ROLES
                // ===============================
                var existingRoles = await _unitOfWork.UserRoleRepository
                    .GetEmployeeRolesWithDetailsByIdAsync(request.DTO.Prop.EmployeeId, request.DTO.Prop.TenantId);

                var incomingRoles = request.DTO.UserRoles ?? new List<UserRoleDTO>();

                // 🔥 Convert to HashSet for ultra fast lookup (O(1))
                var existingRoleIds = existingRoles.Select(x => x.RoleId).ToHashSet();
                var incomingRoleIds = incomingRoles.Select(x => x.RoleId).ToHashSet();

                // ==========================================================
                // 🟥 BULK DELETE (Soft Delete)
                // ==========================================================
                var rolesToDelete = existingRoles
                    .Where(x => !incomingRoleIds.Contains(x.RoleId))
                    .ToList();

                if (rolesToDelete.Any())
                {
                    foreach (var role in rolesToDelete)
                    {
                        role.IsSoftDeleted = true;
                        role.IsActive = false;
                        role.SoftDeletedById = validation.LoggedInEmployeeId;
                        role.DeletedDateTime = DateTime.UtcNow;
                    }

                    // 🔥 Single bulk update call
                    _unitOfWork.UserRoleRepository.UpdateRange(rolesToDelete);
                }

                // ==========================================================
                // 🟩 BULK INSERT
                // ==========================================================
                var rolesToInsert = incomingRoles
                    .Where(x => !existingRoleIds.Contains(x.RoleId))
                    .Select(role => new UserRole
                    {
                        EmployeeId = request.DTO.Prop.EmployeeId,
                        RoleId = role.RoleId,

                        IsPrimaryRole = false,
                        

                        IsActive = role.IsActive,
                        IsSoftDeleted = false,

                        AddedById = validation.LoggedInEmployeeId,
                        AddedDateTime = DateTime.UtcNow,
                        

                       
                    })
                    .ToList();

                if (rolesToInsert.Any())
                {
                    await _unitOfWork.UserRoleRepository.AddRangeAsync(rolesToInsert);
                }

                // ==========================================================
                // 🟨 BULK UPDATE
                // ==========================================================
                var rolesToUpdate = existingRoles
                    .Where(x => incomingRoleIds.Contains(x.RoleId))
                    .ToList();

                if (rolesToUpdate.Any())
                {
                    foreach (var existing in rolesToUpdate)
                    {
                        var input = incomingRoles.First(x => x.RoleId == existing.RoleId);

                        existing.IsActive = input.IsActive;

                        // force business rules
                        existing.IsPrimaryRole = false;
                       

                        existing.UpdatedById = validation.LoggedInEmployeeId;
                        existing.UpdatedDateTime = DateTime.UtcNow;
                    }

                    // 🔥 Single bulk update call
                    _unitOfWork.UserRoleRepository.UpdateRange(rolesToUpdate);
                }

                // ===============================
                // 🔥 SINGLE DB COMMIT
                // ===============================
                await _unitOfWork.SaveChangesAsync();

                // ===============================
                // 9️⃣ RETURN UPDATED DATA
                // ===============================
                var updatedRoles = await _unitOfWork.UserRoleRepository
                    .GetEmployeeRolesWithDetailsByIdAsync(request.DTO.Prop.EmployeeId, request.DTO.Prop.TenantId);

                var userRoleDTOs = _mapper.Map<List<UserRoleDTO>>(updatedRoles);

                return new ApiResponse<List<UserRoleDTO>>
                {
                    IsSucceeded = true,
                    Message = "Roles assigned successfully",
                    Data = userRoleDTOs
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error in AssignUserRoles");
                throw;
            }
        }
    }

}
 