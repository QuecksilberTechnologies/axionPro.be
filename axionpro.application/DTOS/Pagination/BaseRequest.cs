using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.Pagination
{
   
        public class BaseRequest
        {
        public int PageNumber { get; set; }
        public int PageSize { get; set; } 
        public string? SortBy { get; set; }
        public string? SortOrder { get; set; } = "desc";
        public string UserEmployeeId { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;

    }

    

}
