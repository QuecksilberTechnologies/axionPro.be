using axionpro.application.DTOs.BaseDTO;

namespace axionpro.application.DTOS.Employee.Type;

/// <summary>Tenant scope is resolved from authentication, never from a query parameter.</summary>
public sealed class GetEmployeeTypeRequestDTO : PermissionRequestDTO
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
