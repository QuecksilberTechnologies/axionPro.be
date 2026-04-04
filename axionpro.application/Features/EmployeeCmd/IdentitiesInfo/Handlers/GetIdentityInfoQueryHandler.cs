using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.DTOS.Employee.Sensitive;
using axionpro.application.DTOS.StoreProcedures;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.EmployeeCmd.IdentitiesInfo.Handlers
{
    #region Query Definition
    public class GetIdentityInfoQuery
        : IRequest<ApiResponse<List<GetEmployeeIdentityResponseDTO>>>
    {
        public GetIdentityRequestDTO DTO { get; }

        public GetIdentityInfoQuery(GetIdentityRequestDTO dto)
        {
            DTO = dto ?? throw new ArgumentNullException(nameof(dto));
        }
    }

    #endregion

    #region Query Handler

    public class GetIdentityInfoQueryHandler
 : IRequestHandler<GetIdentityInfoQuery, ApiResponse<List<GetEmployeeIdentityResponseDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetIdentityInfoQueryHandler> _logger;
        private readonly IPermissionService _permissionService;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IIdEncoderService _idEncoderService;

        public GetIdentityInfoQueryHandler(
            IUnitOfWork unitOfWork,
            ILogger<GetIdentityInfoQueryHandler> logger,
            IPermissionService permissionService,
            ICommonRequestService commonRequestService,
            IIdEncoderService idEncoderService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _permissionService = permissionService;
            _commonRequestService = commonRequestService;
            _idEncoderService = idEncoderService;
        }

        public async Task<ApiResponse<List<GetEmployeeIdentityResponseDTO>>> Handle(
   GetIdentityInfoQuery request,
   CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("GetIdentityInfo started");

                // ===============================
                // 1️⃣ VALIDATION
                // ===============================
                var validation =
                    await _commonRequestService
                        .ValidateRequestAsync();

                if (!validation.Success)
                    throw new UnauthorizedAccessException(validation.ErrorMessage);

                // ===============================
                // 2️⃣ NULL SAFETY
                // ===============================
                if (request?.DTO == null)
                    throw new ValidationErrorException("Invalid request.");

                // ===============================
                // 3️⃣ DECODE EMPLOYEE ID
                // ===============================
                var employeeId = validation.UserEmployeeId;



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
                //    throw new UnauthorizedAccessException("No permission to view identity info.");

                // ===============================
                // 5️⃣ FETCH DATA
                // ===============================
                var spRecords =
                    await _unitOfWork.StoreProcedureRepository
                        .GetIdentityRecordAsync(
                            employeeId,
                            request.DTO.CountryNationalityId,
                            true);

                // ===============================
                // 6️⃣ SAFE EMPTY HANDLING
                // ===============================
                var items = spRecords ?? new List<GetEmployeeIdentitySp>(); // 🔁 replace with actual type

                var response = items.Any()
                    ? ProjectionHelper.ToGetIdentityResponseDTO(
                        items,
                        _idEncoderService,
                        validation.Claims.TenantEncriptionKey)
                    : new List<GetEmployeeIdentityResponseDTO>();

                _logger.LogInformation("GetIdentityInfo success");

                // ===============================
                // 7️⃣ SUCCESS
                // ===============================
                return ApiResponse<List<GetEmployeeIdentityResponseDTO>>
                    .Success(
                        response,
                        response.Any()
                            ? "Identity information retrieved successfully."
                            : "Identity information not found."
                    );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching identity info");

                throw; // 🚨 MUST
            }
        }
    }


    #endregion
}
