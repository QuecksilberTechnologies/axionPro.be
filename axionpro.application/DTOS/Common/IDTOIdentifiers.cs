using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; using axionpro.domain.Entity; using MediatR;

namespace axionpro.application.DTOS.Common
{
    public class IDTOIdentifiers
    {
        public string EncriptedEmployeeId { get; set; } = null!;
        public string? EncriptedId { get; set; } = null!;
        public long EmployeeId { get; set; } = 0;
        public int Id { get; set; } = 0;
        public long TenantId { get; set; } = 0;
        public string? encryptedTenantId { get; set; } = null!;
    }
}
