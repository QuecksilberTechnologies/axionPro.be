using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Common.Helpers.axionpro.application.Configuration;
using axionpro.application.Common.Helpers.Converters;
using axionpro.application.Common.Helpers.EncryptionHelper;
using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.DTOS.Employee.Bank;
using axionpro.application.DTOS.Employee.Contact;
using axionpro.application.DTOS.Pagination;
using axionpro.application.Features.EmployeeCmd.Contact.Command;
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
using System.Threading;
using System.Threading.Tasks;

namespace axionpro.application.Features.EmployeeCmd.Contact.Handlers
{
    public class CreateContactInfoCommand : IRequest<ApiResponse<List<GetContactResponseDTO>>>
    {
        public CreateContactRequestDTO DTO { get; set; }

        public CreateContactInfoCommand(CreateContactRequestDTO dTO)
        {
            DTO = dTO;
        }
    }
    public class CreateContactInfoCommandHandler
      : IRequestHandler<CreateContactInfoCommand, ApiResponse<List<GetContactResponseDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateContactInfoCommandHandler> _logger;
        private readonly IPermissionService _permissionService;
        private readonly IConfiguration _config;
        private readonly IIdEncoderService _idEncoderService;
        private readonly ICommonRequestService _commonRequestService;

        public CreateContactInfoCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<CreateContactInfoCommandHandler> logger,
            IPermissionService permissionService,
            IConfiguration config,
            IIdEncoderService idEncoderService,
            ICommonRequestService commonRequestService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _permissionService = permissionService;
            _config = config;
            _idEncoderService = idEncoderService;
            _commonRequestService = commonRequestService;
        }

        public async Task<ApiResponse<List<GetContactResponseDTO>>> Handle(
            CreateContactInfoCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // 🔐 STEP 1: Common Validation (SAME AS BANK)
                var validation = await _commonRequestService
                    .ValidateRequestAsync();

                if (!validation.Success)
                    return ApiResponse<List<GetContactResponseDTO>>
                        .Fail(validation.ErrorMessage);

                // 🔓 STEP 2: Assign decoded values
                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;

                request.DTO.Prop.EmployeeId =
                    RequestCommonHelper.DecodeOnlyEmployeeId(
                        request.DTO.EmployeeId,
                        validation.Claims.TenantEncriptionKey,
                        _idEncoderService
                    );

                // 🔑 STEP 3: Permission check
                var permissions =
                    await _permissionService.GetPermissionsAsync(validation.RoleId);

                if (!permissions.Contains("AddContactInfo"))
                {
                    // optional hard-stop
                    // return ApiResponse<List<GetContactResponseDTO>>
                    //     .Fail("You do not have permission to add contact info.");
                }

                // 🧱 STEP 4: Map Entity
                var entity = _mapper.Map<EmployeeContact>(request.DTO);

                entity.EmployeeId = request.DTO.Prop.EmployeeId;
                entity.AddedById = request.DTO.Prop.UserEmployeeId;
                entity.AddedDateTime = DateTime.UtcNow;
                entity.IsActive = true;
                entity.IsSoftDeleted = false;              
                entity.IsEditAllowed = true;
                entity.IsInfoVerified = false;
                entity.IsPrimary = request.DTO.IsPrimary;

                // 💾 STEP 5: Save
                PagedResponseDTO<GetContactResponseDTO> responseDTO =
                    await _unitOfWork.EmployeeContactRepository
                        .CreateAsync(entity);

                // 🔐 STEP 6: Projection + Encrypt IDs
                var encryptedList =
                    ProjectionHelper.ToGetContactResponseDTOs(
                        responseDTO,
                        _idEncoderService,
                        validation.Claims.TenantEncriptionKey
                    );

                await _unitOfWork.CommitTransactionAsync();

                // 📦 STEP 7: API Response
                return new ApiResponse<List<GetContactResponseDTO>>
                {
                    IsSucceeded = true,
                    Message = $"{responseDTO.TotalCount} record(s) retrieved successfully.",
                    PageNumber = responseDTO.PageNumber,
                    PageSize = responseDTO.PageSize,
                    TotalRecords = responseDTO.TotalCount,
                    TotalPages = responseDTO.TotalPages,
                    Data = encryptedList
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();

                _logger.LogError(
                    ex,
                    "Error occurred while adding contact info for EmployeeId: {EmployeeId}",
                    request.DTO?.EmployeeId
                );

                return ApiResponse<List<GetContactResponseDTO>>
                    .Fail("Failed to add contact info.",
                          new List<string> { ex.Message });
            }
        }
    }


}

