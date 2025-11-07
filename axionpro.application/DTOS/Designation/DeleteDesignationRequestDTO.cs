using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.Designation
{
    public class DeleteDesignationRequestDTO
    {

        public required string Id { get; set; }      
        public string? UserEmployeeId { get; set; }      
       
 
        
    }
}
