// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the request model for creating Identity.
// ================================================================

using axionpro.application.DTOS.Common;
using axionpro.application.DTOS.Pagination;
using axionpro.application.Interfaces.IFileStorage;
using MediatR.NotificationPublishers;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.DTOS.Employee.Sensitive
{


    /// <summary>
    /// Represents the CreateEmployeeIdentityRequestDTO data transfer model.
    /// </summary>
    public class CreateEmployeeIdentityRequestDTO : axionpro.application.DTOs.BaseDTO.PermissionRequestDTO
    {
        public List<CreateIdentityRequest> Identities { get; set; }
            = new List<CreateIdentityRequest>();
    }

    /// <summary>
    /// Represents the CreateIdentityRequest application component.
    /// </summary>
    public class CreateIdentityRequest
    {



        //  Core Identity Info
        public required string UserEmployeeId { get; set; }
        public required string EmployeeId { get; set; }
        public int IdentityCategoryDocumentId { get; set; }
        public required string IdentityValue { get; set; } = string.Empty;
        public required  string DocumnetCode  { get; set; }

        //  Document Info
        public IFormFile? IdentityDocumentFile { get; set; }
        //  Validity
        public DateOnly? EffectiveFrom { get; set; }
        public DateOnly? EffectiveTo { get; set; }

        //  Audit Fields
        // public string? DocumentFileName { get; set; }
        //public string? DocumentFilePath { get; set; }

        //  Verification Info
        //  public bool IsVerified { get; set; } = false;
        //  public long? InfoVerifiedById { get; set; }
        //  public DateTime? InfoVerifiedDateTime { get; set; }

        //  Permissions / Flags
        //  public bool IsEditAllowed { get; set; } = true;
        //  public bool HasIdentityUploaded { get; set; } = false;
        //   public bool IsActive { get; set; } = true;



    }

}


