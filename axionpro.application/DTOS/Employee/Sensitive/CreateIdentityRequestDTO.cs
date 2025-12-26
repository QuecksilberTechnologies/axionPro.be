using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Pagination;
using axionpro.application.Interfaces.IFileStorage;
using MediatR.NotificationPublishers;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.Employee.Sensitive
{


    public class CreateEmployeeIdentityRequestDTO
    {
        public List<CreateIdentityRequest> Identities { get; set; }
            = new List<CreateIdentityRequest>();
    }

    public class CreateIdentityRequest
    {



        // 🔹 Core Identity Info
        public required string UserEmployeeId { get; set; }
        public required string EmployeeId { get; set; }
        public int IdentityCategoryDocumentId { get; set; }
        public string IdentityValue { get; set; } = string.Empty;

        // 🔹 Document Info
        public IFormFile? IdentityDocumentFile { get; set; }
        // 🔹 Validity
        public DateOnly? EffectiveFrom { get; set; }
        public DateOnly? EffectiveTo { get; set; }
        public ExtraPropRequestDTO Prop { get; set; } = new ExtraPropRequestDTO();

        // 🔹 Audit Fields
        // public string? DocumentFileName { get; set; }
        //public string? DocumentFilePath { get; set; }

        // 🔹 Verification Info
        //  public bool IsVerified { get; set; } = false;
        //  public long? InfoVerifiedById { get; set; }
        //  public DateTime? InfoVerifiedDateTime { get; set; }

        // 🔹 Permissions / Flags
        //  public bool IsEditAllowed { get; set; } = true;
        //  public bool HasIdentityUploaded { get; set; } = false;
        //   public bool IsActive { get; set; } = true;



    }

}


