// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Returns the centralized Employee contact relation catalogue.
// ================================================================

using axionpro.application.Common.Enums;
using axionpro.application.Constants;
using axionpro.application.DTOS.Employee.Contact;
using axionpro.application.Wrappers;
using MediatR;

namespace axionpro.application.Features.EmployeeCmd.Contact.Handlers;

#region Query

/// <summary>
/// Represents the token-authenticated request for Employee contact relation options.
/// </summary>
public sealed class GetContactRelationOptionsQuery
    : IRequest<ApiResponse<IReadOnlyList<ContactRelationOptionResponseDTO>>>
{
}

#endregion

#region Handler

/// <summary>
/// Handles Employee contact relation option requests without a database read.
/// </summary>
public sealed class GetContactRelationOptionsQueryHandler
    : IRequestHandler<
        GetContactRelationOptionsQuery,
        ApiResponse<IReadOnlyList<ContactRelationOptionResponseDTO>>>
{
    public Task<ApiResponse<IReadOnlyList<ContactRelationOptionResponseDTO>>> Handle(
        GetContactRelationOptionsQuery request,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<ContactRelationOptionResponseDTO> options =
            Enum.GetValues<EmergencyContactRelation>()
                .OrderBy(relation => (int)relation)
                .Select(relation => new ContactRelationOptionResponseDTO
                {
                    Id = (int)relation,
                    Code = relation.ToString().ToUpperInvariant(),
                    Label = relation.ToString()
                })
                .ToList();

        return Task.FromResult(
            ApiResponse<IReadOnlyList<ContactRelationOptionResponseDTO>>.Success(
                options,
                AppConstants.SuccessMessages.ContactRelationOptionsRetrieved));
    }
}

#endregion
