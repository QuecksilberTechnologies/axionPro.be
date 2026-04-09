using AutoMapper;
using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.Constants;
using axionpro.application.DTOs.RoleModulePermission;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IRepositories;
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
        public async Task<ApiResponse<int>> Handle(
    CreateRolePermissionCommand request,
    CancellationToken cancellationToken)
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
                // 2️⃣ VALID ROLE CHECK (CORRECTED ✅)
                // ===============================
                if (request.DTO.RoleId <= 0)
                    throw new ValidationErrorException("Invalid RoleId.");

                var role = await _unitOfWork.RoleRepository
                    .GetRoleAsync(validation.TenantId, request.DTO.RoleId,true);
                
                if (role == null)
                    throw new ValidationErrorException("Selected role not found or does not belong to tenant.");

                // ===============================
                // 3️⃣ PREPARE PERMISSIONS
                // ===============================
                var rolePermissions = request.DTO.ModuleOperations
                    .SelectMany(m => m.Operations.Select(op => new RoleModuleAndPermission
                    {
                        RoleId = request.DTO.RoleId,
                        ModuleId = m.ModuleId,
                        OperationId = op.OperationId,
                        HasAccess = op.HasAccess,
                        IsActive = true,
                        AddedById = validation.UserEmployeeId,
                        AddedDateTime = DateTime.UtcNow,
                        Remark = "Assigned via role permission UI"
                    }))
                    .ToList();

                if (!rolePermissions.Any())
                    throw new ValidationErrorException("No permissions selected.");

                // ===============================
                // 4️⃣ BULK INSERT
                // ===============================
                int insertedCount = await _unitOfWork
                    .UserRolesPermissionOnModuleRepository
                    .BulkInsertAsync(rolePermissions);

                if (insertedCount <= 0)
                    throw new ValidationErrorException("Failed to assign role permissions.");

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "✅ Permissions assigned | TenantId={TenantId} | UserId={UserId} | Count={Count}",
                    validation.TenantId,
                    validation.UserEmployeeId,
                    insertedCount);

                // ===============================
                // 5️⃣ RETURN (FIXED 🔥)
                // ===============================
                return new ApiResponse<int>
                {
                    IsSucceeded = true,
                    Message = "Role permissions created successfully.",
                    Data = insertedCount
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error in CreateRolePermission");
                throw;
            }
        }

    }
}
 