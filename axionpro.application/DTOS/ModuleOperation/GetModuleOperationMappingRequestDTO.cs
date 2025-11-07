using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.ModuleOperation
{
    public class GetModuleOperationMappingRequestDTO: BaseRequest
    {
        [Required]
        public int ModuleId { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "At least one operation must be selected.")]
        public List<OperationDTO> Operation { get; set; }

        [Required]
        public int DataViewStructureId { get; set; }

        [Required]
        public int PageTypeId { get; set; }

        [Required]
        [MaxLength(500)]
        public string PageURL { get; set; }

        public string IconURL { get; set; }

        public bool IsCommonItem { get; set; } = false;

        public bool IsOperational { get; set; } = false;

        public int Priority { get; set; } = 0;

        public string? Remark { get; set; }

        public bool IsActive { get; set; } = true;

        [Required]
        public long AddedById { get; set; }
    }

    public class OperationDTO
    {
        [Required]
        public int Id { get; set; }   // OperationId

        public bool IsSelected { get; set; } = false;
    }

}
