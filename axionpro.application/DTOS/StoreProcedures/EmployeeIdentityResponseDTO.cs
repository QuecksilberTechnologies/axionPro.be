using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.application.DTOS.StoreProcedures
{
    public class GetEmployeeIdentityResponseDTO
    {
        /// <summary>
        /// Row identifier (can be NULL for non-filled required docs)
        /// </summary>
        public long? Id { get; set; }

        /// <summary>
        /// Employee context (NULL when identity not yet created)
        /// </summary>
        public string? EmployeeId { get; set; }

        /// <summary>
        /// Country info
        /// </summary>
        public string CountryCode { get; set; } = string.Empty;
        public string CountryName { get; set; } = string.Empty;

        /// <summary>
        /// Identity classification
        /// </summary>
        public string IdentityCategoryName { get; set; } = string.Empty;

        /// <summary>
        /// Document master
        /// </summary>
        public int IdentityCategoryDocumentId { get; set; }
        public string DocumentCode { get; set; } = string.Empty;
        public string DocumentName { get; set; } = string.Empty;
        public string? Description { get; set; }

        /// <summary>
        /// Rule from CountryIdentityRule
        /// </summary>
        public bool IsMandatory { get; set; }

        /// <summary>
        /// EmployeeIdentity mapping
        /// </summary>
        public long? EmployeeIdentityId { get; set; }
        public string? IdentityValue { get; set; }

        /// <summary>
        /// Status flags
        /// </summary>
        public bool? IsVerified { get; set; }
        public bool? IsEditAllowed { get; set; }
        public bool? HasIdentityUploaded { get; set; }

        /// <summary>
        /// Validity
        /// </summary>
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }

        public bool? IsActive { get; set; }
    }

    [Keyless]
    public class GetEmployeeIdentitySp
    {
        public long? Id { get; set; }

        public long? EmployeeId { get; set; }

        public string? CountryCode { get; set; }
        public string? CountryName { get; set; }

        public string? IdentityCategoryName { get; set; }

        public int IdentityCategoryDocumentId { get; set; }
        public string? DocumentCode { get; set; }
        public string? DocumentName { get; set; }
        public string? Description { get; set; }

        public bool IsMandatory { get; set; }

        public long? EmployeeIdentityId { get; set; }
        public string? IdentityValue { get; set; }

        public bool? IsVerified { get; set; }
        public bool? IsEditAllowed { get; set; }
        public bool? HasIdentityUploaded { get; set; }

        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }

        public bool? IsActive { get; set; }
    }

}
