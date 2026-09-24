using axionpro.application.DTOs.Attendance;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Interfaces.IRepositories;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;

namespace axionpro.application.Features.AttendanceCmd;

public sealed record MarkAttendanceCommand(AttendanceRequestDTO Request)
    : IRequest<ApiResponse<AttendancePunchResponseDTO>>;

public sealed record GetTodayAttendanceQuery : IRequest<ApiResponse<AttendanceTodayResponseDTO>>;

/// <summary>Marks attendance only for the employee represented by the validated Tenant token.</summary>
public sealed class MarkAttendanceCommandHandler(IAttendanceRepository repository,
    ICommonRequestService commonRequestService)
    : IRequestHandler<MarkAttendanceCommand, ApiResponse<AttendancePunchResponseDTO>>
{
    public async Task<ApiResponse<AttendancePunchResponseDTO>> Handle(MarkAttendanceCommand command,
        CancellationToken cancellationToken)
    {
        var request = command.Request ?? throw new ValidationErrorException("Attendance request is required.");
        if (!Enum.IsDefined(request.Action) || !Enum.IsDefined(request.Channel)
            || request.Channel is AttendanceChannel.Biometric or AttendanceChannel.Manual
            || request.IdempotencyKey == Guid.Empty)
            throw new ValidationErrorException("Action, Mobile/Web channel, and IdempotencyKey are required.");
        if (request.Latitude.HasValue != request.Longitude.HasValue)
            throw new ValidationErrorException("Latitude and Longitude must be supplied together.");

        var actor = await commonRequestService.ValidateTenantUserRequestAsync();
        if (!actor.Success || actor.TenantId <= 0 || actor.LoggedInEmployeeId <= 0)
            throw new UnauthorizedAccessException(actor.ErrorMessage ?? "Unauthorized request.");

        var result = await repository.MarkAsync(actor.TenantId, actor.LoggedInEmployeeId,
            request, DateTime.UtcNow, cancellationToken);
        return ApiResponse<AttendancePunchResponseDTO>.Success(result, "Attendance punch recorded successfully.");
    }
}

/// <summary>Returns today's immutable punch timeline for the authenticated employee.</summary>
public sealed class GetTodayAttendanceQueryHandler(IAttendanceRepository repository,
    ICommonRequestService commonRequestService)
    : IRequestHandler<GetTodayAttendanceQuery, ApiResponse<AttendanceTodayResponseDTO>>
{
    public async Task<ApiResponse<AttendanceTodayResponseDTO>> Handle(GetTodayAttendanceQuery query,
        CancellationToken cancellationToken)
    {
        var actor = await commonRequestService.ValidateTenantUserRequestAsync();
        if (!actor.Success || actor.TenantId <= 0 || actor.LoggedInEmployeeId <= 0)
            throw new UnauthorizedAccessException(actor.ErrorMessage ?? "Unauthorized request.");

        var result = await repository.GetTodayAsync(actor.TenantId, actor.LoggedInEmployeeId,
            DateTime.UtcNow, cancellationToken);
        return ApiResponse<AttendanceTodayResponseDTO>.Success(result,
            "Today's attendance retrieved successfully.");
    }
}
