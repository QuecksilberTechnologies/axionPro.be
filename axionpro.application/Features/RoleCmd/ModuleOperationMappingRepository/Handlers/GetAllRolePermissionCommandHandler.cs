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
    public class GetRolePermissionCommand
        : IRequest<ApiResponse<List<RoleModuleAndPermission>>>
    {
        public GetAllActiveRoleModuleOperationsRequestByRoleIdDTO DTO { get; set; }

        public GetRolePermissionCommand(GetAllActiveRoleModuleOperationsRequestByRoleIdDTO dto)
        {
            DTO = dto;
        }
    }


    // HANDLER
    // ===============================
    public class GetRolePermissionCommandHandler
        : IRequestHandler<GetRolePermissionCommand, ApiResponse<List<RoleModuleAndPermission>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetRolePermissionCommandHandler> _logger;
        private readonly ICommonRequestService _commonRequestService;

        public GetRolePermissionCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<GetRolePermissionCommandHandler> logger,
            ICommonRequestService commonRequestService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _commonRequestService = commonRequestService;
        }

        public async Task<ApiResponse<List<RoleModuleAndPermission>>> Handle(
            GetRolePermissionCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("🚀 GetRolePermission started");

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

                // ===============================
                // 3️⃣ FETCH EXISTING PERMISSIONS
                // ===============================
                var existingPermissions = await _unitOfWork
                    .UserRolesPermissionOnModuleRepository
                    .GetByRoleIdAsync(request.DTO.RoleId);

                // ===============================
                // 4️⃣ RESPONSE
                // ===============================
                return new ApiResponse<List<RoleModuleAndPermission>>
                {
                    IsSucceeded = true,
                    Message = "Role permissions fetched successfully.",
                    Data = existingPermissions
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error in GetRolePermission");
                throw;
            }
        }
    }
}
 