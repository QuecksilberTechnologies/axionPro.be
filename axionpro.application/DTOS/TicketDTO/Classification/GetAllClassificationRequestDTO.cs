// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the request model for retrieving All Classification.
// ================================================================

using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
using axionpro.domain.Entity; 
using MediatR;
using axionpro.application.DTOS.Common;

namespace axionpro.application.DTOS.TicketDTO.Classification
{
    /// <summary>
    /// Represents the GetAllClassificationRequestDTO data transfer model.
    /// </summary>
    public class GetAllClassificationRequestDTO : BaseRequest
    {

  



    }
    /// <summary>
    /// Represents the DDLClassificationRequestDTO data transfer model.
    /// </summary>
    public class DDLClassificationRequestDTO 
    {
    public required bool  IsActive { get; set; } = true;
    }
}
