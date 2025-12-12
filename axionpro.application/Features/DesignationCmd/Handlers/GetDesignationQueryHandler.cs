using AutoMapper;
using axionpro.application.Common.Helpers;
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
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace axionpro.application.Features.DesignationCmd.Handlers
{
    public class GetDesignationQuery : IRequest<ApiResponse<List<GetDesignationResponseDTO>>>
    {
        public GetDesignationRequestDTO DTO { get; set; }

        public GetDesignationQuery(GetDesignationRequestDTO dto)
        {
            DTO = dto;
        }
    }

    public class GetDesignationQueryHandler :
        IRequestHandler<GetDesignationQuery, ApiResponse<List<GetDesignationResponseDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetDesignationQueryHandler> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPermissionService _permissionService;
        private readonly IConfiguration _config;
        private readonly IIdEncoderService _idEncoderService;
        private readonly ICommonRequestService _commonRequestService;

        public GetDesignationQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ILogger<GetDesignationQueryHandler> logger,
            IPermissionService permissionService,
            IConfiguration config,
            IIdEncoderService idEncoderService,
            ICommonRequestService commonRequestService
        )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _permissionService = permissionService;
            _config = config;
            _idEncoderService = idEncoderService;
            _commonRequestService = commonRequestService;
        }

        public async Task<ApiResponse<List<GetDesignationResponseDTO>>> Handle(GetDesignationQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // 1️⃣ COMMON VALIDATION (Mandatory)
                var validation = await _commonRequestService.ValidateRequestAsync(request.DTO.UserEmployeeId);

                if (!validation.Success)
                    return ApiResponse<List<GetDesignationResponseDTO>>.Fail(validation.ErrorMessage);

                // Assign decoded values coming from CommonRequestService
                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;

                // Clean sorting fields
                request.DTO.SortOrder = EncryptionSanitizer.CleanEncodedInput(request.DTO.SortOrder);
                request.DTO.SortBy = EncryptionSanitizer.CleanEncodedInput(request.DTO.SortBy);


                // 2️⃣ CHECK PERMISSION
                var permissions = await _permissionService.GetPermissionsAsync(SafeParser.TryParseInt(validation.Claims.RoleId));
                if (!permissions.Contains("AddBankInfo"))
                {
                    // await _unitOfWork.RollbackTransactionAsync();
                    //return ApiResponse<List<GetBankResponseDTO>>.Fail("You do not have permission to add bank info.");
                }
                // 3️⃣ REPOSITORY CALL
                var responseDTO = await _unitOfWork.DesignationRepository.GetAsync(request.DTO);

                if (responseDTO.Items == null || !responseDTO.Items.Any())
                {
                    _logger.LogInformation("⚠️ No designation found for TenantId: {TenantId}",
                        request.DTO.Prop.TenantId);

                    return new ApiResponse<List<GetDesignationResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "No records found.",
                        Data = new List<GetDesignationResponseDTO>(),
                        PageNumber = request.DTO.PageNumber,
                        PageSize = request.DTO.PageSize,
                        TotalRecords = 0,
                        TotalPages = 0
                    };
                }

                // 4️⃣ SUCCESS RESPONSE
                _logger.LogInformation("✅ {Count} designations retrieved successfully for TenantId: {TenantId}",
                    responseDTO.TotalCount, request.DTO.Prop.TenantId);

                return new ApiResponse<List<GetDesignationResponseDTO>>
                {
                    IsSucceeded = true,
                    Message = "Designations retrieved successfully.",
                    Data = responseDTO.Items,
                    PageNumber = responseDTO.PageNumber,
                    PageSize = responseDTO.PageSize,
                    TotalRecords = responseDTO.TotalCount,
                    TotalPages = responseDTO.TotalPages
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error fetching designations");

                return ApiResponse<List<GetDesignationResponseDTO>>
                    .Fail("An error occurred while fetching designations.");
            }
        }
    }
}
