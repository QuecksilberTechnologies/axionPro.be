using AutoMapper;
using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.DTOS.UserRoles;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.UserRolesCmd.Handlers
{
    // ===============================
    // COMMAND
    // ===============================
    public class GetAllUserRolesCommand : IRequest<ApiResponse<List<UserRoleDTO>>>
    {
        public GetUserRoleRequestDTO DTO { get; set; }

        public GetAllUserRolesCommand(GetUserRoleRequestDTO dto)
        {
            DTO = dto;
        }
    }

    // ===============================
    // HANDLER
    // ===============================
    public class GetAllUserRolesCommandHandler
    : IRequestHandler<GetAllUserRolesCommand, ApiResponse<List<UserRoleDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAllUserRolesCommandHandler> _logger;
        private readonly ICommonRequestService _commonRequestService;
        private readonly IIdEncoderService _idEncoderService;

        public GetAllUserRolesCommandHandler(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ILogger<GetAllUserRolesCommandHandler> logger,
            ICommonRequestService commonRequestService,
            IIdEncoderService idEncoderService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _commonRequestService = commonRequestService;
            _idEncoderService = idEncoderService;
        }

        public async Task<ApiResponse<List<UserRoleDTO>>> Handle(
            GetAllUserRolesCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("🚀 GetAllUserRoles started");

                // ===============================
                // 1️⃣ VALIDATION
                // ===============================
                var validation = await _commonRequestService.ValidateRequestAsync();

                if (!validation.Success)
                    throw new UnauthorizedAccessException(validation.ErrorMessage);

                if (request?.DTO == null)
                    throw new ValidationErrorException("Invalid request.");

                request.DTO.Prop ??= new();
                request.DTO.Prop.TenantId = validation.TenantId;

                // ===============================
                // 2️⃣ EMPLOYEE ID DECODE
                // ===============================
                if (string.IsNullOrEmpty(request.DTO.EmployeeId))
                    throw new ValidationErrorException("Invalid EmployeeId.");

                request.DTO.Prop.EmployeeId =
                    RequestCommonHelper.DecodeOnlyEmployeeId(
                        request.DTO.EmployeeId,
                        validation.Claims.TenantEncriptionKey,
                        _idEncoderService);

                // ===============================
                // 3️⃣ GET USER ROLES
                // ===============================
                var userRoles = await _unitOfWork.UserRoleRepository
                    .GetEmployeeRolesWithDetailsByIdAsync(
                        request.DTO.Prop.EmployeeId,
                        request.DTO.Prop.TenantId);

                if (userRoles == null || userRoles.Count == 0)
                {
                    _logger.LogWarning("No roles found for EmployeeId {EmployeeId}", request.DTO.EmployeeId);

                    return new ApiResponse<List<UserRoleDTO>>
                    {
                        IsSucceeded = true,
                        Message = "No roles found",
                        Data = new List<UserRoleDTO>()
                    };
                }

                // ===============================
                // 4️⃣ LOG ROLE IDS
                // ===============================
                var allRoleIdsCsv = string.Join(",",
                    userRoles
                        .Where(r => r.RoleId != null)
                        .Select(r => r.RoleId.ToString()));

                _logger.LogInformation(
                    "Fetched Role IDs for EmployeeId {EmployeeId}: {Roles}",
                    request.DTO.EmployeeId,
                    allRoleIdsCsv);

                // ===============================
                // 5️⃣ MAP TO DTO
                // ===============================
                var userRoleDTOs = _mapper.Map<List<UserRoleDTO>>(userRoles);

                // ===============================
                // 6️⃣ RETURN RESPONSE
                // ===============================
                return new ApiResponse<List<UserRoleDTO>>
                {
                    IsSucceeded = true,
                    Message = "Roles fetched successfully",
                    Data = userRoleDTOs
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error in GetAllUserRoles");
                throw; // middleware handle karega
            }
        }
    }


}
 