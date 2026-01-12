using axionpro.application.DTOS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.AssetDTO.type
{
    public class UpdateTypeRequestDTO
    {
        public int Id { get; set; }              
        public long? CategoryId { get; set; }
        public string? TypeName { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
        public ExtraPropRequestDTO Prop = new ExtraPropRequestDTO();
    }
    }

