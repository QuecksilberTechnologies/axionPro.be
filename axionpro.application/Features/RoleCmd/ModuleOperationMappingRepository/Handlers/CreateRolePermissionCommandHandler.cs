using AutoMapper;
using axionpro.application.DTOs.RoleModulePermission;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.RoleCmd.ModuleOperationMappingRepository.Handlers
{
    // ===============================
    // COMMAND
    // ===============================
    public class CreateRolePermissionCommand : IRequest<ApiResponse<int>>
    {
        public CreateModuleOperationRolePermissionsRequestDTO DTO { get; set; }

        public CreateRolePermissionCommand(CreateModuleOperationRolePermissionsRequestDTO dto)
        {
            DTO = dto;
        }
    }

    // ===============================
    // HANDLER
    // ===============================
    public class CreateRolePermissionCommandHandler
        : IRequestHandler<CreateRolePermissionCommand, ApiResponse<int>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateRolePermissionCommandHandler> _logger;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IIdEncoderService _idEncoderService;

        public CreateRolePermissionCommandHandler(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ILogger<CreateRolePermissionCommandHandler> logger,
            ICommonRequestService commonRequestService,
            IIdEncoderService idEncoderService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _commonRequestService = commonRequestService;
            _idEncoderService = idEncoderService;
        }
        public async Task<ApiResponse<int>> Handle( CreateRolePermissionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("🚀 CreateRolePermission started");

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
                // 2️⃣ VALID ROLE CHECK
                // ===============================
                if (request.DTO.RoleId <= 0)
                    throw new ValidationErrorException("Invalid RoleId.");

                var role = await _unitOfWork.RoleRepository
                    .GetRoleAsync(validation.TenantId, request.DTO.RoleId, true);

                if (role == null)
                    throw new ValidationErrorException("Selected role not found.");


                // ===============================
                // 3️⃣ FETCH EXISTING PERMISSIONS (DB)
                // ===============================
                var existingPermissions = await _unitOfWork
                    .UserRolesPermissionOnModuleRepository.GetByRoleIdAsync(request.DTO.RoleId);

                // HashSet for fast lookup 🔥
                var existingSet = existingPermissions
                    .Select(x => (x.ModuleId, x.OperationId))
                    .ToHashSet();

                // ===============================
                // 4️⃣ PREPARE INSERT + DELETE LIST
                // ===============================
                var toInsert = new List<RoleModuleAndPermission>();
                var toDelete = new List<RoleModuleAndPermission>();

                foreach (var module in request.DTO.ModuleOperations)
                {
                    if (module.Operations == null) continue;

                    foreach (var op in module.Operations)
                    {
                        var key = (module.ModuleId, op.OperationId);

                        // ✅ CASE 1: CHECKED → INSERT if not exists
                        if (op.HasAccess)
                        {
                            if (!existingSet.Contains(key))
                            {
                                toInsert.Add(new RoleModuleAndPermission
                                {
                                    RoleId = request.DTO.RoleId,
                                    ModuleId = module.ModuleId,
                                    OperationId = op.OperationId,
                                    HasAccess = true,
                                    IsActive = true,
                                    AddedById = validation.UserEmployeeId,
                                    AddedDateTime = DateTime.UtcNow,
                                    Remark = "Assigned via role permission UI"
                                });
                            }
                        }
                        // ❌ CASE 2: UNCHECKED → DELETE if exists
                        else
                        {
                            var existing = existingPermissions
                                .FirstOrDefault(x =>
                                    x.ModuleId == module.ModuleId &&
                                    x.OperationId == op.OperationId);

                            if (existing != null)
                            {
                                toDelete.Add(existing);
                            }
                        }
                    }
                }

                // ===============================
                // 5️⃣ APPLY DB OPERATIONS
                // ===============================

                // 🔥 INSERT new permissions
                if (toInsert.Any())
                {
                    await _unitOfWork
                        .UserRolesPermissionOnModuleRepository
                        .BulkInsertAsync(toInsert);
                }

                // 🔥 DELETE removed permissions
                if (toDelete.Any())
                {
                    await _unitOfWork
                        .UserRolesPermissionOnModuleRepository
                        .BulkDeleteAsync(toDelete);
                }

                // ===============================
                // 6️⃣ SAVE CHANGES
                // ===============================
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                int affectedCount = toInsert.Count + toDelete.Count;

                _logger.LogInformation(
                    "✅ Permissions updated | TenantId={TenantId} | RoleId={RoleId} | Inserted={Inserted} | Deleted={Deleted}",
                    validation.TenantId,
                    request.DTO.RoleId,
                    toInsert.Count,
                    toDelete.Count);

                // ===============================
                // 7️⃣ RESPONSE
                // ===============================
                return new ApiResponse<int>
                {
                    IsSucceeded = true,
                    Message = "Role permissions updated successfully.",
                    Data = affectedCount
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error in CreateRolePermission");
                throw; // middleware handle karega 🔥
            }
        }

    }
}
 