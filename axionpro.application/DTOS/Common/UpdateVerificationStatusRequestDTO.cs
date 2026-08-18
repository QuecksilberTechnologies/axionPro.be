// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the request model for updating Verification Status.
// ================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.DTOS.Common
{
    /// <summary>
    /// Represents the UpdateVerificationStatusRequestDTO data transfer model.
    /// </summary>
    public class UpdateVerificationStatusRequestDTO
    {
        [Required]
        public string UserEmployeeId { get; set; } = default!;

        [Required]
        public string EmployeeId { get; set; } = default!;

        [Required]
        public bool IsVerified { get; set; }

    }
    /// <summary>
    /// Represents the UpdateVerificationStatusRequestDTO_ application component.
    /// </summary>
    public class UpdateVerificationStatusRequestDTO_
    {
        [Required]
        public string UserEmployeeId { get; set; } = default!;

        [Required]
        public string EmployeeId { get; set; } = default!;
        [Required]
        public  int TabInfoType { get; set; } = default!;

        [Required]
        public bool IsVerified { get; set; }

    }


}

