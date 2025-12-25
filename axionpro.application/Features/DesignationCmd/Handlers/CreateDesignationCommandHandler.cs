using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.axionpro.application.Configuration;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.DTOs.Department;
using axionpro.application.DTOs.Designation;
using axionpro.application.DTOs.Role;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.Features.EmployeeCmd.EmployeeBase.Handlers;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Interfaces.ITokenService;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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
        public async Task<ApiResponse<List<GetDesignationResponseDTO>>> Handle(CreateDesignationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                //  COMMON VALIDATION (Mandatory)
                var validation = await _commonRequestService.ValidateRequestAsync(request.DTO.UserEmployeeId);

                if (!validation.Success)
                    return ApiResponse<List<GetDesignationResponseDTO>>.Fail(validation.ErrorMessage);

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

                // ✅ Trim and validate DesignationName
                string? designationName = request.DTO.DesignationName?.Trim();
                if (string.IsNullOrWhiteSpace(designationName))
                {
                    return new ApiResponse<List<GetDesignationResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "Designation name should not be empty or whitespace.",
                        Data = null
                    };
                }
                request.DTO.DesignationName = designationName;

                // ✅ Check duplicate
                bool isDuplicate = await  _unitOfWork.DesignationRepository .CheckDuplicateValueAsync(request.DTO.Prop.UserEmployeeId, designationName);

                if (isDuplicate)
                {
                    return new ApiResponse<List<GetDesignationResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "This designation name already exists.",
                        Data = null
                    };
                }

                // ✅ Create designation using repository
                var responseDTO = await _unitOfWork.DesignationRepository.CreateAsync(request.DTO);

                if (responseDTO == null || responseDTO.Items == null || !responseDTO.Items.Any())
                {
                    return new ApiResponse<List<GetDesignationResponseDTO>>
                    {
                        IsSucceeded = false,
                        Message = "No designation was created.",
                        Data = new List<GetDesignationResponseDTO>() // empty list instead of null
                    };
                }

                // var encryptedList = ProjectionHelper.ToGetDesignationResponseDTOs(responseDTO.Items, _encryptionService, tenantKey);

                // 5️⃣ Commit transaction
                await _unitOfWork.CommitTransactionAsync();

                // 6️⃣ Return API response
                return new ApiResponse<List<GetDesignationResponseDTO>>
                {
                    IsSucceeded = true,
                    Message = $"{responseDTO.TotalCount} record(s) retrieved successfully.",
                    PageNumber = responseDTO.PageNumber,
                    PageSize = responseDTO.PageSize,
                    TotalRecords = responseDTO.TotalCount,
                    TotalPages = responseDTO.TotalPages,
                    Data = responseDTO.Items,
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<GetDesignationResponseDTO>>
                {
                    IsSucceeded = false,
                    Message = $"An error occurred: {ex.Message}",
                    Data = null
                };
            }
        }
    }
}
