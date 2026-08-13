// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines and handles the request to delete a ModuleOperation mapping.
// ================================================================

using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using MediatR;

namespace axionpro.application.Features.ModuleCmd.Parent.Commands;

#region Command

/// <summary>
/// Represents the request to deactivate a module-operation mapping.
/// </summary>
public class DeleteModuleOperationCommand : IRequest<ApiResponse<bool>>
{
    /// <summary>
    /// Gets the mapping identifier to deactivate.
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteModuleOperationCommand"/> class.
    /// </summary>
    /// <param name="id">The mapping identifier to deactivate.</param>
    public DeleteModuleOperationCommand(int id)
    {
        Id = id;
    }
}

#endregion

#region Handler

/// <summary>
/// Handles Host-authorized deactivation of module-operation mappings.
/// </summary>
public class DeleteModuleOperationCommandHandler
    : IRequestHandler<DeleteModuleOperationCommand, ApiResponse<bool>>
{
    #region Fields

    private readonly IUnitOfWork _unitOfWork;
    private readonly ICommonRequestService _commonRequestService;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteModuleOperationCommandHandler"/> class.
    /// </summary>
    /// <param name="unitOfWork">Provides the existing module-operation mapping repository.</param>
    /// <param name="commonRequestService">Validates the current Host user request.</param>
    public DeleteModuleOperationCommandHandler(
        IUnitOfWork unitOfWork,
        ICommonRequestService commonRequestService)
    {
        _unitOfWork = unitOfWork;
        _commonRequestService = commonRequestService;
    }

    #endregion

    #region Handle

    /// <summary>
    /// Deactivates the requested mapping and records the authenticated Host user as the updater.
    /// </summary>
    /// <param name="request">The mapping deletion request.</param>
    /// <param name="cancellationToken">The token used to observe cancellation.</param>
    /// <returns>A successful response when the mapping is deactivated.</returns>
    public async Task<ApiResponse<bool>> Handle(
        DeleteModuleOperationCommand request,
        CancellationToken cancellationToken)
    {
        var hostUserId = await _commonRequestService.ValidateHostUserRequestAsync();
        cancellationToken.ThrowIfCancellationRequested();

        if (request is null || request.Id <= 0)
        {
            throw new ValidationErrorException("A valid module operation mapping ID is required.");
        }

        var isDeleted = await _unitOfWork.ModuleRepository
            .DeactivateModuleOperationMappingAsync(request.Id, hostUserId, cancellationToken);

        if (!isDeleted)
        {
            throw new ApiException("Module operation mapping not found.", 404);
        }

        return ApiResponse<bool>.Success(true, "Module operation deleted successfully.");
    }

    #endregion
}

#endregion
