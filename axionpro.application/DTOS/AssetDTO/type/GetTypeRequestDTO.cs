using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.AssetDTO.type
{
    /// <summary>
    /// Represents the data transfer object used to retrieve or filter Asset Types.
    /// </summary>
    public class GetTypeRequestDTO : BaseRequest
    {
       
      
        /// <summary>
        /// If provided, the data will be filtered by the specified TypeId.
        /// Use this to fetch a specific Asset Type record.
        /// </summary>
        public  int? TypeId { get; set; }

        /// <summary>
        /// If provided, the data will be filtered by the specified CategoryId.
        /// Use this to retrieve all Asset Types within a specific category.
        /// </summary>
        public long? CategoryId { get; set; }

        /// <summary>
        /// Indicates whether to fetch active records.
        /// If true, only active records will be retrieved.
        /// This field is mandatory.
        /// </summary>
        public bool? IsActive { get; set; }

        public ExtraPropRequestDTO Prop { get; set; } = new ExtraPropRequestDTO();
    }

}
