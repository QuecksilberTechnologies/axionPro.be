using axionpro.application.DTOS.Employee.Education;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.EmployeeCmd.EducationInfo.Handlers
{
    public class DeleteEducationInfoQuery : IRequest<ApiResponse<bool>>
    {
        public DeleteEducationRequestDTO DTO { get; set; }

        public DeleteEducationInfoQuery(DeleteEducationRequestDTO dTO)
        {
            DTO = dTO;
        }
    }
    public class DeleteEducationInfoQueryHandler
  : IRequestHandler<DeleteEducationInfoQuery, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteEducationInfoQueryHandler> _logger;
        private readonly IPermissionService _permissionService;
        private readonly ICommonRequestService _commonRequestService;

        public DeleteEducationInfoQueryHandler(
            IUnitOfWork unitOfWork,
            ILogger<DeleteEducationInfoQueryHandler> logger,
            IPermissionService permissionService,
            ICommonRequestService commonRequestService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _permissionService = permissionService;
            _commonRequestService = commonRequestService;
        }

        public async Task<ApiResponse<bool>> Handle(
            DeleteEducationInfoQuery request,
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

                // 🔎 STEP 2: Validate Education Id
                if (request.DTO.Id <= 0)
                    return ApiResponse<bool>.Fail("Invalid education record id.");

                // 🔑 STEP 3: Permission check
                var permissions =
                    await _permissionService.GetPermissionsAsync(validation.RoleId);

                if (!permissions.Contains("DeleteEducationInfo"))
                {
                    // optional strict block
                    // return ApiResponse<bool>.Fail("You do not have permission to delete education info.");
                }

                // 📦 STEP 4: Fetch existing record
                var existing =
                    await _unitOfWork.EmployeeEducationRepository
                        .GetSingleRecordAsync(request.DTO.Id, true);

                if (existing == null)
                    return ApiResponse<bool>.Fail("Education record not found.");

                // 🔒 STEP 5: Ownership check
                if (existing.EmployeeId != loggedInEmployeeId)
                    return ApiResponse<bool>.Fail("Unauthorized access.");

                // 🗑️ STEP 6: SOFT DELETE
                existing.IsSoftDeleted = true;
                existing.IsActive = false;
                existing.DeletedDateTime = DateTime.UtcNow;
                existing.SoftDeletedById = loggedInEmployeeId;                   
                existing.FilePath = null;
                existing.FileName = null;
                existing.HasEducationDocUploded = false;
                existing.IsEditAllowed = false;

                bool deleted =
                    await _unitOfWork.EmployeeEducationRepository
                        .DeleteAsync(existing);

                if (!deleted)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return ApiResponse<bool>.Fail("Failed to delete education record.");
                }

                await _unitOfWork.CommitTransactionAsync();
                return ApiResponse<bool>.Success(true, "Education record deleted successfully.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();

                _logger.LogError(
                    ex,
                    "Error deleting education record | EducationId: {Id}",
                    request.DTO?.Id);

                return ApiResponse<bool>.Fail(
                    "Error deleting education record.",
                    new List<string> { ex.Message });
            }
        }
    }




}
