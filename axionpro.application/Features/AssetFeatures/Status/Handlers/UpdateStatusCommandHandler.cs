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
    public class UpdateStatusCommand : IRequest<ApiResponse<bool>>
    {
        public UpdateStatusRequestDTO DTO { get; set; }

        public UpdateStatusCommand(UpdateStatusRequestDTO dTO)
        {
            this.DTO = dTO;
        }

    }
    public class UpdateStatusCommandHandler : IRequestHandler<UpdateStatusCommand, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateStatusCommandHandler> _logger;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IPermissionService _permissionService;

        public UpdateStatusCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<UpdateStatusCommandHandler> logger,
            ICommonRequestService commonRequestService,
            IPermissionService permissionService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _commonRequestService = commonRequestService;
            _permissionService = permissionService;
        }

        public async Task<ApiResponse<bool>> Handle(UpdateStatusCommand request, CancellationToken cancellationToken)
        {
            try
            {
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
                    return ApiResponse<bool>.Fail("Invalid Status Id.");

                // ===============================
                // 3️⃣ PERMISSION (OPTIONAL)
                // ===============================
                var permissions =
                    await _permissionService.GetPermissionsAsync(validation.RoleId);

                // if (!permissions.Contains("DeleteAssetCategory"))
                //     return ApiResponse<bool>.Fail("Permission denied.");

                var isUpdated = await _unitOfWork.AssetStatusRepository.UpdateAsync(request.DTO);
               
                 if(!isUpdated)
                {
                    return new ApiResponse<bool>
                    {
                        IsSucceeded = false,
                        Message = "Asset Status not found or update failed.",
                        Data = false
                    };
                }
                    return new ApiResponse<bool>
                {
                    IsSucceeded = true,
                    Message = "Asset Status updated successfully.",
                    Data = true
                    };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while Updateing asset status.");
                return new ApiResponse<bool>
                {
                    IsSucceeded = false,
                    Message = "Something went wrong while Updateing asset status.",
                    Data = false
                };
            }
        }
    }

}
