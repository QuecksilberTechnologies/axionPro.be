using AutoMapper;
using axionpro.application.DTOS.AssetDTO.status;
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

namespace axionpro.application.Features.AssetFeatures.Status.Handlers
{
    public class DeleteStatusCommand : IRequest<ApiResponse<bool>>
    {
        public DeleteStatusReqestDTO? DTO { get; set; }

        public DeleteStatusCommand(DeleteStatusReqestDTO deleteAssetStatusRequest)
        {
            this.DTO = deleteAssetStatusRequest;
        }
    }
        public class DeleteStatusCommandHandler : IRequestHandler<DeleteStatusCommand, ApiResponse<bool>>
     {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteStatusCommandHandler> _logger;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IPermissionService _permissionService;

        public DeleteStatusCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<DeleteStatusCommandHandler> logger,
            ICommonRequestService commonRequestService,
            IPermissionService permissionService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _commonRequestService = commonRequestService;
            _permissionService = permissionService;
        }


        public async Task<ApiResponse<bool>> Handle(DeleteStatusCommand request, CancellationToken cancellationToken)
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
                bool isDeleted = await _unitOfWork.AssetStatusRepository.DeleteAsync(request.DTO);

                // ✅ Step 3: Check result
                if (!isDeleted)
                {
                    _logger.LogWarning("⚠️ AssetStatus delete failed. Record not found. Id: {Id}, TenantId: {TenantId}",
                        request.DTO.Id, request.DTO.Prop.TenantId);

                    return new ApiResponse<bool>
                    {
                        IsSucceeded = false,
                        Message = "⚠️ Delete failed. Record not found or already deleted.",
                        Data = false
                    };
                }

                // ✅ Step 4: Return success
                _logger.LogInformation("✅ AssetStatus deleted successfully. Id: {Id}, TenantId: {TenantId}",
                    request.DTO.Id, request.DTO.Prop.TenantId);

                return new ApiResponse<bool>
                {
                    IsSucceeded = true,
                    Message = "✅ Asset Status deleted successfully.",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while deleting Asset Status. TenantId: {TenantId}, Id: {Id}",
                    request.DTO?.Prop.TenantId, request.DTO?.Id);

                return new ApiResponse<bool>
                {
                    IsSucceeded = false,
                    Message = $"❌ Something went wrong while deleting Asset Status: {ex.Message}",
                    Data = false
                };
            }
        }
    }

}