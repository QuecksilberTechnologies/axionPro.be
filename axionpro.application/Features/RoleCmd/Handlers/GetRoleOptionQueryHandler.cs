using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.axionpro.application.Configuration;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.DTOS.Designation;
using axionpro.application.DTOS.Role;
using axionpro.application.Features.DepartmentCmd.Handlers;
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
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.RoleCmd.Handlers
{
    public class GetRoleOptionQuery : IRequest<ApiResponse<List<GetRoleOptionResponseDTO>>>
    {
        public GetRoleOptionRequestDTO OptionDTO { get; set; }

        public GetRoleOptionQuery(GetRoleOptionRequestDTO optionDTO)
        {
            OptionDTO = optionDTO;
        }
    }

    public class GetRoleOptionQueryHandler : IRequestHandler<GetRoleOptionQuery, ApiResponse<List<GetRoleOptionResponseDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<GetRoleOptionQueryHandler> _logger;
        private readonly ITokenService _tokenService;
        private readonly IPermissionService _permissionService;
        private readonly IConfiguration _config;
        private readonly IEncryptionService _encryptionService;

        public GetRoleOptionQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<GetRoleOptionQueryHandler> logger,
            ITokenService tokenService,
            IPermissionService permissionService,
            IConfiguration config,
            IEncryptionService encryptionService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _tokenService = tokenService;
            _permissionService = permissionService;
            _config = config;
            _encryptionService = encryptionService;
        }

        public async Task<ApiResponse<List<GetRoleOptionResponseDTO>>> Handle(GetRoleOptionQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var bearerToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
                    .ToString()?.Replace("Bearer ", "");

                if (string.IsNullOrEmpty(bearerToken))
                    return ApiResponse<List<GetRoleOptionResponseDTO>>.Fail("Unauthorized: Token not found.");

                var secretKey = TokenKeyHelper.GetJwtSecret(_config);
                var tokenClaims = TokenClaimHelper.ExtractClaims(bearerToken, secretKey);

                if (tokenClaims == null || string.IsNullOrWhiteSpace(tokenClaims.TenantId))
                {
                    return ApiResponse<List<GetRoleOptionResponseDTO>>.Fail("Unauthorized: Invalid token or tenant.");
                }

                long decryptedTenantId = SafeParser.TryParseLong(tokenClaims.TenantId);

                if (decryptedTenantId <= 0)
                    return ApiResponse<List<GetRoleOptionResponseDTO>>.Fail("Unauthorized: Tenant not found.");

                var departments = await _unitOfWork.RoleRepository.GetOptionAsync(request.OptionDTO, decryptedTenantId);

                if (departments.Data == null || !departments.Data.Any())
                {
                    _logger.LogWarning("No departments found for tenant ID: {TenantId}", decryptedTenantId);

                    return new ApiResponse<List<GetRoleOptionResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = departments.Message,
                        Data = new List<GetRoleOptionResponseDTO>()
                    };
                }

                _logger.LogInformation("Successfully retrieved {Count} departments for tenant {TenantId}.",
                    departments.Data.Count, decryptedTenantId);

                return new ApiResponse<List<GetRoleOptionResponseDTO>>
                {
                    IsSucceeded = true,
                    Message = departments.Message,
                    Data = departments.Data
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching department options for request: {@Request}", request.OptionDTO);

                return new ApiResponse<List<GetRoleOptionResponseDTO>>
                {
                    IsSucceeded = false,
                    Message = "Failed to fetch department options due to an internal error.",
                    Data = new List<GetRoleOptionResponseDTO>()
                };
            }
        }
    }

}
