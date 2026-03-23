using AutoMapper;
using axionpro.application.Common.Helpers.ProjectionHelpers.Employee;
using axionpro.application.Common.Helpers.RequestHelper;
using axionpro.application.DTOs.Module;
using axionpro.application.DTOS.Employee.Contact;
using axionpro.application.DTOS.Pagination;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IEncryptionService;
using axionpro.application.Interfaces.IPermission;
using axionpro.application.Wrappers;

using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;

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
                _logger.LogInformation("CreateContact started");

                // ===============================
                // 1️⃣ VALIDATION
                // ===============================
                var validation = await _commonRequestService
                    .ValidateRequestAsync();

                if (!validation.Success)
                    throw new UnauthorizedAccessException(validation.ErrorMessage);

                // ===============================
                // 2️⃣ NULL SAFETY
                // ===============================
                if (request?.DTO == null)
                    throw new ValidationErrorException("Invalid request.");

                request.DTO.Prop ??= new();

                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;

                request.DTO.Prop.EmployeeId =
                    RequestCommonHelper.DecodeOnlyEmployeeId(
                        request.DTO.EmployeeId,
                        validation.Claims.TenantEncriptionKey,
                        _idEncoderService);

                if (request.DTO.Prop.EmployeeId <= 0)
                    throw new ValidationErrorException("Invalid EmployeeId.");

                // ===============================
                // 3️⃣ PERMISSION CHECK
                // ===============================
                //var hasAccess = await _permissionService.HasAccessAsync(
                //    validation.RoleId,
                //    Modules.Employee,
                //    Operations.Add);

                //if (!hasAccess)
                //    throw new UnauthorizedAccessException("No permission to add contact info.");

                // ===============================
                // 4️⃣ START TRANSACTION
                // ===============================
                await _unitOfWork.BeginTransactionAsync();

                // ===============================
                // 5️⃣ MAP ENTITY
                // ===============================
                var entity = _mapper.Map<EmployeeContact>(request.DTO);

                entity.EmployeeId = request.DTO.Prop.EmployeeId;
                entity.AddedById = request.DTO.Prop.UserEmployeeId;
                entity.AddedDateTime = DateTime.UtcNow;
                entity.IsActive = true;
                entity.IsSoftDeleted = false;
                entity.IsEditAllowed = true;
                entity.IsInfoVerified = false;
                entity.IsPrimary = request.DTO.IsPrimary;

                // ===============================
                // 6️⃣ SAVE
                // ===============================
                var responseDTO =
                    await _unitOfWork.EmployeeContactRepository
                        .CreateAsync(entity);

                if (responseDTO == null)
                    throw new ApiException("Failed to add contact info.", 500);

                // ===============================
                // 7️⃣ PROJECTION
                // ===============================
                var encryptedList =
                    ProjectionHelper.ToGetContactResponseDTOs(
                        responseDTO,
                        _idEncoderService,
                        validation.Claims.TenantEncriptionKey
                    );

                // ===============================
                // 8️⃣ COMMIT
                // ===============================
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("CreateContact success");

                // ===============================
                // 9️⃣ SUCCESS RESPONSE
                // ===============================
                return ApiResponse<List<GetContactResponseDTO>>.Success(
                    encryptedList,
                    "Contact info created successfully."
                );
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();

                _logger.LogError(
                    ex,
                    "Error adding contact info | EmployeeId: {EmployeeId}",
                    request.DTO?.EmployeeId);

                throw; // 🚨 MUST
            }
        }


    }


}

