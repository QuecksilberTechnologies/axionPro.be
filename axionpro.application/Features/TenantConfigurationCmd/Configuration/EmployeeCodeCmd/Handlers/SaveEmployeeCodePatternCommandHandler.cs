using System.Globalization;
using System.Text.RegularExpressions;
using axionpro.application.Common.Enums;
using axionpro.application.DTOS.Tenant;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;

namespace axionpro.application.Features.TenantConfigurationCmd.Configuration.EmployeeCodeCmd.Handlers;

/// <summary>Previews or confirms a pattern insertion using the existing configuration permission pipeline.</summary>
public sealed record CreateEmployeeCodePatternCommand(SaveEmployeeCodePatternRequestDTO DTO)
    : IRequest<ApiResponse<SaveEmployeeCodePatternResponseDTO>>;

/// <summary>Previews or confirms a pattern change and current employee recoding.</summary>
public sealed record UpdateEmployeeCodePatternCommand(SaveEmployeeCodePatternRequestDTO DTO)
    : IRequest<ApiResponse<SaveEmployeeCodePatternResponseDTO>>;

/// <summary>Coordinates pattern validation and transactional repository changes.</summary>
public sealed class SaveEmployeeCodePatternCommandHandler(
    IUnitOfWork unitOfWork,
    ICommonRequestService commonRequestService)
    : IRequestHandler<CreateEmployeeCodePatternCommand, ApiResponse<SaveEmployeeCodePatternResponseDTO>>,
      IRequestHandler<UpdateEmployeeCodePatternCommand, ApiResponse<SaveEmployeeCodePatternResponseDTO>>
{
    #region Command Handlers

    public Task<ApiResponse<SaveEmployeeCodePatternResponseDTO>> Handle(
        CreateEmployeeCodePatternCommand request, CancellationToken cancellationToken)
    {
        return SaveAsync(request.DTO, true, cancellationToken);
    }

    public Task<ApiResponse<SaveEmployeeCodePatternResponseDTO>> Handle(
        UpdateEmployeeCodePatternCommand request, CancellationToken cancellationToken)
    {
        return SaveAsync(request.DTO, false, cancellationToken);
    }

    #endregion

    #region Validation And Persistence

    private async Task<ApiResponse<SaveEmployeeCodePatternResponseDTO>> SaveAsync(
        SaveEmployeeCodePatternRequestDTO dto, bool create, CancellationToken cancellationToken)
    {
        var context = await commonRequestService.ValidateTenantUserRequestAsync();
        if (!context.Success || context.TenantId <= 0 || context.LoggedInEmployeeId <= 0)
        {
            throw new UnauthorizedAccessException("A tenant employee session is required.");
        }

        var operation = await unitOfWork.OperationRepository.GetOperationByIdAsync(dto.OperationId);
        var expected = create ? OperationType.Add : OperationType.Update;
        if (operation?.IsActive != true || operation.OperationType != (int)expected)
        {
            throw new ForbiddenAccessException("The requested action requires its corresponding Add or Update permission.");
        }

        var input = dto.Pattern ?? throw new ValidationErrorException("Pattern is required.");
        var prefix = input.Prefix?.Trim().ToUpperInvariant() ?? string.Empty;
        var separator = input.Separator?.Trim() ?? string.Empty;
        // Preserve the established tenant-create/update validation contract.
        if (prefix.Length is < 1 or > 10 || !Regex.IsMatch(prefix, "^[A-Z]+$") ||
            separator is not ("_" or "/" or "-") ||
            !int.TryParse(input.RunningNumberLength, NumberStyles.None, CultureInfo.InvariantCulture, out var digits) ||
            digits is < 3 or > 7)
        {
            throw new ValidationErrorException("Use a 1–10 letter prefix, separator _, / or -, and running number length 3–7.");
        }

        var pattern = new EmployeeCodePattern
        {
            Prefix = prefix,
            Separator = separator,
            RunningNumberLength = digits,
            IncludeYear = input.IncludeYear,
            IncludeMonth = input.IncludeMonth,
            IncludeDepartment = input.IncludeDepartment
        };
        var result = await unitOfWork.TenantEmployeeCodePatternRepository.SaveWithEmployeeCodesAsync(
            context.TenantId, context.LoggedInEmployeeId, pattern,
            create, dto.Confirm, dto.PreviewHash, cancellationToken);
        return new ApiResponse<SaveEmployeeCodePatternResponseDTO>
        {
            IsSucceeded = true,
            Data = result,
            Message = result.Applied
                ? "Pattern and employee codes updated."
                : "Review all proposed employee codes. Nothing has been changed."
        };
    }

    #endregion
}
