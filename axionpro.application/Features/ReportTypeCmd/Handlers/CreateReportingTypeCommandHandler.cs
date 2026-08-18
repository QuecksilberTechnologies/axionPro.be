// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines and handles Create Reporting Type Command Handler requests.
// ================================================================

using AutoMapper;
using axionpro.application.DTOs.Manager.ReportingType;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.ReportTypeCmd.Handlers
{
    // ✅ Command
    public class CreateReportingTypeCommand : IRequest<ApiResponse<GetReportingTypeResponseDTO>>
    {
        public CreateReportingTypeRequestDTO DTO { get; set; }

        public CreateReportingTypeCommand(CreateReportingTypeRequestDTO dto)
        {
            DTO = dto;
        }
    }

    // ✅ Handler
    public class CreateReportingTypeCommandHandler
        : IRequestHandler<CreateReportingTypeCommand, ApiResponse<GetReportingTypeResponseDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateReportingTypeCommandHandler> _logger;
        private readonly ICommonRequestService _commonRequestService;

        public CreateReportingTypeCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<CreateReportingTypeCommandHandler> logger,
            ICommonRequestService commonRequestService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _commonRequestService = commonRequestService;
        }

        public async Task<ApiResponse<GetReportingTypeResponseDTO>> Handle(
            CreateReportingTypeCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("🔹 CreateReportingType started");

                // ===============================
                // 1️⃣ VALIDATION
                // ===============================
                var validation = await _commonRequestService.ValidateRequestAsync();

                if (!validation.Success)
                    throw new UnauthorizedAccessException(validation.ErrorMessage);

                // ===============================
                // 2️⃣ NULL CHECK
                // ===============================
                if (request?.DTO == null)
                    throw new ValidationErrorException("Invalid request data.");

                //var hasPermission = await _permissionService.HasAccessAsync(
                //    validation.RoleId,
                //    "TicketClassification",   // ModuleName (DB match)
                //    "Delete"                  // Operation
                //);

                //if (!hasPermission)
                //    throw new UnauthorizedAccessException("You do not have permission to delete classification.");
                // ===============================
                // ===============================
                // 3️⃣ REPOSITORY CALL
                // ===============================
                var entity = _mapper.Map<axionpro.domain.Entity.ReportingType>(request.DTO);
                entity.TenantId = validation.TenantId;
                entity.AddedById = validation.LoggedInEmployeeId;
                entity.AddedDateTime = DateTime.UtcNow;
                entity.IsSoftDeleted = false;

                var created = await _unitOfWork.ReportingTypeRepository.AddAsync(entity);

                if (created == null)
                    throw new Exception("ReportingType creation failed.");

                // ===============================
                // 4️⃣ SUCCESS
                // ===============================
                return ApiResponse<GetReportingTypeResponseDTO>
                    .Success(_mapper.Map<GetReportingTypeResponseDTO>(created), "ReportingType created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateReportingType");
                throw; // ✅ middleware handle karega
            }
        }
    }
}
