// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Updates tenant-owned asset categories from authenticated requests.
// ================================================================

using AutoMapper;
using axionpro.application.DTOS.AssetDTO.category;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using MediatR;

namespace axionpro.application.Features.AssetFeatures.Category.Handlers;

#region Command

/// <summary>Represents the request to update an asset category.</summary>
public class UpdateCategoryCommand : IRequest<ApiResponse<bool>>
{
    /// <summary>Initializes a new instance of the <see cref="UpdateCategoryCommand"/> class.</summary>
    public UpdateCategoryCommand(UpdateCategoryReqestDTO dto) => DTO = dto;

    /// <summary>Gets the client-supplied update values.</summary>
    public UpdateCategoryReqestDTO DTO { get; }
}

#endregion

#region Handler

/// <summary>Handles updates to tenant-owned asset categories.</summary>
public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, ApiResponse<bool>>
{
    #region Fields

    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICommonRequestService _commonRequestService;

    #endregion

    #region Constructor

    /// <summary>Initializes a new instance of the <see cref="UpdateCategoryCommandHandler"/> class.</summary>
    public UpdateCategoryCommandHandler(
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

    /// <inheritdoc />
    public async Task<ApiResponse<bool>> Handle(
        UpdateCategoryCommand request,
        CancellationToken cancellationToken)
    {
        if (request.DTO is null || request.DTO.Id <= 0 || string.IsNullOrWhiteSpace(request.DTO.CategoryName))
        {
            throw new ValidationErrorException(
                "A valid category identifier and name are required.",
                new List<string> { "Id must be greater than zero and CategoryName cannot be empty." });
        }

        // Resolve the trusted tenant-user context.
        var validation = await _commonRequestService.ValidateRequestAsync();
        if (!validation.Success)
        {
            throw new UnauthorizedAccessException(validation.ErrorMessage);
        }

        // Load the tenant-owned entity before applying client changes.
        var entity = await _unitOfWork.AssetCategoryRepository.GetByIdForTenantAsync(
            request.DTO.Id,
            validation.TenantId,
            cancellationToken);
        if (entity is null)
        {
            throw new ApiException("Category not found or update failed.", 404);
        }

        _mapper.Map(request.DTO, entity);
        entity.UpdatedById = validation.LoggedInEmployeeId;
        entity.UpdatedDateTime = DateTime.UtcNow;

        var updated = await _unitOfWork.AssetCategoryRepository.UpdateAsync(entity, cancellationToken);
        if (!updated)
        {
            throw new ApiException("Category not found or update failed.", 404);
        }

        return ApiResponse<bool>.Success(true, "Asset Category updated successfully.");
    }

    #endregion
}

#endregion
