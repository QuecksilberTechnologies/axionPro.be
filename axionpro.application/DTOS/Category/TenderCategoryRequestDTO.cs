using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOs.Category
{
    public class TenderCategoryRequestDTO : BaseRequest
    {
        public long Id { get; set; }
        public int CategoryId { get; set; }


    }
}
