using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.AssetDTO.category
{
    public class GetCategoryResponseDTO : BaseRequest
    {
        

        public string? CategoryName { get; set; }

        public string? Remark { get; set; }

        public bool IsActive { get; set; }

       public long AddedById { get; set; }
         public DateTime AddedDateTime { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdatedDateTime { get; set; }




    }
}
