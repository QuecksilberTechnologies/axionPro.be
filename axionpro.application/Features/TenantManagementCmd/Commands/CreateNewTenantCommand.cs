// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Dispatches Host-side Tenant onboarding through the established transactional creation flow.
// ================================================================

using axionpro.application.DTOs.Registration;
using axionpro.application.DTOs.Tenant;
using axionpro.application.Features.RegistrationCmd.Handlers;
using axionpro.application.Wrappers;
using MediatR;

namespace axionpro.application.Features.TenantManagementCmd.Commands;

/// <summary>Represents the Host-side request to onboard a new Tenant.</summary>
public sealed record CreateNewTenantCommand(NewTenantCreationRequestDTO RequestDTO)
    : IRequest<ApiResponse<TenantCreateResponseDTO>>;

/// <summary>Routes Host-side onboarding to the established transactional Tenant creation handler.</summary>
public sealed class CreateNewTenantCommandHandler
    : IRequestHandler<CreateNewTenantCommand, ApiResponse<TenantCreateResponseDTO>>
{
    private readonly ISender _sender;

    public CreateNewTenantCommandHandler(ISender sender) => _sender = sender;

    public Task<ApiResponse<TenantCreateResponseDTO>> Handle(
        CreateNewTenantCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.RequestDTO;
        var pattern = dto.EmployeeCodePattern;
        var legacyRequest = new NewTenantCreationBridgeRequestDTO
        {
            ModuleId = dto.ModuleId,
            OperationId = dto.OperationId,
            SubscriptionPlanId = dto.SubscriptionPlanId,
            TenantIndustryId = dto.TenantIndustryId,
            CompanyName = dto.CompanyName,
            TenantCode = dto.TenantCode,
            CompanyEmailDomain = dto.CompanyEmailDomain,
            GenderId = dto.GenderId,
            TenantEmail = dto.TenantEmail,
            ContactPersonName = dto.ContactPersonName,
            ContactNumber = dto.ContactNumber,
            CountryId = dto.CountryId,
            Prefix = pattern.Prefix,
            IncludeYear = pattern.IncludeYear,
            IncludeMonth = pattern.IncludeMonth,
            IncludeDepartment = pattern.IncludeDepartment,
            Separator = pattern.Separator,
            RunningNumberLength = pattern.RunningNumberLength,
            Profile = dto.Profile,
            InitialLocation = dto.InitialLocation,
            EmailConfiguration = dto.EmailConfiguration
        };

        return _sender.Send(new CreateTenantCommand(legacyRequest), cancellationToken);
    }
}

internal sealed class NewTenantCreationBridgeRequestDTO : TenantCreateRequestDTO, INewTenantOnboardingConfiguration
{
    public NewTenantProfileRequestDTO Profile { get; set; } = new();
    public NewTenantLocationRequestDTO InitialLocation { get; set; } = new();
    public NewTenantEmailConfigurationRequestDTO? EmailConfiguration { get; set; }
}
