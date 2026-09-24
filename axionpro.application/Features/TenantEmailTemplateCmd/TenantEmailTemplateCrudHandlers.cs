using System.Net.Mail;
using AutoMapper;
using axionpro.application.Constants;
using axionpro.application.DTOs.BaseDTO;
using axionpro.application.DTOs.EmailTemplate;
using axionpro.application.Exceptions;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.ICommonRequest;
using axionpro.application.Wrappers;
using axionpro.domain.Entity;
using MediatR;

namespace axionpro.application.Features.TenantEmailTemplateCmd;

#region Requests

public sealed record CreateTenantEmailTemplateCommand(CreateTenantEmailTemplateRequestDTO? DTO)
    : IRequest<ApiResponse<TenantEmailTemplateResponseDTO>>, ITenantEmailTemplatePermissionRequest
{
    public PermissionRequestDTO? PermissionRequest => DTO?.PermissionRequest;
}

public sealed record UpdateTenantEmailTemplateCommand(UpdateTenantEmailTemplateRequestDTO? DTO)
    : IRequest<ApiResponse<TenantEmailTemplateResponseDTO>>, ITenantEmailTemplatePermissionRequest
{
    public PermissionRequestDTO? PermissionRequest => DTO?.PermissionRequest;
}

public sealed record UpdateTenantEmailTemplateStatusCommand(UpdateTenantEmailTemplateStatusRequestDTO? DTO)
    : IRequest<ApiResponse<TenantEmailTemplateResponseDTO>>, ITenantEmailTemplatePermissionRequest
{
    public PermissionRequestDTO? PermissionRequest => DTO?.PermissionRequest;
}

public sealed record SyncTenantEmailTemplatesCommand(SyncTenantEmailTemplatesRequestDTO? DTO)
    : IRequest<ApiResponse<SyncTenantEmailTemplatesResponseDTO>>, ITenantEmailTemplatePermissionRequest
{
    public PermissionRequestDTO? PermissionRequest => DTO?.PermissionRequest;
}

public sealed record DeleteTenantEmailTemplateCommand(int Id, PermissionRequestDTO? PermissionRequest)
    : IRequest<ApiResponse<bool>>, ITenantEmailTemplatePermissionRequest;

public sealed record GetTenantEmailTemplateByIdQuery(int Id, PermissionRequestDTO? PermissionRequest)
    : IRequest<ApiResponse<TenantEmailTemplateResponseDTO>>, ITenantEmailTemplatePermissionRequest;

public sealed record GetAllTenantEmailTemplatesQuery(TenantEmailTemplateListRequestDTO Filter)
    : IRequest<ApiResponse<List<TenantEmailTemplateResponseDTO>>>, ITenantEmailTemplatePermissionRequest
{
    public PermissionRequestDTO? PermissionRequest => Filter;
}

#endregion

#region Handlers

public sealed class CreateTenantEmailTemplateCommandHandler(IUnitOfWork unitOfWork, ICommonRequestService commonRequestService, IMapper mapper)
    : IRequestHandler<CreateTenantEmailTemplateCommand, ApiResponse<TenantEmailTemplateResponseDTO>>
{
    public async Task<ApiResponse<TenantEmailTemplateResponseDTO>> Handle(CreateTenantEmailTemplateCommand request, CancellationToken cancellationToken)
    {
        var dto = request.DTO ?? throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
        var actor = await TenantEmailTemplateActor.GetAsync(commonRequestService);
        var input = TenantEmailTemplateInput.From(dto);
        if (await unitOfWork.TenantEmailTemplateRepository.TemplateCodeExistsAsync(actor.TenantId, input.TemplateCode, cancellationToken: cancellationToken))
        {
            throw new ConflictException(AppConstants.ErrorMessages.DuplicateEmailTemplateCode);
        }
        var entity = input.ToEntity(actor.TenantId, actor.EmployeeId);
        await unitOfWork.TenantEmailTemplateRepository.AddAsync(entity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ApiResponse<TenantEmailTemplateResponseDTO>.Success(mapper.Map<TenantEmailTemplateResponseDTO>(entity), AppConstants.SuccessMessages.EmailTemplateCreated);
    }
}

public sealed class UpdateTenantEmailTemplateCommandHandler(IUnitOfWork unitOfWork, ICommonRequestService commonRequestService, IMapper mapper)
    : IRequestHandler<UpdateTenantEmailTemplateCommand, ApiResponse<TenantEmailTemplateResponseDTO>>
{
    public async Task<ApiResponse<TenantEmailTemplateResponseDTO>> Handle(UpdateTenantEmailTemplateCommand request, CancellationToken cancellationToken)
    {
        var dto = request.DTO ?? throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
        if (dto.Id <= 0) throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier);
        var actor = await TenantEmailTemplateActor.GetAsync(commonRequestService);
        var entity = await unitOfWork.TenantEmailTemplateRepository.GetForUpdateAsync(actor.TenantId, dto.Id, cancellationToken)
            ?? throw new NotFoundException(AppConstants.ErrorMessages.EmailTemplateNotFound);
        var input = TenantEmailTemplateInput.From(dto);
        if (await unitOfWork.TenantEmailTemplateRepository.TemplateCodeExistsAsync(actor.TenantId, input.TemplateCode, entity.Id, cancellationToken))
        {
            throw new ConflictException(AppConstants.ErrorMessages.DuplicateEmailTemplateCode);
        }
        input.ApplyTo(entity, actor.EmployeeId);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ApiResponse<TenantEmailTemplateResponseDTO>.Success(mapper.Map<TenantEmailTemplateResponseDTO>(entity), AppConstants.SuccessMessages.EmailTemplateUpdated);
    }
}

public sealed class UpdateTenantEmailTemplateStatusCommandHandler(IUnitOfWork unitOfWork, ICommonRequestService commonRequestService, IMapper mapper)
    : IRequestHandler<UpdateTenantEmailTemplateStatusCommand, ApiResponse<TenantEmailTemplateResponseDTO>>
{
    public async Task<ApiResponse<TenantEmailTemplateResponseDTO>> Handle(UpdateTenantEmailTemplateStatusCommand request, CancellationToken cancellationToken)
    {
        var dto = request.DTO ?? throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
        if (dto.Id <= 0) throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier);
        var actor = await TenantEmailTemplateActor.GetAsync(commonRequestService);
        var entity = await unitOfWork.TenantEmailTemplateRepository.GetForUpdateAsync(actor.TenantId, dto.Id, cancellationToken)
            ?? throw new NotFoundException(AppConstants.ErrorMessages.EmailTemplateNotFound);
        entity.IsActive = dto.IsActive;
        entity.UpdatedById = actor.EmployeeId;
        entity.UpdatedDateTime = DateTime.UtcNow;
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ApiResponse<TenantEmailTemplateResponseDTO>.Success(mapper.Map<TenantEmailTemplateResponseDTO>(entity), AppConstants.SuccessMessages.EmailTemplateStatusUpdated);
    }
}

public sealed class DeleteTenantEmailTemplateCommandHandler(IUnitOfWork unitOfWork, ICommonRequestService commonRequestService)
    : IRequestHandler<DeleteTenantEmailTemplateCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteTenantEmailTemplateCommand request, CancellationToken cancellationToken)
    {
        if (request.Id <= 0) throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier);
        var actor = await TenantEmailTemplateActor.GetAsync(commonRequestService);
        var entity = await unitOfWork.TenantEmailTemplateRepository.GetForUpdateAsync(actor.TenantId, request.Id, cancellationToken)
            ?? throw new NotFoundException(AppConstants.ErrorMessages.EmailTemplateNotFound);
        if (entity.IsActive) throw new ConflictException(AppConstants.ErrorMessages.EmailTemplateMustBeInactiveToDelete);
        unitOfWork.TenantEmailTemplateRepository.Remove(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ApiResponse<bool>.Success(true, AppConstants.SuccessMessages.EmailTemplateDeleted);
    }
}

public sealed class SyncTenantEmailTemplatesCommandHandler(
    IUnitOfWork unitOfWork,
    ICommonRequestService commonRequestService)
    : IRequestHandler<SyncTenantEmailTemplatesCommand, ApiResponse<SyncTenantEmailTemplatesResponseDTO>>
{
    public async Task<ApiResponse<SyncTenantEmailTemplatesResponseDTO>> Handle(
        SyncTenantEmailTemplatesCommand request,
        CancellationToken cancellationToken)
    {
        _ = request.DTO ?? throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
        var actor = await TenantEmailTemplateActor.GetAsync(commonRequestService);
        var defaultTemplates = await unitOfWork.EmailTemplateRepository.GetActiveTemplatesAsync(cancellationToken);
        var existingCodes = await unitOfWork.TenantEmailTemplateRepository.GetTemplateCodesAsync(
            actor.TenantId,
            cancellationToken);
        var existingCodeSet = existingCodes
            .Where(code => !string.IsNullOrWhiteSpace(code))
            .Select(code => code.Trim())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var missingTemplates = defaultTemplates
            .Where(template =>
                ConstantValues.IsSupportedEmailTemplateCode(template.TemplateCode) &&
                !existingCodeSet.Contains(template.TemplateCode!.Trim()))
            .GroupBy(template => template.TemplateCode!.Trim(), StringComparer.OrdinalIgnoreCase)
            .Select(group => group.OrderByDescending(template => template.AddedDateTime)
                .ThenByDescending(template => template.Id)
                .First())
            .Select(template => CopyDefaultTemplate(template, actor.TenantId, actor.EmployeeId))
            .ToList();

        if (missingTemplates.Count > 0)
        {
            await unitOfWork.TenantEmailTemplateRepository.AddRangeAsync(missingTemplates, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        var response = new SyncTenantEmailTemplatesResponseDTO
        {
            ActiveDefaultTemplateCount = defaultTemplates.Count,
            ExistingTenantTemplateCount = existingCodeSet.Count,
            InsertedTemplateCount = missingTemplates.Count,
            InsertedTemplateCodes = missingTemplates
                .Select(template => template.TemplateCode!)
                .OrderBy(code => code)
                .ToList()
        };

        return ApiResponse<SyncTenantEmailTemplatesResponseDTO>.Success(
            response,
            AppConstants.SuccessMessages.TenantEmailTemplatesSynchronized);
    }

    private static TenantEmailTemplate CopyDefaultTemplate(
        EmailTemplate source,
        long tenantId,
        long employeeId) => new()
    {
        TenantId = tenantId,
        TemplateName = source.TemplateName,
        TemplateCode = source.TemplateCode!.Trim().ToUpperInvariant(),
        Subject = source.Subject,
        Body = source.Body,
        FromEmail = source.FromEmail,
        FromName = source.FromName,
        CcEmail = source.CcEmail,
        BccEmail = source.BccEmail,
        Category = source.Category,
        LanguageCode = source.LanguageCode,
        IsActive = true,
        AddedById = employeeId,
        AddedDateTime = DateTime.UtcNow
    };
}

public sealed class GetTenantEmailTemplateByIdQueryHandler(IUnitOfWork unitOfWork, ICommonRequestService commonRequestService, IMapper mapper)
    : IRequestHandler<GetTenantEmailTemplateByIdQuery, ApiResponse<TenantEmailTemplateResponseDTO>>
{
    public async Task<ApiResponse<TenantEmailTemplateResponseDTO>> Handle(GetTenantEmailTemplateByIdQuery request, CancellationToken cancellationToken)
    {
        if (request.Id <= 0) throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidIdentifier);
        var actor = await TenantEmailTemplateActor.GetAsync(commonRequestService);
        var entity = await unitOfWork.TenantEmailTemplateRepository.GetByIdAsync(actor.TenantId, request.Id, cancellationToken)
            ?? throw new NotFoundException(AppConstants.ErrorMessages.EmailTemplateNotFound);
        return ApiResponse<TenantEmailTemplateResponseDTO>.Success(mapper.Map<TenantEmailTemplateResponseDTO>(entity), AppConstants.SuccessMessages.EmailTemplateRetrieved);
    }
}

public sealed class GetAllTenantEmailTemplatesQueryHandler(IUnitOfWork unitOfWork, ICommonRequestService commonRequestService, IMapper mapper)
    : IRequestHandler<GetAllTenantEmailTemplatesQuery, ApiResponse<List<TenantEmailTemplateResponseDTO>>>
{
    public async Task<ApiResponse<List<TenantEmailTemplateResponseDTO>>> Handle(GetAllTenantEmailTemplatesQuery request, CancellationToken cancellationToken)
    {
        var actor = await TenantEmailTemplateActor.GetAsync(commonRequestService);
        var page = await unitOfWork.TenantEmailTemplateRepository.GetPagedAsync(actor.TenantId, request.Filter, cancellationToken);
        return ApiResponse<List<TenantEmailTemplateResponseDTO>>.SuccessPaginated(mapper.Map<List<TenantEmailTemplateResponseDTO>>(page.Data),
            page.PageNumber, page.PageSize, page.TotalCount, page.TotalPages, AppConstants.SuccessMessages.EmailTemplateRetrieved);
    }
}

#endregion

internal static class TenantEmailTemplateActor
{
    public static async Task<(long TenantId, long EmployeeId)> GetAsync(ICommonRequestService service)
    {
        var actor = await service.ValidateTenantUserRequestAsync();
        if (!actor.Success || actor.TenantId <= 0 || actor.LoggedInEmployeeId <= 0)
        {
            throw new UnauthorizedAccessException(actor.ErrorMessage ?? AppConstants.ErrorMessages.Unauthorized);
        }
        return (actor.TenantId, actor.LoggedInEmployeeId);
    }
}

internal sealed record TenantEmailTemplateInput(string TemplateName, string TemplateCode, string Subject, string Body,
    string? FromEmail, string? FromName, string? CcEmail, string? BccEmail, string? Category, string? LanguageCode, bool IsActive)
{
    public static TenantEmailTemplateInput From(CreateTenantEmailTemplateRequestDTO dto) => Create(dto.TemplateName, dto.TemplateCode, dto.Subject, dto.Body,
        dto.FromEmail, dto.FromName, dto.CcEmail, dto.BccEmail, dto.Category, dto.LanguageCode, dto.IsActive);

    public TenantEmailTemplate ToEntity(long tenantId, long actorId) => new()
    {
        TenantId = tenantId, TemplateName = TemplateName, TemplateCode = TemplateCode, Subject = Subject, Body = Body,
        FromEmail = FromEmail, FromName = FromName, CcEmail = CcEmail, BccEmail = BccEmail, Category = Category,
        LanguageCode = LanguageCode, IsActive = IsActive, AddedById = actorId, AddedDateTime = DateTime.UtcNow
    };

    public void ApplyTo(TenantEmailTemplate entity, long actorId)
    {
        entity.TemplateName = TemplateName;
        entity.TemplateCode = TemplateCode;
        entity.Subject = Subject;
        entity.Body = Body;
        entity.FromEmail = FromEmail;
        entity.FromName = FromName;
        entity.CcEmail = CcEmail;
        entity.BccEmail = BccEmail;
        entity.Category = Category;
        entity.LanguageCode = LanguageCode;
        entity.IsActive = IsActive;
        entity.UpdatedById = actorId;
        entity.UpdatedDateTime = DateTime.UtcNow;
    }

    private static TenantEmailTemplateInput Create(string? name, string? code, string? subject, string? body,
        string? fromEmail, string? fromName, string? cc, string? bcc, string? category, string? language, bool active)
    {
        var normalizedCode = Required(code, 100).ToUpperInvariant();
        if (!ConstantValues.IsSupportedEmailTemplateCode(normalizedCode))
        {
            throw new ValidationErrorException(AppConstants.ErrorMessages.UnsupportedEmailTemplateCode);
        }
        return new(Required(name, 150), normalizedCode, Required(subject, 250), Required(body), Email(fromEmail, 150),
            Optional(fromName, 100), EmailList(cc, 1000), EmailList(bcc, 1000), Optional(category, 100), Optional(language, 10), active);
    }

    private static string Required(string? value, int max = int.MaxValue)
    {
        var result = value?.Trim();
        if (string.IsNullOrWhiteSpace(result) || result.Length > max)
        {
            throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
        }
        return result;
    }
    private static string? Optional(string? value, int max)
    {
        var result = value?.Trim();
        if (string.IsNullOrWhiteSpace(result))
        {
            return null;
        }
        if (result.Length > max)
        {
            throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
        }
        return result;
    }
    private static string? Email(string? value, int max)
    {
        var result = Optional(value, max);
        if (result is not null && !ValidEmail(result))
        {
            throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
        }
        return result;
    }
    private static string? EmailList(string? value, int max)
    {
        var result = Optional(value, max);
        if (result is null)
        {
            return null;
        }
        var values = result.Split([';', ','], StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (values.Length == 0 || values.Any(x => !ValidEmail(x)))
        {
            throw new ValidationErrorException(AppConstants.ErrorMessages.InvalidRequest);
        }
        return string.Join(';', values);
    }
    private static bool ValidEmail(string value)
    {
        try
        {
            return string.Equals(new MailAddress(value).Address, value, StringComparison.OrdinalIgnoreCase);
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
