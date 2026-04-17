using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Pagination;
using axionpro.domain.Entity; 
using MediatR;

namespace axionpro.application.DTOS.TicketDTO.TicketType
{
    public class GetTicketTypeRequestDTO:BaseRequest
    {
        public ExtraPropRequestDTO? Prop { get; set; } = new ExtraPropRequestDTO();


    }
}
public class BaseRequest
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public string? SortBy { get; set; }
    public string? SortOrder { get; set; } = "desc";
    public string? UserEmployeeId { get; set; }



}