// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the request model for retrieving Contact.
// ================================================================

using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Pagination;

namespace axionpro.application.DTOS.Employee.Contact
{
    /// <summary>
    /// Represents the GetContactRequestDTO data transfer model.
    /// </summary>
    public class GetContactRequestDTO : BaseRequest
    {
          
        public required string UserEmployeeId { get; set; }
        public required string EmployeeId { get; set; }      
    }
}
