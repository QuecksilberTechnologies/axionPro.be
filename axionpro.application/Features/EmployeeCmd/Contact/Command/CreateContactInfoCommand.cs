using axionpro.application.DTOS.Employee.Bank;
using axionpro.application.DTOS.Employee.BaseEmployee;
using axionpro.application.DTOS.Employee.Contact;
 
using axionpro.application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Features.EmployeeCmd.Contact.Command
{
    public class CreateContactInfoCommand : IRequest<ApiResponse<List<GetContactResponseDTO>>>
    {
        public CreateContactRequestDTO DTO { get; set; }

        public CreateContactInfoCommand(CreateContactRequestDTO dTO)
        {
            DTO = dTO;
        }
    }
}
