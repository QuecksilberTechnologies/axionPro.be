using axionpro.application.DTOs.BaseDTO;

namespace axionpro.application.DTOS.Employee.Sensitive;

/// <summary>
/// Identifies one persisted employee identity record for soft deletion.
/// </summary>
public sealed class DeleteIdentityRequestDTO : PermissionRequestDTO
{
    public long EmployeeIdentityId { get; set; }
}
