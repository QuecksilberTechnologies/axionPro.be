using axionpro.application.Common.Helpers;
using axionpro.application.DTOs.BaseDTO;
using axionpro.application.DTOs.OrganizationHolidayCalendar;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;

namespace axionpro.application.Features.HolidayCalandarCmd;

#region Requests

public sealed record ListHolidaysQuery(BasicRequestDTO DTO)
    : IRequest<ApiResponse<IReadOnlyList<OrganizationHolidayCalendarDTO>>>;

public sealed record GetHolidayQuery(HolidayByIdRequestDTO DTO)
    : IRequest<ApiResponse<OrganizationHolidayCalendarDTO>>;

public sealed record CreateHolidayCommand(SaveHolidayRequestDTO DTO)
    : IRequest<ApiResponse<OrganizationHolidayCalendarDTO>>;

public sealed record UpdateHolidayCommand(UpdateHolidayRequestDTO DTO)
    : IRequest<ApiResponse<OrganizationHolidayCalendarDTO>>;

public sealed record DeleteHolidayCommand(HolidayByIdRequestDTO DTO)
    : IRequest<ApiResponse<bool>>;

public sealed record ImportHolidaysCommand(ImportHolidayRequestDTO DTO)
    : IRequest<ApiResponse<HolidayImportResultDTO>>;

public sealed record ExportHolidaysQuery(BasicRequestDTO DTO)
    : IRequest<byte[]>;

#endregion

#region Permission pipeline

public sealed class HolidayCalendarPermissionBehavior<TRequest, TResponse>(
    IUnitOfWork unitOfWork,
    ICommonRequestService commonRequestService)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is not ListHolidaysQuery
            and not GetHolidayQuery
            and not CreateHolidayCommand
            and not UpdateHolidayCommand
            and not DeleteHolidayCommand
            and not ImportHolidaysCommand
            and not ExportHolidaysQuery)
        {
            return await next();
        }

        var dto = request switch
        {
            ListHolidaysQuery query => (PermissionRequestDTO)query.DTO,
            GetHolidayQuery query => query.DTO,
            CreateHolidayCommand command => command.DTO,
            UpdateHolidayCommand command => command.DTO,
            DeleteHolidayCommand command => command.DTO,
            ImportHolidaysCommand command => command.DTO,
            ExportHolidaysQuery query => query.DTO,
            _ => throw new ValidationErrorException("Holiday permission request is required.")
        };

        if (dto.ModuleId <= 0 || dto.OperationId <= 0)
        {
            throw new ValidationErrorException("ModuleId and OperationId are required.");
        }

        var moduleCode = await commonRequestService.GetModuleCodeAsync(dto.ModuleId);
        if (!string.Equals(moduleCode, "TENANT_POLICY_HOLIDAY_CALENDAR", StringComparison.OrdinalIgnoreCase))
        {
            throw new ForbiddenAccessException("The selected module is not valid for holiday calendar.");
        }

        var expectedOperation = request switch
        {
            CreateHolidayCommand => "Add",
            UpdateHolidayCommand => "Update",
            DeleteHolidayCommand => "Delete",
            ImportHolidaysCommand => "Import",
            ExportHolidaysQuery => "Export",
            _ => "View"
        };

        var operation = await unitOfWork.OperationRepository.GetOperationByIdAsync(dto.OperationId);
        if (!string.Equals(operation?.OperationName, expectedOperation, StringComparison.OrdinalIgnoreCase))
        {
            throw new ForbiddenAccessException("The selected operation is not valid for this holiday action.");
        }

        var actor = await commonRequestService.ValidateTenantUserRequestAsync();
        if (!actor.Success || actor.TenantId <= 0 || actor.LoggedInEmployeeId <= 0 || actor.RoleId <= 0)
        {
            throw new UnauthorizedAccessException(actor.ErrorMessage ?? "Unauthorized request.");
        }

        var permission = await unitOfWork.StoreProcedureRepository.CheckTenantEmployeePermissionAsync(
            actor.TenantId,
            actor.LoggedInEmployeeId,
            actor.RoleId,
            dto.ModuleId,
            dto.OperationId,
            cancellationToken);

        TenantRuntimePermissionValidator.EnsureAllowed(permission);
        return await next();
    }
}

#endregion

#region Handlers

internal static class HolidayCalendarValidation
{
    public static void Validate(long tenantLocationId, string name, DateOnly date, string? description)
    {
        if (tenantLocationId <= 0)
        {
            throw new ValidationErrorException("TenantLocationId is required.");
        }

        if (string.IsNullOrWhiteSpace(name) || name.Trim().Length > 100)
        {
            throw new ValidationErrorException("HolidayName must contain 1 to 100 characters.");
        }

        if (date == default)
        {
            throw new ValidationErrorException("HolidayDate is required.");
        }

        if (description?.Length > 255)
        {
            throw new ValidationErrorException("Description must not exceed 255 characters.");
        }
    }

    public static OrganizationHolidayCalendarDTO ToDTO(OrganizationHolidayCalendar holiday)
    {
        return new OrganizationHolidayCalendarDTO
        {
            Id = holiday.Id,
            TenantId = holiday.TenantId ?? 0,
            TenantLocationId = holiday.TenantLocationId,
            HolidayName = holiday.HolidayName,
            HolidayDate = holiday.HolidayDate,
            IsOptional = holiday.IsOptional,
            Description = holiday.Description
        };
    }
}

public sealed class ListHolidaysQueryHandler(
    IUnitOfWork unitOfWork,
    ICommonRequestService commonRequestService)
    : IRequestHandler<ListHolidaysQuery, ApiResponse<IReadOnlyList<OrganizationHolidayCalendarDTO>>>
{
    public async Task<ApiResponse<IReadOnlyList<OrganizationHolidayCalendarDTO>>> Handle(
        ListHolidaysQuery request,
        CancellationToken cancellationToken)
    {
        var actor = await commonRequestService.ValidateTenantUserRequestAsync();
        if (request.DTO.TenantLocationId <= 0 || request.DTO.HolidayYear is < 1 or > 9999)
        {
            throw new ValidationErrorException("TenantLocationId and HolidayYear must be valid when supplied.");
        }

        var holidays = await unitOfWork.HolidayCalandarRepository.GetTenantHolidaysAsync(
            actor.TenantId,
            request.DTO.TenantLocationId,
            request.DTO.HolidayYear,
            cancellationToken);

        return ApiResponse<IReadOnlyList<OrganizationHolidayCalendarDTO>>.Success(
            holidays.Select(HolidayCalendarValidation.ToDTO).ToList(),
            "Holidays retrieved successfully.");
    }
}

public sealed class GetHolidayQueryHandler(
    IUnitOfWork unitOfWork,
    ICommonRequestService commonRequestService)
    : IRequestHandler<GetHolidayQuery, ApiResponse<OrganizationHolidayCalendarDTO>>
{
    public async Task<ApiResponse<OrganizationHolidayCalendarDTO>> Handle(
        GetHolidayQuery request,
        CancellationToken cancellationToken)
    {
        var actor = await commonRequestService.ValidateTenantUserRequestAsync();
        var holiday = await unitOfWork.HolidayCalandarRepository.GetTenantHolidayAsync(
            actor.TenantId,
            request.DTO.Id,
            cancellationToken) ?? throw new NotFoundException("Holiday was not found.");

        return ApiResponse<OrganizationHolidayCalendarDTO>.Success(
            HolidayCalendarValidation.ToDTO(holiday),
            "Holiday retrieved successfully.");
    }
}

public sealed class CreateHolidayCommandHandler(
    IUnitOfWork unitOfWork,
    ICommonRequestService commonRequestService)
    : IRequestHandler<CreateHolidayCommand, ApiResponse<OrganizationHolidayCalendarDTO>>
{
    public async Task<ApiResponse<OrganizationHolidayCalendarDTO>> Handle(
        CreateHolidayCommand request,
        CancellationToken cancellationToken)
    {
        var actor = await commonRequestService.ValidateTenantUserRequestAsync();
        var dto = request.DTO;
        HolidayCalendarValidation.Validate(dto.TenantLocationId, dto.HolidayName, dto.HolidayDate, dto.Description);
        if (!await unitOfWork.HolidayCalandarRepository.TenantLocationExistsAsync(
            actor.TenantId, dto.TenantLocationId, cancellationToken))
        {
            throw new ValidationErrorException("Tenant location is not active or does not belong to this tenant.");
        }

        var name = dto.HolidayName.Trim();
        if (await unitOfWork.HolidayCalandarRepository.DuplicateHolidayExistsAsync(
            actor.TenantId, dto.TenantLocationId, dto.HolidayDate, name, null, cancellationToken))
        {
            throw new ValidationErrorException("This holiday already exists for the location and date.");
        }

        var holiday = new OrganizationHolidayCalendar
        {
            TenantId = actor.TenantId,
            TenantLocationId = dto.TenantLocationId,
            HolidayName = name,
            HolidayDate = dto.HolidayDate,
            IsOptional = dto.IsOptional,
            Description = dto.Description?.Trim(),
            IsActive = true,
            IsSoftDeleted = false,
            AddedById = actor.LoggedInEmployeeId,
            AddedDateTime = DateTime.UtcNow
        };

        await unitOfWork.HolidayCalandarRepository.SaveHolidayAsync(holiday, cancellationToken);
        return ApiResponse<OrganizationHolidayCalendarDTO>.Success(
            HolidayCalendarValidation.ToDTO(holiday),
            "Holiday created successfully.");
    }
}

public sealed class UpdateHolidayCommandHandler(
    IUnitOfWork unitOfWork,
    ICommonRequestService commonRequestService)
    : IRequestHandler<UpdateHolidayCommand, ApiResponse<OrganizationHolidayCalendarDTO>>
{
    public async Task<ApiResponse<OrganizationHolidayCalendarDTO>> Handle(
        UpdateHolidayCommand request,
        CancellationToken cancellationToken)
    {
        var actor = await commonRequestService.ValidateTenantUserRequestAsync();
        var dto = request.DTO;
        HolidayCalendarValidation.Validate(dto.TenantLocationId, dto.HolidayName, dto.HolidayDate, dto.Description);
        var holiday = await unitOfWork.HolidayCalandarRepository.GetTenantHolidayAsync(
            actor.TenantId, dto.Id, cancellationToken) ?? throw new NotFoundException("Holiday was not found.");

        if (!await unitOfWork.HolidayCalandarRepository.TenantLocationExistsAsync(
            actor.TenantId, dto.TenantLocationId, cancellationToken))
        {
            throw new ValidationErrorException("Tenant location is not active or does not belong to this tenant.");
        }

        var name = dto.HolidayName.Trim();
        if (await unitOfWork.HolidayCalandarRepository.DuplicateHolidayExistsAsync(
            actor.TenantId, dto.TenantLocationId, dto.HolidayDate, name, dto.Id, cancellationToken))
        {
            throw new ValidationErrorException("This holiday already exists for the location and date.");
        }

        holiday.TenantLocationId = dto.TenantLocationId;
        holiday.HolidayName = name;
        holiday.HolidayDate = dto.HolidayDate;
        holiday.IsOptional = dto.IsOptional;
        holiday.Description = dto.Description?.Trim();
        holiday.UpdatedById = actor.LoggedInEmployeeId;
        holiday.UpdatedDateTime = DateTime.UtcNow;

        await unitOfWork.HolidayCalandarRepository.SaveHolidayAsync(holiday, cancellationToken);
        return ApiResponse<OrganizationHolidayCalendarDTO>.Success(
            HolidayCalendarValidation.ToDTO(holiday),
            "Holiday updated successfully.");
    }
}

public sealed class DeleteHolidayCommandHandler(
    IUnitOfWork unitOfWork,
    ICommonRequestService commonRequestService)
    : IRequestHandler<DeleteHolidayCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(
        DeleteHolidayCommand request,
        CancellationToken cancellationToken)
    {
        var actor = await commonRequestService.ValidateTenantUserRequestAsync();
        var holiday = await unitOfWork.HolidayCalandarRepository.GetTenantHolidayAsync(
            actor.TenantId,
            request.DTO.Id,
            cancellationToken) ?? throw new NotFoundException("Holiday was not found.");

        holiday.IsActive = false;
        holiday.IsSoftDeleted = true;
        holiday.SoftDeletedById = actor.LoggedInEmployeeId;
        holiday.DeletedDateTime = DateTime.UtcNow;
        await unitOfWork.HolidayCalandarRepository.SaveHolidayAsync(holiday, cancellationToken);

        return ApiResponse<bool>.Success(true, "Holiday deleted successfully.");
    }
}

#endregion
