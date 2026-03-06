using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.domain.Entity
{
    // Domain/Entities/FileRecord.cs
    public class FileRecord
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string TenantEncoded { get; set; } = string.Empty; // or tenant Enc
        public string EmployeeEncoded { get; set; } = string.Empty;
        public string OriginalFileName { get; set; } = string.Empty;
        public string StoredPath { get; set; } = string.Empty; // path inside bucket e.g. uploads/tenants/T$abc/E$xyz/file.pdf
        public string ContentType { get; set; } = "application/octet-stream";
        public long Size { get; set; }
        public bool IsSensitive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}
