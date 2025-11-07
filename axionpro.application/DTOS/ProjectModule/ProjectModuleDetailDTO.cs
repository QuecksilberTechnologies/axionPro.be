using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.ProjectModule
{
    public class ModuleDetailResponseDTO
    {
        public int ModuleId { get; set; }

        public string? ModuleName { get; set; }

        public string? SubModuleURL { get; set; }

        public string? ImageIconWeb { get; set; }

        public string? ImageIconMobile { get; set; }

        public bool? IsModuleDisplayInUI { get; set; }

        public bool? IsActive { get; set; }

        public string? Remark { get; set; }

        public int? ParentModuleId { get; set; }


    }

}

 
