using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.axionpro.application.Configuration;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.EncryptionHelper;

using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.DTOs.Department;
using axionpro.application.DTOs.Role;

using axionpro.application.Features.DepartmentCmd.Handlers;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace axionpro.application.Features.RoleCmd.Handlers
{
    public class DeleteRoleQuery : IRequest<ApiResponse<bool>>
    {
        public DeleteRoleRequestDTO DTO { get; set; }

        public DeleteRoleQuery(DeleteRoleRequestDTO dto)
        {
            DTO = dto;
        }
    }

    public class DeleteRoleQueryHandler : IRequestHandler<DeleteRoleQuery, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<DeleteRoleQueryHandler> _logger;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _config;
        private readonly IEncryptionService _encryptionService;
        private readonly IPermissionService _permissionService;
        private readonly IIdEncoderService _idEncoderService;
        private readonly ICommonRequestService _commonRequestService;

        public DeleteRoleQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<DeleteRoleQueryHandler> logger,
            ITokenService tokenService,
            IPermissionService permissionService,
            IConfiguration config,
            IEncryptionService encryptionService, IIdEncoderService idEncoderService, ICommonRequestService commonRequestService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _tokenService = tokenService;
            _permissionService = permissionService;
            _config = config;
            _encryptionService = encryptionService;
            _idEncoderService = idEncoderService;
            _commonRequestService = commonRequestService;
        }

        public async Task<ApiResponse<bool>> Handle(DeleteRoleQuery request, CancellationToken cancellationToken)
        {
            try
            {
                //  COMMON VALIDATION (Mandatory)
                var validation = await _commonRequestService.ValidateRequestAsync(request.DTO.UserEmployeeId);

                if (!validation.Success)
                    return ApiResponse<bool>.Fail(validation.ErrorMessage);

                // Assign decoded values coming from CommonRequestService
                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;


                // ✅ Create  using repository
                var permissions = await _permissionService.GetPermissionsAsync(validation.RoleId);


                if (!permissions.Contains("AddBankInfo"))
                {
                    // await _unitOfWork.RollbackTransactionAsync();
                    //return ApiResponse<List<GetBankResponseDTO>>.Fail("You do not have permission to add bank info.");
                }



                // 🧩 STEP 5: Validate RoleId
                if (request.DTO.Id <=0 )
                {
                    _logger.LogWarning("⚠️ Invalid RoleId for delete operation: {RoleId}", request.DTO.Id);
                    return ApiResponse<bool>.Fail("Invalid RoleId. It must be greater than zero.");
                }
               
                _logger.LogInformation("🗑️ Attempting to delete RoleId: {RoleId} for TenantId: {TenantId}", request.DTO.Id,  request.DTO.Prop.TenantId);

                // 🧩 STEP 6: Repository call

                bool isDeleted = await _unitOfWork.RoleRepository.DeleteAsync(request.DTO, request.DTO.Prop.UserEmployeeId, request.DTO.Id);

                if (!isDeleted)
                {
                    _logger.LogWarning("❌ Delete failed or role not found. RoleId: {RoleId}", request.DTO.Id);
                    return ApiResponse<bool>.Fail("Delete failed. Role not found or already deleted.");
                }

                // 🧩 STEP 7: Commit transaction
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("✅ Role deleted successfully. RoleId: {RoleId}, TenantId: {TenantId}", request.DTO.Id, request.DTO.Prop.TenantId);

                return new ApiResponse<bool>
                {
                    IsSucceeded = true,
                    Message = "Role deleted successfully.",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while deleting RoleId: {RoleId}", request?.DTO?.Id);

                return new ApiResponse<bool>
                {
                    IsSucceeded = false,
                    Message = "Failed to delete role due to an internal error.",
                    Data = false
                };
            }
        }
    }
}
