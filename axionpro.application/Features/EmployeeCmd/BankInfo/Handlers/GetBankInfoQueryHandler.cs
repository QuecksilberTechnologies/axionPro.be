using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.axionpro.application.Configuration;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.Constants;
using axionpro.application.DTOS.Employee.Bank;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.DTOS.Employee.Contact;
using axionpro.application.DTOS.Pagination;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.IRequestValidation;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace axionpro.application.Features.EmployeeCmd.BankInfo.Handlers
{
    public class GetBankInfoQuery : IRequest<ApiResponse<List<GetBankResponseDTO>>>
    {
        public GetBankReqestDTO DTO { get; set; }

        public GetBankInfoQuery(GetBankReqestDTO dTO)
        {
            DTO = dTO;
        }
    }
    public class GetBankInfoQueryHandler : IRequestHandler<GetBankInfoQuery, ApiResponse<List<GetBankResponseDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<GetBankInfoQueryHandler> _logger;
        private readonly ITokenService _tokenService;
        private readonly IPermissionService _permissionService;
        private readonly IConfiguration _config;
        private readonly IEncryptionService _encryptionService;
        private readonly IIdEncoderService _idEncoderService;
        private readonly ICommonRequestService _commonRequestService;


        public GetBankInfoQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<GetBankInfoQueryHandler> logger,
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

        public async Task<ApiResponse<List<GetBankResponseDTO>>> Handle(GetBankInfoQuery request, CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                string? savedFullPath = null;  // 📂 File full path track karne ke liye

                // 1️ COMMON VALIDATION (Mandatory)
                var validation = await _commonRequestService.ValidateRequestAsync(request.DTO.UserEmployeeId);

                if (!validation.Success)
                    return ApiResponse<List<GetBankResponseDTO>>.Fail(validation.ErrorMessage);

                // Assign decoded values coming from CommonRequestService
                  request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                  request.DTO.Prop.TenantId = validation.TenantId;
                  request.DTO.Prop.EmployeeId = RequestCommonHelper.DecodeOnlyEmployeeId(
                  request.DTO.EmployeeId,
                  validation.Claims.TenantEncriptionKey,
                  _idEncoderService
              );


                // ✅ Create  using repository
                var permissions = await _permissionService.GetPermissionsAsync(validation.RoleId);
                if (!permissions.Contains("AddBankInfo"))
                {
                    //await _unitOfWork.RollbackTransactionAsync();
                    //return ApiResponse<List<GetBankResponseDTO>>.Fail("You do not have permission to add bank info.");
                }


                // 🧩 STEP 4: Call Repository to get data GetBankReqestDTO dto, int id, long EmployeeId
                var bankEntities = await _unitOfWork.EmployeeBankRepository.GetInfoAsync(request.DTO);
                if (bankEntities == null || !bankEntities.Items.Any())
                    return ApiResponse<List<GetBankResponseDTO>>.Fail("No bank info found.");

                // 5️⃣ Projection (fastest approach)
                var result = ProjectionHelper.ToGetBankResponseDTOs(bankEntities, _idEncoderService, validation.Claims.TenantEncriptionKey, _config);

                // ✅ Correct paginated return
                return ApiResponse<List<GetBankResponseDTO>>.SuccessPaginatedPercentage(
                    Data: result,
                    Message: "Bank info retrieved successfully.",
                    PageNumber: bankEntities.PageNumber,
                    PageSize: bankEntities.PageSize,
                    TotalRecords: bankEntities.TotalCount,
                    TotalPages: bankEntities.TotalPages,
                    HasUploadedAll: bankEntities.HasUploadedAll,
                    CompletionPercentage: bankEntities.CompletionPercentage


                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching bank info for EmployeeId: {EmployeeId}", request.DTO?.EmployeeId);
                return ApiResponse<List<GetBankResponseDTO>>.Fail("Failed to fetch bank info.", new List<string> { ex.Message });
            }
        }

    }


}
 