using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.Module.ParentModule
{
    public class GetSubParentModulRequestDTO
    {
        public int Id { get; set; }
        public string? ModuleName { get; set; }
        public bool IsActive { get; set; }
        public bool IsCommonMenu { get; set; }

    }
}
