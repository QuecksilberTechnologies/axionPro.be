using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.Common.Contexts
{
     
        public class TenantContext
        {
            public long TenantId { get; set; }
            public string TenantEncryptionKey { get; set; } = string.Empty;
        }
     

}
