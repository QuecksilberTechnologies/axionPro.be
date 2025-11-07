using axionpro.application.DTOS.Pagination;

namespace axionpro.application.DTOS.Employee.Contact
{
    public class GetContactRequestDTO : BaseRequest
    {
          
        public string EmployeeId { get; set; }
        public int? CountryId { get; set; }            // optional filter
        public int? StateId { get; set; }              // optional filter
        public int? DistrictId { get; set; }              // optional filter
        public bool? IsActive { get; set; }            // only active/inactive records
        public bool? IsPrimary { get; set; }           // only primary contacts
        public bool? IsInfoVerified { get; set; }
        public bool? IsEditAllowed { get; set; }
    }
}
