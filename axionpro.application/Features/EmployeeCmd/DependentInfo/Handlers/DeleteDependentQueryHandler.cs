using axionpro.application.DTOS.Common;
using axionpro.application.Features.EmployeeCmd.Contact.Handlers;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.EmployeeCmd.DependentInfo.Handlers
{
    public class DeleteContactQuery : IRequest<ApiResponse<bool>>
    {
        public DeleteRequestDTO DTO;

        public DeleteContactQuery(DeleteRequestDTO dto)
        {
            DTO = dto;
        }
    }

    public class DeleteContactInfoQueryHandler
    : IRequestHandler<DeleteContactQuery, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteContactInfoQueryHandler> _logger;
        private readonly IPermissionService _permissionService;
        private readonly ICommonRequestService _commonRequestService;

        public DeleteContactInfoQueryHandler(
            IUnitOfWork unitOfWork,
            ILogger<DeleteContactInfoQueryHandler> logger,
            IPermissionService permissionService,
            ICommonRequestService commonRequestService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _permissionService = permissionService;
            _commonRequestService = commonRequestService;
        }

        public async Task<ApiResponse<bool>> Handle(
            DeleteContactQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // 🔐 STEP 1: COMMON VALIDATION
                var validation =
                    await _commonRequestService.ValidateRequestAsync();

                if (!validation.Success)
                    return ApiResponse<bool>.Fail(validation.ErrorMessage);

                long loggedInEmployeeId = validation.UserEmployeeId;

                // 🔎 STEP 2: Validate Dependent Id
                if (request.DTO.Id <= 0)
                    return ApiResponse<bool>.Fail("Invalid Dependent id.");

                // 🔑 STEP 3: Permission check
                var permissions =
                    await _permissionService.GetPermissionsAsync(validation.RoleId);

                if (!permissions.Contains("DeleteDependentInfo"))
                {
                    // optional strict block
                    // return ApiResponse<bool>.Fail("You do not have permission to delete contact.");
                }

                // 📦 STEP 4: Fetch existing record
                var existing =
                    await _unitOfWork.EmployeeDependentRepository
                        .GetSingleRecordAsync(request.DTO.Id, true);

                if (existing == null)
                    return ApiResponse<bool>.Fail("Dependent record not found.");

                // 🔒 STEP 5: Ownership check
                if (existing.EmployeeId != loggedInEmployeeId)
                    return ApiResponse<bool>.Fail("Unauthorized access.");

                // 🗑️ STEP 6: Soft Delete
                existing.IsSoftDeleted = true;
                existing.IsActive = false;
                existing.DeletedDateTime = DateTime.UtcNow;
                existing.SoftDeletedById = loggedInEmployeeId;

                bool deleted =
                    await _unitOfWork.EmployeeDependentRepository
                        .DeleteAsync(existing);

                if (!deleted)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return ApiResponse<bool>.Fail("Failed to delete Dependent.");
                }

                await _unitOfWork.CommitTransactionAsync();
                return ApiResponse<bool>.Success(true, "Dependent deleted successfully.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();

                _logger.LogError(
                    ex,
                    "Error deleting Dependent | Dependentd: {Id}",
                    request.DTO?.Id);

                return ApiResponse<bool>.Fail(
                    "Error deleting Dependent.",
                    new List<string> { ex.Message });
            }
        }
    }
}