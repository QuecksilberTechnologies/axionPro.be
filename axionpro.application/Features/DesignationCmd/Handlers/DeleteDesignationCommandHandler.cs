using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.axionpro.application.Configuration;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.DTOs.Designation;
using axionpro.application.Features.DepartmentCmd.Handlers;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
namespace axionpro.application.Features.DesignationCmd.Handlers
{
    public class DeleteDesignationQuery : IRequest<ApiResponse<bool>>
    {
        public DeleteDesignationRequestDTO DTO { get; set; }

        public DeleteDesignationQuery(DeleteDesignationRequestDTO dto)
        {
            DTO = dto;
        }
    }

    /// <summary>
    /// Handler for soft deleting a designation.
    /// </summary>
    public class DeleteDesignationQueryHandler : IRequestHandler<DeleteDesignationQuery, ApiResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<DeleteDesignationQueryHandler> _logger;
        private readonly ITokenService _tokenService;
        private readonly IPermissionService _permissionService;
        private readonly IConfiguration _config;
        private readonly IEncryptionService _encryptionService;
        private readonly IIdEncoderService _idEncoderService;
        private readonly ICommonRequestService _commonRequestService ;
        public DeleteDesignationQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<DeleteDesignationQueryHandler> logger,
            ITokenService tokenService,
            IPermissionService permissionService,
            IConfiguration config,
                IEncryptionService encryptionService, IIdEncoderService idEncoderService, ICommonRequestService commonRequestService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _tokenService = tokenService;
            _permissionService = permissionService;
            _config = config;
            _encryptionService = encryptionService;
            _idEncoderService = idEncoderService;
            _commonRequestService = commonRequestService;
        }

        public async Task<ApiResponse<bool>> Handle(DeleteDesignationQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // 🧩 STEP 1: Validate JWT Token
                //  COMMON VALIDATION (Mandatory)
                var validation = await _commonRequestService.ValidateRequestAsync(request.DTO.UserEmployeeId);

                if (!validation.Success)
                    return ApiResponse<bool>.Fail(validation.ErrorMessage);

                // Assign decoded values coming from CommonRequestService
                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;


                // ✅ Create  using repository
                var permissions = await _permissionService.GetPermissionsAsync(validation.RoleId);


                if (!permissions.Contains("AddBankInfo"))
                {
                    // await _unitOfWork.RollbackTransactionAsync();
                    //return ApiResponse<List<GetBankResponseDTO>>.Fail("You do not have permission to add bank info.");
                }



                _logger.LogInformation("🗑️ Attempting to delete RoleId: {RoleId} for TenantId: {TenantId}", request.DTO.Prop.TenantId, request.DTO.Prop.UserEmployeeId);

                // 🧩 STEP 6: Repository call             

                // 🧩 STEP 6: Call repository for delete
                var isDeleted = await _unitOfWork.DesignationRepository.DeleteDesignationAsync(request.DTO);

                if (isDeleted)
                {
                    _logger.LogInformation("✅ Designation deleted successfully. Id: {Id}, TenantId: {TenantId}",
                        request.DTO.Id, request.DTO.Prop.UserEmployeeId);

                    return ApiResponse<bool>.Success(true, "Designation deleted successfully.");
                }

                return new ApiResponse<bool>
                {
                    IsSucceeded = false,
                    Message = "Designation not found or could not be deleted.",
                    Data = false
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error while deleting designation Id: {Id}", request.DTO?.Id);
                return new ApiResponse<bool>
                {
                    IsSucceeded = false,
                    Message = "Failed to delete designation.",
                    Data = false
                };
            }
        }
    }
}
