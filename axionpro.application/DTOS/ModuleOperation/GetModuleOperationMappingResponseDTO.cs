using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.ModuleOperation
{
    public class GetModuleOperationMappingResponseDTO: BaseRequest
    {
        public int ModuleId { get; set; }
        public List<int> OperationIds { get; set; } = new();
        public string? Message { get; set; }
        public bool Success { get; set; }
    }

}
