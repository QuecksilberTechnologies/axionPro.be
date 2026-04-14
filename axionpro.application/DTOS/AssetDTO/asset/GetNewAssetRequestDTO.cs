using axionpro.application.DTOS.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace axionpro.application.DTOS.AssetDTO.asset
{
    public class GetNewAssetRequestDTO
    {
        public bool IsUnread { get; set; } = true; // Default to true to fetch only new/unread assets
        public ExtraPropRequestDTO Prop { get; set; } = new ExtraPropRequestDTO(); // Include extra properties if needed
    }
}
