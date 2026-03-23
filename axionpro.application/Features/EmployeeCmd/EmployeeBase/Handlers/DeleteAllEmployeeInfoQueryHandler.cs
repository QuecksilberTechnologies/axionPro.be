using AutoMapper;
using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;


namespace axionpro.application.Features.EmployeeCmd.EmployeeBase.Handlers
{
    public class DeleteEmployeeQuery : IRequest<ApiResponse<bool>>
    {
      public DeleteBaseEmployeeRequestDTO DTO;

        public DeleteEmployeeQuery(DeleteBaseEmployeeRequestDTO dto)
        {
            DTO = dto;
        }
    }
    public class DeleteBaseEmployeeInfoQueryHandler : IRequestHandler<DeleteEmployeeQuery, ApiResponse<bool>>
    {
        private readonly IBaseEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteBaseEmployeeInfoQueryHandler> _logger;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IIdEncoderService _idEncoderService;
        private readonly IPermissionService _permissionService;
        public DeleteBaseEmployeeInfoQueryHandler(
            IBaseEmployeeRepository employeeRepository,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            IUnitOfWork unitOfWork,
            ILogger<DeleteBaseEmployeeInfoQueryHandler> logger, ICommonRequestService commonRequestService, IIdEncoderService idEncoderService, IPermissionService permissionService)
        {
            _employeeRepository = employeeRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _commonRequestService = commonRequestService;
            _idEncoderService = idEncoderService;
            _permissionService = permissionService;
        }
        public async Task<ApiResponse<bool>> Handle(
    DeleteEmployeeQuery request,
    CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("DeleteEmployee started");

                // ===============================
                // 1️⃣ VALIDATION
                // ===============================
                var validation =
                    await _commonRequestService.ValidateRequestAsync();

                if (!validation.Success)
                    throw new UnauthorizedAccessException(validation.ErrorMessage);

                // ===============================
                // 2️⃣ NULL SAFETY
                // ===============================
                if (request?.DTO == null)
                    throw new ValidationErrorException("Invalid request.");

                request.DTO.Prop ??= new();

                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;

                request.DTO.Prop.EmployeeId =
                    RequestCommonHelper.DecodeOnlyEmployeeId(
                        request.DTO.EmployeeId,
                        validation.Claims.TenantEncriptionKey,
                        _idEncoderService);

                if (request.DTO.Prop.EmployeeId <= 0)
                    throw new ValidationErrorException("Invalid EmployeeId.");

                // ===============================
                // 3️⃣ PERMISSION (YOUR PATTERN ✅)
                // ===============================
                //var hasAccess = await _permissionService.HasAccessAsync(
                //    validation.RoleId,
                //    Modules.Employee,
                //    Operations.Delete);

                //if (!hasAccess)
                //    throw new UnauthorizedAccessException("No permission to delete employee.");

                // ===============================
                // 4️⃣ FETCH EMPLOYEE
                // ===============================
                var employee =
                    await _unitOfWork.Employees.GetByIdAsync(
                        request.DTO.Prop.EmployeeId,
                        request.DTO.Prop.TenantId,
                        true);

                if (employee == null)
                    throw new ApiException("Employee not found.", 404);

                // ===============================
                // 5️⃣ START TRANSACTION (IMPORTANT)
                // ===============================
                await _unitOfWork.BeginTransactionAsync();

                // ===============================
                // 6️⃣ DELETE (SOFT / CASCADE)
                // ===============================
                var isSuccess =
                    await _unitOfWork.Employees.DeleteAllAsync(employee);

                if (!isSuccess)
                    throw new ApiException("Failed to delete employee.", 500);

                // ===============================
                // 7️⃣ COMMIT
                // ===============================
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("DeleteEmployee success | Id: {Id}", request.DTO.Prop.EmployeeId);

                return ApiResponse<bool>.Success(true, "Employee deleted successfully.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();

                _logger.LogError(
                    ex,
                    "Error deleting employee | Id: {Id}",
                    request.DTO?.EmployeeId);

                throw; // 🚨 MUST
            }
        }

    }



}


