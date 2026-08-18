// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the request model for retrieving Identity.
// ================================================================

using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Pagination;
using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.DTOS.Employee.Sensitive
{
    /// <summary>
    /// Represents the GetIdentityRequestDTO data transfer model.
    /// </summary>
    public class GetIdentityRequestDTO 
    {
             
      
        public required  int CountryNationalityId { get; set; }
    }
}
