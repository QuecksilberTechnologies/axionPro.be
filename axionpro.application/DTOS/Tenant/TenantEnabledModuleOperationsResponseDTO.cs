using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.Tenant
{
    public class TenantEnabledModuleOperationsResponseDTO
    {
       
            long? Id ;
            public long? TenantId { get; set; }                  
            public List<EnabledModuleActiveDTO>? Modules { get; set; }
    }

        public class EnabledModuleActiveDTO
        {
            public int? ParentModuleId { get; set; }
            public string ParentModuleName { get; set; } = string.Empty;
            public int? Id { get; set; }
            public string? ModuleName { get; set; } 
            public bool IsEnabled { get; set; }
            public List<EnabledOperationActiveDTO>? Operations { get; set; }
        
        }

        public class EnabledOperationActiveDTO
        {

            public int Id { get; set; }
            public string? OperationName { get; set; }
             public bool IsEnabled { get; set; }

       }



    }
 
