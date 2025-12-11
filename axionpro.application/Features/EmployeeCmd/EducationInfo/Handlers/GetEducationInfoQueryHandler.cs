using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.axionpro.application.Configuration;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.DTOs.Department;
using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Employee.Dependent;
using axionpro.application.DTOS.Employee.Education;
using axionpro.application.DTOS.Employee.Sensitive;
using axionpro.application.DTOS.Pagination;
using axionpro.application.Features.EmployeeCmd.DependentInfo.Handlers;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace axionpro.application.Features.EmployeeCmd.EducationInfo.Handlers
{
    public class GetEducationInfoQuery : IRequest<ApiResponse<List<GetEducationResponseDTO>>>
    {
        public GetEducationRequestDTO DTO { get; set; }

        public GetEducationInfoQuery(GetEducationRequestDTO dTO)
        {
            DTO = dTO;
        }
    }
    public class GetEducationInfoQueryHandler : IRequestHandler<GetEducationInfoQuery, ApiResponse<List<GetEducationResponseDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<GetEducationInfoQueryHandler> _logger;
        private readonly ITokenService _tokenService;
        private readonly IPermissionService _permissionService;
        private readonly IConfiguration _config;
        private readonly IEncryptionService _encryptionService;
        private readonly IIdEncoderService _idEncoderService;
        private readonly IConfiguration _configuration;


        public GetEducationInfoQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<GetEducationInfoQueryHandler> logger,
            ITokenService tokenService,
            IPermissionService permissionService,
            IConfiguration config,
          IEncryptionService encryptionService, IIdEncoderService idEncoderService, IConfiguration configuration)
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
            this._configuration = configuration;
        }


        public async Task<ApiResponse<List<GetEducationResponseDTO>>> Handle(GetEducationInfoQuery request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                await _unitOfWork.BeginTransactionAsync();
                // 🧩 STEP 1: Validate JWT Token
                var bearerToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
                    .ToString()?.Replace("Bearer ", "");
                request.DTO.Prop ??= new ExtraPropRequestDTO();

                if (string.IsNullOrEmpty(bearerToken))
                    return ApiResponse<List<GetEducationResponseDTO>>.Fail("Unauthorized: Token not found.");

                var secretKey = TokenKeyHelper.GetJwtSecret(_config);
                var tokenClaims = TokenClaimHelper.ExtractClaims(bearerToken, secretKey);

                if (tokenClaims == null || tokenClaims.IsExpired)
                    return ApiResponse<List<GetEducationResponseDTO>>.Fail("Invalid or expired token.");

                // 🧩 STEP 2: Validate Active User
                long loggedInEmpId = await _unitOfWork.CommonRepository.ValidateActiveUserLoginOnlyAsync(tokenClaims.UserId);
                if (loggedInEmpId < 1)
                {
                    _logger.LogWarning("❌ Invalid or inactive user. LoginId: {LoginId}", tokenClaims.UserId);
                    return ApiResponse<List<GetEducationResponseDTO>>.Fail("Unauthorized or inactive user.");
                }

                // 🧩 STEP 3: Tenant and Employee info validation from token
                string tenantKey = tokenClaims.TenantEncriptionKey ?? string.Empty;

                if (string.IsNullOrEmpty(request.DTO.UserEmployeeId) || string.IsNullOrEmpty(tenantKey))
                {
                    _logger.LogWarning("❌ Missing tenantKey or UserEmployeeId.");
                    return ApiResponse<List<GetEducationResponseDTO>>.Fail("User invalid.");
                }

                // Decrypt / convert values
                string finalKey = EncryptionSanitizer.SuperSanitize(tenantKey);
                //UserEmployeeId
                string UserEmpId = EncryptionSanitizer.CleanEncodedInput(request.DTO.UserEmployeeId);
                request.DTO.Prop.UserEmployeeId = _idEncoderService.DecodeId(UserEmpId, finalKey);
                //Token TenantId
                string tokenTenant = EncryptionSanitizer.CleanEncodedInput(tokenClaims.TenantId);
                request.DTO.Prop.TenantId = _idEncoderService.DecodeId(tokenTenant, finalKey);
                //Id              
                // Actual EmployeeId
                string actualEmpId = EncryptionSanitizer.CleanEncodedInput(request.DTO.EmployeeId);
                request.DTO.Prop.EmployeeId = _idEncoderService.DecodeId(actualEmpId, finalKey);
                 
               
                 

                // 🧩 STEP 4: Validate all employee references
               

                if (request.DTO.Prop.UserEmployeeId <= 0 || request.DTO.Prop.TenantId <= 0)
                {
                    _logger.LogWarning("❌ Tenant or employee information missing in token/request.");
                    return ApiResponse<List<GetEducationResponseDTO>>.Fail("Tenant or employee information missing.");
                }

                if (!(request.DTO.Prop.EmployeeId == loggedInEmpId))
                {
                    _logger.LogWarning(
                        "❌ EmployeeId mismatch. RequestEmpId: {ReqEmp}, LoggedEmpId: {LoggedEmp}",
                           request.DTO.Prop.EmployeeId, loggedInEmpId
                    );

                    return ApiResponse<List<GetEducationResponseDTO>>.Fail("Unauthorized: Employee mismatch.");
                }

                var permissions = await _permissionService.GetPermissionsAsync(SafeParser.TryParseInt(tokenClaims.RoleId));
                if (!permissions.Contains("AddBankInfo"))
                {
                    //  await _unitOfWork.RollbackTransactionAsync();
                    //return ApiResponse<List<GetBankResponseDTO>>.Fail("You do not have permission to add bank info.");


                    // 🧩 STEP 4: Call Repository to get data          
                    //  return ApiResponse<List<GetContactResponseDTO>>.Fail("You do not have permission to add identity info.");
                }

               
                // 4️⃣ Fetch Data from Repository
                PagedResponseDTO<GetEducationResponseDTO> Entity = await _unitOfWork.EmployeeEducationRepository.GetInfo(request.DTO);
                if (Entity == null || !Entity.Items.Any())
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return ApiResponse<List<GetEducationResponseDTO>>.Fail("No education info found.");
                }

                // 5️⃣ Projection + Encryption
             //   var encryptedResult = ProjectionHelper.ToGetEducationResponseDTOs(Entity.Items, _encryptionService, tenantKey, request.DTO.EmployeeId);
                var encryptedResult = ProjectionHelper.ToGetEducationResponseDTOs(Entity, _idEncoderService, tenantKey,  _configuration);

                // 6️⃣ Commit Transaction
                await _unitOfWork.CommitTransactionAsync();

                // 7️⃣ Final API Response
                return new ApiResponse<List<GetEducationResponseDTO>>
                {
                    IsSucceeded = true,
                    Message = $"{Entity.TotalCount} record(s) retrieved successfully.",
                    PageNumber = Entity.PageNumber,
                    PageSize = Entity.PageSize,
                    TotalRecords = Entity.TotalCount,
                    TotalPages = Entity.TotalPages,
                    Data = encryptedResult,
                    CompletionPercentage = Entity.CompletionPercentage,
                    HasAllDocUploaded = Entity.HasUploadedAll,
                    
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error while fetching Education info for EmployeeId: {EmployeeId}", request.DTO?.UserEmployeeId);
                return ApiResponse<List<GetEducationResponseDTO>>.Fail("Failed to fetch education info.", new List<string> { ex.Message });
            }
        }
    }
}
