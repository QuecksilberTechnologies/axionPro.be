// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines and handles the request to create an Operation.
// ================================================================

using AutoMapper;
using axionpro.application.DTOs.Operation;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;

namespace axionpro.application.Features.OperationsMasterCmd.Handlers;

#region Command

/// <summary>
/// Represents the request to create an operation.
/// </summary>
public class CreateOperationCommand
    : IRequest<ApiResponse<List<GetOperationResponseDTO>>>
{
    /// <summary>
    /// Gets the operation details to create.
    /// </summary>
    public CreateOperationRequestDTO CreateOperationRequestDTO { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateOperationCommand"/> class.
    /// </summary>
    /// <param name="createOperationRequestDTO">The operation details to create.</param>
    public CreateOperationCommand(CreateOperationRequestDTO createOperationRequestDTO)
    {
        CreateOperationRequestDTO = createOperationRequestDTO;
    }
}

#endregion

#region Handler

/// <summary>
/// Handles the request to create an operation.
/// </summary>
public class CreateOperationCommandHandler
    : IRequestHandler<CreateOperationCommand, ApiResponse<List<GetOperationResponseDTO>>>
{
    #region Fields

    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICommonRequestService _commonRequestService;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateOperationCommandHandler"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work used to access persistence.</param>
    /// <param name="mapper">The mapper used to translate between DTOs and entities.</param>
    /// <param name="commonRequestService">The shared validator for the authenticated Host request.</param>
    public CreateOperationCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ICommonRequestService commonRequestService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _commonRequestService = commonRequestService;
    }

    #endregion

    #region Handle

    /// <summary>
    /// Creates an operation from the supplied request details.
    /// </summary>
    /// <param name="request">The create-operation request.</param>
    /// <param name="cancellationToken">The token used to observe cancellation.</param>
    /// <returns>A successful response containing the current operation list.</returns>
    public async Task<ApiResponse<List<GetOperationResponseDTO>>> Handle(
        CreateOperationCommand request,
        CancellationToken cancellationToken)
    {
        await _commonRequestService.ValidateHostUserRequestAsync();
        cancellationToken.ThrowIfCancellationRequested();

        var dto = request?.CreateOperationRequestDTO
            ?? throw new ValidationErrorException("Operation details are required.");

        if (dto.ProductOwnerId <= 0)
        {
            throw new ValidationErrorException("A valid product owner ID is required.");
        }

        if (string.IsNullOrWhiteSpace(dto.OperationName))
        {
            throw new ValidationErrorException("Operation name is required.");
        }

        if (dto.OperationType <= 0)
        {
            throw new ValidationErrorException("A valid operation type is required.");
        }

        var operation = _mapper.Map<Operation>(dto);
        operation.OperationName = dto.OperationName.Trim();
        operation.AddedById = dto.ProductOwnerId;
        operation.AddedDateTime = DateTime.UtcNow;
        operation.UpdatedById = null;
        operation.UpdateDateTime = null;

        var operations = await _unitOfWork.OperationRepository
            .CreateOperationAsync(operation);

        var response = _mapper.Map<List<GetOperationResponseDTO>>(operations);

        return ApiResponse<List<GetOperationResponseDTO>>.Success(
            response,
            "Operation created successfully.");
    }

    #endregion
}

#endregion
