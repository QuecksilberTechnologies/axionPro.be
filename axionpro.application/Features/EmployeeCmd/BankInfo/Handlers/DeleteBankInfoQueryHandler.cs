using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Employee.Bank;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.EmployeeCmd.BankInfo.Handlers
{
    public class DeleteBankInfoQuery : IRequest<ApiResponse<bool>>
    {
        public DeleteBankRequestDTO DTO { get; }

        public DeleteBankInfoQuery(DeleteBankRequestDTO dto)
        {
            DTO = dto;
        }
    }


    public class DeleteBankInfoQueryHandler
    : IRequestHandler<DeleteBankInfoQuery, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteBankInfoQueryHandler> _logger;
        private readonly IPermissionService _permissionService;
        private readonly ICommonRequestService _commonRequestService;

        public DeleteBankInfoQueryHandler(
            IUnitOfWork unitOfWork,
            ILogger<DeleteBankInfoQueryHandler> logger,
            IPermissionService permissionService,
            ICommonRequestService commonRequestService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _permissionService = permissionService;
            _commonRequestService = commonRequestService;
        }

        public async Task<ApiResponse<bool>> Handle(
            DeleteBankInfoQuery request,
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

                // 🔎 STEP 2: Validate Bank Record Id
                if (request.DTO.Id <= 0)
                    return ApiResponse<bool>.Fail("Invalid bank record id.");

                // 🔑 STEP 3: Permission check
                var permissions =
                    await _permissionService.GetPermissionsAsync(validation.RoleId);

                if (!permissions.Contains("DeleteBankInfo"))
                {
                    // optional strict block
                    // return ApiResponse<bool>.Fail("You do not have permission to delete bank info.");
                }

                // 📦 STEP 4: Fetch existing bank record
                var existing =
                    await _unitOfWork.EmployeeBankRepository
                        .GetSingleRecordAsync(request.DTO.Id, true);

                if (existing == null)
                    return ApiResponse<bool>.Fail("Bank record not found.");

                // 🔒 STEP 5: Ownership check Nahi karna hai permisssion to chck hai na

                 

                // 🗑️ STEP 6: Soft Delete
                existing.IsSoftDeleted = true;
                existing.IsActive = false;
                existing.DeletedDateTime = DateTime.UtcNow;
                existing.SoftDeletedById = loggedInEmployeeId;
                existing.HasChequeDocUploaded = false;
                existing.FilePath = null;
                existing.FileName = null;
                existing.FileType = 0;
                existing.UpdatedById = loggedInEmployeeId;
                existing.UpdatedDateTime = DateTime.UtcNow;

                bool deleted =
                    await _unitOfWork.EmployeeBankRepository
                        .DeleteAsync(existing);

                if (!deleted)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return ApiResponse<bool>.Fail("Failed to delete bank record.");
                }

                await _unitOfWork.CommitTransactionAsync();
                return ApiResponse<bool>.Success(true, "Bank record deleted successfully.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();

                _logger.LogError(
                    ex,
                    "Error deleting bank record | BankId: {Id}",
                    request.DTO?.Id);

                return ApiResponse<bool>.Fail(
                    "Error deleting bank record.",
                    new List<string> { ex.Message });
            }
        }
    }



}
