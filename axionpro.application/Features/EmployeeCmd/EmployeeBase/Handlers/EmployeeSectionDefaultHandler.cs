using axionpro.application.Constants;
using axionpro.application.Common.Enums;
using axionpro.application.DTOs.BaseDTO;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using MediatR;

namespace axionpro.application.Features.EmployeeCmd.EmployeeBase.Handlers;

#region Request and Response

/// <summary>Configures the five operational sections for all current and future tenant employees.</summary>
public sealed class UpdateEmployeeSectionDefaultsRequestDTO : PermissionRequestDTO
{
    public bool IsEditAllowed { get; set; }
}

public sealed class EmployeeSectionDefaultResponseDTO
{
    public string ModuleCode { get; set; } = string.Empty;
    public bool IsEditAllowed { get; set; }
}

public sealed class UpdateEmployeeSectionDefaultsCommand(UpdateEmployeeSectionDefaultsRequestDTO dto)
    : IRequest<ApiResponse<List<EmployeeSectionDefaultResponseDTO>>>
{
    public UpdateEmployeeSectionDefaultsRequestDTO DTO { get; } = dto;
}

#endregion

#region Handler

/// <summary>Uses the Employee permission pipeline and trusted tenant context to update defaults atomically.</summary>
public sealed class UpdateEmployeeSectionDefaultsCommandHandler(
    IUnitOfWork unitOfWork,
    ICommonRequestService commonRequestService)
    : IRequestHandler<UpdateEmployeeSectionDefaultsCommand, ApiResponse<List<EmployeeSectionDefaultResponseDTO>>>
{
    public async Task<ApiResponse<List<EmployeeSectionDefaultResponseDTO>>> Handle(
        UpdateEmployeeSectionDefaultsCommand request,
        CancellationToken cancellationToken)
    {
        var validation = await commonRequestService.ValidateTenantUserRequestAsync();
        if (!validation.Success)
            throw new UnauthorizedAccessException(validation.ErrorMessage);
        if (validation.RoleTypeId != ConstantValues.RoleTypeAdmin)
            throw new ForbiddenAccessException(AppConstants.ErrorMessages.PermissionDenied);

        var operation = await unitOfWork.OperationRepository.GetOperationByIdAsync(request.DTO.OperationId);
        if (operation?.IsActive != true || operation.OperationType != (int)OperationType.Update)
            throw new ForbiddenAccessException(AppConstants.ErrorMessages.PermissionDenied);

        await unitOfWork.BeginTransactionAsync();
        try
        {
            var result = new List<EmployeeSectionDefaultResponseDTO>();
            foreach (var moduleCode in EmployeeOperationalSections.ModuleCodes)
            {
                await unitOfWork.Employees.SetOperationalSectionDefaultAsync(
                    validation.TenantId,
                    moduleCode,
                    request.DTO.IsEditAllowed,
                    validation.LoggedInEmployeeId,
                    cancellationToken);
                result.Add(new EmployeeSectionDefaultResponseDTO
                {
                    ModuleCode = moduleCode,
                    IsEditAllowed = request.DTO.IsEditAllowed
                });
            }

            await unitOfWork.CommitTransactionAsync();
            return ApiResponse<List<EmployeeSectionDefaultResponseDTO>>.Success(
                result, "Employee operational section defaults updated.");
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}

#endregion
