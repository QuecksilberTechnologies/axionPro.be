// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the request model for updating Classification.
// ================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using axionpro.application.DTOS.Common;
using axionpro.application.DTOs.BaseDTO;
using axionpro.domain.Entity; 
using MediatR;

namespace axionpro.application.DTOS.TicketDTO.Classification
{
    /// <summary>
    /// Represents the UpdateClassificationRequestDTO data transfer model.
    /// </summary>
    public class UpdateClassificationRequestDTO : PermissionRequestDTO
    {
  
    public int  Id { get; set; }
    public string? ClassificationName { get; set; } 
    public string? Description { get; set; }
    public bool? IsActive { get; set; }
   
}
}
