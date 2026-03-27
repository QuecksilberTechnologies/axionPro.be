using axionpro.application.DTOs.UserRole;

namespace axionpro.application.DTOs.Employee
{
    public class GetEmployeeLoginInfoResponseDTO
    {
        public string? EmployeeId { get; set; }         
        public int RoleTypeId { get; set; }
        public string? RoleTypeName { get; set; }
        public string? TenantName { get; set; }
        public bool? IsPasswordChangeRequired { get; set; }
        public string? DesignationName { get; set; }
        public int DesignationId { get; set; }
        public bool IsOnboard {  get; set; }
        public int DepartmentId { get; set; }
        public int? CountryId { get; set; }
        public string? Nationality { get; set; }
        public string? DepartmentName { get; set; }
        public int EmployeeTypeId  { get; set; }
        public string? OfficialEmail  { get; set; }
        public string? EmployeeFullName { get; set; }
        public UserRoleDTO? UserPrimaryRole { get; set; }
        public List<UserRoleDTO>? UserSecondryRoles { get; set; }

        public string? ProfileImageLink { get; set; } 

        }







}

