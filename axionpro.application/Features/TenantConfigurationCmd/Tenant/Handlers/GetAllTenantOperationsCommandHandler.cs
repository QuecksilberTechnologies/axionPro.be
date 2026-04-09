using AutoMapper;
using axionpro.application.DTOs.Tenant;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.TenantConfigurationCmd.Tenant.Handlers
{
    // ✅ FIXED COMMAND
    public class GetAllTenantOperationsCommand : IRequest<ApiResponse<TenantEnabledOperationsResponseDTO>>
    {
        public TenantEnabledOperationsRequestDTO Request { get; set; }

        public GetAllTenantOperationsCommand(TenantEnabledOperationsRequestDTO request)
        {
            Request = request;
        }
    }

    // ✅ FIXED HANDLER
    public class GetAllTenantOperationsCommandHandler
        : IRequestHandler<GetAllTenantOperationsCommand, ApiResponse<TenantEnabledOperationsResponseDTO>>
    {
        private readonly ITenantModuleConfigurationRepository _tenantModuleConfigurationRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAllTenantOperationsCommandHandler> _logger;
        private readonly ICommonRequestService _commonRequestService;

        public GetAllTenantOperationsCommandHandler(
            ITenantModuleConfigurationRepository tenantModuleConfigurationRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ILogger<GetAllTenantOperationsCommandHandler> logger,
            ICommonRequestService commonRequestService)
        {
            _tenantModuleConfigurationRepository = tenantModuleConfigurationRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _commonRequestService = commonRequestService;
        }

        public async Task<ApiResponse<TenantEnabledOperationsResponseDTO>> Handle(
            GetAllTenantOperationsCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("🔹 GetAllTenantOperations started");

                // ===============================
                // 1️⃣ VALIDATION
                // ===============================
                if (request?.Request == null)
                    throw new ValidationErrorException("Invalid request data.");

                var validation = await _commonRequestService
                    .ValidateRequestAsync();

                if (!validation.Success)
                    throw new UnauthorizedAccessException(validation.ErrorMessage);

                // ===============================
                // 2️⃣ PERMISSION CHECK (RBAC)
                // ===============================
                //var hasAccess = await _permissionService.HasAccessAsync(
                //    validation.RoleId,
                //    Modules.Role,
                //    Operations.Add);

                //if (!hasAccess)
                //    throw new UnauthorizedAccessException("Access denied.");
                            
                 
                // ===============================
                // 2️⃣ SET CONTEXT
                // ===============================
                request.Request.Prop.TenantId = validation.TenantId;
                TenantEnabledOperation tenantEnabledOperation = new TenantEnabledOperation
                {
                    TenantId = validation.TenantId,
                     
                };
                // ===============================
                // 3️⃣ FETCH DATA
                // ===============================
                var responseDTO = await _unitOfWork
                    .UserRolesPermissionOnModuleRepository.GetAllTenantModuleWithOperation(tenantEnabledOperation);

                // ===============================
                // 4️⃣ RESPONSE
                // ===============================
                return new ApiResponse<TenantEnabledOperationsResponseDTO>
                {
                    IsSucceeded = true,
                    Message = "Modules and operations fetched successfully.",
                    Data = responseDTO
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching module operations for tenant.");

                return new ApiResponse<TenantEnabledOperationsResponseDTO>
                {
                    IsSucceeded = false,
                    Message = "Something went wrong while fetching module operations.",
                    Data = null
                };
            }
        }
    }
}
 