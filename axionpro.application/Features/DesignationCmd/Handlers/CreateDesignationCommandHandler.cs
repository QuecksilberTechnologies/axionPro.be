using AutoMapper;
using axionpro.application.DTOs.Designation;
using axionpro.application.Exceptions;
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


    public class CreateDesignationCommand : IRequest<ApiResponse<List<GetDesignationResponseDTO>>>
    {

        public CreateDesignationRequestDTO DTO { get; set; }

        public CreateDesignationCommand(CreateDesignationRequestDTO dto)
        {
            this.DTO = dto;
        }

    }


    /// <summary>
    /// Handles creation of a Designation.
    /// </summary>
    public class CreateDesignationCommandHandler : IRequestHandler<CreateDesignationCommand, ApiResponse<List<GetDesignationResponseDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CreateDesignationCommandHandler> _logger;
        private readonly ITokenService _tokenService;
        private readonly IPermissionService _permissionService;
        private readonly IConfiguration _config;
        private readonly IEncryptionService _encryptionService;
        private readonly IIdEncoderService _idEncoderService;
        private readonly ICommonRequestService _commonRequestService;

        public CreateDesignationCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<CreateDesignationCommandHandler> logger,
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
        public async Task<ApiResponse<List<GetDesignationResponseDTO>>> Handle(
   CreateDesignationCommand request,
   CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Creating Designation");

                // ===============================
                // 1️⃣ COMMON VALIDATION (AUTH + CONTEXT)
                // ===============================
                var validation = await _commonRequestService
                    .ValidateRequestAsync(request.DTO.UserEmployeeId);

                if (!validation.Success)
                    throw new UnauthorizedAccessException(validation.ErrorMessage);

                // ===============================
                // 2️⃣ NULL SAFETY
                // ===============================
                if (request?.DTO == null)
                    throw new ValidationErrorException(
                        "Invalid request.",
                        new List<string> { "Request DTO is required." }
                    );

                if (request.DTO.Prop == null)
                    request.DTO.Prop = new();

                // Assign values
                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;

                // ===============================
                // 3️⃣ PERMISSION CHECK (RBAC FIXED)
                // ===============================
                //var hasPermission = await _permissionService.HasAccessAsync(
                //    validation.RoleId,
                //    "Designation",   // 🔹 Module
                //    "Add"            // 🔹 Operation
                //);

                //if (!hasPermission)
                //    throw new UnauthorizedAccessException(
                //        "You do not have permission to create designation.");

                // ===============================
                // 4️⃣ BUSINESS VALIDATION
                // ===============================
                string? designationName = request.DTO.DesignationName?.Trim();

                if (string.IsNullOrWhiteSpace(designationName))
                    throw new ValidationErrorException(
                        "Designation name is required.",
                        new List<string> { "DesignationName cannot be empty." }
                    );

                request.DTO.DesignationName = designationName;

                // ===============================
                // 5️⃣ DUPLICATE CHECK (IMPORTANT)
                // ===============================
                bool isDuplicate = await _unitOfWork.DesignationRepository
                    .CheckDuplicateValueAsync(validation.UserEmployeeId, designationName);

                if (isDuplicate)
                    throw new ApiException("This designation name already exists.", 409);

                // ===============================
                // 6️⃣ CREATE DESIGNATION
                // ===============================
                var responseDTO = await _unitOfWork.DesignationRepository
                    .CreateAsync(request.DTO);

                if (responseDTO == null || responseDTO.Items == null || !responseDTO.Items.Any())
                    throw new ApiException("No designation was created.", 500);

                // ===============================
                // 7️⃣ COMMIT TRANSACTION (REQUIRED)
                // ===============================
                await _unitOfWork.CommitTransactionAsync();

                // ===============================
                // 8️⃣ SUCCESS RESPONSE
                // ===============================
                return new ApiResponse<List<GetDesignationResponseDTO>>
                {
                    IsSucceeded = true,
                    Message = $"{responseDTO.TotalCount} record(s) created successfully.",
                    PageNumber = responseDTO.PageNumber,
                    PageSize = responseDTO.PageSize,
                    TotalRecords = responseDTO.TotalCount,
                    TotalPages = responseDTO.TotalPages,
                    Data = responseDTO.Items
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating designation.");

                // ❗ IMPORTANT: middleware handle karega
                throw;
            }
        }
    }
}
