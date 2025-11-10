using axionpro.application.DTOS.Pagination;

namespace axionpro.application.DTOS.Employee.Contact
{
    public class GetContactRequestDTO : BaseRequest
    {
          
        public string EmployeeId { get; set; }
        public string? CountryId { get; set; }            // optional filter
        public string? StateId { get; set; }              // optional filter
        public string? DistrictId { get; set; }              // optional filter
        public bool? IsActive { get; set; }            // only active/inactive records
        public bool? IsPrimary { get; set; }           // only primary contacts
        public bool? IsInfoVerified { get; set; }
        public bool? IsEditAllowed { get; set; }
    }
}
