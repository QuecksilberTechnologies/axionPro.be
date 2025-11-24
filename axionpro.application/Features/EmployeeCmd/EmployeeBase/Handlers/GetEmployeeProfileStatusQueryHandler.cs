using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.axionpro.application.Configuration;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.DTOS.Employee.CompletionPercentage;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IEncryptionService;
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

    public GetEmployeeProfileStatusQueryHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetEmployeeProfileStatusQueryHandler> logger,
        IIdEncoderService idEncoderService,
        IConfiguration config,
        IHttpContextAccessor httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _config = config;
        _idEncoderService = idEncoderService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ApiResponse<List<CompletionSectionDTO>>> Handle(
        GetEmployeeProfileStatusQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Token Extraction
            var bearerToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
                .ToString()?.Replace("Bearer ", "");

            if (string.IsNullOrEmpty(bearerToken))
                return ApiResponse<List<CompletionSectionDTO>>.Fail("Unauthorized: Token not found.");

            var secretKey = TokenKeyHelper.GetJwtSecret(_config);
            var tokenClaims = TokenClaimHelper.ExtractClaims(bearerToken, secretKey);

            if (tokenClaims == null || tokenClaims.IsExpired)
                return ApiResponse<List<CompletionSectionDTO>>.Fail("Invalid or expired token.");

            string finalKey = EncryptionSanitizer.SuperSanitize(secretKey);

            long employeeId = _idEncoderService.DecodeId(request.EmployeeId, finalKey);

            // Repository Call
            var sections = await _unitOfWork.Employees.GetEmployeeCompletionAsync(employeeId);

            return ApiResponse<List<CompletionSectionDTO>>
                .Response(sections, "Employee profile completion info retrieved successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error fetching profile completion status for EmployeeId: {EmployeeId}",
                request.EmployeeId);

            return ApiResponse<List<CompletionSectionDTO>>.Fail(
                "An unexpected error occurred.",
                new List<string> { ex.Message }
            );
        }
    }
}
