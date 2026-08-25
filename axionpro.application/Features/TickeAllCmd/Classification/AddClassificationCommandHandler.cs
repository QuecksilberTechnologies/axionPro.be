// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines and handles Add Classification Command Handler requests.
// ================================================================

using AutoMapper;
using axionpro.application.DTOS.TicketDTO.Classification;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.TickeAllCmd.Classification;

#region Command


/// <summary>
/// Represents the AddClassificationCommand application component.
/// </summary>
public class AddClassificationCommand : IRequest<ApiResponse<GetClassificationResponseDTO>>
{
    public AddClassificationRequestDTO DTO { get; set; }

    public AddClassificationCommand(AddClassificationRequestDTO dto)
    {
        this.DTO = dto;
    }

}
/// <summary>
/// Handles AddClassificationCommand requests.
/// </summary>
#endregion

#region Handler

public class AddClassificationCommandHandler : IRequestHandler<AddClassificationCommand, ApiResponse<GetClassificationResponseDTO>>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICommonRequestService _commonRequestService;
    private readonly ILogger<AddClassificationCommandHandler> _logger;

    public AddClassificationCommandHandler(IMapper mapper, IUnitOfWork unitOfWork, ICommonRequestService commonRequestService, ILogger<AddClassificationCommandHandler> logger)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _commonRequestService = commonRequestService;
        _logger = logger;
    }

    public async Task<ApiResponse<GetClassificationResponseDTO>> Handle(AddClassificationCommand request, CancellationToken cancellationToken)
    {
        string methodName = nameof(AddClassificationCommandHandler);

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            _logger.LogInformation("🔹 {Method} started", methodName);

            // ===============================
            // 1️⃣ VALIDATION
            // ===============================
            var validation = await _commonRequestService.ValidateTenantUserRequestAsync();

            if (!validation.Success)
                throw new UnauthorizedAccessException(validation.ErrorMessage);

            // ===============================
            // 2️⃣ NULL SAFETY
            // ===============================
            if (request?.DTO == null)
                throw new ValidationErrorException(
                    "Invalid request.",
                    new List<string> { "DTO is required." });

            //var hasPermission = await _permissionService.HasAccessAsync(
            //    validation.RoleId,
            //    "TicketClassification",   // ModuleName (DB match)
            //    "Delete"                  // Operation
            //);

            //if (!hasPermission)
            //    throw new UnauthorizedAccessException("You do not have permission to delete classification.");
            // ===============================
            // 4️⃣ REPOSITORY CALL
            // ===============================
            var entity = _mapper.Map<axionpro.domain.Entity.TicketClassification>(request.DTO);
            entity.TenantId = validation.TenantId;
            entity.AddedById = validation.LoggedInEmployeeId;
            entity.AddedDateTime = DateTime.UtcNow;
            entity.IsSoftDeleted = false;

            var result = await _unitOfWork.TicketClassificationRepository.AddAsync(entity);

            if (result == null)
                throw new ApiException("Failed to create classification.", 500);

            // ===============================
            // 5️⃣ COMMIT
            // ===============================
            await _unitOfWork.CommitTransactionAsync();

            _logger.LogInformation("✅ Classification created successfully | Id={Id}", result.Id);

            var response = _mapper.Map<GetClassificationResponseDTO>(result);
            return ApiResponse<GetClassificationResponseDTO>
                .Success(response, "Classification created successfully.");
        }
        catch (Exception ex)
        {
            // ===============================
            //  ROLLBACK
            // ===============================
            await _unitOfWork.RollbackTransactionAsync();

            _logger.LogError(ex, "❌ {Method} failed", methodName);

            throw; //  IMPORTANT (middleware handle करेगा)
        }
    }
}

#endregion
