using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.axionpro.application.Configuration;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.DTOS.Employee.CompletionPercentage;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IEncryptionService;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.EmployeeCmd.EmployeeBase.Handlers
{
    public class GetEmployeeProfileStatusQuery : IRequest<List<CompletionSectionDTO>>
    {
        public string? EmployeeId { get; }

        public GetEmployeeProfileStatusQuery(string empid)
        {
            EmployeeId = empid;
        }
    }

    public class GetEmployeeProfileStatusQueryHandler
        : IRequestHandler<GetEmployeeProfileStatusQuery, List<CompletionSectionDTO>>
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

        public async Task<List<CompletionSectionDTO>> Handle(
            GetEmployeeProfileStatusQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                // 🔐 Extract Bearer Token
                var bearerToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
                     .ToString()?.Replace("Bearer ", "");

                if (string.IsNullOrEmpty(bearerToken))
                    return new List<CompletionSectionDTO>();

                var secretKey = TokenKeyHelper.GetJwtSecret(_config);
                var tokenClaims = TokenClaimHelper.ExtractClaims(bearerToken, secretKey);

                if (tokenClaims == null || tokenClaims.IsExpired)
                    return new List<CompletionSectionDTO>();

                string finalKey = EncryptionSanitizer.SuperSanitize(secretKey);

                long employeeId = _idEncoderService.DecodeId(request.EmployeeId, finalKey);

                // 🔥 Fetch data from repository
                var result = await _unitOfWork.Employees.GetEmployeeCompletionAsync(employeeId);

                // 🔥 NO WRAPPER — returning only LIST
                return result ?? new List<CompletionSectionDTO>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error fetching profile completion status for EmployeeId: {EmployeeId}",
                    request.EmployeeId);

                return new List<CompletionSectionDTO>();
            }
        }
    }
}
