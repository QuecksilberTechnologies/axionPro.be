// ================================================================
// Author  : Deepesh Gupta
// Company : Quecksilber Technologies
// Role    : CEO
// Purpose : Defines the request model for updating Edit Status.
// ================================================================

using System;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.DTOS.Common
{
    public class UpdateEditStatusRequestDTO
    {
        [Required]
        public string UserEmployeeId { get; set; } = default!;

        [Required]
        public string EmployeeId { get; set; } = default!;

        [Required]
        public bool IsEditable { get; set; }
    }

    public class UpdateEditStatusRequestDTO_
    {
        [Required]
        public string UserEmployeeId { get; set; } = default!;

        [Required]
        public string EmployeeId { get; set; } = default!;
        [Required]
        public int TabInfoType { get; set; } = default!;

        [Required]
        public bool IsEditable { get; set; }

    }

}
