using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.axionpro.application.Configuration;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.DTOs.Designation;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.IRequestValidation;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace axionpro.application.Features.DesignationCmd.Handlers
{
    public class UpdateDesignationCommand : IRequest<ApiResponse<bool>>
    {
        public UpdateDesignationRequestDTO DTO { get; set; }

        public UpdateDesignationCommand(UpdateDesignationRequestDTO dto)
        {
            DTO = dto;
        }
    }

    /// <summary>
    /// ✅ Ideal handler for updating an existing Designation (with JWT + Validation)
    /// </summary>
    public class UpdateDesignationCommandHandler : IRequestHandler<UpdateDesignationCommand, ApiResponse<bool>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateDesignationCommandHandler> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITokenService _tokenService;
        private readonly IPermissionService _permissionService;
        private readonly IConfiguration _config;
        private readonly IEncryptionService _encryptionService;
        private readonly IIdEncoderService _idEncoderService;
        private readonly ICommonRequestService _commonRequestService;

        public UpdateDesignationCommandHandler(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ILogger<UpdateDesignationCommandHandler> logger,
            IHttpContextAccessor httpContextAccessor,
            ITokenService tokenService,
            IPermissionService permissionService,
            IConfiguration config,
            IEncryptionService encryptionService, IIdEncoderService idEncoderService, ICommonRequestService commonRequestService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _tokenService = tokenService;
            _permissionService = permissionService;
            _config = config;
            _encryptionService = encryptionService;
            _idEncoderService = idEncoderService;
            _commonRequestService = commonRequestService;
        }

        public async Task<ApiResponse<bool>> Handle(UpdateDesignationCommand request, CancellationToken cancellationToken)
        {
            try
            {
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
                // 🧩 STEP 5: Validate name and existence
                string? designationName = request.DTO.DesignationName?.Trim();
                if (string.IsNullOrWhiteSpace(designationName))
                    return ApiResponse<bool>.Fail("Designation name should not be empty.");
 
                // 🧩 STEP 6: Update in repository
                bool isUpdated = await _unitOfWork.DesignationRepository.UpdateDesignationAsync(request.DTO);

                if (!isUpdated)
                {
                    _logger.LogWarning("⚠️ Update failed for DesignationId {Id}", request.DTO.Id);
                    return ApiResponse<bool>.Fail("No designation was updated.");
                }

                // 🧩 STEP 7: Commit transaction
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("✅ Designation updated successfully. Id: {Id}", request.DTO.Id);

                return ApiResponse<bool>.Success(true, "Designation updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while updating designation: {Message}", ex.Message);
                return ApiResponse<bool>.Fail($"An error occurred while updating designation. {ex.Message}");
            }
        }
    }
}
