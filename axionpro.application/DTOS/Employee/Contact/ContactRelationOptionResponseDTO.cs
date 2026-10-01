// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the response contract for Employee contact relation options.
// ================================================================

namespace axionpro.application.DTOS.Employee.Contact;

/// <summary>
/// Represents one stable Employee contact relation option.
/// </summary>
public sealed class ContactRelationOptionResponseDTO
{
    public int Id { get; init; }

    public string Code { get; init; } = string.Empty;

    public string Label { get; init; } = string.Empty;
}
