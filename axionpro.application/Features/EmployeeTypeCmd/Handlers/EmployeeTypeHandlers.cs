using axionpro.application.Common.Enums;
using axionpro.application.Constants;
using GetEmployeeTypeResponseDTO = axionpro.application.DTOs.EmployeeType.GetEmployeeTypeResponseDTO;
using axionpro.application.DTOS.Employee.Type;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using MediatR;

namespace axionpro.application.Features.EmployeeTypeCmd.Handlers;

#region Commands and queries
public sealed record CreateEmployeeTypeCommand(CreateEmployeeTypeDTO DTO)
    : IRequest<ApiResponse<GetEmployeeTypeResponseDTO>>;
public sealed record GetEmployeeTypesQuery(GetEmployeeTypeRequestDTO DTO)
    : IRequest<ApiResponse<List<GetEmployeeTypeResponseDTO>>>;
#endregion

#region Handlers
public sealed class CreateEmployeeTypeCommandHandler(IUnitOfWork unitOfWork,
    ICommonRequestService commonRequestService)
    : IRequestHandler<CreateEmployeeTypeCommand, ApiResponse<GetEmployeeTypeResponseDTO>>
{
    public async Task<ApiResponse<GetEmployeeTypeResponseDTO>> Handle(
        CreateEmployeeTypeCommand request, CancellationToken cancellationToken)
    {
        var operation = await unitOfWork.OperationRepository.GetOperationByIdAsync(request.DTO.OperationId);
        if (operation?.IsActive != true || operation.OperationType != (int)OperationType.Add)
        {
            throw new ForbiddenAccessException(AppConstants.ErrorMessages.PermissionDenied);
        }
        var actor = await commonRequestService.ValidateTenantUserRequestAsync();
        if (!actor.Success || actor.TenantId <= 0 || actor.LoggedInEmployeeId <= 0)
        {
            throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized);
        }
        var created = await unitOfWork.EmployeeTypeRepository.CreateAsync(
            actor.TenantId, actor.LoggedInEmployeeId, request.DTO, cancellationToken);
        return ApiResponse<GetEmployeeTypeResponseDTO>.Success(created, "EmployeeType created.");
    }
}

public sealed class GetEmployeeTypesQueryHandler(IUnitOfWork unitOfWork,
    ICommonRequestService commonRequestService)
    : IRequestHandler<GetEmployeeTypesQuery, ApiResponse<List<GetEmployeeTypeResponseDTO>>>
{
    public async Task<ApiResponse<List<GetEmployeeTypeResponseDTO>>> Handle(
        GetEmployeeTypesQuery request, CancellationToken cancellationToken)
    {
        if (request.DTO.PageNumber < 1 || request.DTO.PageSize is < 1 or > 100)
        {
            throw new ValidationErrorException("PageNumber must be positive; PageSize must be 1 to 100.");
        }
        var actor = await commonRequestService.ValidateTenantUserRequestAsync();
        if (!actor.Success || actor.TenantId <= 0)
        {
            throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized);
        }
        var all = await unitOfWork.EmployeeTypeRepository.GetAllAsync(actor.TenantId, cancellationToken);
        var offset = (long)(request.DTO.PageNumber - 1) * request.DTO.PageSize;
        var page = offset >= all.Count ? new List<GetEmployeeTypeResponseDTO>()
            : all.Skip((int)offset).Take(request.DTO.PageSize).ToList();
        var response = ApiResponse<List<GetEmployeeTypeResponseDTO>>.Success(page,
            AppConstants.SuccessMessages.EmployeeTypesRetrieved);
        response.PageNumber = request.DTO.PageNumber;
        response.PageSize = request.DTO.PageSize;
        response.TotalRecords = all.Count;
        response.TotalPages = (int)Math.Ceiling((double)all.Count / request.DTO.PageSize);
        return response;
    }
}
#endregion
