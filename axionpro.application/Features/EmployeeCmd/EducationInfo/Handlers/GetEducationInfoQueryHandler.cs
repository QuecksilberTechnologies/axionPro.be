using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.axionpro.application.Configuration;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.DTOs.Department;
using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.DTOS.Employee.Dependent;
using axionpro.application.DTOS.Employee.Education;
using axionpro.application.DTOS.Employee.Sensitive;
using axionpro.application.DTOS.Pagination;
using axionpro.application.Features.EmployeeCmd.DependentInfo.Handlers;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
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
using System.Security.Claims;
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
        private readonly ICommonRequestService _commonRequestService;


        public GetEducationInfoQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<GetEducationInfoQueryHandler> logger,
            ITokenService tokenService,
            IPermissionService permissionService,
            IConfiguration config,
          IEncryptionService encryptionService, IIdEncoderService idEncoderService, IConfiguration configuration, ICommonRequestService commonRequestService)
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
            _commonRequestService = commonRequestService;
        }


        public async Task<ApiResponse<List<GetEducationResponseDTO>>> Handle(GetEducationInfoQuery request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // 1️ COMMON VALIDATION (Mandatory)
                var validation = await _commonRequestService.ValidateRequestAsync(request.DTO.UserEmployeeId);

                if (!validation.Success)
                    return ApiResponse<List<GetEducationResponseDTO>>.Fail(validation.ErrorMessage);

                // Assign decoded values coming from CommonRequestService
                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;

                request.DTO.Prop.EmployeeId = RequestCommonHelper.DecodeOnlyEmployeeId( request.DTO.EmployeeId, validation.Claims.TenantEncriptionKey,  _idEncoderService  );
                // ✅ Create  using repository
                var permissions = await _permissionService.GetPermissionsAsync(validation.RoleId);
                if (!permissions.Contains("AddBankInfo"))
                {
                    //await _unitOfWork.RollbackTransactionAsync();
                    //return ApiResponse<List<GetBankResponseDTO>>.Fail("You do not have permission to add bank info.");
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
                var encryptedResult = ProjectionHelper.ToGetEducationResponseDTOs(Entity, _idEncoderService, validation.Claims.TenantEncriptionKey ,  _configuration);

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
