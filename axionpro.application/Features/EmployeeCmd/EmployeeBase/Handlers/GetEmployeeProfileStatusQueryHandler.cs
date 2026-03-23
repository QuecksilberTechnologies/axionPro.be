using axionpro.application.Common.Helpers.axionpro.application.Configuration;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.DTOS.Employee.CompletionPercentage;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

public class GetEmployeeProfileStatusQuery
    : IRequest<ApiResponse<List<CompletionSectionDTO>>>
{
    public string? EmployeeId { get; }

    public GetEmployeeProfileStatusQuery(string empid)
    {
        EmployeeId = empid;
    }
}

public class GetEmployeeProfileStatusQueryHandler
    : IRequestHandler<GetEmployeeProfileStatusQuery, ApiResponse<List<CompletionSectionDTO>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetEmployeeProfileStatusQueryHandler> _logger;
    private readonly IConfiguration _config;
    private readonly IIdEncoderService _idEncoderService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ICommonRequestService _commonRequestService;
    public GetEmployeeProfileStatusQueryHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetEmployeeProfileStatusQueryHandler> logger,
        IIdEncoderService idEncoderService,
        IConfiguration config,
        IHttpContextAccessor httpContextAccessor, ICommonRequestService commonRequestService)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _config = config;
        _idEncoderService = idEncoderService;
        _httpContextAccessor = httpContextAccessor;
        _commonRequestService = commonRequestService;
    }

    public async Task<ApiResponse<List<CompletionSectionDTO>>> Handle(
   GetEmployeeProfileStatusQuery request,
   CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("GetEmployeeProfileStatus started");

            // ===============================
            // 1️⃣ VALIDATION (AUTH CENTRALIZED)
            // ===============================
            var validation =
                await _commonRequestService.ValidateRequestAsync();

            if (!validation.Success)
                throw new UnauthorizedAccessException(validation.ErrorMessage);

            // ===============================
            // 2️⃣ NULL SAFETY
            // ===============================
            if (string.IsNullOrWhiteSpace(request.EmployeeId))
                throw new ValidationErrorException("Invalid EmployeeId.");

            // ===============================
            // 3️⃣ DECODE EMPLOYEE ID
            // ===============================
            var employeeId =
                _idEncoderService.DecodeId_long(
                    request.EmployeeId,
                    validation.Claims.TenantEncriptionKey);

            if (employeeId <= 0)
                throw new ValidationErrorException("Invalid EmployeeId.");

            // ===============================
            // 4️⃣ PERMISSION (YOUR PATTERN ✅)
            // ===============================
            //var hasAccess = await _permissionService.HasAccessAsync(
            //    validation.RoleId,
            //    Modules.Employee,
            //    Operations.View);

            //if (!hasAccess)
            //    throw new UnauthorizedAccessException("No permission to view profile status.");

            // ===============================
            // 5️⃣ FETCH DATA
            // ===============================
            var sections =
                await _unitOfWork.Employees
                    .GetEmployeeCompletionAsync(employeeId);

            var result = sections ?? new List<CompletionSectionDTO>();

            _logger.LogInformation("GetEmployeeProfileStatus success");

            // ===============================
            // 6️⃣ SUCCESS
            // ===============================
            return ApiResponse<List<CompletionSectionDTO>>
                .Success(
                    result,
                    result.Any()
                        ? "Employee profile completion retrieved."
                        : "No profile completion data found."
                );
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error fetching profile completion | EmployeeId: {EmployeeId}",
                request.EmployeeId);

            throw; // 🚨 MUST
        }
    }
}
