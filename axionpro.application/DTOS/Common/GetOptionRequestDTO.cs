// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the request model for retrieving Option.
// ================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.DTOS.Common
{
   /// <summary>
   /// Represents the GetOptionRequestDTO data transfer model.
   /// </summary>
   public class GetOptionRequestDTO
    {
        public string? UserEmployeeId { get; set; }  
        public  DateTime? TodaysDate { get; set; }        
      
    }
}
