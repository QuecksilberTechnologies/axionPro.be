using axionpro.application.DTOS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.AssetDTO.type
{
    public class DeleteTypeRequestDTO
    {
        public long Id { get; set; }
       
        public ExtraPropRequestDTO Prop = new ExtraPropRequestDTO();

    }
}
