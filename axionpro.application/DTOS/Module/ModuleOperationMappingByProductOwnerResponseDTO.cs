using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.Module
{
    public class ModuleOperationMappingByProductOwnerResponseDTO
    {
        public long Id { get; set; }
        public int? ModuleId { get; set; }
       public List<int>? OperationIds { get; set; }
        public string? DisplayName { get; set; }
        public string? PageURL { get; set; }
        public string? IconURL { get; set; }
        public bool IsCommonItem { get; set; }
        public bool IsOperational { get; set; }
        public int Priority { get; set; }
        public string? Remark { get; set; }
        public bool? IsActive { get; set; }
        public long? AddedById { get; set; }
        public DateTime? AddedDateTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedDateTime { get; set; }
    }
}
