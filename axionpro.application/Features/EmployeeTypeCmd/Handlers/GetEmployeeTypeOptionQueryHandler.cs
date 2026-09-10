using axionpro.application.Constants;
using axionpro.application.DTOS.Employee.Type;
using axionpro.application.DTOS.EmployeeType;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using MediatR;

namespace axionpro.application.Features.EmployeeTypeCmd.Handlers;

#region Query and handler
public sealed record GetEmployeeTypeOptionQuery(GetEmployeeTypeRequestDTO DTO)
    : IRequest<ApiResponse<List<GetEmployeeTypeResponseOptionDTO>>>;

/// <summary>Reads active tenant types after the existing module permission pipeline.</summary>
public sealed class GetEmployeeTypeOptionQueryHandler(
    IUnitOfWork unitOfWork, ICommonRequestService commonRequestService)
    : IRequestHandler<GetEmployeeTypeOptionQuery, ApiResponse<List<GetEmployeeTypeResponseOptionDTO>>>
{
    public async Task<ApiResponse<List<GetEmployeeTypeResponseOptionDTO>>> Handle(
        GetEmployeeTypeOptionQuery request, CancellationToken cancellationToken)
    {
        var actor = await commonRequestService.ValidateTenantUserRequestAsync();
        if (!actor.Success || actor.TenantId <= 0)
        {
            throw new UnauthorizedAccessException(AppConstants.ErrorMessages.Unauthorized);
        }
        var rows = await unitOfWork.EmployeeTypeRepository.GetAllAsync(actor.TenantId, cancellationToken);
        var options = rows.Where(row => row.IsActive == true)
            .Select(row => new GetEmployeeTypeResponseOptionDTO { Id = row.Id, TypeName = row.TypeName }).ToList();
        return ApiResponse<List<GetEmployeeTypeResponseOptionDTO>>.Success(options,
            AppConstants.SuccessMessages.EmployeeTypesRetrieved);
    }
}
#endregion
