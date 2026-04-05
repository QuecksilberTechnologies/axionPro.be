using axionpro.application.DTOS.Common;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.DTOs.PolicyType
{
    public class UpdatePolicyTypeRequestDTO
    {
        public int Id { get; set; }
        public required bool IsActive { get; set; }
        public required bool IsStructured { get; set; }
        public required string PolicyName { get; set; }
        public int PolicyTypeEnumVal { get; set; }

        // 🔥 MULTIPLE EMPLOYEE TYPES
        public required List<int> EmployeeTypeIds { get; set; } = new();

        public IFormFile? FormFile { get; set; }
        public required string Description { get; set; }

        public ExtraPropRequestDTO? Prop { get; set; } = new();
    }
}
