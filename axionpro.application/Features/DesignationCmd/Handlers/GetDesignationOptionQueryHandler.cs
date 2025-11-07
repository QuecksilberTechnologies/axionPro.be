using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.axionpro.application.Configuration;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.DTOs.Department;
using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Department;
using axionpro.application.DTOS.Designation;
using axionpro.application.DTOS.Role;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace axionpro.application.Features.DesignationCmd.Handlers
{
    public class GetDesignationOptionQuery : IRequest<ApiResponse<List<GetDesignationOptionResponseDTO>>>
    {
        public GetDesignationOptionRequestDTO OptionDTO { get; set; }

        public GetDesignationOptionQuery(GetDesignationOptionRequestDTO optionDTO)
        {
            OptionDTO = optionDTO;
        }
    }

    public class GetDesignationOptionQueryHandler : IRequestHandler<GetDesignationOptionQuery, ApiResponse<List<GetDesignationOptionResponseDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<GetDesignationOptionQueryHandler> _logger;
        private readonly ITokenService _tokenService;
        private readonly IPermissionService _permissionService;
        private readonly IConfiguration _config;
        private readonly IEncryptionService _encryptionService;
        private readonly IIdEncoderService _idEncoderService    ;
      


        public GetDesignationOptionQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<GetDesignationOptionQueryHandler> logger,
            ITokenService tokenService,
            IPermissionService permissionService,
            IConfiguration config,
           IEncryptionService encryptionService, IIdEncoderService idEncoderService)
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
        }

        public async Task<ApiResponse<List<GetDesignationOptionResponseDTO>>> Handle(GetDesignationOptionQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var bearerToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
                    .ToString()?.Replace("Bearer ", "");

                if (string.IsNullOrEmpty(bearerToken))
                    return ApiResponse<List<GetDesignationOptionResponseDTO>>.Fail("Unauthorized: Token not found.");

                var secretKey = TokenKeyHelper.GetJwtSecret(_config);
                var tokenClaims = TokenClaimHelper.ExtractClaims(bearerToken, secretKey);

                if (tokenClaims == null || tokenClaims.IsExpired)
                    return ApiResponse<List<GetDesignationOptionResponseDTO>>.Fail("Invalid or expired token.");


                string tenantKey = tokenClaims.TenantEncriptionKey ?? string.Empty;

                if (string.IsNullOrEmpty(request.OptionDTO.UserEmployeeId) || string.IsNullOrEmpty(tenantKey))
                {
                    _logger.LogWarning("❌ Missing tenantKey or UserEmployeeId.");
                    return ApiResponse<List<GetDesignationOptionResponseDTO>>.Fail("User invalid.");
                }

                string finalKey = EncryptionSanitizer.SuperSanitize(tenantKey);
                string UserEmpId = EncryptionSanitizer.CleanEncodedInput(request.OptionDTO.UserEmployeeId);
                long decryptedEmployeeId = _idEncoderService.DecodeId(UserEmpId, finalKey);
                long decryptedTenantId = _idEncoderService.DecodeId(tokenClaims.TenantId, finalKey);

                if (decryptedTenantId <= 0)
                    return ApiResponse<List<GetDesignationOptionResponseDTO>>.Fail("Unauthorized: Tenant not found.");

                var departments = await _unitOfWork.DesignationRepository.GetOptionAsync(request.OptionDTO, decryptedTenantId);

                if (departments.Data == null || !departments.Data.Any())
                {
                    _logger.LogWarning("No departments found for tenant ID: {TenantId}", decryptedTenantId);

                    return new ApiResponse<List<GetDesignationOptionResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = departments.Message,
                        Data = new List<GetDesignationOptionResponseDTO>()
                    };
                }

                _logger.LogInformation("Successfully retrieved {Count} departments for tenant {TenantId}.",
                    departments.Data.Count, decryptedTenantId);

                return new ApiResponse<List<GetDesignationOptionResponseDTO>>
                {
                    IsSucceeded = true,
                    Message = departments.Message,
                    Data = departments.Data
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching department options for request: {@Request}", request.OptionDTO);

                return new ApiResponse<List<GetDesignationOptionResponseDTO>>
                {
                    IsSucceeded = false,
                    Message = "Failed to fetch department options due to an internal error.",
                    Data = new List<GetDesignationOptionResponseDTO >()
                };
            }
        }
    }
}
