using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using axionpro.domain.Entity;
using MediatR;

namespace axionpro.application.DTOS.PolicyTypeDocument
{
    public class CreatePolicyTypeDocumentRequestDTO
    {      
        public int PolicyTypeId { get; set; }
        public string? DocumentTitle { get; set; } = null!;
        public string? FileName { get; set; } = null!;        
        public bool? IsActive { get; set; } = true;

    }
}
