using AutoMapper;
using axionpro.application.DTOs.OrganizationHolidayCalendar;
using axionpro.application.DTOS.Role;
using axionpro.application.DTOS.TicketDTO.TicketType;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.TickeAllCmd.TicketType.Handlers
{
    public class CreateTicketTypeCommand
      : IRequest<ApiResponse<GetTicketTypeResponseDTO>>
    {
        public AddTicketTypeRequestDTO DTO { get; }

        public CreateTicketTypeCommand(AddTicketTypeRequestDTO dto)
        {
            DTO = dto;
        }
    }

    public class CreateTicketTypeCommandHandler
        : IRequestHandler<CreateTicketTypeCommand, ApiResponse<GetTicketTypeResponseDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateTicketTypeCommandHandler> _logger;
        private readonly ICommonRequestService _commonRequestService;

        public CreateTicketTypeCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<CreateTicketTypeCommandHandler> logger,
            ICommonRequestService commonRequestService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _commonRequestService = commonRequestService;
        }

        public async Task<ApiResponse<GetTicketTypeResponseDTO>> Handle(
            CreateTicketTypeCommand request,
            CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // ===============================
                // 1️⃣ COMMON VALIDATION
                // ===============================
                var validation = await _commonRequestService.ValidateRequestAsync();

                if (!validation.Success)
                    throw new UnauthorizedAccessException(validation.ErrorMessage);

                // ===============================
                // 2️⃣ RBAC CHECK 🔥
                // ===============================
                //await _commonRequestService.HasAccessAsync(
                //    ModuleEnum.Ticket,
                //    OperationEnum.Create);

                //// ===============================
                //// 3️⃣ NULL SAFETY
                //// ===============================
                //if (request?.DTO == null)
                //    throw new ValidationErrorException("Invalid request data.");

              //   request.DTO.Prop ??= new BaseRequestDTO();

                request.DTO.Prop.UserEmployeeId = validation.UserEmployeeId;
                request.DTO.Prop.TenantId = validation.TenantId;

                var dto = request.DTO;

                // ===============================
                // 4️⃣ VALIDATIONS
                // ===============================
                if (string.IsNullOrWhiteSpace(dto.TicketTypeName))
                    throw new ValidationErrorException("TicketTypeName is required.");

                if (dto.IsApprovalRequired && dto.ApprovalRoleId == null)
                    throw new ValidationErrorException("ApprovalRoleId is required.");

                if (dto.SLAHours != null && dto.SLAHours <= 0)
                    throw new ValidationErrorException("SLAHours must be greater than 0.");

                // ===============================
                // 5️⃣ FK VALIDATION
                // ===============================
                var header = await _unitOfWork.TicketHeaderRepository.GetByIdAsync(dto.TicketHeaderId);

                if (header == null)
                    throw new ValidationErrorException("Invalid TicketHeaderId.");
                GetSingleRoleRequestDTO getSingleRoleRequestDTO = new GetSingleRoleRequestDTO()
                {
                    Id = dto.ResponsibleRoleId  ,
                    
                };      

                var roleRepository = await _unitOfWork.RoleRepository.GetByIdAsync1(getSingleRoleRequestDTO);

                if (roleRepository == null)
                    throw new ValidationErrorException("Invalid ResponsibleRoleId.");


                if (dto.IsApprovalRequired)
                {
                    getSingleRoleRequestDTO.Id = dto.ApprovalRoleId ?? 0;
                    var approvalRole = await _unitOfWork.RoleRepository.GetByIdAsync1(getSingleRoleRequestDTO);
                  

                    if (approvalRole == null)
                        throw new ValidationErrorException("Invalid ApprovalRoleId.");
                }

                // ===============================
                // 6️⃣ MAP DTO → ENTITY
                // ===============================
                var entity = _mapper.Map<domain.Entity.TicketType>(dto);

                // ===============================
                // 7️⃣ SYSTEM FIELDS
                // ===============================
                entity.TenantId = validation.TenantId;
                entity.AddedById = validation.UserEmployeeId;
                entity.AddedDateTime = DateTime.UtcNow;

                entity.IsActive = true;
                entity.IsSoftDeleted = false;

                entity.UpdatedById = null;
                entity.UpdatedDateTime = null;

                // ===============================
                // 8️⃣ SAVE DATA
                // ===============================
                var inserted = await _unitOfWork.TicketTypeRepository
                    .AddAsync(entity);

                if (inserted == null)
                    throw new ApiException("TicketType creation failed.", 500);

                // ===============================
                // 9️⃣ RESPONSE MAPPING
                // ===============================
               

                // ===============================
                // 🔟 COMMIT
                // ===============================
                await _unitOfWork.CommitTransactionAsync();

                return ApiResponse<GetTicketTypeResponseDTO>
                    .Success(inserted, "TicketType created successfully.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();

                _logger.LogError(ex, "CreateTicketType failed");

                throw;
            }
        }
    }
}