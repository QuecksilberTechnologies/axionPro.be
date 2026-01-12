using AutoMapper;
using axionpro.application.DTOS.AssetDTO.type;
using axionpro.application.Features.AssetFeatures.Status.Handlers;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.AssetFeatures.Type.Handlers
{
    public class DeletetTypeCommand : IRequest<ApiResponse<bool>>
    {
        public DeleteTypeRequestDTO DTO { get; set; }

        public DeletetTypeCommand(DeleteTypeRequestDTO dTO)
        {
            DTO = dTO;
        }
    }

    public class DeletetTypeCommandHandler : IRequestHandler<DeletetTypeCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeletetTypeCommandHandler> _logger;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IPermissionService _permissionService;

        public DeletetTypeCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<DeletetTypeCommandHandler> logger,
            ICommonRequestService commonRequestService,
            IPermissionService permissionService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _commonRequestService = commonRequestService;
            _permissionService = permissionService;
        }

        public async Task<ApiResponse<bool>> Handle(DeletetTypeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Deleting Asset Category");

                // ===============================
                // 1️⃣ COMMON VALIDATION
                // ===============================
                var validation = await _commonRequestService.ValidateRequestAsync();
                if (!validation.Success)
                    return ApiResponse<bool>.Fail(validation.ErrorMessage);

                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;

                // ===============================
                // 2️⃣ BASIC INPUT CHECK
                // ===============================
                if (request.DTO == null || request.DTO.Id <= 0)
                    return ApiResponse<bool>.Fail("Invalid Category Id.");

                // ===============================
                // 3️⃣ PERMISSION (OPTIONAL)
                // ===============================
                var permissions =
                    await _permissionService.GetPermissionsAsync(validation.RoleId);

                // if (!permissions.Contains("DeleteAssetCategory"))
                //     return ApiResponse<bool>.Fail("Permission denied.");

                // ===============================
                // 4️⃣ DELETE (REPO DECIDES RESULT)
                // ===============================

                // ✅ Step 2: Repository call

                // ✅ Step 1: Call repository for soft delete
                bool isDeleted = await _unitOfWork.AssetTypeRepository.DeleteAsync(request.DTO);

                // ✅ Step 2: Handle result
                if (!isDeleted)
                {
                    _logger.LogWarning("⚠️ AssetType delete failed or not found. Id: {Id}, TenantId: {TenantId}",
                        request.DTO.Id, request.DTO.Prop.TenantId);

                    return new ApiResponse<bool>
                    {
                        IsSucceeded = false,
                        Message = "Asset Type not found or already deleted.",
                        Data = false
                    };
                }

                // ✅ Step 3: Successful deletion response
                _logger.LogInformation("✅ AssetType deleted successfully. Id: {Id}, TenantId: {TenantId}",
                    request.DTO.Id, request.DTO.Prop.TenantId);

                return new ApiResponse<bool>
                {
                    IsSucceeded = true,
                    Message = "Asset Type deleted successfully.",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while deleting Asset Type. Id: {Id}, TenantId: {TenantId}",
                    request.DTO.Id, request.DTO.Prop.TenantId);

                return new ApiResponse<bool>
                {
                    IsSucceeded = false,
                    Message = $"An error occurred while deleting Asset Type: {ex.Message}",
                    Data = false
                };
            }
        }

    }
}
 