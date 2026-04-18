using AutoMapper;
using axionpro.application.DTOS.TicketDTO.Classification;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace axionpro.application.Features.TickeAllCmd.Classification; 

public class AddClassificationCommand : IRequest<ApiResponse<GetClassificationResponseDTO>>
{
    public AddClassificationRequestDTO DTO { get; set; }

    public AddClassificationCommand(AddClassificationRequestDTO dto)
    {
        this.DTO = dto;
    }

}
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
            var validation = await _commonRequestService.ValidateRequestAsync();

            if (!validation.Success)
                throw new UnauthorizedAccessException(validation.ErrorMessage);

            // ===============================
            // 2️⃣ NULL SAFETY
            // ===============================
            if (request?.DTO == null)
                throw new ValidationErrorException(
                    "Invalid request.",
                    new List<string> { "DTO is required." });

            // ===============================
            // 3️⃣ SET COMMON PROPS
            // ===============================
            request.DTO.Prop.TenantId = validation.TenantId;
            request.DTO.Prop.EmployeeId = validation.UserEmployeeId;
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
            var result = await _unitOfWork.TicketClassificationRepository
                .AddAsync(request.DTO);

            if (result == null)
                throw new ApiException("Failed to create classification.", 500);

            // ===============================
            // 5️⃣ COMMIT
            // ===============================
            await _unitOfWork.CommitTransactionAsync();

            _logger.LogInformation("✅ Classification created successfully | Id={Id}", result.Id);

            return ApiResponse<GetClassificationResponseDTO>
                .Success(result, "Classification created successfully.");
        }
        catch (Exception ex)
        {
            // ===============================
            // 🔁 ROLLBACK
            // ===============================
            await _unitOfWork.RollbackTransactionAsync();

            _logger.LogError(ex, "❌ {Method} failed", methodName);

            throw; // 🔥 IMPORTANT (middleware handle करेगा)
        }
    }
}
