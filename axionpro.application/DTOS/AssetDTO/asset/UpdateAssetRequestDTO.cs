using axionpro.application.DTOS.Common;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.AssetDTO.asset
{
   

        public class UpdateAssetRequestDTO
        {
            public long Id { get; set; }

            public string? AssetName { get; set; }
            public int? AssetTypeId { get; set; }
            public string? Company { get; set; }
            public string? ModelNo { get; set; }
            public string? Size { get; set; }
            public string? Weight { get; set; }
            public string? Color { get; set; }
            public bool? IsRepairable { get; set; }
            public decimal? Price { get; set; }
            public string? SerialNumber { get; set; }
            public string? Barcode { get; set; }
            public DateTime? PurchaseDate { get; set; }
            public DateTime? WarrantyExpiryDate { get; set; }
            public int? AssetStatusId { get; set; }
            public bool? IsAssigned { get; set; }
            public bool? IsActive { get; set; }

            public IFormFile? AssetImageFile { get; set; }
         

        public ExtraPropRequestDTO? Prop { get; set; } = new ExtraPropRequestDTO();

    }




    }




