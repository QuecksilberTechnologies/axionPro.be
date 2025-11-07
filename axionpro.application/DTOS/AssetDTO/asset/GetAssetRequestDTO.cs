using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.AssetDTO.asset
{
    public class GetAssetRequestDTO
    {
        public long TenantId { get; set; } //mandatory
        public int RoleId { get; set; }      //mandatory
        public long EmployeeId { get; set; }//mandatory
        public long? AssetId { get; set; } //optional for search by id
        public int? AssetTypeId { get; set; } //optional for search by asset type
        public string? SerialNumber { get; set; } //optional for search by serial number
        public DateTime? PurchasedDateTime { get; set; } //optional for search by purchased date
        public DateTime? InBetweenPurchaseDate { get; set; } //optional for search by end of life date
        public DateTime? WarrantyExpiryDateTime { get; set; } //optional for search by warranty expiry date
        public string? ModelNumber { get; set; } //optional for search by model number
        public int? AssetStatusId { get; set; } //optional for search by asset status
        public bool? IsAssigned { get; set; } //optional for search by assignment status
        public long?  TypeId { get; set; }  //optional for search by type id
        public bool? IsActive { get; set; }     //optional for search by active status

    }
}
