using axionpro.application.DTOS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.DTOS.AssetDTO.status
{
    public class DeleteStatusReqestDTO
    {
          public int  Id { get; set; }
       
          public ExtraPropRequestDTO Prop = new ExtraPropRequestDTO();






    }
}
