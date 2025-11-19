using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.Employee.CompletionPercentage
{


    public class CompletionSectionDTO
    {
        public string? SectionName { get; set; }   // Bank, Contact, Experience...
        public double? CompletionPercent { get; set; }       
        public bool? IsInfoVerified { get; set; }
        public bool? IsEditAllowed { get; set; }
        public bool IsSectionCreate { get; set; }
    }


}
