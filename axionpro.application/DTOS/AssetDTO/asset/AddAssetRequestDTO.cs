using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Pagination;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.DTOS.AssetDTO.asset
{


    public class AddAssetRequestDTO 
    {
                          
        public string? AssetName { get; set; }
        public int AssetTypeId { get; set; }
        public string? Company { get; set; }
        public long CategoryId { get; set; } = 0;
        public string? ModelNo { get; set; }
        public string? Size { get; set; }
        public string? Weight { get; set; }
        public string? Color { get; set; }
        public bool IsRepairable { get; set; }
        public decimal Price { get; set; }
        public string? SerialNumber { get; set; }
        public string? Barcode { get; set; }
        // 🔥 ISO SAFE Date Handling
        private DateTime? _purchaseDate;
        public DateTime? PurchaseDate
        {
            get => _purchaseDate;
            set => _purchaseDate = value.HasValue
                ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc)
                : null;
        }

        private DateTime? _warrantyExpiryDate;
        public DateTime? WarrantyExpiryDate
        {
            get => _warrantyExpiryDate;
            set => _warrantyExpiryDate = value.HasValue
                ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc)
                : null;
        }

        public int AssetImageType { get; set; } = 1;       
        public string? AssetImagePath { get; set; }       
        public int AssetStatusId { get; set; }
        public bool IsAssigned { get; set; }
        public bool IsActive { get; set; }
        public IFormFile? AssetImageFile { get; set; }
        public ExtraPropRequestDTO? Prop { get; set; } = new ExtraPropRequestDTO();

    }

}
