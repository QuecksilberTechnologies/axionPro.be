using AutoMapper;
using axionpro.application.DTOS.AssetDTO.type;
using axionpro.application.Exceptions;
using axionpro.application.Features.AssetFeatures.Status.Handlers;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Wrappers;
using axionpro.domain.Entity; 
using axionpro.domain.Entity; 
using MediatR;
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

        public async Task<ApiResponse<bool>> Handle(
       DeletetTypeCommand request,
       CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Deleting Asset Type");

                // ===============================
                // 1️⃣ COMMON VALIDATION (AUTH + CONTEXT)
                // ===============================
                var validation = await _commonRequestService.ValidateRequestAsync();

                // ❌ Old: return Fail
                // ✅ New: throw
                if (!validation.Success)
                    throw new UnauthorizedAccessException(validation.ErrorMessage);

                // ===============================
                // 2️⃣ NULL SAFETY + INPUT VALIDATION
                // ===============================
                if (request?.DTO == null || request.DTO.Id <= 0)
                    throw new ValidationErrorException(
                        "Invalid Type Id.",
                        new List<string> { "Type Id must be greater than 0." }
                    );

                if (request.DTO.Prop == null)
                    request.DTO.Prop = new();

                // Inject values
                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;

                // ===============================
                // 3️⃣ PERMISSION CHECK (RBAC)
                // ===============================
                //var hasPermission = await _permissionService.HasAccessAsync(
                //    validation.RoleId,
                //    "AssetType",   // 🔹 Module
                //    "Delete"       // 🔹 Operation
                //);

                //if (!hasPermission)
                //    throw new UnauthorizedAccessException(
                //        "You do not have permission to delete asset type.");

                // ===============================
                // 4️⃣ DELETE (REPOSITORY)
                // ===============================
                bool isDeleted = await _unitOfWork.AssetTypeRepository
                    .DeleteAsync(request.DTO);

                if (!isDeleted)
                {
                    _logger.LogWarning(
                        "AssetType delete failed or not found. Id: {Id}, TenantId: {TenantId}",
                        request.DTO.Id,
                        request.DTO.Prop.TenantId);

                    throw new ApiException(
                        "Asset Type not found or already deleted.",
                        404
                    );
                }

                // ===============================
                // 5️⃣ SUCCESS LOG
                // ===============================
                _logger.LogInformation(
                    "AssetType deleted successfully. Id: {Id}, TenantId: {TenantId}",
                    request.DTO.Id,
                    request.DTO.Prop.TenantId);

                return ApiResponse<bool>
                    .Success(true, "Asset Type deleted successfully.");
            }
            catch (Exception ex)
            {
                // ❗ IMPORTANT: middleware handle karega
                _logger.LogError(
                    ex,
                    "Error occurred while deleting Asset Type. Id: {Id}, TenantId: {TenantId}",
                    request?.DTO?.Id,
                    request?.DTO?.Prop?.TenantId);

                throw;
            }
        }
    }
}
 